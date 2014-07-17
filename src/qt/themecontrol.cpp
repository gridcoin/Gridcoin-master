#include "themecontrol.h"
#include "optionsmodel.h"

OptionsModel* model;

QStringList alternateStyle = QStringList()
        << "background-color:white; color: black"                   // THEME_BITCOINGUI
        << "background-color:rgb(200,200,200); color: black"        // THEME_WALLETFRAME
        << "background-color:yellow"                                // THEME_RPCCONSOLE
        << "background-color:rgb(30,30,30); color: rgb(0,150,0)"    // THEME_OVERVIEWPAGE   
        << "background-color:rgb(3,132,192); color: rgb(20,20,20)"  // THEME_ADDRESSBOOKPAGE
        << "background-color:yellow; color: rgb(20,20,20)"          // THEME_ABOUTDIALOG
        << "background-color:white; color: rgb(20,20,20)";          // THEME_OPTIONSDIALOG


QStringList defaultStyle = QStringList()
        << "background-color:white; color: default"                 // THEME_BITCOINGUI
        << "background-color:default; color: rgb(0,255,0)"          // THEME_WALLETFRAME
        << "background-color:default"                               // THEME_RPCCONSOLE
        << ""                                                       // THEME_OVERVIEWPAGE
        << ""                                                       // THEME_ADDRESSBOOKPAGE
        << ""                                                       // THEME_ABOUTDIALOG
        << "";                                                      // THEME_OPTIONSDIALOG

void setOptionsModel(OptionsModel *optionsmodel) 
{ 
    model = optionsmodel;
}

void setTheme(QWidget* target, int type)
{
    if (model->getTogglePalette())
    {  
        target->setStyleSheet(alternateStyle.at(type));
    }
    else
    {
        target->setStyleSheet(defaultStyle.at(type));
    }
}
