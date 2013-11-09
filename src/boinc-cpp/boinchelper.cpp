#include "boinchelper.h"

#include <QApplication>
#include <QProcess>

BoincHelper::BoincHelper() :
    m_registered(false)
{
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
#else
#warning Not implemented

    return 0;
#endif
}

int BoincHelper::threads()
{
#if defined(WIN32)
    if (!m_axObject)
    {
        return 0;
    }

    return m_axObject->dynamicCall("BoincThreads()").toInt();
#else
#warning Not implemented

    return 0;
#endif
}

QString BoincHelper::md5()
{
#if defined(WIN32)
    if (!m_axObject)
    {
        return "";
    }

    return m_axObject->dynamicCall("BoincMD5()").toString();
#else
#warning Not implemented

    return "";
#endif
}

int BoincHelper::version()
{
#if defined(WIN32)
    if (!m_axObject)
    {
        return 0;
    }

    return m_axObject->dynamicCall("Version()").toInt();
#else
#warning Not implemented

    return 0;
#endif
}

QString BoincHelper::authenticityString()
{
#if defined(WIN32)
    if (!m_axObject)
    {
        return "";
    }

    return m_axObject->dynamicCall("BoincAuthenticityString()").toString();
#else
#warning Not implemented

    return "";
#endif
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

    m_axObject.reset(new QAxObject("Boinc.Utilization"));

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
    m_axObject.reset();

    const QString appDirPath = QApplication::applicationDirPath() + "\\";

    QProcess::execute(appDirPath + "regasm.exe", QStringList() << "boinc.dll" << "-u");
    QProcess::execute(appDirPath + "regtlibv12.exe", QStringList() << "boinc.tlb" << "-u");
#endif

    m_registered = false;
}
