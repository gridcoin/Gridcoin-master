Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.IO
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Object
Imports System.Security.Cryptography
Imports System.Net.WebClient

Module modBoincCredits
    Public BoincProjects As Double
    Public BoincCredits As Long
    Public BoincDelta As Double
    Public BoincCreditsAvg As Long
    Public BoincDeltaAvg As Double
    Public BoincTotalHostAvg As Double
    Public BoincProjectData As String
    Public TeamGridcoinProjects(70) As String
    Public BoincCreditsAvgAtPointInTime As Double
    Public BoincCreditsAtPointInTime As Double


    Public Function LogBoincCredits()

        Try

        Dim sXMLFile As String
        sXMLFile = GetBoincDataFolder() + Des3DecryptData("suFvLEJcMMaHFzkGNF8eLUaDlUwW9L2G+7MJt4sQbXydsSkihyQLdw==")
        If System.IO.File.Exists(sXMLFile + Des3DecryptData("JnYZen3LlTgblZVdIunYtw==")) Then Kill(sXMLFile + Des3DecryptData("JnYZen3LlTgblZVdIunYtw=="))
        Call modAvgOverTime()
        FileCopy(sXMLFile, sXMLFile + Des3DecryptData("JnYZen3LlTgblZVdIunYtw=="))
        'For each boinc project, retrieve the host_total_credit
        Dim io As New System.IO.StreamReader(sXMLFile + Des3DecryptData("JnYZen3LlTgblZVdIunYtw=="))
        Dim sTemp As String
        Dim sProject As String
        Dim dCredit As Double
        Dim totalCredit As Double
        Dim dTotalProjects As Double
        Dim dTotalHostAvg As Double
        Dim sProjects As String = ""
            Dim sHostId As String = ""
            Dim sSmallProj As String = ""
            Dim sSmallProjExpanded As String = ""
            Dim sProjectsExpanded As String = ""
            Do While Not io.EndOfStream

                sTemp = io.ReadLine
                If sTemp.Contains("<project_name>") Then
                    sProject = XMLValue(sTemp)
                    sSmallProj = Left(sProject, 5)

                    If Not sProjects.Contains(sSmallProj) Then
                        sProjects = sProjects + sSmallProj + ":"
                    End If
                End If
                If sTemp.Contains("host_total_credit") Then
                    dCredit = Val(XMLValue(sTemp))
                    dTotalProjects = dTotalProjects + 1
                    totalCredit = totalCredit + dCredit
                    AddCredits(sProject, dCredit, 0, "host_total_credit")
                End If
                If sTemp.Contains("hostid") Then
                    sHostId = XMLValue(sTemp)

                End If
                If sTemp.Contains("host_expavg_credit") Then
                    dCredit = Val(XMLValue(sTemp))
                    dTotalHostAvg = dTotalHostAvg + dCredit
                    AddCredits(sProject, dCredit, 0, "host_expavg_credit")
                    sSmallProjExpanded = sSmallProj + "_" + Trim(Val(dCredit)) + "_" + Trim(sHostId)

                    If Not sProjectsExpanded.Contains(sProjectsExpanded) Then
                        sProjectsExpanded = sProjectsExpanded + sSmallProjExpanded + ":"
                    End If
                End If
            Loop
        io.Close()
            If Len(sProjects) > 1 Then sProjects = Left(sProjects, Len(sProjects) - 1)
            If Len(sProjectsExpanded) > 1 Then sProjectsExpanded = Left(sProjectsExpanded, Len(sProjectsExpanded) - 1)

            BoincProjectData = sProjectsExpanded
        AddCredits("TOTAL", totalCredit, dTotalProjects, "TOTAL")
        AddCredits("AVG", dTotalHostAvg, dTotalProjects, "AVG")
            BoincTotalHostAvg = dTotalHostAvg
        Catch ex As Exception

        End Try

    End Function

    Public Function ReturnBoincCreditsAtPointInTime(ByVal lLookbackSecs) As Double
        Try

            Dim dtEnd As Date = Now
            Dim dtStart As Date = DateAdd(DateInterval.Second, -lLookbackSecs, dtEnd)
            Dim sPath As String
            sPath = GetBoincDataFolder()
            sPath = sPath + Des3DecryptData("JhY0OC9WiRedUiptEb+eofCHaYrQ5GwjmLec5apcwEs=")
            Dim oSR As New StreamReader(sPath)
            Dim sTemp As String
            Dim dTotalCreditsStart As Double
            Dim dTotalAvgStart As Double
            Dim dProjects As Double
            Dim dtEntry As Date
            While Not oSR.EndOfStream
                sTemp = FromBase64(oSR.ReadLine)
                Dim vTemp() As String
                vTemp = Split(sTemp, ",")
                If UBound(vTemp) > 3 Then
                    If vTemp(1) = "TOTAL" Then
                        dtEntry = vTemp(0)
                        If dtEntry > dtStart And dTotalCreditsStart = 0 Then
                            dTotalCreditsStart = vTemp(2)
                            dProjects = vTemp(3)
                        End If
                    End If

                    If vTemp(1) = "AVG" Then
                        dtEntry = vTemp(0)
                        If dtEntry > dtStart And dTotalAvgStart = 0 Then
                            dTotalAvgStart = vTemp(2)
                            Exit While
                        End If
                    End If
                End If

            End While
            oSR.Close()
            BoincProjects = dProjects
            If BoincProjects < 0 Then BoincProjects = 0
            BoincCreditsAvgAtPointInTime = dTotalAvgStart
            BoincCreditsAtPointInTime = dTotalCreditsStart
            Return dTotalAvgStart
        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function Housecleaning() As Double


        Dim r As Long = Rnd(1) * 100 : If r < 80 Then Exit Function


        Try

            Dim dtEnd As Date = Now
            Dim dtStart As Date = DateAdd(DateInterval.Day, -33, dtEnd)

            Dim sPath As String
            sPath = GetBoincDataFolder()
            sPath = sPath + Des3DecryptData("JhY0OC9WiRedUiptEb+eofCHaYrQ5GwjmLec5apcwEs=")
            If Not File.Exists(sPath) Then Exit Function

            Dim oSR As New StreamReader(sPath)
            Dim sTemp As String
            Dim dtEntry As Date
            Dim sOutPath As String
            sOutPath = GetBoincDataFolder() + Des3DecryptData("JhY0OC9WiRedUiptEb+eobeUZSEvSMGIg/l0J3rb1mOs0wOFFNE+nQ==")
            Dim iRow As Long

            Dim oSW As New StreamWriter(sOutPath, False)

            While Not oSR.EndOfStream
                sTemp = FromBase64(oSR.ReadLine)
                Dim vTemp() As String
                vTemp = Split(sTemp, ",")
                iRow = iRow + 1

                If UBound(vTemp) > 3 Then
                    If IsDate(vTemp(0)) Then
                        dtEntry = vTemp(0)
                        If dtEntry > dtStart Then
                            oSW.WriteLine(ToBase64(sTemp))
                        End If

                    End If
                End If

            End While
            oSR.Close()
            oSW.Close()

            Kill(sPath)
            FileCopy(sOutPath, sPath)
            Kill(sOutPath)

        Catch ex As Exception
            '  MsgBox("HouseCleaning Failed!", MsgBoxStyle.Critical)
        End Try

    End Function
    Private Function AddCredits(ByVal sName As String, ByVal dCredit As Double, ByVal dProjectCount As Double, ByVal sCounterType As String)
        Try

        Dim sPath As String
        sPath = GetBoincDataFolder()

        sPath = sPath + Des3DecryptData("JhY0OC9WiRedUiptEb+eofCHaYrQ5GwjmLec5apcwEs=")

        Dim oSR As New StreamWriter(sPath, True)
        Dim sRow As String
        sRow = Trim(Now) + "," + sName + "," + Trim(dCredit) + "," + Trim(dProjectCount) + "," + sCounterType
        sRow = ToBase64(sRow)
        oSR.WriteLine(sRow)
            oSR.Close()
        Catch ex As Exception

        End Try

    End Function
    Private Function XMLValue(ByVal sRow As String) As String
        Try

        Dim iEnd As Long
        Dim iStart As Long
        iStart = InStr(1, sRow, Chr(62))
        If iStart = 0 Then Exit Function
        iEnd = InStr(iStart, sRow, Chr(60) + Chr(47), CompareMethod.Text)

        If iEnd = 0 Then Exit Function
        Dim sValue As String

        sValue = Mid(sRow, iStart + 1, iEnd - iStart - 1)
            Return sValue
        Catch ex As Exception

        End Try

    End Function
    Public Function GetBoincProgFolder() As String
        Dim sAppDir As String
        sAppDir = KeyValue("boincappfolder")
        If Len(sAppDir) > 0 Then Return sAppDir
        Dim bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 As String
        bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)

        bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 = Trim(Replace(bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9, "(x86)", ""))
        bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 = bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 + "\Boinc\"
        If Not System.IO.Directory.Exists(bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9) Then
            bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + Des3DecryptData("sEl7B/roaQaNGPo+ckyQBA==")
        End If
        Return bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9
    End Function

    Public Function GetBoincDataFolder() As String
        Dim sAppDir As String
        sAppDir = KeyValue("boincdatafolder")
        If Len(sAppDir) > 0 Then Return sAppDir
        Dim bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 As String
        bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 = bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 + "\Boinc\"
        If Not System.IO.Directory.Exists(bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9) Then
            bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + Des3DecryptData("sEl7B/roaQaNGPo+ckyQBA==")

        End If
        Return bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9
    End Function

    Private Function CalculateApiURL(ByVal lProjectId As Long, ByVal sBoincUserId As String) As String
        Try

        Dim vAPI() As String
        vAPI = Split(TeamGridcoinProjects(lProjectId), ",")
        Dim sWap As String
        sWap = vAPI(1) + "/userw.php" + "?" + "id=" + sBoincUserId
            Return sWap
        Catch ex As Exception

        End Try

    End Function
    Public Function ExtractCreditsByProject(ByVal lProjectId As Long, ByVal lUserId As Long, ByVal sGRCAddress As String, ByRef sOutStruct As String) As Double
        'ExtractCreditsByProject
        '-1 Wallet address does not match API address
        '-2 General Exception
        '-3 Cannot connect to Berkeley API
        '-11 Bad or missing ProjectId
        '-12 Invalid UserId
        'Positive number = User Project Avg Daily Credits

        If lProjectId < 1 Or lProjectId > 70 Or lUserId = 0 Then
            Return -11
        End If
        If Len(Trim(lUserId)) < 3 Or Len(Trim(lUserId)) > 7 Then
            Return -12
        End If

        Try
            Dim sURL As String
            sURL = CalculateApiURL(lProjectId, Trim(lUserId))
            Dim w As New MyWebClient
            Dim sWap As String
            Try
                sWap = w.DownloadString(sURL)
            Catch ex As Exception
                Return -3
            End Try
            'Extract credit information from Berkeley API using XML-WAP form Rob Halford emailed to Aysyr
            Dim sWallet As String
            Dim sAvgCred As String
            Dim sTeam As String
            sWallet = ExtractXML("account data", "<br/>", sWap, 5)
            If LCase(Trim(sWallet)) <> LCase(Trim(sGRCAddress)) Then
                Return -1
            End If
            sAvgCred = ExtractXML("user avgcred", "<br/>", sWap, 3)
            sTeam = ExtractXML("team:", "<br/>", sWap, 2)
            Dim sConcat As String
            sConcat = sWallet + "," + sAvgCred + "," + sTeam
            sOutStruct = sConcat
            Return Val(sAvgCred) + 0.001
        Catch ex As Exception
            Return -2
        End Try
    End Function
    Public Function ExtractXML(ByVal sStartElement As String, ByVal sEndElement As String, ByVal sData As String, ByVal minOutLength As Integer) As String
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
            sOut = Replace(sOut, "for", "")
            Return Trim(sOut)
        Catch ex As Exception

        End Try

    End Function

    Public Function BoincCreditsByProject(ByVal lProjectId As Long, ByVal lUserId As Long) As Double
        Dim dCredits As Double
        Dim sOutStruct As String

        Try
            dCredits = ExtractCreditsByProject(lProjectId, lUserId, PublicWalletAddress, sOutStruct)
            Return dCredits

        Catch ex As Exception

        End Try

    End Function
End Module

Public Class MyWebClient
    Inherits System.Net.WebClient
    Protected Overrides Function GetWebRequest(ByVal uri As Uri) As System.Net.WebRequest
        Dim w As System.Net.WebRequest = MyBase.GetWebRequest(uri)
        w.Timeout = 5000
        Return w
    End Function
End Class
