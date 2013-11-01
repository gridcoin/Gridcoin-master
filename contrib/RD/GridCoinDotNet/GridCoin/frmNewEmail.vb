Imports System.IO


Public Class frmNewEmail
    Public DocumentTemplate As String = ""
    Dim lPass As Long
    Public attachFile As OpenFileDialog


    Private Sub frmNewEmail_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim doc As HtmlDocument
        ToFile(DocumentTemplate)
        If lPass = 0 Then WebBrowser1.Navigate("c:\temp.html")
        txtFrom.Text = frmMail.KeyValue("popfromemail")
        txtFrom.ReadOnly = True
    End Sub
    Private Sub ToFile(ByVal sData As String)
        Dim oWriter As New System.IO.StreamWriter("c:\temp.html")
        oWriter.Write(sData)
        oWriter.Close()
    End Sub
    Private Sub WebBrowser1_DocumentCompleted(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        lPass = lPass + 1
        If lPass = 1 Then
            WebBrowser1.Document.DomDocument.GetType.GetProperty("designMode").SetValue(WebBrowser1.Document.DomDocument, "On", Nothing)
        End If
    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Dim client As Net.Mail.SmtpClient = New Net.Mail.SmtpClient(frmMail.KeyValue("smtphost"))
        Dim n As New System.Net.NetworkCredential(frmMail.KeyValue("popuser"), frmMail.KeyValue("poppassword"))
        client.UseDefaultCredentials = False
        client.Credentials = n
        client.Port = 587
        client.EnableSsl = True
        Dim msg As Net.Mail.MailMessage = New Net.Mail.MailMessage
        Dim em As New System.Net.Mail.MailAddress(txtFrom.Text, txtFrom.Text)
        msg.From = em
        Dim emto As New System.Net.Mail.MailAddress(txtTo.Text, txtTo.Text)
        msg.To.Add(emto)
        msg.Body = WebBrowser1.DocumentText

        For x = 0 To listAttachments.Nodes.Count - 1
            Dim sFile As String
            sFile = listAttachments.Nodes(x).Tag
            If System.IO.File.Exists(sFile) Then
                Dim mat As New Net.Mail.Attachment(sFile)
                msg.Attachments.Add(mat)
            End If

        Next

        msg.Subject = txtSubject.Text
        If InStr(1, LCase(msg.Body), "html") > 0 Then msg.IsBodyHtml = True
        Dim sBody As String
        sBody = frmMail.Encrypt(msg.Body)
        msg.Body = sBody
        msg.Subject = msg.Subject + " - encrypted"
        Try
            client.Send(msg)

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error occurred while Sending")
        End Try
        Me.Close()

    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        attachFile = New OpenFileDialog
        Dim result As DialogResult = attachFile.ShowDialog()
        If result <> DialogResult.OK Then
            Return
        End If
        Dim file As New FileInfo(attachFile.FileName)
        Dim na As TreeNode
        na = listAttachments.Nodes.Add(file.Name)
        na.Tag = file.FullName
    End Sub
End Class