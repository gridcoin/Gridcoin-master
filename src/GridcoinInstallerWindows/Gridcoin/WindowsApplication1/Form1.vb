

Public Class Form1


    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim s As New List(Of GPUDevice)


        s = GetCLDevices()


        Stop


    End Sub
End Class
