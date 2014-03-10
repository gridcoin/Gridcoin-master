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


#ifdef WIN32
#include <QAxObject>
#include <ActiveQt/qaxbase.h>
#include <ActiveQt/qaxobject.h>
#endif


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
int CheckCPUWork(std::string lastblockhash, std::string greatblockhash, std::string greatgrandparentsblockhash, std::string greatgreatgrandparentsblockhash, std::string boinchash, bool bUseRPC);
int CheckCPUWorkByBlock(int blocknumber,bool bUseRPC);
int UpgradeClient();
extern std::string TxToString(const CTransaction& tx, const uint256 hashBlock, int64& out_amount, int64& out_locktime, int64& out_projectid, std::string& out_projectaddress, std::string& out_comments, std::string& out_grcaddress);
extern double TxPaidToCPUMiner(const CTransaction& tx, int nBlock, std::string address, double& out_total, std::string& out_comments);
extern std::map<std::string, MiningEntry> CalculateCPUMining();
extern void SendGridcoinProjectBeacons();
int CheckCPUWorkLinux(std::string lastblockhash, std::string greatblockhash, std::string greatgrandparentsblockhash, 
	std::string greatgreatgrandparentsblockhash, std::string boinchash);



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
	o = grc1 + "," + boost::lexical_cast<string>(amountwallet) + "," + grc2 + "," + boost::lexical_cast<string>(amountproject) + "," + grcSENDER;
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

	std::string mygrcaddress = DefaultWalletAddress();


	for (unsigned int i = 0; i < tx.vout.size(); i++)
    {
        const CTxOut& txout = tx.vout[i];
		bCPUTx = false;
		
		std::string sPaid = FormatMoney(txout.nValue);
			
		double paid = lexical_cast<double>(sPaid);
        
			//std::string sPaid = RoundToString(paid,10);
		std::string suffix = "";
		grc1 = PubKeyToGRCAddress(txout.scriptPubKey);
	

		if (sPaid.length() > 4 && paid > 0 && grc1.length() > 5) {
			suffix = sPaid.substr(sPaid.length()-4,4);
			if (suffix == "7900" || suffix=="0117" || suffix == "0079" ) bCPUTx = true;


			if (grc1==mygrcaddress && bCPUTx) {
			 	//printf("MyGrc Amount %s and suffix %s",sPaid.c_str(),suffix.c_str());
			
			 }


		}


		if (bCPUTx) 
		{
	       
			 	MiningEntry me1 = cpuminerpaymentsconsolidated[grc1];

			if (!me1.paid) 
				{
					cpuminerpaymentsconsolidated.insert(map<string,MiningEntry>::value_type(grc1,me1));
					me1.paid=true;
					me1.cputotalpayments=0;
					cpuminerpaymentsconsolidated[grc1] = me1;
		 		}
   	 	   
				me1.cputotalpayments = me1.cputotalpayments + paid;
			try	
			{	
				me1.lastpaiddate =tx.nLockTime;
			} catch(...) 
			{ }
            cpuminerpaymentsconsolidated[grc1]=me1;
			//printf("Created cputx for %s",sPaid.c_str());


		}


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
	
	if (false) {
	///////////////////////////////////////////////////////////////////////////////////
	//Gridcoin - return block info 
	int64 out_amount = 0;
	int64 out_locktime = 0;
	int64 nProjId = 0;
	std::string sProjectAddress = "";
	std::string comments = "";
	std::string grc_address = "";
	std::string o1 = TxToString(tx, hashBlock, out_amount, out_locktime, nProjId, sProjectAddress, comments, grc_address);
	result.push_back(Pair("GRCAddress", grc_address));
	result.push_back(Pair("GRCAmount",out_amount));
	result.push_back(Pair("Comments",comments));
	//////////////////////////////////////////////////////////////////////////////
	}
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
		    printf("Send Project Beacon payment failure.");
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





std::map<std::string, MiningEntry> CalculatePoolMining(bool bPayDuringWalletHour)
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
//	printf("Reaching payout to miners.");
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
											
											int64 currenttime = GetTime();

											int currenthour = boost::lexical_cast<int>(DateTimeStrFormat("%H", currenttime));
				     						int wallethour = HourFromGRCAddress(wallet);
				                            if (  (bPayDuringWalletHour && 
												(wallethour==currenthour || wallethour==currenthour-1 || wallethour==currenthour-2 || wallethour==currenthour+1 || wallethour==currenthour+2)
												)    ||  !bPayDuringWalletHour)
											{


											  MiningEntry meNew;
											  compensated_rows++;
											  string blocktime =  DateTimeStrFormat("%Y-%m-%d %H:%M:%S", pblockindex->GetBlockTime()).c_str();
									 		  int blockhour = boost::lexical_cast<int>(DateTimeStrFormat("%H", pblockindex->GetBlockTime()).c_str());
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
											  total_shares = total_shares + iBU;
											}
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
		return minerpayments;
    
}







std::map<std::string, MiningEntry> CalculateCPUMining()
{


	return cpuminerpayments;


	int nMaxDepth = nBestHeight;
    CBlock block;
	CBlockIndex* pLastBlock = FindBlockByHeight(nMaxDepth);
	block.ReadFromDisk(pLastBlock);
	int64 LastBlockTime = pLastBlock->GetBlockTime();
	
	double diff = GetDifficulty(pLastBlock);
	if (diff < .05) diff = .05;

    double lookback = 24 * 60 * 60 * 3;
	//Gridcoin: 2-21-2014 : Changing lookback to 7 days:

	double total_utilization = 0;
	double total_rows = 0;
    double avg_boinc = 0;
    string wallet = "";
	double maxperuser = 50;

	cpuminerpayments.clear();
	cpuminerpaymentsconsolidated.clear();

	double compensated_rows = 0;
	string last_wallet = "";
	double total_payment = 0;
	double iBU = 0;
	double compensation = 0;

	//printf("Reaching calculatecpumining.");
	double total_shares = 0;
	
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
	double iapproved=0;
	double network_credits = 0;
	double coverage_total = 0;
	double coverage_count = 0;
	double coverage_percent = 0;
	//2-21-2014:

	for(map<string,MiningEntry>::iterator ii=cpuminerpayments.begin(); ii!=cpuminerpayments.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpayments[(*ii).first];
			MiningEntry pow = cpupow[ae.homogenizedkey];
			
			if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2) 
			{
			    coverage_total++;
			}
			if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2 && pow.cpupowverificationtries > 0) 
			{
				coverage_count++;
     		}
	        if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2 && pow.cpupowverificationtries > 0 && pow.cpupowverificationresult > 0) 
			{
				//1-14-2014 consolidate mining payments
				MiningEntry me1;
				me1 = cpuminerpaymentsconsolidated[ae.strAccount];
         	    
				if (me1.paid != true) 
				{ 
					cpuminerpaymentsconsolidated.insert(map<string,MiningEntry>::value_type(ae.strAccount,me1));
					me1.paid=true;
					me1.credits=0;
					me1.networkcredits=0;
					cpuminerpaymentsconsolidated[ae.strAccount] = me1;
		 		}
	            me1.strAccount = ae.strAccount;
				me1.credits = me1.credits + pow.cpupowverificationresult;
	            network_credits = network_credits+me1.credits;		    
				//printf("Increasing network credits by %d",network_credits);

			    cpuminerpaymentsconsolidated[ae.strAccount] = me1;
			}
			
    }


	
	
	coverage_total = 400;
	if (coverage_count < 1) coverage_count=1;

	coverage_percent = coverage_count/coverage_total;
	printf("cov total %f pct %f\r\n",coverage_total,coverage_percent);

	if (coverage_percent > 1) coverage_percent=1;

	//Gridcoin Add total shares to cpuminerpaymentsconsolidated:
	//Max Weekly subsidy : Changing to Weekly due to bloated wallet issues: Gridcoin - 2-21-2014:
	double max_daily_subsidy = 576*150*3;  //576 cpu-miners @ 150 grc per day.
	double rbpps = max_daily_subsidy/(network_credits+.001);
	//Note: As of 2-21-2014, we are up to 1,600,000 boinc mined credits per day; so lets set up a max rbpps so no mistake can be made regarding overpayments:
	if (rbpps > .2) rbpps=.2;

	//Total everything


	for(map<string,MiningEntry>::iterator ii2=cpuminerpaymentsconsolidated.begin(); ii2!=cpuminerpaymentsconsolidated.end(); ++ii2) 
	{

			MiningEntry ae2 = cpuminerpaymentsconsolidated[(*ii2).first];
			MiningEntry pow2 = cpupow[ae2.homogenizedkey];
	        if (ae2.strAccount.length() > 5 && ae2.projectuserid.length() > 2 && pow2.cpupowverificationtries > 0 && pow2.cpupowverificationresult > 0) 
			{

			 				iapproved++;			

			}
	}

		printf("End of CPUMiner Totals");
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	


	try {
	
		
		
		double iApproved2 = 0;

		
		for(map<string,MiningEntry>::iterator ii=cpuminerpayments.begin(); ii!=cpuminerpayments.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpayments[(*ii).first];
			MiningEntry pow = cpupow[ae.homogenizedkey];

			
	        if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2 && pow.cpupowverificationtries > 0 && pow.cpupowverificationresult > 0) 
			{
				
 	                MiningEntry me1;
 				    me1 = cpuminerpaymentsconsolidated[ae.strAccount];
					me1.networkcredits = network_credits;
        			me1.rbpps = rbpps;
					double compensation = me1.rbpps * me1.credits;
					if (compensation > maxperuser) compensation = maxperuser;
					me1.compensation = compensation;
					//2-10-2014

					double owed = compensation - me1.cputotalpayments;
					if (owed < 0) owed = 0;
					if (owed > maxperuser) owed=maxperuser;  //Weekly
					me1.owed = owed;
					me1.approvedtransactions = iapproved;
					//2-21-2014 Weekly Max calculation

					double next_payment_amount = (CPU_MAXIMUM_BLOCK_PAYMENT_AMOUNT/(iapproved+.01))*coverage_percent;
					if (next_payment_amount > owed) next_payment_amount = owed;
					me1.nextpaymentamount = next_payment_amount;
					cpuminerpaymentsconsolidated[ae.strAccount] = me1;
			}

    }



		
		for(map<string,MiningEntry>::iterator ii=cpuminerpaymentsconsolidated.begin(); ii!=cpuminerpaymentsconsolidated.end(); ++ii) 
		{

			MiningEntry me1 = cpuminerpaymentsconsolidated[(*ii).first];
	
	        if (me1.nextpaymentamount > 1) 
			{
				
	    			iApproved2++;

			}
		}


	//2-10-2014 Remove possibility of overpayments:
    

	for(map<string,MiningEntry>::iterator ii=cpuminerpayments.begin(); ii!=cpuminerpayments.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpayments[(*ii).first];
			MiningEntry pow = cpupow[ae.homogenizedkey];
	        MiningEntry me1 = cpuminerpaymentsconsolidated[ae.strAccount];
	

			if (pow.cpupowverificationtries==0) {
					me1.rbpps = rbpps;
					double compensation = me1.rbpps * me1.credits;
					if (compensation > maxperuser) compensation = maxperuser;
					me1.compensation = compensation;
					double owed = compensation - me1.cputotalpayments;
					if (compensation > 5 && me1.cputotalpayments > 5 && owed < 1) {
						//2-21-2014
						pow.cpupowverificationtries=1;
						cpupow[ae.homogenizedkey]=pow;
					}

			}




	        if (me1.nextpaymentamount > 1) 
			{
				
	        		me1.networkcredits = network_credits;
        			me1.rbpps = rbpps;
					double compensation = me1.rbpps * me1.credits;
					if (compensation > maxperuser) compensation = maxperuser;
					me1.compensation = compensation;
				    double owed = compensation - me1.cputotalpayments;
					if (owed < 0) owed = 0;
					if (owed > 450) owed=450;
					me1.owed = owed;
					me1.approvedtransactions = iApproved2;
					double next_payment_amount = (CPU_MAXIMUM_BLOCK_PAYMENT_AMOUNT/(iApproved2+.01))*coverage_percent;
					if (next_payment_amount > owed) next_payment_amount = owed;
					me1.nextpaymentamount = next_payment_amount;
					cpuminerpaymentsconsolidated[ae.strAccount] = me1;
			}

    }

		

	} catch(...)

	{

		printf("Error in calculate cpu mining");

	}


		

	return cpuminerpayments;

  }





void SendGridcoinProjectBeacons()
{
	printf("Sending beacons:");

    
	std::map<std::string, MiningEntry> cpumap = CalculateCPUMining();

	if (!GetCPUMiningMode()) {
		printf("CPU mining disabled; no beacons to send.");
		return;
	}

	//For each Gridcoin project in the lookback period, send a project beacon
	MiningEntry me;
	
	int64 bal = pwalletMain->GetBalance();
	std::string sPaid = FormatMoney(bal);
	double dbal = lexical_cast<double>(sPaid);
    printf("Balance %f",dbal);
	if (dbal < .50) {
		printf("Not enough money to send beacons.");
		return;
	}

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
				printf("Transaction %d previously sent.",i);
			}
		} else
		{
				printf("User not participating in project %d.",i);
		
		}

	}

}







bool strreplace(std::string& str, const std::string& from, const std::string& to) {
    size_t start_pos = str.find(from);
    if(start_pos == std::string::npos)
        return false;
    str.replace(start_pos, from.length(), to);
    return true;
}





bool IsBackslash(char c) 
{  
	int asc = (int)c;
	if (asc == 92) return true;
	return false;
} 

std::string ConvBS(std::string s) 
{

	char ch;
	std::string sOut = "";
	for (unsigned int i=0;i < s.length(); i++) 
	{
		ch = s.at(i);
		if (IsBackslash(ch)) {
			sOut = sOut + "~";
		}
		else
			{
				sOut = sOut + ch;
		}

	}
	return sOut;
}




Value checkwork(const Array& params, bool fHelp)
{
	//RPCcheckwork 1-26-2014

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
	pBlock = FindBlockByHeight(blocknumber-4);
	block.ReadFromDisk(pBlock);
	std::string blockhash4 = pBlock->phashBlock->GetHex().c_str();

	printf("Lastblockhash %s",blockhash1.c_str());
	printf("Lastblockhash %s",blockhash2.c_str());
	printf("Lastblockhash %s",blockhash3.c_str());
	printf("Lastblockhash %s",blockhash4.c_str());
    printf("Boinchash %s",boinchash.c_str());
	entry.push_back(Pair("Last Block Hash",blockhash1));
	entry.push_back(Pair("Prior Block Hash",blockhash2));
	entry.push_back(Pair("Great Block Hash",blockhash3));
	entry.push_back(Pair("Great Great Block Hash",blockhash4));

	}
	catch (...) {
		return -20;
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
#ifdef WIN32
		result = UpgradeClient();
#endif
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

	minerpayments = CalculatePoolMining(false);
    
    int inum = 0;
   
    double rbpps = minerpayments["totals"].rbpps;
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
    double total_payments=0;
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

    cpuminerpayments.clear();
	cpuminerpayments = CalculateCPUMining();
	    
    int inum = 0;
   
    double rbpps = cpuminerpayments["totals"].rbpps;
    double total_payments = 0;
	Object entry;
	
	entry.push_back(Pair("CPU Credit Details Report Version",1.03));
	entry.push_back(Pair("Difficulty",cpuminerpayments["totals"].difficulty));
    entry.push_back(Pair("Lookback Period", cpuminerpayments["totals"].lookback));

	results.push_back(entry);
	std::string row = "";
	row = "#,Account,Project#,Verified RAC,Verification Tries,Total Payments";
	results.push_back(row);

	for(map<string,MiningEntry>::iterator ii=cpuminerpayments.begin(); ii!=cpuminerpayments.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpayments[(*ii).first];

	        if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2) 
			{
		 		inum++;
				MiningEntry CPU = cpupow[ae.homogenizedkey];
				row = RoundToString(inum,0) + "," + ae.strAccount + ", " + RoundToString(ae.projectid,0) + ", " 
					+ RoundToString(CPU.cpupowverificationresult,0) + ", " 
					+ RoundToString(CPU.cpupowverificationtries,0) + ", " + RoundToString(ae.cputotalpayments,2);
		
	     		results.push_back(row);
			}

    }



	Object e2;

	//1-23-2014
	//////////////////////////////////////////////////////////////////////

	//Emit Consolidated report
	e2.push_back(Pair("Consolidated CPU Credit Report Version",1.7));
	results.push_back(e2);
	double gtp = 0;
	row = "#,Account,Verified RAC,Total Payments,Earned,Network Credits,RBPPS,Owed,Next Payment Amount";
	results.push_back(row);

	inum=0;
	for(map<string,MiningEntry>::iterator ii=cpuminerpaymentsconsolidated.begin(); ii!=cpuminerpaymentsconsolidated.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpaymentsconsolidated[(*ii).first];

	        if (ae.strAccount.length() > 5) 
			{ 
	      		inum++;
				gtp=gtp+ae.compensation;
				
				row = RoundToString(inum,0) + "," + ae.strAccount + ", " + RoundToString(ae.credits,0) + ", " 
					+ RoundToString(ae.cputotalpayments,2) + ", " 
					+ RoundToString(ae.compensation,2) + ", " + RoundToString(ae.networkcredits,2) + ", " + RoundToString(ae.rbpps,4) 
					+ ", " + RoundToString(ae.owed,4) + ", " + RoundToString(ae.nextpaymentamount,2);
	     		results.push_back(row);
			}

    }

	/////////////////////////////////// EMIT UNPAID CPUMINER REPORT

	
	//Emit Consolidated report
	e2.push_back(Pair("Consolidated CPU - Unpaid CPU Miner Report",1.6));
	results.push_back(e2);
	double gtp2 = 0;

	inum=0;
	for(map<string,MiningEntry>::iterator ii=cpuminerpaymentsconsolidated.begin(); ii!=cpuminerpaymentsconsolidated.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpaymentsconsolidated[(*ii).first];

	        if (ae.strAccount.length() > 5 && ae.nextpaymentamount > .99) 
			{ 
				double compensation = ae.shares*rbpps;
	     		Object e3;
				inum++;
				e3.push_back(Pair("CPU Miner #",inum));
				e3.push_back(Pair("Payment Comment",ae.strComment));
				e3.push_back(Pair("GRC Address",ae.strAccount));
				e3.push_back(Pair("Total RAC",RoundToString(ae.credits,2)));
				e3.push_back(Pair("Payments",RoundToString(ae.cputotalpayments,4)));
				e3.push_back(Pair("Earned",RoundToString(ae.compensation,4)));
				gtp2=gtp2+ae.compensation;
				e3.push_back(Pair("Network CPUMined RAC",RoundToString(ae.networkcredits,2)));
				e3.push_back(Pair("RBPPBAC",RoundToString(ae.rbpps,4)));
				e3.push_back(Pair("Owed",RoundToString(ae.owed,4)));
				e3.push_back(Pair("Next Payment Amount",RoundToString(ae.nextpaymentamount,2)));
				//	e3.push_back(Pair("Last Paid",ae.lastpaiddate));
				e3.push_back(Pair("Approved CPU-Mining Pool Payment Count",ae.approvedtransactions));

	     		results.push_back(e3);
			}

    }


	
	//////////////////////////////////////////////////////

		Object e5;
		e5.push_back(Pair("Grand Total Payments",RoundToString(gtp,6)));
		results.push_back(e5);
        return results;

}























Value listmycpuminers(const Array& params, bool fHelp)
{

	
	printf("Creating mycpu miner payout report...");
		if (fHelp || params.size() > 3)
        throw runtime_error(
            "listminers [minconf=1] [maxconf=9999999] \n"
            "Returns array of pool mining transactions\n"
            "{}");

	RPCTypeCheck(params, list_of(int_type)(int_type)(array_type));
	Array results;

   cpuminerpayments.clear();
	cpuminerpayments = CalculateCPUMining();
	    
    int inum = 0;
   
    double rbpps = cpuminerpayments["totals"].rbpps;
    Object entry;
	
	entry.push_back(Pair("CPU Credit Details Report Version",1.02));
	entry.push_back(Pair("Difficulty",cpuminerpayments["totals"].difficulty));
    entry.push_back(Pair("Lookback Period", cpuminerpayments["totals"].lookback));

	results.push_back(entry);
	//2-3-2014
    std::string mygrcaddress = DefaultWalletAddress();

	for(map<string,MiningEntry>::iterator ii=cpuminerpayments.begin(); ii!=cpuminerpayments.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpayments[(*ii).first];

	        if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2 && ae.strAccount==mygrcaddress) 
			{
				double compensation = ae.shares*rbpps;
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
				e.push_back(Pair("Total Payments",ae.cputotalpayments));
			
				e.push_back(Pair("Block #",ae.blocknumber));
				e.push_back(Pair("TX ID",ae.transactionid));
			
				e.push_back(Pair("Locktime",ae.locktime));

	     		results.push_back(e);
			}

    }



	Object e2;

	//1-23-2014
	//////////////////////////////////////////////////////////////////////

	//Emit Consolidated report
	e2.push_back(Pair("Consolidated CPU Credit Report Version",1.5));
	results.push_back(e2);
	double gtp = 0;

	inum=0;
	for(map<string,MiningEntry>::iterator ii=cpuminerpaymentsconsolidated.begin(); ii!=cpuminerpaymentsconsolidated.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpaymentsconsolidated[(*ii).first];

	        if (ae.strAccount.length() > 5 && ae.strAccount==mygrcaddress) 
			{ 
				double compensation = ae.shares*rbpps;
	     		Object e33;
				inum++;
				e33.push_back(Pair("CPU Miner #",inum));
				e33.push_back(Pair("Payment Comment",ae.strComment));
				e33.push_back(Pair("GRC Address",ae.strAccount));

				e33.push_back(Pair("Total Avg Daily Credits",RoundToString(ae.credits,2)));
				e33.push_back(Pair("Total Daily CPU Payments to your GRC Address",RoundToString(ae.cputotalpayments,4)));
				e33.push_back(Pair("Amount Earned",RoundToString(ae.compensation,4)));
				gtp=gtp+ae.compensation;

				e33.push_back(Pair("Network CPUMined Boinc Credits",RoundToString(ae.networkcredits,2)));
				e33.push_back(Pair("RBPPBAC Payment Per Boinc Avg Credit",RoundToString(ae.rbpps,4)));
				e33.push_back(Pair("Outstanding Owed Amount",RoundToString(ae.owed,4)));
				e33.push_back(Pair("Next Payment Amount",RoundToString(ae.nextpaymentamount,2)));
				e33.push_back(Pair("Last Paid",ae.lastpaiddate));
			
				e33.push_back(Pair("Approved CPU-Mining Pool Payment Count",ae.approvedtransactions));


	     		results.push_back(e33);
			}

    }

	/////////////////////////////////// EMIT UNPAID CPUMINER REPORT

	
	//Emit Consolidated report
	e2.push_back(Pair("Consolidated CPU - Unpaid CPU Miner Report",1.4));
	results.push_back(e2);
	double gtp2 = 0;

	inum=0;
	for(map<string,MiningEntry>::iterator ii=cpuminerpaymentsconsolidated.begin(); ii!=cpuminerpaymentsconsolidated.end(); ++ii) 
	{

			MiningEntry ae = cpuminerpaymentsconsolidated[(*ii).first];

	        if (ae.strAccount.length() > 5 && ae.nextpaymentamount > .05 && ae.strAccount==mygrcaddress) 
			{ 
				Object e3;
				inum++;
				e3.push_back(Pair("CPU Miner #",inum));
				e3.push_back(Pair("Payment Comment",ae.strComment));
				e3.push_back(Pair("GRC Address",ae.strAccount));

				e3.push_back(Pair("Total Avg Daily Credits",RoundToString(ae.credits,2)));
				e3.push_back(Pair("Total Daily CPU Payments to your GRC Address",RoundToString(ae.cputotalpayments,4)));
				e3.push_back(Pair("Amount Earned",RoundToString(ae.compensation,4)));
				gtp2=gtp2+ae.compensation;

				e3.push_back(Pair("Network CPUMined Boinc Credits",RoundToString(ae.networkcredits,2)));
				e3.push_back(Pair("RBPPBAC Payment Per Boinc Avg Credit",RoundToString(ae.rbpps,4)));
				e3.push_back(Pair("Outstanding Owed Amount",RoundToString(ae.owed,4)));
				e3.push_back(Pair("Next Payment Amount",RoundToString(ae.nextpaymentamount,2)));
					e3.push_back(Pair("Last Paid",ae.lastpaiddate));
			
				e3.push_back(Pair("Approved CPU-Mining Pool Payment Count",ae.approvedtransactions));


	     		results.push_back(e3);
			}

    }


	
	//////////////////////////////////////////////////////

		Object e5;
		e5.push_back(Pair("Grand Total Payments",RoundToString(gtp,6)));
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
