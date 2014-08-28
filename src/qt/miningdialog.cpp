#include "miningdialog.h"
#include "ui_miningdialog.h"
#include <QPushButton>

#ifdef WIN32
#include <QAxObject>
#include <ActiveQt/qaxbase.h>
#include <ActiveQt/qaxobject.h>

#include "../global_objects.hpp"
#include "../global_objects_noui.hpp"

#endif


#include <QProcess>
#include <QMessageBox>
#include "clientmodel.h"
#include "clientversion.h"
#include "uint256.h"
#include "base58.h"
#include "themecontrol.h"


// Copyright 10-3-2013

const int MININGDIALOG_COPYRIGHT_YEAR = 2013;


MiningDialog::MiningDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::MiningDialog)
{
    ui->setupUi(this);
    applyTheme(this, THEME_MININGDIALOG);

    // Set current copyright year
    // ui->copyrightLabel->setText(tr("Copyright") + QString(" &copy; 2009-%1 ").arg(COPYRIGHT_YEAR) + tr("The Bitcoin developers") + QString("<br>") + tr("Copyright") + QString(" &copy; "));
}

void MiningDialog::setModel(ClientModel *model)
{
    if(model)
    {
     
		   connect(ui->btnRefresh, SIGNAL(clicked()), this, SLOT(refreshClicked()));
		   connect(ui->btnExit, SIGNAL(clicked()), this, SLOT(exitClicked()));
		   connect(ui->btnRegister, SIGNAL(clicked()), this, SLOT(registerClicked()));
		   connect(ui->btnUnregister, SIGNAL(clicked()), this, SLOT(unregisterClicked()));
		   MiningDialog::refreshClicked();

    }
}

MiningDialog::~MiningDialog()
{
    delete ui;
}




void MiningDialog::refreshClicked()

{

#ifdef WIN32
	  //Show the boinc homogenized utilization
	  ui->lblProcessingPowerValue->setText(tr("Calculating..."));
      std::string sBoincUtilization="";
      sBoincUtilization = strprintf("%d",nBoincUtilization);
	  QString qsUtilization = QString::fromUtf8(sBoincUtilization.c_str());
	  ui->lblProcessingPowerValue->setText(qsUtilization);
#endif

      int thread_count = 0;
#ifdef WIN32
	  thread_count = globalcom->dynamicCall("BoincThreads()").toInt();
	  std::string sThreads =strprintf("%d",thread_count);
	  QString qsThreads = QString::fromUtf8(sThreads.c_str());
	  ui->lblThreads->setText(qsThreads);
#endif


	  int boinc_proxy_version = 0;
#ifdef WIN32
	  boinc_proxy_version = globalcom->dynamicCall("Version()").toInt();
	  std::string sVersion =strprintf("%d",boinc_proxy_version);
	  QString qsVersion = QString::fromUtf8(sVersion.c_str());
	  ui->lblVersion->setText(qsVersion);
#endif


}

void MiningDialog::registerClicked()

{
	 QMessageBox::StandardButton retval = QMessageBox::question(this, tr("Please Confirm O/S registration of the mining module?"),
                 tr("GridCoin will now register the mining module to allow the measurement of boinc utilization.  If successful, the mining module version will be above 0.") + "<br><br>" + tr("Continue?"),
                 QMessageBox::Yes|QMessageBox::Cancel,
                 QMessageBox::Cancel);
        if(retval == QMessageBox::Yes)
        {

		    MiningDialog::regsvr("regtlibv12.exe","boinc.tlb","");
			MiningDialog::regsvr("regasm.exe","boinc.dll","");
         	MiningDialog::refreshClicked();

		}
		else 
		{

			// No changes have been made.

		}
   


}


void MiningDialog::regsvr(QString program, QString sFilename, QString sArgument)
{

#ifdef WIN32

	
			QString path = QCoreApplication::applicationDirPath() + "\\" + program;
			QProcess p;
			p.start(path, QStringList() << sFilename << sArgument);
			p.waitForFinished();

			if (sArgument=="") 
			{
					globalcom = new QAxObject("boinc.Utilization");
			} else
			{
				globalcom = NULL;
			}
#endif


}


void MiningDialog::unregisterClicked()

{
	    QMessageBox::StandardButton retval = QMessageBox::question(this, tr("Please Confirm un-registration of the mining module?"),
                 tr("GridCoin will now un-register the mining module.  Please close and re-start the program to confirm success (the program may be using the DLL).") + "<br><br>" + tr("Continue?"),
                 QMessageBox::Yes|QMessageBox::Cancel,
                 QMessageBox::Cancel);
        if(retval == QMessageBox::Yes)
        {

			MiningDialog::regsvr("regtlibv12.exe","boinc.tlb","-u");
			MiningDialog::regsvr("regasm.exe","boinc.dll","-u");

			MiningDialog::refreshClicked();

		}
		else 
		{

			// No changes have been made.

		}
   


}


void MiningDialog::exitClicked()
{
    close();
}