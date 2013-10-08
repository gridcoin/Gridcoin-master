Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.IO


Module modUtilization

    Public Const PROCESSBASICINFORMATION As UInteger = 0
    Public Const BOINC_MEMORY_FOOTPRINT As Double = 5000000
    Public Const KERNEL_OVERHEAD As Double = 1.5
    Private last_sample As Double

    Public BoincProcessorUtilization As Double
    Public BoincThreads As Double

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


    Public Sub MonitorProcessorUtilization()
        Do While 1 = 1

            System.Threading.Thread.Sleep(2500)
            ReturnBoincCPUUsage()

        Loop
    End Sub


    Public Function GetUtilizationByPID(ByVal PID As Integer) As Double
        Dim cat As New PerformanceCounterCategory("Process")

        Dim instances() As String = Nothing

        Try
            instances = cat.GetInstanceNames()
        Catch
            Exit Function
        End Try

        Dim i As Integer
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

    End Function

    Public Function ReturnBoincCPUUsage() As Double

        Dim masterProcess As Process()
        masterProcess = Process.GetProcessesByName("BOINC")
        If masterProcess.Length = 0 Then
            GoTo CalculateUsage
        End If


        Dim p As Process
        Dim localAll As Process() = Process.GetProcesses()
        Dim sOut As String

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
        If lThreadCount < 3 Then usage_percent = 0
        If usage_percent = 0 Then memory_usage = 0
        usage_percent = usage_percent + memory_usage
        usage_percent = usage_percent * 100 'Convert to a 3 digit percent
        If usage_percent > 100 Then usage_percent = 100
        If usage_percent < 0 Then usage_percent = 0
        'Create a two point moving average
        Dim avg_sample As Double
        avg_sample = (last_sample + usage_percent) / 2
        BoincProcessorUtilization = avg_sample
        last_sample = BoincProcessorUtilization
        BoincThreads = lThreadCount
        Return usage_percent
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
    Public Function VerifyBoincAuthenticity(ByVal sPath As String) As Long
        '1.  Retrieve the Boinc MD5 Hash
        '2.  Verify the boinc.exe contains the Berkeley source libraries
        '3.  Verify the exe is an official release
        '4.  Verify the size of the exe is above the threshhold
        sPath = "C:\Program Files\BOINC\boinc.exe"
        Dim s As String = File.ReadAllText(sPath)
        Dim info As New FileInfo(sPath)
        Dim sz As Long = info.Length
        'Verify windows & linux size, greater than 1.14 mb 

        If sz < 1140000 Then Return -1 'Invalid executable

        If InStr(1, s, "http://boinc.berkeley.edu") = 0 Then
            Return -2 'failed authenticity check for libraries
        End If

        If InStr(1, s, "LIBEAY32.dll") = 0 Then Return -3 'Failed authenticity check for libraries
        Dim dir As DirectoryInfo
        dir = New DirectoryInfo(sPath)
        Dim sTrayPath As String
        sTrayPath = System.IO.Path.GetDirectoryName(sPath) + "\boinctray.exe"

        info = New FileInfo(sTrayPath)
        sz = info.Length
        If sz < 30000 Then Return -4 'Failed to find Boinc Tray EXE
        Return 1 'Success

    End Function

End Module
