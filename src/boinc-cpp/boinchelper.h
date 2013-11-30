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

    static const float BoincMemoryFootprint = 5000000.0f;

    virtual ~BoincHelper();

    static BoincHelper &instance();

    int utilization();
    int threads();
    QString md5();
    int version();
    QString authenticityString();
    bool registered() const;
    void registerBoinc();
    void unregisterBoinc();
    void showEmailModule();
    void showProjects();
    void showMiningConsole();
    double CPUPoW(const QString &pow);
    QString minedHash();
    QString sourceBlock();
    QString deltaOverTime();
    void setLastBlockHash(const QString &hash);
    void setPublicWalletAddress(const QString &address);

protected:

    bool m_registered;

#if defined(WIN32)
    QSharedPointer<QAxObject> m_axObject;
#elif defined(LINUX)
    struct ProcessData
    {
        ProcessData() :
            ppid(0),
            prev_cputime(0)
        {
        }

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
