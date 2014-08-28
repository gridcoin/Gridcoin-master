#include "transactiondescdialog.h"
#include "ui_transactiondescdialog.h"
#include "themecontrol.h"

#include "transactiontablemodel.h"

#include <QModelIndex>

TransactionDescDialog::TransactionDescDialog(const QModelIndex &idx, QWidget *parent) :
    QDialog(parent),
    ui(new Ui::TransactionDescDialog)
{
    ui->setupUi(this);
    QString desc = idx.data(TransactionTableModel::LongDescriptionRole).toString();
    ui->detailText->setHtml(desc);
    applyTheme(this, THEME_TRANSACTIONDESCDIALOG);
}

TransactionDescDialog::~TransactionDescDialog()
{
    delete ui;
}
