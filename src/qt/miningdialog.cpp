#include "miningdialog.h"
#include "ui_miningdialog.h"

#include <QPushButton>

#include <QProcess>
#include <QTimer>
#include <QMessageBox>

#include "clientmodel.h"
#include "clientversion.h"
#include "uint256.h"
#include "base58.h"

#include "global_objects_noui.hpp"

#include "boinc-cpp/boinchelper.h"

// Copyright 10-3-2013

const int MININGDIALOG_COPYRIGHT_YEAR = 2013;

MiningDialog::MiningDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::MiningDialog)
{
    ui->setupUi(this);

    // Set current copyright year
    // ui->copyrightLabel->setText(tr("Copyright") + QString(" &copy; 2009-%1 ").arg(COPYRIGHT_YEAR) + tr("The Bitcoin developers") + QString("<br>") + tr("Copyright") + QString(" &copy; "));

    QTimer *timer = new QTimer(this);

    connect(timer, SIGNAL(timeout()), this, SLOT(refreshClicked()));

    timer->start(1000);

#if defined(LINUX)

    ui->btnRegister->hide();
    ui->btnUnregister->hide();

#endif
}

void MiningDialog::setModel(ClientModel *model)
{
    if (model)
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
	  //Show the boinc homogenized utilization
      ui->lblProcessingPowerValue->setText(QString("%1%").arg(nBoincUtilization));

      BoincHelper &helper = BoincHelper::instance();

      ui->lblThreads->setText(QString::number(helper.threads()));
      ui->lblVersion->setText(QString::number(helper.version()));
}

void MiningDialog::registerClicked()

{
	 QMessageBox::StandardButton retval = QMessageBox::question(this, tr("Please Confirm O/S registration of the mining module?"),
                 tr("GridCoin will now register the mining module to allow the measurement of boinc utilization.  If successful, the mining module version will be above 0.") + "<br><br>" + tr("Continue?"),
                 QMessageBox::Yes|QMessageBox::Cancel,
                 QMessageBox::Cancel);

     if(retval == QMessageBox::Yes)
     {
         BoincHelper::instance().registerBoinc();

         refreshClicked();
     }
     else
     {
         // No changes have been made.
     }
}

void MiningDialog::unregisterClicked()
{
    QMessageBox::StandardButton retval = QMessageBox::question(this,
                                                               tr("Please Confirm un-registration of the mining module?"),
                                                               tr("GridCoin will now un-register the mining module.  Please close and re-start the program to confirm success (the program may be using the DLL).") + "<br><br>" + tr("Continue?"),
                                                               QMessageBox::Yes|QMessageBox::Cancel,
                                                               QMessageBox::Cancel);

    if(retval == QMessageBox::Yes)
    {
        BoincHelper::instance().unregisterBoinc();

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
