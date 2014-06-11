/********************************************************************************
** Form generated from reading UI file 'sendcoinsdialog.ui'
**
** Created: Wed Jun 11 09:12:11 2014
**      by: Qt User Interface Compiler version 4.8.4
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_SENDCOINSDIALOG_H
#define UI_SENDCOINSDIALOG_H

#include <QtCore/QVariant>
#include <QtGui/QAction>
#include <QtGui/QApplication>
#include <QtGui/QButtonGroup>
#include <QtGui/QDialog>
#include <QtGui/QHBoxLayout>
#include <QtGui/QHeaderView>
#include <QtGui/QLabel>
#include <QtGui/QPushButton>
#include <QtGui/QScrollArea>
#include <QtGui/QSpacerItem>
#include <QtGui/QVBoxLayout>
#include <QtGui/QWidget>

QT_BEGIN_NAMESPACE

class Ui_SendCoinsDialog
{
public:
    QVBoxLayout *verticalLayout;
    QScrollArea *scrollArea;
    QWidget *scrollAreaWidgetContents;
    QVBoxLayout *verticalLayout_2;
    QVBoxLayout *entries;
    QSpacerItem *verticalSpacer;
    QHBoxLayout *horizontalLayout;
    QPushButton *addButton;
    QPushButton *clearButton;
    QHBoxLayout *horizontalLayout_2;
    QLabel *label;
    QLabel *labelBalance;
    QSpacerItem *horizontalSpacer;
    QPushButton *sendButton;

    void setupUi(QDialog *SendCoinsDialog)
    {
        if (SendCoinsDialog->objectName().isEmpty())
            SendCoinsDialog->setObjectName(QString::fromUtf8("SendCoinsDialog"));
        SendCoinsDialog->resize(686, 217);
        QPalette palette;
        QBrush brush(QColor(144, 238, 144, 255));
        brush.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::WindowText, brush);
        QBrush brush1(QColor(0, 0, 0, 255));
        brush1.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Button, brush1);
        palette.setBrush(QPalette::Active, QPalette::Text, brush);
        palette.setBrush(QPalette::Active, QPalette::ButtonText, brush);
        palette.setBrush(QPalette::Active, QPalette::Base, brush1);
        palette.setBrush(QPalette::Active, QPalette::Window, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::WindowText, brush);
        palette.setBrush(QPalette::Inactive, QPalette::Button, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::Text, brush);
        palette.setBrush(QPalette::Inactive, QPalette::ButtonText, brush);
        palette.setBrush(QPalette::Inactive, QPalette::Base, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::Window, brush1);
        palette.setBrush(QPalette::Disabled, QPalette::WindowText, brush);
        palette.setBrush(QPalette::Disabled, QPalette::Button, brush1);
        palette.setBrush(QPalette::Disabled, QPalette::Text, brush);
        palette.setBrush(QPalette::Disabled, QPalette::ButtonText, brush);
        palette.setBrush(QPalette::Disabled, QPalette::Base, brush1);
        palette.setBrush(QPalette::Disabled, QPalette::Window, brush1);
        SendCoinsDialog->setPalette(palette);
        SendCoinsDialog->setAutoFillBackground(false);
        SendCoinsDialog->setStyleSheet(QString::fromUtf8("\n"
"background-color:black;\n"
"color:lightgreen;\n"
""));
        verticalLayout = new QVBoxLayout(SendCoinsDialog);
        verticalLayout->setObjectName(QString::fromUtf8("verticalLayout"));
        scrollArea = new QScrollArea(SendCoinsDialog);
        scrollArea->setObjectName(QString::fromUtf8("scrollArea"));
        scrollArea->setWidgetResizable(true);
        scrollAreaWidgetContents = new QWidget();
        scrollAreaWidgetContents->setObjectName(QString::fromUtf8("scrollAreaWidgetContents"));
        scrollAreaWidgetContents->setGeometry(QRect(0, 0, 666, 165));
        verticalLayout_2 = new QVBoxLayout(scrollAreaWidgetContents);
        verticalLayout_2->setContentsMargins(0, 0, 0, 0);
        verticalLayout_2->setObjectName(QString::fromUtf8("verticalLayout_2"));
        entries = new QVBoxLayout();
        entries->setSpacing(6);
        entries->setObjectName(QString::fromUtf8("entries"));

        verticalLayout_2->addLayout(entries);

        verticalSpacer = new QSpacerItem(20, 40, QSizePolicy::Minimum, QSizePolicy::Expanding);

        verticalLayout_2->addItem(verticalSpacer);

        scrollArea->setWidget(scrollAreaWidgetContents);

        verticalLayout->addWidget(scrollArea);

        horizontalLayout = new QHBoxLayout();
        horizontalLayout->setObjectName(QString::fromUtf8("horizontalLayout"));
        addButton = new QPushButton(SendCoinsDialog);
        addButton->setObjectName(QString::fromUtf8("addButton"));
        addButton->setAutoFillBackground(false);
        addButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon;
        icon.addFile(QString::fromUtf8(":/icons/add"), QSize(), QIcon::Normal, QIcon::Off);
        addButton->setIcon(icon);
        addButton->setAutoDefault(false);

        horizontalLayout->addWidget(addButton);

        clearButton = new QPushButton(SendCoinsDialog);
        clearButton->setObjectName(QString::fromUtf8("clearButton"));
        QSizePolicy sizePolicy(QSizePolicy::Minimum, QSizePolicy::Fixed);
        sizePolicy.setHorizontalStretch(0);
        sizePolicy.setVerticalStretch(0);
        sizePolicy.setHeightForWidth(clearButton->sizePolicy().hasHeightForWidth());
        clearButton->setSizePolicy(sizePolicy);
        clearButton->setAutoFillBackground(false);
        clearButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon1;
        icon1.addFile(QString::fromUtf8(":/icons/remove"), QSize(), QIcon::Normal, QIcon::Off);
        clearButton->setIcon(icon1);
        clearButton->setAutoRepeatDelay(300);
        clearButton->setAutoDefault(false);

        horizontalLayout->addWidget(clearButton);

        horizontalLayout_2 = new QHBoxLayout();
        horizontalLayout_2->setSpacing(3);
        horizontalLayout_2->setObjectName(QString::fromUtf8("horizontalLayout_2"));
        label = new QLabel(SendCoinsDialog);
        label->setObjectName(QString::fromUtf8("label"));

        horizontalLayout_2->addWidget(label);

        labelBalance = new QLabel(SendCoinsDialog);
        labelBalance->setObjectName(QString::fromUtf8("labelBalance"));
        labelBalance->setCursor(QCursor(Qt::IBeamCursor));
        labelBalance->setTextInteractionFlags(Qt::LinksAccessibleByMouse|Qt::TextSelectableByKeyboard|Qt::TextSelectableByMouse);

        horizontalLayout_2->addWidget(labelBalance);


        horizontalLayout->addLayout(horizontalLayout_2);

        horizontalSpacer = new QSpacerItem(40, 20, QSizePolicy::Expanding, QSizePolicy::Minimum);

        horizontalLayout->addItem(horizontalSpacer);

        sendButton = new QPushButton(SendCoinsDialog);
        sendButton->setObjectName(QString::fromUtf8("sendButton"));
        sendButton->setMinimumSize(QSize(150, 0));
        QPalette palette1;
        palette1.setBrush(QPalette::Active, QPalette::WindowText, brush);
        QLinearGradient gradient(0, 0, 1, 0);
        gradient.setSpread(QGradient::PadSpread);
        gradient.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient.setColorAt(0, QColor(0, 0, 0, 255));
        gradient.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush2(gradient);
        palette1.setBrush(QPalette::Active, QPalette::Button, brush2);
        QBrush brush3(QColor(0, 0, 127, 255));
        brush3.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::Light, brush3);
        QBrush brush4(QColor(8, 232, 232, 255));
        brush4.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::Dark, brush4);
        palette1.setBrush(QPalette::Active, QPalette::Text, brush);
        palette1.setBrush(QPalette::Active, QPalette::ButtonText, brush);
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
        palette1.setBrush(QPalette::Inactive, QPalette::WindowText, brush);
        QLinearGradient gradient3(0, 0, 1, 0);
        gradient3.setSpread(QGradient::PadSpread);
        gradient3.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient3.setColorAt(0, QColor(0, 0, 0, 255));
        gradient3.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush7(gradient3);
        palette1.setBrush(QPalette::Inactive, QPalette::Button, brush7);
        palette1.setBrush(QPalette::Inactive, QPalette::Light, brush3);
        palette1.setBrush(QPalette::Inactive, QPalette::Dark, brush4);
        palette1.setBrush(QPalette::Inactive, QPalette::Text, brush);
        palette1.setBrush(QPalette::Inactive, QPalette::ButtonText, brush);
        QLinearGradient gradient4(0, 0, 1, 0);
        gradient4.setSpread(QGradient::PadSpread);
        gradient4.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient4.setColorAt(0, QColor(0, 0, 0, 255));
        gradient4.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush8(gradient4);
        palette1.setBrush(QPalette::Inactive, QPalette::Base, brush8);
        QLinearGradient gradient5(0, 0, 1, 0);
        gradient5.setSpread(QGradient::PadSpread);
        gradient5.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient5.setColorAt(0, QColor(0, 0, 0, 255));
        gradient5.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush9(gradient5);
        palette1.setBrush(QPalette::Inactive, QPalette::Window, brush9);
        palette1.setBrush(QPalette::Disabled, QPalette::WindowText, brush);
        QLinearGradient gradient6(0, 0, 1, 0);
        gradient6.setSpread(QGradient::PadSpread);
        gradient6.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient6.setColorAt(0, QColor(0, 0, 0, 255));
        gradient6.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush10(gradient6);
        palette1.setBrush(QPalette::Disabled, QPalette::Button, brush10);
        palette1.setBrush(QPalette::Disabled, QPalette::Light, brush3);
        palette1.setBrush(QPalette::Disabled, QPalette::Dark, brush4);
        palette1.setBrush(QPalette::Disabled, QPalette::Text, brush);
        palette1.setBrush(QPalette::Disabled, QPalette::ButtonText, brush);
        QLinearGradient gradient7(0, 0, 1, 0);
        gradient7.setSpread(QGradient::PadSpread);
        gradient7.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient7.setColorAt(0, QColor(0, 0, 0, 255));
        gradient7.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush11(gradient7);
        palette1.setBrush(QPalette::Disabled, QPalette::Base, brush11);
        QLinearGradient gradient8(0, 0, 1, 0);
        gradient8.setSpread(QGradient::PadSpread);
        gradient8.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient8.setColorAt(0, QColor(0, 0, 0, 255));
        gradient8.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush12(gradient8);
        palette1.setBrush(QPalette::Disabled, QPalette::Window, brush12);
        sendButton->setPalette(palette1);
        sendButton->setAutoFillBackground(false);
        sendButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon2;
        icon2.addFile(QString::fromUtf8(":/icons/send"), QSize(), QIcon::Normal, QIcon::Off);
        sendButton->setIcon(icon2);
        sendButton->setDefault(true);

        horizontalLayout->addWidget(sendButton);


        verticalLayout->addLayout(horizontalLayout);


        retranslateUi(SendCoinsDialog);

        QMetaObject::connectSlotsByName(SendCoinsDialog);
    } // setupUi

    void retranslateUi(QDialog *SendCoinsDialog)
    {
        SendCoinsDialog->setWindowTitle(QApplication::translate("SendCoinsDialog", "Send Coins", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        addButton->setToolTip(QApplication::translate("SendCoinsDialog", "Send to multiple recipients at once", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        addButton->setText(QApplication::translate("SendCoinsDialog", "Add &Recipient", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        clearButton->setToolTip(QApplication::translate("SendCoinsDialog", "Remove all transaction fields", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        clearButton->setText(QApplication::translate("SendCoinsDialog", "Clear &All", 0, QApplication::UnicodeUTF8));
        label->setText(QApplication::translate("SendCoinsDialog", "Balance:", 0, QApplication::UnicodeUTF8));
        labelBalance->setText(QApplication::translate("SendCoinsDialog", "123.456 LTC", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        sendButton->setToolTip(QApplication::translate("SendCoinsDialog", "Confirm the send action", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        sendButton->setText(QApplication::translate("SendCoinsDialog", "S&end", 0, QApplication::UnicodeUTF8));
    } // retranslateUi

};

namespace Ui {
    class SendCoinsDialog: public Ui_SendCoinsDialog {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_SENDCOINSDIALOG_H
