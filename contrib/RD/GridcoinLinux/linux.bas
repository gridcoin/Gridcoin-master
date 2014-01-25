Attribute VB_Name = "linux"
Option Explicit


Public wmi As Object
Public threads As Long
Private Declare Sub Sleep Lib "kernel32.dll" (ByVal dwMilliseconds As Long)

Public mclsUtilization As boinc.Utilization
Public msBoincProjectData As String
Public mdBoincProjects As Double
Public msBoincMD5 As String
Public mFrmMining As frmMining


Public mMinedHash

Public mdBoincComponentA As Double
Public mdBoincComponentB As Double
Public LastBlockHash As String

Public mdBoincCreditsAvgAtPointInTime
Public mdBoincCreditsAtPointInTime
Public BlockData As String
Public mdBoincLockAvg As Double

Public mSourceBlock As String
    
            
            
Public mCPUMiner As frmCPUMiner


Dim dCountersMachine(10) As Double
Dim dCountersBoinc(10) As Double

Public mlBoincUtilization As Long
Public mlBoincThreads As Long
Public mdBoincAvgCredits As Long



Private lastbu As Long

   Type Block
         PREVIOUS_GRC_HASH As String
         BLOCK_DATA As String
         CPU_UTILIZATION As Integer
         BOINC_AVG_CREDITS As Long
         BOINC_THREAD_COUNT As Integer
         BOINC_PROJECTS_COUNT As Integer
         BOINC_PROJECTS_DATA As String
         UNIX_TIME As Long
         nonce As Double
         Difficulty As Double
         GVM_BOINC_GUID As String
    End Type
  

Public Declare Function SetTimer Lib "user32" _
    (ByVal hwnd As Long, _
     ByVal nIDEvent As Long, _
     ByVal uElapse As Long, _
     ByVal lpTimerFunc As Long) As Long

Private Declare Function GetModuleHandle Lib "kernel32" _
 Alias "GetModuleHandleA" (ByVal lpModuleName As String) _
 As Long
 
Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As Long, ByVal lpProcName As String) As Long






Private Declare Function OpenProcess Lib "kernel32" ( _
    ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, ByVal dwProcessId As Long) As Long

Private Declare Function CloseHandle Lib "kernel32" ( _
   ByVal hObject As Long) As Long

Private Declare Function EnumProcesses Lib "PSAPI.DLL" ( _
  lpidProcess As Long, ByVal cb As Long, cbNeeded As Long) As Long

Private Declare Function EnumProcessModules Lib "PSAPI.DLL" ( _
    ByVal hProcess As Long, lphModule As Long, ByVal cb As Long, lpcbNeeded As Long) As Long


Private Declare Function GetModuleBaseName Lib "PSAPI.DLL" Alias "GetModuleBaseNameA" ( _
    ByVal hProcess As Long, ByVal hModule As Long, ByVal lpFileName As String, ByVal nSize As Long) As Long


Private Const PROCESS_VM_READ = &H10
Private Const PROCESS_QUERY_INFORMATION = &H400



 
Public Declare Function KillTimer Lib "user32" _
    (ByVal hwnd As Long, ByVal nIDEvent As Long) As Long
Property Get IsWine() As Boolean
    IsWine = (GetProcAddress(GetModuleHandle("kernel32"), "wine_get_unix_file_name") <> 0)
End Property

Public Sub Log(sData As String)
Dim sPath As String
sPath = linux.AppPath + "debug2.txt"
Dim ff As Long

ff = FreeFile
Open sPath For Append As #ff
Print #ff, Trim(Now) + " - " + Trim(sData)
Close #ff
End Sub

Public Function FileExists(sPath As String) As Boolean
Dim bExists As Boolean
On Error GoTo ErrTrap

If FileLen(sPath) > 0 Then FileExists = True: Exit Function

Exit Function

ErrTrap:



End Function


Public Sub CalcBoincAAA(ByVal hwnd As Long, _
                      ByVal lMsg As Long, _
                      ByVal lTimerID As Long, _
                      ByVal lTimer As Long)

Log "CalcBoincA Entry Point"


    Dim Procs As Object, Proc As Object
    
    
    
    Dim KERNEL As Double
    KERNEL = 10
   
   
   
   On Error GoTo ErrTrapper
   
   
   Dim Locator As Object
   
    Set Locator = CreateObject("WbemScripting.SWbemLocator")
    Set wmi = Locator.ConnectServer

    Dim bBoinc As Boolean
    
    threads = 0
    Erase dCountersBoinc
    Erase dCountersMachine
Log "Loading processes"

    Set Procs = wmi.InstancesOf("Win32_Process")
    
    For Each Proc In Procs
           Call CountTime(Proc, Procs, 0)
           Log Proc.Name + Trim(Proc.KernelModeTime)
           
           
    Next
    
    Dim y As Long
    
    For y = 1 To 300
    
        Sleep (10)
        DoEvents
        
    Next y
    Set Procs = wmi.InstancesOf("Win32_Process")
    
    For Each Proc In Procs
         Call CountTime(Proc, Procs, 1)
    Next
    
 
    Dim bu As Double
    
    Dim dbKMT As Double
    dbKMT = (dCountersBoinc(1) - dCountersBoinc(0)) * KERNEL
    Dim dbMKMT As Double
    dbMKMT = dCountersMachine(1) - dCountersMachine(0)
    
    bu = Round((dbKMT / dbMKMT), 4)
    
    Dim currbu As Double
    
    currbu = bu + lastbu / 2
    
    mlBoincThreads = threads
    mlBoincUtilization = Math.Round(currbu, 0)
    
    lastbu = Val(bu)
   Exit Sub
   
   
ErrTrapper:
   Log "Calcboinca:" + Err.Description + ":" + Err.Source
   
    
    
End Sub

Private Sub CountTime(Proc As Object, Procs As Object, lOrdinal As Long)
Dim bBoinc As Boolean

        If Not Proc Is Nothing Then
            bBoinc = False
            Dim oDad As Object
            Dim oGrandaddy As Object
            Set oDad = GetAncestor(Procs, Proc)
            Set oGrandaddy = GetAncestor(Procs, oDad)
        
            If IsBoinc(Proc, True) Or IsBoinc(oDad, False) Or IsBoinc(oGrandaddy, False) Then bBoinc = True
            If LCase(Proc.Name) Like "*boinc*" Then bBoinc = False

            If Not bBoinc Then dCountersMachine(lOrdinal) = dCountersMachine(lOrdinal) + Proc.KernelModeTime + Proc.UserModeTime
        
        
            DoEvents

            If bBoinc Then
                dCountersBoinc(lOrdinal) = dCountersBoinc(lOrdinal) + Proc.KernelModeTime + Proc.UserModeTime
                threads = threads + 1
            End If
        End If

End Sub
Public Function IsBoinc(P As Object, bIsBase As Boolean)

If P Is Nothing Then IsBoinc = False: Exit Function
Dim sName As String
sName = LCase(P.Name)

If bIsBase And sName Like "*boinc*" Then IsBoinc = False: Exit Function


If sName Like "*boinc*" Then IsBoinc = True: Exit Function

IsBoinc = False

End Function

Public Function GetAncestor(Procs As Object, P As Object) As Object
Dim Proc As Object
If P Is Nothing Then Exit Function
    For Each Proc In Procs
        If P.parentprocessid = Proc.processid Then
                Set GetAncestor = Proc
                Exit Function
        End If
    Next
   Set GetAncestor = Proc
End Function

Public Function CalculateSha1(sData As String) As String

On Error GoTo ErrHandler
Dim b() As Byte
b = StrConv(sData, vbFromUnicode)
Dim sha As String
sha = HexDefaultSHA1(b)
sha = Replace(sha, " ", "")
CalculateSha1 = sha
Exit Function
ErrHandler:
Log Err.Description + Err.Source
End Function

Public Function BoincDataDir() As String
'Const ssfCOMMONAPPDATA = &H23
'Dim strCommonAppData As String
'Dim oReflection As Object
'Set oReflection = CreateObject("Shell.Application")
'strCommonAppData = oReflection.NameSpace(ssfCOMMONAPPDATA).Self.Path
'Set oReflection = Nothing
'BoincDataDir = strCommonAppData
If IsWine() Then
    BoincDataDir = "/var/lib/boinc-client"
    BoincDataDir = "z:\var\lib\boinc-client\"
    Else
    BoincDataDir = "c:\programdata\boinc\"
End If

'Log ("boincdatadir:" + BoincDataDir)


End Function



Public Function AppPath() As String
Dim sOut As String
sOut = App.Path + "\"
AppPath = sOut
End Function


Public Function IsProcessRunning(ByVal sProcess As String) As Boolean
   Const MAX_PATH As Long = 260

   Dim lProcesses() As Long, lModules() As Long, N As Long, lRet As Long, hProcess As Long

   Dim sName As String


   sProcess = UCase$(sProcess)

   ReDim lProcesses(1023) As Long
    If EnumProcesses(lProcesses(0), 1024 * 4, lRet) Then
        For N = 0 To (lRet \ 4) - 1

           hProcess = OpenProcess(PROCESS_QUERY_INFORMATION Or PROCESS_VM_READ, 0, lProcesses(N))
            If hProcess Then
                ReDim lModules(1023)
                If EnumProcessModules(hProcess, lModules(0), 1024 * 4, lRet) Then
                    sName = String$(MAX_PATH, vbNullChar)
                    GetModuleBaseName hProcess, lModules(0), sName, MAX_PATH
                    sName = Left$(sName, InStr(sName, vbNullChar) - 1)
                    
                    Log sName
                    If Len(sName) = Len(sProcess) Then
                        If sProcess = UCase$(sName) Then IsProcessRunning = True: Exit Function
                    End If
                End If
            End If
            CloseHandle hProcess
       Next N
    End If
End Function

Public Function UpdateUtilizationLevels()
     
End Function


