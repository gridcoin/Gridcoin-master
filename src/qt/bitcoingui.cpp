/*
 * Qt4 bitcoin GUI.
 *
 * W.J. van der Laan 2011-2012
 * The Bitcoin Developers 2011-2012
 */

#include <QApplication>
#include <QProcess>

#ifdef WIN32
#include <QAxObject>
#include <ActiveQt/qaxbase.h>
#include <ActiveQt/qaxobject.h>
#endif




#include "bitcoingui.h"
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
#include "themecontrol.h"


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
#include <boost/lexical_cast.hpp>

#include "bitcoinrpc.h"

#include <iostream>

using namespace std;

const QString BitcoinGUI::DEFAULT_WALLET = "~Default";

int nTick = 0;
int nTickRestart = 0;
int nBlockCount = 0;
int nTick2 = 0;
int nRegVersion;
int nNeedsUpgrade = 0;
double GetPoBDifficulty();
extern int CreateRestorePoint();
extern int DownloadBlocks();
void StopGridcoin3();
bool OutOfSyncByAge();
bool bCheckedForUpgrade = false;
void ThreadCPIDs();
int Races(int iMax1000);
std::string GetGlobalStatus();
std::string GetHttpPage(std::string cpid);
bool TallyNetworkAverages();
void LoadCPIDsInBackground();
std::string BackupGridcoinWallet();
void InitializeCPIDs();
void RestartGridcoinMiner();
extern int UpgradeClient();
extern int CloseGuiMiner();
std::string RetrieveMd5(std::string s1);
void WriteAppCache(std::string key, std::string value);

void HarvestCPIDs(bool cleardata);
extern int RestartClient();
extern int ReindexWallet();
#ifdef WIN32
QAxObject *globalcom = NULL;
#endif
int ThreadSafeVersion();
void FlushGridcoinBlockFile(bool fFinalize);
extern int ReindexBlocks();
bool OutOfSync();
void RestartGridcoin3();
bool FindBlockPos(CValidationState &state, CDiskBlockPos &pos, unsigned int nAddSize, unsigned int nHeight, uint64 nTime, bool fKnown);



extern void SendGridcoinProjectBeacons();
std::string NodesToString();


json_spirit::Value getwork(const json_spirit::Array& params, bool fHelp);
bool TestGridcoinWork(std::string sWork);


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

	// prevents an oben debug window from becoming stuck/unusable on client shutdown
    connect(quitAction, SIGNAL(triggered()), rpcConsole, SLOT(hide()));
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
	for (unsigned int i=0;i < s.length(); i++) 
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




/*
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
	return result;
	*/



int ReindexWallet()
{
			printf("executing grcrestarter reindex");

			QString sFilename = "GRCRestarter.exe";
			QString sArgument = "";
			QString path = QCoreApplication::applicationDirPath() + "\\" + sFilename;
			QProcess p;
			if (!fTestNet)
			{
#ifdef WIN32
				if (!globalcom) 
				{
					globalcom = new QAxObject("Boinc.Utilization");
				}

				globalcom->dynamicCall("ReindexWallet()");
#endif
			}
			else
			{
#ifdef WIN32
				globalcom->dynamicCall("ReindexWalletTestNet()");
#endif
			}
			StartShutdown();
			return 1;
}



int CreateRestorePoint()
{
			printf("executing grcrestarter createrestorepoint");

			QString sFilename = "GRCRestarter.exe";
			QString sArgument = "";
			QString path = QCoreApplication::applicationDirPath() + "\\" + sFilename;
			QProcess p;
            StopGridcoin3();


			if (!fTestNet)
			{
#ifdef WIN32
				globalcom->dynamicCall("CreateRestorePoint()");
#endif
			}
			else
			{
#ifdef WIN32
				globalcom->dynamicCall("CreateRestorePointTestNet()");
#endif
			}
			RestartGridcoin3();

			return 1;
}



int DownloadBlocks()
{
			printf("executing grcrestarter downloadblocks");

			QString sFilename = "GRCRestarter.exe";
			QString sArgument = "";
			QString path = QCoreApplication::applicationDirPath() + "\\" + sFilename;
			QProcess p;

			#ifdef WIN32
				if (!globalcom) 
				{
					globalcom = new QAxObject("Boinc.Utilization");
				}

				globalcom->dynamicCall("DownloadBlocks()");
				StartShutdown();
			#endif
			
			return 1;
}






int RestartClient()
{
	
	printf("executing grcrestarter restart");

	QString sFilename = "GRCRestarter.exe";
			QString sArgument = "";
			QString path = QCoreApplication::applicationDirPath() + "\\" + sFilename;
			QProcess p;
	#ifdef WIN32
			globalcom->dynamicCall("RestartWallet()");
#endif
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
			if (!fTestNet)
			{
#ifdef WIN32
				globalcom->dynamicCall("UpgradeWallet()");
#endif
			}
			else
			{
#ifdef WIN32
				globalcom->dynamicCall("UpgradeWalletTestnet()");
#endif
			}

			StartShutdown();
			return 1;
}

int CloseGuiMiner()
{
	try 
	{
#ifdef WIN32
			globalcom->dynamicCall("CloseGUIMiner()");
#endif
	}
	catch(...) { return 0; } 

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

	downloadAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Download Blocks"), this);
	downloadAction->setStatusTip(tr("Download Blocks"));
	downloadAction->setMenuRole(QAction::TextHeuristicRole);

	
	upgradeAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Upgrade QT Client"), this);
	upgradeAction->setStatusTip(tr("Upgrade QT Client"));
	upgradeAction->setMenuRole(QAction::TextHeuristicRole);


    aboutAction = new QAction(QIcon(":/icons/bitcoin"), tr("&About Gridcoin"), this);
    aboutAction->setStatusTip(tr("Show information about Gridcoin"));
    aboutAction->setMenuRole(QAction::AboutRole);
	
	miningAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Mining Console"), this);
	miningAction->setStatusTip(tr("Go to the mining console"));
	miningAction->setMenuRole(QAction::TextHeuristicRole);


	emailAction = new QAction(QIcon(":/icons/bitcoin"), tr("&E-Mail Center"), this);
	emailAction->setStatusTip(tr("Go to the E-Mail center"));
	emailAction->setMenuRole(QAction::TextHeuristicRole);

	sqlAction = new QAction(QIcon(":/icons/bitcoin"), tr("&SQL Query Analyzer"), this);
	sqlAction->setStatusTip(tr("SQL Query Analyzer"));
	sqlAction->setMenuRole(QAction::TextHeuristicRole);

	leaderboardAction = new QAction(QIcon(":/icons/bitcoin"), tr("&Leaderboard"), this);
	leaderboardAction->setStatusTip(tr("Leaderboard"));
	leaderboardAction->setMenuRole(QAction::TextHeuristicRole);

    //aboutQtAction = new QAction(QIcon(":/trolltech/qmessagebox/images/qtlogo-64.png"), tr("About &Qt"), this);
    //aboutQtAction->setStatusTip(tr("Show information about Qt"));
    //aboutQtAction->setMenuRole(QAction::AboutQtRole);

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
	connect(rebuildAction, SIGNAL(triggered()), this, SLOT(rebuildClicked()));
	connect(upgradeAction, SIGNAL(triggered()), this, SLOT(upgradeClicked()));
	connect(downloadAction, SIGNAL(triggered()), this, SLOT(downloadClicked()));

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
	
	QMenu *upgrade = appMenuBar->addMenu(tr("&Upgrade QT Client"));
	upgrade->addSeparator();
	upgrade->addAction(upgradeAction);


	QMenu *rebuild = appMenuBar->addMenu(tr("&Rebuild Block Chain"));
	rebuild->addSeparator();
	rebuild->addAction(rebuildAction);
	rebuild->addSeparator();
	rebuild->addAction(downloadAction);
	rebuild->addSeparator();
	
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
    connect(&dlg, SIGNAL(optionsApplied()), this, SLOT(updateTheme()));
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
#ifdef WIN32
    globalcom->dynamicCall("ShowEmailModule()");
#endif

}

void BitcoinGUI::rebuildClicked()
{
	printf("Rebuilding...");
	ReindexBlocks();
}


void BitcoinGUI::upgradeClicked()
{
	printf("Upgrading Gridcoin...");
	UpgradeClient();
	
}

void BitcoinGUI::downloadClicked()
{
	DownloadBlocks();

}

void BitcoinGUI::sqlClicked()
{
#ifdef WIN32

	if (!globalcom) 
	{
		globalcom = new QAxObject("Boinc.Utilization");
	}
    
    globalcom->dynamicCall("ShowSql()");

#endif

}

void BitcoinGUI::leaderboardClicked()
{
	#ifdef WIN32

	if (globalcom==NULL) {
		globalcom = new QAxObject("Boinc.Utilization");
	}
    
    globalcom->dynamicCall("ShowLeaderboard()");
#endif
}



void BitcoinGUI::miningClicked()
{
		
#ifdef WIN32

	if (globalcom==NULL) {
		globalcom = new QAxObject("Boinc.Utilization");
	}
    
      globalcom->dynamicCall("ShowMiningConsole()");
#endif
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
		//If we have never loaded net averages load them now:
		if (!bNetAveragesLoaded)
		{
			TallyNetworkAverages();
		}

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


void BitcoinGUI::threadsafewin32call(const QString& h1,const QString& h2,const QString& h3,const QString& h4,const QString& h5, int *result)
{
	#ifdef WIN32

	printf("calling threadsafe callwin32");
	*result = globalcom->dynamicCall("ThreadSafeWin32Call(QString,QString,QString,QString,QString)",h1,h2,h3,h4,h5).toInt();
#endif
}



void BitcoinGUI::GetResult(QString sLog, QString *sOut)

{
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




bool Timer(std::string timer_name, int max_ms)
{
	mvTimers[timer_name] = mvTimers[timer_name] + 1;
	if (mvTimers[timer_name] > max_ms)
	{
		mvTimers[timer_name]=0;
		return true;
	}
	return false;
}







void ReinstantiateGlobalcom()
{
#ifdef WIN32

			//Note, on Windows, if the performance counters are corrupted, rebuild them by going to an elevated command prompt and 
	   		//issue the command: lodctr /r (to rebuild the performance counters in the registry)
			std::string os = GetArg("-os", "windows");
			if (os == "linux" || os == "mac")
			{
				printf("Instantiating globalcom for Linux");			
				globalcom = new QAxObject("Boinc.LinuxUtilization");
			}
			else
			{
					globalcom = new QAxObject("Boinc.Utilization");
					printf("Instantiating globalcom for Windows");
			}
		
			globalcom->dynamicCall("ShowMiningConsole()");
			//printf("Showing Mining Console");
			if (bCheckedForUpgrade == false && !fTestNet)
			{
							int nNeedsUpgrade = 0;
							bool bCheckedForUpgrade = true;
							printf("Checking to see if Gridcoin needs upgraded\r\n");
							nNeedsUpgrade = globalcom->dynamicCall("ClientNeedsUpgrade()").toInt();
							if (nNeedsUpgrade) UpgradeClient();
			}
#endif
}


void BitcoinGUI::timerfire()
{
	try {

		std::string time1 =  DateTimeStrFormat("%m-%d-%Y %H:%M:%S", GetTime());
		if (Timer("timestamp",15))
		{
			printf("Timestamp: %s\r\n",time1.c_str());
		}


		
	
		//Backup the wallet once per day:
		if (Timer("backupwallet", 6*60*20))
		{

			std::string backup_results = BackupGridcoinWallet();
			printf("Daily backup results: %s\r\n",backup_results.c_str());
			//Create a restore point once per day
			int r = CreateRestorePoint();
			printf("Created restore point : %i",r);
		}

		if (Timer("start",1))
		{
			#ifdef WIN32
			if (globalcom==NULL) ReinstantiateGlobalcom();
			nRegVersion = globalcom->dynamicCall("Version()").toInt();
			sRegVer = boost::lexical_cast<std::string>(nRegVersion);
			#endif
			
		}

		
		
		if (Timer("status_update",2))
		{
			std::string status = GetGlobalStatus();
    		bForceUpdate=true;
		}

		if (Timer("update_boinc_magnitude", 5))
		{
			    
			    double POB = GetPoBDifficulty();
				if (!fTestNet && (POB==99 || POB < .76) && !OutOfSyncByAge()) 
				{
					//Do this when wallet *was* out of sync, is now in sync, and PoB calculation is out of whack:
					TallyNetworkAverages();
					POB = GetPoBDifficulty();
				}
				//
				QString bm = QString::fromUtf8(RoundToString(boincmagnitude,2).c_str());
				nBoincUtilization = (int)boincmagnitude;
				#ifdef WIN32
				globalcom->dynamicCall("BoincMagnitude(Qstring)", bm);
				#endif
		}
   

		

		if (Timer("net_averages",350)) 
		{
			printf("\r\nReharvesting Gridcoin Net Averages\r\n");
			//Freeze mining threads
		   TallyNetworkAverages();
			//Restart the PoB miners, since the PoB Diff probably changed:
		}
		


		if (mapArgs["-restartnetlayer"] == "true") 
		{
			if (Timer("restart_network",30))
			{
				printf("\r\nRestarting gridcoin's network layer @ %s\r\n",time1.c_str());
				RestartGridcoin3();
			}
		}

        if (false)
		{
		if (Timer("gather_cpids",10000))
		{
			printf("\r\nReharvesting cpids in background thread...\r\n");
		   LoadCPIDsInBackground();
		}
		}

		if (false)
		{
				if (Timer("sql",2))
				{

					#ifdef WIN32
					//Upload the current block to the GVM
					//printf("Ready to sync SQL...\r\n");
     				//QString lbh = QString::fromUtf8(hashBestChain.ToString().c_str()); 
	    			//globalcom->dynamicCall("SetLastBlockHash(QString)",lbh);
					//Retrieve SQL high block number:
					int iSqlBlock = 0;
					iSqlBlock = globalcom->dynamicCall("RetrieveSqlHighBlock()").toInt();
     				//Send Gridcoin block to SQL:
					QString qsblock = QString::fromUtf8(RetrieveBlocksAsString(iSqlBlock).c_str());
					globalcom->dynamicCall("SetSqlBlock(Qstring)",qsblock);
	    			//Set Public Wallet Address
     				//QString pwa = QString::fromUtf8(DefaultWalletAddress().c_str()); 
					//globalcom->dynamicCall("SetPublicWalletAddress(QString)",pwa);
	    			//Set Best Block
	    			globalcom->dynamicCall("SetBestBlock(int)", nBestHeight);
					#endif
				}
		}
		}
		catch(std::runtime_error &e) 
		{
			printf("GENERAL RUNTIME ERROR!");
		}


}



QString BitcoinGUI::toqstring(int o) 
{
	std::string pre="";
	pre=strprintf("%d",o);
	QString str1 = QString::fromUtf8(pre.c_str());
	return str1;
}

std::string tostdstring(QString q) 
{
	std::string ss1 = q.toLocal8Bit().constData();
	return ss1;
}



void BitcoinGUI::detectShutdown()
{

	 // Tell the main threads to shutdown.
     if (ShutdownRequested())
        QMetaObject::invokeMethod(QCoreApplication::instance(), "quit", Qt::QueuedConnection);
}

void BitcoinGUI::updateTheme()
{
	applyTheme(this, THEME_BITCOINGUI);
	applyTheme(walletFrame, THEME_WALLETFRAME);
	rpcConsole->triggerTheme();
	emit(pagesView());
}



