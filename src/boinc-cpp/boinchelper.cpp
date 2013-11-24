#include "boinchelper.h"

#include <QApplication>
#include <QProcess>
#include <QCryptographicHash>
#include <QFile>

#if defined(LINUX)
    #include <proc/readproc.h>
    #include <proc/sysinfo.h>
    #include <unistd.h>
#endif

BoincHelper::BoincHelper() :
    m_registered(false)
{
#if defined(LINUX)

    meminfo();

    m_pageSize = sysconf(_SC_PAGE_SIZE);

    m_prevUpTime = 0;
    m_boinc_tid = 0;
    m_threadsNum = 0;
    m_averageCPUUtilization = 0.0f;
    m_totalMemoryUtilization = 0.0f;

#endif
}

BoincHelper::~BoincHelper()
{
    unregisterBoinc();
}

BoincHelper &BoincHelper::instance()
{
    static BoincHelper inst;

    return inst;
}

void BoincHelper::showEmailModule()
{
#if defined(WIN32)

    if (!m_axObject)
    {
        return;
    }

    m_axObject->dynamicCall("ShowEmailModule()");

#else

#warning Not implemented

#endif
}

int BoincHelper::utilization()
{
#if defined(WIN32)

    if (!m_axObject)
    {
        return 0;
    }

    return m_axObject->dynamicCall("BoincUtilization()").toInt();

#elif defined(LINUX)

    /*
     Instantaneous CPU percentage is commonly desired, but is not tracked
     by the kernel and is therefore not available anywhere procps can read.
     Tracking a percentage has to be implemented in the application by taking
     a snapshot, waiting a little while, and taking another snapshot to
     learn the utime+stime spent during the interval. This is the reason why
     top shows all CPU percentages as 0.0% when it starts, and corrects them
     on the next interval. procps provides a convenient place to store the
     CPU percentage, but does not implement it in the library.
    */

    PROCTAB *pt = openproc(PROC_FILLMEM | PROC_FILLSTAT | PROC_FILLSTATUS);

    if (!pt)
    {
        return 0;
    }

    proc_t pi;

    memset(&pi, 0, sizeof(pi));

    int up_time = uptime(0, 0);
    int elapsed = up_time - m_prevUpTime;

    m_threadsNum = 0;
    m_averageCPUUtilization = 0;
    m_totalMemoryUtilization = 0;

    while (readproc(pt, &pi))
    {
        QString cmd = pi.cmd;

        bool worker = false;

        if (cmd.compare("boinc") == 0)
        {
            if (m_boinc_tid != pi.tid)
            {
                m_processData.clear();
                m_boinc_tid = pi.tid;
                m_threadsNum = 0;
            }
        }
        else if (m_boinc_tid && pi.ppid == m_boinc_tid)
        {
            ++m_threadsNum;
            worker = true;
        }
        else
        {
            continue;
        }

        ProcessData &pd = m_processData[pi.tid];

        pd.cmd = cmd;
        pd.ppid = pi.ppid;

        unsigned long long cputime = pi.utime + pi.stime;

        if (worker && elapsed)
        {
            float utilization = float(cputime - pd.prev_cputime) / elapsed;
            long memory = pi.resident * m_pageSize; // in bytes

            m_averageCPUUtilization += utilization;
            m_totalMemoryUtilization += memory;
        }

        pd.prev_cputime = cputime;
    }

    if (m_threadsNum)
    {
        m_averageCPUUtilization /= m_threadsNum;
    }

    m_prevUpTime = up_time;

    closeproc(pt);

    // usage logic
    float usage_percent = qMin(75.0f, m_averageCPUUtilization);
    float usage_memory = qMin(25.0f, m_totalMemoryUtilization);

    if (usage_percent <= 1.0f)
    {
        usage_memory = 0;
    }

    usage_percent += usage_memory;

    return usage_percent;

#else

#warning Not implemented

#endif

    return 0;
}

int BoincHelper::threads()
{
#if defined(WIN32)

    if (!m_axObject)
    {
        return 0;
    }

    return m_axObject->dynamicCall("BoincThreads()").toInt();

#elif defined(LINUX)

    return m_threadsNum;

#else

#warning Not implemented

#endif

    return 0;
}

QString BoincHelper::md5()
{
#if defined(WIN32)

    if (!m_axObject)
    {
        return "";
    }

    return m_axObject->dynamicCall("BoincMD5()").toString();

#elif defined(LINUX)

    QCryptographicHash hash(QCryptographicHash::Md5);

    QFile f("/usr/bin/boinc");

    if (!f.open(QIODevice::ReadOnly))
    {
        return "";
    }

    hash.addData(f.readAll());

    return hash.result().toHex();

#else

#warning Not implemented

#endif

    return "";
}

int BoincHelper::version()
{
#if defined(WIN32)

    if (!m_axObject)
    {
        return 0;
    }

    return m_axObject->dynamicCall("Version()").toInt();

#elif defined(LINUX)

    return 1;

#else

#warning Not implemented

#endif

    return 0;
}

QString BoincHelper::authenticityString()
{
#if defined(WIN32)

    if (!m_axObject)
    {
        return "";
    }

    return m_axObject->dynamicCall("BoincAuthenticityString()").toString();

#elif defined(LINUX)
    /*
      '1.  Retrieve the Boinc MD5 Hash
      '2.  Verify the boinc.exe contains the Berkeley source libraries
      '3.  Verify the exe is an official release
      '4.  Verify the size of the exe is above the threshhold
    */

    // -1 = Invalid Executable
    // -2 = Failed Authenticity Check
    // -3 = Failed library check
    // -4 = Failed to Find boinc tray
    // -10= Error during enumeration
    //  1 = Success

    QFile f("/usr/bin/boinc");

    if (!f.open(QIODevice::ReadOnly | QIODevice::Text))
    {
        return "-10";
    }

    if (f.size() < 758528 / 2)
    {
        return "-1";
    }

    const QByteArray &data = f.readAll();

    if (!data.contains("http://boinc.berkeley.edu"))
    {
        return "-2";
    }

    // there is no "LIBEAY32.dll" and boinctray

    return "1";

#else

#warning Not implemented

#endif

    return "";
}

bool BoincHelper::registered() const
{
    return m_registered;
}

void BoincHelper::registerBoinc()
{
    if (m_registered)
    {
        return;
    }

#if defined(WIN32)

    const QString appDirPath = QApplication::applicationDirPath() + "\\";

    if (!QProcess::execute(appDirPath + "regtlibv12.exe", QStringList() << "boinc.tlb"))
    {
        return;
    }

    if (!QProcess::execute(appDirPath + "regasm.exe", QStringList() << "boinc.dll"))
    {
        return;
    }

    m_axObject = QSharedPointer<QAxObject>(new QAxObject("Boinc.Utilization"));

    if (!m_axObject)
    {
        return;
    }

#endif

    m_registered = true;
}

void BoincHelper::unregisterBoinc()
{
    if (!m_registered)
    {
        return;
    }

#if defined(WIN32)

    m_axObject.clear();

    const QString appDirPath = QApplication::applicationDirPath() + "\\";

    QProcess::execute(appDirPath + "regasm.exe", QStringList() << "boinc.dll" << "-u");
    QProcess::execute(appDirPath + "regtlibv12.exe", QStringList() << "boinc.tlb" << "-u");

#endif

    m_registered = false;
}
