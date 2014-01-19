﻿Imports Finisar.SQLite
Imports System.Windows.Forms
Imports System.Text

Public Class frmLeaderboard

    Private Sub frmLeaderboard_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim sql As String
        'Populate Gridcoin Network Average Credits Per Project
        sql = "Select '' as Rank, avg(credits) as [Net Avg Credits], ProjectName as [Project Name] from leaderboard " _
            & " group by project   order by Project"
        SqlToGrid(sql, dgvNetworkStats, True)
        'Populate Gridcoin Leaders:
        sql = "Select '' as Rank, avg(credits) Credits, Address [GRC Address] from leaderboard " _
            & " group by Address order by avg(credits) desc "
        SqlToGrid(sql, dgvLeaders, True)
        'Populate Gridcoin Leaders by Project:
        'Dim bp As BoincProject
        Dim iRow As Long = 0
        For y = 0 To UBound(modBoincLeaderboard.vProj)
            If Len(vProj(y)) > 1 Then
                Dim vProjExpanded() As String
                vProjExpanded = Split(vProj(y), "|")
                If UBound(vProjExpanded) = 1 Then
                    sql = "Select '' as Rank, avg(credits) Credits, ProjectName [Project Name], Address [GRC Address] from leaderboard " _
                        & " WHERE ProjectName='" + Trim(vProjExpanded(1)) + "'" _
                        & " Group by Address, ProjectName " _
                        & " Order by avg(credits) desc "
                    SqlToGrid(sql, dgvLeadersByProject, False)
                End If
            End If
        Next
    End Sub

    Private Sub SqlToGrid(sql As String, dgv As DataGridView, bClear As Boolean)
        Dim mData As New Sql
        Dim dr As Finisar.SQLite.SQLiteDataReader
        Try
            dr = mData.Query(sql)
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Gridcoin Analysis Error")
            Exit Sub
        End Try
        If dr Is Nothing Then Exit Sub
        If dr.FieldCount = 0 Then Exit Sub

        If bClear Or dgv.Rows.Count = 0 Then
            dgv.Rows.Clear()
            dgv.Columns.Clear()
            dgv.BackgroundColor = Drawing.Color.Black
            dgv.ForeColor = Drawing.Color.Lime
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
       

        End If

        Dim sValue As String
        Dim iRow As Long = 0


        Try
            If dgv.Rows.Count = 0 Then
            
                For x = 0 To dr.FieldCount - 1
                    Dim dc As New System.Windows.Forms.DataGridViewColumn
                    dc.Name = dr.GetName(x)
                    Dim dgvct As New System.Windows.Forms.DataGridViewTextBoxCell
                    dgvct.Style.BackColor = Drawing.Color.Black
                    dgvct.Style.ForeColor = Drawing.Color.Lime
                    dc.CellTemplate = dgvct
                    dgv.Columns.Add(dc)
                    Dim dcc As New DataGridViewCellStyle
                    dcc.Font = New System.Drawing.Font("Verdana bold", 10, Drawing.FontStyle.Regular)
                    dgv.Columns(x).DefaultCellStyle = dcc
                Next x
                Dim dgcc As New DataGridViewCellStyle
                dgcc.ForeColor = System.Drawing.Color.SandyBrown
                dgv.Font = New System.Drawing.Font("Verdana", 10, Drawing.FontStyle.Bold)
                dgv.ColumnHeadersDefaultCellStyle = dgcc
                dgv.RowHeadersVisible = False
                dgv.EditMode = DataGridViewEditMode.EditProgrammatically
                For x = 0 To dr.FieldCount - 1
                    dgv.Columns(x).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                Next x
                dgv.ReadOnly = True
                
            End If

            While dr.Read
                dgv.Rows.Add()
                iRow = iRow + 1

                For x = 0 To dr.FieldCount - 1
                    sValue = dr(x).ToString
                    If x = 0 Then sValue = iRow
                    dgv.Rows(dgv.Rows.Count - 2).Cells(x).Value = Trim(sValue)

                Next x
        
            End While
            Exit Sub
        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Gridcoin Analysis Error")
        End Try
    End Sub
    
End Class