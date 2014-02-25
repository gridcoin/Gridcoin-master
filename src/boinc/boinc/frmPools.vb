Public Class frmPools
    
    
    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click
        Dim bResult As Boolean
        bResult = mclsUtilization.AuthenticateToPool()
        Dim sResult As String
        If bResult = False Then
            sResult = "Authentication failure.  Ensure poolurl, pooluser, and poolpassword are set in the config file."
        Else
            sResult = "Authentication successful."
        End If
        MsgBox(sResult, MsgBoxStyle.Information, "Result")


    End Sub
End Class
