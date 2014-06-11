/****************************************************************************
** Meta object code from reading C++ file 'miningdialog.h'
**
** Created: Wed Jun 11 09:18:30 2014
**      by: The Qt Meta Object Compiler version 63 (Qt 4.8.4)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../src/qt/miningdialog.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'miningdialog.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.4. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_MiningDialog[] = {

 // content:
       6,       // revision
       0,       // classname
       0,    0, // classinfo
       5,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: signature, parameters, type, tag, flags
      14,   13,   13,   13, 0x08,
      31,   13,   13,   13, 0x08,
      49,   13,   13,   13, 0x08,
      63,   13,   13,   13, 0x08,
     111,   83,   13,   13, 0x08,

       0        // eod
};

static const char qt_meta_stringdata_MiningDialog[] = {
    "MiningDialog\0\0refreshClicked()\0"
    "registerClicked()\0exitClicked()\0"
    "unregisterClicked()\0program,sFilename,sArgument\0"
    "regsvr(QString,QString,QString)\0"
};

void MiningDialog::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        MiningDialog *_t = static_cast<MiningDialog *>(_o);
        switch (_id) {
        case 0: _t->refreshClicked(); break;
        case 1: _t->registerClicked(); break;
        case 2: _t->exitClicked(); break;
        case 3: _t->unregisterClicked(); break;
        case 4: _t->regsvr((*reinterpret_cast< QString(*)>(_a[1])),(*reinterpret_cast< QString(*)>(_a[2])),(*reinterpret_cast< QString(*)>(_a[3]))); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData MiningDialog::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject MiningDialog::staticMetaObject = {
    { &QDialog::staticMetaObject, qt_meta_stringdata_MiningDialog,
      qt_meta_data_MiningDialog, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &MiningDialog::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *MiningDialog::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *MiningDialog::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_MiningDialog))
        return static_cast<void*>(const_cast< MiningDialog*>(this));
    return QDialog::qt_metacast(_clname);
}

int MiningDialog::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QDialog::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 5)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 5;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
