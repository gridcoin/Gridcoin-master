Imports System.Net
Imports System.IO

Public Class Form1


    Private prodURL As String = "http://www.gridcoin.us/download/"
    Private testURL As String = "http://www.gridcoin.us/download/signed/"
    Private bTestNet As Boolean


    Private Sub RemoveBlocksDir(d As System.IO.DirectoryInfo)
        Try
            d.Delete(True)
        Catch ex As Exception
        End Try
    End Sub
    Public Sub KillMiners()
        For x = 1 To 5
            KillProcess("guiminer*")
            KillProcess("cgminer")
            KillProcess("GridcoinMiner*")
            KillProcess("reaper*")
        Next x
        System.Threading.Thread.Sleep(3000)
        GC.Collect()
    End Sub
    Private Function GetURL() As String
        If bTestNet Then
            Return testURL
        Else
            Return prodURL
        End If
    End Function
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load





        '''''''''''''''''''''RESTORE SNAPSHOT
        If Environment.GetCommandLineArgs.Length > 0 Then

            If Environment.CommandLine.Contains("testnet") Then
                bTestNet = True
            End If

            If Environment.CommandLine.Contains("restoresnapshot") Then
                Try
                    KillMiners()
                    KillProcess("gridcoin*")
                    Me.Show()
                    Me.BringToFront()
                    Me.Update() : Me.Refresh() : Application.DoEvents()
                    txtStatus.Text = "Restoring block chain from snapshot..."
                    Me.Refresh()
                    Application.DoEvents()
                    System.Threading.Thread.Sleep(7000)
                    Dim bErr As Boolean
                    For x = 1 To 5
                        Try
                            Call RestoreFromSnapshot()
                        Catch ex As Exception
                            System.Threading.Thread.Sleep(3000)
                            bErr = True
                        End Try
                        If Not bErr Then Exit For
                    Next x
                    StartGridcoin()
                    Environment.Exit(0)
                    End
                Catch ex As Exception
                End Try
            End If
        End If

        '''''''''''''''''''RESTORE POINT (No need to Kill Miners)
        If Environment.GetCommandLineArgs.Length > 0 Then
            If Environment.CommandLine.Contains("restorepoint") Then
                Call Snapshot()
                Environment.Exit(0)
                End
            End If
        End If


        ''''''''''''''''''''''REINDEX KILL MINERS
        If Environment.GetCommandLineArgs.Length > 0 Then
            If Environment.CommandLine.Contains("reindex") Then
                Try
                    KillMiners()
                    KillProcess("gridcoin*")
                    System.Threading.Thread.Sleep(1000)
                    RemoveGrcDataDir()
                    StartGridcoin()
                    Environment.Exit(0)
                    End

                Catch ex As Exception
                End Try
            End If
        End If
        ''''''''''''''''''''''''Upgrade the Upgrader Program
        If Environment.CommandLine.Contains("restoregrcrestarter") Then
            Try
                System.Threading.Thread.Sleep(3000)
                Dim sSource As String = GetGRCAppDir() + "\" + "grcrestarter_copy.exe"
                Dim sTarget As String = GetGRCAppDir() + "\" + "grcrestarter.exe"
                FileCopy(sSource, sTarget)
                Environment.Exit(0)
            Catch ex As Exception

            End Try
        End If
        '''''''''''''''''''''''''''''''''UPGRADE'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If Environment.CommandLine.Contains("testnetupgrade") Then
            bTestNet = True
        End If
        If Environment.CommandLine.Contains("upgrade") Then
            Try
                KillMiners()
                ProgressBar1.Maximum = 1000
                ProgressBar1.Value = 1
                Me.Show()
                Me.BringToFront()
                Me.Update()
                Me.Refresh()
                Application.DoEvents()
                txtStatus.Text = "Waiting for Gridcoin Wallet to exit..."
                Me.Refresh()
                Application.DoEvents()
                System.Threading.Thread.Sleep(8000)
                KillProcess("gridcoin*")

                'Upgrade the wallet before continuing
                Dim sMsg As String = DynamicUpgradeWithManifest()
                If Len(sMsg) > 0 Then
                    MsgBox("Upgrade Failed. " + sMsg, MsgBoxStyle.Critical)
                    Environment.Exit(0)
                End If
                txtStatus.Text = "Upgrade Successful."
                For x = ProgressBar1.Value To ProgressBar1.Maximum
                    ProgressBar1.Value = x
                    Me.Refresh()
                    Me.Update()
                    Application.DoEvents()
                Next
                StartGridcoin()

                UpgradeGrcRestarter("restoregrcrestarter")
                Environment.Exit(0)
                End

            Catch ex As Exception
                MsgBox("Upgrade Failed. " + ex.Message, MsgBoxStyle.Critical)
            End Try
        End If

        If Environment.CommandLine.Contains("unbase64file") Then
            UnBase64File("cgminer.zip")
            Environment.Exit(0)
        End If

        If Environment.CommandLine.Contains("base64file") Then
            Base64File("cgminer.zip")
            Environment.Exit(0)
        End If
        ''''''''''''''''''''''''''''''''''''''Else....
        If Environment.CommandLine.Contains("120") Then
            System.Threading.Thread.Sleep(120000)
        End If
        KillMiners()
        StartGridcoin()
        Environment.Exit(0)
        End
    End Sub
    Public Sub StartGridcoin()
        'Start the wallet.
        Try
            Dim p As Process = New Process()
            Dim pi As ProcessStartInfo = New ProcessStartInfo()
            Dim fi As New System.IO.FileInfo(Application.ExecutablePath)
            pi.WorkingDirectory = fi.DirectoryName
            pi.UseShellExecute = True
            pi.FileName = fi.DirectoryName + "\gridcoin-qt.exe"
            If bTestNet Then pi.Arguments = "-testnet"

            pi.WindowStyle = ProcessWindowStyle.Maximized
            pi.CreateNoWindow = False
            p.StartInfo = pi
            p.Start()
        Catch ex As Exception
        End Try
    End Sub
    Public Sub RemoveGrcDataDir()
        For x = 1 To 10
            Dim sDataDir As String = GRCDataDir()
            Dim sBlocks = sDataDir + "blocks"
            Dim sChain = sDataDir + "chainstate"
            Dim sDatabase = sDataDir + "database"
            Dim dBlock As New System.IO.DirectoryInfo(sBlocks)
            RemoveBlocksDir(dBlock)
            Dim dChain As New System.IO.DirectoryInfo(sChain)
            RemoveBlocksDir(dChain)
            Dim dDatabase As New System.IO.DirectoryInfo(sDatabase)
            RemoveBlocksDir(dDatabase)
            If dDatabase.Exists = False And dChain.Exists = False And dBlock.Exists = False Then Exit For
            Threading.Thread.Sleep(1000)
        Next
    End Sub
    Public Sub Snapshot()
        Dim sDataDir As String = GRCDataDir()
        Dim sBlocks = sDataDir + "blocks"
        Dim sChain = sDataDir + "chainstate"
        Dim sSnapshotBlocks = sDataDir + "snapshot\blocks"
        Dim sSnapshotChain = sDataDir + "snapshot\chainstate"
        Dim sSnapDir As String = sDataDir + "snapshot"
        Dim Dsnap As DirectoryInfo = New DirectoryInfo(sSnapDir)
        Try
            Dsnap.Delete(True)
        Catch ex As Exception
        End Try
        DirectorySnapshot(sBlocks, sSnapshotBlocks, True)
        DirectorySnapshot(sChain, sSnapshotChain, True)
    End Sub
    Public Sub RestoreFromSnapshot()
        Dim sDataDir As String = GRCDataDir()
        Dim sBlocks = sDataDir + "blocks"
        Dim sChain = sDataDir + "chainstate"
        Dim sDatabase = sDataDir + "database"
        Dim sSnapshotBlocks = sDataDir + "snapshot\blocks"
        Dim sSnapshotChain = sDataDir + "snapshot\chainstate"
        Dim sSnapshotDatabase = sDataDir + "snapshot\database"
        RemoveGrcDataDir()

        Try
            DirectorySnapshot(sSnapshotBlocks, sBlocks, True)
        Catch ex As Exception
        End Try
        Try
            DirectorySnapshot(sSnapshotChain, sChain, True)
        Catch ex As Exception
        End Try
        Try
        Catch ex As Exception

        End Try
    End Sub
    Private Sub DirectorySnapshot( _
        ByVal sourceDirName As String, _
        ByVal destDirName As String, _
        ByVal copySubDirs As Boolean)
        ' Get the subdirectories for the specified directory. 
        Dim dir As DirectoryInfo = New DirectoryInfo(sourceDirName)
        Dim dirs As DirectoryInfo() = dir.GetDirectories()
        If Not dir.Exists Then
            'Throw New DirectoryNotFoundException( _                "Source directory does not exist or could not be found: " _                + sourceDirName)
        End If
        'Remove the destination directory
        Dim dDestination As New System.IO.DirectoryInfo(destDirName)
        Try
            RemoveBlocksDir(dDestination)
        Catch ex As Exception
        End Try
        ' If the destination directory doesn't exist, create it. 
        If Not Directory.Exists(destDirName) Then
            Directory.CreateDirectory(destDirName)
        End If
        ' Get the files in the directory and copy them to the new location. 
        Dim files As FileInfo() = dir.GetFiles()
        For Each file In files
            Dim temppath As String = Path.Combine(destDirName, file.Name)
            Try
                file.CopyTo(temppath, False)
            Catch ex As Exception
            End Try
        Next file
        ' If copying subdirectories, copy them and their contents to new location. 
        If copySubDirs Then
            For Each subdir In dirs
                Dim temppath As String = Path.Combine(destDirName, subdir.Name)
                DirectorySnapshot(subdir.FullName, temppath, copySubDirs)
            Next subdir
        End If
    End Sub

    Public Function GRCDataDir() As String
        Dim sFolder As String
        sFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Gridcoin\"
        If bTestNet Then sFolder = sFolder + "testnet\"
        Return sFolder
    End Function
    Private Function DownloadFile(ByVal sFile As String)

        Dim sLocalPath As String = GetGRCAppDir()
        Dim sLocalFile As String = sFile

        If LCase(sLocalFile) = "grcrestarter.exe" Then sLocalFile = "grcrestarter_copy.exe"
        Dim sLocalPathFile As String = sLocalPath + "\" + sLocalFile

        txtStatus.Text = "Upgrading file " + sFile + "..."
        Try
            Kill(sLocalPathFile)
        Catch ex As Exception
        End Try

        Dim sURL As String = GetURL() + sFile
        Dim myWebClient As New MyWebClient()
        myWebClient.DownloadFile(sURL, sLocalPathFile)
        Me.Refresh()
        System.Threading.Thread.Sleep(500)

    End Function
    Public Function GetGRCAppDir() As String
        Try
            Dim fi As New System.IO.FileInfo(Application.ExecutablePath)
            Return fi.DirectoryName
        Catch ex As Exception
        End Try
    End Function
    Public Function KillProcess(ByVal sWildcard As String)
        Try
            For Each p As Process In Process.GetProcesses
                If p.ProcessName Like sWildcard Then
                    p.Kill()
                End If
            Next
        Catch ex As Exception
        End Try
    End Function

    Public Function Base64File(sFileName As String)
        Dim sFilePath As String = GetGRCAppDir() + "\" + sFileName
        Dim b() As Byte
        b = System.IO.File.ReadAllBytes(sFilePath)
        Dim sBase64 As String = System.Convert.ToBase64String(b, 0, b.Length)
        b = System.Text.Encoding.ASCII.GetBytes(sBase64)
        System.IO.File.WriteAllBytes(sFilePath, b)
    End Function
    Public Function UnBase64File(sFileName As String)
        Dim sFilePath As String = GetGRCAppDir() + "\" + sFileName
        Dim b() As Byte
        b = System.IO.File.ReadAllBytes(sFilePath)
        Dim value As String = System.Text.ASCIIEncoding.ASCII.GetString(b)
        b = System.Convert.FromBase64String(value)
        System.IO.File.WriteAllBytes(sFilePath, b)
    End Function

    Public Function NeedsUpgrade() As Boolean
        Try

            Dim sMsg As String
            Dim sURL As String = GetURL()
            Dim w As New MyWebClient
            Dim sFiles As String
            sFiles = w.DownloadString(sURL)
            Dim vFiles() As String = Split(sFiles, "<br>")
            If UBound(vFiles) < 10 Then
                Return False
            End If

            sMsg = ""
            For iRow As Integer = 0 To UBound(vFiles)
                Dim sRow As String = vFiles(iRow)
                Dim sFile As String = ExtractFilename("<a", "</a>", sRow, 5)
                If Len(sFile) > 1 Then
                    If sFile = "boinc.dll" Then
                        Dim sDT As String
                        sDT = Mid(sRow, 1, 20)
                        sDT = Trim(sDT)

                        Dim dDt As DateTime
                        dDt = CDate(Trim(sDT))
                        dDt = TimeZoneInfo.ConvertTime(dDt, System.TimeZoneInfo.Utc)
                        'Hosting server is PST, so subtract Utc - 7 to achieve PST:
                        dDt = DateAdd(DateInterval.Hour, -3, dDt)
                        'local file time
                        Dim sLocalPath As String = GetGRCAppDir()
                        Dim sLocalFile As String = sFile
                        If LCase(sLocalFile) = "grcrestarter.exe" Then sLocalFile = "grcrestarter_copy.exe"
                        Dim sLocalPathFile As String = sLocalPath + "\" + sLocalFile
                        Dim dtLocal As DateTime
                        Try
                            dtLocal = System.IO.File.GetLastWriteTime(sLocalPathFile)

                        Catch ex As Exception
                            Return False


                        End Try
                        If dDt > dtLocal Then
                            Return True
                        End If



                    End If
                End If
            Next iRow
        Catch ex As Exception
            Return False

        End Try

    End Function

    Public Function DynamicUpgradeWithManifest() As String
        Dim sMsg As String
        For iTry As Long = 1 To 10
            Dim sURL As String = GetURL()
            Dim w As New MyWebClient
            Dim sFiles As String
            sFiles = w.DownloadString(sURL)
            Dim vFiles() As String = Split(sFiles, "<br>")
            ProgressBar1.Maximum = vFiles.Length + 1
            If UBound(vFiles) < 10 Then Return "No mirror found, unable to upgrade."
            sMsg = ""
            For iRow As Integer = 0 To UBound(vFiles)
                Dim sRow As String = vFiles(iRow)
                ProgressBar1.Value = iRow
                ProgressBar1.Update() : ProgressBar1.Refresh() : Me.Refresh() : Application.DoEvents()
                Dim sFile As String = ExtractFilename("<a", "</a>", sRow, 5)
                If Len(sFile) > 1 Then
                    txtStatus.Text = "Upgrading " + sFile + "..."
                    txtStatus.Width = Me.Width
                    txtStatus.Refresh()
                    txtStatus.Update()
                    Application.DoEvents()

                    Try
                        DownloadFile(sFile)
                    Catch ex As Exception
                        sMsg = sMsg + ex.Message + ".    "
                    End Try
                End If
            Next iRow
            If sMsg = "" Then Exit For
        Next iTry
        Return sMsg
    End Function

    Public Function ExtractFilename(ByVal sStartElement As String, ByVal sEndElement As String, ByVal sData As String, ByVal minOutLength As Integer) As String
        Try
            Dim sDataBackup As String
            sDataBackup = LCase(sData)
            Dim iStart As Integer
            Dim iEnd As Long
            Dim sOut As String
            iStart = InStr(1, sDataBackup, sStartElement) + Len(sStartElement) + 1
            iEnd = InStr(iStart + minOutLength, sDataBackup, sEndElement)
            sOut = Mid(sData, iStart, iEnd - iStart)
            sOut = Replace(sOut, ",", "")
            sOut = Replace(sOut, "br/>", "")
            sOut = Replace(sOut, "</a>", "")
            Dim iPrefix As Long
            iPrefix = InStr(1, sOut, ">")
            Dim sPrefix As String
            sPrefix = Mid(sOut, 1, iPrefix)
            sOut = Replace(sOut, sPrefix, "")
            Dim sExt As String
            sExt = LCase(Mid(sOut, Len(sOut) - 2, 3))
            sOut = LCase(sOut)
            If sExt = "pdf" Or LCase(sOut).Contains("to parent directory") Or sExt = "msi" Or sExt = "pdb" Or sExt = "xml" Or LCase(sOut).Contains("vshost") Or sExt = "txt" Or sOut = "gridcoin" Or sOut = "gridcoin_ro" Or sOut = "older" Or sExt = "cpp" Or sOut = "web.config" Then sOut = ""
            If sOut = "gridcoin.zip" Then sOut = ""
            If sOut = "gridcoinrdtestharness.exe.exe" Or sOut = "gridcoinrdtestharness.exe" Then sOut = ""
            If sOut = "cgminer_base64.zip" Then sOut = ""
            If sOut = "signed" Then sOut = ""

            Return Trim(sOut)
        Catch ex As Exception
            Dim message As String = ex.Message


        End Try
    End Function
    Public Function UpgradeGrcRestarter(sParams As String)
        Dim p As Process = New Process()
        Dim pi As ProcessStartInfo = New ProcessStartInfo()
        pi.WorkingDirectory = GetGRCAppDir()
        pi.UseShellExecute = True
        pi.Arguments = sParams
        pi.FileName = Trim("GRCRestarter_copy.exe")
        p.StartInfo = pi
        p.Start()
    End Function




End Class

Public Class MyWebClient
    Inherits System.Net.WebClient
    Private timeout As Long = 125000

    Protected Overrides Function GetWebRequest(ByVal uri As Uri) As System.Net.WebRequest
        Dim w As System.Net.WebRequest = MyBase.GetWebRequest(uri)
        w.Timeout = timeout

        Return (w)
    End Function
End Class
