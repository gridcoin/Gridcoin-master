Imports Finisar.SQLite
Imports System.Windows.Forms
Imports System.Text

Public Class frmSQL
    Private mData As Sql

    Private Sub frmSQL_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'Copy the prod database to the read only database:
        
        Dim sPath As String = GetGridFolder() + "Sql\gridcoin"
        Dim sROPath As String = GetGridFolder() + "Sql\gridcoin_ro"
        Try
            FileCopy(sPath, sROPath)

        Catch ex As Exception

        End Try
        mData = New Sql("gridcoin_ro")


        'Query available tables
        Dim dr As Finisar.SQLite.SQLiteDataReader
        lvTables.View = Windows.Forms.View.Details
        Dim h1 As New System.Windows.Forms.ColumnHeader
        dr = mData.Query("SELECT * FROM sqlite_master WHERE type='table';")
        'dr = mData.Query(".tables;")
        lvTables.Columns.Clear()
        lvTables.Columns.Add("Table")
        lvTables.Columns.Add("Rows")
        lvTables.Columns(1).Width = (lvTables.Width * 0.33) - 1

        lvTables.Columns(0).Width = (lvTables.Width * 0.66) - 1
        lvTables.FullRowSelect = True
        lvTables.HeaderStyle = Windows.Forms.ColumnHeaderStyle.Nonclickable
        Dim iRow As Long
        AddHandler lvTables.DrawColumnHeader, AddressOf lvTables_DrawColumnHeader
        AddHandler lvTables.DrawSubItem, AddressOf lvTables_DrawSubItem
        Dim lRC As Long

        While (dr.Read)
            Dim sTable As String = dr.GetString(1)
            Dim lvItem As New System.Windows.Forms.ListViewItem(sTable)
            Dim sql As String

            sql = "Select count(*) as Count1 from " + Trim(sTable) + ";"
            lRC = Val(mData.QueryFirstRow(sql, "Count1"))
            lvItem.SubItems.Add(Trim(lRC))

            lvItem.BackColor = Drawing.Color.Black
            lvItem.ForeColor = Drawing.Color.Lime
            lvTables.Items.Add(lvItem)
            iRow = iRow + 1
        End While
        dr.Close()

        lvTables.BackColor = Drawing.Color.Black
        lvTables.ForeColor = Drawing.Color.Lime

    End Sub

    Private Sub lvTables_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs)
        e.Graphics.FillRectangle(Drawing.Brushes.Black, e.Bounds)
        e.Graphics.DrawString(e.Header.Text, lvTables.Font, Drawing.Brushes.Lime, e.Bounds)
        e.Graphics.DrawLine(Drawing.Pens.White, e.Bounds.X, e.Bounds.Y + 15, e.Bounds.X + 40, e.Bounds.Y + 15)
    End Sub
    Private Sub lvTables_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs)
        e.Graphics.FillRectangle(Drawing.Brushes.Black, e.Bounds)
        e.DrawText()
    End Sub
    Private Sub btnExec_Click(sender As System.Object, e As System.EventArgs) Handles btnExec.Click
        Dim dr As Finisar.SQLite.SQLiteDataReader
        Try
            dr = mData.Query(rtbQuery.Text)

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Gridcoin Query Analayzer")

            Exit Sub

        End Try
        If dr Is Nothing Then Exit Sub
        dgv.Rows.Clear()
        dgv.Columns.Clear()
        dgv.BackgroundColor = Drawing.Color.Black
        dgv.ForeColor = Drawing.Color.Lime
        Dim sValue As String
        Dim iRow As Long
        Try

        For x = 0 To dr.FieldCount - 1
            Dim dc As New System.Windows.Forms.DataGridViewColumn
            dc.Name = dr.GetName(x)
            Dim dgvct As New System.Windows.Forms.DataGridViewTextBoxCell
            dgvct.Style.BackColor = Drawing.Color.Black
            dgvct.Style.ForeColor = Drawing.Color.Lime
            dc.CellTemplate = dgvct
            dgv.Columns.Add(dc)
            Next x
            Dim dgcc As New DataGridViewCellStyle

            dgcc.ForeColor = System.Drawing.Color.SandyBrown
            dgv.ColumnHeadersDefaultCellStyle = dgcc
            For x = 0 To dr.FieldCount - 1
                dgv.Columns(x).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            Next

         While dr.Read
            dgv.Rows.Add()
            For x = 0 To dr.FieldCount - 1
                sValue = dr(x).ToString
                dgv.Rows(iRow).Cells(x).Value = sValue
            Next x
            iRow = iRow + 1

            End While
            For x = 0 To dr.FieldCount - 1
                dgv.Columns(x).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            Next

            Exit Sub
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Gridcoin Query Analayzer")

        End Try

    End Sub
    Public Function SerializeTable(sTable As String, lStartRow As Long, lEndRow As Long) As StringBuilder
        Dim sql As String
        sql = "Select * From " + sTable + " WHERE ID >= " + Trim(lStartRow) + " AND ID <= " + Trim(lEndRow)
        Dim dr As Finisar.SQLite.SQLiteDataReader
        dr = mData.Query(sql)
        Dim iRow As Long
        Dim sbOut As New StringBuilder
        Dim sRow As String
        Dim sValue As String
        While dr.Read
            iRow = iRow + 1
            sRow = ""
            For x = 0 To dr.FieldCount - 1
                sValue = "" & dr(x).ToString
                sRow = sRow & sValue & "|"
            Next x
            sbOut.AppendLine(sRow)
        End While
        dr.Close()
        Return sbOut
    End Function
    Public Function GetManifestForTable(sTable As String) As String
        Dim sql As String
        sql = "Select min(id) as lmin From " + sTable
        Dim dr As Finisar.SQLite.SQLiteDataReader
        Dim lStart As Long
        Dim lEnd As Long
        lStart = mData.QueryFirstRow(sql, "lmin")
        sql = "Select max(id) as lmax from " + sTable
        lEnd = mData.QueryFirstRow(sql, "lmax")
        Dim sOut As String
        sOut = Trim(sTable) + "," + Trim(lStart) + "," + Trim(lEnd)
        Return sOut

    End Function
    Public Function CreateManifest() As StringBuilder
        Dim dr As Finisar.SQLite.SQLiteDataReader
        dr = mData.Query("SELECT * FROM sqlite_master WHERE type='table';")
        'todo order by
        Dim iRow As Long
        Dim sRow As String
        Dim sbManifest As New StringBuilder
        While (dr.Read)
            Dim sTable As String = dr.GetString(1)
            sRow = GetManifestForTable(sTable)
            sbManifest.AppendLine(sRow)
            iRow = iRow + 1
        End While
        Return sbManifest

    End Function
    
    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs)
        Dim s As New StringBuilder
        s = CreateManifest()
        s = SerializeTable("peers", 1, 1)
        s = SerializeTable("system", 1, 1)
    End Sub
    Private Sub rtbQuery_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles rtbQuery.KeyDown
        If e.KeyCode = Keys.F5 Then
            Call btnExec_Click(Nothing, Nothing)
        End If
    End Sub
End Class