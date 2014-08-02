/****************************************************************************
** Meta object code from reading C++ file 'optionsdialog.h'
**
** Created: Sat Jul 19 14:53:58 2014
**      by: The Qt Meta Object Compiler version 63 (Qt 4.8.4)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../src/qt/optionsdialog.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'optionsdialog.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.4. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_OptionsDialog[] = {

 // content:
       6,       // revision
       0,       // classname
       0,    0, // classinfo
      15,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       2,       // signalCount

 // signals: signature, parameters, type, tag, flags
      29,   15,   14,   14, 0x05,
      68,   14,   14,   14, 0x05,

 // slots: signature, parameters, type, tag, flags
      85,   14,   14,   14, 0x08,
     105,   14,   14,   14, 0x08,
     126,   14,   14,   14, 0x08,
     146,   14,   14,   14, 0x08,
     174,  167,   14,   14, 0x08,
     199,   14,   14,   14, 0x08,
     224,   14,   14,   14, 0x08,
     246,   14,   14,   14, 0x08,
     272,   14,   14,   14, 0x08,
     297,   14,   14,   14, 0x08,
     324,   14,   14,   14, 0x08,
     350,   14,   14,   14, 0x08,
     384,  370,   14,   14, 0x08,

       0        // eod
};

static const char qt_meta_stringdata_OptionsDialog[] = {
    "OptionsDialog\0\0object,fValid\0"
    "proxyIpValid(QValidatedLineEdit*,bool)\0"
    "optionsApplied()\0enableApplyButton()\0"
    "disableApplyButton()\0enableSaveButtons()\0"
    "disableSaveButtons()\0fState\0"
    "setSaveButtonState(bool)\0"
    "on_resetButton_clicked()\0on_okButton_clicked()\0"
    "on_cancelButton_clicked()\0"
    "on_applyButton_clicked()\0"
    "showRestartWarning_Proxy()\0"
    "showRestartWarning_Lang()\0updateDisplayUnit()\0"
    "object,fState\0"
    "handleProxyIpValid(QValidatedLineEdit*,bool)\0"
};

void OptionsDialog::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        OptionsDialog *_t = static_cast<OptionsDialog *>(_o);
        switch (_id) {
        case 0: _t->proxyIpValid((*reinterpret_cast< QValidatedLineEdit*(*)>(_a[1])),(*reinterpret_cast< bool(*)>(_a[2]))); break;
        case 1: _t->optionsApplied(); break;
        case 2: _t->enableApplyButton(); break;
        case 3: _t->disableApplyButton(); break;
        case 4: _t->enableSaveButtons(); break;
        case 5: _t->disableSaveButtons(); break;
        case 6: _t->setSaveButtonState((*reinterpret_cast< bool(*)>(_a[1]))); break;
        case 7: _t->on_resetButton_clicked(); break;
        case 8: _t->on_okButton_clicked(); break;
        case 9: _t->on_cancelButton_clicked(); break;
        case 10: _t->on_applyButton_clicked(); break;
        case 11: _t->showRestartWarning_Proxy(); break;
        case 12: _t->showRestartWarning_Lang(); break;
        case 13: _t->updateDisplayUnit(); break;
        case 14: _t->handleProxyIpValid((*reinterpret_cast< QValidatedLineEdit*(*)>(_a[1])),(*reinterpret_cast< bool(*)>(_a[2]))); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData OptionsDialog::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject OptionsDialog::staticMetaObject = {
    { &QDialog::staticMetaObject, qt_meta_stringdata_OptionsDialog,
      qt_meta_data_OptionsDialog, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &OptionsDialog::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *OptionsDialog::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *OptionsDialog::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_OptionsDialog))
        return static_cast<void*>(const_cast< OptionsDialog*>(this));
    return QDialog::qt_metacast(_clname);
}

int OptionsDialog::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QDialog::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 15)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 15;
    }
    return _id;
}

// SIGNAL 0
void OptionsDialog::proxyIpValid(QValidatedLineEdit * _t1, bool _t2)
{
    void *_a[] = { 0, const_cast<void*>(reinterpret_cast<const void*>(&_t1)), const_cast<void*>(reinterpret_cast<const void*>(&_t2)) };
    QMetaObject::activate(this, &staticMetaObject, 0, _a);
}

// SIGNAL 1
void OptionsDialog::optionsApplied()
{
    QMetaObject::activate(this, &staticMetaObject, 1, 0);
}
QT_END_MOC_NAMESPACE
