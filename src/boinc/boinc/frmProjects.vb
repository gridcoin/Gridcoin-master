Public Class frmProjects

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        For x = 1 To 5
            Dim sUserId As String
            Dim c() As Windows.Forms.Control
            c = Me.Controls.Find("txtProject" + Trim(x), True)
            sUserId = c(0).Text
            If IsNumeric(sUserId) Then
                UpdateKey("Project" + Trim(x), sUserId)
            End If
        Next
    End Sub
    Private Sub Refresh()
        For x = 1 To 5
            Dim sUserId As String
            Dim c() As Windows.Forms.Control
            c = Me.Controls.Find("txtProject" + Trim(x), True)
            sUserId = KeyValue("Project" + Trim(x))
            c(0).Text = Trim(sUserId)
        Next
    End Sub

    Private Sub frmProjects_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        Refresh()
        btnQuery_Click(Nothing, Nothing)

    End Sub

    Private Sub GroupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub frmProjects_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Me.Hide()
        e.Cancel = True

    End Sub

    Private Sub btnQuery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuery.Click
        For x = 1 To 5
            Dim sUserId As String
            Dim c() As Windows.Forms.Control
            c = Me.Controls.Find("lblCredits" + Trim(x), True)
            sUserId = KeyValue("Project" + Trim(x))
            Dim dCredits As Double
            dCredits = clsGVM.BoincCreditsByProject(x, Val(sUserId))

            Windows.Forms.Application.DoEvents()
            System.Threading.Thread.Sleep(100)

            c(0).Text = Trim(dCredits)
        Next
    End Sub
End Class
