/********************************************************************************
** Form generated from reading UI file 'sendcoinsentry.ui'
**
** Created: Wed Jun 11 09:12:12 2014
**      by: Qt User Interface Compiler version 4.8.4
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_SENDCOINSENTRY_H
#define UI_SENDCOINSENTRY_H

#include <QtCore/QVariant>
#include <QtGui/QAction>
#include <QtGui/QApplication>
#include <QtGui/QButtonGroup>
#include <QtGui/QFrame>
#include <QtGui/QGridLayout>
#include <QtGui/QHBoxLayout>
#include <QtGui/QHeaderView>
#include <QtGui/QLabel>
#include <QtGui/QToolButton>
#include "bitcoinamountfield.h"
#include "qvalidatedlineedit.h"

QT_BEGIN_NAMESPACE

class Ui_SendCoinsEntry
{
public:
    QGridLayout *gridLayout;
    QLabel *label;
    QLabel *label_2;
    BitcoinAmountField *payAmount;
    QLabel *label_4;
    QHBoxLayout *payToLayout;
    QValidatedLineEdit *payTo;
    QToolButton *addressBookButton;
    QToolButton *pasteButton;
    QToolButton *deleteButton;
    QValidatedLineEdit *addAsLabel;

    void setupUi(QFrame *SendCoinsEntry)
    {
        if (SendCoinsEntry->objectName().isEmpty())
            SendCoinsEntry->setObjectName(QString::fromUtf8("SendCoinsEntry"));
        SendCoinsEntry->resize(729, 136);
        QPalette palette;
        QBrush brush(QColor(0, 255, 0, 255));
        brush.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::WindowText, brush);
        QBrush brush1(QColor(0, 0, 0, 255));
        brush1.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Base, brush1);
        palette.setBrush(QPalette::Active, QPalette::Window, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::WindowText, brush);
        palette.setBrush(QPalette::Inactive, QPalette::Base, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::Window, brush1);
        QBrush brush2(QColor(120, 120, 120, 255));
        brush2.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Disabled, QPalette::WindowText, brush2);
        palette.setBrush(QPalette::Disabled, QPalette::Base, brush1);
        palette.setBrush(QPalette::Disabled, QPalette::Window, brush1);
        SendCoinsEntry->setPalette(palette);
        SendCoinsEntry->setAutoFillBackground(true);
        SendCoinsEntry->setFrameShape(QFrame::StyledPanel);
        SendCoinsEntry->setFrameShadow(QFrame::Sunken);
        gridLayout = new QGridLayout(SendCoinsEntry);
        gridLayout->setSpacing(12);
        gridLayout->setObjectName(QString::fromUtf8("gridLayout"));
        label = new QLabel(SendCoinsEntry);
        label->setObjectName(QString::fromUtf8("label"));
        label->setAlignment(Qt::AlignRight|Qt::AlignTrailing|Qt::AlignVCenter);

        gridLayout->addWidget(label, 5, 0, 1, 1);

        label_2 = new QLabel(SendCoinsEntry);
        label_2->setObjectName(QString::fromUtf8("label_2"));
        label_2->setAlignment(Qt::AlignRight|Qt::AlignTrailing|Qt::AlignVCenter);

        gridLayout->addWidget(label_2, 3, 0, 1, 1);

        payAmount = new BitcoinAmountField(SendCoinsEntry);
        payAmount->setObjectName(QString::fromUtf8("payAmount"));
        QPalette palette1;
        QBrush brush3(QColor(144, 238, 144, 255));
        brush3.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::WindowText, brush3);
        QLinearGradient gradient(0, 0, 1, 0);
        gradient.setSpread(QGradient::PadSpread);
        gradient.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient.setColorAt(0, QColor(0, 0, 0, 255));
        gradient.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush4(gradient);
        palette1.setBrush(QPalette::Active, QPalette::Button, brush4);
        palette1.setBrush(QPalette::Active, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Midlight, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Mid, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Text, brush3);
        palette1.setBrush(QPalette::Active, QPalette::BrightText, brush1);
        palette1.setBrush(QPalette::Active, QPalette::ButtonText, brush3);
        QLinearGradient gradient1(0, 0, 1, 0);
        gradient1.setSpread(QGradient::PadSpread);
        gradient1.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient1.setColorAt(0, QColor(0, 0, 0, 255));
        gradient1.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush5(gradient1);
        palette1.setBrush(QPalette::Active, QPalette::Base, brush5);
        QLinearGradient gradient2(0, 0, 1, 0);
        gradient2.setSpread(QGradient::PadSpread);
        gradient2.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient2.setColorAt(0, QColor(0, 0, 0, 255));
        gradient2.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush6(gradient2);
        palette1.setBrush(QPalette::Active, QPalette::Window, brush6);
        palette1.setBrush(QPalette::Active, QPalette::Shadow, brush1);
        palette1.setBrush(QPalette::Active, QPalette::HighlightedText, brush);
        QBrush brush7(QColor(4, 4, 4, 255));
        brush7.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::AlternateBase, brush7);
        palette1.setBrush(QPalette::Active, QPalette::NoRole, brush7);
        QBrush brush8(QColor(255, 255, 220, 255));
        brush8.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::ToolTipBase, brush8);
        palette1.setBrush(QPalette::Active, QPalette::ToolTipText, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::WindowText, brush3);
        QLinearGradient gradient3(0, 0, 1, 0);
        gradient3.setSpread(QGradient::PadSpread);
        gradient3.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient3.setColorAt(0, QColor(0, 0, 0, 255));
        gradient3.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush9(gradient3);
        palette1.setBrush(QPalette::Inactive, QPalette::Button, brush9);
        palette1.setBrush(QPalette::Inactive, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Midlight, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Mid, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Text, brush3);
        palette1.setBrush(QPalette::Inactive, QPalette::BrightText, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::ButtonText, brush3);
        QLinearGradient gradient4(0, 0, 1, 0);
        gradient4.setSpread(QGradient::PadSpread);
        gradient4.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient4.setColorAt(0, QColor(0, 0, 0, 255));
        gradient4.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush10(gradient4);
        palette1.setBrush(QPalette::Inactive, QPalette::Base, brush10);
        QLinearGradient gradient5(0, 0, 1, 0);
        gradient5.setSpread(QGradient::PadSpread);
        gradient5.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient5.setColorAt(0, QColor(0, 0, 0, 255));
        gradient5.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush11(gradient5);
        palette1.setBrush(QPalette::Inactive, QPalette::Window, brush11);
        palette1.setBrush(QPalette::Inactive, QPalette::Shadow, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush);
        palette1.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush7);
        palette1.setBrush(QPalette::Inactive, QPalette::NoRole, brush7);
        palette1.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush8);
        palette1.setBrush(QPalette::Inactive, QPalette::ToolTipText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::WindowText, brush3);
        QLinearGradient gradient6(0, 0, 1, 0);
        gradient6.setSpread(QGradient::PadSpread);
        gradient6.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient6.setColorAt(0, QColor(0, 0, 0, 255));
        gradient6.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush12(gradient6);
        palette1.setBrush(QPalette::Disabled, QPalette::Button, brush12);
        palette1.setBrush(QPalette::Disabled, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Midlight, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Mid, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Text, brush3);
        palette1.setBrush(QPalette::Disabled, QPalette::BrightText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::ButtonText, brush3);
        QLinearGradient gradient7(0, 0, 1, 0);
        gradient7.setSpread(QGradient::PadSpread);
        gradient7.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient7.setColorAt(0, QColor(0, 0, 0, 255));
        gradient7.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush13(gradient7);
        palette1.setBrush(QPalette::Disabled, QPalette::Base, brush13);
        QLinearGradient gradient8(0, 0, 1, 0);
        gradient8.setSpread(QGradient::PadSpread);
        gradient8.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient8.setColorAt(0, QColor(0, 0, 0, 255));
        gradient8.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush14(gradient8);
        palette1.setBrush(QPalette::Disabled, QPalette::Window, brush14);
        palette1.setBrush(QPalette::Disabled, QPalette::Shadow, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush);
        palette1.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush7);
        palette1.setBrush(QPalette::Disabled, QPalette::NoRole, brush7);
        palette1.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush8);
        palette1.setBrush(QPalette::Disabled, QPalette::ToolTipText, brush1);
        payAmount->setPalette(palette1);
        payAmount->setAutoFillBackground(false);
        payAmount->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));

        gridLayout->addWidget(payAmount, 5, 1, 1, 1);

        label_4 = new QLabel(SendCoinsEntry);
        label_4->setObjectName(QString::fromUtf8("label_4"));
        label_4->setAlignment(Qt::AlignRight|Qt::AlignTrailing|Qt::AlignVCenter);

        gridLayout->addWidget(label_4, 4, 0, 1, 1);

        payToLayout = new QHBoxLayout();
        payToLayout->setSpacing(0);
        payToLayout->setObjectName(QString::fromUtf8("payToLayout"));
        payTo = new QValidatedLineEdit(SendCoinsEntry);
        payTo->setObjectName(QString::fromUtf8("payTo"));
        QPalette palette2;
        palette2.setBrush(QPalette::Active, QPalette::WindowText, brush3);
        QLinearGradient gradient9(0, 0, 1, 0);
        gradient9.setSpread(QGradient::PadSpread);
        gradient9.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient9.setColorAt(0, QColor(0, 0, 0, 255));
        gradient9.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush15(gradient9);
        palette2.setBrush(QPalette::Active, QPalette::Button, brush15);
        QBrush brush16(QColor(49, 49, 49, 255));
        brush16.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Light, brush16);
        QBrush brush17(QColor(41, 41, 41, 255));
        brush17.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Midlight, brush17);
        QBrush brush18(QColor(16, 16, 16, 255));
        brush18.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Dark, brush18);
        QBrush brush19(QColor(22, 22, 22, 255));
        brush19.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Mid, brush19);
        palette2.setBrush(QPalette::Active, QPalette::Text, brush3);
        palette2.setBrush(QPalette::Active, QPalette::BrightText, brush);
        palette2.setBrush(QPalette::Active, QPalette::ButtonText, brush3);
        QLinearGradient gradient10(0, 0, 1, 0);
        gradient10.setSpread(QGradient::PadSpread);
        gradient10.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient10.setColorAt(0, QColor(0, 0, 0, 255));
        gradient10.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush20(gradient10);
        palette2.setBrush(QPalette::Active, QPalette::Base, brush20);
        QLinearGradient gradient11(0, 0, 1, 0);
        gradient11.setSpread(QGradient::PadSpread);
        gradient11.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient11.setColorAt(0, QColor(0, 0, 0, 255));
        gradient11.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush21(gradient11);
        palette2.setBrush(QPalette::Active, QPalette::Window, brush21);
        palette2.setBrush(QPalette::Active, QPalette::Shadow, brush1);
        palette2.setBrush(QPalette::Active, QPalette::HighlightedText, brush1);
        palette2.setBrush(QPalette::Active, QPalette::AlternateBase, brush18);
        QBrush brush22(QColor(0, 0, 127, 255));
        brush22.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::NoRole, brush22);
        palette2.setBrush(QPalette::Active, QPalette::ToolTipBase, brush1);
        palette2.setBrush(QPalette::Active, QPalette::ToolTipText, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::WindowText, brush3);
        QLinearGradient gradient12(0, 0, 1, 0);
        gradient12.setSpread(QGradient::PadSpread);
        gradient12.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient12.setColorAt(0, QColor(0, 0, 0, 255));
        gradient12.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush23(gradient12);
        palette2.setBrush(QPalette::Inactive, QPalette::Button, brush23);
        palette2.setBrush(QPalette::Inactive, QPalette::Light, brush16);
        palette2.setBrush(QPalette::Inactive, QPalette::Midlight, brush17);
        palette2.setBrush(QPalette::Inactive, QPalette::Dark, brush18);
        palette2.setBrush(QPalette::Inactive, QPalette::Mid, brush19);
        palette2.setBrush(QPalette::Inactive, QPalette::Text, brush3);
        palette2.setBrush(QPalette::Inactive, QPalette::BrightText, brush);
        palette2.setBrush(QPalette::Inactive, QPalette::ButtonText, brush3);
        QLinearGradient gradient13(0, 0, 1, 0);
        gradient13.setSpread(QGradient::PadSpread);
        gradient13.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient13.setColorAt(0, QColor(0, 0, 0, 255));
        gradient13.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush24(gradient13);
        palette2.setBrush(QPalette::Inactive, QPalette::Base, brush24);
        QLinearGradient gradient14(0, 0, 1, 0);
        gradient14.setSpread(QGradient::PadSpread);
        gradient14.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient14.setColorAt(0, QColor(0, 0, 0, 255));
        gradient14.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush25(gradient14);
        palette2.setBrush(QPalette::Inactive, QPalette::Window, brush25);
        palette2.setBrush(QPalette::Inactive, QPalette::Shadow, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush18);
        palette2.setBrush(QPalette::Inactive, QPalette::NoRole, brush22);
        palette2.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::ToolTipText, brush1);
        palette2.setBrush(QPalette::Disabled, QPalette::WindowText, brush3);
        QLinearGradient gradient15(0, 0, 1, 0);
        gradient15.setSpread(QGradient::PadSpread);
        gradient15.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient15.setColorAt(0, QColor(0, 0, 0, 255));
        gradient15.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush26(gradient15);
        palette2.setBrush(QPalette::Disabled, QPalette::Button, brush26);
        palette2.setBrush(QPalette::Disabled, QPalette::Light, brush16);
        palette2.setBrush(QPalette::Disabled, QPalette::Midlight, brush17);
        palette2.setBrush(QPalette::Disabled, QPalette::Dark, brush18);
        palette2.setBrush(QPalette::Disabled, QPalette::Mid, brush19);
        palette2.setBrush(QPalette::Disabled, QPalette::Text, brush3);
        palette2.setBrush(QPalette::Disabled, QPalette::BrightText, brush);
        palette2.setBrush(QPalette::Disabled, QPalette::ButtonText, brush3);
        QLinearGradient gradient16(0, 0, 1, 0);
        gradient16.setSpread(QGradient::PadSpread);
        gradient16.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient16.setColorAt(0, QColor(0, 0, 0, 255));
        gradient16.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush27(gradient16);
        palette2.setBrush(QPalette::Disabled, QPalette::Base, brush27);
        QLinearGradient gradient17(0, 0, 1, 0);
        gradient17.setSpread(QGradient::PadSpread);
        gradient17.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient17.setColorAt(0, QColor(0, 0, 0, 255));
        gradient17.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush28(gradient17);
        palette2.setBrush(QPalette::Disabled, QPalette::Window, brush28);
        palette2.setBrush(QPalette::Disabled, QPalette::Shadow, brush1);
        palette2.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush1);
        QBrush brush29(QColor(33, 33, 33, 255));
        brush29.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush29);
        palette2.setBrush(QPalette::Disabled, QPalette::NoRole, brush22);
        palette2.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush1);
        palette2.setBrush(QPalette::Disabled, QPalette::ToolTipText, brush1);
        payTo->setPalette(palette2);
        payTo->setAutoFillBackground(false);
        payTo->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        payTo->setMaxLength(34);

        payToLayout->addWidget(payTo);

        addressBookButton = new QToolButton(SendCoinsEntry);
        addressBookButton->setObjectName(QString::fromUtf8("addressBookButton"));
        addressBookButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon;
        icon.addFile(QString::fromUtf8(":/icons/address-book"), QSize(), QIcon::Normal, QIcon::Off);
        addressBookButton->setIcon(icon);

        payToLayout->addWidget(addressBookButton);

        pasteButton = new QToolButton(SendCoinsEntry);
        pasteButton->setObjectName(QString::fromUtf8("pasteButton"));
        pasteButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon1;
        icon1.addFile(QString::fromUtf8(":/icons/editpaste"), QSize(), QIcon::Normal, QIcon::Off);
        pasteButton->setIcon(icon1);

        payToLayout->addWidget(pasteButton);

        deleteButton = new QToolButton(SendCoinsEntry);
        deleteButton->setObjectName(QString::fromUtf8("deleteButton"));
        deleteButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon2;
        icon2.addFile(QString::fromUtf8(":/icons/remove"), QSize(), QIcon::Normal, QIcon::Off);
        deleteButton->setIcon(icon2);

        payToLayout->addWidget(deleteButton);


        gridLayout->addLayout(payToLayout, 3, 1, 1, 1);

        addAsLabel = new QValidatedLineEdit(SendCoinsEntry);
        addAsLabel->setObjectName(QString::fromUtf8("addAsLabel"));
        QPalette palette3;
        palette3.setBrush(QPalette::Active, QPalette::WindowText, brush3);
        QLinearGradient gradient18(0, 0, 1, 0);
        gradient18.setSpread(QGradient::PadSpread);
        gradient18.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient18.setColorAt(0, QColor(0, 0, 0, 255));
        gradient18.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush30(gradient18);
        palette3.setBrush(QPalette::Active, QPalette::Button, brush30);
        palette3.setBrush(QPalette::Active, QPalette::Light, brush1);
        palette3.setBrush(QPalette::Active, QPalette::Midlight, brush22);
        palette3.setBrush(QPalette::Active, QPalette::Dark, brush1);
        palette3.setBrush(QPalette::Active, QPalette::Text, brush3);
        palette3.setBrush(QPalette::Active, QPalette::BrightText, brush);
        palette3.setBrush(QPalette::Active, QPalette::ButtonText, brush3);
        QLinearGradient gradient19(0, 0, 1, 0);
        gradient19.setSpread(QGradient::PadSpread);
        gradient19.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient19.setColorAt(0, QColor(0, 0, 0, 255));
        gradient19.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush31(gradient19);
        palette3.setBrush(QPalette::Active, QPalette::Base, brush31);
        QLinearGradient gradient20(0, 0, 1, 0);
        gradient20.setSpread(QGradient::PadSpread);
        gradient20.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient20.setColorAt(0, QColor(0, 0, 0, 255));
        gradient20.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush32(gradient20);
        palette3.setBrush(QPalette::Active, QPalette::Window, brush32);
        QBrush brush33(QColor(51, 153, 255, 255));
        brush33.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Active, QPalette::Highlight, brush33);
        palette3.setBrush(QPalette::Active, QPalette::HighlightedText, brush);
        palette3.setBrush(QPalette::Active, QPalette::AlternateBase, brush1);
        palette3.setBrush(QPalette::Active, QPalette::NoRole, brush22);
        QBrush brush34(QColor(0, 255, 255, 255));
        brush34.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Active, QPalette::ToolTipBase, brush34);
        palette3.setBrush(QPalette::Inactive, QPalette::WindowText, brush3);
        QLinearGradient gradient21(0, 0, 1, 0);
        gradient21.setSpread(QGradient::PadSpread);
        gradient21.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient21.setColorAt(0, QColor(0, 0, 0, 255));
        gradient21.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush35(gradient21);
        palette3.setBrush(QPalette::Inactive, QPalette::Button, brush35);
        palette3.setBrush(QPalette::Inactive, QPalette::Light, brush1);
        QBrush brush36(QColor(227, 227, 227, 255));
        brush36.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Inactive, QPalette::Midlight, brush36);
        palette3.setBrush(QPalette::Inactive, QPalette::Dark, brush1);
        palette3.setBrush(QPalette::Inactive, QPalette::Text, brush3);
        palette3.setBrush(QPalette::Inactive, QPalette::BrightText, brush);
        palette3.setBrush(QPalette::Inactive, QPalette::ButtonText, brush3);
        QLinearGradient gradient22(0, 0, 1, 0);
        gradient22.setSpread(QGradient::PadSpread);
        gradient22.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient22.setColorAt(0, QColor(0, 0, 0, 255));
        gradient22.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush37(gradient22);
        palette3.setBrush(QPalette::Inactive, QPalette::Base, brush37);
        QLinearGradient gradient23(0, 0, 1, 0);
        gradient23.setSpread(QGradient::PadSpread);
        gradient23.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient23.setColorAt(0, QColor(0, 0, 0, 255));
        gradient23.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush38(gradient23);
        palette3.setBrush(QPalette::Inactive, QPalette::Window, brush38);
        palette3.setBrush(QPalette::Inactive, QPalette::Highlight, brush1);
        palette3.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush);
        palette3.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush1);
        palette3.setBrush(QPalette::Inactive, QPalette::NoRole, brush22);
        palette3.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush22);
        palette3.setBrush(QPalette::Disabled, QPalette::WindowText, brush3);
        QLinearGradient gradient24(0, 0, 1, 0);
        gradient24.setSpread(QGradient::PadSpread);
        gradient24.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient24.setColorAt(0, QColor(0, 0, 0, 255));
        gradient24.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush39(gradient24);
        palette3.setBrush(QPalette::Disabled, QPalette::Button, brush39);
        palette3.setBrush(QPalette::Disabled, QPalette::Light, brush1);
        QBrush brush40(QColor(247, 247, 247, 255));
        brush40.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Disabled, QPalette::Midlight, brush40);
        palette3.setBrush(QPalette::Disabled, QPalette::Dark, brush1);
        palette3.setBrush(QPalette::Disabled, QPalette::Text, brush3);
        palette3.setBrush(QPalette::Disabled, QPalette::BrightText, brush);
        palette3.setBrush(QPalette::Disabled, QPalette::ButtonText, brush3);
        QLinearGradient gradient25(0, 0, 1, 0);
        gradient25.setSpread(QGradient::PadSpread);
        gradient25.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient25.setColorAt(0, QColor(0, 0, 0, 255));
        gradient25.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush41(gradient25);
        palette3.setBrush(QPalette::Disabled, QPalette::Base, brush41);
        QLinearGradient gradient26(0, 0, 1, 0);
        gradient26.setSpread(QGradient::PadSpread);
        gradient26.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient26.setColorAt(0, QColor(0, 0, 0, 255));
        gradient26.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush42(gradient26);
        palette3.setBrush(QPalette::Disabled, QPalette::Window, brush42);
        palette3.setBrush(QPalette::Disabled, QPalette::Highlight, brush33);
        palette3.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush);
        palette3.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush1);
        palette3.setBrush(QPalette::Disabled, QPalette::NoRole, brush22);
        palette3.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush8);
        addAsLabel->setPalette(palette3);
        addAsLabel->setAutoFillBackground(false);
        addAsLabel->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));

        gridLayout->addWidget(addAsLabel, 4, 1, 1, 1);

#ifndef QT_NO_SHORTCUT
        label->setBuddy(payAmount);
        label_2->setBuddy(payTo);
        label_4->setBuddy(addAsLabel);
#endif // QT_NO_SHORTCUT

        retranslateUi(SendCoinsEntry);

        QMetaObject::connectSlotsByName(SendCoinsEntry);
    } // setupUi

    void retranslateUi(QFrame *SendCoinsEntry)
    {
        SendCoinsEntry->setWindowTitle(QApplication::translate("SendCoinsEntry", "Form", 0, QApplication::UnicodeUTF8));
        label->setText(QApplication::translate("SendCoinsEntry", "A&mount:", 0, QApplication::UnicodeUTF8));
        label_2->setText(QApplication::translate("SendCoinsEntry", "Pay &To:", 0, QApplication::UnicodeUTF8));
        label_4->setText(QApplication::translate("SendCoinsEntry", "&Label:", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        payTo->setToolTip(QApplication::translate("SendCoinsEntry", "The address to send the payment to (e.g. 1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L)", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
#ifndef QT_NO_TOOLTIP
        addressBookButton->setToolTip(QApplication::translate("SendCoinsEntry", "Choose address from address book", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        addressBookButton->setText(QString());
        addressBookButton->setShortcut(QApplication::translate("SendCoinsEntry", "Alt+A", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        pasteButton->setToolTip(QApplication::translate("SendCoinsEntry", "Paste address from clipboard", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        pasteButton->setText(QString());
        pasteButton->setShortcut(QApplication::translate("SendCoinsEntry", "Alt+P", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        deleteButton->setToolTip(QApplication::translate("SendCoinsEntry", "Remove this recipient", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        deleteButton->setText(QString());
#ifndef QT_NO_TOOLTIP
        addAsLabel->setToolTip(QApplication::translate("SendCoinsEntry", "Enter a label for this address to add it to your address book", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
    } // retranslateUi

};

namespace Ui {
    class SendCoinsEntry: public Ui_SendCoinsEntry {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_SENDCOINSENTRY_H
