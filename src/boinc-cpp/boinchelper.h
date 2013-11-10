#ifndef BOINCHELPER_H
#define BOINCHELPER_H

#if defined(WIN32)
    #include <QAxObject>
#endif

#include <QString>
#include <QSharedPointer>
#include <QMap>

class BoincHelper
{
public:

    static const float BOINC_MEMORY_FOOTPRINT = 5000000.0f;

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

#elif defined(LINUX)

    struct ProcessData
    {
        ProcessData();

        QString cmd;
        int ppid;
        unsigned long long prev_cputime;
    };

    int m_prevUpTime;
    QMap<int, ProcessData> m_processData;
    int m_boinc_tid;
    int m_threadsNum;
    float m_averageCPUUtilization;
    float m_totalMemoryUtilization;
    long m_pageSize;

#endif

private:
    BoincHelper();
    BoincHelper(BoincHelper &);
    BoincHelper &operator=(const BoincHelper &);
};

#endif // BOINCHELPER_H
