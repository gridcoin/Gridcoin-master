
Imports System.Net.Sockets

Public Class frmGridWorker



    Public IP As String
    Public TCPNumber As Long


    Public Enum MessageCodes ' Enter all codes below in --> UPPER CASE <---
        ACK = 0
        TEXT = 1
        DISCONNECTED = 2
        GREETING = 3
        GREETINGRESPONSE = 4
        PAGE = 5
        TYPING = 6
        TYPINGCANCEL = 7
        GETLIST = 8
    End Enum

    Private ConnectTo As String = ""
    Private ChatClient As TcpClient = Nothing
    Private FieldMarker As String = Chr(0)
    Private MessageMarker As String = Chr(1)
    Private Typing() As String = {"/", "-", "\", "|"}
    Private Const AckIntervalInSeconds As Integer = 60
    Private ContinueProcessingMessages As Boolean = True
    Private SendFinalDisconnectMessage As Boolean = False
 
    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(ByVal ConnectTo As String)
        InitializeComponent()
        Me.ConnectTo = ConnectTo
        Me.Text = "Connecting to " & ConnectTo & " ..."
    End Sub

    Public Sub New(ByVal ChatClient As TcpClient)
        InitializeComponent()
        Me.ChatClient = ChatClient
        mConnectionCount = mConnectionCount + 1
        Me.TCPNumber = mConnectionCount

        '6-9
        mConnections.Add(mConnectionCount, Me)


        Me.IP = ChatClient.Client.RemoteEndPoint.ToString

        Dim sSucker As String
        sSucker = ChatClient.Client.LocalEndPoint.ToString




    End Sub

    Private Sub ChatForm_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        AckTimer.Interval = TimeSpan.FromSeconds(AckIntervalInSeconds).TotalMilliseconds
        SetChatState(False)
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub SetChatState(ByVal state As Boolean)
        AckTimer.Enabled = state
    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If IsNothing(ChatClient) Then ' We are initiating the connection
            Try
                ChatClient = New TcpClient()
                ChatClient.Connect(ConnectTo, 50000) ' Blocks until connection is made
            Catch ex As Exception
                ContinueProcessingMessages = False
                BackgroundWorker1.ReportProgress(-2) ' Connection Failed
            End Try
        Else
            SendMessage(MessageCodes.GREETING, Environment.MachineName) ' We accepted a Connection: Send our name
        End If

        If Not IsNothing(ChatClient) AndAlso ChatClient.Connected Then
            BackgroundWorker1.ReportProgress(1) ' Enable the Chat Interface
            SendFinalDisconnectMessage = True

            Dim bytesRead As Integer
            Dim buffer(1024) As Byte
            Dim Messages As String = ""
            Dim MessageMarkerIndex As Integer
            While ContinueProcessingMessages
                Try
                    bytesRead = ChatClient.GetStream.Read(buffer, 0, buffer.Length) ' <-- Blocks until Data is Received
                    If bytesRead > 0 Then ' <-- Zero is returned if Connection is Closed and no more data is available
                        Messages = Messages & System.Text.ASCIIEncoding.ASCII.GetString(buffer, 0, bytesRead) ' Append the received data to our message queue
                        MessageMarkerIndex = Messages.IndexOf(MessageMarker) ' See if the End of Message marker is present
                        While MessageMarkerIndex <> -1 ' If we have received at least one complete message
                            BackgroundWorker1.ReportProgress(0, Messages.Substring(0, MessageMarkerIndex)) ' Let the GUI handle the complete Message
                            Messages = Messages.Remove(0, MessageMarkerIndex + 1) ' Remove the processed message
                            MessageMarkerIndex = Messages.IndexOf(MessageMarker) ' See if there are more End of Message markers present
                        End While
                    End If
                Catch ex As Exception
                    ContinueProcessingMessages = False
                    BackgroundWorker1.ReportProgress(-1)
                End Try
            End While
        End If
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged

        Select Case e.ProgressPercentage
            Case -1 ' Raised From Exception in Receiving Loop in BackgroundWorker()
                SendFinalDisconnectMessage = False
                SetChatState(False)
                Me.Text = Me.Text & " {Connection Lost}"

            Case -2 ' Initial Connection Failed
                SendFinalDisconnectMessage = False
                Me.Text = "Failed to Connect!"
                MessageBox.Show("No response from " & ConnectTo & " ...", "Connection Failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Me.Close()

            Case 1 ' Connection Made
                SetChatState(True) ' Enable the Chat Interface

            Case 0 ' Normal Message Received
                If Not IsNothing(e.UserState) AndAlso TypeOf e.UserState Is String Then
                    Dim msg As String = CType(e.UserState, String)
                    Dim values() As String = msg.Split(FieldMarker)
                    If values.Length >= 2 Then ' Forward compatibility for messages with more than two fields
                        Dim strCode As String = values(0)
                        Dim value As String = values(1) ' All messages should have at least two fields (even if the second isn't used)
                        Try
                            Dim code As MessageCodes = [Enum].Parse(GetType(MessageCodes), strCode.ToUpper)
                            Select Case code
                                Case MessageCodes.ACK ' Ack signal received 


                                Case MessageCodes.GETLIST
                                    DisplayMessage(Color.DarkGreen, value)


                                Case MessageCodes.GREETING ' We have received a name from the other side
                                    Me.Text = value
                                    DisplayMessage(Color.Red, value & " Connected")

                                    SendMessage(MessageCodes.GREETINGRESPONSE, Environment.MachineName) ' Send our name back...

                                Case MessageCodes.GREETINGRESPONSE ' We sent our name and have now received a name back
                                    Me.Text = value
                                    DisplayMessage(Color.Red, value & " Connected")

                                Case MessageCodes.TYPING ' The other side is typing a message...
                                    Static index As Integer = -1
                                    index = index + 1
                                    If index > Typing.GetUpperBound(0) Then
                                        index = 0
                                    End If
                                    lblStatus.Text = value & " " & Typing(index)

                                Case MessageCodes.TYPINGCANCEL ' The other side has cleared their message textbox
                                    lblStatus.Text = ""

                                Case MessageCodes.TEXT ' A text message from the other person has arrived
                                    DisplayMessage(Color.DarkGreen, value)


                                Case MessageCodes.PAGE ' We have received a Page request

                                Case MessageCodes.DISCONNECTED ' The other person closed their chat window
                                    ContinueProcessingMessages = False
                                    lblStatus.Text = ""
                                    DisplayMessage(Color.Red, value)
                                    Me.Text = Me.Text & " {Disconnected}"
                                    SetChatState(False)
                                    SendFinalDisconnectMessage = False

                            End Select
                        Catch ex As Exception
                        End Try
                    End If
                End If
        End Select
    End Sub

    Public Function SendMessage(ByVal Code As MessageCodes, ByVal Value As String) As Boolean
        Try ' Async Write so we don't lock up the GUI in the event of dropped connections
            Dim msg() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(Code.ToString & FieldMarker & Value & MessageMarker)

         


            ChatClient.GetStream.BeginWrite(msg, 0, msg.Length, AddressOf MyWriteCallBack, ChatClient.GetStream)
            Return True
        Catch ex As Exception
            SendFinalDisconnectMessage = False
        End Try
        Return False
    End Function

    Public Sub MyWriteCallBack(ByVal ar As IAsyncResult)
        Try
            CType(ar.AsyncState, NetworkStream).EndWrite(ar)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub AckTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SendMessage(MessageCodes.ACK, "Ack")
    End Sub

    Private Sub tbMessageToSend_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbMessageToSend.TextChanged

    End Sub

    Private Sub btnSendMessage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSendMessage.Click

    End Sub

    Private Sub DisplayMessage(ByVal clr As Color, ByVal msg As String)
        Try
            Dim SelStart As Integer = tbMessageToSend.SelectionStart
            Dim SelLength As Integer = tbMessageToSend.SelectionLength

            rtbConversation.SelectionStart = rtbConversation.TextLength
            rtbConversation.SelectionColor = clr
            rtbConversation.SelectedText = "[" & DateTime.Now.ToShortTimeString & "] " & msg & vbCrLf
            rtbConversation.Focus()
            rtbConversation.ScrollToCaret()

            tbMessageToSend.Focus()
            tbMessageToSend.SelectionStart = SelStart
            tbMessageToSend.SelectionLength = SelLength
        Catch ex As Exception
        End Try
    End Sub


    Private Sub btnSendPage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSendPage.Click
        SendMessage(MessageCodes.PAGE, "Paging")
    End Sub

    Private Sub ChatForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If SendFinalDisconnectMessage Then
            SendMessage(MessageCodes.DISCONNECTED, Environment.MachineName & " Disconnected")
        End If
        Try
            If Not IsNothing(ChatClient) AndAlso ChatClient.Connected Then
                ChatClient.Close()
            End If
        Catch ex As Exception
        End Try
    End Sub

End Class