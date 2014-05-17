/********************************************************************************
** Form generated from reading UI file 'sendcoinsentry.ui'
**
** Created: Sat May 17 08:20:36 2014
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
        palette1.setBrush(QPalette::Active, QPalette::WindowText, brush);
        palette1.setBrush(QPalette::Active, QPalette::Button, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Midlight, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Mid, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Text, brush);
        palette1.setBrush(QPalette::Active, QPalette::BrightText, brush1);
        palette1.setBrush(QPalette::Active, QPalette::ButtonText, brush);
        palette1.setBrush(QPalette::Active, QPalette::Base, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Window, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Shadow, brush1);
        palette1.setBrush(QPalette::Active, QPalette::HighlightedText, brush);
        QBrush brush3(QColor(4, 4, 4, 255));
        brush3.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::AlternateBase, brush3);
        palette1.setBrush(QPalette::Active, QPalette::NoRole, brush3);
        QBrush brush4(QColor(255, 255, 220, 255));
        brush4.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::ToolTipBase, brush4);
        palette1.setBrush(QPalette::Active, QPalette::ToolTipText, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::WindowText, brush);
        palette1.setBrush(QPalette::Inactive, QPalette::Button, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Midlight, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Mid, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Text, brush);
        palette1.setBrush(QPalette::Inactive, QPalette::BrightText, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::ButtonText, brush);
        palette1.setBrush(QPalette::Inactive, QPalette::Base, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Window, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Shadow, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush);
        palette1.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush3);
        palette1.setBrush(QPalette::Inactive, QPalette::NoRole, brush3);
        palette1.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush4);
        palette1.setBrush(QPalette::Inactive, QPalette::ToolTipText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::WindowText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Button, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Midlight, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Mid, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Text, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::BrightText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::ButtonText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Base, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Window, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Shadow, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush);
        palette1.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush3);
        palette1.setBrush(QPalette::Disabled, QPalette::NoRole, brush3);
        palette1.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush4);
        palette1.setBrush(QPalette::Disabled, QPalette::ToolTipText, brush1);
        payAmount->setPalette(palette1);
        payAmount->setAutoFillBackground(true);

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
        palette2.setBrush(QPalette::Active, QPalette::WindowText, brush);
        QBrush brush5(QColor(33, 33, 33, 255));
        brush5.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Button, brush5);
        QBrush brush6(QColor(49, 49, 49, 255));
        brush6.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Light, brush6);
        QBrush brush7(QColor(41, 41, 41, 255));
        brush7.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Midlight, brush7);
        QBrush brush8(QColor(16, 16, 16, 255));
        brush8.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Dark, brush8);
        QBrush brush9(QColor(22, 22, 22, 255));
        brush9.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Mid, brush9);
        palette2.setBrush(QPalette::Active, QPalette::Text, brush);
        palette2.setBrush(QPalette::Active, QPalette::BrightText, brush);
        palette2.setBrush(QPalette::Active, QPalette::ButtonText, brush);
        QBrush brush10(QColor(5, 10, 17, 255));
        brush10.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Base, brush10);
        QBrush brush11(QColor(20, 20, 20, 255));
        brush11.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Window, brush11);
        palette2.setBrush(QPalette::Active, QPalette::Shadow, brush1);
        palette2.setBrush(QPalette::Active, QPalette::HighlightedText, brush1);
        palette2.setBrush(QPalette::Active, QPalette::AlternateBase, brush8);
        QBrush brush12(QColor(0, 0, 127, 255));
        brush12.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::NoRole, brush12);
        palette2.setBrush(QPalette::Active, QPalette::ToolTipBase, brush1);
        palette2.setBrush(QPalette::Active, QPalette::ToolTipText, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::WindowText, brush);
        palette2.setBrush(QPalette::Inactive, QPalette::Button, brush5);
        palette2.setBrush(QPalette::Inactive, QPalette::Light, brush6);
        palette2.setBrush(QPalette::Inactive, QPalette::Midlight, brush7);
        palette2.setBrush(QPalette::Inactive, QPalette::Dark, brush8);
        palette2.setBrush(QPalette::Inactive, QPalette::Mid, brush9);
        palette2.setBrush(QPalette::Inactive, QPalette::Text, brush);
        palette2.setBrush(QPalette::Inactive, QPalette::BrightText, brush);
        palette2.setBrush(QPalette::Inactive, QPalette::ButtonText, brush);
        palette2.setBrush(QPalette::Inactive, QPalette::Base, brush10);
        palette2.setBrush(QPalette::Inactive, QPalette::Window, brush11);
        palette2.setBrush(QPalette::Inactive, QPalette::Shadow, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush8);
        palette2.setBrush(QPalette::Inactive, QPalette::NoRole, brush12);
        palette2.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::ToolTipText, brush1);
        palette2.setBrush(QPalette::Disabled, QPalette::WindowText, brush8);
        palette2.setBrush(QPalette::Disabled, QPalette::Button, brush5);
        palette2.setBrush(QPalette::Disabled, QPalette::Light, brush6);
        palette2.setBrush(QPalette::Disabled, QPalette::Midlight, brush7);
        palette2.setBrush(QPalette::Disabled, QPalette::Dark, brush8);
        palette2.setBrush(QPalette::Disabled, QPalette::Mid, brush9);
        palette2.setBrush(QPalette::Disabled, QPalette::Text, brush8);
        palette2.setBrush(QPalette::Disabled, QPalette::BrightText, brush);
        palette2.setBrush(QPalette::Disabled, QPalette::ButtonText, brush8);
        palette2.setBrush(QPalette::Disabled, QPalette::Base, brush11);
        palette2.setBrush(QPalette::Disabled, QPalette::Window, brush11);
        palette2.setBrush(QPalette::Disabled, QPalette::Shadow, brush1);
        palette2.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush1);
        palette2.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush5);
        palette2.setBrush(QPalette::Disabled, QPalette::NoRole, brush12);
        palette2.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush1);
        palette2.setBrush(QPalette::Disabled, QPalette::ToolTipText, brush1);
        payTo->setPalette(palette2);
        payTo->setAutoFillBackground(true);
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
        palette3.setBrush(QPalette::Active, QPalette::WindowText, brush);
        palette3.setBrush(QPalette::Active, QPalette::Button, brush);
        palette3.setBrush(QPalette::Active, QPalette::Light, brush1);
        palette3.setBrush(QPalette::Active, QPalette::Midlight, brush12);
        palette3.setBrush(QPalette::Active, QPalette::Dark, brush1);
        palette3.setBrush(QPalette::Active, QPalette::Text, brush);
        palette3.setBrush(QPalette::Active, QPalette::BrightText, brush);
        palette3.setBrush(QPalette::Active, QPalette::ButtonText, brush1);
        palette3.setBrush(QPalette::Active, QPalette::Base, brush1);
        palette3.setBrush(QPalette::Active, QPalette::Window, brush5);
        QBrush brush13(QColor(51, 153, 255, 255));
        brush13.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Active, QPalette::Highlight, brush13);
        palette3.setBrush(QPalette::Active, QPalette::HighlightedText, brush);
        palette3.setBrush(QPalette::Active, QPalette::AlternateBase, brush1);
        palette3.setBrush(QPalette::Active, QPalette::NoRole, brush12);
        QBrush brush14(QColor(0, 255, 255, 255));
        brush14.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Active, QPalette::ToolTipBase, brush14);
        palette3.setBrush(QPalette::Inactive, QPalette::WindowText, brush);
        palette3.setBrush(QPalette::Inactive, QPalette::Button, brush);
        palette3.setBrush(QPalette::Inactive, QPalette::Light, brush1);
        QBrush brush15(QColor(227, 227, 227, 255));
        brush15.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Inactive, QPalette::Midlight, brush15);
        palette3.setBrush(QPalette::Inactive, QPalette::Dark, brush1);
        palette3.setBrush(QPalette::Inactive, QPalette::Text, brush);
        palette3.setBrush(QPalette::Inactive, QPalette::BrightText, brush);
        palette3.setBrush(QPalette::Inactive, QPalette::ButtonText, brush1);
        palette3.setBrush(QPalette::Inactive, QPalette::Base, brush1);
        palette3.setBrush(QPalette::Inactive, QPalette::Window, brush5);
        palette3.setBrush(QPalette::Inactive, QPalette::Highlight, brush1);
        palette3.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush);
        palette3.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush1);
        palette3.setBrush(QPalette::Inactive, QPalette::NoRole, brush12);
        palette3.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush12);
        palette3.setBrush(QPalette::Disabled, QPalette::WindowText, brush1);
        palette3.setBrush(QPalette::Disabled, QPalette::Button, brush);
        palette3.setBrush(QPalette::Disabled, QPalette::Light, brush1);
        QBrush brush16(QColor(247, 247, 247, 255));
        brush16.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Disabled, QPalette::Midlight, brush16);
        palette3.setBrush(QPalette::Disabled, QPalette::Dark, brush1);
        palette3.setBrush(QPalette::Disabled, QPalette::Text, brush1);
        palette3.setBrush(QPalette::Disabled, QPalette::BrightText, brush);
        palette3.setBrush(QPalette::Disabled, QPalette::ButtonText, brush1);
        palette3.setBrush(QPalette::Disabled, QPalette::Base, brush5);
        palette3.setBrush(QPalette::Disabled, QPalette::Window, brush5);
        palette3.setBrush(QPalette::Disabled, QPalette::Highlight, brush13);
        palette3.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush);
        palette3.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush1);
        palette3.setBrush(QPalette::Disabled, QPalette::NoRole, brush12);
        palette3.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush4);
        addAsLabel->setPalette(palette3);
        addAsLabel->setAutoFillBackground(true);

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
