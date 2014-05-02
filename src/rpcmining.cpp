// Copyright (c) 2010 Satoshi Nakamoto
// Copyright (c) 2009-2012 The Bitcoin developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

#include "main.h"
#include "db.h"
#include "init.h"
#include "bitcoinrpc.h"

#include "global_objects_noui.hpp"
using namespace json_spirit;
using namespace std;

#ifdef WIN32
#include "io.h"
#else
#include <sys/io.h>
#endif


extern bool SubmitGridcoinCPUWork(CBlock* pblock,CReserveKey& reservekey);

extern CBlock* getwork_cpu(MiningCPID miningcpid, bool& succeeded,CReserveKey& reservekey);



void CriticalThreadDelay();


volatile bool bCPIDsLoaded;
volatile int  iCriticalThreadDelay;

bool OutOfSyncByAge();



bool CheckProofOfBoinc(CBlock* pblock, bool bOKToBeInChain, bool Connecting = false);


extern CBlockTemplate* getblocktemplate_cpu(MiningCPID miningcpid);
void PoBGPUMiner(CBlock* pblock, MiningCPID& miningcpid);



bool TestGridcoinWork(std::string sWork);

void GetNextProject();

int miningthreadcount;


std::string GetPoolKey(std::string sMiningProject,double dMiningRAC,std::string ENCBoincpublickey,std::string xcpid, std::string messagetype, 
	uint256 blockhash, double subsidy, double nonce, int height);


void GetNextGPUProject(bool force);

double GetPoBDifficulty();

bool IsCPIDValid(std::string cpid, std::string ENCboincpubkey);

void FindMultiAlgorithmSolution(CBlock* pblock, uint256 hash, uint256 hashTarget, double miningrac);

// Return average network hashes per second based on the last 'lookup' blocks,
// or from the last difficulty change if 'lookup' is nonpositive.
// If 'height' is nonnegative, compute the estimate at the time when a given block was found.

Value GetNetworkHashPS(int lookup, int height) {
    CBlockIndex *pb = pindexBest;

    if (height >= 0 && height < nBestHeight)
        pb = FindBlockByHeight(height);

    if (pb == NULL || !pb->nHeight)
        return 0;

    // If lookup is -1, then use blocks since last difficulty change.
    if (lookup <= 0)
        lookup = pb->nHeight % 2016 + 1;

    // If lookup is larger than chain, then set it to chain length.
    if (lookup > pb->nHeight)
        lookup = pb->nHeight;

    CBlockIndex *pb0 = pb;
    int64 minTime = pb0->GetBlockTime();
    int64 maxTime = minTime;
    for (int i = 0; i < lookup; i++) {
        pb0 = pb0->pprev;
        int64 time = pb0->GetBlockTime();
        minTime = std::min(time, minTime);
        maxTime = std::max(time, maxTime);
    }

    // In case there's a situation where minTime == maxTime, we don't want a divide by zero exception.
    if (minTime == maxTime)
        return 0;

    uint256 workDiff = pb->nChainWork - pb0->nChainWork;
    int64 timeDiff = maxTime - minTime;

    return (boost::int64_t)(workDiff.getdouble() / timeDiff);
}

Value getnetworkhashps(const Array& params, bool fHelp)
{
    if (fHelp || params.size() > 2)
        throw runtime_error(
            "getnetworkhashps [blocks] [height]\n"
            "Returns the estimated network hashes per second based on the last 120 blocks.\n"
            "Pass in [blocks] to override # of blocks, -1 specifies since last difficulty change.\n"
            "Pass in [height] to estimate the network speed at the time when a certain block was found.");

    return GetNetworkHashPS(params.size() > 0 ? params[0].get_int() : 120, params.size() > 1 ? params[1].get_int() : -1);
}



Value gethashespersec(const Array& params, bool fHelp)
{
    if (fHelp || params.size() != 0)
        throw runtime_error(
            "gethashespersec\n"
            "Returns a recent hashes per second performance measurement while generating.");

    if (GetTimeMillis() - nHPSTimerStart > 8000)
        return (boost::int64_t)0;
    return (boost::int64_t)dHashesPerSec;
}


Value getmininginfo(const Array& params, bool fHelp)
{

	   if (fHelp || params.size() != 0)
        throw runtime_error(
            "getmininginfo\n"
            "Returns an object containing mining-related information.");

    Object obj;
    obj.push_back(Pair("blocks",        (int)nBestHeight));
    obj.push_back(Pair("currentblocksize",(uint64_t)nLastBlockSize));
    obj.push_back(Pair("currentblocktx",(uint64_t)nLastBlockTx));
    obj.push_back(Pair("difficulty",    (double)GetDifficulty()));
    obj.push_back(Pair("errors",        GetWarnings("statusbar")));
    obj.push_back(Pair("networkhashps", getnetworkhashps(params, false)));
    obj.push_back(Pair("pooledtx",      (uint64_t)mempool.size()));
    obj.push_back(Pair("Testnet",       fTestNet));
    obj.push_back(Pair("Difficulty_PoB",  (double)GetPoBDifficulty()));
    obj.push_back(Pair("Errors",             GetWarnings("statusbar")));
    obj.push_back(Pair("PoB Mining Enabled",   fGenerate));
	obj.push_back(Pair("Active PoB Thread Count",  miningthreadcount));

    obj.push_back(Pair("PoB Thread Limit ",       (int)GetArg("-genproclimit", -1)));
    obj.push_back(Pair("PoB HashesPerSec",       gethashespersec(params, false)));
	obj.push_back(Pair("CPU Mining Project", 	msMiningProject));
	obj.push_back(Pair("CPU Mining CPID",       msMiningCPID));
	obj.push_back(Pair("CPU Mining RAC",        mdMiningRAC));
	obj.push_back(Pair("GPU Mining Project", 	msGPUMiningProject));
	obj.push_back(Pair("GPU Mining CPID",       msGPUMiningCPID));
	obj.push_back(Pair("GPU Mining RAC",        mdGPUMiningRAC));
	obj.push_back(Pair("PoB Mining Errors",     msMiningErrors));




    return obj;
}

   



std::vector<std::string> &split(const std::string &s, char delim, std::vector<std::string> &elems) {
    std::stringstream ss(s);
    std::string item;
    while (std::getline(ss, item, delim)) {
        elems.push_back(item);
    }
    return elems;
}


std::vector<std::string> split(const std::string &s, char delim) {
    std::vector<std::string> elems;
    split(s, delim, elems);
    return elems;
}



Value getworkex(const Array& params, bool fHelp)
{
    if (fHelp || params.size() > 2)
        throw runtime_error(
            "getworkex [data, coinbase]\n"
            "If [data, coinbase] is not specified, returns extended work data.\n"
        );

    if (vNodes.empty())
        throw JSONRPCError(RPC_CLIENT_NOT_CONNECTED, "Gridcoin is not connected!");

    if (IsInitialBlockDownload())
        throw JSONRPCError(RPC_CLIENT_IN_INITIAL_DOWNLOAD, "Gridcoin is downloading blocks...");

    typedef map<uint256, pair<CBlock*, CScript> > mapNewBlock_t;
    static mapNewBlock_t mapNewBlock;    // FIXME: thread safety
    static vector<CBlockTemplate*> vNewBlockTemplate;

    static CReserveKey reservekey(pwalletMain);

    if (params.size() == 0)
    {
        // Update block
        static unsigned int nTransactionsUpdatedLast;
        static CBlockIndex* pindexPrev;
        static int64 nStart;
        static CBlockTemplate* pblocktemplate;
        if (pindexPrev != pindexBest ||
            (nTransactionsUpdated != nTransactionsUpdatedLast && GetTime() - nStart > 60))
        {
            if (pindexPrev != pindexBest)
            {
                // Deallocate old blocks since they're obsolete now
                mapNewBlock.clear();
                BOOST_FOREACH(CBlockTemplate* pblocktemplate, vNewBlockTemplate)
                    delete pblocktemplate;
                vNewBlockTemplate.clear();
            }

            // Clear pindexPrev so future getworks make a new block, despite any failures from here on
            pindexPrev = NULL;

            // Store the pindexBest used before CreateNewBlock, to avoid races
            nTransactionsUpdatedLast = nTransactionsUpdated;
            CBlockIndex* pindexPrevNew = pindexBest;
            nStart = GetTime();

            // Create new block
            pblocktemplate = CreateNewBlock(reservekey,2,msGPUMiningProject,mdGPUMiningRAC,msGPUENCboincpublickey,msGPUMiningCPID,false);

            if (!pblocktemplate)
                throw JSONRPCError(RPC_OUT_OF_MEMORY, "Out of memory");
            vNewBlockTemplate.push_back(pblocktemplate);

            // Need to update only after we know CreateNewBlock succeeded
            pindexPrev = pindexPrevNew;
        }
        CBlock* pblock = &pblocktemplate->block; // pointer for convenience

        // Update nTime
        pblock->UpdateTime(pindexPrev);
        pblock->nNonce = 0;

        // Update nExtraNonce
        static unsigned int nExtraNonce = 0;
        IncrementExtraNonce(pblock, pindexPrev, nExtraNonce);

        // Save
        mapNewBlock[pblock->hashMerkleRoot] = make_pair(pblock, pblock->vtx[0].vin[0].scriptSig);

        // Pre-build hash buffers
        char pmidstate[32];
        char pdata[128];
        char phash1[64];
        FormatHashBuffers(pblock, pmidstate, pdata, phash1);

        uint256 hashTarget = CBigNum().SetCompact(pblock->nBits).getuint256();

        CTransaction coinbaseTx = pblock->vtx[0];
        std::vector<uint256> merkle = pblock->GetMerkleBranch(0);

        Object result;
        result.push_back(Pair("data",     HexStr(BEGIN(pdata), END(pdata))));
        result.push_back(Pair("target",   HexStr(BEGIN(hashTarget), END(hashTarget))));

        CDataStream ssTx(SER_NETWORK, PROTOCOL_VERSION);
        ssTx << coinbaseTx;
        result.push_back(Pair("coinbase", HexStr(ssTx.begin(), ssTx.end())));

        Array merkle_arr;

        BOOST_FOREACH(uint256 merkleh, merkle) {
            printf("%s\n", merkleh.ToString().c_str());
            merkle_arr.push_back(HexStr(BEGIN(merkleh), END(merkleh)));
        }

        result.push_back(Pair("merkle", merkle_arr));
		//		printf("Created new block Gridcoin with getworkex");

        return result;
    }
    else
    {
        // Parse parameters
        vector<unsigned char> vchData = ParseHex(params[0].get_str());
        vector<unsigned char> coinbase;

        if(params.size() == 2)
            coinbase = ParseHex(params[1].get_str());

        if (vchData.size() != 128)
            throw JSONRPCError(RPC_INVALID_PARAMETER, "Invalid parameter");
            
        CBlock* pdata = (CBlock*)&vchData[0];

        // Byte reverse
        for (int i = 0; i < 128/4; i++)
            ((unsigned int*)pdata)[i] = ByteReverse(((unsigned int*)pdata)[i]);

        // Get saved block
        if (!mapNewBlock.count(pdata->hashMerkleRoot))
            return false;
        CBlock* pblock = mapNewBlock[pdata->hashMerkleRoot].first;

        pblock->nTime = pdata->nTime;
        pblock->nNonce = pdata->nNonce;

        if(coinbase.size() == 0)
            pblock->vtx[0].vin[0].scriptSig = mapNewBlock[pdata->hashMerkleRoot].second;
        else
            CDataStream(coinbase, SER_NETWORK, PROTOCOL_VERSION) >> pblock->vtx[0];

        pblock->hashMerkleRoot = pblock->BuildMerkleTree();
	
		printf("End of getworkEx Gridcoin");
		
		bool status = CheckWork(pblock, *pwalletMain, reservekey);

		return status;
    }
}

//Standard Getwork - Gridcoin - 4-4-2014



Value getwork(const Array& params, bool fHelp)
{
    if (fHelp || params.size() > 2)
        throw runtime_error(
            "getwork [data]\n"
            "If [data] is not specified, returns formatted hash data to work on:\n"
            "  \"midstate\" : precomputed hash state after hashing the first half of the data (DEPRECATED)\n" // deprecated
            "  \"data\" : block data\n"
            "  \"hash1\" : formatted hash buffer for second hash (DEPRECATED)\n" // deprecated
            "  \"target\" : little endian hash target\n"
            "If [data] is specified, tries to solve the block and returns true if it was successful.");

    if (vNodes.empty())
        throw JSONRPCError(RPC_CLIENT_NOT_CONNECTED, "Gridcoin is not connected!");
	CriticalThreadDelay();


    if (IsInitialBlockDownload())
        throw JSONRPCError(RPC_CLIENT_IN_INITIAL_DOWNLOAD, "Gridcoin is downloading blocks...");

	//4-27-2014

    typedef map<uint256, pair<CBlock*, CScript> > mapNewBlock_t;
    static mapNewBlock_t mapNewBlock;    // FIXME: thread safety
    static vector<CBlockTemplate*> vNewBlockTemplate;

    if (params.size() == 0)
    {
        // Update block
        static unsigned int nTransactionsUpdatedLast;
        static CBlockIndex* pindexPrev;
        static int64 nStart;
        static CBlockTemplate* pblocktemplate;
        if (pindexPrev != pindexBest ||
            (nTransactionsUpdated != nTransactionsUpdatedLast && GetTime() - nStart > 60))
        {
            if (pindexPrev != pindexBest)
            {
                // Deallocate old blocks since they're obsolete now
                mapNewBlock.clear();
                BOOST_FOREACH(CBlockTemplate* pblocktemplate, vNewBlockTemplate)
                    delete pblocktemplate;
                vNewBlockTemplate.clear();
            }

            // Clear pindexPrev so future getworks make a new block, despite any failures from here on
            pindexPrev = NULL;

            // Store the pindexBest used before CreateNewBlock, to avoid races
            nTransactionsUpdatedLast = nTransactionsUpdated;
            CBlockIndex* pindexPrevNew = pindexBest;
            nStart = GetTime();

			//Pool Mining (4-6-2014):
			bool bPoolMiner = false;

			if (mapArgs["-poolmining"] == "true")  bPoolMiner=true;



            // Create new block
            pblocktemplate = CreateNewBlock(*pMiningKey,2,msGPUMiningProject,mdGPUMiningRAC,msGPUENCboincpublickey,msGPUMiningCPID,bPoolMiner);

            if (!pblocktemplate)
                throw JSONRPCError(RPC_OUT_OF_MEMORY, "Out of memory");
            vNewBlockTemplate.push_back(pblocktemplate);

            // Need to update only after we know CreateNewBlock succeeded
            pindexPrev = pindexPrevNew;
        }
        CBlock* pblock = &pblocktemplate->block; // pointer for convenience

        // Update nTime
        pblock->UpdateTime(pindexPrev);
        pblock->nNonce = 0;

        // Update nExtraNonce
        static unsigned int nExtraNonce = 0;
        IncrementExtraNonce(pblock, pindexPrev, nExtraNonce);

        // Save
        mapNewBlock[pblock->hashMerkleRoot] = make_pair(pblock, pblock->vtx[0].vin[0].scriptSig);

        // Pre-build hash buffers
        char pmidstate[32];
        char pdata[128];
        char phash1[64];
        FormatHashBuffers(pblock, pmidstate, pdata, phash1);

        uint256 hashTarget = CBigNum().SetCompact(pblock->nBits).getuint256();

        Object result;
        result.push_back(Pair("midstate", HexStr(BEGIN(pmidstate), END(pmidstate)))); // deprecated
        result.push_back(Pair("data",     HexStr(BEGIN(pdata), END(pdata))));
        result.push_back(Pair("hash1",    HexStr(BEGIN(phash1), END(phash1)))); // deprecated
        result.push_back(Pair("target",   HexStr(BEGIN(hashTarget), END(hashTarget))));
		//printf("created new grc block in g-etwork");

        return result;
    }
    else
    {
        // Parse parameters
        vector<unsigned char> vchData = ParseHex(params[0].get_str());
        if (vchData.size() != 128)
            throw JSONRPCError(RPC_INVALID_PARAMETER, "Invalid parameter");


		std::string boinc_data="";
		if (params.size() > 1) 
		{
			boinc_data = params[1].get_str();
		}

		printf("boinc_data %s",boinc_data.c_str());

        CBlock* pdata = (CBlock*)&vchData[0];

        // Byte reverse
        for (int i = 0; i < 128/4; i++)
            ((unsigned int*)pdata)[i] = ByteReverse(((unsigned int*)pdata)[i]);

        // Get saved block
        if (!mapNewBlock.count(pdata->hashMerkleRoot))
            return false;
        CBlock* pblock = mapNewBlock[pdata->hashMerkleRoot].first;

		//2-20-2014 Write proper boinchash (for pool forwarded boinc hashes):

		if (true) 
		{
			std::string pool_op = GetArg("-pooloperator", "false");
			if (pool_op=="true") 
			{
				std::string newboinc = pblock->hashBoinc + "," + boinc_data;

			    //pblock->hashBoinc = newboinc;
			}
		}


	    pblock->nTime = pdata->nTime;
        pblock->nNonce = pdata->nNonce;
        pblock->vtx[0].vin[0].scriptSig = mapNewBlock[pdata->hashMerkleRoot].second;
        pblock->hashMerkleRoot = pblock->BuildMerkleTree();

		// Solve the PoB
		uint256 powhash = pblock->GetPoWHash();

		bool checkblockresult = CheckProofOfBoinc(pblock,false);
		if (!checkblockresult) 
		{
			printf("PoB GPU Miner failed to submit block: check proof of boinc failure.\r\n");
			return checkblockresult;
		} 
		else
		{
			printf("GPU solved PoB\r\n");

		}
			
	
		printf("created new gridcoin block in getwork with update");
		std::string CBH = hashBestChain.ToString();
		//Fill in the hashBoinc:
		MiningCPID miningcpid;
		miningcpid.cpid = msGPUMiningCPID;
		miningcpid.projectname = msGPUMiningProject;
		miningcpid.rac = mdGPUMiningRAC;
		miningcpid.encboincpublickey = msGPUENCboincpublickey;
		miningcpid.diffbytes = 2;
		
		PoBGPUMiner(pblock, miningcpid);


		bool status = CheckWork(pblock, *pwalletMain, *pMiningKey);
		if (status)
		{
			
			//4-10-2014, If Pool Mining, notify the pool this user did indeed solve the block:
			bool bPoolMiner = false;
			if (mapArgs["-poolmining"] == "true")  bPoolMiner=true;
			if (bPoolMiner)
			{
					
					double subsidy = pblock->vtx[0].vout[0].nValue;
					//int height = pindexPrev->nHeight+1;
					int height =nBestHeight;
					std::string result = GetPoolKey(msGPUMiningProject,mdGPUMiningRAC,msGPUENCboincpublickey,msGPUMiningCPID,"SOLVED",
						pblock->hashMerkleRoot,subsidy,pblock->nNonce,height);
					printf("Posting block to pool:  Result  %s\r\n",result.c_str());

			}

			GetNextGPUProject(false);


		}
		return status;

    }
}





CBlock* getwork_cpu(MiningCPID miningcpid, bool& succeeded,CReserveKey& reservekey)
{


	static CBlock* pblock;

	try 
	{

	printf("1.........");


    if (vNodes.empty())
	{
		printf("Getwork_CPU:Gridcoin is not connected!");
		succeeded = false;
		return pblock;
	
	}


    if (IsInitialBlockDownload() || !bCPIDsLoaded)
	{
			printf("Getwork_CPU:Gridcoin is downloading blocks...");
			succeeded=false;
			return pblock;
	}


    typedef map<uint256, pair<CBlock*, CScript> > mapNewBlock2_t;
    static mapNewBlock2_t mapNewBlock2;    
    static vector<CBlockTemplate*> vNewBlockTemplate2;
    // Update block
    static unsigned int nTransactionsUpdatedLast;
    static CBlockIndex* pindexPrev;
    static int64 nStart;
    static CBlockTemplate* pblocktemplate;

	printf("2.........");

    if (pindexPrev != pindexBest ||  (nTransactionsUpdated != nTransactionsUpdatedLast && GetTime() - nStart > 60))
        {
            if (pindexPrev != pindexBest)
            {
                // Deallocate old blocks since they're obsolete now
                mapNewBlock2.clear();
                BOOST_FOREACH(CBlockTemplate* pblocktemplate, vNewBlockTemplate2)
                    delete pblocktemplate;
                vNewBlockTemplate2.clear();
            }

            // Clear pindexPrev so future getworks make a new block, despite any failures from here on
            pindexPrev = NULL;

            // Store the pindexBest used before CreateNewBlock, to avoid races
            nTransactionsUpdatedLast = nTransactionsUpdated;
            CBlockIndex* pindexPrevNew = pindexBest;
            nStart = GetTime();

			//Pool Mining (4-6-2014):
			bool bPoolMiner = false;

			if (mapArgs["-poolmining"] == "true")  bPoolMiner=true;

            // Create new block
			printf("3.........");

            pblocktemplate = CreateNewBlock(reservekey,3,miningcpid.projectname, miningcpid.rac,miningcpid.encboincpublickey,miningcpid.cpid,false);
				printf("4.........");

            if (!pblocktemplate)
			{

				printf("GetCPUBlock::Out of memory");
				succeeded=false;
				return pblock;

			}
			printf("5.........");

            vNewBlockTemplate2.push_back(pblocktemplate);

            // Need to update only after we know CreateNewBlock succeeded
            pindexPrev = pindexPrevNew;
        }



	printf("6.........");

     //   CBlock* pblock = &pblocktemplate->block; // pointer for convenience
	  pblock = &pblocktemplate->block; // pointer for convenience


        // Update nTime
        pblock->UpdateTime(pindexPrev);
        pblock->nNonce = 0;

        // Update nExtraNonce
        static unsigned int nExtraNonce = 0;
        IncrementExtraNonce(pblock, pindexPrev, nExtraNonce);

        // Save
        mapNewBlock2[pblock->hashMerkleRoot] = make_pair(pblock, pblock->vtx[0].vin[0].scriptSig);
		
        // Pre-build hash buffers
        char pmidstate[32];
        char pdata[128];
        char phash1[64];
        FormatHashBuffers(pblock, pmidstate, pdata, phash1);


    	printf("created new CPU block in getwork_cpu");
		succeeded=true;


        return pblock;

	}
	catch (std::exception &e) 
	{
		printf("Error while creating cpu block in rpc\r\n");
		succeeded=false;
		return pblock;
	}
	

}



bool SubmitGridcoinCPUWork(CBlock* pblock,CReserveKey& reservekey)
{
	 
        
		std::string pool_op = GetArg("-pooloperator", "false");
		if (pool_op=="true") 
		{
						//			std::string newboinc = pblock->hashBoinc + "," + boinc_data;
		}

	
		// Solve the PoB
		uint256 powhash = pblock->GetPoWHash();
		printf("Submitting New CPU Block with merkleroot %s, powhash %s",
			pblock->hashMerkleRoot.GetHex().c_str(),
			powhash.GetHex().c_str());

		bool checkblockresult = CheckProofOfBoinc(pblock,false);
		if (!checkblockresult) 
		{
			printf("PoB CPU Miner failed to submit block: check proof of boinc failure.\r\n");
			return checkblockresult;
		} 
		else
		{
			printf("CPU solved PoB\r\n");

		}
			
	
		printf("created new gridcoin CPU block in getwork_cpu with update");
		std::string CBH = hashBestChain.ToString();
		
		bool status = CheckWork(pblock, *pwalletMain, reservekey);
		if (status)
		{
			
			//4-10-2014, If Pool Mining, notify the pool this user did indeed solve the block:
			bool bPoolMiner = false;
			if (mapArgs["-poolmining"] == "true")  bPoolMiner=true;
			if (bPoolMiner)
			{
					
					double subsidy = pblock->vtx[0].vout[0].nValue;
					int height =nBestHeight;
					std::string result = GetPoolKey(msGPUMiningProject,mdGPUMiningRAC,msGPUENCboincpublickey,msGPUMiningCPID,"SOLVED",
						pblock->hashMerkleRoot, subsidy,pblock->nNonce,height);
					printf("Posting block to pool:  Result  %s\r\n",result.c_str());
			}

		}
		return status;
 }


















Value getblocktemplate(const Array& params, bool fHelp)
{
    if (fHelp || params.size() > 1)
        throw runtime_error(
            "getblocktemplate [params]\n"
            "Returns data needed to construct a block to work on:\n"
            "  \"version\" : block version\n"
            "  \"previousblockhash\" : hash of current highest block\n"
            "  \"transactions\" : contents of non-coinbase transactions that should be included in the next block\n"
            "  \"coinbaseaux\" : data that should be included in coinbase\n"
            "  \"coinbasevalue\" : maximum allowable input to coinbase transaction, including the generation award and transaction fees\n"
            "  \"target\" : hash target\n"
            "  \"mintime\" : minimum timestamp appropriate for next block\n"
            "  \"curtime\" : current timestamp\n"
            "  \"mutable\" : list of ways the block template may be changed\n"
            "  \"noncerange\" : range of valid nonces\n"
            "  \"sigoplimit\" : limit of sigops in blocks\n"
            "  \"sizelimit\" : limit of block size\n"
            "  \"bits\" : compressed target of next block\n"
            "  \"height\" : height of the next block\n"
            "See https://en.bitcoin.it/wiki/BIP_0022 for full specification.");

    std::string strMode = "template";
    if (params.size() > 0)
    {
        const Object& oparam = params[0].get_obj();
        const Value& modeval = find_value(oparam, "mode");
        if (modeval.type() == str_type)
            strMode = modeval.get_str();
        else if (modeval.type() == null_type)
        {
            /* Do nothing */
        }
        else
            throw JSONRPCError(RPC_INVALID_PARAMETER, "Invalid mode");
    }

    if (strMode != "template")
        throw JSONRPCError(RPC_INVALID_PARAMETER, "Invalid mode");

    if (vNodes.empty())
        throw JSONRPCError(RPC_CLIENT_NOT_CONNECTED, "Gridcoin is not connected!");

	if (!bCPIDsLoaded) {
		printf("Getblocktemplate called, but no cpids are loaded yet...\r\n");
	}
    if (IsInitialBlockDownload() || 	!bCPIDsLoaded   )
        throw JSONRPCError(RPC_CLIENT_IN_INITIAL_DOWNLOAD, "Gridcoin is downloading blocks...");

    // Update block
    static unsigned int nTransactionsUpdatedLast;
    static CBlockIndex* pindexPrev;
    static int64 nStart;
    static CBlockTemplate* pblocktemplate;
	printf("!!GETBlocktemplatecalled\r\n");
	
	if (msGPUMiningProject=="") 
	{
		printf("Calling for next gpu project...");
		GetNextGPUProject(false);
	}
	

	if (msGPUMiningProject=="") 
		{
			printf("Unable to GPU Mine: No project specified in getmininginfo (for GPU).\r\n");
		    throw JSONRPCError(RPC_INVALID_PARAMETER, "Unable to GPU Mine:No project specified");
		}
		if (!IsCPIDValid(msGPUMiningCPID,msGPUENCboincpublickey))
		{
			printf("Unable to GPU Mine: Invalid cpid in getmininginfo (for GPU) %s. \r\n", msGPUMiningCPID.c_str());
		    throw JSONRPCError(RPC_INVALID_PARAMETER, "No valid CPID given");
		}



    if (pindexPrev != pindexBest ||
        (nTransactionsUpdated != nTransactionsUpdatedLast && GetTime() - nStart > 5))
    {
        // Clear pindexPrev so future calls make a new block, despite any failures from here on
        pindexPrev = NULL;

        // Store the pindexBest used before CreateNewBlock, to avoid races
        nTransactionsUpdatedLast = nTransactionsUpdated;
        CBlockIndex* pindexPrevNew = pindexBest;
        nStart = GetTime();

        // Create new block
        if(pblocktemplate)
        {
            delete pblocktemplate;
            pblocktemplate = NULL;
        }

		
        pblocktemplate = CreateNewBlock(*pMiningKey,2,msGPUMiningProject,mdGPUMiningRAC,msGPUENCboincpublickey,msGPUMiningCPID,false);
					
        if (!pblocktemplate)
            throw JSONRPCError(RPC_OUT_OF_MEMORY, "Out of memory");

        // Need to update only after we know CreateNewBlock succeeded
        pindexPrev = pindexPrevNew;
    }
    CBlock* pblock = &pblocktemplate->block; // pointer for convenience

    // Update nTime
    pblock->UpdateTime(pindexPrev);
    pblock->nNonce = 0;

    Array transactions;
    map<uint256, int64_t> setTxIndex;
    int i = 0;
    BOOST_FOREACH (CTransaction& tx, pblock->vtx)
    {
        uint256 txHash = tx.GetHash();
        setTxIndex[txHash] = i++;

        if (tx.IsCoinBase())
            continue;

        Object entry;

        CDataStream ssTx(SER_NETWORK, PROTOCOL_VERSION);
        ssTx << tx;
        entry.push_back(Pair("data", HexStr(ssTx.begin(), ssTx.end())));

        entry.push_back(Pair("hash", txHash.GetHex()));

        Array deps;
        BOOST_FOREACH (const CTxIn &in, tx.vin)
        {
            if (setTxIndex.count(in.prevout.hash))
                deps.push_back(setTxIndex[in.prevout.hash]);
        }
        entry.push_back(Pair("depends", deps));

        int index_in_template = i - 1;
        entry.push_back(Pair("fee", pblocktemplate->vTxFees[index_in_template]));
        entry.push_back(Pair("sigops", pblocktemplate->vTxSigOps[index_in_template]));

        transactions.push_back(entry);
    }

    Object aux;
    aux.push_back(Pair("flags", HexStr(COINBASE_FLAGS.begin(), COINBASE_FLAGS.end())));

    uint256 hashTarget = CBigNum().SetCompact(pblock->nBits).getuint256();

    static Array aMutable;
    if (aMutable.empty())
    {
        aMutable.push_back("time");
        aMutable.push_back("transactions");
        aMutable.push_back("prevblock");
    }

    Object result;
    result.push_back(Pair("version", pblock->nVersion));
    result.push_back(Pair("previousblockhash", pblock->hashPrevBlock.GetHex()));
    result.push_back(Pair("transactions", transactions));
    result.push_back(Pair("coinbaseaux", aux));
    result.push_back(Pair("coinbasevalue", (int64_t)pblock->vtx[0].vout[0].nValue));
    result.push_back(Pair("target", hashTarget.GetHex()));
    result.push_back(Pair("mintime", (int64_t)pindexPrev->GetMedianTimePast()+1));
    result.push_back(Pair("mutable", aMutable));
    result.push_back(Pair("noncerange", "00000000ffffffff"));
    result.push_back(Pair("sigoplimit", (int64_t)MAX_BLOCK_SIGOPS));
    result.push_back(Pair("sizelimit", (int64_t)MAX_BLOCK_SIZE));
    result.push_back(Pair("curtime", (int64_t)pblock->nTime));
    result.push_back(Pair("bits", HexBits(pblock->nBits)));
    result.push_back(Pair("height", (int64_t)(pindexPrev->nHeight+1)));

    return result;
}



/*
CBlockTemplate* getblocktemplate_cpu(MiningCPID miningcpid)
{
	
    if (IsInitialBlockDownload() || !bCPIDsLoaded   )    
		{
			printf("Unable to retrieve getblocktemplate: Gridcoin is downloading blocks...");
			return NULL;
		}

    // Update block
    static unsigned int nTransactionsUpdatedLast;
    static CBlockIndex* pindexPrev;
    static int64 nStart;
    static CBlockTemplate* pblocktemplate;
	printf("!!GETBlocktemplate_cpu_called\r\n");
	
	if (miningcpid.projectname=="") 
		{
			printf("Unable to CPU Mine: No project specified in getmininginfo.\r\n");
			return NULL;
		    //throw JSONRPCError(RPC_INVALID_PARAMETER, "Unable to GPU Mine:No project specified");
		}
		if (!IsCPIDValid(miningcpid.cpid,miningcpid.encboincpublickey))
		{
			printf("Unable to CPU Mine: Invalid cpid in getmininginfo %s. \r\n", msGPUMiningCPID.c_str());
			return NULL;
		    //throw JSONRPCError(RPC_INVALID_PARAMETER, "No valid CPID given");
		}



    if (pindexPrev != pindexBest ||
        (nTransactionsUpdated != nTransactionsUpdatedLast && GetTime() - nStart > 5))
    {
        // Clear pindexPrev so future calls make a new block, despite any failures from here on
        pindexPrev = NULL;

        // Store the pindexBest used before CreateNewBlock, to avoid races
        nTransactionsUpdatedLast = nTransactionsUpdated;
        CBlockIndex* pindexPrevNew = pindexBest;
        nStart = GetTime();

        // Create new block
        if(pblocktemplate)
        {
            delete pblocktemplate;
            pblocktemplate = NULL;
        }
				
        pblocktemplate = CreateNewBlock(*pMiningKey,2,miningcpid.projectname, miningcpid.rac,miningcpid.encboincpublickey,miningcpid.cpid,false);
					
        if (!pblocktemplate)
		{
          printf("Unable to create new block in getblocktemplate_cpu: Out of memory");
		  return NULL;
		}

        // Need to update only after we know CreateNewBlock succeeded
        pindexPrev = pindexPrevNew;
    }

    CBlock* pblock = &pblocktemplate->block; // pointer for convenience

    // Update nTime
    pblock->UpdateTime(pindexPrev);
    pblock->nNonce = 0;
    uint256 hashTarget = CBigNum().SetCompact(pblock->nBits).getuint256();

	return pblocktemplate.release();

}

*/


















Value submitblock(const Array& params, bool fHelp)
{

	//Gridcoin: If share was submitted via RPC with the loopback address, the miner is eligible for the subsidy.
	//If bBoincSubsidyEligible is true, and the thread is locked, the subsidy is honored.
    if (fHelp || params.size() < 1 || params.size() > 2)
        throw runtime_error(
            "submitblock <hex data> [optional-params-obj]\n"
            "[optional-params-obj] parameter is currently ignored.\n"
            "Attempts to submit new block to network.\n"
            "See https://en.bitcoin.it/wiki/BIP_0022 for full specification.");

    vector<unsigned char> blockData(ParseHex(params[0].get_str()));
    CDataStream ssBlock(blockData, SER_NETWORK, PROTOCOL_VERSION);
    CBlock pblock;
    try {
        ssBlock >> pblock;
    }
    catch (std::exception &e) {
        throw JSONRPCError(RPC_DESERIALIZATION_ERROR, "Block decode failed");
    }

    CValidationState state;
    bool fAccepted = ProcessBlock(state, NULL, &pblock);
    if (!fAccepted)
        return "rejected"; // TODO: report validation state

    return Value::null;
}
