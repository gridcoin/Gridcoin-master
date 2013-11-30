#include "aboutdialog.h"
#include "ui_aboutdialog.h"

#include "clientmodel.h"
#include "clientversion.h"

#include "global_objects_noui.hpp"

const int ABOUTDIALOG_COPYRIGHT_YEAR = 2013;

AboutDialog::AboutDialog(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::AboutDialog)
{
    ui->setupUi(this);

    // Set current copyright year and boinc utilization
    QString cr = "Copyright 2009-2013 The Bitcoin/Litecoin/Gridcoin developers";
    std::string sBoincUtilization="";
    sBoincUtilization = strprintf("%d",nBoincUtilization);
	QString qsUtilization = QString::fromUtf8(sBoincUtilization.c_str());
	QString qsRegVersion  = QString::fromUtf8(sRegVer.c_str());
	ui->copyrightLabel->setText("Boinc Utilization: " + qsUtilization + "              " + ", Registered Version: " + qsRegVersion + "             " + cr);
}

void AboutDialog::setModel(ClientModel *model)
{
    if(model)
    {
        ui->versionLabel->setText(model->formatFullVersion());
    }
}

AboutDialog::~AboutDialog()
{
    delete ui;
}

void AboutDialog::on_buttonBox_accepted()
{
	std::string s1 = "Notepad.exe";
	close();
}
