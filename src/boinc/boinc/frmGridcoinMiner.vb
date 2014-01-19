﻿Imports ICSharpCode.SharpZipLib.Core
Imports System.IO

Imports ICSharpCode.SharpZipLib.Zip


Public Class frmGridcoinMiner
    Public mGpus As New EnumerateGPUs


    Private Function GetVal(sName) As String
        Dim c() As Windows.Forms.Control
        Try

            c = Me.Controls.Find("txt_" + sName, True)
            Dim sOut As String
            sOut = c(0).Text
            If sName = "enabled" Then
                Dim cChk As System.Windows.Forms.CheckBox
                cChk = DirectCast(c(0), System.Windows.Forms.CheckBox)
                sOut = Trim(CBool(cChk.Checked))
            End If

            Return sOut
        Catch ex As Exception

        End Try

    End Function

    Private Sub UpdateMinerKey(sDevId As String, sKey As String)
        Dim sValue As String
        sValue = GetVal(sKey)
        Dim sKey2 As String
        sKey2 = LCase("dev" + Trim(sDevId) + "_" + sKey)
        UpdateKey(sKey2, sValue)
    End Sub
    Private Sub RefreshMinerUI(sDevId As String, sKey As String)
        Dim c() As Windows.Forms.Control
        c = Me.Controls.Find("txt_" + sKey, True)
        Dim sOut As String
        Dim sValue As String
        sValue = KeyValue("dev" + Trim(sDevId) + "_" + sKey)
        If sKey = "enabled" Then
            Dim cChk As System.Windows.Forms.CheckBox
            cChk = DirectCast(c(0), System.Windows.Forms.CheckBox)
            If sValue = "" Then sValue = "0"
            cChk.Checked = CBool(sValue)
            Exit Sub
        End If
        c(0).Text = sValue

    End Sub
    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try

            Dim sDevId As String
            sDevId = cmbDeviceID.SelectedIndex
            UpdateMinerKey(sDevId, "gpu_thread_concurrency")
            UpdateMinerKey(sDevId, "worksize")
            UpdateMinerKey(sDevId, "intensity")
            UpdateMinerKey(sDevId, "lookup_gap")
            UpdateMinerKey(sDevId, "enabled")
            UpdateKey("dev" + Trim(sDevId) + "_name", lblDeviceName.Text)

        Catch ex As Exception

        End Try
    End Sub
    Private Sub Refresh()
        Dim sDevId As String
        sDevId = cmbDeviceID.SelectedIndex
        RefreshMinerUI(sDevId, "gpu_thread_concurrency")
        RefreshMinerUI(sDevId, "worksize")
        RefreshMinerUI(sDevId, "intensity")
        RefreshMinerUI(sDevId, "lookup_gap")
        RefreshMinerUI(sDevId, "enabled")
        lblDeviceName.Text = cmbDeviceID.Text
    End Sub

    Private Sub frmProjects_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        Refresh()

    End Sub

    Private Sub frmProjects_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Me.Hide()
        e.Cancel = True

    End Sub


    Private Sub btnRefresh_Click(sender As System.Object, e As System.EventArgs) Handles btnRefresh.Click
        Call Refresh()

    End Sub

    Private Sub cmbDeviceID_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbDeviceID.SelectedIndexChanged
        Call Refresh()

    End Sub

    Private Sub frmGridcoinMiner_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        cmbDefaults.Items.Add("None")
        cmbDefaults.Items.Add("7950-Tahiti-Low")
        cmbDefaults.Items.Add("7950-Tahiti-High")
        cmbDefaults.Items.Add("6870-Barts")
        cmbDefaults.Items.Add("7870-Pitcairn")


        'Add each real device
        Dim gpus As List(Of GPUEnumerator.GPU)
        gpus = mGpus.SuckInGPUs()
        For x = 0 To gpus.Count - 1
            Dim sNarr As String = Trim(gpus(x).id) + " - " + Trim(gpus(x).name) + " - " + Trim(gpus(x).vendor) '+ " - " + Trim(gpus(x).version)
            cmbDeviceID.Items.Add(sNarr)
            If x = 0 Then cmbDeviceID.SelectedIndex = 0
        Next

        If gpus.Count > 0 Then
            Call btnRefresh_Click(Nothing, Nothing)

        End If
    End Sub

    Private Sub cmbDefaults_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbDefaults.SelectedIndexChanged
        If cmbDefaults.Text = "7950-Tahiti-Low" Then
            txt_gpu_thread_concurrency.Text = "8192"
            txt_worksize.Text = "256"
            txt_intensity.Text = "13"
            txt_lookup_gap.Text = "2"
        End If
        If cmbDefaults.Text = "7950-Tahiti-High" Then
            txt_gpu_thread_concurrency.Text = "21712"
            txt_worksize.Text = "256"
            txt_intensity.Text = "19"
            txt_lookup_gap.Text = "2"
        End If
        If cmbDefaults.Text = "6870-Barts" Then
            txt_gpu_thread_concurrency.Text = "6720"
            txt_worksize.Text = "256"
            txt_intensity.Text = "16"
            txt_lookup_gap.Text = "2"
        End If
        If cmbDefaults.Text = "7870-Pitcairn" Then
            txt_gpu_thread_concurrency.Text = "16384"
            txt_worksize.Text = "256"
            txt_intensity.Text = "17"
            txt_lookup_gap.Text = "2"
        End If
    End Sub

    Private Sub btnCreateCgminerInstance_Click(sender As System.Object, e As System.EventArgs) Handles btnCreateCgminerInstance.Click
        Dim sPath As String
        Dim lDevId As Long
        lDevId = cmbDeviceID.SelectedIndex
        btnSave_Click(Nothing, Nothing)

        sPath = GetGridFolder() + "cgminer" + Trim(lDevId) + "\"
        Dim sNarr As String = "Warning:  This command will create a new instance of CGMiner in folder " + sPath + " and will overwrite the cgm" + Trim(lDevId) + " configuration file.  Continue?"
        Dim lResult As MsgBoxResult = MsgBox(sNarr, MsgBoxStyle.OkCancel, "Create CG Miner Instance")

        If lResult <> MsgBoxResult.Ok Then Exit Sub
        KillGuiMiner()
        System.Threading.Thread.Sleep(2000)



        Try
            If Not System.IO.Directory.Exists(sPath) Then MkDir(sPath)
            Dim sSourcePath As String = GetGRCAppDir() + "\"

            Dim sSourceZip As String = sSourcePath + "cgminer.zip"

            Dim sBase64File As String = sSourcePath + "cgminer_base64.zip"
            If System.IO.File.Exists(sBase64File) Then
                UnBase64File(sBase64File, sSourceZip)
                System.Threading.Thread.Sleep(1000)
            End If


            Dim sSourceZipPreferred As String = GetGridFolder() + "cgminer.zip"

            If System.IO.File.Exists(sSourceZipPreferred) Then sSourceZip = sSourceZipPreferred 'Allow user to override default cgminer.zip


            If Not System.IO.File.Exists(sSourceZip) Then
                MsgBox(sSourcePath + " not found. ", vbCritical, "Gridcoin - Create New CGMiner Instance")
                Exit Sub

            End If


            ExtractZipFile(sSourceZip, sPath)


            Dim sCGPath As String
            sCGPath = sPath + "cgm" + Trim(lDevId)
            Dim sPort As String
            sPort = KeyValue("rpcport")
            Dim sUser As String = KeyValue("rpcuser")
            Dim sPass As String = KeyValue("rpcpassword")
            Dim lIntensity As Long = KeyValue("dev" + Trim(lDevId) + "_intensity")

            Dim lWS As Long = KeyValue("dev" + Trim(lDevId) + "_worksize")
            Dim lLG As Long = KeyValue("dev" + Trim(lDevId) + "_lookup_gap")
            Dim lTC As Long = KeyValue("dev" + Trim(lDevId) + "_gpu_thread_concurrency")

            Dim sResult As String
            sResult = WriteCgMinerFile(lDevId, sPort, sUser, sPass, lIntensity, lWS, lLG, lTC, sCGPath)
            MsgBox(sResult, vbInformation, "Result")

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Error while creating Cg Miner Instance")
        End Try

    End Sub
    Public Sub ExtractZipFile(archiveFilenameIn As String, outFolder As String)

        Try

        Dim di As New DirectoryInfo(outFolder)
        di.Delete(True)
        di.Create()
        Catch ex As Exception

        End Try

        Dim zf As ZipFile = Nothing
        Try
            Dim fs As FileStream = File.OpenRead(archiveFilenameIn)
            zf = New ZipFile(fs)
            '    If Not [String].IsNullOrEmpty(password) Then    ' AES encrypted entries are handled automatically
            'zf.Password = password
            'End If
            For Each zipEntry As ZipEntry In zf
                If Not zipEntry.IsFile Then     ' Ignore directories
                    Continue For
                End If
                Dim entryFileName As [String] = zipEntry.Name

                Dim buffer As Byte() = New Byte(4095) {}    ' 4K is optimum
                Dim zipStream As Stream = zf.GetInputStream(zipEntry)

                ' Manipulate the output filename here as desired.
                Dim fullZipToPath As [String] = Path.Combine(outFolder, entryFileName)
                Using streamWriter As FileStream = File.Create(fullZipToPath)
                    StreamUtils.Copy(zipStream, streamWriter, buffer)
                End Using
            Next
        Finally
            If zf IsNot Nothing Then
                zf.IsStreamOwner = True     ' Makes close also shut the underlying stream
                ' Ensure we release resources
                zf.Close()
            End If
        End Try
    End Sub


End Class
