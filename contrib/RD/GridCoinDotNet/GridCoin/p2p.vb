Imports System.Net
Imports System.Threading
Imports System.Net.Sockets
Public Class p2p
    Inherits ApplicationContext

    Private Server As TcpListener = Nothing
    Private ServerThread As Thread = Nothing
    Private WithEvents Tray As New NotifyIcon
    Private Threads As New List(Of Thread)

    Public Sub New()
        ' Tray.Icon = My.Resources.Chat
        Tray.Visible = True
        Tray.Text = "p2p"

        Server = New TcpListener(IPAddress.Any, 8181) ' <-- Listen on Port 50,000
        ServerThread = New Thread(AddressOf ConnectionListener)
        ServerThread.IsBackground = True
        ServerThread.Start()
    End Sub

    Private Sub ConnectionListener()
        Try
            Server.Start()
            While True
                Dim client As TcpClient = Server.AcceptTcpClient ' Blocks until Connection Request is Received
                Dim T As New Thread(AddressOf StartChatForm)
                Threads.Add(T)
                T.Start(client)
            End While
        Catch ex As Exception
            MessageBox.Show("Unable to Accept Connections", "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
        Application.ExitThread()
    End Sub

    Private Sub StartChatForm(ByVal client As Object)
        '  Application.Run(New frmGridWorker(CType(client, TcpClient))) ' Start a New ChatForm with the TcpClient Connection
        ' Threads.Remove(Thread.CurrentThread) ' We don't get here until the ChatForm is closed
    End Sub

    Private Sub Tray_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tray.Click
        'ChatStart.Show()
        'frmMain.Show()

    End Sub

    Private Sub ChatServer_ThreadExit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ThreadExit
        Tray.Visible = False
    End Sub

End Class
