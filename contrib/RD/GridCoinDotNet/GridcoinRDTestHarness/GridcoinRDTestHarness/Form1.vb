
Imports boinc

Imports System.Net.HttpWebRequest
Imports System.Text
Imports System.IO


Imports System.Net

Public Class Form1

    'Public m As New Utilization


    Public r As New Utilization

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Retrieve the data from the web Page
        'Call CgSend("gpuintensity|0,15")
        '        Dim u As Double
        '       u = r.BoincUtilization
        Dim lb As New frmLeaderboard
        lb.Show()


        ' Debug.Print(u)
        r.UpdateLeaderBoard()

        Dim x As New frmSQL
        x.Show()
        Exit Sub


        'Dim z As New Utilization
        'z.ShowMiningConsole()
        Dim d As New Sql
        Dim sIp As String
        Dim bIp As Boolean
    
    End Sub
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

     

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

    End Sub
End Class
