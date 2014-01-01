Imports Finisar.SQLite

Public Class frmSQL
    Private mData As New data


    Private Sub frmSQL_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'Query available tables
        Dim dr As Finisar.SQLite.SQLiteDataReader

        dr = mData.Query("SELECT * FROM dbname.sqlite_master WHERE type='table';")
        While dr.Read
            lvTables.Items.Add(dr.GetString(0))
        End While


    End Sub
End Class