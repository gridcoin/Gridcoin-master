/********************************************************************************
** Form generated from reading UI file 'addressbookpage.ui'
**
** Created: Tue May 20 20:09:56 2014
**      by: Qt User Interface Compiler version 4.8.4
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_ADDRESSBOOKPAGE_H
#define UI_ADDRESSBOOKPAGE_H

#include <QtCore/QVariant>
#include <QtGui/QAction>
#include <QtGui/QApplication>
#include <QtGui/QButtonGroup>
#include <QtGui/QDialogButtonBox>
#include <QtGui/QHBoxLayout>
#include <QtGui/QHeaderView>
#include <QtGui/QLabel>
#include <QtGui/QPushButton>
#include <QtGui/QSpacerItem>
#include <QtGui/QTableView>
#include <QtGui/QVBoxLayout>
#include <QtGui/QWidget>

QT_BEGIN_NAMESPACE

class Ui_AddressBookPage
{
public:
    QVBoxLayout *verticalLayout;
    QLabel *labelExplanation;
    QTableView *tableView;
    QHBoxLayout *horizontalLayout;
    QPushButton *newAddress;
    QPushButton *copyAddress;
    QPushButton *showQRCode;
    QPushButton *signMessage;
    QPushButton *verifyMessage;
    QPushButton *deleteAddress;
    QSpacerItem *horizontalSpacer;
    QPushButton *exportButton;
    QDialogButtonBox *buttonBox;

    void setupUi(QWidget *AddressBookPage)
    {
        if (AddressBookPage->objectName().isEmpty())
            AddressBookPage->setObjectName(QString::fromUtf8("AddressBookPage"));
        AddressBookPage->resize(839, 380);
        QPalette palette;
        QBrush brush(QColor(255, 255, 0, 255));
        brush.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::WindowText, brush);
        QBrush brush1(QColor(0, 255, 0, 255));
        brush1.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Button, brush1);
        QBrush brush2(QColor(0, 255, 127, 255));
        brush2.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Light, brush2);
        QBrush brush3(QColor(170, 170, 127, 255));
        brush3.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Midlight, brush3);
        QBrush brush4(QColor(5, 5, 5, 255));
        brush4.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Dark, brush4);
        QBrush brush5(QColor(7, 7, 7, 255));
        brush5.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Mid, brush5);
        palette.setBrush(QPalette::Active, QPalette::Text, brush1);
        palette.setBrush(QPalette::Active, QPalette::BrightText, brush1);
        palette.setBrush(QPalette::Active, QPalette::ButtonText, brush1);
        QBrush brush6(QColor(8, 2, 9, 255));
        brush6.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Base, brush6);
        QBrush brush7(QColor(18, 18, 18, 255));
        brush7.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Window, brush7);
        QBrush brush8(QColor(0, 0, 0, 255));
        brush8.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::Shadow, brush8);
        QBrush brush9(QColor(40, 40, 40, 255));
        brush9.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::AlternateBase, brush9);
        QBrush brush10(QColor(3, 13, 8, 255));
        brush10.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::NoRole, brush10);
        QBrush brush11(QColor(255, 255, 220, 255));
        brush11.setStyle(Qt::SolidPattern);
        palette.setBrush(QPalette::Active, QPalette::ToolTipBase, brush11);
        palette.setBrush(QPalette::Active, QPalette::ToolTipText, brush8);
        palette.setBrush(QPalette::Inactive, QPalette::WindowText, brush);
        palette.setBrush(QPalette::Inactive, QPalette::Button, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::Light, brush2);
        palette.setBrush(QPalette::Inactive, QPalette::Midlight, brush3);
        palette.setBrush(QPalette::Inactive, QPalette::Dark, brush4);
        palette.setBrush(QPalette::Inactive, QPalette::Mid, brush5);
        palette.setBrush(QPalette::Inactive, QPalette::Text, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::BrightText, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::ButtonText, brush1);
        palette.setBrush(QPalette::Inactive, QPalette::Base, brush6);
        palette.setBrush(QPalette::Inactive, QPalette::Window, brush7);
        palette.setBrush(QPalette::Inactive, QPalette::Shadow, brush8);
        palette.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush9);
        palette.setBrush(QPalette::Inactive, QPalette::NoRole, brush10);
        palette.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush11);
        palette.setBrush(QPalette::Inactive, QPalette::ToolTipText, brush8);
        palette.setBrush(QPalette::Disabled, QPalette::WindowText, brush4);
        palette.setBrush(QPalette::Disabled, QPalette::Button, brush1);
        palette.setBrush(QPalette::Disabled, QPalette::Light, brush2);
        palette.setBrush(QPalette::Disabled, QPalette::Midlight, brush3);
        palette.setBrush(QPalette::Disabled, QPalette::Dark, brush4);
        palette.setBrush(QPalette::Disabled, QPalette::Mid, brush5);
        palette.setBrush(QPalette::Disabled, QPalette::Text, brush4);
        palette.setBrush(QPalette::Disabled, QPalette::BrightText, brush1);
        palette.setBrush(QPalette::Disabled, QPalette::ButtonText, brush4);
        palette.setBrush(QPalette::Disabled, QPalette::Base, brush7);
        palette.setBrush(QPalette::Disabled, QPalette::Window, brush7);
        palette.setBrush(QPalette::Disabled, QPalette::Shadow, brush8);
        palette.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush9);
        palette.setBrush(QPalette::Disabled, QPalette::NoRole, brush10);
        palette.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush11);
        palette.setBrush(QPalette::Disabled, QPalette::ToolTipText, brush8);
        AddressBookPage->setPalette(palette);
        AddressBookPage->setAutoFillBackground(true);
        verticalLayout = new QVBoxLayout(AddressBookPage);
        verticalLayout->setObjectName(QString::fromUtf8("verticalLayout"));
        labelExplanation = new QLabel(AddressBookPage);
        labelExplanation->setObjectName(QString::fromUtf8("labelExplanation"));
        QPalette palette1;
        palette1.setBrush(QPalette::Active, QPalette::WindowText, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Button, brush2);
        palette1.setBrush(QPalette::Active, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Active, QPalette::Text, brush1);
        QBrush brush12(QColor(0, 106, 0, 255));
        brush12.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::ButtonText, brush12);
        QBrush brush13(QColor(1, 8, 2, 255));
        brush13.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::Base, brush13);
        QBrush brush14(QColor(12, 16, 1, 255));
        brush14.setStyle(Qt::SolidPattern);
        palette1.setBrush(QPalette::Active, QPalette::Window, brush14);
        palette1.setBrush(QPalette::Active, QPalette::HighlightedText, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::WindowText, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Button, brush2);
        palette1.setBrush(QPalette::Inactive, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::Text, brush1);
        palette1.setBrush(QPalette::Inactive, QPalette::ButtonText, brush12);
        palette1.setBrush(QPalette::Inactive, QPalette::Base, brush13);
        palette1.setBrush(QPalette::Inactive, QPalette::Window, brush14);
        palette1.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::WindowText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Button, brush2);
        palette1.setBrush(QPalette::Disabled, QPalette::Light, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Dark, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Text, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::ButtonText, brush1);
        palette1.setBrush(QPalette::Disabled, QPalette::Base, brush14);
        palette1.setBrush(QPalette::Disabled, QPalette::Window, brush14);
        palette1.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush1);
        labelExplanation->setPalette(palette1);
        labelExplanation->setAutoFillBackground(true);
        labelExplanation->setTextFormat(Qt::PlainText);
        labelExplanation->setWordWrap(true);

        verticalLayout->addWidget(labelExplanation);

        tableView = new QTableView(AddressBookPage);
        tableView->setObjectName(QString::fromUtf8("tableView"));
        QPalette palette2;
        QBrush brush15(QColor(0, 128, 0, 255));
        brush15.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::WindowText, brush15);
        QLinearGradient gradient(0, 0, 1, 0);
        gradient.setSpread(QGradient::PadSpread);
        gradient.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient.setColorAt(0, QColor(0, 0, 0, 255));
        gradient.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush16(gradient);
        palette2.setBrush(QPalette::Active, QPalette::Button, brush16);
        QBrush brush17(QColor(14, 127, 118, 255));
        brush17.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Dark, brush17);
        palette2.setBrush(QPalette::Active, QPalette::Text, brush15);
        palette2.setBrush(QPalette::Active, QPalette::ButtonText, brush15);
        QLinearGradient gradient1(0, 0, 1, 0);
        gradient1.setSpread(QGradient::PadSpread);
        gradient1.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient1.setColorAt(0, QColor(0, 0, 0, 255));
        gradient1.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush18(gradient1);
        palette2.setBrush(QPalette::Active, QPalette::Base, brush18);
        QLinearGradient gradient2(0, 0, 1, 0);
        gradient2.setSpread(QGradient::PadSpread);
        gradient2.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient2.setColorAt(0, QColor(0, 0, 0, 255));
        gradient2.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush19(gradient2);
        palette2.setBrush(QPalette::Active, QPalette::Window, brush19);
        QBrush brush20(QColor(112, 112, 112, 255));
        brush20.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::Shadow, brush20);
        palette2.setBrush(QPalette::Active, QPalette::HighlightedText, brush1);
        QBrush brush21(QColor(7, 11, 20, 255));
        brush21.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::AlternateBase, brush21);
        QBrush brush22(QColor(17, 22, 6, 255));
        brush22.setStyle(Qt::SolidPattern);
        palette2.setBrush(QPalette::Active, QPalette::NoRole, brush22);
        palette2.setBrush(QPalette::Inactive, QPalette::WindowText, brush15);
        QLinearGradient gradient3(0, 0, 1, 0);
        gradient3.setSpread(QGradient::PadSpread);
        gradient3.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient3.setColorAt(0, QColor(0, 0, 0, 255));
        gradient3.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush23(gradient3);
        palette2.setBrush(QPalette::Inactive, QPalette::Button, brush23);
        palette2.setBrush(QPalette::Inactive, QPalette::Dark, brush17);
        palette2.setBrush(QPalette::Inactive, QPalette::Text, brush15);
        palette2.setBrush(QPalette::Inactive, QPalette::ButtonText, brush15);
        QLinearGradient gradient4(0, 0, 1, 0);
        gradient4.setSpread(QGradient::PadSpread);
        gradient4.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient4.setColorAt(0, QColor(0, 0, 0, 255));
        gradient4.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush24(gradient4);
        palette2.setBrush(QPalette::Inactive, QPalette::Base, brush24);
        QLinearGradient gradient5(0, 0, 1, 0);
        gradient5.setSpread(QGradient::PadSpread);
        gradient5.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient5.setColorAt(0, QColor(0, 0, 0, 255));
        gradient5.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush25(gradient5);
        palette2.setBrush(QPalette::Inactive, QPalette::Window, brush25);
        palette2.setBrush(QPalette::Inactive, QPalette::Shadow, brush20);
        palette2.setBrush(QPalette::Inactive, QPalette::HighlightedText, brush1);
        palette2.setBrush(QPalette::Inactive, QPalette::AlternateBase, brush21);
        palette2.setBrush(QPalette::Inactive, QPalette::NoRole, brush22);
        palette2.setBrush(QPalette::Disabled, QPalette::WindowText, brush15);
        QLinearGradient gradient6(0, 0, 1, 0);
        gradient6.setSpread(QGradient::PadSpread);
        gradient6.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient6.setColorAt(0, QColor(0, 0, 0, 255));
        gradient6.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush26(gradient6);
        palette2.setBrush(QPalette::Disabled, QPalette::Button, brush26);
        palette2.setBrush(QPalette::Disabled, QPalette::Dark, brush17);
        palette2.setBrush(QPalette::Disabled, QPalette::Text, brush15);
        palette2.setBrush(QPalette::Disabled, QPalette::ButtonText, brush15);
        QLinearGradient gradient7(0, 0, 1, 0);
        gradient7.setSpread(QGradient::PadSpread);
        gradient7.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient7.setColorAt(0, QColor(0, 0, 0, 255));
        gradient7.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush27(gradient7);
        palette2.setBrush(QPalette::Disabled, QPalette::Base, brush27);
        QLinearGradient gradient8(0, 0, 1, 0);
        gradient8.setSpread(QGradient::PadSpread);
        gradient8.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient8.setColorAt(0, QColor(0, 0, 0, 255));
        gradient8.setColorAt(1, QColor(34, 34, 34, 255));
        QBrush brush28(gradient8);
        palette2.setBrush(QPalette::Disabled, QPalette::Window, brush28);
        palette2.setBrush(QPalette::Disabled, QPalette::Shadow, brush20);
        palette2.setBrush(QPalette::Disabled, QPalette::HighlightedText, brush1);
        palette2.setBrush(QPalette::Disabled, QPalette::AlternateBase, brush21);
        palette2.setBrush(QPalette::Disabled, QPalette::NoRole, brush22);
        tableView->setPalette(palette2);
        tableView->setContextMenuPolicy(Qt::CustomContextMenu);
        tableView->setAutoFillBackground(true);
        tableView->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(34,34, 34, 255));\n"
"color:green;\n"
""));
        tableView->setTabKeyNavigation(false);
        tableView->setAlternatingRowColors(true);
        tableView->setSelectionMode(QAbstractItemView::SingleSelection);
        tableView->setSelectionBehavior(QAbstractItemView::SelectRows);
        tableView->setSortingEnabled(true);
        tableView->verticalHeader()->setVisible(false);

        verticalLayout->addWidget(tableView);

        horizontalLayout = new QHBoxLayout();
        horizontalLayout->setObjectName(QString::fromUtf8("horizontalLayout"));
        newAddress = new QPushButton(AddressBookPage);
        newAddress->setObjectName(QString::fromUtf8("newAddress"));
        QPalette palette3;
        QBrush brush29(QColor(144, 238, 144, 255));
        brush29.setStyle(Qt::SolidPattern);
        palette3.setBrush(QPalette::Active, QPalette::WindowText, brush29);
        QLinearGradient gradient9(0, 0, 1, 0);
        gradient9.setSpread(QGradient::PadSpread);
        gradient9.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient9.setColorAt(0, QColor(0, 0, 0, 255));
        gradient9.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush30(gradient9);
        palette3.setBrush(QPalette::Active, QPalette::Button, brush30);
        palette3.setBrush(QPalette::Active, QPalette::Text, brush29);
        palette3.setBrush(QPalette::Active, QPalette::ButtonText, brush29);
        QLinearGradient gradient10(0, 0, 1, 0);
        gradient10.setSpread(QGradient::PadSpread);
        gradient10.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient10.setColorAt(0, QColor(0, 0, 0, 255));
        gradient10.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush31(gradient10);
        palette3.setBrush(QPalette::Active, QPalette::Base, brush31);
        QLinearGradient gradient11(0, 0, 1, 0);
        gradient11.setSpread(QGradient::PadSpread);
        gradient11.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient11.setColorAt(0, QColor(0, 0, 0, 255));
        gradient11.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush32(gradient11);
        palette3.setBrush(QPalette::Active, QPalette::Window, brush32);
        palette3.setBrush(QPalette::Inactive, QPalette::WindowText, brush29);
        QLinearGradient gradient12(0, 0, 1, 0);
        gradient12.setSpread(QGradient::PadSpread);
        gradient12.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient12.setColorAt(0, QColor(0, 0, 0, 255));
        gradient12.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush33(gradient12);
        palette3.setBrush(QPalette::Inactive, QPalette::Button, brush33);
        palette3.setBrush(QPalette::Inactive, QPalette::Text, brush29);
        palette3.setBrush(QPalette::Inactive, QPalette::ButtonText, brush29);
        QLinearGradient gradient13(0, 0, 1, 0);
        gradient13.setSpread(QGradient::PadSpread);
        gradient13.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient13.setColorAt(0, QColor(0, 0, 0, 255));
        gradient13.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush34(gradient13);
        palette3.setBrush(QPalette::Inactive, QPalette::Base, brush34);
        QLinearGradient gradient14(0, 0, 1, 0);
        gradient14.setSpread(QGradient::PadSpread);
        gradient14.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient14.setColorAt(0, QColor(0, 0, 0, 255));
        gradient14.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush35(gradient14);
        palette3.setBrush(QPalette::Inactive, QPalette::Window, brush35);
        palette3.setBrush(QPalette::Disabled, QPalette::WindowText, brush29);
        QLinearGradient gradient15(0, 0, 1, 0);
        gradient15.setSpread(QGradient::PadSpread);
        gradient15.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient15.setColorAt(0, QColor(0, 0, 0, 255));
        gradient15.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush36(gradient15);
        palette3.setBrush(QPalette::Disabled, QPalette::Button, brush36);
        palette3.setBrush(QPalette::Disabled, QPalette::Text, brush29);
        palette3.setBrush(QPalette::Disabled, QPalette::ButtonText, brush29);
        QLinearGradient gradient16(0, 0, 1, 0);
        gradient16.setSpread(QGradient::PadSpread);
        gradient16.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient16.setColorAt(0, QColor(0, 0, 0, 255));
        gradient16.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush37(gradient16);
        palette3.setBrush(QPalette::Disabled, QPalette::Base, brush37);
        QLinearGradient gradient17(0, 0, 1, 0);
        gradient17.setSpread(QGradient::PadSpread);
        gradient17.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient17.setColorAt(0, QColor(0, 0, 0, 255));
        gradient17.setColorAt(1, QColor(0, 240, 0, 255));
        QBrush brush38(gradient17);
        palette3.setBrush(QPalette::Disabled, QPalette::Window, brush38);
        newAddress->setPalette(palette3);
        QFont font;
        font.setFamily(QString::fromUtf8("MS PGothic"));
        font.setPointSize(10);
        newAddress->setFont(font);
        newAddress->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(0, 240, 000, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon;
        icon.addFile(QString::fromUtf8(":/icons/add"), QSize(), QIcon::Normal, QIcon::Off);
        newAddress->setIcon(icon);

        horizontalLayout->addWidget(newAddress);

        copyAddress = new QPushButton(AddressBookPage);
        copyAddress->setObjectName(QString::fromUtf8("copyAddress"));
        QPalette palette4;
        palette4.setBrush(QPalette::Active, QPalette::WindowText, brush29);
        QLinearGradient gradient18(0, 0, 1, 0);
        gradient18.setSpread(QGradient::PadSpread);
        gradient18.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient18.setColorAt(0, QColor(0, 0, 0, 255));
        gradient18.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush39(gradient18);
        palette4.setBrush(QPalette::Active, QPalette::Button, brush39);
        palette4.setBrush(QPalette::Active, QPalette::Text, brush29);
        palette4.setBrush(QPalette::Active, QPalette::ButtonText, brush29);
        QLinearGradient gradient19(0, 0, 1, 0);
        gradient19.setSpread(QGradient::PadSpread);
        gradient19.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient19.setColorAt(0, QColor(0, 0, 0, 255));
        gradient19.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush40(gradient19);
        palette4.setBrush(QPalette::Active, QPalette::Base, brush40);
        QLinearGradient gradient20(0, 0, 1, 0);
        gradient20.setSpread(QGradient::PadSpread);
        gradient20.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient20.setColorAt(0, QColor(0, 0, 0, 255));
        gradient20.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush41(gradient20);
        palette4.setBrush(QPalette::Active, QPalette::Window, brush41);
        QBrush brush42(QColor(170, 0, 127, 255));
        brush42.setStyle(Qt::SolidPattern);
        palette4.setBrush(QPalette::Active, QPalette::Shadow, brush42);
        QBrush brush43(QColor(51, 153, 255, 255));
        brush43.setStyle(Qt::SolidPattern);
        palette4.setBrush(QPalette::Active, QPalette::Highlight, brush43);
        QBrush brush44(QColor(0, 0, 127, 255));
        brush44.setStyle(Qt::SolidPattern);
        palette4.setBrush(QPalette::Active, QPalette::ToolTipBase, brush44);
        palette4.setBrush(QPalette::Inactive, QPalette::WindowText, brush29);
        QLinearGradient gradient21(0, 0, 1, 0);
        gradient21.setSpread(QGradient::PadSpread);
        gradient21.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient21.setColorAt(0, QColor(0, 0, 0, 255));
        gradient21.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush45(gradient21);
        palette4.setBrush(QPalette::Inactive, QPalette::Button, brush45);
        palette4.setBrush(QPalette::Inactive, QPalette::Text, brush29);
        palette4.setBrush(QPalette::Inactive, QPalette::ButtonText, brush29);
        QLinearGradient gradient22(0, 0, 1, 0);
        gradient22.setSpread(QGradient::PadSpread);
        gradient22.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient22.setColorAt(0, QColor(0, 0, 0, 255));
        gradient22.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush46(gradient22);
        palette4.setBrush(QPalette::Inactive, QPalette::Base, brush46);
        QLinearGradient gradient23(0, 0, 1, 0);
        gradient23.setSpread(QGradient::PadSpread);
        gradient23.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient23.setColorAt(0, QColor(0, 0, 0, 255));
        gradient23.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush47(gradient23);
        palette4.setBrush(QPalette::Inactive, QPalette::Window, brush47);
        palette4.setBrush(QPalette::Inactive, QPalette::Shadow, brush42);
        palette4.setBrush(QPalette::Inactive, QPalette::Highlight, brush8);
        palette4.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush44);
        palette4.setBrush(QPalette::Disabled, QPalette::WindowText, brush29);
        QLinearGradient gradient24(0, 0, 1, 0);
        gradient24.setSpread(QGradient::PadSpread);
        gradient24.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient24.setColorAt(0, QColor(0, 0, 0, 255));
        gradient24.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush48(gradient24);
        palette4.setBrush(QPalette::Disabled, QPalette::Button, brush48);
        palette4.setBrush(QPalette::Disabled, QPalette::Text, brush29);
        palette4.setBrush(QPalette::Disabled, QPalette::ButtonText, brush29);
        QLinearGradient gradient25(0, 0, 1, 0);
        gradient25.setSpread(QGradient::PadSpread);
        gradient25.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient25.setColorAt(0, QColor(0, 0, 0, 255));
        gradient25.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush49(gradient25);
        palette4.setBrush(QPalette::Disabled, QPalette::Base, brush49);
        QLinearGradient gradient26(0, 0, 1, 0);
        gradient26.setSpread(QGradient::PadSpread);
        gradient26.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient26.setColorAt(0, QColor(0, 0, 0, 255));
        gradient26.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush50(gradient26);
        palette4.setBrush(QPalette::Disabled, QPalette::Window, brush50);
        palette4.setBrush(QPalette::Disabled, QPalette::Shadow, brush42);
        palette4.setBrush(QPalette::Disabled, QPalette::Highlight, brush43);
        palette4.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush44);
        copyAddress->setPalette(palette4);
        copyAddress->setFont(font);
        copyAddress->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon1;
        icon1.addFile(QString::fromUtf8(":/icons/editcopy"), QSize(), QIcon::Normal, QIcon::Off);
        copyAddress->setIcon(icon1);

        horizontalLayout->addWidget(copyAddress);

        showQRCode = new QPushButton(AddressBookPage);
        showQRCode->setObjectName(QString::fromUtf8("showQRCode"));
        QPalette palette5;
        palette5.setBrush(QPalette::Active, QPalette::WindowText, brush29);
        QLinearGradient gradient27(0, 0, 1, 0);
        gradient27.setSpread(QGradient::PadSpread);
        gradient27.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient27.setColorAt(0, QColor(0, 0, 0, 255));
        gradient27.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush51(gradient27);
        palette5.setBrush(QPalette::Active, QPalette::Button, brush51);
        palette5.setBrush(QPalette::Active, QPalette::Text, brush29);
        palette5.setBrush(QPalette::Active, QPalette::ButtonText, brush29);
        QLinearGradient gradient28(0, 0, 1, 0);
        gradient28.setSpread(QGradient::PadSpread);
        gradient28.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient28.setColorAt(0, QColor(0, 0, 0, 255));
        gradient28.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush52(gradient28);
        palette5.setBrush(QPalette::Active, QPalette::Base, brush52);
        QLinearGradient gradient29(0, 0, 1, 0);
        gradient29.setSpread(QGradient::PadSpread);
        gradient29.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient29.setColorAt(0, QColor(0, 0, 0, 255));
        gradient29.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush53(gradient29);
        palette5.setBrush(QPalette::Active, QPalette::Window, brush53);
        palette5.setBrush(QPalette::Active, QPalette::ToolTipBase, brush44);
        palette5.setBrush(QPalette::Inactive, QPalette::WindowText, brush29);
        QLinearGradient gradient30(0, 0, 1, 0);
        gradient30.setSpread(QGradient::PadSpread);
        gradient30.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient30.setColorAt(0, QColor(0, 0, 0, 255));
        gradient30.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush54(gradient30);
        palette5.setBrush(QPalette::Inactive, QPalette::Button, brush54);
        palette5.setBrush(QPalette::Inactive, QPalette::Text, brush29);
        palette5.setBrush(QPalette::Inactive, QPalette::ButtonText, brush29);
        QLinearGradient gradient31(0, 0, 1, 0);
        gradient31.setSpread(QGradient::PadSpread);
        gradient31.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient31.setColorAt(0, QColor(0, 0, 0, 255));
        gradient31.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush55(gradient31);
        palette5.setBrush(QPalette::Inactive, QPalette::Base, brush55);
        QLinearGradient gradient32(0, 0, 1, 0);
        gradient32.setSpread(QGradient::PadSpread);
        gradient32.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient32.setColorAt(0, QColor(0, 0, 0, 255));
        gradient32.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush56(gradient32);
        palette5.setBrush(QPalette::Inactive, QPalette::Window, brush56);
        palette5.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush44);
        palette5.setBrush(QPalette::Disabled, QPalette::WindowText, brush29);
        QLinearGradient gradient33(0, 0, 1, 0);
        gradient33.setSpread(QGradient::PadSpread);
        gradient33.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient33.setColorAt(0, QColor(0, 0, 0, 255));
        gradient33.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush57(gradient33);
        palette5.setBrush(QPalette::Disabled, QPalette::Button, brush57);
        palette5.setBrush(QPalette::Disabled, QPalette::Text, brush29);
        palette5.setBrush(QPalette::Disabled, QPalette::ButtonText, brush29);
        QLinearGradient gradient34(0, 0, 1, 0);
        gradient34.setSpread(QGradient::PadSpread);
        gradient34.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient34.setColorAt(0, QColor(0, 0, 0, 255));
        gradient34.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush58(gradient34);
        palette5.setBrush(QPalette::Disabled, QPalette::Base, brush58);
        QLinearGradient gradient35(0, 0, 1, 0);
        gradient35.setSpread(QGradient::PadSpread);
        gradient35.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient35.setColorAt(0, QColor(0, 0, 0, 255));
        gradient35.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush59(gradient35);
        palette5.setBrush(QPalette::Disabled, QPalette::Window, brush59);
        palette5.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush44);
        showQRCode->setPalette(palette5);
        showQRCode->setFont(font);
        showQRCode->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon2;
        icon2.addFile(QString::fromUtf8(":/icons/qrcode"), QSize(), QIcon::Normal, QIcon::Off);
        showQRCode->setIcon(icon2);

        horizontalLayout->addWidget(showQRCode);

        signMessage = new QPushButton(AddressBookPage);
        signMessage->setObjectName(QString::fromUtf8("signMessage"));
        QPalette palette6;
        palette6.setBrush(QPalette::Active, QPalette::WindowText, brush29);
        QLinearGradient gradient36(0, 0, 1, 0);
        gradient36.setSpread(QGradient::PadSpread);
        gradient36.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient36.setColorAt(0, QColor(0, 0, 0, 255));
        gradient36.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush60(gradient36);
        palette6.setBrush(QPalette::Active, QPalette::Button, brush60);
        palette6.setBrush(QPalette::Active, QPalette::Text, brush29);
        palette6.setBrush(QPalette::Active, QPalette::ButtonText, brush29);
        QLinearGradient gradient37(0, 0, 1, 0);
        gradient37.setSpread(QGradient::PadSpread);
        gradient37.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient37.setColorAt(0, QColor(0, 0, 0, 255));
        gradient37.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush61(gradient37);
        palette6.setBrush(QPalette::Active, QPalette::Base, brush61);
        QLinearGradient gradient38(0, 0, 1, 0);
        gradient38.setSpread(QGradient::PadSpread);
        gradient38.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient38.setColorAt(0, QColor(0, 0, 0, 255));
        gradient38.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush62(gradient38);
        palette6.setBrush(QPalette::Active, QPalette::Window, brush62);
        palette6.setBrush(QPalette::Active, QPalette::ToolTipBase, brush44);
        palette6.setBrush(QPalette::Inactive, QPalette::WindowText, brush29);
        QLinearGradient gradient39(0, 0, 1, 0);
        gradient39.setSpread(QGradient::PadSpread);
        gradient39.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient39.setColorAt(0, QColor(0, 0, 0, 255));
        gradient39.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush63(gradient39);
        palette6.setBrush(QPalette::Inactive, QPalette::Button, brush63);
        palette6.setBrush(QPalette::Inactive, QPalette::Text, brush29);
        palette6.setBrush(QPalette::Inactive, QPalette::ButtonText, brush29);
        QLinearGradient gradient40(0, 0, 1, 0);
        gradient40.setSpread(QGradient::PadSpread);
        gradient40.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient40.setColorAt(0, QColor(0, 0, 0, 255));
        gradient40.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush64(gradient40);
        palette6.setBrush(QPalette::Inactive, QPalette::Base, brush64);
        QLinearGradient gradient41(0, 0, 1, 0);
        gradient41.setSpread(QGradient::PadSpread);
        gradient41.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient41.setColorAt(0, QColor(0, 0, 0, 255));
        gradient41.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush65(gradient41);
        palette6.setBrush(QPalette::Inactive, QPalette::Window, brush65);
        palette6.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush44);
        palette6.setBrush(QPalette::Disabled, QPalette::WindowText, brush29);
        QLinearGradient gradient42(0, 0, 1, 0);
        gradient42.setSpread(QGradient::PadSpread);
        gradient42.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient42.setColorAt(0, QColor(0, 0, 0, 255));
        gradient42.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush66(gradient42);
        palette6.setBrush(QPalette::Disabled, QPalette::Button, brush66);
        palette6.setBrush(QPalette::Disabled, QPalette::Text, brush29);
        palette6.setBrush(QPalette::Disabled, QPalette::ButtonText, brush29);
        QLinearGradient gradient43(0, 0, 1, 0);
        gradient43.setSpread(QGradient::PadSpread);
        gradient43.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient43.setColorAt(0, QColor(0, 0, 0, 255));
        gradient43.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush67(gradient43);
        palette6.setBrush(QPalette::Disabled, QPalette::Base, brush67);
        QLinearGradient gradient44(0, 0, 1, 0);
        gradient44.setSpread(QGradient::PadSpread);
        gradient44.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient44.setColorAt(0, QColor(0, 0, 0, 255));
        gradient44.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush68(gradient44);
        palette6.setBrush(QPalette::Disabled, QPalette::Window, brush68);
        palette6.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush44);
        signMessage->setPalette(palette6);
        signMessage->setFont(font);
        signMessage->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon3;
        icon3.addFile(QString::fromUtf8(":/icons/edit"), QSize(), QIcon::Normal, QIcon::Off);
        signMessage->setIcon(icon3);

        horizontalLayout->addWidget(signMessage);

        verifyMessage = new QPushButton(AddressBookPage);
        verifyMessage->setObjectName(QString::fromUtf8("verifyMessage"));
        QPalette palette7;
        palette7.setBrush(QPalette::Active, QPalette::WindowText, brush29);
        QLinearGradient gradient45(0, 0, 1, 0);
        gradient45.setSpread(QGradient::PadSpread);
        gradient45.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient45.setColorAt(0, QColor(0, 0, 0, 255));
        gradient45.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush69(gradient45);
        palette7.setBrush(QPalette::Active, QPalette::Button, brush69);
        palette7.setBrush(QPalette::Active, QPalette::Text, brush29);
        palette7.setBrush(QPalette::Active, QPalette::ButtonText, brush29);
        QLinearGradient gradient46(0, 0, 1, 0);
        gradient46.setSpread(QGradient::PadSpread);
        gradient46.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient46.setColorAt(0, QColor(0, 0, 0, 255));
        gradient46.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush70(gradient46);
        palette7.setBrush(QPalette::Active, QPalette::Base, brush70);
        QLinearGradient gradient47(0, 0, 1, 0);
        gradient47.setSpread(QGradient::PadSpread);
        gradient47.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient47.setColorAt(0, QColor(0, 0, 0, 255));
        gradient47.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush71(gradient47);
        palette7.setBrush(QPalette::Active, QPalette::Window, brush71);
        palette7.setBrush(QPalette::Active, QPalette::ToolTipBase, brush44);
        palette7.setBrush(QPalette::Inactive, QPalette::WindowText, brush29);
        QLinearGradient gradient48(0, 0, 1, 0);
        gradient48.setSpread(QGradient::PadSpread);
        gradient48.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient48.setColorAt(0, QColor(0, 0, 0, 255));
        gradient48.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush72(gradient48);
        palette7.setBrush(QPalette::Inactive, QPalette::Button, brush72);
        palette7.setBrush(QPalette::Inactive, QPalette::Text, brush29);
        palette7.setBrush(QPalette::Inactive, QPalette::ButtonText, brush29);
        QLinearGradient gradient49(0, 0, 1, 0);
        gradient49.setSpread(QGradient::PadSpread);
        gradient49.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient49.setColorAt(0, QColor(0, 0, 0, 255));
        gradient49.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush73(gradient49);
        palette7.setBrush(QPalette::Inactive, QPalette::Base, brush73);
        QLinearGradient gradient50(0, 0, 1, 0);
        gradient50.setSpread(QGradient::PadSpread);
        gradient50.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient50.setColorAt(0, QColor(0, 0, 0, 255));
        gradient50.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush74(gradient50);
        palette7.setBrush(QPalette::Inactive, QPalette::Window, brush74);
        palette7.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush44);
        palette7.setBrush(QPalette::Disabled, QPalette::WindowText, brush29);
        QLinearGradient gradient51(0, 0, 1, 0);
        gradient51.setSpread(QGradient::PadSpread);
        gradient51.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient51.setColorAt(0, QColor(0, 0, 0, 255));
        gradient51.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush75(gradient51);
        palette7.setBrush(QPalette::Disabled, QPalette::Button, brush75);
        palette7.setBrush(QPalette::Disabled, QPalette::Text, brush29);
        palette7.setBrush(QPalette::Disabled, QPalette::ButtonText, brush29);
        QLinearGradient gradient52(0, 0, 1, 0);
        gradient52.setSpread(QGradient::PadSpread);
        gradient52.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient52.setColorAt(0, QColor(0, 0, 0, 255));
        gradient52.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush76(gradient52);
        palette7.setBrush(QPalette::Disabled, QPalette::Base, brush76);
        QLinearGradient gradient53(0, 0, 1, 0);
        gradient53.setSpread(QGradient::PadSpread);
        gradient53.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient53.setColorAt(0, QColor(0, 0, 0, 255));
        gradient53.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush77(gradient53);
        palette7.setBrush(QPalette::Disabled, QPalette::Window, brush77);
        palette7.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush44);
        verifyMessage->setPalette(palette7);
        verifyMessage->setFont(font);
        verifyMessage->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon4;
        icon4.addFile(QString::fromUtf8(":/icons/transaction_0"), QSize(), QIcon::Normal, QIcon::Off);
        verifyMessage->setIcon(icon4);

        horizontalLayout->addWidget(verifyMessage);

        deleteAddress = new QPushButton(AddressBookPage);
        deleteAddress->setObjectName(QString::fromUtf8("deleteAddress"));
        QPalette palette8;
        palette8.setBrush(QPalette::Active, QPalette::WindowText, brush29);
        QLinearGradient gradient54(0, 0, 1, 0);
        gradient54.setSpread(QGradient::PadSpread);
        gradient54.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient54.setColorAt(0, QColor(0, 0, 0, 255));
        gradient54.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush78(gradient54);
        palette8.setBrush(QPalette::Active, QPalette::Button, brush78);
        palette8.setBrush(QPalette::Active, QPalette::Text, brush29);
        palette8.setBrush(QPalette::Active, QPalette::ButtonText, brush29);
        QLinearGradient gradient55(0, 0, 1, 0);
        gradient55.setSpread(QGradient::PadSpread);
        gradient55.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient55.setColorAt(0, QColor(0, 0, 0, 255));
        gradient55.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush79(gradient55);
        palette8.setBrush(QPalette::Active, QPalette::Base, brush79);
        QLinearGradient gradient56(0, 0, 1, 0);
        gradient56.setSpread(QGradient::PadSpread);
        gradient56.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient56.setColorAt(0, QColor(0, 0, 0, 255));
        gradient56.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush80(gradient56);
        palette8.setBrush(QPalette::Active, QPalette::Window, brush80);
        palette8.setBrush(QPalette::Active, QPalette::ToolTipBase, brush44);
        palette8.setBrush(QPalette::Inactive, QPalette::WindowText, brush29);
        QLinearGradient gradient57(0, 0, 1, 0);
        gradient57.setSpread(QGradient::PadSpread);
        gradient57.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient57.setColorAt(0, QColor(0, 0, 0, 255));
        gradient57.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush81(gradient57);
        palette8.setBrush(QPalette::Inactive, QPalette::Button, brush81);
        palette8.setBrush(QPalette::Inactive, QPalette::Text, brush29);
        palette8.setBrush(QPalette::Inactive, QPalette::ButtonText, brush29);
        QLinearGradient gradient58(0, 0, 1, 0);
        gradient58.setSpread(QGradient::PadSpread);
        gradient58.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient58.setColorAt(0, QColor(0, 0, 0, 255));
        gradient58.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush82(gradient58);
        palette8.setBrush(QPalette::Inactive, QPalette::Base, brush82);
        QLinearGradient gradient59(0, 0, 1, 0);
        gradient59.setSpread(QGradient::PadSpread);
        gradient59.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient59.setColorAt(0, QColor(0, 0, 0, 255));
        gradient59.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush83(gradient59);
        palette8.setBrush(QPalette::Inactive, QPalette::Window, brush83);
        palette8.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush44);
        palette8.setBrush(QPalette::Disabled, QPalette::WindowText, brush29);
        QLinearGradient gradient60(0, 0, 1, 0);
        gradient60.setSpread(QGradient::PadSpread);
        gradient60.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient60.setColorAt(0, QColor(0, 0, 0, 255));
        gradient60.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush84(gradient60);
        palette8.setBrush(QPalette::Disabled, QPalette::Button, brush84);
        palette8.setBrush(QPalette::Disabled, QPalette::Text, brush29);
        palette8.setBrush(QPalette::Disabled, QPalette::ButtonText, brush29);
        QLinearGradient gradient61(0, 0, 1, 0);
        gradient61.setSpread(QGradient::PadSpread);
        gradient61.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient61.setColorAt(0, QColor(0, 0, 0, 255));
        gradient61.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush85(gradient61);
        palette8.setBrush(QPalette::Disabled, QPalette::Base, brush85);
        QLinearGradient gradient62(0, 0, 1, 0);
        gradient62.setSpread(QGradient::PadSpread);
        gradient62.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient62.setColorAt(0, QColor(0, 0, 0, 255));
        gradient62.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush86(gradient62);
        palette8.setBrush(QPalette::Disabled, QPalette::Window, brush86);
        palette8.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush44);
        deleteAddress->setPalette(palette8);
        deleteAddress->setFont(font);
        deleteAddress->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon5;
        icon5.addFile(QString::fromUtf8(":/icons/remove"), QSize(), QIcon::Normal, QIcon::Off);
        deleteAddress->setIcon(icon5);

        horizontalLayout->addWidget(deleteAddress);

        horizontalSpacer = new QSpacerItem(40, 20, QSizePolicy::Expanding, QSizePolicy::Minimum);

        horizontalLayout->addItem(horizontalSpacer);

        exportButton = new QPushButton(AddressBookPage);
        exportButton->setObjectName(QString::fromUtf8("exportButton"));
        QPalette palette9;
        palette9.setBrush(QPalette::Active, QPalette::WindowText, brush29);
        QLinearGradient gradient63(0, 0, 1, 0);
        gradient63.setSpread(QGradient::PadSpread);
        gradient63.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient63.setColorAt(0, QColor(0, 0, 0, 255));
        gradient63.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush87(gradient63);
        palette9.setBrush(QPalette::Active, QPalette::Button, brush87);
        palette9.setBrush(QPalette::Active, QPalette::Text, brush29);
        palette9.setBrush(QPalette::Active, QPalette::ButtonText, brush29);
        QLinearGradient gradient64(0, 0, 1, 0);
        gradient64.setSpread(QGradient::PadSpread);
        gradient64.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient64.setColorAt(0, QColor(0, 0, 0, 255));
        gradient64.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush88(gradient64);
        palette9.setBrush(QPalette::Active, QPalette::Base, brush88);
        QLinearGradient gradient65(0, 0, 1, 0);
        gradient65.setSpread(QGradient::PadSpread);
        gradient65.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient65.setColorAt(0, QColor(0, 0, 0, 255));
        gradient65.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush89(gradient65);
        palette9.setBrush(QPalette::Active, QPalette::Window, brush89);
        palette9.setBrush(QPalette::Active, QPalette::ToolTipBase, brush44);
        palette9.setBrush(QPalette::Inactive, QPalette::WindowText, brush29);
        QLinearGradient gradient66(0, 0, 1, 0);
        gradient66.setSpread(QGradient::PadSpread);
        gradient66.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient66.setColorAt(0, QColor(0, 0, 0, 255));
        gradient66.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush90(gradient66);
        palette9.setBrush(QPalette::Inactive, QPalette::Button, brush90);
        palette9.setBrush(QPalette::Inactive, QPalette::Text, brush29);
        palette9.setBrush(QPalette::Inactive, QPalette::ButtonText, brush29);
        QLinearGradient gradient67(0, 0, 1, 0);
        gradient67.setSpread(QGradient::PadSpread);
        gradient67.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient67.setColorAt(0, QColor(0, 0, 0, 255));
        gradient67.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush91(gradient67);
        palette9.setBrush(QPalette::Inactive, QPalette::Base, brush91);
        QLinearGradient gradient68(0, 0, 1, 0);
        gradient68.setSpread(QGradient::PadSpread);
        gradient68.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient68.setColorAt(0, QColor(0, 0, 0, 255));
        gradient68.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush92(gradient68);
        palette9.setBrush(QPalette::Inactive, QPalette::Window, brush92);
        palette9.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush44);
        palette9.setBrush(QPalette::Disabled, QPalette::WindowText, brush29);
        QLinearGradient gradient69(0, 0, 1, 0);
        gradient69.setSpread(QGradient::PadSpread);
        gradient69.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient69.setColorAt(0, QColor(0, 0, 0, 255));
        gradient69.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush93(gradient69);
        palette9.setBrush(QPalette::Disabled, QPalette::Button, brush93);
        palette9.setBrush(QPalette::Disabled, QPalette::Text, brush29);
        palette9.setBrush(QPalette::Disabled, QPalette::ButtonText, brush29);
        QLinearGradient gradient70(0, 0, 1, 0);
        gradient70.setSpread(QGradient::PadSpread);
        gradient70.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient70.setColorAt(0, QColor(0, 0, 0, 255));
        gradient70.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush94(gradient70);
        palette9.setBrush(QPalette::Disabled, QPalette::Base, brush94);
        QLinearGradient gradient71(0, 0, 1, 0);
        gradient71.setSpread(QGradient::PadSpread);
        gradient71.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient71.setColorAt(0, QColor(0, 0, 0, 255));
        gradient71.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush95(gradient71);
        palette9.setBrush(QPalette::Disabled, QPalette::Window, brush95);
        palette9.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush44);
        exportButton->setPalette(palette9);
        exportButton->setFont(font);
        exportButton->setAutoFillBackground(false);
        exportButton->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        QIcon icon6;
        icon6.addFile(QString::fromUtf8(":/icons/export"), QSize(), QIcon::Normal, QIcon::Off);
        exportButton->setIcon(icon6);

        horizontalLayout->addWidget(exportButton);

        buttonBox = new QDialogButtonBox(AddressBookPage);
        buttonBox->setObjectName(QString::fromUtf8("buttonBox"));
        QSizePolicy sizePolicy(QSizePolicy::Maximum, QSizePolicy::Fixed);
        sizePolicy.setHorizontalStretch(0);
        sizePolicy.setVerticalStretch(0);
        sizePolicy.setHeightForWidth(buttonBox->sizePolicy().hasHeightForWidth());
        buttonBox->setSizePolicy(sizePolicy);
        QPalette palette10;
        palette10.setBrush(QPalette::Active, QPalette::WindowText, brush29);
        QLinearGradient gradient72(0, 0, 1, 0);
        gradient72.setSpread(QGradient::PadSpread);
        gradient72.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient72.setColorAt(0, QColor(0, 0, 0, 255));
        gradient72.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush96(gradient72);
        palette10.setBrush(QPalette::Active, QPalette::Button, brush96);
        palette10.setBrush(QPalette::Active, QPalette::Text, brush29);
        palette10.setBrush(QPalette::Active, QPalette::ButtonText, brush29);
        QLinearGradient gradient73(0, 0, 1, 0);
        gradient73.setSpread(QGradient::PadSpread);
        gradient73.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient73.setColorAt(0, QColor(0, 0, 0, 255));
        gradient73.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush97(gradient73);
        palette10.setBrush(QPalette::Active, QPalette::Base, brush97);
        QLinearGradient gradient74(0, 0, 1, 0);
        gradient74.setSpread(QGradient::PadSpread);
        gradient74.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient74.setColorAt(0, QColor(0, 0, 0, 255));
        gradient74.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush98(gradient74);
        palette10.setBrush(QPalette::Active, QPalette::Window, brush98);
        palette10.setBrush(QPalette::Active, QPalette::ToolTipBase, brush44);
        palette10.setBrush(QPalette::Inactive, QPalette::WindowText, brush29);
        QLinearGradient gradient75(0, 0, 1, 0);
        gradient75.setSpread(QGradient::PadSpread);
        gradient75.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient75.setColorAt(0, QColor(0, 0, 0, 255));
        gradient75.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush99(gradient75);
        palette10.setBrush(QPalette::Inactive, QPalette::Button, brush99);
        palette10.setBrush(QPalette::Inactive, QPalette::Text, brush29);
        palette10.setBrush(QPalette::Inactive, QPalette::ButtonText, brush29);
        QLinearGradient gradient76(0, 0, 1, 0);
        gradient76.setSpread(QGradient::PadSpread);
        gradient76.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient76.setColorAt(0, QColor(0, 0, 0, 255));
        gradient76.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush100(gradient76);
        palette10.setBrush(QPalette::Inactive, QPalette::Base, brush100);
        QLinearGradient gradient77(0, 0, 1, 0);
        gradient77.setSpread(QGradient::PadSpread);
        gradient77.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient77.setColorAt(0, QColor(0, 0, 0, 255));
        gradient77.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush101(gradient77);
        palette10.setBrush(QPalette::Inactive, QPalette::Window, brush101);
        palette10.setBrush(QPalette::Inactive, QPalette::ToolTipBase, brush44);
        palette10.setBrush(QPalette::Disabled, QPalette::WindowText, brush29);
        QLinearGradient gradient78(0, 0, 1, 0);
        gradient78.setSpread(QGradient::PadSpread);
        gradient78.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient78.setColorAt(0, QColor(0, 0, 0, 255));
        gradient78.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush102(gradient78);
        palette10.setBrush(QPalette::Disabled, QPalette::Button, brush102);
        palette10.setBrush(QPalette::Disabled, QPalette::Text, brush29);
        palette10.setBrush(QPalette::Disabled, QPalette::ButtonText, brush29);
        QLinearGradient gradient79(0, 0, 1, 0);
        gradient79.setSpread(QGradient::PadSpread);
        gradient79.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient79.setColorAt(0, QColor(0, 0, 0, 255));
        gradient79.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush103(gradient79);
        palette10.setBrush(QPalette::Disabled, QPalette::Base, brush103);
        QLinearGradient gradient80(0, 0, 1, 0);
        gradient80.setSpread(QGradient::PadSpread);
        gradient80.setCoordinateMode(QGradient::ObjectBoundingMode);
        gradient80.setColorAt(0, QColor(0, 0, 0, 255));
        gradient80.setColorAt(1, QColor(127, 127, 127, 255));
        QBrush brush104(gradient80);
        palette10.setBrush(QPalette::Disabled, QPalette::Window, brush104);
        palette10.setBrush(QPalette::Disabled, QPalette::ToolTipBase, brush44);
        buttonBox->setPalette(palette10);
        buttonBox->setFont(font);
        buttonBox->setStyleSheet(QString::fromUtf8("\n"
"background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255));\n"
"color:lightgreen;\n"
""));
        buttonBox->setStandardButtons(QDialogButtonBox::Ok);

        horizontalLayout->addWidget(buttonBox);


        verticalLayout->addLayout(horizontalLayout);


        retranslateUi(AddressBookPage);

        QMetaObject::connectSlotsByName(AddressBookPage);
    } // setupUi

    void retranslateUi(QWidget *AddressBookPage)
    {
        AddressBookPage->setWindowTitle(QApplication::translate("AddressBookPage", "Address Book", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        tableView->setToolTip(QApplication::translate("AddressBookPage", "Double-click to edit address or label", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
#ifndef QT_NO_TOOLTIP
        newAddress->setToolTip(QApplication::translate("AddressBookPage", "Create a new address", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        newAddress->setText(QApplication::translate("AddressBookPage", "&New Address", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        copyAddress->setToolTip(QApplication::translate("AddressBookPage", "Copy the currently selected address to the system clipboard", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        copyAddress->setText(QApplication::translate("AddressBookPage", "&Copy Address", 0, QApplication::UnicodeUTF8));
        showQRCode->setText(QApplication::translate("AddressBookPage", "Show &QR Code", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        signMessage->setToolTip(QApplication::translate("AddressBookPage", "Sign a message to prove you own a Gridcoin address", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        signMessage->setText(QApplication::translate("AddressBookPage", "Sign &Message", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        verifyMessage->setToolTip(QApplication::translate("AddressBookPage", "Verify a message to ensure it was signed with a specified Gridcoin address", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        verifyMessage->setText(QApplication::translate("AddressBookPage", "&Verify Message", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        deleteAddress->setToolTip(QApplication::translate("AddressBookPage", "Delete the currently selected address from the list", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        deleteAddress->setText(QApplication::translate("AddressBookPage", "&Delete", 0, QApplication::UnicodeUTF8));
#ifndef QT_NO_TOOLTIP
        exportButton->setToolTip(QApplication::translate("AddressBookPage", "Export the data in the current tab to a file", 0, QApplication::UnicodeUTF8));
#endif // QT_NO_TOOLTIP
        exportButton->setText(QApplication::translate("AddressBookPage", "&Export", 0, QApplication::UnicodeUTF8));
    } // retranslateUi

};

namespace Ui {
    class AddressBookPage: public Ui_AddressBookPage {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_ADDRESSBOOKPAGE_H
