Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.IO
Imports System.Reflection

Module modGRC
    Public mfrmMining As frmMining
    Public clsGVM As GridcoinVirtualMachine.GVM
    Public mfrmProjects As frmProjects
    Public mfrmSql As frmSQL
    Public mfrmGridcoinMiner As frmGridcoinMiner
    Public mfrmLeaderboard As frmLeaderboard
    Structure xGridcoinMiningStructure
        Public Shared device As String = "0"
        Public Shared gpu_thread_concurrency As String = "8192"
        Public Shared worksize As String = "256"
        Public Shared intensity As String = "13"
        Public Shared lookup_gap As String = "2"
    End Structure
    Public Function xReturnMiningValue(iDeviceId As Integer, sKey As String, bUsePrefix As Boolean) As String
        Dim g As New xGridcoinMiningStructure
        Dim sOut As String = ""
        Dim sLookupKey As String = LCase("dev" + Trim(iDeviceId) + "_" + sKey)
        If Not bUsePrefix Then sLookupKey = LCase(sKey)
        sOut = KeyValue(sLookupKey)
        Return sOut
    End Function
    Public Function GetGRCAppDir() As String
        Try
            Dim fi As New System.IO.FileInfo(Assembly.GetExecutingAssembly().Location)
            Return fi.DirectoryName
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
    Public Function UnBase64File(sSourceFileName As String, sTargetFileName As String)
        Dim b() As Byte
        b = System.IO.File.ReadAllBytes(sSourceFileName)
        Dim value As String = System.Text.ASCIIEncoding.ASCII.GetString(b)
        b = System.Convert.FromBase64String(value)
        System.IO.File.WriteAllBytes(sTargetFileName, b)
        System.Threading.Thread.Sleep(1000)

    End Function

    Public Function RestartWallet1(sParams As String)
        Dim p As Process = New Process()
        Dim pi As ProcessStartInfo = New ProcessStartInfo()
        pi.WorkingDirectory = GetGRCAppDir()
        pi.UseShellExecute = True
        pi.Arguments = sParams
        pi.FileName = Trim("GRCRestarter.exe")
        p.StartInfo = pi
        p.Start()
    End Function
    Public Function ConfigPath() As String
        Dim sFolder As String
        sFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Gridcoin"
        Dim sPath As String
        sPath = sFolder + "\gridcoin.conf"
        Return sPath
    End Function
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
    Public Function GetGridFolder() As String
        Dim sTemp As String
        sTemp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Gridcoin\"
        Return sTemp
    End Function
    Public Function KeyValue(ByVal sKey As String) As String
        Try
            Dim sPath As String = ConfigPath()
            Dim sr As New StreamReader(sPath)
            Dim sRow As String
            Dim vRow() As String
            Do While sr.EndOfStream = False
                sRow = sr.ReadLine
                vRow = Split(sRow, "=")
                If LCase(vRow(0)) = LCase(sKey) Then
                    sr.Close()
                    Return Trim(vRow(1) & "")
                End If
            Loop
            sr.Close()
        Catch ex As Exception
            Return ""
        End Try
    End Function
    Public Function UpdateKey(ByVal sKey As String, ByVal sValue As String)
        Try
            Dim sInPath As String = ConfigPath()
            Dim sOutPath As String = ConfigPath() + ".bak"

            Dim bFound As Boolean

            Dim sr As New StreamReader(sInPath)
            Dim sw As New StreamWriter(sOutPath, False)

            Dim sRow As String
            Dim vRow() As String
            Do While sr.EndOfStream = False
                sRow = sr.ReadLine
                vRow = Split(sRow, "=")
                If UBound(vRow) > 0 Then
                    If LCase(vRow(0)) = LCase(sKey) Then
                        sw.WriteLine(sKey + "=" + sValue)
                        bFound = True
                    Else
                        sw.WriteLine(sRow)
                    End If
                End If

            Loop
            If bFound = False Then
                sw.WriteLine(sKey + "=" + sValue)
            End If
            sr.Close()
            sw.Close()
            Kill(sInPath)
            FileCopy(sOutPath, sInPath)
            Kill(sOutPath)
        Catch ex As Exception
            Return ""
        End Try
    End Function
    Public Function cBOO(data As Object) As Boolean
        Dim bOut As Boolean
        Try
            Dim sBoo As String = data.ToString
            bOut = CBool(sBoo)
        Catch ex As Exception
            Return False
        End Try
        Return bOut
    End Function
    Public Sub KillGuiMiner()
        Log("Closing all miners (guiminer,cgminer,conhost,reaper).")

        For x = 1 To 6
            Try
                KillProcess("guiminer*")
                KillProcess("cgminer*")
                KillProcess("conhost*")
                KillProcess("reaper*")
                mCGMinerHwnd(x - 1) = 0
            Catch ex As Exception
            End Try
        Next x

    End Sub

    Public Sub Snapshot()
        Dim sDataDir As String = GRCDataDir()
        Dim sBlocks = sDataDir + "blocks"
        Dim sChain = sDataDir + "chainstate"
        '   Dim sDatabase = sDataDir + "database"
        Dim sSnapshotBlocks = sDataDir + "snapshot\blocks"
        Dim sSnapshotChain = sDataDir + "snapshot\chainstate"
        Dim sSnapshotDatabase = sDataDir + "snapshot\database"
        Dim sSnapshotDir = sDataDir + "snapshot"

        Try
            Dim dDestination2 As New System.IO.DirectoryInfo(sSnapshotDir)
            dDestination2.Delete()

        Catch ex As Exception

        End Try
       
        DirectorySnapshot(sBlocks, sSnapshotBlocks, True)
        DirectorySnapshot(sChain, sSnapshotChain, True)

        'DirectorySnapshot(sDatabase, sSnapshotDatabase, True)
    End Sub
    Public Sub RestoreFromSnapshot()
        Dim sDataDir As String = GRCDataDir()
        Dim sBlocks = sDataDir + "blocks"
        Dim sChain = sDataDir + "chainstate"
        Dim sSnapshotBlocks = sDataDir + "snapshot\blocks"
        Dim sSnapshotChain = sDataDir + "snapshot\chainstate"
        DirectorySnapshot(sSnapshotBlocks, sBlocks, True)
        DirectorySnapshot(sSnapshotChain, sChain, True)
    End Sub


    Private Sub DirectorySnapshot( _
        ByVal sourceDirName As String, _
        ByVal destDirName As String, _
        ByVal copySubDirs As Boolean)

        ' Get the subdirectories for the specified directory. 
        Dim dir As DirectoryInfo = New DirectoryInfo(sourceDirName)
        Dim dirs As DirectoryInfo() = dir.GetDirectories()

        If Not dir.Exists Then
            'Throw New DirectoryNotFoundException( _               "Source directory does not exist or could not be found: " _        + sourceDirName)

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
                Log("Error while copying " + file.Name + " to " + temppath)

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


    Private Sub RemoveBlocksDir(d As System.IO.DirectoryInfo)
        Try
            d.Delete(True)
        Catch ex As Exception
        End Try
    End Sub


    Public Function GRCDataDir() As String
        Dim sFolder As String
        sFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Gridcoin\"
        Return sFolder
    End Function
    Public Function GetBlockExplorerBlockNumber() As Long
        Try
            Dim w As New MyWebClient
            Dim sWap As String
            Try
                sWap = w.DownloadString("http://explorer.gridcoin.us")
                'Blocks:</TD><TD> 35671</TD></TR>
            Catch ex As Exception
                Return -3
            End Try
            Dim iStart As Integer
            iStart = InStr(1, sWap, "Blocks:</TD>") + 12
            Dim iEnd As Integer
            iEnd = InStr(iStart, sWap, "</TD>")
            Dim sBlocks As String
            sBlocks = Mid(sWap, iStart, iEnd - iStart)
            sBlocks = Replace(sBlocks, "</TD>", "")
            sBlocks = Replace(sBlocks, "</TR>", "")
            sBlocks = Replace(sBlocks, "<TD>", "")
            sBlocks = Trim(sBlocks)
            Dim lBlock As Long
            lBlock = Val(sBlocks)
            Return lBlock
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Public Sub Log(sData As String)
        Try
            Dim sPath As String
            sPath = GetGridFolder() + "debug2.log"
            Dim sw As New System.IO.StreamWriter(sPath, True)
            sw.WriteLine(Trim(Now) + ", " + sData)
            sw.Close()
        Catch ex As Exception
        End Try

    End Sub



    Public Function LoadNodes(data As String)
        Exit Function '
        'Fix:1/5/2014 8:01:18 AM, getnewid:Argument 'Expression' cannot be converted to type 'DBNull'.

        Dim mData As New Sql

        Dim vData() As String
        vData = Split(data, vbCrLf)
        Dim sRow As String
        Dim vRow() As String
        Dim sNode As String
        Dim sVersion As String
        Dim sql As String
        Dim lNewId As Long

        Try

            For x = 0 To UBound(vData)
                sRow = vData(x)
                vRow = Split(sRow, "|")
                If UBound(vRow) >= 1 Then
                    sNode = vRow(0)
                    sVersion = vRow(1)
                    lNewId = GetNewId("peers", mData)

                    sql = "Insert into peers values ('" + Trim(lNewId) + "','" + Trim(sNode) + "','" + Trim(sVersion) + "',date('now'))"
                    Log(sql)

                    mData.Exec(sql)
                End If

            Next x

            Exit Function

        Catch ex As Exception
            Log("LoadNodes:" + ex.Message)

        End Try

    End Function
    Public Function GetNewId(sTable As String, mData As Sql) As Long
        Try
            Dim sql As String
            sql = "Select max(id) as maxid from " + sTable
        Dim vID As Long
        vID = Val(mData.QueryFirstRow(sql, "maxid")) + 1
        Return vID
        Catch ex As Exception
            Log("getnewid:" + ex.Message)
        End Try
    End Function
End Module

Public Class MyWebClient
    Inherits System.Net.WebClient
    Protected Overrides Function GetWebRequest(ByVal uri As Uri) As System.Net.WebRequest
        Dim w As System.Net.WebRequest = MyBase.GetWebRequest(uri)
        w.Timeout = 7000
        Return w
    End Function
End Class


