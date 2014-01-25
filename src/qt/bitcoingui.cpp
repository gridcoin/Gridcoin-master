/*
 * Qt4 bitcoin GUI.
 *
 * W.J. van der Laan 2011-2012
 * The Bitcoin Developers 2011-2012
 */

#include <QApplication>
#include <QAxObject>
#include <ActiveQt/qaxbase.h>
#include <ActiveQt/qaxobject.h>


#include <QProcess>

#include "bitcoingui.h"
#include "..\global_objects.hpp"
#include "..\global_objects_noui.hpp"

#include "transactiontablemodel.h"
#include "optionsdialog.h"
#include "aboutdialog.h"
#include "miningdialog.h"
#include "clientmodel.h"
#include "walletmodel.h"
#include "walletframe.h"
#include "optionsmodel.h"
#include "transactiondescdialog.h"
#include "bitcoinunits.h"
#include "guiconstants.h"
#include "notificator.h"
#include "guiutil.h"
#include "rpcconsole.h"
#include "ui_interface.h"
#include "wallet.h"
#include "init.h"


#include <boost/lexical_cast.hpp>

#ifdef Q_OS_MAC
#include "macdockiconhandler.h"
#endif

#include <QMenuBar>
#include <QMenu>
#include <QIcon>
#include <QVBoxLayout>
#include <QToolBar>
#include <QStatusBar>
#include <QLabel>
#include <QMessageBox>
#include <QProgressBar>
#include <QStackedWidget>
#include <QDateTime>
#include <QMovie>
#include <QTimer>
#include <QDragEnterEvent>
#if QT_VERSION < 0x050000
#include <QUrl>
#endif
#include <QMimeData>
#include <QStyle>
#include <QSettings>
#include <QDesktopWidget>
#include <QListWidget>
#include "bitcoinrpc.h"

#include <iostream>

using namespace std;

const QString BitcoinGUI::DEFAULT_WALLET = "~Default";
int nTick = 0;
int nTickRestart = 0;
int nBlockCount = 0;
int nTick2 = 0;
int nRegVersion;

extern void SendGridcoinProjectBeacons();
std::string NodesToString();

extern int UpgradeClient();
extern int CheckCPUWorkByCurrentBlock(std::string boinchash, int nBlockHeight);
extern int CloseGuiMiner();
void RestartGridcoin3();

std::map<std::string, MiningEntry> CalculateCPUMining();


json_spirit::Value getwork(const json_spirit::Array& params, bool fHelp);
bool TestGridcoinWork(std::string sWork);


extern int CheckCPUWorkLinux(std::string lastblockhash, std::string greatblockhash, std::string greatgrandparentsblockhash, 	std::string greatgreatgrandparentsblockhash, std::string boinchash);



std::string GetGridcoinWork();
extern int RestartClient();
extern int ReindexWallet();

void FlushGridcoinBlockFile(bool fFinalize);

extern int ReindexBlocks();
bool FindBlockPos(CValidationState &state, CDiskBlockPos &pos, unsigned int nAddSize, unsigned int nHeight, uint64 nTime, bool fKnown);

QAxObject *globalcom;

int cputick = 0;

	
BitcoinGUI::BitcoinGUI(QWidget *parent) :
    QMainWindow(parent),
    clientModel(0),
    encryptWalletAction(0),
    changePassphraseAction(0),
    aboutQtAction(0),
    trayIcon(0),
    notificator(0),
    rpcConsole(0),
    prevBlocks(0)
{
    restoreWindowGeometry();
    setWindowTitle(tr("Gridcoin") + " - " + tr("Wallet"));
#ifndef Q_OS_MAC
    QApplication::setWindowIcon(QIcon(":icons/bitcoin"));
    setWindowIcon(QIcon(":icons/bitcoin"));
#else
    setUnifiedTitleAndToolBarOnMac(true);
    QApplication::setAttribute(Qt::AA_DontShowIconsInMenus);
#endif
    // Create wallet frame and make it the central widget
    walletFrame = new WalletFrame(this);
    setCentralWidget(walletFrame);

    // Accept D&D of URIs
    setAcceptDrops(true);

    // Create actions for the toolbar, menu bar and tray/dock icon
    // Needs walletFrame to be initialized
    createActions();

    // Create application menu bar
    createMenuBar();

    // Create the toolbars
    createToolBars();

    // Create system tray icon and notification
    createTrayIcon();

    // Create status bar
    statusBar();

    // Status bar notification icons
    QFrame *frameBlocks = new QFrame();
    frameBlocks->setContentsMargins(0,0,0,0);
    frameBlocks->setMinimumWidth(56);
    frameBlocks->setMaximumWidth(56);
    QHBoxLayout *frameBlocksLayout = new QHBoxLayout(frameBlocks);
    frameBlocksLayout->setContentsMargins(3,0,3,0);
    frameBlocksLayout->setSpacing(3);
    labelEncryptionIcon = new QLabel();
    labelConnectionsIcon = new QLabel();
    labelBlocksIcon = new QLabel();
    frameBlocksLayout->addStretch();
    frameBlocksLayout->addWidget(labelEncryptionIcon);
    frameBlocksLayout->addStretch();
    frameBlocksLayout->addWidget(labelConnectionsIcon);
    frameBlocksLayout->addStretch();
    frameBlocksLayout->addWidget(labelBlocksIcon);
    frameBlocksLayout->addStretch();

    // Progress bar and label for blocks download
    progressBarLabel = new QLabel();
    progressBarLabel->setVisible(false);
    progressBar = new QProgressBar();
    progressBar->setAlignment(Qt::AlignCenter);
    progressBar->setVisible(false);

    // Override style sheet for progress bar for styles that have a segmented progress bar,
    // as they make the text unreadable (workaround for issue #1071)
    // See https://qt-project.org/doc/qt-4.8/gallery.html
    QString curStyle = QApplication::style()->metaObject()->className();
    if(curStyle == "QWindowsStyle" || curStyle == "QWindowsXPStyle")
    {
        progressBar->setStyleSheet("QProgressBar { background-color: #08e8e8; border: 1px solid grey; border-radius: 7px; padding: 1px; text-align: center; } QProgressBar::chunk { background: QLinearGradient(x1: 0, y1: 0, x2: 1, y2: 0, stop: 0 #FF8000, stop: 1 orange); border-radius: 7px; margin: 0px; }");
    }

    statusBar()->addWidget(progressBarLabel);
    statusBar()->addWidget(progressBar);
    statusBar()->addPermanentWidget(frameBlocks);

    syncIconMovie = new QMovie(":/movies/update_spinner", "mng", this);

    rpcConsole = new RPCConsole(this);
    connect(openRPCConsoleAction, SIGNAL(triggered()), rpcConsole, SLOT(show()));

    // Install event filter to be able to catch status tip events (QEvent::StatusTip)
    this->installEventFilter(this);
}

BitcoinGUI::~BitcoinGUI()
{
    saveWindowGeometry();
    if(trayIcon) // Hide tray icon, as deleting will let it linger until quit (on Ubuntu)
        trayIcon->hide();
#ifdef Q_OS_MAC
    delete appMenuBar;
    MacDockIconHandler::instance()->setMainWindow(NULL);
#endif
}




extern  int CheckCPUWork(std::string lastblockhash, std::string greatblockhash, std::string greatgrandparentsblockhash, std::string greatgreatgrandparentsblockhash, std::string boinchash);

extern  int CheckCPUWorkByBlock(int blocknumber);








int CheckCPUWorkLinux(std::string lastblockhash, std::string greatblockhash, std::string greatgrandparentsblockhash, 
	std::string greatgreatgrandparentsblockhash, std::string boinchash)
{
	int result = 0;
	std::string v1 = lastblockhash + "[COL]" + greatblockhash + "[COL]" + greatgrandparentsblockhash + "[COL]" + greatgreatgrandparentsblockhash + "[COL]" + boinchash;

	//QString h1 = QString::fromUtf8(lastblockhash.c_str()); 
	//QString h2 = QString::fromUtf8(greatblockhash.c_str());
	//QString h3 = QString::fromUtf8(greatgrandparentsblockhash.c_str());
	//QString h4 = QString::fromUtf8(greatgreatgrandparentsblockhash.c_str());
    //QString h5 = QString::fromUtf8(boinchash.c_str());
	//Gridcoin start new instance;
	QString h1 = QString::fromUtf8(v1.c_str());
	
	printf("checkworklinux %s",v1.c_str());

	//QString r1 =  globalcom->dynamicCall("CheckWorkLinux2(QString,QString,QString,QString,QString)",h1,h1,h1,h1,h1).toInt();
	//	result =  globalcom->dynamicCall("CheckWorkLinux(QString)",h1).toInt();
	int iRegVer = 0;
	iRegVer = globalcom->dynamicCall("Version()").toInt();
	sRegVer = boost::lexical_cast<std::string>(iRegVer);
	
	printf("Reg Version %s",sRegVer.c_str());


//   QString qsblock = QString::fromUtf8(lastblockhash.c_str());
	//	globalcom->dynamicCall("SetLinux10(Qstring)",qsblock);
	//	result  = globalcom->dynamicCall("Linux10()").toInt();




	//QString ba = ba_1.toString();
		//sBoincBA = ba.toUtf8().constData();

//	printf("baa %s",sBoincBA.c_str());

	return 0;
}

















int CheckCPUWork(std::string lastblockhash, std::string greatblockhash, std::string greatgrandparentsblockhash, 
	std::string greatgreatgrandparentsblockhash, std::string boinchash)
{
	//+1 Valid
    //-1 CPU Hash does not contain gridcoin block hash
    //-2 CPU Source Hash Invalid
    //-10 Boinc Hash Invalid
    //-11 Boinc Hash Present but invalid
    //-12 MD5 Error
    //-14 Rehashed output error
    //-15 CPU hash does not match SHA computed hash
    //-16 General Error
	int result = 0;
	QString h1 = QString::fromUtf8(lastblockhash.c_str()); 
	QString h2 = QString::fromUtf8(greatblockhash.c_str());
	QString h3 = QString::fromUtf8(greatgrandparentsblockhash.c_str());
	QString h4 = QString::fromUtf8(greatgreatgrandparentsblockhash.c_str());

    QString h5 = QString::fromUtf8(boinchash.c_str());
	//Gridcoin : ToDo: Ensure Linux can CheckWork  1-25-2014:
	printf("RegVersion %d",nRegVersion);
	if (nRegVersion < 25) 
	{
		printf("Linux:Overriding");
			return 1;

	}

	//Gridcoin start new instance;
	if (globalcom==NULL) {
			printf("globalcom==null");
			      MilliSleep(5000);
			if (globalcom==NULL) {
				printf("globalcom still == null");
				MilliSleep(5000);
				if (globalcom==NULL) {
							printf("globalcom still == null II");
							MilliSleep(5000);
				}
			}
			//globalcom = new QAxObject("Boinc.Utilization");	
	}
	if (globalcom==NULL) return -30;

	try {
		result =  globalcom->dynamicCall("CheckWork(QString,QString,QString,QString,QString)",h1,h2,h3,h4,h5).toInt();
	} catch(...) 
	{

		printf("CheckWorkByBlock Failure.");
		result = -31;
	}
	return result;
}



std::string Trim(int64 i)
{
	std::string s = "";
	s=boost::lexical_cast<std::string>(i);
	return s;
}


std::string TrimD(double i)
{
	std::string s = "";
	s=boost::lexical_cast<std::string>(i);
	return s;
}


bool IsInvalidChar(char c) 
{  
	int asc = (int)c;

	if (asc >= 0 && asc < 32) return true;
	if (asc > 128) return true;
	if (asc == 124) return true;
	return false;
} 
std::string Clean(std::string s) 
{

	char ch;
	std::string sOut = "";
	for (int i=0;i < s.length(); i++) 
	{
		ch = s.at(i);
		if (IsInvalidChar(ch)==false) sOut = sOut + ch;

	}
	return sOut;


}

std::string RetrieveBlockAsString(int lSqlBlock)
{

	//Insert into Blocks (hash,confirmations,size,height,version,merkleroot,tx,time,nonce,bits,difficulty,boinchash,previousblockhash,nextblockhash) VALUES ();
    
	try {
	
		if (lSqlBlock==0) lSqlBlock=1;
		if (lSqlBlock > nBestHeight-2) return "";
		CBlock block;
		CBlockIndex* blockindex = FindBlockByHeight(lSqlBlock);
		block.ReadFromDisk(blockindex);

		std::string s = "";
		std::string d = "|";

	s = block.GetHash().GetHex() + d + "C" + d 	+ Trim(GetSerializeSize(block, SER_NETWORK, PROTOCOL_VERSION)) + d 	+ Trim(blockindex->nHeight) + d;
	s = s + Trim(block.nVersion) + d + block.hashMerkleRoot.GetHex() + d + "TX" + d + Trim(block.GetBlockTime()) + d + Trim(block.nNonce) + d + Trim(block.nBits) + d;
	s = s + TrimD(GetDifficulty(blockindex)) + d
		+		Clean(block.hashBoinc) + d 
		+ blockindex->pprev->GetBlockHash().GetHex() + d;
	s = s + blockindex->pnext->GetBlockHash().GetHex();
		return s;
	}
	catch(...)
	{
		printf("Runtime error in RetrieveBlockAsString");
		return "";

	}

}



std::string RetrieveBlocksAsString(int lSqlBlock)
{
	std::string sout = "";
	if (lSqlBlock > nBestHeight-5) return "";

	for (int i = lSqlBlock; i < lSqlBlock+101; i++) {
		sout = sout + RetrieveBlockAsString(i) + "{ROW}";
	}
	return sout;
}





int CheckCPUWorkByBlock(int blocknumber)
{
	try {
	//Blocks newer than BestHeight-100 must be checked:
    if (blocknumber < nBestHeight-100) return 1;
	//Blocks before 26150 (Dec 4, 2013) are grandfathered in (boinchash did not implement CPU mining before this date):
	if (blocknumber < 42250) return 1;
    CBlock block;
	CBlockIndex* pBlock = FindBlockByHeight(blocknumber);
	block.ReadFromDisk(pBlock);
	std::string boinchash = block.hashBoinc.c_str();
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


	int result = 0;
	result = CheckCPUWork(blockhash1,blockhash2,blockhash3,blockhash4,boinchash);
	return result;
	} 
    	catch (std::exception &e) 
	{
			return -20; //General Error: CheckCPUWorkByBlock Fails
    }

}


bool OutOfSync() 
{
	//12-25-2013

	if (  (GetNumBlocksOfPeers() > nBestHeight+3)  ||  (nBestHeight > GetNumBlocksOfPeers()+3) ) return true;
	if ( fReindex || fImporting || IsInitialBlockDownload() ) return true;
	return false;

}


int CheckCPUWorkByCurrentBlock(std::string boinchash, int nBlockHeight)
{
	try {

		if (OutOfSync() )
		{
			printf("Checkcpuworkbycurrentblock:OutOfSync=true best height %d   numofblocks %d",nBestHeight,GetNumBlocksOfPeers());
			return 1;
    	}
    
	if (nBlockHeight < nBestHeight-100) return 1;
	//Blocks before 26150 (Dec 4, 2013) are grandfathered in (boinchash did not implement CPU mining before this date):
	//Adding (12-23-2013) Added AcceptBlock enforcement at block 36850
	if (!fTestNet && nBlockHeight < 42250) return 1;
    if (fTestNet  && nBlockHeight < 806  ) return 1;
	  
	CBlock block;
	CBlockIndex* pBlock = FindBlockByHeight(nBlockHeight-1);
	block.ReadFromDisk(pBlock);
	std::string blockhash1 = pBlock->phashBlock->GetHex().c_str();
	pBlock = FindBlockByHeight(nBlockHeight-1);
	block.ReadFromDisk(pBlock);
	std::string blockhash2 = pBlock->phashBlock->GetHex().c_str();
	
	pBlock = FindBlockByHeight(nBlockHeight-2);
	block.ReadFromDisk(pBlock);
	std::string blockhash3 = pBlock->phashBlock->GetHex().c_str();

	pBlock = FindBlockByHeight(nBlockHeight-3);
	block.ReadFromDisk(pBlock);
	std::string blockhash4 = pBlock->phashBlock->GetHex().c_str();
	//1-25-2014


	int result = 0;
	result = CheckCPUWork(blockhash1,blockhash2,blockhash3,blockhash4,boinchash);
	if (result != 0) {
		    if (OutOfSync() )
			{
				printf("Checkcpuworkbycurrentblock:OutOfSync=true2 best height %d   numofblocks %d",nBestHeight,GetNumBlocksOfPeers());
				return 1;
			}

			printf("Checkcpuworkbycurrentblock:Error - best height %d   numofblocks %d",nBestHeight,GetNumBlocksOfPeers());
	}
	return result;
	} 
   
	catch(std::runtime_error &e) 
	{
		printf("General Checkcpuworkbyblock failure.");
			return -20; //General Error: CheckCPUWorkByBlock Fails
    }

}





int ReindexWallet()
{
	printf("executing grcrestarter reindex");

			QString sFilename = "GRCRestarter.exe";
			QString sArgument = "";
			QString path = QCoreApplication::applicationDirPath() + "\\" + sFilename;
			QProcess p;
			globalcom->dynamicCall("ReindexWallet()");
			StartShutdown();
			return 1;
}



int RestartClient()
{
	
	printf("executing grcrestarter restart");

	QString sFilename = "GRCRestarter.exe";
			QString sArgument = "";
			QString path = QCoreApplication::applicationDirPath() + "\\" + sFilename;
			QProcess p;
			globalcom->dynamicCall("RestartWallet()");
			StartShutdown();
			return 1;
}


int UpgradeClient()
{
	printf("Executing upgrade");

			QString sFilename = "GRCRestarter.exe";
			QString sArgument = "upgrade";
			QString path = QCoreApplication::applicationDirPath() + "\\" + sFilename;
			QProcess p;
			globalcom->dynamicCall("UpgradeWallet()");
			StartShutdown();
			return 1;
}

int CloseGuiMiner()
{
	try {
	globalcom->dynamicCall("CloseGUIMiner()");
	} catch(...) { return 0; } 

	return 1;
}

void BitcoinGUI::createActions()
{
	

    QActionGroup *tabGroup = new QActionGroup(this);

    overviewAction = new QAction(QIcon(":/icons/overview"), tr("&Overview"), this);
    overviewAction->setStatusTip(tr("Show general overview of wallet"));
    overviewAction->setToolTip(overviewAction->statusTip());
    overviewAction->setCheckable(true);
    overviewAction->setShortcut(QKeySequence(Qt::ALT + Qt::Key_1));
    tabGroup->addAction(overviewAction);

    sendCoinsAction = new QAction(QIcon(":/icons/send"), tr("&Send"), this);
    sendCoinsAction->setStatusTip(tr("Send coins to a Gridcoin address"));
    sendCoinsAction->setToolTip(sendCoinsAction->statusTip());
    sendCoinsAction->setCheckable(true);
    sendCoinsAction->setShortcut(QKeySequence(Qt::ALT + Qt::Key_2));
    tabGroup->addAction(sendCoinsAction);

    receiveCoinsAction = new QAction(QIcon(":/icons/receiving_addresses"), tr("&Receive"), this);
    receiveCoinsAction->setStatusTip(tr("Show the list of addresses for receiving payments"));
    receiveCoinsAction->setToolTip(receiveCoinsAction->statusTip());
    receiveCoinsAction->setCheckable(true);
    receiveCoinsAction->setShortcut(QKeySequence(Qt::ALT + Qt::Key_3));
    tabGroup->addAction(receiveCoinsAction);

    historyAction = new QAction(QIcon(":/icons/history"), tr("&Transactions"), this);
    historyAction->setStatusTip(tr("Browse transaction history"));
    historyAction->setToolTip(historyAction->statusTip());
    historyAction->setCheckable(true);
    historyAction->setShortcut(QKeySequence(Qt::ALT + Qt::Key_4));
    tabGroup->addAction(historyAction);

    addressBookAction = new QAction(QIcon(":/icons/address-book"), tr("&Addresses"), this);
    addressBookAction->setStatusTip(tr("Edit the list of stored addresses and labels"));
    addressBookAction->setToolTip(addressBookAction->statusTip());
    addressBookAction->setCheckable(true);
    addressBookAction->setShortcut(QKeySequence(Qt::ALT + Qt::Key_5));
    tabGroup->addAction(addressBookAction);

    connect(overviewAction, SIGNAL(triggered()), this, SLOT(showNormalIfMinimized()));
    connect(overviewAction, SIGNAL(triggered()), this, SLOT(gotoOverviewPage()));
    connect(sendCoinsAction, SIGNAL(triggered()), this, SLOT(showNormalIfMinimized()));
    connect(sendCoinsAction, SIGNAL(triggered()), this, SLOT(gotoSendCoinsPage()));
    connect(receiveCoinsAction, SIGNAL(triggered()), this, SLOT(showNormalIfMinimized()));
    connect(receiveCoinsAction, SIGNAL(triggered()), this, SLOT(gotoReceiveCoinsPage()));
    connect(historyAction, SIGNAL(triggered()), this, SLOT(showNormalIfMinimized()));
    connect(historyAction, SIGNAL(triggered()), this, SLOT(gotoHistoryPage()));
    connect(addressBookAction, SIGNAL(triggered()), this, SLOT(showNormalIfMinimized()));
    connect(addressBookAction, SIGNAL(triggered()), this, SLOT(gotoAddressBookPage()));

    quitAction = new QAction(QIcon(":/icons/quit"), tr("E&xit"), this);
    quitAction->setStatusTip(tr("Quit application"));
    quitAction->setShortcut(QKeySequence(Qt::CTRL + Qt::Key_Q));
    quitAction->setMenuRole(QAction::QuitRole);

	
	rebuildAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Rebuild Block Chain"), this);
	rebuildAction->setStatusTip(tr("Rebuild Block Chain"));
	rebuildAction->setMenuRole(QAction::TextHeuristicRole);



    aboutAction = new QAction(QIcon(":/icons/bitcoin"), tr("&About Gridcoin"), this);
    aboutAction->setStatusTip(tr("Show information about Gridcoin"));
    aboutAction->setMenuRole(QAction::AboutRole);
	
	miningAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Mining Console"), this);
	miningAction->setStatusTip(tr("Go to the mining console"));
	miningAction->setMenuRole(QAction::TextHeuristicRole);

	projectsAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Projects Console"), this);
	projectsAction->setStatusTip(tr("Go to the projects console"));
	projectsAction->setMenuRole(QAction::TextHeuristicRole);



	emailAction = new QAction(QIcon(":/icons/bitcoin"), tr("&E-Mail Center"), this);
	emailAction->setStatusTip(tr("Go to the E-Mail center"));
	emailAction->setMenuRole(QAction::TextHeuristicRole);

	sqlAction = new QAction(QIcon(":/icons/bitcoin"), tr("&SQL Query Analyzer"), this);
	sqlAction->setStatusTip(tr("SQL Query Analyzer"));
	sqlAction->setMenuRole(QAction::TextHeuristicRole);

	leaderboardAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Leaderboard"), this);
	leaderboardAction->setStatusTip(tr("Leaderboard"));
	leaderboardAction->setMenuRole(QAction::TextHeuristicRole);

    aboutQtAction = new QAction(QIcon(":/trolltech/qmessagebox/images/qtlogo-64.png"), tr("About &Qt"), this);
    aboutQtAction->setStatusTip(tr("Show information about Qt"));
    aboutQtAction->setMenuRole(QAction::AboutQtRole);

    optionsAction = new QAction(QIcon(":/icons/options"), tr("&Options..."), this);
    optionsAction->setStatusTip(tr("Modify configuration options for Gridcoin"));
    optionsAction->setMenuRole(QAction::PreferencesRole);
    toggleHideAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Show / Hide"), this);
    toggleHideAction->setStatusTip(tr("Show or hide the main Window"));

    encryptWalletAction = new QAction(QIcon(":/icons/lock_closed"), tr("&Encrypt Wallet..."), this);
    encryptWalletAction->setStatusTip(tr("Encrypt the private keys that belong to your wallet"));
    encryptWalletAction->setCheckable(true);
    backupWalletAction = new QAction(QIcon(":/icons/filesave"), tr("&Backup Wallet..."), this);
    backupWalletAction->setStatusTip(tr("Backup wallet to another location"));
    changePassphraseAction = new QAction(QIcon(":/icons/key"), tr("&Change Passphrase..."), this);
    changePassphraseAction->setStatusTip(tr("Change the passphrase used for wallet encryption"));
    signMessageAction = new QAction(QIcon(":/icons/edit"), tr("Sign &message..."), this);
    signMessageAction->setStatusTip(tr("Sign messages with your Gridcoin addresses to prove you own them"));
    verifyMessageAction = new QAction(QIcon(":/icons/transaction_0"), tr("&Verify message..."), this);
    verifyMessageAction->setStatusTip(tr("Verify messages to ensure they were signed with specified Gridcoin addresses"));

    openRPCConsoleAction = new QAction(QIcon(":/icons/debugwindow"), tr("&Debug window"), this);
    openRPCConsoleAction->setStatusTip(tr("Open debugging and diagnostic console"));

    connect(quitAction, SIGNAL(triggered()), qApp, SLOT(quit()));
    connect(aboutAction, SIGNAL(triggered()), this, SLOT(aboutClicked()));
    connect(aboutQtAction, SIGNAL(triggered()), qApp, SLOT(aboutQt()));
    connect(optionsAction, SIGNAL(triggered()), this, SLOT(optionsClicked()));
    connect(toggleHideAction, SIGNAL(triggered()), this, SLOT(toggleHidden()));
    connect(encryptWalletAction, SIGNAL(triggered(bool)), walletFrame, SLOT(encryptWallet(bool)));
    connect(backupWalletAction, SIGNAL(triggered()), walletFrame, SLOT(backupWallet()));
    connect(changePassphraseAction, SIGNAL(triggered()), walletFrame, SLOT(changePassphrase()));
    connect(signMessageAction, SIGNAL(triggered()), this, SLOT(gotoSignMessageTab()));
    connect(verifyMessageAction, SIGNAL(triggered()), this, SLOT(gotoVerifyMessageTab()));
	connect(miningAction, SIGNAL(triggered()), this, SLOT(miningClicked()));
	connect(emailAction, SIGNAL(triggered()), this, SLOT(emailClicked()));
	connect(projectsAction, SIGNAL(triggered()), this, SLOT(projectsClicked()));
	connect(rebuildAction, SIGNAL(triggered()), this, SLOT(rebuildClicked()));
	connect(sqlAction, SIGNAL(triggered()), this, SLOT(sqlClicked()));
	connect(leaderboardAction, SIGNAL(triggered()), this, SLOT(leaderboardClicked()));
	
}

void BitcoinGUI::createMenuBar()
{
#ifdef Q_OS_MAC
    // Create a decoupled menu bar on Mac which stays even if the window is closed
    appMenuBar = new QMenuBar();
#else
    // Get the main window's menu bar on other platforms
    appMenuBar = menuBar();
#endif

    // Configure the menus
    QMenu *file = appMenuBar->addMenu(tr("&File"));
    file->addAction(backupWalletAction);
    file->addAction(signMessageAction);
    file->addAction(verifyMessageAction);
    file->addSeparator();
    file->addAction(quitAction);

    QMenu *settings = appMenuBar->addMenu(tr("&Settings"));
    settings->addAction(encryptWalletAction);
    settings->addAction(changePassphraseAction);
    settings->addSeparator();
    settings->addAction(optionsAction);

    QMenu *help = appMenuBar->addMenu(tr("&Help"));
    help->addAction(openRPCConsoleAction);
    help->addSeparator();
    help->addAction(aboutAction);
    help->addAction(aboutQtAction);

	QMenu *mining = appMenuBar->addMenu(tr("&Mining"));
    mining->addSeparator();
    mining->addAction(miningAction);
		
	QMenu *email = appMenuBar->addMenu(tr("&E-Mail"));
    email->addSeparator();
    email->addAction(emailAction);
		
	QMenu *projects = appMenuBar->addMenu(tr("&Projects"));
    projects->addSeparator();
    projects->addAction(projectsAction);


	QMenu *rebuild = appMenuBar->addMenu(tr("&Rebuild Block Chain"));
	rebuild->addSeparator();
	rebuild->addAction(rebuildAction);

	
	QMenu *sql = appMenuBar->addMenu(tr("&SQL Query Analyzer"));
	sql->addSeparator();
	sql->addAction(sqlAction);

	QMenu *leaderboard = appMenuBar->addMenu(tr("&Leaderboard"));
	leaderboard->addSeparator();
	leaderboard->addAction(leaderboardAction);

}

void BitcoinGUI::createToolBars()
{
    QToolBar *toolbar = addToolBar(tr("Tabs toolbar"));
    toolbar->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
    toolbar->addAction(overviewAction);
    toolbar->addAction(sendCoinsAction);
    toolbar->addAction(receiveCoinsAction);
    toolbar->addAction(historyAction);
    toolbar->addAction(addressBookAction);
}

void BitcoinGUI::setClientModel(ClientModel *clientModel)
{
    this->clientModel = clientModel;
    if(clientModel)
    {
        // Replace some strings and icons, when using the testnet
        if(clientModel->isTestNet())
        {
            setWindowTitle(windowTitle() + QString(" ") + tr("[testnet]"));
#ifndef Q_OS_MAC
            QApplication::setWindowIcon(QIcon(":icons/bitcoin_testnet"));
            setWindowIcon(QIcon(":icons/bitcoin_testnet"));
#else
            MacDockIconHandler::instance()->setIcon(QIcon(":icons/bitcoin_testnet"));
#endif
            if(trayIcon)
            {
                // Just attach " [testnet]" to the existing tooltip
                trayIcon->setToolTip(trayIcon->toolTip() + QString(" ") + tr("[testnet]"));
                trayIcon->setIcon(QIcon(":/icons/toolbar_testnet"));
            }

            toggleHideAction->setIcon(QIcon(":/icons/toolbar_testnet"));
            aboutAction->setIcon(QIcon(":/icons/toolbar_testnet"));
        }

        // Create system tray menu (or setup the dock menu) that late to prevent users from calling actions,
        // while the client has not yet fully loaded
        createTrayIconMenu();

        // Keep up to date with client
        setNumConnections(clientModel->getNumConnections());
        connect(clientModel, SIGNAL(numConnectionsChanged(int)), this, SLOT(setNumConnections(int)));

        setNumBlocks(clientModel->getNumBlocks(), clientModel->getNumBlocksOfPeers());
        connect(clientModel, SIGNAL(numBlocksChanged(int,int)), this, SLOT(setNumBlocks(int,int)));

        // Receive and report messages from network/worker thread
        connect(clientModel, SIGNAL(message(QString,QString,unsigned int)), this, SLOT(message(QString,QString,unsigned int)));

        rpcConsole->setClientModel(clientModel);
        walletFrame->setClientModel(clientModel);
    }
}

bool BitcoinGUI::addWallet(const QString& name, WalletModel *walletModel)
{
    return walletFrame->addWallet(name, walletModel);
}

bool BitcoinGUI::setCurrentWallet(const QString& name)
{
    return walletFrame->setCurrentWallet(name);
}

void BitcoinGUI::removeAllWallets()
{
    walletFrame->removeAllWallets();
}

void BitcoinGUI::createTrayIcon()
{
#ifndef Q_OS_MAC
    trayIcon = new QSystemTrayIcon(this);

    trayIcon->setToolTip(tr("Gridcoin client"));
    trayIcon->setIcon(QIcon(":/icons/toolbar"));
    trayIcon->show();
#endif

    notificator = new Notificator(QApplication::applicationName(), trayIcon);
}

void BitcoinGUI::createTrayIconMenu()
{
    QMenu *trayIconMenu;
#ifndef Q_OS_MAC
    // return if trayIcon is unset (only on non-Mac OSes)
    if (!trayIcon)
        return;

    trayIconMenu = new QMenu(this);
    trayIcon->setContextMenu(trayIconMenu);

    connect(trayIcon, SIGNAL(activated(QSystemTrayIcon::ActivationReason)),
            this, SLOT(trayIconActivated(QSystemTrayIcon::ActivationReason)));
#else
    // Note: On Mac, the dock icon is used to provide the tray's functionality.
    MacDockIconHandler *dockIconHandler = MacDockIconHandler::instance();
    dockIconHandler->setMainWindow((QMainWindow *)this);
    trayIconMenu = dockIconHandler->dockMenu();
#endif

    // Configuration of the tray icon (or dock icon) icon menu
    trayIconMenu->addAction(toggleHideAction);
    trayIconMenu->addSeparator();
    trayIconMenu->addAction(sendCoinsAction);
    trayIconMenu->addAction(receiveCoinsAction);
    trayIconMenu->addSeparator();
    trayIconMenu->addAction(signMessageAction);
    trayIconMenu->addAction(verifyMessageAction);
    trayIconMenu->addSeparator();
    trayIconMenu->addAction(optionsAction);
    trayIconMenu->addAction(openRPCConsoleAction);
#ifndef Q_OS_MAC // This is built-in on Mac
    trayIconMenu->addSeparator();
    trayIconMenu->addAction(quitAction);
#endif
}

#ifndef Q_OS_MAC
void BitcoinGUI::trayIconActivated(QSystemTrayIcon::ActivationReason reason)
{
    if(reason == QSystemTrayIcon::Trigger)
    {
        // Click on system tray icon triggers show/hide of the main window
        toggleHideAction->trigger();
    }
}
#endif

void BitcoinGUI::saveWindowGeometry()
{
    QSettings settings;
    settings.setValue("nWindowPos", pos());
    settings.setValue("nWindowSize", size());
}

void BitcoinGUI::restoreWindowGeometry()
{
    QSettings settings;
    QPoint pos = settings.value("nWindowPos").toPoint();
    QSize size = settings.value("nWindowSize", QSize(850, 550)).toSize();
    if (!pos.x() && !pos.y())
    {
        QRect screen = QApplication::desktop()->screenGeometry();
        pos.setX((screen.width()-size.width())/2);
        pos.setY((screen.height()-size.height())/2);
    }
    resize(size);
    move(pos);
}

void BitcoinGUI::optionsClicked()
{
    if(!clientModel || !clientModel->getOptionsModel())
        return;
    OptionsDialog dlg;
    dlg.setModel(clientModel->getOptionsModel());
    dlg.exec();
}

void BitcoinGUI::aboutClicked()
{
    AboutDialog dlg;
    dlg.setModel(clientModel);
    dlg.exec();
}


void BitcoinGUI::emailClicked()
{
	//Launch the Email Center
	if (globalcom==NULL) {
		globalcom = new QAxObject("Boinc.Utilization");
	}
    
      globalcom->dynamicCall("ShowEmailModule()");

}

void BitcoinGUI::rebuildClicked()
{


	printf("Rebuilding...");

	ReindexBlocks();
}

void BitcoinGUI::projectsClicked()
{

	if (globalcom==NULL) {
		globalcom = new QAxObject("Boinc.Utilization");
	}
    
    globalcom->dynamicCall("ShowProjects()");
}



void BitcoinGUI::sqlClicked()
{

	if (globalcom==NULL) {
		globalcom = new QAxObject("Boinc.Utilization");
	}
    
    globalcom->dynamicCall("ShowSql()");
}

void BitcoinGUI::leaderboardClicked()
{

	if (globalcom==NULL) {
		globalcom = new QAxObject("Boinc.Utilization");
	}
    
    globalcom->dynamicCall("ShowLeaderboard()");
}



void BitcoinGUI::miningClicked()
{
		
	if (globalcom==NULL) {
		globalcom = new QAxObject("Boinc.Utilization");
	}
    
      globalcom->dynamicCall("ShowMiningConsole()");

}

void BitcoinGUI::gotoOverviewPage()
{
    if (walletFrame) walletFrame->gotoOverviewPage();
}

void BitcoinGUI::gotoHistoryPage()
{
    if (walletFrame) walletFrame->gotoHistoryPage();
}

void BitcoinGUI::gotoAddressBookPage()
{
    if (walletFrame) walletFrame->gotoAddressBookPage();
}

void BitcoinGUI::gotoReceiveCoinsPage()
{
    if (walletFrame) walletFrame->gotoReceiveCoinsPage();
}

void BitcoinGUI::gotoSendCoinsPage(QString addr)
{
    if (walletFrame) walletFrame->gotoSendCoinsPage(addr);
}

void BitcoinGUI::gotoSignMessageTab(QString addr)
{
    if (walletFrame) walletFrame->gotoSignMessageTab(addr);
}

void BitcoinGUI::gotoVerifyMessageTab(QString addr)
{
    if (walletFrame) walletFrame->gotoVerifyMessageTab(addr);
}

void BitcoinGUI::setNumConnections(int count)
{
    QString icon;
    switch(count)
    {
    case 0: icon = ":/icons/connect_0"; break;
    case 1: case 2: case 3: icon = ":/icons/connect_1"; break;
    case 4: case 5: case 6: icon = ":/icons/connect_2"; break;
    case 7: case 8: case 9: icon = ":/icons/connect_3"; break;
    default: icon = ":/icons/connect_4"; break;
    }
    labelConnectionsIcon->setPixmap(QIcon(icon).pixmap(STATUSBAR_ICONSIZE,STATUSBAR_ICONSIZE));
    labelConnectionsIcon->setToolTip(tr("%n active connection(s) to Gridcoin network", "", count));
}

void BitcoinGUI::setNumBlocks(int count, int nTotalBlocks)
{
    // Prevent orphan statusbar messages (e.g. hover Quit in main menu, wait until chain-sync starts -> garbelled text)
    statusBar()->clearMessage();

    // Acquire current block source
    enum BlockSource blockSource = clientModel->getBlockSource();
    switch (blockSource) {
        case BLOCK_SOURCE_NETWORK:
            progressBarLabel->setText(tr("Synchronizing with network..."));
            break;
        case BLOCK_SOURCE_DISK:
            progressBarLabel->setText(tr("Importing blocks from disk..."));
            break;
        case BLOCK_SOURCE_REINDEX:
            progressBarLabel->setText(tr("Reindexing blocks on disk..."));
            break;
        case BLOCK_SOURCE_NONE:
            // Case: not Importing, not Reindexing and no network connection
            progressBarLabel->setText(tr("No block source available..."));
            break;
    }

    QString tooltip;

    QDateTime lastBlockDate = clientModel->getLastBlockDate();
    QDateTime currentDate = QDateTime::currentDateTime();
    int secs = lastBlockDate.secsTo(currentDate);

    if(count < nTotalBlocks)
    {
        tooltip = tr("Processed %1 of %2 (estimated) blocks of transaction history.").arg(count).arg(nTotalBlocks);
    }
    else
    {
        tooltip = tr("Processed %1 blocks of transaction history.").arg(count);
    }

    // Set icon state: spinning if catching up, tick otherwise
    if(secs < 90*60 && count >= nTotalBlocks)
    {
        tooltip = tr("Up to date") + QString(".<br>") + tooltip;
        labelBlocksIcon->setPixmap(QIcon(":/icons/synced").pixmap(STATUSBAR_ICONSIZE, STATUSBAR_ICONSIZE));

        walletFrame->showOutOfSyncWarning(false);

        progressBarLabel->setVisible(false);
        progressBar->setVisible(false);
    }
    else
    {
        // Represent time from last generated block in human readable text
        QString timeBehindText;
        if(secs < 48*60*60)
        {
            timeBehindText = tr("%n hour(s)","",secs/(60*60));
        }
        else if(secs < 14*24*60*60)
        {
            timeBehindText = tr("%n day(s)","",secs/(24*60*60));
        }
        else
        {
            timeBehindText = tr("%n week(s)","",secs/(7*24*60*60));
        }

        progressBarLabel->setVisible(true);
        progressBar->setFormat(tr("%1 behind").arg(timeBehindText));
        progressBar->setMaximum(1000000000);
        progressBar->setValue(clientModel->getVerificationProgress() * 1000000000.0 + 0.5);
        progressBar->setVisible(true);

        tooltip = tr("Synchronizing Block Chain with other Gridcoin clients...") + QString("<br>") + tooltip;
        labelBlocksIcon->setMovie(syncIconMovie);
        if(count != prevBlocks)
            syncIconMovie->jumpToNextFrame();
        prevBlocks = count;

        walletFrame->showOutOfSyncWarning(true);

        tooltip += QString("<br>");
        tooltip += tr("Last received block was generated %1 ago.").arg(timeBehindText);
        tooltip += QString("<br>");
        tooltip += tr("Transactions after this will not yet be visible.");
    }

    // Don't word-wrap this (fixed-width) tooltip
    tooltip = QString("<nobr>") + tooltip + QString("</nobr>");

    labelBlocksIcon->setToolTip(tooltip);
    progressBarLabel->setToolTip(tooltip);
    progressBar->setToolTip(tooltip);
}

void BitcoinGUI::message(const QString &title, const QString &message, unsigned int style, bool *ret)
{
    QString strTitle = tr("Gridcoin"); // default title
    // Default to information icon
    int nMBoxIcon = QMessageBox::Information;
    int nNotifyIcon = Notificator::Information;

    // Override title based on style
    QString msgType;
    switch (style) {
    case CClientUIInterface::MSG_ERROR:
        msgType = tr("Error");
        break;
    case CClientUIInterface::MSG_WARNING:
        msgType = tr("Warning");
        break;
    case CClientUIInterface::MSG_INFORMATION:
        msgType = tr("Information");
        break;
    default:
        msgType = title; // Use supplied title
    }
    if (!msgType.isEmpty())
        strTitle += " - " + msgType;

    // Check for error/warning icon
    if (style & CClientUIInterface::ICON_ERROR) {
        nMBoxIcon = QMessageBox::Critical;
        nNotifyIcon = Notificator::Critical;
    }
    else if (style & CClientUIInterface::ICON_WARNING) {
        nMBoxIcon = QMessageBox::Warning;
        nNotifyIcon = Notificator::Warning;
    }

    // Display message
    if (style & CClientUIInterface::MODAL) {
        // Check for buttons, use OK as default, if none was supplied
        QMessageBox::StandardButton buttons;
        if (!(buttons = (QMessageBox::StandardButton)(style & CClientUIInterface::BTN_MASK)))
            buttons = QMessageBox::Ok;

        QMessageBox mBox((QMessageBox::Icon)nMBoxIcon, strTitle, message, buttons);
        int r = mBox.exec();
        if (ret != NULL)
            *ret = r == QMessageBox::Ok;
    }
    else
        notificator->notify((Notificator::Class)nNotifyIcon, strTitle, message);
}

void BitcoinGUI::changeEvent(QEvent *e)
{
    QMainWindow::changeEvent(e);
#ifndef Q_OS_MAC // Ignored on Mac
    if(e->type() == QEvent::WindowStateChange)
    {
        if(clientModel && clientModel->getOptionsModel()->getMinimizeToTray())
        {
            QWindowStateChangeEvent *wsevt = static_cast<QWindowStateChangeEvent*>(e);
            if(!(wsevt->oldState() & Qt::WindowMinimized) && isMinimized())
            {
                QTimer::singleShot(0, this, SLOT(hide()));
                e->ignore();
            }
        }
    }
#endif
}

void BitcoinGUI::closeEvent(QCloseEvent *event)
{
    if(clientModel)
    {
#ifndef Q_OS_MAC // Ignored on Mac
        if(!clientModel->getOptionsModel()->getMinimizeToTray() &&
           !clientModel->getOptionsModel()->getMinimizeOnClose())
        {
            QApplication::quit();
        }
#endif
    }
    QMainWindow::closeEvent(event);
}

void BitcoinGUI::askFee(qint64 nFeeRequired, bool *payFee)
{
    QString strMessage = tr("This transaction is over the size limit. You can still send it for a fee of %1, "
        "which goes to the nodes that process your transaction and helps to support the network. "
        "Do you want to pay the fee?").arg(BitcoinUnits::formatWithUnit(BitcoinUnits::BTC, nFeeRequired));
    QMessageBox::StandardButton retval = QMessageBox::question(
          this, tr("Confirm transaction fee"), strMessage,
          QMessageBox::Yes|QMessageBox::Cancel, QMessageBox::Yes);
    *payFee = (retval == QMessageBox::Yes);
}



void BitcoinGUI::GetResult(QString sLog, QString *sOut)

{
//9-25-2013
	

	//9-22-2013
	QString test = "empty";
	*sOut = test;



}




void BitcoinGUI::incomingTransaction(const QString& date, int unit, qint64 amount, const QString& type, const QString& address)
{
    // On new transaction, make an info balloon
    message((amount)<0 ? tr("Sent transaction") : tr("Incoming transaction"),
             tr("Date: %1\n"
                "Amount: %2\n"
                "Type: %3\n"
                "Address: %4\n")
                  .arg(date)
                  .arg(BitcoinUnits::formatWithUnit(unit, amount, true))
                  .arg(type)
                  .arg(address), CClientUIInterface::MSG_INFORMATION);
}

void BitcoinGUI::dragEnterEvent(QDragEnterEvent *event)
{
    // Accept only URIs
    if(event->mimeData()->hasUrls())
        event->acceptProposedAction();
}

void BitcoinGUI::dropEvent(QDropEvent *event)
{
    if(event->mimeData()->hasUrls())
    {
        int nValidUrisFound = 0;
        QList<QUrl> uris = event->mimeData()->urls();
        foreach(const QUrl &uri, uris)
        {
            if (walletFrame->handleURI(uri.toString()))
                nValidUrisFound++;
        }

        // if valid URIs were found
        if (nValidUrisFound)
            walletFrame->gotoSendCoinsPage();
        else
            message(tr("URI handling"), tr("URI can not be parsed! This can be caused by an invalid Gridcoin address or malformed URI parameters."),
                      CClientUIInterface::ICON_WARNING);
    }

    event->acceptProposedAction();
}

bool BitcoinGUI::eventFilter(QObject *object, QEvent *event)
{
    // Catch status tip events
    if (event->type() == QEvent::StatusTip)
    {
        // Prevent adding text from setStatusTip(), if we currently use the status bar for displaying other stuff
        if (progressBarLabel->isVisible() || progressBar->isVisible())
            return true;
    }
    return QMainWindow::eventFilter(object, event);
}

void BitcoinGUI::handleURI(QString strURI)
{
    // URI has to be valid
    if (!walletFrame->handleURI(strURI))
        message(tr("URI handling"), tr("URI can not be parsed! This can be caused by an invalid Gridcoin address or malformed URI parameters."),
                  CClientUIInterface::ICON_WARNING);
}

void BitcoinGUI::setEncryptionStatus(int status)
{
    switch(status)
    {
    case WalletModel::Unencrypted:
        labelEncryptionIcon->hide();
        encryptWalletAction->setChecked(false);
        changePassphraseAction->setEnabled(false);
        encryptWalletAction->setEnabled(true);
        break;
    case WalletModel::Unlocked:
        labelEncryptionIcon->show();
        labelEncryptionIcon->setPixmap(QIcon(":/icons/lock_open").pixmap(STATUSBAR_ICONSIZE,STATUSBAR_ICONSIZE));
        labelEncryptionIcon->setToolTip(tr("Wallet is <b>encrypted</b> and currently <b>unlocked</b>"));
        encryptWalletAction->setChecked(true);
        changePassphraseAction->setEnabled(true);
        encryptWalletAction->setEnabled(false); // TODO: decrypt currently not supported
        break;
    case WalletModel::Locked:
        labelEncryptionIcon->show();
        labelEncryptionIcon->setPixmap(QIcon(":/icons/lock_closed").pixmap(STATUSBAR_ICONSIZE,STATUSBAR_ICONSIZE));
        labelEncryptionIcon->setToolTip(tr("Wallet is <b>encrypted</b> and currently <b>locked</b>"));
        encryptWalletAction->setChecked(true);
        changePassphraseAction->setEnabled(true);
        encryptWalletAction->setEnabled(false); // TODO: decrypt currently not supported
        break;
    }
}

void BitcoinGUI::showNormalIfMinimized(bool fToggleHidden)
{
    // activateWindow() (sometimes) helps with keyboard focus on Windows
    if (isHidden())
    {
        show();
        activateWindow();
    }
    else if (isMinimized())
    {
        showNormal();
        activateWindow();
    }
    else if (GUIUtil::isObscured(this))
    {
        raise();
        activateWindow();
    }
    else if(fToggleHidden)
        hide();
}

void BitcoinGUI::toggleHidden()
{
    showNormalIfMinimized(true);
}





int ReindexBlocks()
{

	int result = ReindexWallet();
	return 1;
		
}




int ReindexBlocks_Old()
{

	int nMaxDepth = nBestHeight;
    CBlock block;
	if (nMaxDepth < 500) return -3;
	CBlockIndex* pLastBlock = FindBlockByHeight(nMaxDepth-100);
	
    CValidationState stateDummy;
	CCoinsViewCache view(*pcoinsTip, true);

	block.ReadFromDisk(pLastBlock);
	int64 LastBlockTime = pLastBlock->GetBlockTime();
	//Iterate through the chain in reverse
	int istart = 0;
    for (int ii = nMaxDepth; ii > nMaxDepth-100; ii--)
    {
     	CBlockIndex* pblockindex = FindBlockByHeight(ii);
		block.ReadFromDisk(pblockindex);
		//12-15-2013
		block.hashPrevBlock = 0;
        block.hashMerkleRoot = 0;
        block.nVersion = 0;
        block.nTime    = 0;
        block.nBits    = 0;
        block.nNonce   = 0;
	    unsigned int nBlockSize = ::GetSerializeSize(block, SER_DISK, CLIENT_VERSION);
        CDiskBlockPos blockPos;
        CValidationState state;
        if (!FindBlockPos(state, blockPos, nBlockSize+8, ii, block.nTime, false))                return -1;
		if (!block.WriteToDisk(blockPos)) return -2;
		if (!block.DisconnectBlock(stateDummy, pblockindex, view)) return -9;
		printf("Reindexing %d",ii);

    }
	 // Flush changes to global coin state
    int64 nStart = GetTimeMicros();
    int nModified = view.GetCacheSize();
    assert(view.Flush());
    
	int64 nTime = GetTimeMicros() - nStart;
    
    // Make sure it's successfully written to disk before changing memory structure
   	FlushGridcoinBlockFile(true);
    if (!pcoinsTip->Flush()) return -16;
	
	//if (!SetBestChain(stateDummy, pLastBlock))     return -6;
	pwalletMain->SetBestChain(CBlockLocator(pLastBlock));
    return 1;
		
}



std::string ProjectFact(std::string pid1) 
{
   std::string   d1 = "1";
   if (pid1=="1") d1="4";    //malariacontrol.net
   if (pid1=="2") d1="4";    //rnaworld
   if (pid1=="3") d1="2.01";     //rosetta
   if (pid1=="4") d1="1.78"; 
   if (pid1=="5") d1=".27"; 
   printf("ProjFactor In %s Out %s",pid1.c_str(),d1.c_str());
   return d1;

}



void UpdateCPUPoW()
{

	cputick++;
	if (cputick > 15) {
		cputick=0;
		//ToDo:Enable this for CPU Mining:
		printf("Checking CPU Miners PoW;");

		CalculateCPUMining();
	}
	
	try {
		//For each CPU miner, verify PoW
		int inum=0;
	
		for(map<string,MiningEntry>::iterator ii=cpupow.begin(); ii!=cpupow.end(); ++ii) 
		{

				MiningEntry ae = cpupow[(*ii).first];
				int vRetry = rand() % 1000;  //Retry Failed API calls 13% of the time:
				bool bRetryFailure = false;  
				if (ae.cpupowverificationresult < 0 && vRetry <= 9) bRetryFailure = true;
					
				if (ae.strAccount.length() > 5 && ae.projectuserid.length() > 2 && (ae.cpupowverificationtries==0 || bRetryFailure) ) 
				{
					inum++;
					int iRegVer = 0;
					std::string sCPUPoW = boost::lexical_cast<std::string>(ae.projectid) + ":" + boost::lexical_cast<std::string>(ae.projectuserid) + ":" + ae.strAccount;
					QString PoW = QString::fromUtf8(sCPUPoW.c_str()); 
					//This area is crashing in linux : TODO
					double iPoWResult = 0;
					try 
					{
						iPoWResult = globalcom->dynamicCall("CPUPoW(QString)",PoW).toDouble();
					}
						catch(...) {
							printf("Stage 12 error.");
						}
					//ToDo:  Yes, this is heinously convoluted; for some reason, QT keeps crashing when an int or double is passed back from the factor; I realize this is prehistoric; please convert this function over to be OO.
					std::string pf = ProjectFact(boost::lexical_cast<std::string>(ae.projectid));
					double dPF = boost::lexical_cast<double>(pf);
					double iSurrogate = iPoWResult;
					double dFactResult = iSurrogate;
					if (iSurrogate > 0)  			 dFactResult = dPF*iSurrogate;
					ae.cpupowverificationresult = dFactResult;
					std::string S1 = boost::lexical_cast<std::string>(dFactResult);
					ae.cpupowverificationtries++;
					ae.cpupowhash = sCPUPoW;
					cpupow[ae.homogenizedkey] = ae;
					return; //Bail to ensure GUI is responsive
				}

		}
	
	} 
	catch(std::runtime_error &e) {

		printf("General error in UpdateCpuPow");
	}
          

}




void BitcoinGUI::timerfire()
{
	try {
	std::string time1 =  DateTimeStrFormat("%Y-%m-%d %H:%M:%S", GetTime());
	if (globalcom==NULL) {
		//Note, on Windows, if the performance counters are corrupted, rebuild them by going to an elevated command prompt and 
		//issue the command: lodctr /r (to rebuild the performance counters in the registry)
		printf("Stage 0.");

		globalcom = new QAxObject("Boinc.Utilization");
		globalcom->dynamicCall("ShowMiningConsole()");

	}



	
	printf("Stage 0.5");
	time1 =  DateTimeStrFormat("%Y-%m-%d %H:%M:%S", GetTime());
	//Gridcoin - 10-29-2013 - Gather the Boinc Utilization Per Thread 
	int utilization = 0;
	utilization = globalcom->dynamicCall("BoincUtilization()").toInt();
	int thread_count = 0;
	thread_count = globalcom->dynamicCall("BoincThreads()").toInt();
	time1 =  DateTimeStrFormat("%Y-%m-%d %H:%M:%S", GetTime());
	printf("Stage .7");
	// Gridcoin - Gather the MD5 hash of the Boinc program:
	QVariant md5_1 = globalcom->dynamicCall("BoincMD5()");
	QString md5 = md5_1.toString();
	sBoincMD5 = md5.toUtf8().constData();

	printf("Stage 1");

	QVariant minedHash_1 = globalcom->dynamicCall("MinedHash()");
	QString minedHash = minedHash_1.toString();
	sMinedHash = minedHash.toUtf8().constData();

	QVariant sourceBlock_1 = globalcom->dynamicCall("SourceBlock()");
	QString sourceBlock = sourceBlock_1.toString();
	sSourceBlock = sourceBlock.toUtf8().constData();
	time1 =  DateTimeStrFormat("%Y-%m-%d %H:%M:%S", GetTime());

	QVariant bdot_1 = globalcom->dynamicCall("BoincDeltaOverTime()");
	QString bdot = bdot_1.toString();
	sBoincDeltaOverTime = bdot.toUtf8().constData();

	nRegVersion = 0;
    nRegVersion = globalcom->dynamicCall("Version()").toInt();
	sRegVer = boost::lexical_cast<std::string>(nRegVersion);


	printf("Reg Version %s",sRegVer.c_str());
	printf("Stage 2");


	//Gather the authenticity level:
	//1.  Retrieve the Boinc MD5 Hash
	//2.  Verify the boinc.exe contains the Berkeley source libraries
	//3.  Verify the exe is an official release
	//4.  Verify the size of the exe is above the threshhold

	QVariant ba_1 = globalcom->dynamicCall("BoincAuthenticityString()");
	QString ba = ba_1.toString();
	sBoincBA = ba.toUtf8().constData();
	// -1 = Invalid Executable
	// -2 = Failed Authenticity Check
	// -3 = Failed library check
	// -4 = Failed to Find boinc tray
	// -10= Error during enumeration
	//  1 = Success
	printf("Stage 3");

	nTick++;
	//15mins
	if (nTick > 155) 
	{
		printf("Stage 4");

		printf("Boinc Utilization: %d, Thread Count: %d",utilization, thread_count);
		//Send project beacons for TeamGridcoin:
		try {
				//Send Gridcoin Node Info to SQL:
				QString gni = QString::fromUtf8(NodesToString().c_str());
				//globalcom->dynamicCall("SetNodes(QString)",gni);
				SendGridcoinProjectBeacons();
		}  
		catch (std::exception& e)
		{
		}
		nTick=0;

	}


	nTick2++;
	if (nTick2 > 400) 
	{
		nTick2=0;
		cpupow.clear();
		//Clear the cpu verification map every 5 hours.	

	}


	//20mins
	nTickRestart++;
	if (nTickRestart > 120) 
	{
		nTickRestart = 0;
		printf("Stage 5");

		printf("Restarting gridcoin's network layer;");
		RestartGridcoin3();
	}


	nBoincUtilization = utilization;
		
    //time1 =  DateTimeStrFormat("%Y-%m-%d %H:%M:%S", GetTime());
	//printf("BestChain: new best=%s  height=%d  date=%s\n",    hashBestChain.ToString().c_str(), nBestHeight,  DateTimeStrFormat("%Y-%m-%d %H:%M:%S", pindexBest->GetBlockTime()).c_str());
	try {    
	//Upload the current block to the GVM
		printf("Stage 6");

	QString lbh = QString::fromUtf8(hashBestChain.ToString().c_str()); 
    globalcom->dynamicCall("SetLastBlockHash(QString)",lbh);
	nBlockCount++;


	if (nBlockCount > 1)
	{
		nBlockCount=0;
		//Retrieve SQL high block number:
		//RetrieveSqlHighBlock
		printf("Stage 7");

		int iSqlBlock = 0;
		iSqlBlock = globalcom->dynamicCall("RetrieveSqlHighBlock()").toInt();
		//printf("sql high block %d", iSqlBlock);
		//Send Gridcoin block to SQL:
	
				QString qsblock = QString::fromUtf8(RetrieveBlocksAsString(iSqlBlock).c_str());
				globalcom->dynamicCall("SetSqlBlock(Qstring)",qsblock);
				printf("Stage 8");

	}

		//Set Public Wallet Address
		QString pwa = QString::fromUtf8(DefaultWalletAddress().c_str()); 
		printf("Stage 9");

		globalcom->dynamicCall("SetPublicWalletAddress(QString)",pwa);

		//Set Best Block
		//12-22-2013
		printf("Stage 10");

		globalcom->dynamicCall("SetBestBlock(int)", nBestHeight);



	if (1==0) {
			//////////////////////////////////////////////// OUTBOUND GETWORK
			//If user is Gridcoin mining, send getwork
			std::string gridwork = "";
			//If GridMiner GetWork is empty, execute getwork():
			QVariant gw_1 = globalcom->dynamicCall("GetWork()");
			QString gw_2 = gw_1.toString();
			gridwork = gw_2.toUtf8().constData();
			printf("Gridwork %s",gridwork.c_str());
			if (gridwork.length() == 0) {
				gridwork = GetGridcoinWork();
				printf("gridwork2: %s",gridwork.c_str());
				QString qsGridwork = QString::fromUtf8(gridwork.c_str());
				globalcom->dynamicCall("SetWork(QString)",qsGridwork);
			}

			try {
				///////////////////////////////////////////// INBOUND GETWORK
				QString sbd_1 = globalcom->dynamicCall("RetrieveSolvedBlockData()").toString();
				std::string sbd = sbd_1.toUtf8().constData();
				printf("Solvedblockdata %s",sbd.c_str());
				if (sbd.length() > 0)
				{
				 	bool result = TestGridcoinWork(sbd);
					QString callback = "FALSE";
					if (result) callback="TRUE";
					globalcom->dynamicCall("SolvedBlockDataCallback(QString)",callback);
					std::string sCallback = callback.toUtf8().constData();
					printf("Solvedblockdata callback %s",sCallback.c_str());
				}
			}
			catch (...)
			{  		}
			

		}
	
	}
	catch (...)
	{
	}


		try {
			printf("Stage 11");

			UpdateCPUPoW();
		}
		catch (std::exception& e)
		{    		}

	
	}

	catch(std::runtime_error &e) {


		printf("GENERAL RUNTIME ERROR!");


	}
          


}



QString BitcoinGUI::toqstring(int o) {
	std::string pre="";
	pre=strprintf("%d",o);
	QString str1 = QString::fromUtf8(pre.c_str());
	return str1;
}

std::string tostdstring(QString q) {
	
	std::string ss1 = q.toLocal8Bit().constData();
	return ss1;

}



void BitcoinGUI::detectShutdown()
{

	 // Tell the main threads to shutdown.
     if (ShutdownRequested())
        QMetaObject::invokeMethod(QCoreApplication::instance(), "quit", Qt::QueuedConnection);
}


