Public Class Form1

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        KillProcess("gridcoin-qt*")
        KillProcess("cgminer*")
        KillProcess("reaper*")

        KillProcess("guiminer*")

        System.Threading.Thread.Sleep(5000)

        GC.Collect()



        System.Threading.Thread.Sleep(2000)

        'Now Start the wallet.
        Try
            Dim p As Process = New Process()
            Dim pi As ProcessStartInfo = New ProcessStartInfo()
            Dim fi As New System.IO.FileInfo(Application.ExecutablePath)
            pi.WorkingDirectory = fi.DirectoryName
            pi.UseShellExecute = False
            pi.FileName = fi.DirectoryName + "\gridcoin-qt.exe"
            pi.WindowStyle = ProcessWindowStyle.Maximized
            pi.CreateNoWindow = False
            p.StartInfo = pi
            p.Start()
            Threading.Thread.Sleep(200)
            Environment.Exit(0)
            End

        Catch ex As Exception

        End Try


    End Sub


    Public Function KillProcess(ByVal sWildcard As String)
        For Each p As Process In Process.GetProcesses
            If p.ProcessName Like sWildcard Then
                p.Kill()
            End If
        Next
    End Function

End Class
