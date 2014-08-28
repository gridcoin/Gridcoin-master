#include "themecontrol.h"
#include "optionsmodel.h"
#include <QList>
#include <QString>
#include <QStringList>
#include <sys/types.h>

QStringList Style;

void initStyle()
{
    for (int i = 0; i < ELEMENTS; i++) 
    {Style << ""; }
    setTheme();
}

void alternateStyle()
{
    QString def_font= "font: 12pt;";
    QString def_background = "background-color:rgb(30,30,30);";
    QString def_color = "color:rgb(30,180,30);";
    QString def_color_light = "color:lightgreen;";
    QString def_gradient_moderate = "background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(30, 30, 30, 255), stop:1 rgba(127, 127, 127, 255)); "; 
    QString def_gradient_extreme = "background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(30, 30, 30, 255), stop:1 rgba(255, 255, 255, 255)); "; 

    
        Style.replace(THEME_ABOUTDIALOG, def_gradient_moderate+def_color_light);

        Style.replace(THEME_ADDRESSBOOKPAGE, def_background);
        Style.replace(THEME_ADDRESSBOOKPAGE_TABLE, def_gradient_extreme);
        Style.replace(THEME_ADDRESSBOOKPAGE_BUTTON_A, def_gradient_extreme);
        Style.replace(THEME_ADDRESSBOOKPAGE_BUTTON_B, "background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(0, 240, 100, 255));");

        Style.replace(THEME_ALT_TEXT, def_color_light);
        
        Style.replace(THEME_ASKPASSPHRASEDIALOG, def_background+"color: rgb(0,150,150)");
        Style.replace(THEME_ASKPASSPHRASEDIALOG_BUTTON, def_gradient_moderate);
        
        Style.replace(THEME_BITCOINGUI,"color: black");
        
        Style.replace(THEME_EDITADDRESSDIALOG, "");
        
        Style.replace(THEME_MININGDIALOG,def_background+def_color);
        
        Style.replace(THEME_OPTIONSDIALOG, def_background);
        Style.replace(THEME_OPTIONSDIALOG_TAB, def_gradient_extreme+def_color);
        Style.replace(THEME_OPTIONSDIALOG_BUTTON, def_gradient_extreme+def_color);
        
        Style.replace(THEME_OVERVIEWPAGE, "");
        
        Style.replace(THEME_RECEIVECOINSPAGE, def_background);
        Style.replace(THEME_RECEIVECOINSPAGE_TABLE, def_gradient_extreme);
        Style.replace(THEME_RECEIVECOINSPAGE_BUTTON_A, def_gradient_extreme);
        Style.replace(THEME_RECEIVECOINSPAGE_BUTTON_B, "background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(0, 240, 100, 255));");
        
        Style.replace(THEME_RPCCONSOLE, def_gradient_moderate);
        Style.replace(THEME_RPCCONSOLE_TAB, def_background+def_color_light);
        Style.replace(THEME_RPCCONSOLE_CONSOLE, def_gradient_extreme+def_color);
        Style.replace(THEME_RPCCONSOLE_SUBTITLE, "color:yellow");
        Style.replace(THEME_RPCCONSOLE_BUTTON, def_gradient_moderate+def_color);
        Style.replace(THEME_RPCCONSOLE_LABEL, def_color);

        Style.replace(THEME_SENDCOINSPAGE, def_background);
        Style.replace(THEME_SENDCOINSPAGE_BUTTON, def_gradient_moderate);

        Style.replace(THEME_SENDCOINSENTRY, def_background);
        Style.replace(THEME_SENDCOINSENTRY_BUTTON, def_gradient_extreme);
        Style.replace(THEME_SENDCOINSENTRY_LINE, def_gradient_moderate);

        Style.replace(THEME_SIGNVERIFYDIALOG, def_gradient_extreme);
        Style.replace(THEME_SIGNVERIFYDIALOG_TAB, def_background+def_color);
        Style.replace(THEME_SIGNVERIFYDIALOG_BUTTON, def_gradient_moderate);

        Style.replace(THEME_TRANSACTIONDESCDIALOG, def_gradient_moderate+"color:blue");

        Style.replace(THEME_TRANSACTIONVIEW, def_background+"alternate-background-color: rgb(60,55,65);"+def_color);
        Style.replace(THEME_TRANSACTIONVIEW_HEADER, "color: red;");
        
        Style.replace(THEME_WALLETFRAME, def_font+def_background+def_color);
}

void defaultStyle()
{
    QString def_font= "font:12pt;";
    QString def_background = "background-color:black;";
    QString def_color = "color:rgb(0,255,0);";
    QString def_color_light = "color:lightgreen;";
    QString def_gradient_moderate = "background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(127, 127, 127, 255)); "; 
    QString def_gradient_extreme = "background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(255, 255, 255, 255)); "; 

        Style.replace(THEME_ABOUTDIALOG, def_background+def_color);
        
        Style.replace(THEME_ADDRESSBOOKPAGE, def_background);
        Style.replace(THEME_ADDRESSBOOKPAGE_TABLE, def_gradient_extreme);
        Style.replace(THEME_ADDRESSBOOKPAGE_BUTTON_A, def_gradient_extreme);
        Style.replace(THEME_ADDRESSBOOKPAGE_BUTTON_B, "background-color:black;");

        Style.replace(THEME_ALT_TEXT, def_color_light);
        
        Style.replace(THEME_ASKPASSPHRASEDIALOG, def_background);
        Style.replace(THEME_ASKPASSPHRASEDIALOG_BUTTON, def_gradient_moderate);
        
        Style.replace(THEME_BITCOINGUI,"background-color:white");
        
        Style.replace(THEME_EDITADDRESSDIALOG,"");
        
        Style.replace(THEME_MININGDIALOG,"");
        
        Style.replace(THEME_OPTIONSDIALOG, def_background);
        Style.replace(THEME_OPTIONSDIALOG_TAB, def_gradient_extreme+def_color);
        Style.replace(THEME_OPTIONSDIALOG_BUTTON, def_gradient_extreme+def_color);
        
        Style.replace(THEME_OVERVIEWPAGE, "background-color:default"+def_color);
        
        Style.replace(THEME_RECEIVECOINSPAGE, def_background);
        Style.replace(THEME_RECEIVECOINSPAGE_TABLE, def_gradient_extreme);
        Style.replace(THEME_RECEIVECOINSPAGE_BUTTON_A, def_gradient_extreme);
        Style.replace(THEME_RECEIVECOINSPAGE_BUTTON_B, "background-color:qlineargradient(spread:pad, x1:0, y1:0, x2:1, y2:0, stop:0 rgba(0, 0, 0, 255), stop:1 rgba(0, 240, 000, 255));");
        
        Style.replace(THEME_RPCCONSOLE, def_background);
        Style.replace(THEME_RPCCONSOLE_TAB, "color:blue;");
        Style.replace(THEME_RPCCONSOLE_CONSOLE, def_background+def_color);
        Style.replace(THEME_RPCCONSOLE_SUBTITLE, "color:lightblue;");
        Style.replace(THEME_RPCCONSOLE_BUTTON,def_gradient_moderate+def_color);
        Style.replace(THEME_RPCCONSOLE_LABEL, def_color);

        Style.replace(THEME_SENDCOINSPAGE, def_background);
        Style.replace(THEME_SENDCOINSPAGE_BUTTON, def_gradient_moderate);

        Style.replace(THEME_SENDCOINSENTRY, def_background);
        Style.replace(THEME_SENDCOINSENTRY_BUTTON, def_gradient_moderate);
        Style.replace(THEME_SENDCOINSENTRY_LINE, def_gradient_moderate);

        Style.replace(THEME_SIGNVERIFYDIALOG, def_gradient_extreme);
        Style.replace(THEME_SIGNVERIFYDIALOG_TAB, def_background+def_color);
        Style.replace(THEME_SIGNVERIFYDIALOG_BUTTON, def_gradient_moderate);

        Style.replace(THEME_TRANSACTIONDESCDIALOG, def_gradient_moderate+"color:yellow");

        Style.replace(THEME_TRANSACTIONVIEW, "background-color:rgb(30,30,30);alternate-background-color: rgb(38, 25, 45);"+def_color);
        Style.replace(THEME_TRANSACTIONVIEW_HEADER, def_color);
        
        Style.replace(THEME_WALLETFRAME, def_font+def_background+def_color);
}

OptionsModel* model;

void setOptionsModel(OptionsModel *optionsmodel) 
{ 
    model = optionsmodel;
}

void applyTheme(QWidget* target, int type)
{
        target->setStyleSheet(Style.at(type));
}

void setTheme()
{
    if (model->getTogglePalette())
    {  
        alternateStyle();
    }
    else
    {
        defaultStyle();
    }
}
