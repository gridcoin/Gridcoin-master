#ifndef BOINCHELPER_H
#define BOINCHELPER_H

#if defined(WIN32)
#include <QAxObject>
#endif

#include <QString>
#include <QSharedPointer>

class BoincHelper
{
public:
    virtual ~BoincHelper();

    static BoincHelper &instance();

    void showEmailModule();
    int utilization();
    int threads();
    QString md5();
    int version();
    QString authenticityString();
    bool registered() const;
    void registerBoinc();
    void unregisterBoinc();

protected:
    bool m_registered;

#if defined(WIN32)
    QSharedPointer<QAxObject> m_axObject;
#endif

private:
    BoincHelper();
    BoincHelper(BoincHelper &);
    BoincHelper &operator=(const BoincHelper &);
};

#endif // BOINCHELPER_H
