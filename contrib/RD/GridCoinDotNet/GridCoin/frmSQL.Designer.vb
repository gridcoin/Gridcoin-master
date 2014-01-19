<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSQL
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
        Me.gbQueryAnalyzer = New System.Windows.Forms.GroupBox()
        Me.lvTables = New System.Windows.Forms.ListView()
        Me.rtbQuery = New System.Windows.Forms.RichTextBox()
        Me.gbResultsPane = New System.Windows.Forms.GroupBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.btnExec = New System.Windows.Forms.Button()
        Me.gbQueryAnalyzer.SuspendLayout()
        Me.gbResultsPane.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'gbQueryAnalyzer
        '
        Me.gbQueryAnalyzer.Controls.Add(Me.rtbQuery)
        Me.gbQueryAnalyzer.Controls.Add(Me.lvTables)
        Me.gbQueryAnalyzer.Location = New System.Drawing.Point(28, 22)
        Me.gbQueryAnalyzer.Name = "gbQueryAnalyzer"
        Me.gbQueryAnalyzer.Size = New System.Drawing.Size(903, 334)
        Me.gbQueryAnalyzer.TabIndex = 1
        Me.gbQueryAnalyzer.TabStop = False
        Me.gbQueryAnalyzer.Text = "Query Analyzer"
        '
        'lvTables
        '
        Me.lvTables.Location = New System.Drawing.Point(6, 15)
        Me.lvTables.Name = "lvTables"
        Me.lvTables.Size = New System.Drawing.Size(158, 312)
        Me.lvTables.TabIndex = 1
        Me.lvTables.UseCompatibleStateImageBehavior = False
        '
        'rtbQuery
        '
        Me.rtbQuery.Location = New System.Drawing.Point(170, 15)
        Me.rtbQuery.Name = "rtbQuery"
        Me.rtbQuery.Size = New System.Drawing.Size(727, 313)
        Me.rtbQuery.TabIndex = 2
        Me.rtbQuery.Text = ""
        '
        'gbResultsPane
        '
        Me.gbResultsPane.Controls.Add(Me.DataGridView1)
        Me.gbResultsPane.Location = New System.Drawing.Point(28, 362)
        Me.gbResultsPane.Name = "gbResultsPane"
        Me.gbResultsPane.Size = New System.Drawing.Size(903, 310)
        Me.gbResultsPane.TabIndex = 2
        Me.gbResultsPane.TabStop = False
        Me.gbResultsPane.Text = "Results"
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(6, 19)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(891, 285)
        Me.DataGridView1.TabIndex = 0
        '
        'btnExec
        '
        Me.btnExec.Location = New System.Drawing.Point(375, 3)
        Me.btnExec.Name = "btnExec"
        Me.btnExec.Size = New System.Drawing.Size(76, 28)
        Me.btnExec.TabIndex = 3
        Me.btnExec.Text = "Execute"
        Me.btnExec.UseVisualStyleBackColor = True
        '
        'frmSQL
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(978, 713)
        Me.Controls.Add(Me.btnExec)
        Me.Controls.Add(Me.gbResultsPane)
        Me.Controls.Add(Me.gbQueryAnalyzer)
        Me.Name = "frmSQL"
        Me.Text = "Gridcoin Query Analyzer"
        Me.gbQueryAnalyzer.ResumeLayout(False)
        Me.gbResultsPane.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gbQueryAnalyzer As System.Windows.Forms.GroupBox
    Friend WithEvents rtbQuery As System.Windows.Forms.RichTextBox
    Friend WithEvents lvTables As System.Windows.Forms.ListView
    Friend WithEvents gbResultsPane As System.Windows.Forms.GroupBox
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents btnExec As System.Windows.Forms.Button
End Class
