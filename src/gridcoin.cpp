
#include "alert.h"
#include "checkpoints.h"
#include "db.h"
#include "txdb.h"
#include "net.h"
#include "init.h"
#include "ui_interface.h"
#include "checkqueue.h"
#include <boost/algorithm/string/replace.hpp>
#include <boost/filesystem.hpp>
#include <boost/filesystem/fstream.hpp>

#include <list>
#include <QObject>
#include <QThread>

using namespace std;


class MyThread : public QThread
{
    Q_OBJECT

    void run() {
        QString result;

        while (1==1)
        {
            printf("Processing");
            MilliSleep(2000);
        }
    }

signals:
    void resultReady(const QString &s);
};
