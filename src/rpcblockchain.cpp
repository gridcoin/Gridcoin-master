// Copyright (c) 2010 Satoshi Nakamoto
// Copyright (c) 2009-2012 The Bitcoin developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

#include "main.h"
#include "bitcoinrpc.h"

#include <boost/lexical_cast.hpp>

using namespace json_spirit;
using namespace std;

void ScriptPubKeyToJSON(const CScript& scriptPubKey, Object& out);


extern std::string SendMultiProngedTransaction(int projectid, std::string userid);

extern std::map<string,MiningEntry> BlockToCPUMinerPayments(const CBlock& block, const CBlockIndex* blockindex);

std::string TxToString(const CTransaction& tx, const uint256 hashBlock, int64& out_amount, int64& out_locktime, int64& out_projectid, std::string& out_projectaddress, std::string& comments, std::string& out_grcaddress);


double TxPaidToCPUMiner(const CTransaction& tx, int nBlock, std::string address, double& out_total, std::string& out_comments);




double GetDifficulty(const CBlockIndex* blockindex)
{
    // Floating point number that is a multiple of the minimum difficulty,
    // minimum difficulty = 1.0.
    if (blockindex == NULL)
    {
        if (pindexBest == NULL)
            return 1.0;
        else
            blockindex = pindexBest;
    }

    int nShift = (blockindex->nBits >> 24) & 0xff;

    double dDiff =
        (double)0x0000ffff / (double)(blockindex->nBits & 0x00ffffff);

    while (nShift < 29)
    {
        dDiff *= 256.0;
        nShift++;
    }
    while (nShift > 29)
    {
        dDiff /= 256.0;
        nShift--;
    }

    return dDiff;
}


Object blockToJSON(const CBlock& block, const CBlockIndex* blockindex)
{
    Object result;
    result.push_back(Pair("hash", block.GetHash().GetHex()));
    CMerkleTx txGen(block.vtx[0]);
    txGen.SetMerkleBranch(&block);
    result.push_back(Pair("confirmations", (int)txGen.GetDepthInMainChain()));
    result.push_back(Pair("size", (int)::GetSerializeSize(block, SER_NETWORK, PROTOCOL_VERSION)));
    result.push_back(Pair("height", blockindex->nHeight));
    result.push_back(Pair("version", block.nVersion));
    result.push_back(Pair("merkleroot", block.hashMerkleRoot.GetHex()));
    Array txs;
    BOOST_FOREACH(const CTransaction&tx, block.vtx)
        txs.push_back(tx.GetHash().GetHex());
    result.push_back(Pair("tx", txs));
    result.push_back(Pair("time", (boost::int64_t)block.GetBlockTime()));
    result.push_back(Pair("nonce", (boost::uint64_t)block.nNonce));
    result.push_back(Pair("bits", HexBits(block.nBits)));
    result.push_back(Pair("difficulty", GetDifficulty(blockindex)));
	result.push_back(Pair("boinchash", block.hashBoinc));

    if (blockindex->pprev)
        result.push_back(Pair("previousblockhash", blockindex->pprev->GetBlockHash().GetHex()));
    if (blockindex->pnext)
        result.push_back(Pair("nextblockhash", blockindex->pnext->GetBlockHash().GetHex()));
    return result;
}


std::string BoincProjectAddress(int projectid) 
{
   if (projectid == 1) return "FufdrPKUXNMrSFNMky6u18F6r6rwUmnAbb";
   if (projectid == 2) return "G3HA4ouWr1zbhKd7jUU5ZRqR8sPsYdYmYb"; //http://www.rnaworld.de/rnaworld
   if (projectid == 3) return "G1dUDVaFG8HmxuzRyCHhqU5k7eQied1nWx"; //http://boinc.bakerlab.org/rosetta
   if (projectid == 4) return "G6RdibWpbYQgvvcTThx6NG2vHdBK1a51eE"; //http://docking.cis.udel.edu
   if (projectid == 5) return "FvDfoheNe74JcUp6uf3N8cPeU4KeUsxPq7"; //http://milkyway.cs.rpi.edu/milkyway
   return "";
}

int BoincProjectId(std::string grc) 
{
   if (grc=="FufdrPKUXNMrSFNMky6u18F6r6rwUmnAbb") return 1; //http://www.malariacontrol.net
   if (grc=="G3HA4ouWr1zbhKd7jUU5ZRqR8sPsYdYmYb") return 2; //http://www.rnaworld.de/rnaworld
   if (grc=="G1dUDVaFG8HmxuzRyCHhqU5k7eQied1nWx") return 3; //http://boinc.bakerlab.org/rosetta
   if (grc=="G6RdibWpbYQgvvcTThx6NG2vHdBK1a51eE") return 4; //http://docking.cis.udel.edu
   if (grc=="FvDfoheNe74JcUp6uf3N8cPeU4KeUsxPq7") return 5; //http://milkyway.cs.rpi.edu/milkyway
   return 0;
}


std::map<string,MiningEntry> BlockToCPUMinerPayments(const CBlock& block, const CBlockIndex* blockindex)
{
   // result.push_back(Pair("hash", block.GetHash().GetHex()));
    CMerkleTx txGen(block.vtx[0]);
    txGen.SetMerkleBranch(&block);
    //result.push_back(Pair("size", (int)::GetSerializeSize(block, SER_NETWORK, PROTOCOL_VERSION)));
    //result.push_back(Pair("version", block.nVersion));
    //result.push_back(Pair("merkleroot", block.hashMerkleRoot.GetHex()));
	std::string grcout = "";
	std::string grc = "";
	int64 project_amount = 0;
	int64 project_locktime = 0;
	int64 projectid = 0;
	std::string projectaddress = "";
	std::string strAccount ="";
    BOOST_FOREACH(const CTransaction&tx, block.vtx)
	{
	  std::string txid = tx.GetHash().GetHex().c_str();
	  projectaddress="";
	  projectid=0;
	  project_amount = 0;
	  project_locktime=0;
	  double out_total=0;
	  strAccount="";
	  std::string comments = "";
	  std::string cpucomments = "";
	  std::string out_grc_address = "";
      grc = TxToString(tx, 0, project_amount, project_locktime, projectid, projectaddress, comments, out_grc_address);
	  double blocktotal = 0;

	  blocktotal = TxPaidToCPUMiner(tx, blockindex->nHeight, "", out_total, cpucomments);
				
	
	  if (grc.length() > 20 && projectid > 0 && project_amount > 0 && projectaddress.length() > 20) {
			strAccount = RoundToString(projectid,0) + out_grc_address;
  	    	MiningEntry me = cpuminerpayments[strAccount];

			if (!me.paid) 
			{
				cpuminerpayments.insert(map<string,MiningEntry>::value_type(strAccount,me));
				me.paid = true;
				cpuminerpayments[strAccount]=me;
	 		}
     	    me.strAccount = out_grc_address;
    	    me.projectid = projectid;
		    me.locktime = blockindex->GetBlockTime();
		
			std::string account1 = RoundToString(project_amount,0);
			me.projectuserid = account1;
     		me.transactionid = txid;
	    	me.blocknumber = blockindex->nHeight;
	 		me.projectaddress = projectaddress;
			me.strComment = comments + ":"+cpucomments;
			me.homogenizedkey = strAccount;
	  	    //printf("Logging cpuminer payment for %s",grc.c_str());
	    	cpuminerpayments[strAccount]=me;
			//Add this item to the CpuPoW check map
			MiningEntry cpume = cpupow[strAccount];
					
			if (!cpume.paid) 
			{
					cpume.strAccount = me.strAccount;
					cpume.projectid = projectid;
					cpume.locktime = me.locktime;
					cpume.projectuserid = account1;
					cpume.transactionid = txid;
					cpume.blocknumber = me.blocknumber;
					cpume.projectaddress = projectaddress;
					cpume.homogenizedkey=strAccount;
					cpupow.insert(map<string,MiningEntry>::value_type(strAccount,cpume));
					cpume.paid=true;
					cpupow[strAccount] = cpume;
			}
		

	  }
	  
	}
    
    return cpuminerpayments;
}






Value getblockcount(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 0)
        throw runtime_error(
            "getblockcount\n"
            "Returns the number of blocks in the longest block chain.");

    return nBestHeight;
}


Value getdifficulty(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 0)
        throw runtime_error(
            "getdifficulty\n"
            "Returns the proof-of-work difficulty as a multiple of the minimum difficulty.");

    return GetDifficulty();
}


Value settxfee(const Array& params, bool fHelp)
{
    if (fHelp || params.size() < 1 || params.size() > 1)
        throw runtime_error(
            "settxfee <amount>\n"
            "<amount> is a real and is rounded to the nearest 0.00000001");

    // Amount
    int64 nAmount = 0;
    if (params[0].get_real() != 0.0)
        nAmount = AmountFromValue(params[0]);        // rejects 0.0 amounts

    nTransactionFee = nAmount;
    return true;
}

Value getrawmempool(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 0)
        throw runtime_error(
            "getrawmempool\n"
            "Returns all transaction ids in memory pool.");

    vector<uint256> vtxid;
    mempool.queryHashes(vtxid);

    Array a;
    BOOST_FOREACH(const uint256& hash, vtxid)
        a.push_back(hash.ToString());

    return a;
}

Value getblockhash(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 1)
        throw runtime_error(
            "getblockhash <index>\n"
            "Returns hash of block in best-block-chain at <index>.");

    int nHeight = params[0].get_int();
    if (nHeight < 0 || nHeight > nBestHeight)
        throw runtime_error("Block number out of range.");

    CBlockIndex* pblockindex = FindBlockByHeight(nHeight);
    return pblockindex->phashBlock->GetHex();
}


Value getblockbyhash(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 1)
        throw runtime_error(
            "getblockbyhash <hash>\n"
            "Returns details of a block with given block-hash.");

    std::string strHash = params[0].get_str();
    uint256 hash(strHash);

    if (mapBlockIndex.count(hash) == 0)
        throw JSONRPCError(RPC_INVALID_ADDRESS_OR_KEY, "Block not found");

    CBlock block;
    CBlockIndex* pblockindex = mapBlockIndex[hash];
    block.ReadFromDisk(pblockindex);

    return blockToJSON(block, pblockindex);
}


//MainGetBlock 

Value getblock(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 1)
        throw runtime_error(
            "getblock <hash>\n"
            "Returns details of a block with given block-hash.");

    int nHeight = params[0].get_int();
    if (nHeight < 0 || nHeight > nBestHeight)
        throw runtime_error("Block number out of range.");

    CBlockIndex* pblockindex = FindBlockByHeight(nHeight);
	CBlock block;


	printf("preparing cpulist");

    block.ReadFromDisk(pblockindex);
    
	Object e= blockToJSON(block, pblockindex);
	cpuminerpayments.clear();

    cpuminerpayments = BlockToCPUMinerPayments(block,pblockindex);

	int inum=0;
				e.push_back(Pair("Report Version",1.7));
	
	for(map<string,MiningEntry>::iterator ii=cpuminerpayments.begin(); ii!=cpuminerpayments.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpayments[(*ii).first];

	        if (ae.strAccount.length() > 5) 
			{
				inum++;
				e.push_back(Pair("Payment #",inum));
				e.push_back(Pair("Payment Comment", ae.strComment));
				e.push_back(Pair("Project UserID", ae.projectuserid));
				e.push_back(Pair("Address", ae.strAccount));
				e.push_back(Pair("Project ID", ae.projectid));
				e.push_back(Pair("Project Address", ae.projectaddress));
				e.push_back(Pair("Tx Id", ae.transactionid));
				//printf("Writing cpumining entry # %d",inum);

			}
	}



	return e;

}

Value gettxoutsetinfo(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 0)
        throw runtime_error(
            "gettxoutsetinfo\n"
            "Returns statistics about the unspent transaction output set.");

    Object ret;

    CCoinsStats stats;
    if (pcoinsTip->GetStats(stats)) {
        ret.push_back(Pair("height", (boost::int64_t)stats.nHeight));
        ret.push_back(Pair("bestblock", stats.hashBlock.GetHex()));
        ret.push_back(Pair("transactions", (boost::int64_t)stats.nTransactions));
        ret.push_back(Pair("txouts", (boost::int64_t)stats.nTransactionOutputs));
        ret.push_back(Pair("bytes_serialized", (boost::int64_t)stats.nSerializedSize));
        ret.push_back(Pair("hash_serialized", stats.hashSerialized.GetHex()));
        ret.push_back(Pair("total_amount", ValueFromAmount(stats.nTotalAmount)));
    }
    return ret;
}

Value gettxout(const Array& params, bool fHelp)
{
    if (fHelp || params.size() < 2 || params.size() > 3)
        throw runtime_error(
            "gettxout <txid> <n> [includemempool=true]\n"
            "Returns details about an unspent transaction output.");

    Object ret;

    std::string strHash = params[0].get_str();
    uint256 hash(strHash);
    int n = params[1].get_int();
    bool fMempool = true;
    if (params.size() > 2)
        fMempool = params[2].get_bool();

    CCoins coins;
    if (fMempool) {
        LOCK(mempool.cs);
        CCoinsViewMemPool view(*pcoinsTip, mempool);
        if (!view.GetCoins(hash, coins))
            return Value::null;
        mempool.pruneSpent(hash, coins); // TODO: this should be done by the CCoinsViewMemPool
    } else {
        if (!pcoinsTip->GetCoins(hash, coins))
            return Value::null;
    }
    if (n<0 || (unsigned int)n>=coins.vout.size() || coins.vout[n].IsNull())
        return Value::null;

    ret.push_back(Pair("bestblock", pcoinsTip->GetBestBlock()->GetBlockHash().GetHex()));
    if ((unsigned int)coins.nHeight == MEMPOOL_HEIGHT)
        ret.push_back(Pair("confirmations", 0));
    else
        ret.push_back(Pair("confirmations", pcoinsTip->GetBestBlock()->nHeight - coins.nHeight + 1));
    ret.push_back(Pair("value", ValueFromAmount(coins.vout[n].nValue)));
    Object o;
    ScriptPubKeyToJSON(coins.vout[n].scriptPubKey, o);
    ret.push_back(Pair("scriptPubKey", o));
    ret.push_back(Pair("version", coins.nVersion));
    ret.push_back(Pair("coinbase", coins.fCoinBase));

    return ret;
}


