Imports System.Runtime.InteropServices


Module modFunctions
    Public Const PROCESSBASICINFORMATION As UInteger = 0

    Public mP2P As p2p
    Public sql As String


    ' Public mConnections As New Dictionary(Of Integer, frmGridWorker)
 
    Public mConnectionCount As Long
    Public mData As data
    Public Const BOINC_MEMORY_FOOTPRINT As Double = 5000000
    Public Const KERNEL_OVERHEAD As Double = 1.5

    Private last_sample As Double



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




    Public BoincProcessorUtilization As Double
    Public BoincThreads As Double

    Public Sub MonitorProcessorUtilization()
        Do While 1 = 1

            System.Threading.Thread.Sleep(62000)
            Dim zOut As Long
            ReturnBoincCPUUsage()

        Loop
    End Sub


    Public Function GetPIDProcUtilization(ByVal PID As Integer) As Double
        Dim cat As New PerformanceCounterCategory("Process")

        Dim instances() As String
        Try
            instances = cat.GetInstanceNames()
        Catch
            Stop

        End Try

        Dim xx1 As Double
        For Each instance In instances
            Using cnt As PerformanceCounter = New PerformanceCounter("Process", "ID Process", instance, True)
                Dim val As Integer = CType(cnt.RawValue, Int32)
                If val = PID Then
                    Dim pc As PerformanceCounter = New PerformanceCounter("Process", "% Processor Time", instance, True)
                    pc.NextValue()
                    Dim d As Double
                    For xx1 = 1 To 4
                        Threading.Thread.Sleep(100)
                        d = pc.NextValue
                        Application.DoEvents()
                        If d > 0 And xx1 > 1 Then Return d
                    Next xx1
                End If
            End Using
        Next
        Return 0

    End Function

    Public Function ReturnBoincCPUUsage() As Double

        Dim masterProcess As Process()
        masterProcess = Process.GetProcessesByName("BOINC")
        If masterProcess.Length = 0 Then GoTo Calculate
        
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
            'If this is a boinc process thread
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

                        current_pid_utilization = GetPIDProcUtilization(p.Id)
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

              
                End Try
            End If

        Next

calculate:
      
        Dim usage_percent As Double = 0


        If total_runningtime.TotalSeconds > 0 Then
            usage_percent = (total_processortime.TotalSeconds / total_runningtime.TotalSeconds) * KERNEL_OVERHEAD
        End If



        Dim system_memory As Double
        system_memory = 2328699904.0
        Dim memory_usage As Double
        memory_usage = total_memory_usage / BOINC_MEMORY_FOOTPRINT
        'Attribute up to 25% of the score to boinc project, memory usage

        If memory_usage > 0.25 Then memory_usage = 0.25
        If memory_usage < 0 Then memory_usage = 0
        'Attribute up to 75% of the score to boinc thread processor utilization totals

        If usage_percent > 0.75 Then usage_percent = 0.75
        If usage_percent < 0 Then usage_percent = 0

        usage_percent = usage_percent + memory_usage
        usage_percent = usage_percent * 100 'Convert to a 3 digit percent


        If usage_percent > 100 Then usage_percent = 100
        If usage_percent < 0 Then usage_percent = 0
        Dim avg_sample As Double
        avg_sample = (last_sample + usage_percent) / 2

        BoincProcessorUtilization = avg_sample
        last_sample = BoincProcessorUtilization

        BoincThreads = lThreadCount

        Return usage_percent



    End Function
    Public Function xGetParent(ByVal sPID As IntPtr) As IntPtr


        Dim pParentProcess As Process
        Dim iP1 As IntPtr
        '      iP1 = GetInheritedParent(sPID)
        pParentProcess = Process.GetProcessById(iP1)


        Dim pGreatParent As IntPtr
        Dim pGG As Process

        '     pGG = Process.GetProcessById(GetInheritedParent(pParentProcess.Handle))






        Return iP1

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

    Public Function Broadcast()
        For x = 1 To mConnectionCount
            '    If Not mConnections(x) Is Nothing Then
            '    mConnections(x).SendMessage(frmGridWorker.MessageCodes.TEXT, "Hello")


            '  End If
        Next

    End Function


 
End Module
