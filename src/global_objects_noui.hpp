#ifndef GLOBAL_OBJECTS_NOUI_HPP
#define GLOBAL_OBJECTS_NOUI_HPP

extern int nBoincUtilization;
extern std::string sBoincMD5;
extern std::string sBoincBA;
extern std::string sRegVer;
extern std::string sBoincDeltaOverTime;
extern std::string sBoincAuthenticity;
extern std::string sMinedHash;
extern std::string sSourceBlock;

extern int nRegVersion;



extern bool bBoincSubsidyEligible;
extern bool bPoolMiningMode;
extern bool bCPUMiningMode;

extern double nMegaHashProtection;
extern double nMegaHashBoincProtection;
extern double MEGAHASH_VIOLATION_THRESHHOLD;
//extern double MEGAHASH_BOINC_VIOLATION_THRESHHOLD;
extern double MEGAHASH_VIOLATION_COUNT;
extern double MEGAHASH_VIOLATION_COUNT_THRESHHOLD;

    struct MiningEntry {


        double shares;
		std::string strComment;
        std::string strAccount;
		double payout;
		double rbpps;
		double totalutilization;
		double totalrows;
		double avgboinc;
		bool   paid;
		double payments;
		double lookback;
		double difficulty;
		int blockhour;
		int wallethour;
		std::string boinchash;
		int projectid;
		std::string projectuserid;
		std::string transactionid;
		double blocknumber;
		double locktime;
		std::string projectaddress;
		double cpupowverificationtime;
		double cpupowverificationresult;
		double cpupowverificationtries;
		std::string homogenizedkey;
		std::string cpupowhash;
		double credits;
		double lastpaid;
		double lastpaiddate;
	

		double totalpayments;


		double networkcredits;
		double compensation;
	    double owed;
	    double approvedtransactions;
	    double nextpaymentamount;

		double cputotalpayments;

	
		double dummy1;
		double dummy2;
		double dummy3;

		bool approved;


    };




extern std::map<std::string, MiningEntry> minerpayments;
extern std::map<std::string, MiningEntry> cpuminerpayments;
extern std::map<std::string, MiningEntry> cpupow;
extern std::map<std::string, MiningEntry> cpuminerpaymentsconsolidated;

extern std::map<int, int> blockcache;



extern double nMinerPaymentCount;


#endif /* GLOBAL_OBJECTS_NOUI_HPP */



