Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Diagnostics

Imports System.Timers
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Threading

Public Class frmMining

    Public clsUtilization As Utilization
    Private RefreshCount As Long
    Private RestartedMinerAt As DateTime
    Private RestartedWalletAt As DateTime

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

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        Refresh2(True)
    End Sub
    Private Sub UpdateCharts()
        Try
            Dim thCharts As New Thread(AddressOf UpdateChartsThread)
            thCharts.IsBackground = True
            thCharts.Start()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub UpdateChartsThread()
        Try
            ChartBoinc()
            ChartBoincUtilization()
            Me.Update()
        Catch ex As Exception
        End Try
    End Sub
    Public Sub Refresh2(ByVal bStatsOnly As Boolean)
        bCharting = False
        Try
            If Not bStatsOnly Then
                UpdateCharts()
            Else
                ChartBoincUtilization()
            End If
        Catch ex As Exception
        End Try

        Try
            lblPower.Text = Trim(Math.Round(clsUtilization.BoincUtilization, 1))
            lblThreadCount.Text = Trim(clsUtilization.BoincThreads)
            lblVersion.Text = Trim(clsUtilization.Version)
            lblAvgCredits.Text = Trim(clsUtilization.BoincTotalCreditsAvg)

        Catch ex As Exception

        End Try

        Try
            lblMD5.Text = Trim(clsUtilization.BoincMD5)
            RefreshCount = RefreshCount + 1
            If RefreshCount = 1 Then
                UpdateChartsThread()

                For x = 1 To 1 'Soft start while client starts
                    Application.DoEvents()
                    System.Threading.Thread.Sleep(700)
                Next
                ReStartGuiMiner()
                RestartedWalletAt = Now
                System.Threading.Thread.Sleep(100)
            End If

        Catch ex As Exception
        End Try
        Try
            Dim lMinSetting = Val(KeyValue("RestartMiner"))
            Dim lRunning = Math.Abs(DateDiff(DateInterval.Minute, RestartedMinerAt, Now))
            If lMinSetting = 0 Then
                lblRestartMiner.Text = "Never"
            Else
                Dim dCountdown As Double
                dCountdown = lMinSetting - lRunning
                lblRestartMiner.Text = Trim(dCountdown)
                If dCountdown <= 0 Then
                    ReStartGuiMiner()
                    Refresh2(True)
                End If
            End If

        Catch ex As Exception

        End Try
        Try
            Dim lMinSetting = Val(KeyValue("RestartWallet"))
            Dim lRunning = Math.Abs(DateDiff(DateInterval.Minute, RestartedWalletAt, Now))
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

        Catch ex As Exception

        End Try

        bSuccessfullyLoaded = True

    End Sub
    Public Sub KillGuiMiner()

        Try

            For x = 1 To 6
                KillProcess("guiminer")

                KillProcess("cgminer*")

                KillProcess("cgminer")

                KillProcess("reaper*")

                KillProcess("reaper")

                KillProcess("conhost.exe") 'Kills up to 4 instances of surrogates
            Next x
        Catch ex As Exception
        End Try
    End Sub
    Public Function GetGRCAppDir() As String
        Try
            Dim fi As New System.IO.FileInfo(Application.ExecutablePath)
            Return fi.DirectoryName
        Catch ex As Exception
        End Try
    End Function
    Public Sub RestartWallet()
        Try
            'First kill CGMiner, Reaper and GuiMiner
            For x = 1 To 4
                KillGuiMiner()
                Threading.Thread.Sleep(500) 'Let CGMiner & Reaper close.
                Application.DoEvents()
            Next
            Threading.Thread.Sleep(2000)
            Dim p As Process = New Process()
            Dim pi As ProcessStartInfo = New ProcessStartInfo()
            pi.WorkingDirectory = GetGRCAppDir()
            pi.UseShellExecute = True
            pi.FileName = "GRCRestarter.exe"
            p.StartInfo = pi
            p.Start()
        Catch ex As Exception
        End Try
    End Sub
    Public Sub ChartBoinc()
        Try
            If bCharting Then Exit Sub

            bCharting = True

            Chart1.Series.Clear()
            Chart1.Titles.Clear()
            Chart1.Titles.Add("Boinc Utilization")
            Chart1.BackColor = Color.Black
            Chart1.ForeColor = Color.Green

            Dim seriesAvgCredits As New Series
            seriesAvgCredits.Name = "Avg Daily Credits"
            seriesAvgCredits.ChartType = SeriesChartType.Line
            seriesAvgCredits.LabelBackColor = Color.Black
            seriesAvgCredits.LabelForeColor = Color.Green

            Chart1.ChartAreas(0).AxisX.LabelStyle.Format = "MM-dd-yyyy"
            Chart1.ChartAreas(0).AxisX.IntervalType = DateTimeIntervalType.Weeks
            Chart1.ChartAreas(0).AxisX.TitleForeColor = Color.Gainsboro

            Chart1.ChartAreas(0).ShadowColor = Color.Chocolate
            Chart1.ChartAreas(0).BackSecondaryColor = Color.Gray
            Chart1.ChartAreas(0).BorderColor = Color.Gray
            Chart1.Legends(0).BackColor = Color.Black
            Chart1.Legends(0).ForeColor = Color.Green

            Dim seriesTotalCredits As New Series
            seriesTotalCredits.ChartType = SeriesChartType.FastLine
            seriesTotalCredits.Name = "Total Daily Credits"
            Dim dProj As Double
            Dim seriesProjects As New Series
            seriesProjects.Name = "Projects"
            Chart1.ChartAreas(0).AxisX.Interval = 1
            seriesProjects.ChartType = SeriesChartType.StepLine
            Dim lookback As Double '
            For x = 30 To 0.5 Step -1.5
                lookback = x * 3600 * 24
                clsGVM.ReturnBoincCreditsAtPointInTime(lookback)
                Dim l1 As Double
                Dim l2 As Double
                Dim l3 As Double
                Dim dAvg As Double
                l1 = clsGVM.BoincCredits
                clsGVM.ReturnBoincCreditsAtPointInTime(lookback - (3600 * 24))
                l2 = clsGVM.BoincCredits
                dAvg = clsGVM.BoincCreditsAvg
                l3 = Math.Abs(l1 - l2)
                Application.DoEvents()
                Chart1.Refresh()
                System.Threading.Thread.Sleep(50)
                Dim pCreditsAvg As New DataPoint
                dProj = clsGVM.BoincProjects
                Dim d1 As Date = DateAdd(DateInterval.Day, -x, Now)
                pCreditsAvg.SetValueXY(d1, dAvg)
                seriesAvgCredits.Points.Add(pCreditsAvg)
                Dim dpProj As New DataPoint()
                dpProj.SetValueXY(d1, dProj * (dAvg / 10))
                seriesProjects.Points.Add(dpProj)
                Dim pCreditsTotal As New DataPoint()
                pCreditsTotal.SetValueXY(d1, l3)
                seriesTotalCredits.Points.Add(pCreditsTotal)
            Next
            Chart1.Series.Add(seriesTotalCredits)
            Chart1.Series.Add(seriesAvgCredits)
            Chart1.Series.Add(seriesProjects)
        Catch ex As Exception
        End Try
        bCharting = False

    End Sub
    Public Sub ChartBoincUtilization()
        Try
            ChartUtilization.Series.Clear()
            ChartUtilization.Titles.Clear()
            ChartUtilization.BackColor = Color.Transparent


            ChartUtilization.ForeColor = Color.Blue

            ChartUtilization.Titles.Add("Utilization")
            ChartUtilization.ChartAreas(0).BackColor = Color.Transparent
            ChartUtilization.ChartAreas(0).BackSecondaryColor = Color.White
            ChartUtilization.Legends(0).BackColor = Color.Transparent
            ChartUtilization.Legends(0).ForeColor = Color.Green

        Dim sUtilization As New Series
        sUtilization.Name = "Utilization"
        sUtilization.ChartType = SeriesChartType.Pie
            sUtilization.LegendText = "Boinc Utilization"

            sUtilization.LabelBackColor = Color.Green
            sUtilization.IsValueShownAsLabel = False

            sUtilization.LabelForeColor = Color.Honeydew

            ChartUtilization.Series.Add(sUtilization)

        Dim bu As Double
            bu = Math.Round(clsUtilization.BoincUtilization, 1)
            If RefreshCount < 2 Then bu = 4

            ChartUtilization.Series("Utilization").Points.AddY(bu)
            ChartUtilization.Series("Utilization").LabelBackColor = Color.Transparent
            ChartUtilization.Series("Utilization").Points(0).Label = Trim(bu)
            ChartUtilization.Series("Utilization").Points(0).Color = Color.Blue
            ChartUtilization.Series("Utilization").Points(0).LegendToolTip = Trim(bu) + " utilization."
            ChartUtilization.Series("Utilization").Points.AddY(100 - bu)
            ChartUtilization.Series("Utilization").Points(1).IsVisibleInLegend = False
            ChartUtilization.Series("Utilization")("PointWidth") = "0.5"
            ChartUtilization.Series("Utilization").IsValueShownAsLabel = False
          
            ChartUtilization.Series("Utilization")("BarLabelStyle") = "Center"
            ChartUtilization.ChartAreas(0).Area3DStyle.Enable3D = True
            ChartUtilization.Series("Utilization")("DrawingStyle") = "Cylinder"
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Call Refresh2(False)
    End Sub

    Private Sub btnRestartMiner_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRestartMiner.Click
        Try
            ReStartGuiMiner()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub RefreshRestartMinutes()
    End Sub
    Private Sub frmMining_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If bDisposing Then
            Me.Close()
            Me.Dispose()
            KillGuiMiner()
            Exit Sub
        End If
        Me.Hide()
        e.Cancel = True
    End Sub
    Private Sub btnRestart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRestart.Click
        RestartWallet()
    End Sub

    Private Sub btnCloseWallet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            KillGuiMiner()
            KillProcess("gridcoin-qt*")
        Catch ex As Exception
        End Try
    End Sub
    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        KillGuiMiner()
    End Sub

    Private Sub btnCloseWallet_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCloseWallet.Click
        Try

            KillGuiMiner()
            KillProcess("gridcoin-qt*")
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnHide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHide.Click
        Me.Hide()
    End Sub

    Private Sub btnmining_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTestCPUMiner.Click
        clsGVM.LastBlockHash = Trim(Now)
        Dim sNarr As String
        sNarr = clsGVM.SourceBlock + "  OUTPUTS : " + clsGVM.MinedHash
        MsgBox(sNarr)
    End Sub

    Private Sub timerBoincBlock_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerBoincBlock.Tick
        lblCPUMinerElapsed.Text = Trim(Math.Round(clsGVM.CPUMiner.KHPS, 0))
        lblLastBlockHash.Text = clsGVM.LastBlockHash
        If clsGVM.CPUMiner.Status = False Then
            pbBoincBlock.Visible = False
            lblBoincBlock.Text = clsGVM.CPUMiner.MinedHash
        Else
            pbBoincBlock.Visible = True
            pbBoincBlock.Value = clsGVM.CPUMiner.Elapsed.Seconds
            If clsGVM.CPUMiner.Elapsed.Seconds > pbBoincBlock.Maximum - 5 Then pbBoincBlock.Maximum = pbBoincBlock.Maximum + 15
        End If
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
                Exit Sub
            End If

            p.Start()
            Application.DoEvents()
            Threading.Thread.Sleep(500)
            Application.DoEvents()

            sProcName = p.ProcessName

        Catch ex As Exception
            lblThanks.Text = "Error loading GUIMiner."
            Exit Sub

        End Try

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
                hwnd = FindWindowByCaption(IntPtr.Zero, sCap)
                If CDbl(hwnd) > 1 Then Exit Do

                If iTimeOut > 9 Then Exit Do
                Application.DoEvents()

            Loop

        Catch ex As Exception
            Try
                hwnd = FindWindowByCaption(IntPtr.Zero, sCap)

            Catch exx As Exception

            End Try

        End Try

        If 1 = 0 Then
            Do While hwnd = 0
                hwnd = FindWindowByCaption(IntPtr.Zero, sCap)
                System.Threading.Thread.Sleep(300)
                iTimeOut = iTimeOut + 1
                If iTimeOut > 500 Then Exit Do
            Loop
        End If
        Dim c As Control

        Try
            c = Pb1
            If Not hwnd.Equals(IntPtr.Zero) Then
                Dim sThanks As String
                sThanks = "Special Thanks go to Taco Time, Kiv MacLeod, m0mchil, and puddinpop for guiminer, cgminer and reaper."
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
            lblThanks.Text = "Error initializing guiminer."
            Exit Sub

        End Try


    End Sub
    Public Sub updateGh()
    End Sub
    Public Sub ReStartGuiMiner()
        Try
            KillGuiMiner()
            RestartedMinerAt = Now
            Me.Visible = True
            ReStartGuiMiner_Old()
        Catch ex As Exception
        End Try
    End Sub

End Class