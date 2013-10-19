<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGridCoin
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tabOverview = New System.Windows.Forms.TabPage()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lvRecent = New System.Windows.Forms.ListView()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lblBalance = New System.Windows.Forms.Label()
        Me.lblUnconfirmed = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblNumberOfTransactions = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tabSendCoins = New System.Windows.Forms.TabPage()
        Me.tabReceiveCoins = New System.Windows.Forms.TabPage()
        Me.tabTransactions = New System.Windows.Forms.TabPage()
        Me.tabAddressBook = New System.Windows.Forms.TabPage()
        Me.tabMining = New System.Windows.Forms.TabPage()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackupWalletToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EncryptWalletToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DebugWindowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblCPU = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.tabOverview.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.tabOverview)
        Me.TabControl1.Controls.Add(Me.tabSendCoins)
        Me.TabControl1.Controls.Add(Me.tabReceiveCoins)
        Me.TabControl1.Controls.Add(Me.tabTransactions)
        Me.TabControl1.Controls.Add(Me.tabAddressBook)
        Me.TabControl1.Controls.Add(Me.tabMining)
        Me.TabControl1.Location = New System.Drawing.Point(24, 31)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(887, 379)
        Me.TabControl1.TabIndex = 0
        '
        'tabOverview
        '
        Me.tabOverview.Controls.Add(Me.GroupBox2)
        Me.tabOverview.Controls.Add(Me.GroupBox1)
        Me.tabOverview.Location = New System.Drawing.Point(4, 22)
        Me.tabOverview.Name = "tabOverview"
        Me.tabOverview.Padding = New System.Windows.Forms.Padding(3)
        Me.tabOverview.Size = New System.Drawing.Size(879, 353)
        Me.tabOverview.TabIndex = 0
        Me.tabOverview.Text = "Overview"
        Me.tabOverview.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lvRecent)
        Me.GroupBox2.Location = New System.Drawing.Point(408, 30)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(438, 265)
        Me.GroupBox2.TabIndex = 6
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Recent Transactions"
        '
        'lvRecent
        '
        Me.lvRecent.Location = New System.Drawing.Point(15, 31)
        Me.lvRecent.Name = "lvRecent"
        Me.lvRecent.Size = New System.Drawing.Size(408, 211)
        Me.lvRecent.TabIndex = 0
        Me.lvRecent.UseCompatibleStateImageBehavior = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lblBalance)
        Me.GroupBox1.Controls.Add(Me.lblUnconfirmed)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.lblNumberOfTransactions)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Location = New System.Drawing.Point(55, 28)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(328, 268)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Wallet"
        '
        'lblBalance
        '
        Me.lblBalance.AutoSize = True
        Me.lblBalance.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBalance.Location = New System.Drawing.Point(176, 39)
        Me.lblBalance.Name = "lblBalance"
        Me.lblBalance.Size = New System.Drawing.Size(59, 13)
        Me.lblBalance.TabIndex = 6
        Me.lblBalance.Text = "0.00 LTC"
        '
        'lblUnconfirmed
        '
        Me.lblUnconfirmed.AutoSize = True
        Me.lblUnconfirmed.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnconfirmed.Location = New System.Drawing.Point(176, 85)
        Me.lblUnconfirmed.Name = "lblUnconfirmed"
        Me.lblUnconfirmed.Size = New System.Drawing.Size(59, 13)
        Me.lblUnconfirmed.TabIndex = 5
        Me.lblUnconfirmed.Text = "0.00 LTC"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(36, 85)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(70, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Unconfirmed:"
        '
        'lblNumberOfTransactions
        '
        Me.lblNumberOfTransactions.AutoSize = True
        Me.lblNumberOfTransactions.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNumberOfTransactions.Location = New System.Drawing.Point(176, 128)
        Me.lblNumberOfTransactions.Name = "lblNumberOfTransactions"
        Me.lblNumberOfTransactions.Size = New System.Drawing.Size(14, 13)
        Me.lblNumberOfTransactions.TabIndex = 4
        Me.lblNumberOfTransactions.Text = "0"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(36, 39)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(49, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Balance:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(37, 128)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(123, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Number of Transactions:"
        '
        'tabSendCoins
        '
        Me.tabSendCoins.Location = New System.Drawing.Point(4, 22)
        Me.tabSendCoins.Name = "tabSendCoins"
        Me.tabSendCoins.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSendCoins.Size = New System.Drawing.Size(879, 353)
        Me.tabSendCoins.TabIndex = 1
        Me.tabSendCoins.Text = "Send Coins"
        Me.tabSendCoins.UseVisualStyleBackColor = True
        '
        'tabReceiveCoins
        '
        Me.tabReceiveCoins.Location = New System.Drawing.Point(4, 22)
        Me.tabReceiveCoins.Name = "tabReceiveCoins"
        Me.tabReceiveCoins.Size = New System.Drawing.Size(879, 353)
        Me.tabReceiveCoins.TabIndex = 2
        Me.tabReceiveCoins.Text = "Receive Coins"
        Me.tabReceiveCoins.UseVisualStyleBackColor = True
        '
        'tabTransactions
        '
        Me.tabTransactions.Location = New System.Drawing.Point(4, 22)
        Me.tabTransactions.Name = "tabTransactions"
        Me.tabTransactions.Size = New System.Drawing.Size(879, 353)
        Me.tabTransactions.TabIndex = 3
        Me.tabTransactions.Text = "Transactions"
        Me.tabTransactions.UseVisualStyleBackColor = True
        '
        'tabAddressBook
        '
        Me.tabAddressBook.Location = New System.Drawing.Point(4, 22)
        Me.tabAddressBook.Name = "tabAddressBook"
        Me.tabAddressBook.Size = New System.Drawing.Size(879, 353)
        Me.tabAddressBook.TabIndex = 4
        Me.tabAddressBook.Text = "Address Book"
        Me.tabAddressBook.UseVisualStyleBackColor = True
        '
        'tabMining
        '
        Me.tabMining.Location = New System.Drawing.Point(4, 22)
        Me.tabMining.Name = "tabMining"
        Me.tabMining.Size = New System.Drawing.Size(879, 353)
        Me.tabMining.TabIndex = 5
        Me.tabMining.Text = "Mining"
        Me.tabMining.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.SettingsToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(940, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BackupWalletToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(35, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'BackupWalletToolStripMenuItem
        '
        Me.BackupWalletToolStripMenuItem.Name = "BackupWalletToolStripMenuItem"
        Me.BackupWalletToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.BackupWalletToolStripMenuItem.Text = "Backup Wallet"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(141, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EncryptWalletToolStripMenuItem, Me.OptionsToolStripMenuItem})
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(58, 20)
        Me.SettingsToolStripMenuItem.Text = "Settings"
        '
        'EncryptWalletToolStripMenuItem
        '
        Me.EncryptWalletToolStripMenuItem.Name = "EncryptWalletToolStripMenuItem"
        Me.EncryptWalletToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.EncryptWalletToolStripMenuItem.Text = "Encrypt Wallet"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(144, 22)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DebugWindowToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'DebugWindowToolStripMenuItem
        '
        Me.DebugWindowToolStripMenuItem.Name = "DebugWindowToolStripMenuItem"
        Me.DebugWindowToolStripMenuItem.Size = New System.Drawing.Size(146, 22)
        Me.DebugWindowToolStripMenuItem.Text = "Debug Window"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.AboutToolStripMenuItem.Text = "About"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(162, 415)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(72, 25)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 2577
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(802, 427)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "CPU Usage:"
        '
        'lblCPU
        '
        Me.lblCPU.AutoSize = True
        Me.lblCPU.Location = New System.Drawing.Point(874, 427)
        Me.lblCPU.Name = "lblCPU"
        Me.lblCPU.Size = New System.Drawing.Size(33, 13)
        Me.lblCPU.TabIndex = 5
        Me.lblCPU.Text = "100%"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(262, 420)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(56, 20)
        Me.Button2.TabIndex = 6
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(505, 420)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(105, 19)
        Me.Button3.TabIndex = 7
        Me.Button3.Text = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'frmGridCoin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(940, 449)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.lblCPU)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.Name = "frmGridCoin"
        Me.Text = "Gridcoin Client"
        Me.TabControl1.ResumeLayout(False)
        Me.tabOverview.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents tabOverview As System.Windows.Forms.TabPage
    Friend WithEvents tabSendCoins As System.Windows.Forms.TabPage
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackupWalletToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EncryptWalletToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DebugWindowToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tabReceiveCoins As System.Windows.Forms.TabPage
    Friend WithEvents tabTransactions As System.Windows.Forms.TabPage
    Friend WithEvents tabAddressBook As System.Windows.Forms.TabPage
    Friend WithEvents tabMining As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lblBalance As System.Windows.Forms.Label
    Friend WithEvents lblUnconfirmed As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblNumberOfTransactions As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents lvRecent As System.Windows.Forms.ListView
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblCPU As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button

End Class
