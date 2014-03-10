Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.IO
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Object
Imports System.Security.Cryptography
Imports System.Timers

Module modUtilization

    <System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, Pack:=1)> _
    Public Structure Process_Basic_Information
        Public ExitStatus As IntPtr
        Public PepBaseAddress As IntPtr
        Public AffinityMask As IntPtr
        Public BasePriority As IntPtr
        Public UniqueProcessID As IntPtr
        Public InheritedFromUniqueProcessId As IntPtr
    End Structure


    <System.Runtime.InteropServices.DllImport("ntdll.dll", EntryPoint:="NtQueryInformationProcess")> _
    Public Function NtQueryInformationProcess(ByVal handle As IntPtr, ByVal processinformationclass As UInteger, ByRef ProcessInformation As Process_Basic_Information, ByVal ProcessInformationLength As Integer, ByRef ReturnLength As UInteger) As Integer
    End Function

    Private mBCE As Long = 0

    Public Const PROCESSBASICINFORMATION As UInteger = 0
    Public LastBlockHash As String = ""
    Public _timerBoincCredits As System.Timers.Timer
    Public _timerBoincUtilization As System.Timers.Timer

    Public mnBestBlock As Long




    Public Const BOINC_MEMORY_FOOTPRINT As Double = 5000000
    Public Const KERNEL_OVERHEAD As Double = 1.5
    Private last_sample As Double = 0
    Public BoincAvgOverTime As String = ""
    Public _BoincMD5 = ""
    Public _BoincAuthenticity As Double = 0
    Public BlockData As String = ""
    Public PublicWalletAddress As String = ""
    Public mdProcNarrComponent1 As Double
    Public mdProcNarrComponent2 As Double

    Public mBoincProcessorUtilization As Double = 0
    Public mBoincThreads As Double = 0

  
    Public Sub Initialize()
        Housecleaning()
        TeamGridcoinProjects(1) = "FufdrPKUXNMrSFNMky6u18F6r6rwUmnAbb,http://www.malariacontrol.net"
        TeamGridcoinProjects(2) = "G3HA4ouWr1zbhKd7jUU5ZRqR8sPsYdYmYb,http://www.rnaworld.de/rnaworld"
        TeamGridcoinProjects(3) = "G1dUDVaFG8HmxuzRyCHhqU5k7eQied1nWx,http://boinc.bakerlab.org/rosetta"
        TeamGridcoinProjects(4) = "G6RdibWpbYQgvvcTThx6NG2vHdBK1a51eE,http://docking.cis.udel.edu"
        TeamGridcoinProjects(5) = "FvDfoheNe74JcUp6uf3N8cPeU4KeUsxPq7,http://milkyway.cs.rpi.edu/milkyway"

        If _timerBoincUtilization Is Nothing Then
            _timerBoincUtilization = New System.Timers.Timer(20000)
            AddHandler _timerBoincUtilization.Elapsed, New ElapsedEventHandler(AddressOf BoincUtilizationTimerElapsed)
            _timerBoincUtilization.Enabled = True
        End If

        If _timerBoincCredits Is Nothing Then
            _timerBoincCredits = New System.Timers.Timer(300000)
            AddHandler _timerBoincCredits.Elapsed, New ElapsedEventHandler(AddressOf BoincCreditsElapsed)
            _timerBoincCredits.Enabled = True
            BoincCreditsElapsed()

        End If

    End Sub
    Private Sub BoincUtilizationTimerElapsed()
        Try
            _timerBoincUtilization.Enabled = False
            ReturnBoincCPUUsage()
        Catch ex As Exception
        End Try
        _timerBoincUtilization.Enabled = True
    End Sub
    Private Sub BoincCreditsElapsed()
        Try
            mBCE = mBCE + 1
            If mBCE > 1 Then LogBoincCredits()
            ReturnBoincCreditsAtPointInTime(86400 / 2)
            modBoincCredits.BoincCredits = BoincCreditsAtPointInTime
            modBoincCredits.BoincCreditsAvg = BoincCreditsAvgAtPointInTime

        Catch ex As Exception
        End Try
    End Sub
 
    Public Function modAvgOverTime()
        Try
            Dim sample1 As Double
            Dim sample2 As Double
            Dim sample3 As Double
            sample1 = ReturnBoincCreditsAtPointInTime(86400)
            sample2 = ReturnBoincCreditsAtPointInTime(604800)
            sample3 = ReturnBoincCreditsAtPointInTime(2419200)
            Dim sOut As String
            sOut = Trim(Math.Round(sample1, 0)) + ":" + Trim(Math.Round(sample2, 0)) + ":" + Trim(Math.Round(sample3, 0))
            BoincAvgOverTime = sOut
            Return sOut
        Catch ex As Exception
            Return "?:?:?"
        End Try

    End Function
    Public Function GetUtilizationByPID(ByVal PID As Integer) As Double
        Dim cat As New PerformanceCounterCategory("Process")
        Dim instances() As String = Nothing
        Try
            instances = cat.GetInstanceNames()
        Catch
            Return 0
        End Try
        Dim i As Integer
        Try

        For Each instance In instances
            Using cnt As PerformanceCounter = New PerformanceCounter("Process", "ID Process", instance, True)
                Dim val As Integer = CType(cnt.RawValue, Int32)
                If val = PID Then
                    Dim pc As PerformanceCounter = New PerformanceCounter("Process", "% Processor Time", instance, True)
                    pc.NextValue()
                    Dim dPIDProcessorTime As Double
                    For i = 1 To 4
                        Threading.Thread.Sleep(100)
                        dPIDProcessorTime = pc.NextValue
                        If dPIDProcessorTime > 0 And i > 1 Then Return dPIDProcessorTime
                    Next i
                End If
            End Using
        Next
            Return 0

        Catch ex As Exception
            Return 0

        End Try

    End Function
    Public Function ReturnBoincCPUUsage() As Double
        Dim thBoincCPU As New Thread(AddressOf Thread_ReturnBoincCPUUsage)
        thBoincCPU.IsBackground = True
        thBoincCPU.Start()
        Return mBoincProcessorUtilization
    End Function
    Public Function Thread_ReturnBoincCPUUsage() As Double
        Dim masterProcess As Process()
        masterProcess = Process.GetProcessesByName("BOINC")
        If masterProcess.Length = 0 Then
            GoTo CalculateUsage
        End If
        Dim p As Process
        Dim localAll As Process() = Process.GetProcesses()
        Dim runningtime As TimeSpan
        Dim processortime As TimeSpan
        Dim percent As Double
        Dim total_runningtime As TimeSpan
        Dim total_processortime As TimeSpan
        Dim lThreadCount As Double
        Dim total_memory_usage As Double
        Dim current_pid_utilization As Double
        Dim ptc As ProcessThreadCollection
        Dim pt As ProcessThread
        Dim parentProcess As Process
        Dim greatParent As Process
        Dim isBoinc As Boolean
        For Each p In localAll
            'If this a boinc process
            If p.ProcessName <> "System" And p.ProcessName <> "Idle" Then
                Try
                    isBoinc = False

                    parentProcess = GetInheritedParent(p.Handle)
                    greatParent = GetInheritedParent(parentProcess.Handle)
                    If Not parentProcess Is Nothing Then
                        If parentProcess.Id = masterProcess(0).Id Then isBoinc = True
                        If parentProcess.ProcessName.ToLower = "boinc" Or parentProcess.ProcessName.ToLower = "boincmgr" Then isBoinc = True

                    End If
                    If Not greatParent Is Nothing Then
                        If greatParent.ProcessName.ToLower = "boinc" Or greatParent.ProcessName.ToLower = "boincmgr" Then isBoinc = True
                    End If

                    If isBoinc Then

                        current_pid_utilization = GetUtilizationByPID(p.Id)
                        If current_pid_utilization > 0 Then
                            runningtime = Now - p.StartTime
                            processortime = p.TotalProcessorTime
                            percent = (processortime.TotalSeconds + p.UserProcessorTime.TotalSeconds) / runningtime.TotalSeconds
                            total_runningtime = total_runningtime + runningtime
                            total_processortime = total_processortime + processortime
                            lThreadCount = lThreadCount + 1
                            total_memory_usage = total_memory_usage + p.PrivateMemorySize64
                        End If
                    End If
                Catch ex As Exception
                    'This happens when a process is stopped in the middle of the count, or when we have a reference to an object that was collected

                End Try
            End If

        Next
CalculateUsage:

        Dim usage_percent As Double = 0
        If total_runningtime.TotalSeconds > 0 Then
            usage_percent = (total_processortime.TotalSeconds / total_runningtime.TotalSeconds) * KERNEL_OVERHEAD
        End If
        Dim memory_usage As Double
        memory_usage = total_memory_usage / BOINC_MEMORY_FOOTPRINT
        'Attribute up to 25% of the score to boinc project, memory usage
        If memory_usage > 0.25 Then memory_usage = 0.25
        If memory_usage < 0 Then memory_usage = 0
        'Attribute up to 75% of the score to boinc thread processor utilization totals
        If usage_percent > 0.75 Then usage_percent = 0.75
        If usage_percent < 0 Then usage_percent = 0
        'no credit for memory if processor is idle
        If lThreadCount < 1 Then usage_percent = 0
        If usage_percent = 0 Then memory_usage = 0
        usage_percent = usage_percent + memory_usage
        usage_percent = usage_percent * 100 'Convert to a 3 digit percent
        If usage_percent > 100 Then usage_percent = 100
        If usage_percent < 0 Then usage_percent = 0
        Dim h As Double
        h = HomogenizedDailyCredits(usage_percent)
        usage_percent = h
        'Create a two point moving average
        Dim avg_sample As Double
        avg_sample = (last_sample + usage_percent) / 2
        mBoincProcessorUtilization = Math.Round(avg_sample, 0)
        last_sample = mBoincProcessorUtilization
        mBoincThreads = lThreadCount
        Return usage_percent
    End Function
    Public Function HomogenizedDailyCredits(cpu_use As Double)
        Dim dAvg As Double = BoincCreditsAvg
        If dAvg > 3000 Then dAvg = 3000
        If dAvg < 0.01 Then dAvg = 0.01
        Dim dUsage As Double = dAvg / 10
        If dUsage > 100 Then dUsage = 100
        If dUsage < 0 Then dUsage = 0
        mdProcNarrComponent1 = cpu_use
        mdProcNarrComponent2 = dUsage
        Dim avg1 As Double = (cpu_use + dUsage) / 2
        Return avg1
    End Function
    Public Function GetInheritedParent(ByVal sPID As IntPtr) As Process
        Try
            Dim ProccessInfo As New Process_Basic_Information
            'Used as an output parameter by the API function
            Dim RetLength As UInteger
            NtQueryInformationProcess(sPID, PROCESSBASICINFORMATION, ProccessInfo, Marshal.SizeOf(ProccessInfo), RetLength)
            Dim sParentID As IntPtr
            sParentID = ProccessInfo.InheritedFromUniqueProcessId
            Dim pOut As Process
            pOut = Process.GetProcessById(sParentID)
            Return pOut
        Catch ex As Exception

        End Try
    End Function

    Public Function GetSleepLevelByAddress(sAddress As String, ByRef dScryptSleep As Double, sBlockhash As String, _
                                            ByRef dNetLevel As Double) As Boolean
        'Retrieve User level
        dScryptSleep = 0
        dNetLevel = 0
        Try
            Dim sql As String
            Dim mData As New Sql("gridcoin_leaderboard")
            sql = "Select ScryptSleepChance from leaderboard Where Address='" + sAddress + "'"
            Dim gr As New GridcoinReader
            gr = mData.GetGridcoinReader(sql)
            If gr.Rows = 0 Then
                dScryptSleep = 0.5

            End If
            'Dim grr As GridcoinReader.GridcoinRow
            dScryptSleep = gr.Value(1, "ScryptSleepChance")
            mData = Nothing
        Catch ex As Exception
            Log("GetSleepLevelByAddress: " + ex.Message)
        End Try

        ''''''''''''''''Newbie Sleep Level Patch:
        If dScryptSleep = 0 Then dScryptSleep = NewbieSleepLevel()

        If dScryptSleep = 0 Then dScryptSleep = 0.5


        Try

        'Calculate Net Level
        Dim sBlockSuffix As String

        If Len(sBlockhash) > 3 Then
            sBlockSuffix = Mid(sBlockhash, Len(sBlockhash) - 3, 3)
        End If
        Dim dDecSuffix = CDbl("&h" + Trim(sBlockSuffix))
        Dim dCalc = dDecSuffix / 40.96
        dCalc = dCalc / 100
        Dim sSleepStatus As String = "WORK"
        If dScryptSleep >= dCalc Then sSleepStatus = "WORK" Else sSleepStatus = "SLEEP"
        dNetLevel = dCalc
        If sSleepStatus = "SLEEP" Then
            Return False
        Else
            Return True
        End If

        Catch ex As Exception
            Log("GetSleepByAddress" + ex.Message + ":" + ex.Source)

        End Try

    End Function

End Module
