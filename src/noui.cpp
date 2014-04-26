// Copyright (c) 2010 Satoshi Nakamoto
// Copyright (c) 2009-2012 The Bitcoin developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

#include "ui_interface.h"
#include "init.h"
#include "bitcoinrpc.h"

#include <string>

static bool noui_ThreadSafeMessageBox(const std::string& message, const std::string& caption, unsigned int style)
{
    std::string strCaption;
    // Check for usage of predefined caption
    switch (style) {
    case CClientUIInterface::MSG_ERROR:
        strCaption += _("Error");
        break;
    case CClientUIInterface::MSG_WARNING:
        strCaption += _("Warning");
        break;
    case CClientUIInterface::MSG_INFORMATION:
        strCaption += _("Information");
        break;
    default:
        strCaption += caption; // Use supplied caption (can be empty)
    }

    printf("%s: %s\n", strCaption.c_str(), message.c_str());
    fprintf(stderr, "%s: %s\n", strCaption.c_str(), message.c_str());
    return false;
}

static bool noui_ThreadSafeAskFee(int64 /*nFeeRequired*/)
{
    return true;
}



static int noui_ThreadSafeWin32Call(const std::string& h1,const std::string& h2,const std::string& h3,const std::string& h4,const std::string& h5) 
{
	return -101;
}

static void noui_InitMessage(const std::string &message)
{
    printf("init message: %s\n", message.c_str());
}

static int noui_ThreadSafeVersion()
{
	return 0;
}

void noui_connect()
{
    // Connect bitcoind signal handlers
    uiInterface.ThreadSafeMessageBox.connect(noui_ThreadSafeMessageBox);
    uiInterface.ThreadSafeAskFee.connect(noui_ThreadSafeAskFee);
    uiInterface.InitMessage.connect(noui_InitMessage);
    uiInterface.ThreadSafeVersion.connect(noui_ThreadSafeVersion);	
	//	uiInterface.ThreadSafeWin32Call.connect(noui_ThreadSafeWin32Call);
}



