/****************************************************************************
** Meta object code from reading C++ file 'bitcoingui.h'
**
** Created: Wed Jan 1 17:56:03 2014
**      by: The Qt Meta Object Compiler version 63 (Qt 4.8.4)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../src/qt/bitcoingui.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'bitcoingui.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.4. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_BitcoinGUI[] = {

 // content:
       6,       // revision
       0,       // classname
       0,    0, // classinfo
      33,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: signature, parameters, type, tag, flags
      18,   12,   11,   11, 0x0a,
      60,   41,   11,   11, 0x0a,
      89,   82,   11,   11, 0x0a,
     138,  114,   11,   11, 0x0a,
     194,  174,   11,   11, 0x2a,
     244,  224,   11,   11, 0x0a,
     275,  265,   11,   11, 0x0a,
     310,  303,   11,   11, 0x0a,
     359,  329,   11,   11, 0x0a,
     415,   11,   11,   11, 0x08,
     434,   11,   11,   11, 0x08,
     452,   11,   11,   11, 0x08,
     474,   11,   11,   11, 0x08,
     502,  497,   11,   11, 0x08,
     529,   11,   11,   11, 0x28,
     549,  497,   11,   11, 0x08,
     577,   11,   11,   11, 0x28,
     598,  497,   11,   11, 0x08,
     628,   11,   11,   11, 0x28,
     651,   11,   11,   11, 0x08,
     668,   11,   11,   11, 0x08,
     683,   11,   11,   11, 0x08,
     699,   11,   11,   11, 0x08,
     714,   11,   11,   11, 0x08,
     727,   11,   11,   11, 0x08,
     748,   11,   11,   11, 0x08,
     765,   11,   11,   11, 0x08,
     790,  783,   11,   11, 0x08,
     857,  843,   11,   11, 0x08,
     885,   11,   11,   11, 0x28,
     909,   11,   11,   11, 0x08,
     924,   11,   11,   11, 0x08,
     941,   11,   11,   11, 0x08,

       0        // eod
};

static const char qt_meta_stringdata_BitcoinGUI[] = {
    "BitcoinGUI\0\0count\0setNumConnections(int)\0"
    "count,nTotalBlocks\0setNumBlocks(int,int)\0"
    "status\0setEncryptionStatus(int)\0"
    "title,message,style,ret\0"
    "message(QString,QString,uint,bool*)\0"
    "title,message,style\0message(QString,QString,uint)\0"
    "nFeeRequired,payFee\0askFee(qint64,bool*)\0"
    "sLog,sOut\0GetResult(QString,QString*)\0"
    "strURI\0handleURI(QString)\0"
    "date,unit,amount,type,address\0"
    "incomingTransaction(QString,int,qint64,QString,QString)\0"
    "gotoOverviewPage()\0gotoHistoryPage()\0"
    "gotoAddressBookPage()\0gotoReceiveCoinsPage()\0"
    "addr\0gotoSendCoinsPage(QString)\0"
    "gotoSendCoinsPage()\0gotoSignMessageTab(QString)\0"
    "gotoSignMessageTab()\0gotoVerifyMessageTab(QString)\0"
    "gotoVerifyMessageTab()\0optionsClicked()\0"
    "aboutClicked()\0miningClicked()\0"
    "emailClicked()\0sqlClicked()\0"
    "leaderboardClicked()\0rebuildClicked()\0"
    "projectsClicked()\0reason\0"
    "trayIconActivated(QSystemTrayIcon::ActivationReason)\0"
    "fToggleHidden\0showNormalIfMinimized(bool)\0"
    "showNormalIfMinimized()\0toggleHidden()\0"
    "detectShutdown()\0timerfire()\0"
};

void BitcoinGUI::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        BitcoinGUI *_t = static_cast<BitcoinGUI *>(_o);
        switch (_id) {
        case 0: _t->setNumConnections((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 1: _t->setNumBlocks((*reinterpret_cast< int(*)>(_a[1])),(*reinterpret_cast< int(*)>(_a[2]))); break;
        case 2: _t->setEncryptionStatus((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 3: _t->message((*reinterpret_cast< const QString(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2])),(*reinterpret_cast< uint(*)>(_a[3])),(*reinterpret_cast< bool*(*)>(_a[4]))); break;
        case 4: _t->message((*reinterpret_cast< const QString(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2])),(*reinterpret_cast< uint(*)>(_a[3]))); break;
        case 5: _t->askFee((*reinterpret_cast< qint64(*)>(_a[1])),(*reinterpret_cast< bool*(*)>(_a[2]))); break;
        case 6: _t->GetResult((*reinterpret_cast< QString(*)>(_a[1])),(*reinterpret_cast< QString*(*)>(_a[2]))); break;
        case 7: _t->handleURI((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 8: _t->incomingTransaction((*reinterpret_cast< const QString(*)>(_a[1])),(*reinterpret_cast< int(*)>(_a[2])),(*reinterpret_cast< qint64(*)>(_a[3])),(*reinterpret_cast< const QString(*)>(_a[4])),(*reinterpret_cast< const QString(*)>(_a[5]))); break;
        case 9: _t->gotoOverviewPage(); break;
        case 10: _t->gotoHistoryPage(); break;
        case 11: _t->gotoAddressBookPage(); break;
        case 12: _t->gotoReceiveCoinsPage(); break;
        case 13: _t->gotoSendCoinsPage((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 14: _t->gotoSendCoinsPage(); break;
        case 15: _t->gotoSignMessageTab((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 16: _t->gotoSignMessageTab(); break;
        case 17: _t->gotoVerifyMessageTab((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 18: _t->gotoVerifyMessageTab(); break;
        case 19: _t->optionsClicked(); break;
        case 20: _t->aboutClicked(); break;
        case 21: _t->miningClicked(); break;
        case 22: _t->emailClicked(); break;
        case 23: _t->sqlClicked(); break;
        case 24: _t->leaderboardClicked(); break;
        case 25: _t->rebuildClicked(); break;
        case 26: _t->projectsClicked(); break;
        case 27: _t->trayIconActivated((*reinterpret_cast< QSystemTrayIcon::ActivationReason(*)>(_a[1]))); break;
        case 28: _t->showNormalIfMinimized((*reinterpret_cast< bool(*)>(_a[1]))); break;
        case 29: _t->showNormalIfMinimized(); break;
        case 30: _t->toggleHidden(); break;
        case 31: _t->detectShutdown(); break;
        case 32: _t->timerfire(); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData BitcoinGUI::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject BitcoinGUI::staticMetaObject = {
    { &QMainWindow::staticMetaObject, qt_meta_stringdata_BitcoinGUI,
      qt_meta_data_BitcoinGUI, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &BitcoinGUI::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *BitcoinGUI::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *BitcoinGUI::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_BitcoinGUI))
        return static_cast<void*>(const_cast< BitcoinGUI*>(this));
    return QMainWindow::qt_metacast(_clname);
}

int BitcoinGUI::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QMainWindow::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 33)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 33;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
