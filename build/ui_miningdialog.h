/********************************************************************************
** Form generated from reading UI file 'miningdialog.ui'
**
** Created: Wed Jun 11 09:12:11 2014
**      by: Qt User Interface Compiler version 4.8.4
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_MININGDIALOG_H
#define UI_MININGDIALOG_H

#include <QtCore/QVariant>
#include <QtGui/QAction>
#include <QtGui/QApplication>
#include <QtGui/QButtonGroup>
#include <QtGui/QDialog>
#include <QtGui/QDialogButtonBox>
#include <QtGui/QFrame>
#include <QtGui/QHBoxLayout>
#include <QtGui/QHeaderView>
#include <QtGui/QLabel>
#include <QtGui/QPushButton>
#include <QtGui/QTableView>
#include <QtGui/QVBoxLayout>

QT_BEGIN_NAMESPACE

class Ui_MiningDialog
{
public:
    QHBoxLayout *horizontalLayout_2;
    QVBoxLayout *verticalLayout_2;
    QFrame *frame;
    QTableView *tableView;
    QLabel *lblBoincPower;
    QLabel *lblProcessingPowerValue;
    QLabel *lblBoincPower_2;
    QLabel *lblThreads;
    QDialogButtonBox *buttonBox;
    QPushButton *btnRefresh;
    QLabel *lblBoincPower_3;
    QLabel *lblVersion;
    QPushButton *btnRegister;
    QPushButton *btnExit;
    QPushButton *btnUnregister;

    void setupUi(QDialog *MiningDialog)
    {
        if (MiningDialog->objectName().isEmpty())
            MiningDialog->setObjectName(QString::fromUtf8("MiningDialog"));
        MiningDialog->resize(1126, 594);
        horizontalLayout_2 = new QHBoxLayout(MiningDialog);
        horizontalLayout_2->setObjectName(QString::fromUtf8("horizontalLayout_2"));
        verticalLayout_2 = new QVBoxLayout();
        verticalLayout_2->setObjectName(QString::fromUtf8("verticalLayout_2"));
        frame = new QFrame(MiningDialog);
        frame->setObjectName(QString::fromUtf8("frame"));
        frame->setFrameShape(QFrame::StyledPanel);
        frame->setFrameShadow(QFrame::Raised);
        tableView = new QTableView(frame);
        tableView->setObjectName(QString::fromUtf8("tableView"));
        tableView->setGeometry(QRect(20, 10, 761, 431));
        lblBoincPower = new QLabel(frame);
        lblBoincPower->setObjectName(QString::fromUtf8("lblBoincPower"));
        lblBoincPower->setGeometry(QRect(60, 30, 181, 33));
        lblBoincPower->setMaximumSize(QSize(300, 33));
        QFont font;
        font.setPointSize(10);
        font.setBold(true);
        font.setWeight(75);
        lblBoincPower->setFont(font);
        lblBoincPower->setCursor(QCursor(Qt::IBeamCursor));
        lblBoincPower->setText(QString::fromUtf8("Boinc Processing Power:"));
        lblBoincPower->setTextFormat(Qt::RichText);
        lblBoincPower->setTextInteractionFlags(Qt::LinksAccessibleByMouse|Qt::TextSelectableByKeyboard|Qt::TextSelectableByMouse);
        lblProcessingPowerValue = new QLabel(frame);
        lblProcessingPowerValue->setObjectName(QString::fromUtf8("lblProcessingPowerValue"));
        lblProcessingPowerValue->setGeometry(QRect(280, 30, 222, 33));
        lblProcessingPowerValue->setMaximumSize(QSize(222, 33));
        QFont font1;
        font1.setPointSize(12);
        font1.setBold(true);
        font1.setWeight(75);
        lblProcessingPowerValue->setFont(font1);
        lblBoincPower_2 = new QLabel(frame);
        lblBoincPower_2->setObjectName(QString::fromUtf8("lblBoincPower_2"));
        lblBoincPower_2->setGeometry(QRect(60, 80, 181, 33));
        lblBoincPower_2->setMaximumSize(QSize(300, 33));
        lblBoincPower_2->setCursor(QCursor(Qt::IBeamCursor));
        lblBoincPower_2->setText(QString::fromUtf8("Threads:"));
        lblBoincPower_2->setTextFormat(Qt::RichText);
        lblBoincPower_2->setTextInteractionFlags(Qt::LinksAccessibleByMouse|Qt::TextSelectableByKeyboard|Qt::TextSelectableByMouse);
        lblThreads = new QLabel(frame);
        lblThreads->setObjectName(QString::fromUtf8("lblThreads"));
        lblThreads->setGeometry(QRect(280, 80, 222, 33));
        lblThreads->setMaximumSize(QSize(222, 33));
        buttonBox = new QDialogButtonBox(frame);
        buttonBox->setObjectName(QString::fromUtf8("buttonBox"));
        buttonBox->setGeometry(QRect(-820, 280, 77, 23));
        buttonBox->setMaximumSize(QSize(555, 555));
        buttonBox->setOrientation(Qt::Horizontal);
        buttonBox->setStandardButtons(QDialogButtonBox::Ok);
        btnRefresh = new QPushButton(frame);
        btnRefresh->setObjectName(QString::fromUtf8("btnRefresh"));
        btnRefresh->setGeometry(QRect(60, 370, 75, 23));
        lblBoincPower_3 = new QLabel(frame);
        lblBoincPower_3->setObjectName(QString::fromUtf8("lblBoincPower_3"));
        lblBoincPower_3->setGeometry(QRect(60, 120, 181, 33));
        lblBoincPower_3->setMaximumSize(QSize(300, 33));
        lblBoincPower_3->setCursor(QCursor(Qt::IBeamCursor));
        lblBoincPower_3->setText(QString::fromUtf8("Mining Module Registered Version:"));
        lblBoincPower_3->setTextFormat(Qt::RichText);
        lblBoincPower_3->setTextInteractionFlags(Qt::LinksAccessibleByMouse|Qt::TextSelectableByKeyboard|Qt::TextSelectableByMouse);
        lblVersion = new QLabel(frame);
        lblVersion->setObjectName(QString::fromUtf8("lblVersion"));
        lblVersion->setGeometry(QRect(280, 120, 222, 33));
        lblVersion->setMaximumSize(QSize(222, 33));
        btnRegister = new QPushButton(frame);
        btnRegister->setObjectName(QString::fromUtf8("btnRegister"));
        btnRegister->setGeometry(QRect(150, 370, 121, 23));
        btnExit = new QPushButton(frame);
        btnExit->setObjectName(QString::fromUtf8("btnExit"));
        btnExit->setGeometry(QRect(430, 370, 75, 23));
        btnUnregister = new QPushButton(frame);
        btnUnregister->setObjectName(QString::fromUtf8("btnUnregister"));
        btnUnregister->setGeometry(QRect(280, 370, 141, 23));

        verticalLayout_2->addWidget(frame);


        horizontalLayout_2->addLayout(verticalLayout_2);


        retranslateUi(MiningDialog);
        QObject::connect(buttonBox, SIGNAL(accepted()), MiningDialog, SLOT(accept()));
        QObject::connect(buttonBox, SIGNAL(rejected()), MiningDialog, SLOT(reject()));

        QMetaObject::connectSlotsByName(MiningDialog);
    } // setupUi

    void retranslateUi(QDialog *MiningDialog)
    {
        MiningDialog->setWindowTitle(QApplication::translate("MiningDialog", "Gridcoin - Mining Console", 0, QApplication::UnicodeUTF8));
        lblProcessingPowerValue->setText(QApplication::translate("MiningDialog", "0%", 0, QApplication::UnicodeUTF8));
        lblThreads->setText(QApplication::translate("MiningDialog", "0", 0, QApplication::UnicodeUTF8));
        btnRefresh->setText(QApplication::translate("MiningDialog", "Refresh", 0, QApplication::UnicodeUTF8));
        lblVersion->setText(QApplication::translate("MiningDialog", "False", 0, QApplication::UnicodeUTF8));
        btnRegister->setText(QApplication::translate("MiningDialog", "Register Mining Module", 0, QApplication::UnicodeUTF8));
        btnExit->setText(QApplication::translate("MiningDialog", "Exit", 0, QApplication::UnicodeUTF8));
        btnUnregister->setText(QApplication::translate("MiningDialog", "Un-Register Mining Module", 0, QApplication::UnicodeUTF8));
    } // retranslateUi

};

namespace Ui {
    class MiningDialog: public Ui_MiningDialog {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_MININGDIALOG_H
