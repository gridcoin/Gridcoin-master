/****************************************************************************
** Meta object code from reading C++ file 'walletview.h'
**
** Created by: The Qt Meta Object Compiler version 63 (Qt 4.8.5)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../src/qt/walletview.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'walletview.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.5. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_WalletView[] = {

 // content:
       6,       // revision
       0,       // classname
       0,    0, // classinfo
      16,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: signature, parameters, type, tag, flags
      12,   11,   11,   11, 0x0a,
      31,   11,   11,   11, 0x0a,
      49,   11,   11,   11, 0x0a,
      71,   11,   11,   11, 0x0a,
      99,   94,   11,   11, 0x0a,
     126,   11,   11,   11, 0x2a,
     146,   94,   11,   11, 0x0a,
     174,   11,   11,   11, 0x2a,
     195,   94,   11,   11, 0x0a,
     225,   11,   11,   11, 0x2a,
     262,  248,   11,   11, 0x0a,
     310,  303,   11,   11, 0x0a,
     330,   11,   11,   11, 0x0a,
     345,   11,   11,   11, 0x0a,
     364,   11,   11,   11, 0x0a,
     379,   11,   11,   11, 0x0a,

       0        // eod
};

static const char qt_meta_stringdata_WalletView[] = {
    "WalletView\0\0gotoOverviewPage()\0"
    "gotoHistoryPage()\0gotoAddressBookPage()\0"
    "gotoReceiveCoinsPage()\0addr\0"
    "gotoSendCoinsPage(QString)\0"
    "gotoSendCoinsPage()\0gotoSignMessageTab(QString)\0"
    "gotoSignMessageTab()\0gotoVerifyMessageTab(QString)\0"
    "gotoVerifyMessageTab()\0parent,start,\0"
    "incomingTransaction(QModelIndex,int,int)\0"
    "status\0encryptWallet(bool)\0backupWallet()\0"
    "changePassphrase()\0unlockWallet()\0"
    "setEncryptionStatus()\0"
};

void WalletView::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        WalletView *_t = static_cast<WalletView *>(_o);
        switch (_id) {
        case 0: _t->gotoOverviewPage(); break;
        case 1: _t->gotoHistoryPage(); break;
        case 2: _t->gotoAddressBookPage(); break;
        case 3: _t->gotoReceiveCoinsPage(); break;
        case 4: _t->gotoSendCoinsPage((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 5: _t->gotoSendCoinsPage(); break;
        case 6: _t->gotoSignMessageTab((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 7: _t->gotoSignMessageTab(); break;
        case 8: _t->gotoVerifyMessageTab((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 9: _t->gotoVerifyMessageTab(); break;
        case 10: _t->incomingTransaction((*reinterpret_cast< const QModelIndex(*)>(_a[1])),(*reinterpret_cast< int(*)>(_a[2])),(*reinterpret_cast< int(*)>(_a[3]))); break;
        case 11: _t->encryptWallet((*reinterpret_cast< bool(*)>(_a[1]))); break;
        case 12: _t->backupWallet(); break;
        case 13: _t->changePassphrase(); break;
        case 14: _t->unlockWallet(); break;
        case 15: _t->setEncryptionStatus(); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData WalletView::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject WalletView::staticMetaObject = {
    { &QStackedWidget::staticMetaObject, qt_meta_stringdata_WalletView,
      qt_meta_data_WalletView, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &WalletView::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *WalletView::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *WalletView::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_WalletView))
        return static_cast<void*>(const_cast< WalletView*>(this));
    return QStackedWidget::qt_metacast(_clname);
}

int WalletView::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QStackedWidget::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 16)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 16;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
