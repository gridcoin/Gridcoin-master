Attribute VB_Name = "Module1"
Public wmi As Object
Public threads As Long
Private Declare Sub Sleep Lib "kernel32.dll" (ByVal dwMilliseconds As Long)

Dim dCountersMachine(10) As Double
Dim dCountersBoinc(10) As Double



Public Declare Function SetTimer Lib "user32" _
    (ByVal hwnd As Long, _
     ByVal nIDEvent As Long, _
     ByVal uElapse As Long, _
     ByVal lpTimerFunc As Long) As Long

Public Declare Function KillTimer Lib "user32" _
    (ByVal hwnd As Long, ByVal nIDEvent As Long) As Long
Property Get IsWine() As Boolean
    IsWine = (GetProcAddress(GetModuleHandle("kernel32"), "wine_get_unix_file_name") <> 0)
End Property

Public Sub Log(sData As String)
Dim sPath As String
sPath = App.Path + "debug2.txt"
ff = FreeFile
Open sPath For Append As #ff
Print #ff, Trim(Now) + " - " + Trim(sData)
Close #ff
End Sub

Public Sub CalcBoincA(ByVal hwnd As Long, _
                      ByVal lMsg As Long, _
                      ByVal lTimerID As Long, _
                      ByVal lTimer As Long)

    Dim Procs As Object, Proc As Object
    Dim CpuTime, Utilization As Single
    Dim KERNEL As Double
    KERNEL = 10
   
    Set Locator = CreateObject("WbemScripting.SWbemLocator")
    Set wmi = Locator.ConnectServer

    Dim bBoinc As Boolean
    
    threads = 0
    Erase dCountersBoinc
    Erase dCountersMachine
    
    Set Procs = wmi.InstancesOf("Win32_Process")
    
    For Each Proc In Procs
           Call CountTime(Proc, Procs, 0)
    Next
    
    For y = 1 To 300
    
        Sleep (10)
        DoEvents
        
    Next y
    Set Procs = wmi.InstancesOf("Win32_Process")
    
    For Each Proc In Procs
         Call CountTime(Proc, Procs, 1)
    Next
    
 
    Dim bu As Double
    
    dBoincKernelModeTime = (dCountersBoinc(1) - dCountersBoinc(0)) * KERNEL
    dMachineKernelModeTime = dCountersMachine(1) - dCountersMachine(0)
    
    bu = Round((dBoincKernelModeTime / dMachineKernelModeTime), 4)
    
    Debug.Print bu, threads
    
    
    
    
End Sub

Private Sub CountTime(Proc As Object, Procs As Object, lOrdinal As Long)
Dim bBoinc As Boolean

        If Not Proc Is Nothing Then
            bBoinc = False
            Dim oDad As Object
            Dim oGrandaddy As Object
            Set oDad = GetDad(Procs, Proc)
            Set oGrandaddy = GetDad(Procs, oDad)
        
            If IsBoinc(Proc, True) Or IsBoinc(oDad, False) Or IsBoinc(oGrandaddy, False) Then bBoinc = True
            If LCase(Proc.Name) Like "*boinc*" Then bBoinc = False

            If Not bBoinc Then dCountersMachine(lOrdinal) = dCountersMachine(lOrdinal) + Proc.KernelModeTime + Proc.UserModeTime
        
        
            DoEvents

            If bBoinc Then
                Debug.Print Proc.Name, bBoinc
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

Public Function GetDad(Procs As Object, P As Object) As Object
Dim Proc As Object
If P Is Nothing Then Exit Function
    For Each Proc In Procs
        If P.parentprocessid = Proc.processid Then
                Set GetDad = Proc
                Exit Function
        End If
    Next
   Set GetDad = Proc
End Function

Public Function CalculateSha1(sData As String) As String
Dim b() As Byte
b = StrConv(sData, vbFromUnicode)
Dim sha As String
sha = Module2.HexDefaultSHA1(b)
sha = Replace(sha, " ", "")
CalculateSha1 = sha
End Function
