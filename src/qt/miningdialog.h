#ifndef MININGDIALOG_H
#define MININGDIALOG_H

#include <QDialog>

namespace Ui {
    class MiningDialog;
}
class ClientModel;

/** "Mining" dialog box */
class MiningDialog : public QDialog
{
    Q_OBJECT

public:
    explicit MiningDialog(QWidget *parent = 0);
    ~MiningDialog();

    void setModel(ClientModel *model);

private:
    Ui::MiningDialog *ui;

private slots:
    void refreshClicked();
	void registerClicked();
	void exitClicked();
	void unregisterClicked();
};

#endif // MININGDIALOG_H
