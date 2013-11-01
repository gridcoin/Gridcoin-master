Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim g As New boinc.frmMail
        Dim lVer As Long
        Dim h As New boinc.Utilization
        lVer = h.Version

        g.Show()
        g.RetrievePop3Emails()

    End Sub
End Class
