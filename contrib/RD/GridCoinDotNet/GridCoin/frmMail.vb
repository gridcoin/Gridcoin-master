Imports OpenPop
Imports OpenPop.Mime
Imports OpenPop.Mime.Header
Imports OpenPop.Pop3
Imports OpenPop.Pop3.Exceptions
Imports OpenPop.Common.Logging
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Text

Imports Message = OpenPop.Mime.Message
Imports GridCoinDotNet.Gridcoin

Public Class frmMail
    Public WithEvents cms As New ContextMenuStrip
    Public saveFile As SaveFileDialog
    Public p3 As Pop3.Pop3Client

    '    Private sFrom As String
    '   Private sTo As String
    '  Private sSubject As String

    
    Dim messages As New Dictionary(Of Long, Message)

    Public Sub Login()
        Dim sHost As String = KeyValue("pophost")
        Dim sUser As String = KeyValue("popuser")
        Dim sPass As String = KeyValue("poppassword")
        Dim iPort As Long = Val(KeyValue("popport"))
        Dim bSSL As Boolean = IIf(Trim(LCase(KeyValue("popssl"))) = "true", True, False)

        p3 = New Pop3.Pop3Client
        'Add Handlers for Online Storage
        AddHandler listMessages.AfterSelect, AddressOf ListMessagesMessageSelected
        AddHandler listAttachments.AfterSelect, AddressOf ListAttachmentsAttachmentSelected
        AddHandler listMessages.MouseDown, AddressOf MessagesContextHandler
        'Add Handlers for Offline Storage
        AddHandler listOfflineStorage.AfterSelect, AddressOf OfflineMessageSelected
        AddHandler listOfflineStorage.AfterSelect, AddressOf OfflineAttachmentSelected
        AddHandler listOfflineStorage.MouseDown, AddressOf OfflineContextHandler
        If p3.Connected Then
            p3.Disconnect()
        End If
        p3.Connect(sHost, iPort, bSSL)
        p3.Authenticate(sUser, sPass)
    End Sub
    Public Sub RetrievePop3Emails()



        Try
            Login()


            Dim count As Integer = p3.GetMessageCount()

            messages.Clear()
            listMessages.Nodes.Clear()
            listAttachments.Nodes.Clear()

            Dim success As Integer = 0
            Dim fail As Integer = 0
            For i As Integer = count To 1 Step -1
                If IsDisposed Then
                    Return
                End If

                Application.DoEvents()
                Try
                    Dim message As Message = p3.GetMessage(i)
                    'If the message is encrypted, decrypt it and move it:
                    If message.Headers.Subject.Contains("encrypted") Then
                        Dim sRawBody As String = ""
                        Dim sBody As String
                        Dim bIsHTML As Boolean
                        GetBodyOfMessage(message, sRawBody, bIsHTML)
                        Dim sDecrypted = Decrypt(sRawBody)
                        Serialize(message, sDecrypted, i, bIsHTML)
                        GoTo dontaddit
                    End If

                    ' Add the message to the dictionary using the messageNumber
                    messages.Add(i, message)
                    ' Create a TreeNode tree that mimics the Message hierarchy
                    Dim node As TreeNode = New TreeNodeBuilder().VisitMessage(message)
                    node.Tag = i
                    listMessages.Nodes.Add(node)
dontaddit:

                    Application.DoEvents()
                    success += 1
                Catch e As Exception
                    DefaultLogger.Log.LogError(("TestForm: Message fetching failed: " + e.Message & vbCr & vbLf & "Stack trace:" & vbCr & vbLf) + e.StackTrace)
                    fail += 1
                End Try

            Next
            PopulateOffLineMessages()
            If fail > 0 Then
                'Add Logging
            End If
        Catch generatedExceptionName As InvalidLoginException
            MessageBox.Show(Me, "The server did not accept the user credentials!", "Authentication Failure")
        Catch generatedExceptionName As PopServerNotFoundException
            MessageBox.Show(Me, "The server could not be found", "POP3 Retrieval")
        Catch generatedExceptionName As PopServerLockedException
            MessageBox.Show(Me, "The mailbox is locked. It might be in use or under maintenance. Are you connected elsewhere?", "POP3 Account Locked")
        Catch generatedExceptionName As LoginDelayException
            MessageBox.Show(Me, "Login not allowed. Server enforces delay between logins. Have you connected recently?", "POP3 Account Login Delay")
        Catch e As Exception
            MessageBox.Show(Me, "Error occurred retrieving mail. " + e.Message, "POP3 Retrieval")
        Finally
            ' Enable the Gridcoin buttons again
        End Try
    End Sub


    Private Sub OfflineMessageSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)
        Dim fn As String
        fn = listOfflineStorage.SelectedNode.Tag
        Dim sBody As String
        sBody = GetOfflineBody(fn)
        WebBrowser1.DocumentText = sBody
        'Populate the Attachments:
        Dim sFolder As String = GetGridPath("MailAttachments")
        Dim sFG As String
        sFG = Mid(fn, 1, Len(fn) - 4)
        Dim di As New DirectoryInfo(sFolder)
        Dim fiArr As FileInfo() = di.GetFiles()
        Dim fri As FileInfo
        Dim sFGP As String
        listAttachments.Nodes.Clear()
        Dim eFI As New FileInfo(fn)
        For Each fri In fiArr
            If Mid(fri.Name, 1, 35) = Mid(eFI.Name, 1, 35) Then
                sFGP = Mid(fri.FullName, 1, 35)
                Dim zx As New TreeNode
                zx.Text = Mid(fri.FullName, 36, Len(fri.FullName))
                zx.Tag = fri.FullName
                listAttachments.Nodes.Add(zx)
            End If
        Next fri
    End Sub
    Private Sub OfflineAttachmentSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)

    End Sub
    Private Sub OfflineContextHandler(ByVal sender As Object, ByVal e As MouseEventArgs)

    End Sub
    Private Sub ListMessagesMessageSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)
        ' Fetch the selected message
        Dim message As Message = messages(GetMessageNumberFromSelectedNode(listMessages.SelectedNode))
        ' Clear the attachment list from any previus shown attachments
        listAttachments.Nodes.Clear()
        Dim attachments As List(Of MessagePart) = message.FindAllAttachments()
        For Each attachment As MessagePart In attachments
            ' Add the attachment to the list of attachments
            Dim addedNode As TreeNode = listAttachments.Nodes.Add((attachment.FileName))
            ' Keep a reference to the attachment in the Tag property
            addedNode.Tag = attachment
        Next
        ' Only show that attachmentPanel if there is attachments in the message
        Dim hadAttachments As Boolean = attachments.Count > 0
        ' Generate header table
        Dim dataSet As New DataSet()
        Dim table As DataTable = dataSet.Tables.Add("Headers")
        table.Columns.Add("Header")
        table.Columns.Add("Value")
        Dim rows As DataRowCollection = table.Rows
        ' Add all known headers
        rows.Add(New Object() {"Content-Description", message.Headers.ContentDescription})
        rows.Add(New Object() {"Content-Id", message.Headers.ContentId})
        For Each keyword As String In message.Headers.Keywords
            rows.Add(New Object() {"Keyword", keyword})
        Next
        For Each dispositionNotificationTo As RfcMailAddress In message.Headers.DispositionNotificationTo
            rows.Add(New Object() {"Disposition-Notification-To", dispositionNotificationTo})
        Next
        For Each received As Received In message.Headers.Received
            rows.Add(New Object() {"Received", received.Raw})
        Next
        rows.Add(New Object() {"Importance", message.Headers.Importance})
        rows.Add(New Object() {"Content-Transfer-Encoding", message.Headers.ContentTransferEncoding})
        For Each cc As RfcMailAddress In message.Headers.Cc
            rows.Add(New Object() {"Cc", cc})
        Next
        For Each bcc As RfcMailAddress In message.Headers.Bcc
            rows.Add(New Object() {"Bcc", bcc})
        Next
        For Each [to] As RfcMailAddress In message.Headers.[To]
            rows.Add(New Object() {"To", [to]})
        Next
        rows.Add(New Object() {"From", message.Headers.From})
        rows.Add(New Object() {"Reply-To", message.Headers.ReplyTo})
        For Each inReplyTo As String In message.Headers.InReplyTo
            rows.Add(New Object() {"In-Reply-To", inReplyTo})
        Next
        For Each reference As String In message.Headers.References
            rows.Add(New Object() {"References", reference})
        Next
        rows.Add(New Object() {"Sender", message.Headers.Sender})
        rows.Add(New Object() {"Content-Type", message.Headers.ContentType})
        rows.Add(New Object() {"Content-Disposition", message.Headers.ContentDisposition})
        rows.Add(New Object() {"Date", message.Headers.[Date]})
        rows.Add(New Object() {"Date", message.Headers.DateSent})
        rows.Add(New Object() {"Message-Id", message.Headers.MessageId})
        rows.Add(New Object() {"Mime-Version", message.Headers.MimeVersion})
        rows.Add(New Object() {"Return-Path", message.Headers.ReturnPath})
        rows.Add(New Object() {"Subject", message.Headers.Subject})
        ' Add all unknown headers
        For Each key As String In message.Headers.UnknownHeaders
            Dim values As String() = message.Headers.UnknownHeaders.GetValues(key)
            If values IsNot Nothing Then
                For Each value As String In values
                    rows.Add(New Object() {key, value})
                Next
            End If
        Next
        Dim sBody As String
        Dim sRawBody As String = ""
        Dim bIsHTML As Boolean
        sBody = GetBodyOfMessage(message, sRawBody, bIsHTML)
        WebBrowser1.DocumentText = sBody
    End Sub
    Public Function GetBodyOfMessage(ByVal m As Message, ByRef sRawBody As String, ByRef bIsHTML As Boolean) As String
        ' Find the first text/plain version
        Dim sHeader As String = "<PRE>"
        sHeader = sHeader + "From: " + m.Headers.From.ToString() + vbCrLf
        sHeader = sHeader + "To: " + MaToString(m.Headers.To) + vbCrLf
        sHeader = sHeader + "Date: " + m.Headers.DateSent.ToString() + vbCrLf
        sHeader = sHeader + "CC: " + MaToString(m.Headers.Cc) + vbCrLf
        sHeader = sHeader + "</PRE>" + vbCrLf

        Dim plainTextPart As MessagePart = m.FindFirstPlainTextVersion()
        Dim htmlPart As MessagePart = m.FindFirstHtmlVersion()
        Dim sBody As String
        sRawBody = ""
        If htmlPart IsNot Nothing Then
            ' The message had a text/plain version - show that one
            sBody = sHeader + htmlPart.GetBodyAsText()
            sRawBody = htmlPart.GetBodyAsText
            bIsHTML = True
        Else
            If plainTextPart IsNot Nothing Then
                sBody = sHeader + "<pre>" + plainTextPart.GetBodyAsText()
                sRawBody = plainTextPart.GetBodyAsText
                bIsHTML = False
            Else

                Dim textVersions As List(Of MessagePart) = m.FindAllTextVersions()
                If textVersions.Count >= 1 Then
                    sBody = sHeader + "<pre>" + textVersions(0).GetBodyAsText()
                    sRawBody = textVersions(0).GetBodyAsText
                    bIsHTML = False
                End If

            End If

        End If
        Return sBody
    End Function
    Public Function Serialize(ByVal m As Message, ByVal sDecryptedBody As String, ByVal lMessageNumber As Long, ByVal bIsHTML As Boolean)
        'Store the message offline
        Dim sFN As String
        Dim sFG As String
        sFG = Guid.NewGuid.ToString
        sFN = sFG + ".eml"
        Dim sPath As String
        Dim sEmailFolder As String
        sEmailFolder = GetGridPath("Email")
        Try
            If Not System.IO.Directory.Exists(sEmailFolder) Then MkDir(sEmailFolder)

        Catch ex As Exception

        End Try
        sPath = sEmailFolder + "\" + sFN
        Dim sw As New StreamWriter(sPath)
        sw.WriteLine("FROM: " + m.Headers.From.ToString())
        sw.WriteLine("TO: " + MaToString(m.Headers.To))
        sw.WriteLine("CC: " + MaToString(m.Headers.Cc))
        sw.WriteLine("SUBJECT: " + m.Headers.Subject)
        sw.WriteLine("SENT: " + m.Headers.DateSent)
        sw.WriteLine("TYPE: " + IIf(bIsHTML, "HTML", "PLAINTEXT"))
        sw.WriteLine("BODY: " + vbCrLf + sDecryptedBody)

        'Save attachments
        Dim attachments As List(Of MessagePart) = m.FindAllAttachments()
        For Each attachment As MessagePart In attachments
            ' Add the attachment to the list of attachments
            Dim sFile As String
            sFile = GetGridPath("MailAttachments") + "\" + sFG + "_" + attachment.FileName
            Dim file As New FileInfo(sFile)
            If file.Exists Then file.Delete()
            Try
                attachment.Save(file)
            Catch ex As Exception
            End Try
        Next

        p3.DeleteMessage(lMessageNumber)
        sw.Close()


    End Function
    Public Sub PopulateOffLineMessages()
        Dim sFolder As String = GetGridPath("Email")
        Dim di As New DirectoryInfo(sFolder)
        Dim fiArr As FileInfo() = di.GetFiles()
        Dim fri As FileInfo
        Dim sSubject As String
        listOfflineStorage.Nodes.Clear()
        For Each fri In fiArr
            sSubject = GetOfflineSubject(fri.FullName)
            Dim zx As New TreeNode
            zx.Text = sSubject
            zx.Tag = fri.FullName
            listOfflineStorage.Nodes.Add(zx)
        Next fri
    End Sub
    Public Function GetOfflineSubject(ByVal sFileName As String) As String
        Dim sr As New StreamReader(sFileName)
        Dim sSubject As String
        For x = 1 To 4
            sSubject = sr.ReadLine()
        Next
        sr.Close()
        Return sSubject
    End Function
    Public Function GetOfflineBody(ByVal sFileName As String) As String
        Dim sr As New StreamReader(sFileName)
        Dim sSubject As String
        'From, To, Subject, Sent, Type
        Dim sHeader As String
        Dim sTemp As String
        sHeader = "<PRE>"
        For x = 1 To 6
            sTemp = sr.ReadLine()
            sHeader = sHeader + sTemp + vbCrLf
        Next
        sHeader = sHeader + vbCrLf + "</PRE>"
        sTemp = sr.ReadLine() 'Body
        Dim sBody As String = ""
        Dim sOut As String
        Do While sr.EndOfStream = False
            sBody = sBody + sr.ReadLine
        Loop
        sr.Close()
        Return sHeader + sBody
    End Function
    Public Function Decrypt(ByVal sIn As String) As String
        Dim sChar As String
        Dim lAsc As Long
        For x = 1 To Len(sIn)
            sChar = Mid(sIn, x, 1)
            lAsc = Asc(sChar) - 1
            Mid(sIn, x, 1) = Chr(lAsc)
        Next
        Return sIn
    End Function
    Public Function Encrypt(ByVal sIn As String) As String
        Dim sChar As String
        Dim lAsc As Long
        For x = 1 To Len(sIn)
            sChar = Mid(sIn, x, 1)
            lAsc = Asc(sChar) + 1
            Mid(sIn, x, 1) = Chr(lAsc)
        Next
        Return sIn
    End Function
    Public Function MaToString(ByVal c As System.Collections.Generic.List(Of OpenPop.Mime.Header.RfcMailAddress)) As String
        Dim sOut As String
        For x = 0 To c.Count - 1
            sOut = sOut + c(x).DisplayName + " [" + c(x).Address + "]; "
        Next
        Return sOut
    End Function
    Private Shared Function GetMessageNumberFromSelectedNode(ByVal node As TreeNode) As Integer
        If node Is Nothing Then
            Throw New ArgumentNullException("node")
        End If
        ' Check if we are at the root, by seeing if it has the Tag property set to an int
        If TypeOf node.Tag Is Integer Then
            Return CInt(node.Tag)
        End If
        ' Otherwise we are not at the root, move up the tree
        Return GetMessageNumberFromSelectedNode(node.Parent)
    End Function
    Private Sub ListAttachmentsAttachmentSelected(ByVal sender As Object, ByVal args As TreeViewEventArgs)
        ' Fetch the attachment part which is currently selected
        If TypeName(listAttachments.SelectedNode.Tag) = "String" Then
            Exit Sub
        End If
        Dim attachment As MessagePart = DirectCast(listAttachments.SelectedNode.Tag, MessagePart)
        saveFile = New SaveFileDialog
        If attachment IsNot Nothing Then
            saveFile.FileName = attachment.FileName
            Dim result As DialogResult = saveFile.ShowDialog()
            If result <> DialogResult.OK Then
                Return
            End If

            ' Now we want to save the attachment
            Dim file As New FileInfo(saveFile.FileName)
            If file.Exists Then
                file.Delete()
            End If
            Try
                attachment.Save(file)
                MessageBox.Show(Me, "Attachment saved successfully")
            Catch e As Exception
                MessageBox.Show(Me, "Attachment saving failed. Exception message: " + e.Message)
            End Try
        Else
            MessageBox.Show(Me, "Attachment object was null!")
        End If
    End Sub
    Private Sub MessagesContextHandler(ByVal sender As Object, ByVal e As MouseEventArgs)

        If e.Button = Windows.Forms.MouseButtons.Left Then
            If listMessages.ContextMenuStrip Is Nothing Then Exit Sub
            listMessages.ContextMenuStrip.Visible = False
        End If

        If e.Button = Windows.Forms.MouseButtons.Right Then
            listMessages.SelectedNode = listMessages.GetNodeAt(e.X, e.Y)
            cms.Items.Clear()
            cms.Items.Add("Delete Message")
            AddHandler cms.Items(0).Click, AddressOf MenuDeleteMessageClick
            cms.Items.Add("Forward Message")
            AddHandler cms.Items(1).Click, AddressOf MenuForwardMessageClick
            listMessages.ContextMenuStrip = cms
            listMessages.ContextMenuStrip.Show()
        End If

    End Sub
   
    Private Sub MenuForwardMessageClick(ByVal sender As Object, ByVal e As System.EventArgs)
        If listMessages.SelectedNode IsNot Nothing Then
            Dim f As New frmNewEmail
            f.DocumentTemplate = Me.WebBrowser1.DocumentText
            Dim messageNumber As Integer = GetMessageNumberFromSelectedNode(listMessages.SelectedNode)
            Dim m As Message

            Try

                m = p3.GetMessage(messageNumber)

            Catch ex As Exception
                Try
                    Login()
                    m = p3.GetMessage(messageNumber)

                Catch ex2 As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Critical, "Unable to retrieve source message")
                    Exit Sub

                End Try
               

            End Try

            f.txtSubject.Text = m.Headers.Subject
            f.txtTo.Text = m.Headers.From.Address
            'Add the attachments
            Dim attachments As List(Of MessagePart) = m.FindAllAttachments()
            For Each attachment As MessagePart In attachments
                ' Add the attachment to the list of attachments
                Dim sPath As String
                '10-21-2013
                sPath = GetGridPath("Temp") + "\" + attachment.FileName
                Dim file As New FileInfo(sPath)
                If file.Exists Then
                    Try
                        file.Delete()

                    Catch ex As Exception

                    End Try
                End If
                Try
                    attachment.Save(file)
                Catch ex As Exception
                End Try
                Dim addedNode As TreeNode = f.listAttachments.Nodes.Add(sPath)
                ' Keep a reference to the attachment in the Tag property
                addedNode.Tag = sPath
                Try
                    f.listAttachments.Nodes.Add(addedNode)
                Catch ex As Exception

                End Try

            Next
            f.Show()

        End If

    End Sub
    Private Sub MenuDeleteMessageClick(ByVal sender As Object, ByVal e As EventArgs)
        If listMessages.SelectedNode IsNot Nothing Then
            Dim drRet As DialogResult = MessageBox.Show(Me, "Are you sure to delete the email?", "Delete email", MessageBoxButtons.YesNo)
            If drRet = DialogResult.Yes Then
                Dim messageNumber As Integer = GetMessageNumberFromSelectedNode(listMessages.SelectedNode)
                Try
                    Login()
                    p3.DeleteMessage(messageNumber)
                    listMessages.SelectedNode.Remove()
                    listMessages.Refresh()
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Critical, "Error occurred while removing the message.")
                End Try
            End If
        End If
    End Sub
    Public Function GetGridPath(ByVal sType As String) As String
        Dim sTemp As String
        sTemp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Gridcoin\" + sType
        If System.IO.Directory.Exists(sTemp) = False Then
            Try
                System.IO.Directory.CreateDirectory(sTemp)
            Catch ex As Exception

            End Try
        End If
        Return sTemp
    End Function
    Public Function KeyValue(ByVal sKey As String) As String
        Try
            Dim sFolder As String
            sFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Gridcoin"
            Dim sPath As String
            sPath = sFolder + "\gridcoin.conf"
            Dim sr As New StreamReader(sPath)
            Dim sRow As String
            Dim vRow() As String
            Do While sr.EndOfStream = False
                sRow = sr.ReadLine
                vRow = Split(sRow, "=")
                If LCase(vRow(0)) = LCase(sKey) Then
                    Return vRow(1)
                End If
            Loop
        Catch ex As Exception
            Return ""

        End Try
    End Function
    Private Sub UidlButtonClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim uids As List(Of String) = p3.GetMessageUids()

        Dim stringBuilder As New StringBuilder()
        stringBuilder.Append("UIDL:")
        stringBuilder.Append(vbCr & vbLf)
        For Each uid As String In uids
            stringBuilder.Append(uid)
            stringBuilder.Append(vbCr & vbLf)
        Next
        WebBrowser1.DocumentText = stringBuilder.ToString()

    End Sub

    Private Sub MenuViewSourceClick(ByVal sender As Object, ByVal e As EventArgs)
        If listMessages.SelectedNode IsNot Nothing Then
            Dim messageNumber As Integer = GetMessageNumberFromSelectedNode(listMessages.SelectedNode)
            Dim m As Message = messages(messageNumber)
            ' We do not know the encoding of the full message - and the parts could be differently
            ' encoded. Therefore we take a choice of simply using US-ASCII encoding on the raw bytes
            ' to get the source code for the message. Any bytes not in th US-ASCII encoding, will then be
            ' turned into question marks "?"
            '  Dim sourceForm As New ShowSourceForm(Encoding.ASCII.GetString(m.RawMessage))
        End If
    End Sub

    Private Sub MenuStrip1_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles MenuStrip1.ItemClicked

    End Sub

    Private Sub ComposeNewMessageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComposeNewMessageToolStripMenuItem.Click
        frmNewEmail.Show()

    End Sub

    Private Sub RefreshToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshToolStripMenuItem.Click
        RetrievePop3Emails()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click

        Me.Close()

    End Sub
End Class