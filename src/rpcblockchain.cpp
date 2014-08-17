// Copyright (c) 2010 Satoshi Nakamoto
// Copyright (c) 2009-2012 The Bitcoin developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

#include "main.h"
#include "bitcoinrpc.h"
#include <fstream>




#include "init.h" // for pwalletMain
#include <boost/lexical_cast.hpp>
#include <boost/algorithm/string/case_conv.hpp> // for to_lower()

using namespace json_spirit;
using namespace std;

int CreateRestorePoint();
int DownloadBlocks();

double GetBlockValueByHash(uint256 hash);


double cdbl(std::string s, int place);


void GetNextGPUProject(bool force);

void SetExternalValue(std::string key, bool bValue);


void ScriptPubKeyToJSON(const CScript& scriptPubKey, Object& out);

CBigNum ReturnProofOfWorkLimit(int algo);

bool GetBlockNew(uint256 blockhash, int& out_height, CBlock& blk, bool bForceDiskRead);

void ResendWalletTransactions2();
		
bool AESSkeinHash(unsigned int diffbytes, double rac, uint256 scrypthash, std::string& out_skein, std::string& out_aes512);
std::string aes_complex_hash(uint256 scrypt_hash);
std::vector<std::string> split(std::string s, std::string delim);


double Lederstrumpf(double RAC, double NetworkRAC);

int TestAESHash(double rac, unsigned int diffbytes, uint256 scrypt_hash, std::string aeshash);

extern std::string SendMultiProngedTransaction(int projectid, std::string userid);

std::string TxToString(const CTransaction& tx, const uint256 hashBlock, int64& out_amount, int64& out_locktime, int64& out_projectid, std::string& out_projectaddress, std::string& comments, std::string& out_grcaddress);
extern bool FindTransactionSlow(uint256 txhashin, CTransaction& txout,  std::string& out_errors);

bool FindBlockPos(CValidationState &state, CDiskBlockPos &pos, unsigned int nAddSize, unsigned int nHeight, uint64 nTime, bool fKnown);

extern double GetPoBDifficulty();
bool IsCPIDValid(std::string cpid, std::string ENCboincpubkey);

std::string RetrieveMd5(std::string s1);


std::string GetPoolKey(std::string sMiningProject,double dMiningRAC,std::string ENCBoincpublickey,std::string xcpid, std::string messagetype, uint256 blockhash, 
	 double subsidy, double nonce, int height, int blocktype);

std::string getfilecontents(std::string filename);


MiningCPID DeserializeBoincBlock(std::string block);

void StopGridcoin3();



std::string GridcoinHttpPost(std::string msg, std::string boincauth, std::string urlPage, bool bUseDNS);


std::string RacStringFromDiff(double RAC, unsigned int diffbytes);
void PobSleep(int milliseconds);
extern double GetNetworkAvgByProject(std::string projectname);
extern bool FindRAC(bool CheckingWork, std::string TargetCPID, std::string TargetProjectName, double pobdiff, bool bCreditNodeVerification, std::string& out_errors, int& out_position);
void HarvestCPIDs(bool cleardata);
bool TallyNetworkAverages();
void RestartGridcoin3();
std::string GetHttpPage(std::string cpid);
bool GridDecrypt(const std::vector<unsigned char>& vchCiphertext,std::vector<unsigned char>& vchPlaintext);
bool GridEncrypt(std::vector<unsigned char> vchPlaintext, std::vector<unsigned char> &vchCiphertext);



double GetPoBDifficulty()
{

	if (mvNetwork.size() < 1) 	
	{
		return 99;
	}

				StructCPID structcpid = mvNetwork["NETWORK"];
				if (!structcpid.initialized) 
				{
							return 99;
				}
				double networkrac = structcpid.rac;
				double networkavgrac = structcpid.AverageRAC;
				double networkprojects = structcpid.NetworkProjects;
				//During 14 day lookback, calculate day-blocks
				if (networkprojects == 0) networkprojects=1;
				double dayblocks = networkprojects/576;
				if (dayblocks > 14)   dayblocks=14;
				if (dayblocks < .005) 
				{
					if (fTestNet) 
					{
						dayblocks=.005;
					}
					else
					{
						dayblocks=99;
					}

				}
				mdLastPoBDifficulty = dayblocks;
				return dayblocks;
}

Value getbestblockhash(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 0)
        throw runtime_error(
            "getbestblockhash\n"
            "Returns the hash of the best (tip) block in the longest block chain.");

    return hashBestChain.GetHex();
}

double GetNetworkAvgByProject(std::string projectname)
{
	try 
	{
		if (mvNetwork.size() < 1)
		{
			return 9999;
			//Would rather prevent errors before initialization than risk re-tallying while 
			//CreateBlock() is in an unsafe threadstate.
		}
	
		StructCPID structcpid = mvNetwork[projectname];
		if (!structcpid.initialized) return 9999;

	//	double networkrac = structcpid.rac;
		double networkavgrac = structcpid.AverageRAC;
		//double networkprojects = structcpid.NetworkProjects;
		return networkavgrac;
	}
	catch (std::exception& e)
	{
			printf("Error retrieving Network Avg\r\n");
			return 9999;
	}


}




double GetDifficulty(const CBlockIndex* blockindex)
{
    // Floating point number that is a multiple of the minimum difficulty,
    // minimum difficulty = 1.0.
    if (blockindex == NULL)
    {
        if (pindexBest == NULL)
		{
			mdLastDifficulty = 1;
            return 1.0;
		}
        else
		{
            blockindex = pindexBest;
		}
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
	mdLastDifficulty = dDiff;
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
	
	//result.push_back(Pair("boinchash", block.hashBoinc));

    MiningCPID bb = DeserializeBoincBlock(block.hashBoinc);
	uint256 blockhash = block.GetPoWHash();
	std::string sblockhash = blockhash.GetHex();
	result.push_back(Pair("BlockType", block.BlockType));
	result.push_back(Pair("CPID", bb.cpid));
	result.push_back(Pair("ProjectName", bb.projectname));
	result.push_back(Pair("BlockDiffBytes", (double)bb.diffbytes));
	result.push_back(Pair("RAC", bb.rac));
	result.push_back(Pair("NetworkRAC", bb.NetworkRAC));
	//if (block.hashBoinc.length() > 10) 
	//{
		//result.push_back(Pair("BoincHash",block.hashBoinc.substr(0,100)));
	//}
	result.push_back(Pair("PoBDifficulty", bb.pobdifficulty));
	result.push_back(Pair("AES512SkeinHash", bb.aesskein));
	std::string skein2 = aes_complex_hash(blockhash);
	result.push_back(Pair("AESCalcHash",skein2));
	uint256 boincpowhash = block.hashMerkleRoot + bb.nonce;

	int iav  = TestAESHash(bb.rac, (unsigned int)bb.diffbytes, boincpowhash, bb.aesskein);
	result.push_back(Pair("AES512Valid",iav));
	result.push_back(Pair("ClientVersion",bb.clientversion));	
	std::string hbd = AdvancedDecrypt(bb.enccpid);
	bool IsCpidValid = IsCPIDValid(bb.cpid, bb.enccpid);

	result.push_back(Pair("CPIDValid",IsCpidValid));
	result.push_back(Pair("PoWHash",blockhash.GetHex()));
	//Subsidy 6-29-2014

	result.push_back(Pair("Subsidy", ValueFromAmount(GetBlockValueByHash(block.GetHash()))));

    if (blockindex->pprev)
        result.push_back(Pair("previousblockhash", blockindex->pprev->GetBlockHash().GetHex()));
    if (blockindex->pnext)
        result.push_back(Pair("nextblockhash", blockindex->pnext->GetBlockHash().GetHex()));
    return result;
}


int BoincProjectId(std::string grc) 
{
   return 0;
}



bool RetrieveTxFromBlock(const CBlock& block, const CBlockIndex* blockindex, uint256 txhashin, CTransaction& txout)
{
    CMerkleTx txGen(block.vtx[0]);
    txGen.SetMerkleBranch(&block);
    BOOST_FOREACH(const CTransaction&tx, block.vtx)
	{
	
	  if (tx.GetHash() == txhashin) 
	  {   
		  txout = tx;
		  return true;
	  }
	}
	return false;	
}









bool FindRAC(bool CheckingWork, std::string TargetCPID, std::string TargetProjectName, double pobdiff, bool bCreditNodeVerification,
	std::string& out_errors, int& out_position)
{

	try 
	{
		
					//Gridcoin; Find CPID+Project+RAC in chain
					int nMaxDepth = nBestHeight-1;

					if (nMaxDepth < 3) nMaxDepth=3;

					double pobdifficulty;
					if (bCreditNodeVerification)
					{
							pobdifficulty=14;
					}
					else
					{
							pobdifficulty = pobdiff;
					}

	

					if (pobdifficulty < .002) pobdifficulty=.002;
					int nLookback = 576*pobdifficulty; //Daily block count * Lookback in days
					int nMinDepth = nMaxDepth - nLookback;
					if (nMinDepth < 2) nMinDepth = 2;
					out_position = 0;

	
					////////////////////////////
					if (CheckingWork) nMinDepth=nMinDepth+10;
					if (nMinDepth > nBestHeight) nMinDepth=nBestHeight-1;

					////////////////////////////
					if (nMinDepth > nMaxDepth) 
					{
						nMinDepth = nMaxDepth-1;
					}
	
					if (nMaxDepth < 5 || nMinDepth < 5) return false;
	
	
					//Check the cache first:
					StructCPIDCache cache;
					std::string sKey = TargetCPID + ":" + TargetProjectName;
					cache = mvCPIDCache[sKey]; 
					double cachedblocknumber = 0;
					if (cache.initialized)
					{
						cachedblocknumber=cache.blocknumber;
					}
					if (cachedblocknumber > 0 && cachedblocknumber >= nMinDepth && cachedblocknumber <= nMaxDepth && cache.cpidproject==sKey) 
					{
	
						out_position = cache.blocknumber;
							if (CheckingWork) printf("Project %s  found at position %i   PoBLevel %f    Start depth %i     end depth %i   \r\n",
							TargetProjectName.c_str(),out_position,pobdifficulty,nMaxDepth,nMinDepth);

						return true;
					}
	
					CBlock block;
					out_errors = "";

					for (int ii = nMaxDepth; ii > nMinDepth; ii--)
					{
     					CBlockIndex* pblockindex = FindBlockByHeight(ii);
						int out_height = 0;
						bool result1 = GetBlockNew(pblockindex->GetBlockHash(), out_height, block, false);
						
						if (result1)
						{
		
				    			MiningCPID bb = DeserializeBoincBlock(block.hashBoinc);

				
								if (bb.cpid==TargetCPID && bb.projectname==TargetProjectName && block.nVersion==3)
								{
									out_position = ii;
									//Cache this:
									cache = mvCPIDCache[sKey]; 
									if (!cache.initialized)
										{
											cache.initialized = true;
											mvCPIDCache.insert(map<string,StructCPIDCache>::value_type(sKey, cache));
										}
									cache.cpid = TargetCPID;
									cache.cpidproject = sKey;
									cache.blocknumber = ii;
									if (CheckingWork) printf("Project %s  found at position %i   PoBLevel %f    Start depth %i     end depth %i   \r\n",TargetProjectName.c_str(),ii,pobdifficulty,nMaxDepth,nMinDepth);

    								mvCPIDCache[sKey]=cache;
									return true;
								}
						}

					}

					printf("Start depth %i end depth %i",nMaxDepth,nMinDepth);
					out_errors = out_errors + "Start depth " + RoundToString(nMaxDepth,0) + "; ";
					out_errors = out_errors + "Not found; ";
					return false;
		}
	catch (std::exception& e)
	{
		return false;
	}
	
}



bool FindTransactionSlow(uint256 txhashin, CTransaction& txout,  std::string& out_errors)
{
	
	int nMaxDepth = nBestHeight;
    CBlock block;
	CBlockIndex* pLastBlock = FindBlockByHeight(nMaxDepth);
	block.ReadFromDisk(pLastBlock);
	out_errors = "Scanning blockchain slow; ";

    for (int ii = nMaxDepth; ii > 1; ii--)
    {
     	CBlockIndex* pblockindex = FindBlockByHeight(ii);
		block.ReadFromDisk(pblockindex);
		bool result = RetrieveTxFromBlock(block,pblockindex,txhashin,txout);
		if (result) return true;

    }
	out_errors = out_errors + "Not found; ";

	return false;
	
}







Value verifychain(const Array& params, bool fHelp)
 {
     if (fHelp || params.size() > 2)
         throw runtime_error(
             "verifychain [check level] [num blocks]\n"
             "Verifies blockchain database.");
 
     int nCheckLevel = GetArg("-checklevel", 3);
     int nCheckDepth = GetArg("-checkblocks", 288);
     if (params.size() > 0)
         nCheckLevel = params[0].get_int();
     if (params.size() > 1)
         nCheckDepth = params[1].get_int();
 
     return VerifyDB(nCheckLevel, nCheckDepth);
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


Value showblock(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 1)
        throw runtime_error(
            "showblock <index>\n"
            "Returns all information about the block at <index>.");

    int nHeight = params[0].get_int();
    if (nHeight < 0 || nHeight > nBestHeight)
        throw runtime_error("Block number out of range.");

    CBlockIndex* pblockindex = FindBlockByHeight(nHeight);

    //return pblockindex->phashBlock->GetHex();

	//std::string strHash = pblockindex->phashBlock->GetHex().get_str();
    //uint256 hash(strHash);

    if (pblockindex==NULL)
        throw JSONRPCError(RPC_INVALID_ADDRESS_OR_KEY, "Block not found");

    CBlock block;
//    CBlockIndex* pblockindex = mapBlockIndex[hash];
    block.ReadFromDisk(pblockindex);


    return blockToJSON(block, pblockindex);

}


Value getblock(const Array& params, bool fHelp)
{
    if (fHelp || params.size() < 1 || params.size() > 2)
        throw runtime_error(
            "getblock <hash> [verbose=true]\n"
            "If verbose is false, returns a string that is serialized, hex-encoded data for block <hash>.\n"
            "If verbose is true, returns an Object with information about block <hash>."
        );

    std::string strHash = params[0].get_str();
    uint256 hash(strHash);

    bool fVerbose = true;
    if (params.size() > 1)
        fVerbose = params[1].get_bool();

    if (mapBlockIndex.count(hash) == 0)
        throw JSONRPCError(RPC_INVALID_ADDRESS_OR_KEY, "Block not found");

    CBlock block;
    CBlockIndex* pblockindex = mapBlockIndex[hash];
    block.ReadFromDisk(pblockindex);

    if (!fVerbose)
    {
        CDataStream ssBlock(SER_NETWORK, PROTOCOL_VERSION);
        ssBlock << block;
        std::string strHex = HexStr(ssBlock.begin(), ssBlock.end());
        return strHex;
    }

    return blockToJSON(block, pblockindex);
}


//Main GetBlock 

Value getblockbynumber(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 1)
        throw runtime_error(
            "getblockbynumber <hash>\n"
            "Returns details of a block with given block-hash.");

    int nHeight = params[0].get_int();
    if (nHeight < 0 || nHeight > nBestHeight)
        throw runtime_error("Block number out of range.");

    CBlockIndex* pblockindex = FindBlockByHeight(nHeight);
	CBlock block;

    block.ReadFromDisk(pblockindex);
	Object e= blockToJSON(block, pblockindex);
	return e;

}


void filecopy(FILE *dest, FILE *src)
{
    const int size = 16384;
    char buffer[size];

    while (!feof(src))
    {
        int n = fread(buffer, 1, size, src);
        fwrite(buffer, 1, n, dest);
    }

    fflush(dest);
}


void fileopen_and_copy(std::string src, std::string dest)
{
    FILE * infile  = fopen(src.c_str(),  "rb");
    FILE * outfile = fopen(dest.c_str(), "wb");

    filecopy(outfile, infile);

    fclose(infile);
    fclose(outfile);
}




	
std::string BackupGridcoinWallet()
{
	//5-1-2014

	std::string filename = "grc_" + DateTimeStrFormat("%m-%d-%Y", GetTime()) + ".dat";
	std::string filename_backup = "backup.dat";
	
	std::string standard_filename = "std_" + DateTimeStrFormat("%m-%d-%Y", GetTime()) + ".dat";
	std::string source_filename   = "wallet.dat";

	boost::filesystem::path path = GetDataDir() / "walletbackups" / filename;
	boost::filesystem::path target_path_standard = GetDataDir() / "walletbackups" / standard_filename;
	boost::filesystem::path source_path_standard = GetDataDir() / source_filename;
	boost::filesystem::path dest_path_std = GetDataDir() / "walletbackups" / filename_backup;
    boost::filesystem::create_directories(path.parent_path());
	std::string errors = "";
	//Copy the standard wallet first:
	//	fileopen_and_copy(source_path_standard.string().c_str(), target_path_standard.string().c_str());
	BackupWallet(*pwalletMain, target_path_standard.string().c_str());


	
	//Dump all private keys into the Level 2 backup
	//5-4-2014

	ofstream myBackup;

	myBackup.open (path.string().c_str());
	
    string strAccount;
	BOOST_FOREACH(const PAIRTYPE(CTxDestination, string)& item, pwalletMain->mapAddressBook)
    {
    	 const CBitcoinAddress& address = item.first;
		 const std::string& strName = item.second;
		 bool fMine = IsMine(*pwalletMain, address.Get());
		 if (fMine) 
		 {
			std::string strAddress=CBitcoinAddress(address).ToString();
			//CBitcoinAddress address;

			CKeyID keyID;
			if (!address.GetKeyID(keyID))   
			{
				errors = errors + "During wallet backup, Address does not refer to a key"+ "\r\n";

			}
			else
			{
				CKey vchSecret;
				if (!pwalletMain->GetKey(keyID, vchSecret))
				{
					errors = errors + "During Wallet Backup, Private key for address is not known " + strAddress + "\r\n";
				}
				else
				{
					std::string private_key = CBitcoinSecret(vchSecret).ToString();
					//Append to file
					std::string strAddr = CBitcoinAddress(keyID).ToString();
					std::string record = private_key + "<|>" + strAddr + "<KEY>";
					myBackup << record;

				}
			}

		 }
    }

	
	std::string reserve_keys = pwalletMain->GetAllGridcoinKeys();
	myBackup << reserve_keys;


	/*
	std::map<CKeyID, int64_t> mapKeyBirth;
	std::set<CKeyID> setKeyPool;
	pwalletMain->GetKeyBirthTimes(mapKeyBirth);
	pwalletMain->GetAllReserveKeys(setKeyPool);
	  // sort time/key pairs 
	std::vector<std::pair<int64_t, CKeyID> > vKeyBirth; 
	for (std::map<CKeyID, int64_t>::const_iterator it = mapKeyBirth.begin(); it != mapKeyBirth.end(); it++) 
	{
		 vKeyBirth.push_back(std::make_pair(it->second, it->first));
    } 
	    mapKeyBirth.clear();
		std::sort(vKeyBirth.begin(), vKeyBirth.end());
		// produce output  
	 for (std::vector<std::pair<int64_t, CKeyID> >::const_iterator it = vKeyBirth.begin(); it != vKeyBirth.end(); it++) 
	 {
		    const CKeyID &keyid = it->second; 
			std::string strAddr = CBitcoinAddress(keyid).ToString();
			CKey key;    //private
				
		    if (pwalletMain->GetKey(keyid, key)) 
			{
					std::string private_key = CBitcoinSecret(key).ToString();
					//Append to file
					std::string strAddr = CBitcoinAddress(keyid).ToString();
					std::string record = private_key + "<|>" + strAddr + "<KEY>";
					myBackup << record;
			}

	 }

	 */


	myBackup.close();
	//Bitcoin:
	//Copy grc backup to backup.dat:
	fileopen_and_copy(path.string().c_str(),dest_path_std.string().c_str());


	return errors;


}





std::string RestoreGridcoinBackupWallet()
{
	//AdvancedBackup
	//AdvancedSalvage
	//4-26-2014



	boost::filesystem::path path = GetDataDir() / "walletbackups" / "backup.dat";
	std::string errors = "";
	std::string sWallet = getfilecontents(path.string().c_str());
	if (sWallet == "-1") return "Unable to open backup file.";
		
    string strSecret = "from file";
    string strLabel = "Restored";

	std::vector<std::string> vWallet = split(sWallet.c_str(),"<KEY>");
	if (vWallet.size() > 1)
	{
 	    for (unsigned int i = 0; i < vWallet.size(); i++)
		{
			std::string sKey = vWallet[i];
			if (sKey.length() > 2)
			{
					printf("Restoring private key %s",sKey.substr(0,5).c_str());
					//Key is delimited by <|>
					std::vector<std::string> vKey = split(sKey.c_str(),"<|>");
					if (vKey.size() > 1)
					{
							std::string sSecret = vKey[0];
							std::string sPublic = vKey[1];
							CBitcoinSecret vchSecret;
							bool fGood = vchSecret.SetString(sSecret);
							if (!fGood)
							{
								errors = errors + "Invalid private key : " + sSecret + "\r\n";
							}
							else
							{
								 CKey key = vchSecret.GetKey();
								 CPubKey pubkey = key.GetPubKey();
								
								 CKeyID vchAddress = pubkey.GetID();
								 {
									 LOCK2(cs_main, pwalletMain->cs_wallet);
							 
									 if (!pwalletMain->AddKeyPubKey(key, pubkey)) 
									 {
										 errors = errors + "Error adding key to wallet: " + sKey + "\r\n";
									 }

									 if (i==0)
									 {
										pwalletMain->SetDefaultKey(pubkey);
										pwalletMain->SetAddressBookName(vchAddress, strLabel);
									 }
							 		 pwalletMain->MarkDirty();
							
      
								 }
							}
					}
			}

		}

	}


	//Rescan
	{
		   LOCK2(cs_main, pwalletMain->cs_wallet);
		    if (true) {
				pwalletMain->ScanForWalletTransactions(pindexGenesisBlock, true);
				pwalletMain->ReacceptWalletTransactions();
			}
     
	}

	printf("Rebuilding wallet, results: %s",errors.c_str());
    return errors;

}


uint256 Skein(std::string sInput)
{
	uint256 uiSkein = GridcoinMultipleAlgoHash(BEGIN(sInput), END(sInput));
	return uiSkein;
}	



Value execute(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 1)
        throw runtime_error(
		"execute <string::itemname>\n"
        "Executes an arbitrary command by name.");

    std::string sItem = params[0].get_str();

	if (sItem=="") throw runtime_error("Item invalid.");

    Array results;
	Object oOut;
	oOut.push_back(Pair("Command",sItem));
	results.push_back(oOut);
    Object entry;
		
	if (sItem == "restorepoint")
	{
			int r=-1;
			#if defined(WIN32) && defined(QT_GUI)
			//We must stop the node before we can do this
			r = CreateRestorePoint();
			RestartGridcoin3();

			#endif 
			entry.push_back(Pair("Restore Point",r));
			results.push_back(entry);
	}
	else if (sItem == "downloadblocks")
	{
			int r=-1;
			#if defined(WIN32) && defined(QT_GUI)
				r = DownloadBlocks();
			#endif 
			entry.push_back(Pair("Download Blocks",r));
			results.push_back(entry);
	}
	else if (sItem == "tally")
	{
			TallyNetworkAverages();
			entry.push_back(Pair("Tally Network Averages",1));
			results.push_back(entry);
	}
	else if (sItem == "resetcpids")
	{
			mvCPIDCache.clear();
    	    HarvestCPIDs(true);
		    entry.push_back(Pair("Reset",1));
			results.push_back(entry);
	}
	else if (sItem == "backupwallet")
	{
			std::string result = BackupGridcoinWallet();
		    entry.push_back(Pair("Backup Wallet Result", result));
		    results.push_back(entry);
	}
	else if (sItem == "restorewallet")
	{
			std::string result = RestoreGridcoinBackupWallet();
			entry.push_back(Pair("Restore Wallet Result", result));
			results.push_back(entry);
	}
	else if (sItem == "resendwallettx")
	{
			ResendWalletTransactions2();
			entry.push_back(Pair("Resending unsent wallet transactions...",1));
			results.push_back(entry);
	} 
	else if (sItem == "postcpid")
	{
			std::string result = GetHttpPage("859038ff4a9",true);
			entry.push_back(Pair("POST Result",result));
	        results.push_back(entry);
	}
	else if (sItem == "encrypt")
	{
			std::string s1 = "1234";
			std::string s1dec = AdvancedCrypt(s1);
			std::string s1out = AdvancedDecrypt(s1dec);
		    entry.push_back(Pair("Execute Encrypt result1",s1));
		    entry.push_back(Pair("Execute Encrypt result2",s1dec));
			entry.push_back(Pair("Execute Encrypt result3",s1out));
			results.push_back(entry);
	}
	else if (sItem == "restartnet")
	{
			printf("Restarting gridcoin's network layer;");
			RestartGridcoin3();
			entry.push_back(Pair("Execute","Restarted Gridcoins network layer."));
	   		results.push_back(entry);
	}
	else if (sItem == "findrac")
	{
			int position = 0;
			std::string out_errors = "";
	    	std::string TargetCPID = "123";
			std::string TargetProjectName="Docking";
			bool result = FindRAC(false,TargetCPID, TargetProjectName, 1, false,out_errors, position);
			entry.push_back(Pair("TargetCPID",TargetCPID));
			entry.push_back(Pair("Errors",out_errors));
		   	results.push_back(entry);
	}
	else if (sItem=="genminingkey")
	{
		 //7-3-2014
		 std::string grc = DefaultWalletAddress();
		 GetNextGPUProject(true);
		 bool IsCpidValid = IsCPIDValid(msGPUMiningCPID, msGPUENCboincpublickey);
		 if (!IsCpidValid)
		 {
			entry.push_back(Pair("Errors","Failed to retrieve boinc CPID"));

		 }
		 std::string bpk = AdvancedDecrypt(msGPUENCboincpublickey);
		 std::string bpmd5 = RetrieveMd5(bpk);
		 std::string concatminingkey = grc + ";" + bpk;
    	 //entry.push_back(Pair("BPK",bpk));
		 //entry.push_back(Pair("md5",bpmd5));
		 //entry.push_back(Pair("concat",concatminingkey));
	 	 std::string miningkey = EncodeBase64(concatminingkey);
		 entry.push_back(Pair("miningkey",miningkey));
         results.push_back(entry);
	
	}
	else
	{
			entry.push_back(Pair("Command " + sItem + " not found.",-1));
			results.push_back(entry);
	}
	return results;    
		
}





Value listitem(const Array& params, bool fHelp)
{
    if (fHelp || (params.size() != 1  && params.size() != 2))
        throw runtime_error(
		"listitem <string::itemname>\n"
        "Returns details of a given item by name.");

    std::string sitem = params[0].get_str();
	
	std::string args = "";
	if (params.size()==2)
	{
		args=params[1].get_str();
	}
	
	if (sitem=="") throw runtime_error("Item invalid.");

    Array results;
	Object e2;
	e2.push_back(Pair("Command",sitem));
	results.push_back(e2);






	if (sitem == "projects") 
	{

		for(map<string,StructCPID>::iterator ii=mvBoincProjects.begin(); ii!=mvBoincProjects.end(); ++ii) 
		{

			StructCPID structcpid = mvBoincProjects[(*ii).first];

	        if (structcpid.initialized) 
			{ 
				Object entry;
				entry.push_back(Pair("Project",structcpid.projectname));
				entry.push_back(Pair("URL",structcpid.link));
				results.push_back(entry);

			}
		}
		return results;

	}

	if (sitem == "leder")
	{
		
		double subsidy = Lederstrumpf(1000,1000);
		Object entry;
		entry.push_back(Pair("Subsidy",subsidy));
		if (args.length() > 1)
		{
			double myrac=cdbl(args,0);
			subsidy = Lederstrumpf(myrac,1000);
			entry.push_back(Pair("Subsidy",subsidy));//7-19-2014
		}
		results.push_back(entry);

	}




	if (sitem == "network") 
	{
		//4-9-2014

		for(map<string,StructCPID>::iterator ii=mvNetwork.begin(); ii!=mvNetwork.end(); ++ii) 
		{

			StructCPID structcpid = mvNetwork[(*ii).first];

	        if (structcpid.initialized) 
			{ 
				Object entry;
				entry.push_back(Pair("Project",structcpid.projectname));
				entry.push_back(Pair("RAC",structcpid.rac));
				entry.push_back(Pair("Avg RAC",structcpid.AverageRAC));

				entry.push_back(Pair("Entries",structcpid.entries));

				
				if (structcpid.projectname=="NETWORK") 
				{
						entry.push_back(Pair("Network Projects",structcpid.NetworkProjects));

				}
				results.push_back(entry);

			}
		}
		return results;

	}






	if (sitem=="cpids") {
		//Dump vectors:
		
		if (mvCPIDs.size() < 1) 
		{
			HarvestCPIDs(false);
		}
		printf ("generating cpid report %s",sitem.c_str());


		for(map<string,StructCPID>::iterator ii=mvCPIDs.begin(); ii!=mvCPIDs.end(); ++ii) 
		{

			StructCPID structcpid = mvCPIDs[(*ii).first];

	        if (structcpid.initialized) 
			{ 
				Object entry;
	
				entry.push_back(Pair("Project",structcpid.projectname));
				entry.push_back(Pair("CPID",structcpid.cpid));
				entry.push_back(Pair("CPIDhash",structcpid.cpidhash));
				entry.push_back(Pair("Email",structcpid.emailhash));
				entry.push_back(Pair("UTC",structcpid.utc));
				entry.push_back(Pair("RAC",structcpid.rac));
				entry.push_back(Pair("Team",structcpid.team));
				entry.push_back(Pair("RecTime",structcpid.rectime));
				entry.push_back(Pair("Age",structcpid.age));
				entry.push_back(Pair("Verified UTC",structcpid.verifiedutc));
				entry.push_back(Pair("Verified RAC",structcpid.verifiedrac));
				entry.push_back(Pair("Verified Team",structcpid.verifiedteam));
				entry.push_back(Pair("Verified RecTime",structcpid.verifiedrectime));
				entry.push_back(Pair("Verified RAC Age",structcpid.verifiedage));
				entry.push_back(Pair("Is my CPID Valid?",structcpid.Iscpidvalid));
				
				entry.push_back(Pair("CPID Link",structcpid.link));
				entry.push_back(Pair("Errors",structcpid.errors));


				results.push_back(entry);

			}
		}


    }

    return results;


		
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


