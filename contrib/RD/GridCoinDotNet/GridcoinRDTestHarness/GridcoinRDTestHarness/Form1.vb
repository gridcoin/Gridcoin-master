Public Class Form1
    Public z As New boinc.Utilization

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Retrieve the data from the web Page
        z.SetPublicWalletAddress("")
    End Sub
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        z.ShowMiningConsole()
    End Sub
End Class
