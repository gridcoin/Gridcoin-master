/********************************************************************************
** Form generated from reading UI file 'optionsdialog.ui'
**
** Created: Sat Jul 19 14:52:39 2014
**      by: Qt User Interface Compiler version 4.8.4
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_OPTIONSDIALOG_H
#define UI_OPTIONSDIALOG_H

#include <QtCore/QVariant>
#include <QtGui/QAction>
#include <QtGui/QApplication>
#include <QtGui/QButtonGroup>
#include <QtGui/QCheckBox>
#include <QtGui/QDialog>
#include <QtGui/QHBoxLayout>
#include <QtGui/QHeaderView>
#include <QtGui/QLabel>
#include <QtGui/QLineEdit>
#include <QtGui/QPushButton>
#include <QtGui/QSpacerItem>
#include <QtGui/QTabWidget>
#include <QtGui/QVBoxLayout>
#include <QtGui/QWidget>
#include "bitcoinamountfield.h"
#include "qvalidatedlineedit.h"
#include "qvaluecombobox.h"

QT_BEGIN_NAMESPACE

class Ui_OptionsDialog
{
public:
    QVBoxLayout *verticalLayout;
    QTabWidget *tabWidget;
    QWidget *tabMain;
    QVBoxLayout *verticalLayout_Main;
    QLabel *transactionFeeInfoLabel;
    QHBoxLayout *horizontalLayout_1_Main;
    QLabel *transactionFeeLabel;
    BitcoinAmountField *transactionFee;
    QSpacerItem *horizontalSpacer_1_Main;
    QCheckBox *bitcoinAtStartup;
    QSpacerItem *verticalSpacer_Main;
    QHBoxLayout *horizontalLayout_2_Main;
    QSpacerItem *horizontalSpacer_2_Main;
    QPushButton *resetButton;
    QWidget *tabNetwork;
    QVBoxLayout *verticalLayout_Network;
    QCheckBox *mapPortUpnp;
    QCheckBox *connectSocks;
    QHBoxLayout *horizontalLayout_Network;
    QLabel *proxyIpLabel;
    QValidatedLineEdit *proxyIp;
    QLabel *proxyPortLabel;
    QLineEdit *proxyPort;
    QLabel *socksVersionLabel;
    QValueComboBox *socksVersion;
    QSpacerItem *horizontalSpacer_Network;
    QSpacerItem *verticalSpacer_Network;
    QWidget *tabWindow;
    QVBoxLayout *verticalLayout_Window;
    QCheckBox *minimizeToTray;
    QCheckBox *minimizeOnClose;
    QSpacerItem *verticalSpacer_Window;
    QWidget *tabDisplay;
    QVBoxLayout *verticalLayout_Display;
    QHBoxLayout *horizontalLayout_1_Display;
    QLabel *langLabel;
    QValueComboBox *lang;
    QHBoxLayout *horizontalLayout_2_Display;
    QLabel *unitLabel;
    QValueComboBox *unit;
    QCheckBox *displayAddresses;
    QCheckBox *togglePalette;
    QSpacerItem *verticalSpacer_Display;
    QHBoxLayout *horizontalLayout_Buttons;
    QSpacerItem *horizontalSpacer_1;
    QLabel *statusLabel;
    QSpacerItem *horizontalSpacer_2;
    QPushButton *okButton;
    QPushButton *cancelButton;
    QPushButton *applyButton;

    void setupUi(QDialog *OptionsDialog)
    {
        if (OptionsDialog->objectName().isEmpty())
            OptionsDialog->setObjectName(QString::fromUtf8("OptionsDialog"));
        OptionsDialog->resize(540, 380);
        QPalette palette;
        QBrush brush(QColor(0, 255, 0, 255));
        brush.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::WindowText, brush);
        QBrush brush1(QColor(255, 255, 255, 255));
        brush1.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Base, brush1);
        QBrush brush2(QColor(0, 0, 0, 255));
        brush2.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Window, brush2);
        palette.setBrush(QPalette::Inactive, QPalette::WindowText, brush);
        palette.setBrush(QPalette::Inactive, QPalette::Base, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::Window, brush2);
        QBrush brush3(QColor(120, 120, 120, 255));
        brush3.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Disabled, QPalette::WindowText, brush3);
        palette.setBrush(QPalette::Disabled, QPalette::Base, brush2);
        palette.setBrush(QPalette::Disabled, QPalette::Window, brush2);
        OptionsDialog->setPalette(palette);
        OptionsDialog->setAutoFillBackground(true);
        OptionsDialog->setModal(true);
        verticalLayout = new QVBoxLayout(OptionsDialog);
        verticalLayout->setObjectName(QString::fromUtf8("verticalLayout"));
        tabWidget = new QTabWidget(OptionsDialog);
        tabWidget->setObjectName(QString::fromUtf8("tabWidget"));
        QPalette palette1;
        QBrush brush4(QColor(0, 128, 0, 255));
        brush4.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::WindowText, brush4);
        QLinearGradient gradient(0, 0, 1, 0);
        gradient.setSpread(QGradient::PadSpread);
        gradient.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient.setColorAt(0, QColor(0, 0, 0, 255));
        gradient.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush5(gradient);
        palette1.setBrush(QPalette::Active, QPalette::Button, brush5);
        QBrush brush6(QColor(0, 0, 127, 255));
        brush6.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::Light, brush6);
        palette1.setBrush(QPalette::Active, QPalette::Text, brush4);
        QBrush brush7(QColor(170, 255, 127, 255));
        brush7.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::BrightText, brush7);
        palette1.setBrush(QPalette::Active, QPalette::ButtonText, brush4);
        QLinearGradient gradient1(0, 0, 1, 0);
        gradient1.setSpread(QGradient::PadSpread);
        gradient1.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient1.setColorAt(0, QColor(0, 0, 0, 255));
        gradient1.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush8(gradient1);
        palette1.setBrush(QPalette::Active, QPalette::Base, brush8);
        QLinearGradient gradient2(0, 0, 1, 0);
        gradient2.setSpread(QGradient::PadSpread);
        gradient2.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient2.setColorAt(0, QColor(0, 0, 0, 255));
        gradient2.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush9(gradient2);
        palette1.setBrush(QPalette::Active, QPalette::Window, brush9);
        QBrush brush10(QColor(255, 255, 0, 255));
        brush10.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::HighlightedText, brush10);
        palette1.setBrush(QPalette::Active, QPalette::AlternateBase, brush6);
        QBrush brush11(QColor(0, 0, 255, 255));
        brush11.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::NoRole, brush11);
        palette1.setBrush(QPalette::Inactive, QPalette::WindowText, brush4);
        QLinearGradient gradient3(0, 0, 1, 0);
        gradient3.setSpread(QGradient::PadSpread);
        gradient3.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient3.setColorAt(0, QColor(0, 0, 0, 255));
        gradient3.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush12(gradient3);
        palette1.setBrush(QPalette::Inactive, QPalette::Button, brush12);
        palette1.setBrush(QPalette::Inactive, QPalette::Light, brush6);
        palette1.setBrush(QPalette::Inactive, QPalette::Text, brush4);
        palette1.setBrush(QPalette::Inactive, QPalette::BrightText, brush7);
        palette1.setBrush(QPalette::Inactive, QPalette::ButtonText, brush4);
        QLinearGradient gradient4(0, 0, 1, 0);
        gradient4.setSpread(QGradient::PadSpread);
        gradient4.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient4.setColorAt(0, QColor(0, 0, 0, 255));
        gradient4.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush13(gradient4);
        palette1.setBrush(QPalette::Inactive, QPalette::Base, brush13);
        QLinearGradient gradient5(0, 0, 1, 0);
        gradient5.setSpread(QGradient::PadSpread);
        gradient5.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient5.setColorAt(0, QColor(0, 0, 0, 255));
        gradient5.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush14(gradient5);
        palette1.setBrush(QPalette::Inactive, QPalette::Window, brush14);
        palette1.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush10);
        palette1.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush6);
        palette1.setBrush(QPalette::Inactive, QPalette::NoRole, brush11);
        palette1.setBrush(QPalette::Disabled, QPalette::WindowText, brush4);
        QLinearGradient gradient6(0, 0, 1, 0);
        gradient6.setSpread(QGradient::PadSpread);
        gradient6.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient6.setColorAt(0, QColor(0, 0, 0, 255));
        gradient6.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush15(gradient6);
        palette1.setBrush(QPalette::Disabled, QPalette::Button, brush15);
        palette1.setBrush(QPalette::Disabled, QPalette::Light, brush6);
        palette1.setBrush(QPalette::Disabled, QPalette::Text, brush4);
        palette1.setBrush(QPalette::Disabled, QPalette::BrightText, brush7);
        palette1.setBrush(QPalette::Disabled, QPalette::ButtonText, brush4);
        QLinearGradient gradient7(0, 0, 1, 0);
        gradient7.setSpread(QGradient::PadSpread);
        gradient7.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient7.setColorAt(0, QColor(0, 0, 0, 255));
        gradient7.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush16(gradient7);
        palette1.setBrush(QPalette::Disabled, QPalette::Base, brush16);
        QLinearGradient gradient8(0, 0, 1, 0);
        gradient8.setSpread(QGradient::PadSpread);
        gradient8.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient8.setColorAt(0, QColor(0, 0, 0, 255));
        gradient8.setColorAt(1, QColor(255, 255, 255, 255));
        QBrush brush17(gradient8);
        palette1.setBrush(QPalette::Disabled, QPalette::Window, brush17);
        palette1.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush10);
        palette1.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush6);
        palette1.setBrush(QPalette::Disabled, QPalette::NoRole, brush11);
        tabWidget->setPalette(palette1);
        tabWidget->setAutoFillBackground(false);
        tabWidget->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(255, 255, 255, 255));\n"
"color:green;\n"
""));
        tabWidget->setTabPosition(QTabWidget::North);
        tabMain = new QWidget();
        tabMain->setObjectName(QString::fromUtf8("tabMain"));
        verticalLayout_Main = new QVBoxLayout(tabMain);
        verticalLayout_Main->setObjectName(QString::fromUtf8("verticalLayout_Main"));
        transactionFeeInfoLabel = new QLabel(tabMain);
        transactionFeeInfoLabel->setObjectName(QString::fromUtf8("transactionFeeInfoLabel"));
        transactionFeeInfoLabel->setStyleSheet(QString::fromUtf8("color:lightgreen;\n"
""));
        transactionFeeInfoLabel->setTextFormat(Qt::PlainText);
        transactionFeeInfoLabel->setWordWrap(true);

        verticalLayout_Main->addWidget(transactionFeeInfoLabel);

        horizontalLayout_1_Main = new QHBoxLayout();
        horizontalLayout_1_Main->setObjectName(QString::fromUtf8("horizontalLayout_1_Main"));
        transactionFeeLabel = new QLabel(tabMain);
        transactionFeeLabel->setObjectName(QString::fromUtf8("transactionFeeLabel"));
        transactionFeeLabel->setTextFormat(Qt::PlainText);

        horizontalLayout_1_Main->addWidget(transactionFeeLabel);

        transactionFee = new BitcoinAmountField(tabMain);
        transactionFee->setObjectName(QString::fromUtf8("transactionFee"));

        horizontalLayout_1_Main->addWidget(transactionFee);

        horizontalSpacer_1_Main = new QSpacerItem(40, 20, QSizePolicy::Expanding, QSizePolicy::Minimum);

        horizontalLayout_1_Main->addItem(horizontalSpacer_1_Main);


        verticalLayout_Main->addLayout(horizontalLayout_1_Main);

        bitcoinAtStartup = new QCheckBox(tabMain);
        bitcoinAtStartup->setObjectName(QString::fromUtf8("bitcoinAtStartup"));
        bitcoinAtStartup->setStyleSheet(QString::fromUtf8("color:lightgreen;\n"
""));

        verticalLayout_Main->addWidget(bitcoinAtStartup);

        verticalSpacer_Main = new QSpacerItem(20, 40, QSizePolicy::Minimum, QSizePolicy::Expanding);

        verticalLayout_Main->addItem(verticalSpacer_Main);

        horizontalLayout_2_Main = new QHBoxLayout();
        horizontalLayout_2_Main->setObjectName(QString::fromUtf8("horizontalLayout_2_Main"));
        horizontalSpacer_2_Main = new QSpacerItem(40, 20, QSizePolicy::Expanding, QSizePolicy::Minimum);

        horizontalLayout_2_Main->addItem(horizontalSpacer_2_Main);

        resetButton = new QPushButton(tabMain);
        resetButton->setObjectName(QString::fromUtf8("resetButton"));
        resetButton->setAutoDefault(false);

        horizontalLayout_2_Main->addWidget(resetButton);


        verticalLayout_Main->addLayout(horizontalLayout_2_Main);

        tabWidget->addTab(tabMain, QString());
        tabNetwork = new QWidget();
        tabNetwork->setObjectName(QString::fromUtf8("tabNetwork"));
        verticalLayout_Network = new QVBoxLayout(tabNetwork);
        verticalLayout_Network->setObjectName(QString::fromUtf8("verticalLayout_Network"));
        mapPortUpnp = new QCheckBox(tabNetwork);
        mapPortUpnp->setObjectName(QString::fromUtf8("mapPortUpnp"));

        verticalLayout_Network->addWidget(mapPortUpnp);

        connectSocks = new QCheckBox(tabNetwork);
        connectSocks->setObjectName(QString::fromUtf8("connectSocks"));

        verticalLayout_Network->addWidget(connectSocks);

        horizontalLayout_Network = new QHBoxLayout();
        horizontalLayout_Network->setObjectName(QString::fromUtf8("horizontalLayout_Network"));
        proxyIpLabel = new QLabel(tabNetwork);
        proxyIpLabel->setObjectName(QString::fromUtf8("proxyIpLabel"));
        proxyIpLabel->setTextFormat(Qt::PlainText);

        horizontalLayout_Network->addWidget(proxyIpLabel);

        proxyIp = new QValidatedLineEdit(tabNetwork);
        proxyIp->setObjectName(QString::fromUtf8("proxyIp"));
        proxyIp->setMaximumSize(QSize(140, 16777215));

        horizontalLayout_Network->addWidget(proxyIp);

        proxyPortLabel = new QLabel(tabNetwork);
        proxyPortLabel->setObjectName(QString::fromUtf8("proxyPortLabel"));
        proxyPortLabel->setTextFormat(Qt::PlainText);

        horizontalLayout_Network->addWidget(proxyPortLabel);

        proxyPort = new QLineEdit(tabNetwork);
        proxyPort->setObjectName(QString::fromUtf8("proxyPort"));
        proxyPort->setMaximumSize(QSize(55, 16777215));

        horizontalLayout_Network->addWidget(proxyPort);

        socksVersionLabel = new QLabel(tabNetwork);
        socksVersionLabel->setObjectName(QString::fromUtf8("socksVersionLabel"));
        socksVersionLabel->setTextFormat(Qt::PlainText);

        horizontalLayout_Network->addWidget(socksVersionLabel);

        socksVersion = new QValueComboBox(tabNetwork);
        socksVersion->setObjectName(QString::fromUtf8("socksVersion"));

        horizontalLayout_Network->addWidget(socksVersion);

        horizontalSpacer_Network = new QSpacerItem(40, 20, QSizePolicy::Expanding, QSizePolicy::Minimum);

        horizontalLayout_Network->addItem(horizontalSpacer_Network);


        verticalLayout_Network->addLayout(horizontalLayout_Network);

        verticalSpacer_Network = new QSpacerItem(20, 40, QSizePolicy::Minimum, QSizePolicy::Expanding);

        verticalLayout_Network->addItem(verticalSpacer_Network);

        tabWidget->addTab(tabNetwork, QString());
        tabWindow = new QWidget();
        tabWindow->setObjectName(QString::fromUtf8("tabWindow"));
        verticalLayout_Window = new QVBoxLayout(tabWindow);
        verticalLayout_Window->setObjectName(QString::fromUtf8("verticalLayout_Window"));
        minimizeToTray = new QCheckBox(tabWindow);
        minimizeToTray->setObjectName(QString::fromUtf8("minimizeToTray"));
        minimizeToTray->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));

        verticalLayout_Window->addWidget(minimizeToTray);

        minimizeOnClose = new QCheckBox(tabWindow);
        minimizeOnClose->setObjectName(QString::fromUtf8("minimizeOnClose"));
        minimizeOnClose->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));

        verticalLayout_Window->addWidget(minimizeOnClose);

        verticalSpacer_Window = new QSpacerItem(20, 40, QSizePolicy::Minimum, QSizePolicy::Expanding);

        verticalLayout_Window->addItem(verticalSpacer_Window);

        tabWidget->addTab(tabWindow, QString());
        tabDisplay = new QWidget();
        tabDisplay->setObjectName(QString::fromUtf8("tabDisplay"));
        verticalLayout_Display = new QVBoxLayout(tabDisplay);
        verticalLayout_Display->setObjectName(QString::fromUtf8("verticalLayout_Display"));
        horizontalLayout_1_Display = new QHBoxLayout();
        horizontalLayout_1_Display->setObjectName(QString::fromUtf8("horizontalLayout_1_Display"));
        langLabel = new QLabel(tabDisplay);
        langLabel->setObjectName(QString::fromUtf8("langLabel"));
        langLabel->setTextFormat(Qt::PlainText);

        horizontalLayout_1_Display->addWidget(langLabel);

        lang = new QValueComboBox(tabDisplay);
        lang->setObjectName(QString::fromUtf8("lang"));

        horizontalLayout_1_Display->addWidget(lang);


        verticalLayout_Display->addLayout(horizontalLayout_1_Display);

        horizontalLayout_2_Display = new QHBoxLayout();
        horizontalLayout_2_Display->setObjectName(QString::fromUtf8("horizontalLayout_2_Display"));
        unitLabel = new QLabel(tabDisplay);
        unitLabel->setObjectName(QString::fromUtf8("unitLabel"));
        unitLabel->setTextFormat(Qt::PlainText);

        horizontalLayout_2_Display->addWidget(unitLabel);

        unit = new QValueComboBox(tabDisplay);
        unit->setObjectName(QString::fromUtf8("unit"));

        horizontalLayout_2_Display->addWidget(unit);


        verticalLayout_Display->addLayout(horizontalLayout_2_Display);

        displayAddresses = new QCheckBox(tabDisplay);
        displayAddresses->setObjectName(QString::fromUtf8("displayAddresses"));

        verticalLayout_Display->addWidget(displayAddresses);

        togglePalette = new QCheckBox(tabDisplay);
        togglePalette->setObjectName(QString::fromUtf8("togglePalette"));

        verticalLayout_Display->addWidget(togglePalette);

        verticalSpacer_Display = new QSpacerItem(20, 40, QSizePolicy::Minimum, QSizePolicy::Expanding);

        verticalLayout_Display->addItem(verticalSpacer_Display);

        tabWidget->addTab(tabDisplay, QString());

        verticalLayout->addWidget(tabWidget);

        horizontalLayout_Buttons = new QHBoxLayout();
        horizontalLayout_Buttons->setObjectName(QString::fromUtf8("horizontalLayout_Buttons"));
        horizontalSpacer_1 = new QSpacerItem(40, 48, QSizePolicy::Expanding, QSizePolicy::Minimum);

        horizontalLayout_Buttons->addItem(horizontalSpacer_1);

        statusLabel = new QLabel(OptionsDialog);
        statusLabel->setObjectName(QString::fromUtf8("statusLabel"));
        QFont font;
        font.setBold(true);
        font.setWeight(75);
        statusLabel->setFont(font);
        statusLabel->setTextFormat(Qt::PlainText);
        statusLabel->setWordWrap(true);

        horizontalLayout_Buttons->addWidget(statusLabel);

        horizontalSpacer_2 = new QSpacerItem(40, 48, QSizePolicy::Expanding, QSizePolicy::Minimum);

        horizontalLayout_Buttons->addItem(horizontalSpacer_2);

        okButton = new QPushButton(OptionsDialog);
        okButton->setObjectName(QString::fromUtf8("okButton"));
        okButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(255, 255, 255, 255));\n"
"color:lightgreen;\n"
""));

        horizontalLayout_Buttons->addWidget(okButton);

        cancelButton = new QPushButton(OptionsDialog);
        cancelButton->setObjectName(QString::fromUtf8("cancelButton"));
        cancelButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(255, 255, 255, 255));\n"
"color:lightgreen;\n"
""));
        cancelButton->setAutoDefault(false);

        horizontalLayout_Buttons->addWidget(cancelButton);

        applyButton = new QPushButton(OptionsDialog);
        applyButton->setObjectName(QString::fromUtf8("applyButton"));
        applyButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(255, 255, 255, 255));\n"
"color:lightgreen;\n"
""));
        applyButton->setAutoDefault(false);

        horizontalLayout_Buttons->addWidget(applyButton);


        verticalLayout->addLayout(horizontalLayout_Buttons);

#ifndef QT_NO_SHORTCUT
        transactionFeeLabel->setBuddy(transactionFee);
        proxyIpLabel->setBuddy(proxyIp);
        proxyPortLabel->setBuddy(proxyPort);
        socksVersionLabel->setBuddy(socksVersion);
        langLabel->setBuddy(lang);
        unitLabel->setBuddy(unit);
#endif // QT_NO_SHORTCUT

        retranslateUi(OptionsDialog);

        tabWidget->setCurrentIndex(3);


        QMetaObject::connectSlotsByName(OptionsDialog);
    } // setupUi

    void retranslateUi(QDialog *OptionsDialog)
    {
        OptionsDialog->setWindowTitle(QApplication::translate("OptionsDialog", "Options", 0, QApplication::UnicodeUTF8));
        transactionFeeInfoLabel->setText(QApplication::translate("OptionsDialog", "Optional transaction fee per kB that helps make sure your transactions are processed quickly. Most transactions are 1 kB.", 0, QApplication::UnicodeUTF8));
        transactionFeeLabel->setText(QApplication::translate("OptionsDialog", "Pay transaction &fee", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        bitcoinAtStartup->setToolTip(QApplication::translate("OptionsDialog", "Automatically start Gridcoin after logging in to the system.", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        bitcoinAtStartup->setText(QApplication::translate("OptionsDialog", "&Start Gridcoin on system login", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        resetButton->setToolTip(QApplication::translate("OptionsDialog", "Reset all client options to default.", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        resetButton->setText(QApplication::translate("OptionsDialog", "&Reset Options", 0, QApplication::UnicodeUTF8));
        tabWidget->setTabText(tabWidget->indexOf(tabMain), QApplication::translate("OptionsDialog", "&Main", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        mapPortUpnp->setToolTip(QApplication::translate("OptionsDialog", "Automatically open the Gridcoin client port on the router. This only works when your router supports UPnP and it is enabled.", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        mapPortUpnp->setText(QApplication::translate("OptionsDialog", "Map port using &UPnP", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        connectSocks->setToolTip(QApplication::translate("OptionsDialog", "Connect to the Gridcoin network through a SOCKS proxy (e.g. when connecting through Tor).", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        connectSocks->setText(QApplication::translate("OptionsDialog", "&Connect through SOCKS proxy:", 0, QApplication::UnicodeUTF8));
        proxyIpLabel->setText(QApplication::translate("OptionsDialog", "Proxy &IP:", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        proxyIp->setToolTip(QApplication::translate("OptionsDialog", "IP address of the proxy (e.g. 127.0.0.1)", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        proxyPortLabel->setText(QApplication::translate("OptionsDialog", "&Port:", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        proxyPort->setToolTip(QApplication::translate("OptionsDialog", "Port of the proxy (e.g. 9050)", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        socksVersionLabel->setText(QApplication::translate("OptionsDialog", "SOCKS &Version:", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        socksVersion->setToolTip(QApplication::translate("OptionsDialog", "SOCKS version of the proxy (e.g. 5)", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        tabWidget->setTabText(tabWidget->indexOf(tabNetwork), QApplication::translate("OptionsDialog", "&Network", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        minimizeToTray->setToolTip(QApplication::translate("OptionsDialog", "Show only a tray icon after minimizing the window.", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        minimizeToTray->setText(QApplication::translate("OptionsDialog", "&Minimize to the tray instead of the taskbar", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        minimizeOnClose->setToolTip(QApplication::translate("OptionsDialog", "Minimize instead of exit the application when the window is closed. When this option is enabled, the application will be closed only after selecting Quit in the menu.", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        minimizeOnClose->setText(QApplication::translate("OptionsDialog", "M&inimize on close", 0, QApplication::UnicodeUTF8));
        tabWidget->setTabText(tabWidget->indexOf(tabWindow), QApplication::translate("OptionsDialog", "&Window", 0, QApplication::UnicodeUTF8));
        langLabel->setText(QApplication::translate("OptionsDialog", "User Interface &language:", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        lang->setToolTip(QApplication::translate("OptionsDialog", "The user interface language can be set here. This setting will take effect after restarting Gridcoin.", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        unitLabel->setText(QApplication::translate("OptionsDialog", "&Unit to show amounts in:", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        unit->setToolTip(QApplication::translate("OptionsDialog", "Choose the default subdivision unit to show in the interface and when sending coins.", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
#ifndef QT_NO_TOOLTIP
        displayAddresses->setToolTip(QApplication::translate("OptionsDialog", "Whether to show Gridcoin addresses in the transaction list or not.", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        displayAddresses->setText(QApplication::translate("OptionsDialog", "&Display addresses in transaction list", 0, QApplication::UnicodeUTF8));
        togglePalette->setText(QApplication::translate("OptionsDialog", "Alternative Theme", 0, QApplication::UnicodeUTF8));
        tabWidget->setTabText(tabWidget->indexOf(tabDisplay), QApplication::translate("OptionsDialog", "&Display", 0, QApplication::UnicodeUTF8));
        statusLabel->setText(QString());
        okButton->setText(QApplication::translate("OptionsDialog", "&OK", 0, QApplication::UnicodeUTF8));
        cancelButton->setText(QApplication::translate("OptionsDialog", "&Cancel", 0, QApplication::UnicodeUTF8));
        applyButton->setText(QApplication::translate("OptionsDialog", "&Apply", 0, QApplication::UnicodeUTF8));
    } // retranslateUi

};

namespace Ui {
    class OptionsDialog: public Ui_OptionsDialog {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_OPTIONSDIALOG_H
