/****************************************************************************
** Meta object code from reading C++ file 'bitcoingui.h'
**
** Created: Sat Aug 2 07:51:08 2014
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
      37,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       1,       // signalCount

 // signals: signature, parameters, type, tag, flags
      12,   11,   11,   11, 0x05,

 // slots: signature, parameters, type, tag, flags
      30,   24,   11,   11, 0x0a,
      72,   53,   11,   11, 0x0a,
     101,   94,   11,   11, 0x0a,
     150,  126,   11,   11, 0x0a,
     206,  186,   11,   11, 0x2a,
     256,  236,   11,   11, 0x0a,
     299,  277,   11,   11, 0x0a,
     375,  365,   11,   11, 0x0a,
     410,  403,   11,   11, 0x0a,
     459,  429,   11,   11, 0x0a,
     515,   11,   11,   11, 0x0a,
     529,   11,   11,   11, 0x08,
     548,   11,   11,   11, 0x08,
     566,   11,   11,   11, 0x08,
     588,   11,   11,   11, 0x08,
     616,  611,   11,   11, 0x08,
     643,   11,   11,   11, 0x28,
     663,  611,   11,   11, 0x08,
     691,   11,   11,   11, 0x28,
     712,  611,   11,   11, 0x08,
     742,   11,   11,   11, 0x28,
     765,   11,   11,   11, 0x08,
     782,   11,   11,   11, 0x08,
     797,   11,   11,   11, 0x08,
     813,   11,   11,   11, 0x08,
     828,   11,   11,   11, 0x08,
     841,   11,   11,   11, 0x08,
     862,   11,   11,   11, 0x08,
     879,   11,   11,   11, 0x08,
     896,   11,   11,   11, 0x08,
     921,  914,   11,   11, 0x08,
     988,  974,   11,   11, 0x08,
    1016,   11,   11,   11, 0x28,
    1040,   11,   11,   11, 0x08,
    1055,   11,   11,   11, 0x08,
    1072,   11,   11,   11, 0x08,

       0        // eod
};

static const char qt_meta_stringdata_BitcoinGUI[] = {
    "BitcoinGUI\0\0pagesView()\0count\0"
    "setNumConnections(int)\0count,nTotalBlocks\0"
    "setNumBlocks(int,int)\0status\0"
    "setEncryptionStatus(int)\0"
    "title,message,style,ret\0"
    "message(QString,QString,uint,bool*)\0"
    "title,message,style\0message(QString,QString,uint)\0"
    "nFeeRequired,payFee\0askFee(qint64,bool*)\0"
    "h1,h2,h3,h4,h5,result\0"
    "threadsafewin32call(QString,QString,QString,QString,QString,int*)\0"
    "sLog,sOut\0GetResult(QString,QString*)\0"
    "strURI\0handleURI(QString)\0"
    "date,unit,amount,type,address\0"
    "incomingTransaction(QString,int,qint64,QString,QString)\0"
    "updateTheme()\0gotoOverviewPage()\0"
    "gotoHistoryPage()\0gotoAddressBookPage()\0"
    "gotoReceiveCoinsPage()\0addr\0"
    "gotoSendCoinsPage(QString)\0"
    "gotoSendCoinsPage()\0gotoSignMessageTab(QString)\0"
    "gotoSignMessageTab()\0gotoVerifyMessageTab(QString)\0"
    "gotoVerifyMessageTab()\0optionsClicked()\0"
    "aboutClicked()\0miningClicked()\0"
    "emailClicked()\0sqlClicked()\0"
    "leaderboardClicked()\0rebuildClicked()\0"
    "upgradeClicked()\0downloadClicked()\0"
    "reason\0trayIconActivated(QSystemTrayIcon::ActivationReason)\0"
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
        case 0: _t->pagesView(); break;
        case 1: _t->setNumConnections((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 2: _t->setNumBlocks((*reinterpret_cast< int(*)>(_a[1])),(*reinterpret_cast< int(*)>(_a[2]))); break;
        case 3: _t->setEncryptionStatus((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 4: _t->message((*reinterpret_cast< const QString(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2])),(*reinterpret_cast< uint(*)>(_a[3])),(*reinterpret_cast< bool*(*)>(_a[4]))); break;
        case 5: _t->message((*reinterpret_cast< const QString(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2])),(*reinterpret_cast< uint(*)>(_a[3]))); break;
        case 6: _t->askFee((*reinterpret_cast< qint64(*)>(_a[1])),(*reinterpret_cast< bool*(*)>(_a[2]))); break;
        case 7: _t->threadsafewin32call((*reinterpret_cast< const QString(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2])),(*reinterpret_cast< const QString(*)>(_a[3])),(*reinterpret_cast< const QString(*)>(_a[4])),(*reinterpret_cast< const QString(*)>(_a[5])),(*reinterpret_cast< int*(*)>(_a[6]))); break;
        case 8: _t->GetResult((*reinterpret_cast< QString(*)>(_a[1])),(*reinterpret_cast< QString*(*)>(_a[2]))); break;
        case 9: _t->handleURI((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 10: _t->incomingTransaction((*reinterpret_cast< const QString(*)>(_a[1])),(*reinterpret_cast< int(*)>(_a[2])),(*reinterpret_cast< qint64(*)>(_a[3])),(*reinterpret_cast< const QString(*)>(_a[4])),(*reinterpret_cast< const QString(*)>(_a[5]))); break;
        case 11: _t->updateTheme(); break;
        case 12: _t->gotoOverviewPage(); break;
        case 13: _t->gotoHistoryPage(); break;
        case 14: _t->gotoAddressBookPage(); break;
        case 15: _t->gotoReceiveCoinsPage(); break;
        case 16: _t->gotoSendCoinsPage((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 17: _t->gotoSendCoinsPage(); break;
        case 18: _t->gotoSignMessageTab((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 19: _t->gotoSignMessageTab(); break;
        case 20: _t->gotoVerifyMessageTab((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 21: _t->gotoVerifyMessageTab(); break;
        case 22: _t->optionsClicked(); break;
        case 23: _t->aboutClicked(); break;
        case 24: _t->miningClicked(); break;
        case 25: _t->emailClicked(); break;
        case 26: _t->sqlClicked(); break;
        case 27: _t->leaderboardClicked(); break;
        case 28: _t->rebuildClicked(); break;
        case 29: _t->upgradeClicked(); break;
        case 30: _t->downloadClicked(); break;
        case 31: _t->trayIconActivated((*reinterpret_cast< QSystemTrayIcon::ActivationReason(*)>(_a[1]))); break;
        case 32: _t->showNormalIfMinimized((*reinterpret_cast< bool(*)>(_a[1]))); break;
        case 33: _t->showNormalIfMinimized(); break;
        case 34: _t->toggleHidden(); break;
        case 35: _t->detectShutdown(); break;
        case 36: _t->timerfire(); break;
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
        if (_id < 37)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 37;
    }
    return _id;
}

// SIGNAL 0
void BitcoinGUI::pagesView()
{
    QMetaObject::activate(this, &staticMetaObject, 0, 0);
}
QT_END_MOC_NAMESPACE
