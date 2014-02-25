// Copyright (c) 2010 Satoshi Nakamoto
// Copyright (c) 2009-2012 The Bitcoin developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

#include <boost/assign/list_of.hpp>

#include "base58.h"
#include "bitcoinrpc.h"
#include "db.h"
#include "init.h"
#include "main.h"
#include "net.h"
#include "wallet.h"
#include <boost/algorithm/string.hpp>
#include <boost/foreach.hpp>
#include <boost/lexical_cast.hpp>

using namespace std;
using namespace boost;

using namespace boost::assign;
using namespace json_spirit;

//
// Utilities: convert hex-encoded Values
// (throws error if not hex).
//


std::string BoincProjectAddress(int projectid);
int BoincProjectId(std::string grc);
int CheckCPUWork(std::string lastblockhash, std::string greatblockhash, std::string greatgrandparentsblockhash, std::string boinchash);
int CheckCPUWorkByBlock(int blocknumber);
int UpgradeClient();


extern std::string TxToString(const CTransaction& tx, const uint256 hashBlock, int64& out_amount, int64& out_locktime, int64& out_projectid, std::string& out_projectaddress, std::string& out_comments, std::string& out_grcaddress);

extern double TxPaidToCPUMiner(const CTransaction& tx, int nBlock, std::string address, double& out_total, std::string& out_comments);



extern void SendGridcoinProjectBeacons();


std::map<string,MiningEntry> BlockToCPUMinerPayments(const CBlock& block, const CBlockIndex* blockindex);


uint256 ParseHashV(const Value& v, string strName)
{
    string strHex;
    if (v.type() == str_type)
        strHex = v.get_str();
    if (!IsHex(strHex)) // Note: IsHex("") is false
        throw JSONRPCError(RPC_INVALID_PARAMETER, strName+" must be hexadecimal string (not '"+strHex+"')");
    uint256 result;
    result.SetHex(strHex);
    return result;
}
uint256 ParseHashO(const Object& o, string strKey)
{
    return ParseHashV(find_value(o, strKey), strKey);
}
vector<unsigned char> ParseHexV(const Value& v, string strName)
{
    string strHex;
    if (v.type() == str_type)
        strHex = v.get_str();
    if (!IsHex(strHex))
        throw JSONRPCError(RPC_INVALID_PARAMETER, strName+" must be hexadecimal string (not '"+strHex+"')");
    return ParseHex(strHex);
}
vector<unsigned char> ParseHexO(const Object& o, string strKey)
{
    return ParseHexV(find_value(o, strKey), strKey);
}

void ScriptPubKeyToJSON(const CScript& scriptPubKey, Object& out)
{
    txnouttype type;
    vector<CTxDestination> addresses;
    int nRequired;

    out.push_back(Pair("asm", scriptPubKey.ToString()));
    out.push_back(Pair("hex", HexStr(scriptPubKey.begin(), scriptPubKey.end())));

    if (!ExtractDestinations(scriptPubKey, type, addresses, nRequired))
    {
        out.push_back(Pair("type", GetTxnOutputType(TX_NONSTANDARD)));
        return;
    }

    out.push_back(Pair("reqSigs", nRequired));
    out.push_back(Pair("type", GetTxnOutputType(type)));

    Array a;
    BOOST_FOREACH(const CTxDestination& addr, addresses)
        a.push_back(CBitcoinAddress(addr).ToString());
    out.push_back(Pair("addresses", a));
}



std::string PubKeyToGRCAddress(const CScript& scriptPubKey)
{
    txnouttype type;
    vector<CTxDestination> addresses;
    int nRequired;

    if (!ExtractDestinations(scriptPubKey, type, addresses, nRequired))
    {
        return "";
    }

    Array a;
	std::string grcaddress = "";
    BOOST_FOREACH(const CTxDestination& addr, addresses)
	{
		grcaddress = CBitcoinAddress(addr).ToString();
	}
	return grcaddress;
}



std::string TxToString(const CTransaction& tx, const uint256 hashBlock, int64& out_amount, int64& out_locktime, int64& out_projectid, 
		std::string& out_projectaddress, std::string& out_comments, std::string& out_grcaddress)
{
	//Returns project information and user public wallet address that initiated the project

	int64 locktime = (boost::int64_t)tx.nLockTime;
	int64 amountproject  = 0;
	std::string grc1 = "";
	int64 amountwallet = 0;
	std::string grc2 = "";
	//Project transactions are always 3
	if (tx.vout.size()  != 4) {
		return "";
	}

	out_projectaddress="";
	out_projectid = 0;
	out_locktime = 0;
	out_amount = 0;
	
	//Output 1 contains project info
    const CTxOut& txproject = tx.vout[0];
	amountwallet = txproject.nValue; 
	
	if (amountwallet != 167) {
		//printf("project amount mismatch.");
	}

    grc1 = PubKeyToGRCAddress(txproject.scriptPubKey);
	int boincprojectid = BoincProjectId(grc1);
	if (boincprojectid == 0) return ""; //Project transaction always map to a project
	// Extract the Sender GRC address

	const CTxOut& txSENDER = tx.vout[3]; //TX OUTPUT #1
	std::string grcSENDER = PubKeyToGRCAddress(txSENDER.scriptPubKey);
	
	const CTxOut& txProjectPayment = tx.vout[2];
	amountproject = txProjectPayment.nValue;
	if (txSENDER.nValue != 267) {
		return "";
	}

	if (amountproject == 260) {
		return ""; // Project checksum mismatch
	}
	std::string o = "";
	o = grc1 + "," + boost::lexical_cast<string>(amountwallet) + "," + grc2 + "," + boost::lexical_cast<string>(amountproject) + "----" + grcSENDER;
	out_comments = o;

	out_amount = amountwallet;
	out_locktime = locktime;
	out_projectid = boincprojectid;
	out_projectaddress = grc1;
	out_grcaddress = grcSENDER;
	return grcSENDER;
	
}





double TxPaidToCPUMiner(const CTransaction& tx, int nBlock, std::string address, double& out_total, std::string& out_comments)
{

	//Returns tx amount paid to cpu miner in coinbase 
	std::string grc1 = "";
	double total = 0;
	bool bCPUTx = false;
	for (unsigned int i = 0; i < tx.vout.size(); i++)
    {
        const CTxOut& txout = tx.vout[i];
		bCPUTx = false;
        double paid = DoubleFromAmount(txout.nValue);
		std::string sPaid = RoundToString(paid,10);
		std::string suffix = "";
		 grc1 = PubKeyToGRCAddress(txout.scriptPubKey);
		
		if (sPaid.length() > 4 && paid > 0) {
			suffix = sPaid.substr(sPaid.length()-4,4);
			if (suffix == "7900") bCPUTx = true;
		}

	    if (!cpuminerpaymentsconsolidated[grc1].paid && bCPUTx) 
				{
					MiningEntry me;
					cpuminerpaymentsconsolidated.insert(map<string,MiningEntry>::value_type(grc1,me));
					cpuminerpaymentsconsolidated[grc1].paid=true;
					cpuminerpaymentsconsolidated[grc1] = me;
		 		}
    	MiningEntry me1 = cpuminerpaymentsconsolidated[grc1];
		me1.totalpayments = me1.totalpayments + paid;
		cpuminerpaymentsconsolidated[grc1]=me1;
    }
    
	std::string o = "";
	o = grc1 + "," + boost::lexical_cast<string>(total);
	out_comments = o;
	out_total = total;
	return total;
	
}







void TxToJSON(const CTransaction& tx, const uint256 hashBlock, Object& entry)
{
    entry.push_back(Pair("txid", tx.GetHash().GetHex()));
    entry.push_back(Pair("version", tx.nVersion));
    entry.push_back(Pair("locktime", (boost::int64_t)tx.nLockTime));
    Array vin;
    BOOST_FOREACH(const CTxIn& txin, tx.vin)
    {
        Object in;
        if (tx.IsCoinBase())
            in.push_back(Pair("coinbase", HexStr(txin.scriptSig.begin(), txin.scriptSig.end())));
        else
        {
            in.push_back(Pair("txid", txin.prevout.hash.GetHex()));
            in.push_back(Pair("vout", (boost::int64_t)txin.prevout.n));
            Object o;
            o.push_back(Pair("asm", txin.scriptSig.ToString()));
            o.push_back(Pair("hex", HexStr(txin.scriptSig.begin(), txin.scriptSig.end())));
            in.push_back(Pair("scriptSig", o));
        }
        in.push_back(Pair("sequence", (boost::int64_t)txin.nSequence));
        vin.push_back(in);
    }
    entry.push_back(Pair("vin", vin));
    Array vout;
    for (unsigned int i = 0; i < tx.vout.size(); i++)
    {
        const CTxOut& txout = tx.vout[i];
        Object out;
        out.push_back(Pair("value", ValueFromAmount(txout.nValue)));
        out.push_back(Pair("n", (boost::int64_t)i));
        Object o;
        ScriptPubKeyToJSON(txout.scriptPubKey, o);
        out.push_back(Pair("scriptPubKey", o));
        vout.push_back(out);
    }
    entry.push_back(Pair("vout", vout));

    if (hashBlock != 0)
    {
        entry.push_back(Pair("blockhash", hashBlock.GetHex()));
        map<uint256, CBlockIndex*>::iterator mi = mapBlockIndex.find(hashBlock);
        if (mi != mapBlockIndex.end() && (*mi).second)
        {
            CBlockIndex* pindex = (*mi).second;
            if (pindex->IsInMainChain())
            {
                entry.push_back(Pair("confirmations", 1 + nBestHeight - pindex->nHeight));
                entry.push_back(Pair("time", (boost::int64_t)pindex->nTime));
                entry.push_back(Pair("blocktime", (boost::int64_t)pindex->nTime));
            }
            else
                entry.push_back(Pair("confirmations", 0));
        }
    }
}

Value getrawtransaction(const Array& params, bool fHelp)
{
    if (fHelp || params.size() < 1 || params.size() > 2)
        throw runtime_error(
            "getrawtransaction <txid> [verbose=0]\n"
            "If verbose=0, returns a string that is\n"
            "serialized, hex-encoded data for <txid>.\n"
            "If verbose is non-zero, returns an Object\n"
            "with information about <txid>.");

    uint256 hash = ParseHashV(params[0], "parameter 1");

    bool fVerbose = false;
    if (params.size() > 1)
        fVerbose = (params[1].get_int() != 0);

    CTransaction tx;
    uint256 hashBlock = 0;
    if (!GetTransaction(hash, tx, hashBlock, true))
        throw JSONRPCError(RPC_INVALID_ADDRESS_OR_KEY, "No information available about transaction");

    CDataStream ssTx(SER_NETWORK, PROTOCOL_VERSION);
    ssTx << tx;
    string strHex = HexStr(ssTx.begin(), ssTx.end());

    if (!fVerbose)
        return strHex;

    Object result;
    result.push_back(Pair("hex", strHex));
    TxToJSON(tx, hashBlock, result);
	//Gridcoin - return block info 
	int64 out_amount = 0;
	int64 out_locktime = 0;
	int64 nProjId = 0;
	std::string sProjectAddress = "";
	std::string comments = "";
	std::string grc_address = "";
	std::string o1 = TxToString(tx, hashBlock, out_amount, out_locktime, nProjId, sProjectAddress, comments, grc_address);


	result.push_back(Pair("GRCAddress", grc_address));
	//result.push_back(Pair("GRCAmount",out_amount));
	
	result.push_back(Pair("Comments",comments));
	

    return result;
}





Value listunspent(const Array& params, bool fHelp)
{
    if (fHelp || params.size() > 3)
        throw runtime_error(
            "listunspent [minconf=1] [maxconf=9999999]  [\"address\",...]\n"
            "Returns array of unspent transaction outputs\n"
            "with between minconf and maxconf (inclusive) confirmations.\n"
            "Optionally filtered to only include txouts paid to specified addresses.\n"
            "Results are an array of Objects, each of which has:\n"
            "{txid, vout, scriptPubKey, amount, confirmations}");

    RPCTypeCheck(params, list_of(int_type)(int_type)(array_type));

    int nMinDepth = 1;
    if (params.size() > 0)
        nMinDepth = params[0].get_int();

    int nMaxDepth = 9999999;
    if (params.size() > 1)
        nMaxDepth = params[1].get_int();

    set<CBitcoinAddress> setAddress;
    if (params.size() > 2)
    {
        Array inputs = params[2].get_array();
        BOOST_FOREACH(Value& input, inputs)
        {
            CBitcoinAddress address(input.get_str());
            if (!address.IsValid())
                throw JSONRPCError(RPC_INVALID_ADDRESS_OR_KEY, string("Invalid Gridcoin address: ")+input.get_str());
            if (setAddress.count(address))
                throw JSONRPCError(RPC_INVALID_PARAMETER, string("Invalid parameter, duplicated address: ")+input.get_str());
           setAddress.insert(address);
        }
    }

    Array results;
    vector<COutput> vecOutputs;
    pwalletMain->AvailableCoins(vecOutputs, false);
    BOOST_FOREACH(const COutput& out, vecOutputs)
    {
        if (out.nDepth < nMinDepth || out.nDepth > nMaxDepth)
            continue;

        if (setAddress.size())
        {
            CTxDestination address;
            if (!ExtractDestination(out.tx->vout[out.i].scriptPubKey, address))
                continue;

            if (!setAddress.count(address))
                continue;
        }

        int64 nValue = out.tx->vout[out.i].nValue;
        const CScript& pk = out.tx->vout[out.i].scriptPubKey;
        Object entry;
        entry.push_back(Pair("txid", out.tx->GetHash().GetHex()));
        entry.push_back(Pair("vout", out.i));
        CTxDestination address;
        if (ExtractDestination(out.tx->vout[out.i].scriptPubKey, address))
        {
            entry.push_back(Pair("address", CBitcoinAddress(address).ToString()));
            if (pwalletMain->mapAddressBook.count(address))
                entry.push_back(Pair("account", pwalletMain->mapAddressBook[address]));
        }
        entry.push_back(Pair("scriptPubKey", HexStr(pk.begin(), pk.end())));
        if (pk.IsPayToScriptHash())
        {
            CTxDestination address;
            if (ExtractDestination(pk, address))
            {
                const CScriptID& hash = boost::get<const CScriptID&>(address);
                CScript redeemScript;
                if (pwalletMain->GetCScript(hash, redeemScript))
                    entry.push_back(Pair("redeemScript", HexStr(redeemScript.begin(), redeemScript.end())));
            }
        }
        entry.push_back(Pair("amount",ValueFromAmount(nValue)));
        entry.push_back(Pair("confirmations",out.nDepth));
        results.push_back(entry);
    }

    return results;
}


static inline bool GetCPUMiningMode()
{
    if (mapArgs["-cpumining"] == "true") 
	{
		return true;
	} 
	return false;
}



static inline bool GetPoolMiningMode()
{
    if (mapArgs["-poolmining"] == "true") 
	{
		return true;
	} 
	return false;
}


static inline string GetBoincProjectUserId(int projectid)
{
	std::string key = "-Project" + RoundToString(projectid,0);
	return mapArgs[key];
}



Value getpoolminingmode(const Array& params, bool fHelp)
{
    if (fHelp || params.size() > 3)
        throw runtime_error("getpoolminingmode\r\n         Returns true or false.  Set poolmining=true in gridcoin.conf\n");
		Object entry;
		bool pm = GetPoolMiningMode();
		Array results;
     	entry.push_back(Pair("PoolMiningMode=", pm));
		results.push_back(entry);
		return results;
}



















std::string SendMultiProngedTransaction(int projectid, std::string userid)
{
     std::vector<std::pair<CScript, int64> > vecSend;
     CScript scriptPubKey;
	 std::string projectaddress = BoincProjectAddress(projectid);
	 std::string grcaddress = DefaultWalletAddress();
	 CBitcoinAddress address1(projectaddress);
     CBitcoinAddress address2(grcaddress);
	 CBitcoinAddress address3(grcaddress);

     
	 if (userid.length() > 7 || userid=="" || userid.length() < 3) return "Invalid project userid";
	//11-23-2013
	  
     long lAmount = boost::lexical_cast<long>(userid);
	       
	 scriptPubKey.SetDestination(address1.Get());
	 vecSend.push_back(make_pair(scriptPubKey, lAmount));

	 scriptPubKey.SetDestination(address2.Get());
	 vecSend.push_back(make_pair(scriptPubKey, 167));
	 
	 scriptPubKey.SetDestination(address3.Get());
	 vecSend.push_back(make_pair(scriptPubKey, 267));

	 CWalletTx wtx;
	 if (pwalletMain->IsLocked()) return "Wallet Locked";
	    CReserveKey keyChange(pwalletMain);

     int64 nFeeRequired = 0;
     std::string strFailReason;
     LOCK2(cs_main, pwalletMain->cs_wallet);

     bool fCreated = pwalletMain->CreateTransaction(vecSend, wtx, keyChange, nFeeRequired, strFailReason);

     if(!pwalletMain->CommitTransaction(wtx, keyChange))
       {
            return "failure";
       }
        std::string txid = wtx.GetHash().GetHex();
        return "succeed";

}






















std::string Compensate2(string grc_address, int64 nAmount, string commentfrom, string commentto)
{

	try {
			CBitcoinAddress address(grc_address);
			if (!address.IsValid()) return "Invalid Gridcoin address";
			CWalletTx wtx;
			wtx.mapValue["comment"] = commentfrom;
			wtx.mapValue["to"]      = commentto;
			if (pwalletMain->IsLocked()) return "Wallet Locked";
		     string strError = pwalletMain->SendMoneyToDestination(address.Get(), nAmount, wtx);
			if (strError != "") return strError;
			string transaction_id = wtx.GetHash().GetHex().c_str();
			if (transaction_id.length() > 5) {
				return "SUCCESS";
			}
			return "FAIL";
	}
			catch (std::exception &e) {
  
		return "FAIL2";
	}
}






std::string Compensate(string grc_address, double dAmount, string commentfrom, string commentto)
{

	try {
			CBitcoinAddress address(grc_address);
			if (!address.IsValid()) return "Invalid Gridcoin address";
			CWalletTx wtx;
			wtx.mapValue["comment"] = commentfrom;
			wtx.mapValue["to"]      = commentto;
			if (pwalletMain->IsLocked()) return "Wallet Locked";
  		    int64 nAmount = AmountFromValue(dAmount);
			string strError = pwalletMain->SendMoneyToDestination(address.Get(), nAmount, wtx);
			if (strError != "") return strError;
			string transaction_id = wtx.GetHash().GetHex().c_str();
			if (transaction_id.length() > 5) {
				return "SUCCESS";
			}
			return "FAIL";
	}
			catch (std::exception &e) {
  
		return "FAIL2";
	}
}


std::string RoundToString(double d, int place)
{
	//boost::lexical_cast<string>(iBU)
    std::ostringstream ss;
    ss << std::fixed << std::setprecision(place) << d ;
    return ss.str() ;
}





std::map<std::string, MiningEntry> CalculatePoolMining()
{

	int nMaxDepth = nBestHeight;
    CBlock block;

	CBlockIndex* pLastBlock = FindBlockByHeight(nMaxDepth);
	block.ReadFromDisk(pLastBlock);
	int64 LastBlockTime = pLastBlock->GetBlockTime();

	//Gridcoin - 10-30-2013  Calculate the lookback period
	//      ---------------------- Difficulty Calculation For Lookback Period: ------------------------------------
	////    Difficulty = (24) *60 mins * 60 secs * 1.8
    //////  400Kh/s @ 1 difficulty = 8 blocks per day (lookbackperiod = 2 days)

	double diff = GetDifficulty(pLastBlock);
	if (diff < .05) diff = .05;
	double lookback = diff * 24 * 60 * 60 * 1.8;
	double total_utilization = 0;
	double total_rows = 0;
    double avg_boinc = 0;
    string wallet = "";
	
    //  -1 showing for miner2 ba packet
    //	boinc_authenticity_packet = BoincMD5,sBoincBALevel,UtilizationLevel,CARD_VERSION,POOL_MINING_MODE,WalletAddress();
	//  Compensate Miners -----------------------------------------
	//  std::map<std::string, MiningEntry> minerpayments;
	//  11-1-2013
    minerpayments.clear();

	double compensated_rows = 0;
	string last_wallet = "";
	double total_payment = 0;
	double iBU = 0;
	double compensation = 0;
	printf("Reaching payout to miners.");
	double total_shares = 0;

	MiningEntry ae;
    //Iterate through the chain in reverse
	int pay = 1;

    for (int i = nMaxDepth; i > 0; i--)
    {
    	pay = blockcache[i];
		if (pay == 0) {
			//blockcache.insert(map<string,MiningEntry>::value_type(wallet,meNew));
			blockcache.insert(map<int,int>::value_type(i,1));
		}
		if (pay != 10)
		{
		CBlockIndex* pblockindex = FindBlockByHeight(i);
	    block.ReadFromDisk(pblockindex);
	
		int64 nActualTimespan = LastBlockTime - pblockindex->GetBlockTime();
		blockcache[i]=20;

        if (block.hashBoinc.length() > 10 && block.hashBoinc.length() < 750) {
				vector<std::string> vecBoinc;
		        std::string brh = block.hashBoinc;
				boost::split(vecBoinc,brh,boost::is_any_of(","));
				if (vecBoinc.size() > 4) {
					string pool_mode = vecBoinc[4];
					if (pool_mode=="SOLO_MINING" || pool_mode=="POOL_MINING") {
						        if (nActualTimespan > lookback) i=0;

						     	if (nActualTimespan < lookback) {
								if (pool_mode == "POOL_MINING") {
										string md5 = vecBoinc[0];
										string BA = vecBoinc[1];
										string bu = vecBoinc[2];
										string wallet = vecBoinc[5];
									    iBU = boost::lexical_cast<double>(bu);
										//11-1-2013  At this point in time we are using GRC public addresses, check for size between 20-40

										if (iBU > 0 && wallet.length() > 20 && wallet.length() < 44) {
														
											MiningEntry meNew;
											compensated_rows++;
											string blocktime =  DateTimeStrFormat("%Y-%m-%d %H:%M:%S", pblockindex->GetBlockTime()).c_str();
											int blockhour = boost::lexical_cast<int>(DateTimeStrFormat("%H", pblockindex->GetBlockTime()).c_str());
											int wallethour = HourFromGRCAddress(wallet);
											blockcache[i]=1;
											meNew = minerpayments[wallet];
											if (meNew.paid != true) {
												meNew.paid=true;
												meNew.payments=0;
												minerpayments.insert(map<string,MiningEntry>::value_type(wallet,meNew));
											} 
											
											meNew.strAccount = wallet;
											meNew.shares = meNew.shares + iBU;
											meNew.payments++;
											meNew.blockhour = blockhour;
											meNew.wallethour = wallethour;
											meNew.boinchash = block.hashBoinc;
											//string narr = "GRCMiningPmt_Shares=" + RoundToString(total_shares,2) + "_" + RoundToString(rbpps,7) + "RBPPS[Total:GRC" + RoundToString(payout,4) + "],Payment=" + RoundToString(total_payment,4);
											string long_narr = "Shares:" + RoundToString(meNew.shares,2) +",Block:" + RoundToString(i,0) + ",TimeSpan:" + RoundToString(nActualTimespan,0);
											meNew.strComment = long_narr;
											minerpayments[wallet]=meNew;
											total_utilization = total_utilization + iBU;
											total_rows++;
											//printf("PoolMining: %s ",long_narr.c_str());
											total_shares = total_shares + iBU;
					
										}

								}
							}
					  }
				}
			}
		}
	}
	
		//Figure out avg boinc utilization
    	double rbpps = 0;
	    double payout = 0;
		if (total_rows > 0) {
			avg_boinc = total_utilization/total_rows;
			payout = avg_boinc * 1.5;
			if (payout > 150) payout = 150;
			if (payout < 5) payout=5;
			if (total_utilization < .01) total_utilization=.01;
			rbpps = payout/total_utilization;
		 }

		//End of Payments
		//Persist the rbpps, payout, total_utilization, total_rows:
		MiningEntry me;
    	me.rbpps = rbpps;
		me.payout = payout;
		me.totalutilization = total_utilization;
		me.totalrows= total_rows;
		me.avgboinc = avg_boinc;
		me.lookback = lookback;
		me.difficulty  = diff;

		minerpayments.insert(map<string,MiningEntry>::value_type("totals",me));
		//printf("rbpps: %d ",rbpps);
		return minerpayments;
    
}







std::map<std::string, MiningEntry> CalculateCPUMining()
{

	int nMaxDepth = nBestHeight;
    CBlock block;
	CBlockIndex* pLastBlock = FindBlockByHeight(nMaxDepth);
	block.ReadFromDisk(pLastBlock);
	int64 LastBlockTime = pLastBlock->GetBlockTime();
	
	double diff = GetDifficulty(pLastBlock);
	if (diff < .05) diff = .05;
    double lookback = 24 * 60 * 60;

	double total_utilization = 0;
	double total_rows = 0;
    double avg_boinc = 0;
    string wallet = "";

	cpuminerpayments.clear();
	cpuminerpaymentsconsolidated.clear();

	double compensated_rows = 0;
	string last_wallet = "";
	double total_payment = 0;
	double iBU = 0;
	double compensation = 0;
	printf("Reaching payout CPU to miners.");
	double total_shares = 0;
	//1-28-2014

	MiningEntry ae;
    //Iterate through the chain in reverse
	
	int istart = 0;

    for (int ii = nMaxDepth; ii > 0; ii--)
    {
     	CBlockIndex* pblockindex = FindBlockByHeight(ii);
		block.ReadFromDisk(pblockindex);
	    int64 nActualTimespan = LastBlockTime - pblockindex->GetBlockTime();
	    if (nActualTimespan > lookback) 
		{
						istart = ii;
						break;
		}
    }
	

    for (int i = istart; i <= nMaxDepth; i++)
    {
     	CBlockIndex* pblockindex = FindBlockByHeight(i);
		block.ReadFromDisk(pblockindex);
	    int64 nActualTimespan = LastBlockTime - pblockindex->GetBlockTime();
        cpuminerpayments = BlockToCPUMinerPayments(block,pblockindex);
		MiningEntry me;
    	me.lookback = lookback;
		me.difficulty  = diff;
		cpuminerpayments.insert(map<string,MiningEntry>::value_type("totals",me));
	}  

	//Consolidate the Project-CPUMiners-ProjectCredits collection into CPUMiners collection - For each CPU miner, verify PoW
	int inum=0;
	
	for(map<string,MiningEntry>::iterator ii=cpuminerpayments.begin(); ii!=cpuminerpayments.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpayments[(*ii).first];
			MiningEntry pow = cpupow[ae.homogenizedkey];

	        if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2 && pow.cpupowverificationtries > 0 && pow.cpupowverificationresult > 0) 
			{
				inum++;
		    	
				if (!cpuminerpaymentsconsolidated[ae.strAccount].paid) 
				{ 
					MiningEntry me2;
					cpuminerpaymentsconsolidated.insert(map<string,MiningEntry>::value_type(ae.strAccount,me2));
					cpuminerpaymentsconsolidated[ae.strAccount].paid=true;
					cpuminerpaymentsconsolidated[ae.strAccount] = me2;
		 		}
	            MiningEntry me1;
				me1 = cpuminerpaymentsconsolidated[ae.strAccount];
         	    me1.strAccount = ae.strAccount;
				me1.credits = me1.credits + pow.cpupowverificationresult;
			    
			    cpuminerpaymentsconsolidated[ae.strAccount] = me1;
			}

    }

	return cpuminerpayments;

  }





void SendGridcoinProjectBeacons()
{

	if (!GetCPUMiningMode()) return;

	std::map<std::string, MiningEntry> cpumap = CalculateCPUMining();
	//For each Gridcoin project in the lookback period, send a project beacon
	MiningEntry me;

	for (int i = 1; i < 6; i++) 
	{
		std::string key = RoundToString(i,0) + DefaultWalletAddress();
		//Verify user is participating first:
		std::string userid = GetBoincProjectUserId(i);
		if (userid.length() > 2)  
		{
			if (cpumap[key].locktime == 0) 
			{
				
				SendMultiProngedTransaction(i,userid);
				printf("Sending beacon for project %d",i);
				cpumap[key].locktime = 1;
				cpuminerpayments[key].locktime=1;
			}
			else
			{
				printf("Transaction previously sent.");
			}
		}

	}

}










Value checkwork(const Array& params, bool fHelp)
{
	 if (fHelp)
        throw runtime_error(
            "checkwork <blocknumber> \n"
            "Returns CPU Miner Result Code after checking block"
            + HelpRequiringPassphrase());

	 printf("Starting checkwork...");

    int blocknumber = params[0].get_int();
    if (blocknumber < 5 || blocknumber > nBestHeight)
        throw runtime_error("Block number out of range.");
	Object entry;
	
	try {
	
	entry.push_back(Pair("Check Work",1.1));
	entry.push_back(Pair("Block Number",blocknumber));
	//11-29-2013
    CBlock block;
	CBlockIndex* pBlock = FindBlockByHeight(blocknumber);
	block.ReadFromDisk(pBlock);
	std::string boinchash = block.hashBoinc.c_str();
	entry.push_back(Pair("Boinc Hash",boinchash));
	
	pBlock = FindBlockByHeight(blocknumber-1);
	block.ReadFromDisk(pBlock);
	std::string blockhash1 = pBlock->phashBlock->GetHex().c_str();
	pBlock = FindBlockByHeight(blocknumber-2);
	block.ReadFromDisk(pBlock);
	std::string blockhash2 = pBlock->phashBlock->GetHex().c_str();
	pBlock = FindBlockByHeight(blocknumber-3);
	block.ReadFromDisk(pBlock);
	std::string blockhash3 = pBlock->phashBlock->GetHex().c_str();
	entry.push_back(Pair("Last Block Hash",blockhash1));
	entry.push_back(Pair("Prior Block Hash",blockhash2));
	entry.push_back(Pair("Great Block Hash",blockhash3));
	int result = 0;
//	result = CheckCPUWork(blockhash1,blockhash2,blockhash3,boinchash);
	entry.push_back(Pair("Check Work Result",result));
//	result = CheckCPUWorkByBlock(blocknumber);
	entry.push_back(Pair("CheckWorkByBlock", result));
	}

	 catch (std::exception &e) {
        throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "Block decode failed");
	 }
   
	return entry;

}




Value upgrade(const Array& params, bool fHelp)
{
		if (fHelp || params.size() > 3)
        throw runtime_error(
            "upgrade \n"
            "Upgrades wallet to the latest version.\n"
            "{}");
		Object entry;
		entry.push_back(Pair("Upgrading Wallet Version",1.0));
		int result = 0;
	//	result = UpgradeClient();
     	entry.push_back(Pair("Result",result));
		return result;
}


Value listminers(const Array& params, bool fHelp)
{

	printf("Creating miner payout report...");
		if (fHelp || params.size() > 3)
        throw runtime_error(
            "listminers [minconf=1] [maxconf=9999999] \n"
            "Returns array of pool mining transactions\n"
            "{}");

	RPCTypeCheck(params, list_of(int_type)(int_type)(array_type));

	Array results;

	minerpayments = CalculatePoolMining();
    
    int inum = 0;
   
    double rbpps = minerpayments["totals"].rbpps;
    double total_payments = 0;
	Object entry;
	
	entry.push_back(Pair("Mining Report Version",1.3));
	std::string boinc_authenticity = BoincAuthenticity();
	entry.push_back(Pair("Boinc Version",boinc_authenticity));
	entry.push_back(Pair("Difficulty",minerpayments["totals"].difficulty));
	entry.push_back(Pair("Grand Total Utilization",minerpayments["totals"].totalutilization));
	entry.push_back(Pair("Average Boinc", minerpayments["totals"].avgboinc));
	entry.push_back(Pair("Grand Total Block Payout",minerpayments["totals"].payout));
	entry.push_back(Pair("Grand Total RBPPS",rbpps));
	entry.push_back(Pair("Lookback Period",minerpayments["totals"].lookback));

	results.push_back(entry);

	for(map<string,MiningEntry>::iterator ii=minerpayments.begin(); ii!=minerpayments.end(); ++ii) 
	{

			MiningEntry ae = minerpayments[(*ii).first];

	        if (ae.strAccount.length() > 5) 
			{
				double compensation = ae.shares*rbpps;
	     		Object e;
				inum++;
				e.push_back(Pair("Payment #",inum));
				e.push_back(Pair("Payment Comment",ae.strComment));
				e.push_back(Pair("Block Hour",ae.blockhour));
				e.push_back(Pair("Wallet Hour",ae.wallethour));
				int64 currenttime = GetTime();

				int currenthour = boost::lexical_cast<int>(DateTimeStrFormat("%H", currenttime));
			    e.push_back(Pair("Current Hour",currenthour));
				
				e.push_back(Pair("Boinc Hash",ae.boinchash));
				
				e.push_back(Pair("Shares", RoundToString(ae.shares,2)));
				e.push_back(Pair("Account",ae.strAccount));
				e.push_back(Pair("Payments",ae.payments));
				e.push_back(Pair("Compensation",RoundToString(compensation,6)));
				total_payments = total_payments + compensation;
	     		results.push_back(e);
			}

    }
		Object e3;
		e3.push_back(Pair("Grand Total Payments",RoundToString(total_payments,6)));
		results.push_back(e3);
        return results;

}




Value listcpuminers(const Array& params, bool fHelp)
{

	printf("Creating cpu miner payout report...");
		if (fHelp || params.size() > 3)
        throw runtime_error(
            "listminers [minconf=1] [maxconf=9999999] \n"
            "Returns array of pool mining transactions\n"
            "{}");

	RPCTypeCheck(params, list_of(int_type)(int_type)(array_type));
	Array results;

    if (nMinerPaymentCount > 5) nMinerPaymentCount=0;
	nMinerPaymentCount++;
	cpuminerpayments.clear();
	cpuminerpayments = CalculateCPUMining();
	    
    int inum = 0;
   
    double rbpps = cpuminerpayments["totals"].rbpps;
    double total_payments = 0;
	Object entry;
	
	entry.push_back(Pair("CPU Credit Details Report Version",1.01));
	entry.push_back(Pair("Difficulty",cpuminerpayments["totals"].difficulty));
    entry.push_back(Pair("Lookback Period", cpuminerpayments["totals"].lookback));

	results.push_back(entry);

	for(map<string,MiningEntry>::iterator ii=cpuminerpayments.begin(); ii!=cpuminerpayments.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpayments[(*ii).first];

	        if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2) 
			{
				
				//double compensation = ae.shares*rbpps;
	     		Object e;
				inum++;
				e.push_back(Pair("CPU Miner #",inum));
				e.push_back(Pair("Payment Comment",ae.strComment));
				e.push_back(Pair("GRC Address",ae.strAccount));
				e.push_back(Pair("ProjectId",ae.projectid));
				e.push_back(Pair("ProjectAddress",ae.projectaddress));
				e.push_back(Pair("Project UserId",ae.projectuserid));
				MiningEntry CPU = cpupow[ae.homogenizedkey];
				e.push_back(Pair("CPU Daily Avg Credits Earned",CPU.cpupowverificationresult));
				e.push_back(Pair("CPU Verification Tries",CPU.cpupowverificationtries));
				e.push_back(Pair("CPU Verification GRC Address",CPU.strAccount));

				e.push_back(Pair("CPU PoW Key",CPU.cpupowhash));
				e.push_back(Pair("Total Payments",ae.totalpayments));
			
				e.push_back(Pair("Block #",ae.blocknumber));
				e.push_back(Pair("TX ID",ae.transactionid));
			
				e.push_back(Pair("Locktime",ae.locktime));

	     		results.push_back(e);
			}

    }



	Object e2;


	//Emit Consolidated report
	e2.push_back(Pair("Consolidated CPU Credit Report Version",1.0));
	results.push_back(e2);

	inum=0;
	for(map<string,MiningEntry>::iterator ii=cpuminerpaymentsconsolidated.begin(); ii!=cpuminerpaymentsconsolidated.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpaymentsconsolidated[(*ii).first];

	        if (ae.strAccount.length() > 5) 
			{
				//double compensation = ae.shares*rbpps;
	     		Object e3;
				inum++;
				e3.push_back(Pair("CPU Miner #",inum));
				e3.push_back(Pair("Payment Comment",ae.strComment));
				e3.push_back(Pair("GRC Address",ae.strAccount));
				e3.push_back(Pair("Total Avg Daily Credits",ae.credits));
				e3.push_back(Pair("Last Paid",ae.lastpaid));
				e3.push_back(Pair("Total Payments",ae.totalpayments));
			
	     		results.push_back(e3);
			}

    }


		Object e5;
		e5.push_back(Pair("Grand Total Payments",RoundToString(total_payments,6)));
		results.push_back(e5);
        return results;

}







Value createrawtransaction(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 2)
        throw runtime_error(
            "createrawtransaction [{\"txid\":txid,\"vout\":n},...] {address:amount,...}\n"
            "Create a transaction spending given inputs\n"
            "(array of objects containing transaction id and output number),\n"
            "sending to given address(es).\n"
            "Returns hex-encoded raw transaction.\n"
            "Note that the transaction's inputs are not signed, and\n"
            "it is not stored in the wallet or transmitted to the network.");

    RPCTypeCheck(params, list_of(array_type)(obj_type));

    Array inputs = params[0].get_array();
    Object sendTo = params[1].get_obj();

    CTransaction rawTx;

    BOOST_FOREACH(const Value& input, inputs)
    {
        const Object& o = input.get_obj();

        uint256 txid = ParseHashO(o, "txid");

        const Value& vout_v = find_value(o, "vout");
        if (vout_v.type() != int_type)
            throw JSONRPCError(RPC_INVALID_PARAMETER, "Invalid parameter, missing vout key");
        int nOutput = vout_v.get_int();
        if (nOutput < 0)
            throw JSONRPCError(RPC_INVALID_PARAMETER, "Invalid parameter, vout must be positive");

        CTxIn in(COutPoint(txid, nOutput));
        rawTx.vin.push_back(in);
    }

    set<CBitcoinAddress> setAddress;
    BOOST_FOREACH(const Pair& s, sendTo)
    {
        CBitcoinAddress address(s.name_);
        if (!address.IsValid())
            throw JSONRPCError(RPC_INVALID_ADDRESS_OR_KEY, string("Invalid Gridcoin address: ")+s.name_);

        if (setAddress.count(address))
            throw JSONRPCError(RPC_INVALID_PARAMETER, string("Invalid parameter, duplicated address: ")+s.name_);
        setAddress.insert(address);

        CScript scriptPubKey;
        scriptPubKey.SetDestination(address.Get());
        int64 nAmount = AmountFromValue(s.value_);

        CTxOut out(nAmount, scriptPubKey);
        rawTx.vout.push_back(out);
    }

    CDataStream ss(SER_NETWORK, PROTOCOL_VERSION);
    ss << rawTx;
    return HexStr(ss.begin(), ss.end());
}

Value decoderawtransaction(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 1)
        throw runtime_error(
            "decoderawtransaction <hex string>\n"
            "Return a JSON object representing the serialized, hex-encoded transaction.");

    vector<unsigned char> txData(ParseHexV(params[0], "argument"));
    CDataStream ssData(txData, SER_NETWORK, PROTOCOL_VERSION);
    CTransaction tx;
    try {
        ssData >> tx;
    }
    catch (std::exception &e) {
        throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "TX decode failed");
    }

    Object result;
    TxToJSON(tx, 0, result);

    return result;
}

Value signrawtransaction(const Array& params, bool fHelp)
{
    if (fHelp || params.size() < 1 || params.size() > 4)
        throw runtime_error(
            "signrawtransaction <hex string> [{\"txid\":txid,\"vout\":n,\"scriptPubKey\":hex,\"redeemScript\":hex},...] [<privatekey1>,...] [sighashtype=\"ALL\"]\n"
            "Sign inputs for raw transaction (serialized, hex-encoded).\n"
            "Second optional argument (may be null) is an array of previous transaction outputs that\n"
            "this transaction depends on but may not yet be in the block chain.\n"
            "Third optional argument (may be null) is an array of base58-encoded private\n"
            "keys that, if given, will be the only keys used to sign the transaction.\n"
            "Fourth optional argument is a string that is one of six values; ALL, NONE, SINGLE or\n"
            "ALL|ANYONECANPAY, NONE|ANYONECANPAY, SINGLE|ANYONECANPAY.\n"
            "Returns json object with keys:\n"
            "  hex : raw transaction with signature(s) (hex-encoded string)\n"
            "  complete : 1 if transaction has a complete set of signature (0 if not)"
            + HelpRequiringPassphrase());

    RPCTypeCheck(params, list_of(str_type)(array_type)(array_type)(str_type), true);

    vector<unsigned char> txData(ParseHexV(params[0], "argument 1"));
    CDataStream ssData(txData, SER_NETWORK, PROTOCOL_VERSION);
    vector<CTransaction> txVariants;
    while (!ssData.empty())
    {
        try {
            CTransaction tx;
            ssData >> tx;
            txVariants.push_back(tx);
        }
        catch (std::exception &e) {
            throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "TX decode failed");
        }
    }

    if (txVariants.empty())
        throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "Missing transaction");

    // mergedTx will end up with all the signatures; it
    // starts as a clone of the rawtx:
    CTransaction mergedTx(txVariants[0]);
    bool fComplete = true;

    // Fetch previous transactions (inputs):
    CCoinsView viewDummy;
    CCoinsViewCache view(viewDummy);
    {
        LOCK(mempool.cs);
        CCoinsViewCache &viewChain = *pcoinsTip;
        CCoinsViewMemPool viewMempool(viewChain, mempool);
        view.SetBackend(viewMempool); // temporarily switch cache backend to db+mempool view

        BOOST_FOREACH(const CTxIn& txin, mergedTx.vin) {
            const uint256& prevHash = txin.prevout.hash;
            CCoins coins;
            view.GetCoins(prevHash, coins); // this is certainly allowed to fail
        }

        view.SetBackend(viewDummy); // switch back to avoid locking mempool for too long
    }

    bool fGivenKeys = false;
    CBasicKeyStore tempKeystore;
    if (params.size() > 2 && params[2].type() != null_type)
    {
        fGivenKeys = true;
        Array keys = params[2].get_array();
        BOOST_FOREACH(Value k, keys)
        {
            CBitcoinSecret vchSecret;
            bool fGood = vchSecret.SetString(k.get_str());
            if (!fGood)
                throw JSONRPCError(RPC_INVALID_ADDRESS_OR_KEY, "Invalid private key");
            CKey key = vchSecret.GetKey();
            tempKeystore.AddKey(key);
        }
    }
    else
        EnsureWalletIsUnlocked();

    // Add previous txouts given in the RPC call:
    if (params.size() > 1 && params[1].type() != null_type)
    {
        Array prevTxs = params[1].get_array();
        BOOST_FOREACH(Value& p, prevTxs)
        {
            if (p.type() != obj_type)
                throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "expected object with {\"txid'\",\"vout\",\"scriptPubKey\"}");

            Object prevOut = p.get_obj();

            RPCTypeCheck(prevOut, map_list_of("txid", str_type)("vout", int_type)("scriptPubKey", str_type));

            uint256 txid = ParseHashO(prevOut, "txid");

            int nOut = find_value(prevOut, "vout").get_int();
            if (nOut < 0)
                throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "vout must be positive");

            vector<unsigned char> pkData(ParseHexO(prevOut, "scriptPubKey"));
            CScript scriptPubKey(pkData.begin(), pkData.end());

            CCoins coins;
            if (view.GetCoins(txid, coins)) {
                if (coins.IsAvailable(nOut) && coins.vout[nOut].scriptPubKey != scriptPubKey) {
                    string err("Previous output scriptPubKey mismatch:\n");
                    err = err + coins.vout[nOut].scriptPubKey.ToString() + "\nvs:\n"+
                        scriptPubKey.ToString();
                    throw JSONRPCError(RPC_DESERIALIZATION_ERROR, err);
                }
                // what todo if txid is known, but the actual output isn't?
            }
            if ((unsigned int)nOut >= coins.vout.size())
                coins.vout.resize(nOut+1);
            coins.vout[nOut].scriptPubKey = scriptPubKey;
            coins.vout[nOut].nValue = 0; // we don't know the actual output value
            view.SetCoins(txid, coins);

            // if redeemScript given and not using the local wallet (private keys
            // given), add redeemScript to the tempKeystore so it can be signed:
            if (fGivenKeys && scriptPubKey.IsPayToScriptHash())
            {
                RPCTypeCheck(prevOut, map_list_of("txid", str_type)("vout", int_type)("scriptPubKey", str_type)("redeemScript",str_type));
                Value v = find_value(prevOut, "redeemScript");
                if (!(v == Value::null))
                {
                    vector<unsigned char> rsData(ParseHexV(v, "redeemScript"));
                    CScript redeemScript(rsData.begin(), rsData.end());
                    tempKeystore.AddCScript(redeemScript);
                }
            }
        }
    }

    const CKeyStore& keystore = (fGivenKeys ? tempKeystore : *pwalletMain);

    int nHashType = SIGHASH_ALL;
    if (params.size() > 3 && params[3].type() != null_type)
    {
        static map<string, int> mapSigHashValues =
            boost::assign::map_list_of
            (string("ALL"), int(SIGHASH_ALL))
            (string("ALL|ANYONECANPAY"), int(SIGHASH_ALL|SIGHASH_ANYONECANPAY))
            (string("NONE"), int(SIGHASH_NONE))
            (string("NONE|ANYONECANPAY"), int(SIGHASH_NONE|SIGHASH_ANYONECANPAY))
            (string("SINGLE"), int(SIGHASH_SINGLE))
            (string("SINGLE|ANYONECANPAY"), int(SIGHASH_SINGLE|SIGHASH_ANYONECANPAY))
            ;
        string strHashType = params[3].get_str();
        if (mapSigHashValues.count(strHashType))
            nHashType = mapSigHashValues[strHashType];
        else
            throw JSONRPCError(RPC_INVALID_PARAMETER, "Invalid sighash param");
    }

    bool fHashSingle = ((nHashType & ~SIGHASH_ANYONECANPAY) == SIGHASH_SINGLE);

    // Sign what we can:
    for (unsigned int i = 0; i < mergedTx.vin.size(); i++)
    {
        CTxIn& txin = mergedTx.vin[i];
        CCoins coins;
        if (!view.GetCoins(txin.prevout.hash, coins) || !coins.IsAvailable(txin.prevout.n))
        {
            fComplete = false;
            continue;
        }
        const CScript& prevPubKey = coins.vout[txin.prevout.n].scriptPubKey;

        txin.scriptSig.clear();
        // Only sign SIGHASH_SINGLE if there's a corresponding output:
        if (!fHashSingle || (i < mergedTx.vout.size()))
            SignSignature(keystore, prevPubKey, mergedTx, i, nHashType);

        // ... and merge in other signatures:
        BOOST_FOREACH(const CTransaction& txv, txVariants)
        {
            txin.scriptSig = CombineSignatures(prevPubKey, mergedTx, i, txin.scriptSig, txv.vin[i].scriptSig);
        }
        if (!VerifyScript(txin.scriptSig, prevPubKey, mergedTx, i, SCRIPT_VERIFY_P2SH | SCRIPT_VERIFY_STRICTENC, 0))
            fComplete = false;
    }

    Object result;
    CDataStream ssTx(SER_NETWORK, PROTOCOL_VERSION);
    ssTx << mergedTx;
    result.push_back(Pair("hex", HexStr(ssTx.begin(), ssTx.end())));
    result.push_back(Pair("complete", fComplete));

    return result;
}

Value sendrawtransaction(const Array& params, bool fHelp)
{
    if (fHelp || params.size() < 1 || params.size() > 1)
        throw runtime_error(
            "sendrawtransaction <hex string>\n"
            "Submits raw transaction (serialized, hex-encoded) to local node and network.");

    // parse hex string from parameter
    vector<unsigned char> txData(ParseHexV(params[0], "parameter"));
    CDataStream ssData(txData, SER_NETWORK, PROTOCOL_VERSION);
    CTransaction tx;

    // deserialize binary data stream
    try {
        ssData >> tx;
    }
    catch (std::exception &e) {
        throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "TX decode failed");
    }
    uint256 hashTx = tx.GetHash();

    bool fHave = false;
    CCoinsViewCache &view = *pcoinsTip;
    CCoins existingCoins;
    {
        fHave = view.GetCoins(hashTx, existingCoins);
        if (!fHave) {
            // push to local node
            CValidationState state;
            if (!tx.AcceptToMemoryPool(state, true, false))
                throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "TX rejected"); // TODO: report validation state
        }
    }
    if (fHave) {
        if (existingCoins.nHeight < 1000000000)
            throw JSONRPCError(RPC_INVALID_ADDRESS_OR_KEY, "transaction already in block chain");
        // Not in block, but already in the memory pool; will drop
        // through to re-relay it.
    } else {
        SyncWithWallets(hashTx, tx, NULL, true);
    }
    RelayTransaction(tx, hashTx);

    return hashTx.GetHex();
}
