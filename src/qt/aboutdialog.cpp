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
    ui->labelUtilization->setText(tr("Boinc Utilization: %1").arg(nBoincUtilization));

    ui->copyrightLabel->setText(tr("Copyright 2009-%1 The Bitcoin/Litecoin/Gridcoin developers")
                                .arg(ABOUTDIALOG_COPYRIGHT_YEAR));
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
