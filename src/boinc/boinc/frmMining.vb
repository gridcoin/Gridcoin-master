Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Diagnostics
Imports System.Timers
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Threading
Imports boinc

Public Class frmMining
    Private MaxHR As Double = 1
    Private LastMHRate As String = ""
    Private lMHRateCounter As Long = 0
    Private mIDelay As Long = 0

    Private clsUtilization As Utilization
    Private RefreshCount As Long
    Private RestartedMinerAt As DateTime
    Private RestartedWalletAt As DateTime
    Private bUICharted As Boolean = False
    Public bDisposing As Boolean
    Public bSuccessfullyLoaded As Boolean
    Private bCharting As Boolean
    Private mMh(10) As Double
    Private mShares(10) As Double
    Private mStales(10) As Double
    Private mInvalids(10) As Double
    Private mProcess(10) As Process
    Private mProcessInfo(10) As ProcessStartInfo
    Private mThreadTimer(10) As System.Timers.Timer
    Private mRTB(10) As RichTextBox
    Private mLineCount(10) As Integer
    Private mEnabled(10) As Boolean
    Private msReaperOut(10) As String
    Private miInitCounter As Long
    Private msLastBlockHash As String
    Private mlElapsedTime As Long
    Private msLastSleepStatus As String

    Public Sub SetClsUtilization(c As Utilization)
        clsUtilization = c
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        Refresh2(True)
    End Sub
    Private Sub UpdateCharts()
        Try
            ChartBoinc()
            UpdateChartHashRate()
            ChartBoincUtilization()

            Me.Update()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub OneMinuteUpdate()
        Try

             ChartBoincUtilization()

            
            Dim lMinSetting As Long
            Dim lRunning As Long

            If False Then
                lMinSetting = Val(KeyValue("RestartMiner"))
                lRunning = Math.Abs(DateDiff(DateInterval.Minute, RestartedMinerAt, Now))
                If lMinSetting = 0 Then
                    lblRestartMiner.Text = "Never"
                Else
                    Dim dCountdown As Double
                    dCountdown = lMinSetting - lRunning
                    lblRestartMiner.Text = Trim(dCountdown)
                    If dCountdown <= 0 Then
                        Refresh2(True)
                    End If
                End If
            End If


            If False Then
                lMinSetting = Val(KeyValue("RestartWallet"))
                lRunning = Math.Abs(DateDiff(DateInterval.Minute, RestartedWalletAt, Now))
                If lMinSetting = 0 Then
                    lblRestartWallet.Text = "Never"
                Else
                    Dim dCountdown As Double
                    dCountdown = lMinSetting - lRunning
                    lblRestartWallet.Text = Trim(dCountdown)
                    If dCountdown <= 0 Then
                        RestartedWalletAt = Now
                        RestartWallet()
                    End If
                End If
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch exx As Exception
            Log("One minute update:" + exx.Message)

        End Try


    End Sub
    Public Function CalculateScryptSleep() As Double
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
            bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + mclsUtilization.Des3Decrypt("sEl7B/roaQaNGPo+ckyQBA==")
        End If
        Return bigtime3f7o6l0daedrf4597acff2affbb5ed209f439aFroBearden0edd44ae1167a1e9be6eeb5cc2acd9c9
    End Function

    Public Function ForceBoincToUseGPUs(sSleepDirective As String)
        Try

        Dim sFolder As String = GetBoincProgFolder()
        Dim sPath As String = sFolder & "boinccmd.exe"
        Dim sBinary As String
            If sSleepDirective = "sleep" Then sBinary = "1" Else sBinary = "0"
            Dim sOverFn As String = GetBoincDataFolder() + "global_prefs_override.xml"
            Dim sr As New StreamReader(sOverFn)
        Dim sOut As String
            Dim sXML As String
        While sr.EndOfStream = False
            sXML = sr.ReadLine()
            If InStr(1, sXML, "<run_gpu_if_user_active>") > 0 Then
                sXML = "<run_gpu_if_user_active>" + Trim(sBinary) + "</run_gpu_if_user_active>"

            End If
            sOut += sXML + vbCrLf
        End While
        sr.Close()
        Dim sw As New StreamWriter(sOverFn)
        sw.Write(sOut)
        sw.Close()

        Try
            Dim p As Process = New Process()
            Dim pi As ProcessStartInfo = New ProcessStartInfo()
            pi.WorkingDirectory = GetBoincProgFolder()
                pi.UseShellExecute = True
                pi.WindowStyle = ProcessWindowStyle.Hidden

            pi.FileName = "boinccmd.exe"
            pi.Arguments = "--read_global_prefs_override"

            p.StartInfo = pi
            p.Start()
        Catch ex As Exception
            Log("Error updating boinc overrides" + ex.Message)
        End Try

        Catch ex As Exception
            Log("Error updating boinc overrides(b):" + ex.Message)

        End Try

    End Function
    Public Sub Refresh2(ByVal bStatsOnly As Boolean)
        bCharting = False
        updateGh()

        Try
            If Not bStatsOnly Then
                UpdateCharts()
            Else
                ChartBoincUtilization()
                UpdateChartHashRate()
            End If
        Catch ex As Exception
        End Try
    End Sub
   
    Public Sub RestartWallet()

        Try
            Dim p As Process = New Process()
            Dim pi As ProcessStartInfo = New ProcessStartInfo()
            pi.WorkingDirectory = GetGRCAppDir()
            pi.UseShellExecute = True
            pi.FileName = "GRCRestarter.exe"
            pi.Arguments = "reindex"
            p.StartInfo = pi
            p.Start()
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ChartBoinc()
        Dim last_total As Double
        Dim seriesAvgCredits As Series : Dim seriesTotalCredits As Series : Dim seriesProjects As Series


        Try
            If bCharting Then Exit Sub
            bCharting = True
            If Chart1.Titles.Count < 1 Then
                Chart1.Series.Clear()

                Chart1.Titles.Clear()
                Chart1.Titles.Add("Boinc Utilization")
                Chart1.BackColor = Color.Transparent : Chart1.ForeColor = Color.Lime
                Chart1.ChartAreas(0).AxisX.IntervalType = DateTimeIntervalType.Weeks : Chart1.ChartAreas(0).AxisX.TitleForeColor = Color.White
                Chart1.ChartAreas(0).BackSecondaryColor = Color.Transparent : Chart1.ChartAreas(0).AxisX.LabelStyle.ForeColor = Color.Lime
                Chart1.ChartAreas(0).AxisY.LabelStyle.ForeColor = Color.Lime : Chart1.ChartAreas(0).ShadowColor = Color.Chocolate
                Chart1.ChartAreas(0).BackSecondaryColor = Color.Gray : Chart1.ChartAreas(0).BorderColor = Color.Gray
                Chart1.Legends(0).ForeColor = Color.Lime
                Chart1.ChartAreas(0).AxisX.LabelStyle.Format = "MM-dd-yyyy"
                seriesAvgCredits = New Series

                seriesAvgCredits.Name = "Avg Daily Credits" : seriesAvgCredits.ChartType = SeriesChartType.Line
                seriesAvgCredits.LabelForeColor = Color.Lime
                seriesTotalCredits = New Series
                seriesTotalCredits.ChartType = SeriesChartType.FastLine
                seriesTotalCredits.Name = "Total Daily Credits"
                seriesProjects = New Series
                seriesProjects.Name = "Projects" : Chart1.ChartAreas(0).AxisX.Interval = 1 : seriesProjects.ChartType = SeriesChartType.StepLine
                Chart1.Series.Add(seriesTotalCredits)
                Chart1.Series.Add(seriesAvgCredits)
                Chart1.Series.Add(seriesProjects)

            End If

            seriesAvgCredits.Points.Clear()
            seriesTotalCredits.Points.Clear()
            seriesProjects.Points.Clear()

        
            Dim dProj As Double
            Dim lookback As Double
            For x = 30 To 0.5 Step -6
                lookback = x * 3600 * 24
                clsGVM.ReturnBoincCreditsAtPointInTime(lookback)
                Dim l1 As Double
                Dim l2 As Double
                Dim l3 As Double
                l1 = clsGVM.BoincCreditsAvgAtPointInTime
                clsGVM.ReturnBoincCreditsAtPointInTime(lookback - (3600 * 24))
                l2 = clsGVM.BoincCreditsAtPointInTime
                l3 = Math.Abs(l2 - last_total)
                last_total = l2
                If l3 > (l1 * 5) Then l3 = l1
                Application.DoEvents()
                Dim pCreditsAvg As New DataPoint
                dProj = clsGVM.BoincProjects
                Dim d1 As Date = DateAdd(DateInterval.Day, -x, Now)
                pCreditsAvg.SetValueXY(d1, l1)
                seriesAvgCredits.Points.Add(pCreditsAvg)
                Dim dpProj As New DataPoint()
                dpProj.SetValueXY(d1, dProj * (l1 / 10))
                seriesProjects.Points.Add(dpProj)
                Dim pCreditsTotal As New DataPoint()
                pCreditsTotal.SetValueXY(d1, l3)
                seriesTotalCredits.Points.Add(pCreditsTotal)
            Next
            'Chart1.Refresh()
        Catch ex As Exception
        End Try
        bCharting = False
    End Sub
    Public Sub ChartBoincUtilization()
        Try

            If ChartUtilization.Titles.Count < 1 Then

                ChartUtilization.Series.Clear()
                ChartUtilization.Titles.Clear()
                ChartUtilization.Visible = True

                ChartUtilization.BackColor = Color.Transparent : ChartUtilization.ForeColor = Color.Blue
                ChartUtilization.Titles.Add("Boinc Magnitude")
                ChartUtilization.ChartAreas(0).BackColor = Color.Transparent
                ChartUtilization.ChartAreas(0).BackSecondaryColor = Color.White
                ChartUtilization.Legends(0).BackColor = Color.Transparent
                ChartUtilization.Legends(0).ForeColor = Color.Honeydew
                Dim sUtilization As New Series
                sUtilization.Name = "Magnitude" : sUtilization.ChartType = SeriesChartType.Pie
                sUtilization.LegendText = "Boinc Magnitude"
                sUtilization.LabelBackColor = Color.Lime : sUtilization.IsValueShownAsLabel = False
                sUtilization.LabelForeColor = Color.Honeydew
                ChartUtilization.Series.Add(sUtilization)
            End If

            Dim bu As Double
            bu = Math.Round(Val(clsUtilization._boincmagnitude), 1)
            ChartUtilization.Series(0).Points.Clear()

            If Not bUICharted Then bUICharted = True : bu = 2
            ChartUtilization.Series(0).Points.AddY(bu)
            ChartUtilization.Series(0).LabelBackColor = Color.Transparent
            ChartUtilization.Series(0).Points(0).Label = Trim(bu)
            ChartUtilization.Series(0).Points(0).Color = Color.Blue
            ChartUtilization.Series(0).Points(0).LegendToolTip = Trim(bu) + " magnitude"

            ChartUtilization.Series(0).Points.AddY(100 - bu)
            ChartUtilization.Series(0).Points(1).IsVisibleInLegend = False
            ChartUtilization.Series(0)("PointWidth") = "0.5"
            ChartUtilization.Series(0).IsValueShownAsLabel = False
            ChartUtilization.Series(0)("BarLabelStyle") = "Center"
            ChartUtilization.ChartAreas(0).Area3DStyle.Enable3D = True
            ChartUtilization.Series(0)("DrawingStyle") = "Cylinder"
        Catch ex As Exception



        End Try
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tOneMinute.Tick

        Call Refresh2(False)
        Call OneMinuteUpdate()

    End Sub

    Private Sub btnRestartMiner_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRestartMiner.Click
        Try
            ReStartMiners()

        Catch ex As Exception
        End Try
    End Sub
    Private Sub RefreshRestartMinutes()
    End Sub

    Private Sub frmMining_Activated(sender As Object, e As System.EventArgs) Handles Me.Activated
      
    End Sub
    Private Sub frmMining_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If bDisposing Then
            KillGuiMiner()

            Me.Close()
            Me.Dispose()
            Exit Sub
        End If
        Me.Hide()
        e.Cancel = True
    End Sub
    Private Sub btnRestart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RestartWallet()
    End Sub

    Private Sub btnCloseWallet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            KillGuiMiner()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        KillGuiMiner()
    End Sub

    Private Sub btnHide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHide.Click
        Me.Hide()
    End Sub

    Private Sub btnmining_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        clsGVM.LastBlockHash = Trim(Now)
        Dim sNarr As String
        sNarr = clsGVM.SourceBlock + "  OUTPUTS : " + clsGVM.MinedHash
        MsgBox(sNarr)
    End Sub

    Private Sub timerBoincBlock_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerBoincBlock.Tick
        '   lblCPUMinerElapsed.Text = Trim(Math.Round(clsGVM.CPUMiner.KHPS, 0))
        '        lblLastBlockHash.Text = Mid(clsGVM.LastBlockHash, 1, 43) + "..."
        mlElapsedTime = mlElapsedTime + 1

      
        '       msLastBlockHash = lblLastBlockHash.Text

    End Sub

    Public Sub ReStartGuiMiner_Old()

        Try

            KillGuiMiner()
            Threading.Thread.Sleep(200)
            Application.DoEvents()
            Pb1.Refresh()
            RestartedMinerAt = Now
            Me.Visible = True
        Catch ex As Exception
        End Try


        Try
            Dim p As Process
            Dim hwnd As IntPtr
            Dim sCap As String
            sCap = "GUIMiner-scrypt alpha"
            Dim iTimeOut As Long
            Dim sProcName As String
            Dim pi As ProcessStartInfo
            Try
                p = New Process()
                pi = New ProcessStartInfo()
                pi.WorkingDirectory = GetGridFolder() + "guiminer\"
                pi.UseShellExecute = False
                pi.FileName = pi.WorkingDirectory + "\guiminer.exe"
                pi.WindowStyle = ProcessWindowStyle.Maximized
                pi.CreateNoWindow = False
                p.StartInfo = pi
                If Not File.Exists(pi.FileName) Then
                    lblThanks.Text = "GUI Miner missing. "
                    If IsGPUEnabled(0) Then lblThanks.Text = ""
                    Exit Sub
                End If
                p.Start()
                Application.DoEvents()
                Threading.Thread.Sleep(100)
                Application.DoEvents()
                sProcName = p.ProcessName

            Catch ex As Exception
                lblThanks.Text = "Error loading GUIMiner."
                Exit Sub
            End Try

            hwnd = GetHwndByProcessName(sProcName, sCap)
            Dim c As Control
            Try
                'TabControl1.TabIndex = 1 'guiminer
                TabControl1.SelectedTab = TabControl1.TabPages(1)
                c = Pb1
                If Not hwnd.Equals(IntPtr.Zero) Then
                    Dim sThanks As String
                    sThanks = "Special Thanks go to Taco Time, Kiv MacLeod, m0mchil, and puddinpop for guiminer, cgminer and reap-er."
                    lblThanks.Text = sThanks
                    lblThanks.Font = New Font("Arial", 5)
                    pi.WindowStyle = ProcessWindowStyle.Maximized
                    SetParent(hwnd, c.Handle)
                    ShowWindow(hwnd, 5)
                    SetWindowText(hwnd, "Miner1")
                    RemoveTitleBar(hwnd)
                    MoveWindow(hwnd, 0, 0, c.Width, c.Height, True)
                    If c.Width < 910 Then
                        MoveWindow(hwnd, 0, 0, 917, 386, True)
                    End If
                    c.Refresh()
                    Application.DoEvents()
                End If
            Catch ex As Exception
                lblThanks.Text = "Error initializing guiminer. " + ex.Message
                Exit Sub
            End Try


        Catch ex As Exception
            lblThanks.Text = "Unhandled Exception in Mining Console " + ex.Message

        End Try



    End Sub

    Private Function GetHwndByProcessName(sProcName As String, sCaption As String) As IntPtr


        Dim hwnd As IntPtr
        Dim iTimeOut As Integer

        Try
            Do While hwnd = 0
                For Each guiminer As Process In Process.GetProcesses
                    If guiminer.ProcessName Like sProcName + "*" Then
                        hwnd = guiminer.Handle
                        Exit For
                    End If
                Next
                System.Threading.Thread.Sleep(300)
                iTimeOut = iTimeOut + 1
                hwnd = FindWindowByCaption(IntPtr.Zero, sCaption)
                If CDbl(hwnd) > 1 Then Exit Do
                If iTimeOut > 33 Then Exit Do
                Application.DoEvents()
            Loop

        Catch ex As Exception
            Try
                hwnd = FindWindowByCaption(IntPtr.Zero, sCaption)

            Catch exx As Exception

            End Try

        End Try
        Return hwnd

    End Function
    Private Sub BootGridcoinMiner(iInstance As Integer)
        Dim pi As ProcessStartInfo
        Dim p As Process

        Try
            p = New Process()
            pi = New ProcessStartInfo()
            '  pi.WorkingDirectory = GetGRCAppDir()
            pi.WorkingDirectory = GetGridFolder() + "cgminer" + Trim(iInstance) + "\"
            pi.WindowStyle = ProcessWindowStyle.Minimized


            'The process object must have shellexecute set to false in order to use env variables:

            pi.UseShellExecute = False


            If pi.UseShellExecute = False Then
                Try
                    pi.EnvironmentVariables.Remove("GPU_MAX_ALLOC_PERCENT")
                    pi.EnvironmentVariables.Remove("GPU_USE_SYNC_OBJECTS")
                Catch ex As Exception
                End Try

                Try
                    pi.EnvironmentVariables.Add("GPU_MAX_ALLOC_PERCENT", "100")
                Catch ex As Exception
                End Try
                Try
                    pi.EnvironmentVariables.Add("GPU_USE_SYNC_OBJECTS", "1")
                Catch ex As Exception
                End Try
            End If

            pi.FileName = pi.WorkingDirectory + "\cgminer.exe"

            pi.Arguments = "-c cgm" + Trim(iInstance)
            pi.CreateNoWindow = False
            pi.WindowStyle = ProcessWindowStyle.Normal

            p.StartInfo = pi
            If Not File.Exists(pi.FileName) Then
                lblThanks.Text = "CGMiner is missing. "
                Exit Sub
            End If

            p.Start()
            TabCGMINER.Select()
            Dim hwnd As IntPtr
            Dim hwnd2 As IntPtr

            Threading.Thread.Sleep(100)
            For x = 0 To 30
                Application.DoEvents()
                hwnd2 = GetHwndByProcessName(p.ProcessName, p.MainWindowTitle)
                ShowWindow(hwnd2, 0) 'Make invisible
                If p.Id <> 0 And p.MainWindowTitle <> "" Then Exit For
                Threading.Thread.Sleep(100)
            Next
            hwnd = GetHwndByProcessName(p.ProcessName, p.MainWindowTitle)
            mCGMinerHwnd(iInstance) = hwnd
            mCGMinerAPIDown(iInstance) = 0
            ShowWindow(hwnd, 0) 'Make invisible
            Application.DoEvents()
        Catch Ex As Exception
            lblThanks.Text = Ex.Message
        End Try

    End Sub
    Public Sub updateGh()
        Dim dGh(4) As Double
        Dim dStales(4) As Double
        Dim dAccepted(4) As Double
        Dim dGhTotal(0) As Double
        Dim dStalesTotal(0) As Double
        Dim dAcceptedTotal(0) As Double
        Dim dInvalid(5) As Double
        Dim dInvalidTotal(0) As Double

        Dim sMsg As String

        Try
            lblCGMessage.Text = ""

            If chkMiningEnabled.Checked = True Then
                
                For x = 0 To 4
                    If IsGPUEnabled(x) Then
                        clsUtilization.RetrieveGPUStats(x, dGh(x), dStales(x), dAccepted(x), dInvalid(x), sMsg)
                        If Len(sMsg) > 0 Then
                            lblCGMessage.Text = lblCGMessage.Text + " " + sMsg
                        End If

                        dGhTotal(0) = dGhTotal(0) + dGh(x)
                        dStalesTotal(0) = dStalesTotal(0) + dStales(x)
                        dAcceptedTotal(0) = dAcceptedTotal(0) + dAccepted(x)
                        dInvalidTotal(0) = dInvalidTotal(0) + dInvalid(x)
                        Dim sControl As String = "txtGPU" + Trim(x)
                        UpdateGPUUIValue(sControl, Trim(Math.Round(dGh(x), 0)))
                    End If

                Next x
            End If

            lblGPUMhs.Text = Trim(Math.Round(dGhTotal(0), 2))
            lblAccepted.Text = Trim(dAcceptedTotal(0))
            lblStale.Text = Trim(dStalesTotal(0))
            lblInvalid.Text = Trim(dInvalidTotal(0))

            msBlockHeight.Text = Trim(nBestBlock)

            If LastMHRate = lblGPUMhs.Text Then lMHRateCounter = lMHRateCounter + 1
            If lMHRateCounter > 2 Then
                lMHRateCounter = 0
                'Clear Miner MhRate
                'clsUtilization.ClearGPUStats()
            End If
            LastMHRate = lblGPUMhs.Text
        Catch ex As Exception

        End Try


        'Update Boinc Stats:
        Try
            '  lblPower.Text = Trim(Math.Round(clsUtilization.BoincUtilization, 1))
            ' lblThreadCount.Text = Trim(clsUtilization.BoincThreads)
            lblVersion.Text = Trim(clsUtilization.Version)
            lblAvgCredits.Text = Trim(clsUtilization.BoincTotalCreditsAvg)
            lblMD5.Text = Trim(clsUtilization.BoincMD5)
        Catch ex As Exception

        End Try


    End Sub
    Public Function IsGPUEnabled(lDevId As Long) As Boolean
        Dim sEnabled As String

        Try

        sEnabled = KeyValue("dev" + Trim(lDevId) + "_enabled")

        If sEnabled = "" Then sEnabled = "0"
        Return CBool(sEnabled)
        Catch ex As Exception

        End Try

    End Function
    Public Sub RestartMiners()
        Dim tMiners As New System.Threading.Thread(AddressOf ThreadReStartMiners)
        tMiners.Priority = ThreadPriority.Lowest
        tMiners.Start()

    End Sub
    Public Sub ThreadReStartMiners()

        If KeyValue("suppressminingconsole") = "true" Then Exit Sub

        Log("Restarting all miners.")

        Try
            KillGuiMiner()
            RestartedMinerAt = Now


            Try

            Catch ex As Exception

            End Try

            ReStartGuiMiner_Old()
            Dim sEnabled As String
            Dim bEnabled As Boolean
            If chkMiningEnabled.Checked Then
                'Start Gridcoin Miner
                For x = 0 To 5
                    bEnabled = IsGPUEnabled(x)
                    If bEnabled Then
                        BootGridcoinMiner(x)
                    End If
                    Dim lWaitDelay As Long
                    lWaitDelay = Val(KeyValue("RestartMinersWaitDelay"))
                    If lWaitDelay > 0 Then
                        System.Threading.Thread.Sleep(lWaitDelay)
                    End If
                Next
            End If
            RefreshGPUList()
            mIDelay = 6000
            UpdateIntensity()

        Catch ex As Exception
        End Try
    End Sub
    Private Sub InitializeFormMining()
        ReStartMiners()

    End Sub
    Private Sub timerReaper_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerReaper.Tick
        Try
            miInitCounter = miInitCounter + 1
            If miInitCounter = 3 Then
                Call InitializeFormMining()

            End If
            updateGh()
            If miInitCounter = 50 Then Call UpdateIntensity()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub HideToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles HideToolStripMenuItem.Click
        Me.Hide()

    End Sub

    Private Sub ConfigurationToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ConfigurationToolStripMenuItem.Click
        Call clsUtilization.ShowGridcoinMiner()
    End Sub

    Public Sub UpdateChartHashRate()
        Try
            ChartHashRate.Series.Clear()
            ChartHashRate.Titles.Clear()
            ChartHashRate.BackColor = Color.Transparent
            ChartHashRate.ForeColor = Color.Red
            ChartHashRate.Titles.Add("GPU Hash Rate")
            ChartHashRate.Titles(0).ForeColor = Color.Green
            
            ChartHashRate.ChartAreas(0).BackColor = Color.Transparent
            ChartHashRate.ChartAreas(0).BackSecondaryColor = Color.PaleVioletRed
            Dim sHR As New Series
            sHR.Name = "HR"
            sHR.ChartType = SeriesChartType.Pie
            sHR.LabelBackColor = Color.Lime
            sHR.IsValueShownAsLabel = False
            sHR.LabelForeColor = Color.Honeydew
            ChartHashRate.Series.Add(sHR)
            Dim dHR As Double
            dHR = Val(lblGPUMhs.Text)
            If dHR > MaxHR Then MaxHR = dHR
            '''''''''''''''''''''''''''''''Add Max Hash Rate

            ChartHashRate.Series("HR").Points.AddY(MaxHR)
            ChartHashRate.Series("HR").LabelBackColor = Color.Transparent
            ChartHashRate.Series("HR").Points(0).Label = Trim(dHR)
            ChartHashRate.Series("HR").Points(0).Color = Color.Blue
            ChartHashRate.Series("HR").Points(0).LegendToolTip = Trim(dHR) + " Hash Rate"
            ''''''''''''''''''''''''''''''''''Add Actual Hash Rate
            ChartHashRate.Series("HR").Points.AddY(MaxHR - dHR)
            ChartHashRate.Series("HR")("PointWidth") = "0.5"
            ChartHashRate.Series("HR").IsValueShownAsLabel = False
            ChartHashRate.Series("HR")("BarLabelStyle") = "Center"
            ChartHashRate.ChartAreas(0).Area3DStyle.Enable3D = True
            ChartHashRate.Series("HR")("DrawingStyle") = "Cylinder"
            ChartHashRate.Update()

        Catch ex As Exception
        End Try

    End Sub
    Private Sub UpdateGPUUIValue(sKey As String, sValue As String)
        Try
            Dim c() As Windows.Forms.Control
            c = Me.Controls.Find(sKey, True)
            c(0).Text = sValue
        Catch ex As Exception

        End Try
    End Sub
    Private Sub frmMining_Load(sender As Object, e As System.EventArgs) Handles Me.Load
    
        Try

      
        RestartedWalletAt = Now
        'Set the defaults for the checkboxes
        chkFullSpeed.Checked = cBOO(KeyValue("chkFullSpeed"))
        chkMiningEnabled.Checked = cBOO(KeyValue("chkMiningEnabled"))
        chkCGMonitor.Checked = cBOO(KeyValue("chkCGMonitor"))
        bSuccessfullyLoaded = True
        If KeyValue("suppressminingconsole") = "true" Then Exit Sub

        Call OneMinuteUpdate()
        RefreshGPUList()

        ResizeScreen()

        BoincWebBrowser.ScriptErrorsSuppressed = True

        BoincWebBrowser.Navigate("http://boincstats.com/en/stats/-1/team/detail/118094994/overview")
        Catch ex As Exception

        End Try


    End Sub
    Private Sub PersistCheckboxes()
        If Not bSuccessfullyLoaded Then Exit Sub

        Try

        'Set the defaults for the checkboxes
        UpdateKey("chkFullSpeed", chkFullSpeed.Checked.ToString)
        UpdateKey("chkMiningEnabled", chkMiningEnabled.Checked.ToString)
        UpdateKey("chkCGMonitor", chkCGMonitor.Checked.ToString)
        Catch ex As Exception

        End Try

    End Sub
    Private Sub ResizeScreen()
        Dim height As Double = Screen.PrimaryScreen.Bounds.Height - 130

        Dim maxheight As Double = 0
        Try
            For Each c As Control In Me.Controls
                If (c.Location.Y + c.Height) > maxheight Then maxheight = (c.Location.Y + c.Height)
            Next
            If maxheight < height Then Exit Sub

            Dim stretch As Double
            stretch = height / maxheight
            For Each c As Control In Me.Controls
                Dim newY As Double = c.Location.Y * stretch
                Dim newHeight As Double = c.Height * stretch
                c.Height = newHeight
                c.Location = New Point(c.Location.X, newY)
            Next
            Me.Height = Me.Height * stretch
            Me.Height = Me.Height + 50
            Me.Top = 10
        Catch ex As Exception

        End Try

    End Sub
    Private Sub RefreshGPUList()
        Try

        cmbSelectedGPU.Items.Clear()

        For x = 0 To 5
            If IsGPUEnabled(x) Then
                Dim sNarr As String
                    sNarr = KeyValue("dev" + Trim(x) + "_name")
                    If Len(sNarr) > 0 Then
                        cmbSelectedGPU.Items.Add(sNarr)
                        cmbSelectedGPU.SelectedIndex = x
                    End If

                End If
            Next
        Catch ex As Exception

        End Try

    End Sub
    Private Sub chkFullSpeed_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkFullSpeed.CheckedChanged

        mIDelay = 1
        UpdateIntensity()
        PersistCheckboxes()

    End Sub

    Private Sub chkMiningEnabled_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkMiningEnabled.CheckedChanged
        mIDelay = 1
        UpdateIntensity()
        PersistCheckboxes()

    End Sub
    Public Sub UpdateIntensity()
        Dim tIntense As New System.Threading.Thread(AddressOf ThreadUpdateIntensity)
        tIntense.Start()
    End Sub
    Private Sub ThreadUpdateIntensity()
        Try
            System.Threading.Thread.Sleep(mIDelay)


            For x = 0 To 5
                If IsGPUEnabled(x) Then
                    Dim dI As Double
                    dI = Val(KeyValue("dev" + Trim(x) + "_intensity"))
                    If chkFullSpeed.Checked = False Then dI = dI * 0.65
                    If chkMiningEnabled.Checked = False Then dI = 0
                    Dim sResult As String
                    sResult = modCgMiner.UpdateIntensity(x, dI)

                End If

            Next

        Catch ex As Exception

        End Try

    End Sub

    Private Sub RichTextBox1_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles RichTextBox1.KeyDown

        If e.KeyCode <> Keys.Enter Then Exit Sub
        If Len(RichTextBox1.Text) < 2 Then Exit Sub
        
        Dim cmd As String
        For z = RichTextBox1.Text.Length - 1 To 1 Step -1
            If Mid(RichTextBox1.Text, z, 1) = Chr(10) Or z = 1 Then
                cmd = Mid(RichTextBox1.Text, z, RichTextBox1.Text.Length)
                Exit For
            End If
        Next
        cmd = Trim(cmd)
        cmd = Replace(cmd, Chr(10), "")
        cmd = Replace(cmd, Chr(13), "")
        
        Dim vCmd() As String
        vCmd = Split(cmd, " ")
        If UBound(vCmd) = 1 Then
            If vCmd(0) = "summary" And Len(vCmd(1)) > 0 Then
                Dim lDevId As Long = Val(vCmd(1))
                Dim sOut As String
                sOut = modCgMiner.CgSend("summary", lDevId)
                RichTextBox1.AppendText(vbCrLf + sOut + vbCrLf)

            End If
        End If
    End Sub

    Private Sub btnShowCgminer_Click(sender As System.Object, e As System.EventArgs) Handles btnShowCgminer.Click
        Dim lDevId As Long = cmbSelectedGPU.SelectedIndex
        If lDevId > -1 Then ShowWindow(mCGMinerHwnd(lDevId), 1)

    End Sub

    Private Sub btnHideCgminer_Click(sender As System.Object, e As System.EventArgs) Handles btnHideCgminer.Click
        Dim lDevId As Long = cmbSelectedGPU.SelectedIndex
        If lDevId > -1 Then ShowWindow(mCGMinerHwnd(lDevId), 0)

    End Sub

    Private Sub TimerCGMonitor_Tick(sender As System.Object, e As System.EventArgs) Handles TimerCGMonitor.Tick
        'RefreshLeaderboardPosition()
        'Scrypt Sleep : if we are sleeping, don't bother monitoring the GPUs:
      
        'For each enabled CGMiner, restart if down:
        Try
            If chkCGMonitor.Checked = False Then Exit Sub
            For x = 0 To 5
                If IsGPUEnabled(x) Then
                    If Val(lblGPUMhs.Text) = 0 Then
                        Log("CGMiner down.  Restarting miners.")

                        RestartMiners()
                        Log("Miners restarted.")

                        Exit Sub
                    End If
                End If
            Next

        Catch ex As Exception

        End Try

    End Sub

    Private Sub MenuStrip1_ItemClicked(sender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles MenuStrip1.ItemClicked

    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim sMessage1 As String
        sMessage1 = KeyValue("CPUMessage2")
        sMessage1 = "notified"
        If sMessage1 = "" Then
            Dim sMsg As String = "Note: If you are CPUMining, we recently added a requirement to be a member of Team 'Gridcoin'.  " _
                                 & "Please click on Projects, and verify that you are a member of Team Gridcoin for each project you participating in.  " _
                                 & "Thank you for supporting Gridcoin.  This message will not be displayed again."

            sMsg = "Note: Due to a security flaw that was discovered in the CPUMiner Payment System, the CPUMiner payment system had to be shut down.  GPU mining works with Gridcoin as previously designed.  Please read the forums at CryptoCoinTalk for any updates."

            MsgBox(sMsg, MsgBoxStyle.Critical, "Gridcoin Network Message")
            UpdateKey("CPUMessage2", "Notified")
        End If


    End Sub

    Private Sub chkCGMonitor_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkCGMonitor.CheckedChanged

        Try
            PersistCheckboxes()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub PoolsToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles PoolsToolStripMenuItem.Click
        Dim p As New frmPools
        p.Show()

    End Sub


    Private Sub btnReauthenticate_Click(sender As System.Object, e As System.EventArgs)
        
    End Sub
End Class