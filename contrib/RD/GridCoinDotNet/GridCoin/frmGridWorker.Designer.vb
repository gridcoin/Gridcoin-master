<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGridWorker
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.AckTimer = New System.Windows.Forms.Timer(Me.components)
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.btnList = New System.Windows.Forms.Button()
        Me.btnSendPage = New System.Windows.Forms.Button()
        Me.btnSendMessage = New System.Windows.Forms.Button()
        Me.tbMessageToSend = New System.Windows.Forms.TextBox()
        Me.rtbConversation = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = True
        Me.BackgroundWorker1.WorkerSupportsCancellation = True
        '
        'AckTimer
        '
        Me.AckTimer.Interval = 60000
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(149, 150)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(39, 13)
        Me.lblStatus.TabIndex = 0
        Me.lblStatus.Text = "Label1"
        '
        'btnList
        '
        Me.btnList.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnList.Location = New System.Drawing.Point(637, 228)
        Me.btnList.Name = "btnList"
        Me.btnList.Size = New System.Drawing.Size(60, 23)
        Me.btnList.TabIndex = 11
        Me.btnList.Text = "List"
        Me.btnList.UseVisualStyleBackColor = True
        '
        'btnSendPage
        '
        Me.btnSendPage.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSendPage.Enabled = False
        Me.btnSendPage.Location = New System.Drawing.Point(556, 228)
        Me.btnSendPage.Name = "btnSendPage"
        Me.btnSendPage.Size = New System.Drawing.Size(75, 23)
        Me.btnSendPage.TabIndex = 10
        Me.btnSendPage.Text = "Page"
        Me.btnSendPage.UseVisualStyleBackColor = True
        '
        'btnSendMessage
        '
        Me.btnSendMessage.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSendMessage.Enabled = False
        Me.btnSendMessage.Location = New System.Drawing.Point(489, 228)
        Me.btnSendMessage.Name = "btnSendMessage"
        Me.btnSendMessage.Size = New System.Drawing.Size(75, 23)
        Me.btnSendMessage.TabIndex = 9
        Me.btnSendMessage.Text = "Send"
        Me.btnSendMessage.UseVisualStyleBackColor = True
        '
        'tbMessageToSend
        '
        Me.tbMessageToSend.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbMessageToSend.Enabled = False
        Me.tbMessageToSend.Location = New System.Drawing.Point(25, 228)
        Me.tbMessageToSend.Name = "tbMessageToSend"
        Me.tbMessageToSend.Size = New System.Drawing.Size(458, 20)
        Me.tbMessageToSend.TabIndex = 8
        '
        'rtbConversation
        '
        Me.rtbConversation.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rtbConversation.Location = New System.Drawing.Point(76, 277)
        Me.rtbConversation.Name = "rtbConversation"
        Me.rtbConversation.ReadOnly = True
        Me.rtbConversation.Size = New System.Drawing.Size(621, 189)
        Me.rtbConversation.TabIndex = 12
        Me.rtbConversation.TabStop = False
        Me.rtbConversation.Text = ""
        '
        'frmGridWorker
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(722, 478)
        Me.Controls.Add(Me.rtbConversation)
        Me.Controls.Add(Me.btnList)
        Me.Controls.Add(Me.btnSendPage)
        Me.Controls.Add(Me.btnSendMessage)
        Me.Controls.Add(Me.tbMessageToSend)
        Me.Controls.Add(Me.lblStatus)
        Me.Name = "frmGridWorker"
        Me.Text = "frmGridWorker"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents AckTimer As System.Windows.Forms.Timer
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents btnList As System.Windows.Forms.Button
    Friend WithEvents btnSendPage As System.Windows.Forms.Button
    Friend WithEvents btnSendMessage As System.Windows.Forms.Button
    Friend WithEvents tbMessageToSend As System.Windows.Forms.TextBox
    Friend WithEvents rtbConversation As System.Windows.Forms.RichTextBox
End Class
