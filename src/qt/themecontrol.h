#ifndef THEMESELECT_H
#define THEMESELECT_H

#include <QWidget>

class OptionsModel;

enum themes
{
	THEME_BITCOINGUI,
	THEME_WALLETFRAME,
	THEME_RPCCONSOLE,
	THEME_OVERVIEWPAGE,
	THEME_ADDRESSBOOKPAGE,
	THEME_ABOUTDIALOG,
	THEME_OPTIONSDIALOG
};

void setTheme(QWidget* target, int type);

void setOptionsModel(OptionsModel *optionsmodel);

#endif
