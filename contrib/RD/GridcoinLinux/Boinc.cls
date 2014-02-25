VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "LinuxUtilization"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Public msLinux10 As String
Private myTimer As Double

Public Function Version() As Double
Version = 9


End Function
Public Function ShowMiningConsole()
On Error GoTo ErrTrap

mLinuxGui.ShowMiningWindow
Exit Function

ErrTrap:

End Function


Private Sub Class_Initialize()
On Error GoTo ErrTrap


Log "Initializing"
Set mclsUtilization = Me
Set mCPUMiner = New frmCPUMiner
mCPUMiner.Show vbModal
Set mLinuxGui = CreateObject("GridcoinLinuxGUI.gui")
LogBoincCredits (True)
Log "Finished Initializing"

Exit Sub

ErrTrap:
Log Err.Description + ":" + Err.Source


End Sub
Public Function LogClientState()
Exit Function
Open BoincDataDir + "client_state.xml" For Input As #1
Dim sTemp As String
Do While EOF(1) = False
    Line Input #1, sTemp
    Log sTemp
Loop
Close #1
End Function
Private Sub Class_Terminate()
mLinuxGui.DestroyMiningWindow
Log "Exiting."

End Sub

Public Function ToSha1(sData As String) As String
ToSha1 = CalculateSha1(sData)

End Function

 Public Function BoincMD5() As String
 On Error GoTo ErrTrap
        BoincMD5 = GetMd52()
        Exit Function
ErrTrap:
        
 End Function
 Public Function SetLinux10(sData As String)
 msLinux10 = sData
 
 End Function
Public Function Linux10() As Double
Linux10 = -20

End Function

Public Function CheckWorkLinux(sData As String) As Integer
Log sData
CheckWorkLinux = -55
Exit Function
Dim vData() As String
vData = Split(sData, "[COL]")
If UBound(vData) < 4 Then CheckWorkLinux = -50: Exit Function
Dim dResult As Double
dResult = CheckWork(vData(0), vData(1), vData(2), vData(3), vData(4))
CheckWorkLinux = dResult

End Function
 Public Sub SetSqlBlock(ByVal data As String)
End Sub

Public Function RetrieveSqlHighBlock() As Long
RetrieveSqlHighBlock = 0
End Function

   Public Function CheckWork(ByVal sGRCHash1 As String, ByVal sGRCHash2 As String, _
            ByVal sGRCHash3 As String, ByVal sGRCHash4 As String, ByVal sBoinchash As String) As Double
            On Error GoTo ErrTrap
            Log "Checking work " + sGRCHash1
            CheckWork = Val(CInt(mCheckWork(sGRCHash1, sGRCHash2, sGRCHash3, sGRCHash4, sBoinchash)))
            Exit Function
ErrTrap:
            Log Err.Message
End Function
   
Public Property Get BoincAvgCredits() As Double
        BoincAvgCredits = linux.mdBoincAvgCredits
 End Property


Public Property Get BoincUtilization() As Double
        BoincUtilization = linux.mlBoincUtilization
        
End Property

Public Property Get BoincThreads() As Double
       BoincThreads = linux.mlBoincThreads
End Property
Public Property Get BoincProjects() As Double
    BoincProjects = linux.mdBoincProjects
End Property
Public Property Get BoincProjectData() As String
    BoincProjectData = linux.msBoincProjectData
    
End Property

 Public Function BoincAuthenticityString() As String
    
          BoincAuthenticityString = Trim(VerifyBoincAuthenticity)
          
          
End Function
   
Public Property Get BoincCreditsAvgAtPointInTime() As Double
        BoincCreditsAvgAtPointInTime = linux.mdBoincCreditsAvgAtPointInTime
        
End Property
Public Property Get BoincCreditsAtPointInTime() As Double
        BoincCreditsAtPointInTime = mdBoincCreditsAtPointInTime
        
End Property
Public Property Get BoincTotalCreditsAvg() As Double
    BoincTotalCreditsAvg = mBoincCreditsAvg
End Property
   
Public Property Get BoincTotalCredits() As Double
    BoincTotalCredits = mBoincCredits
End Property
   
  Public Function CPUPoW(ByVal sHash As String) As Double
  On Error GoTo ErrTrap
  CPUPoW = -5
  Exit Function
  
        Dim vHash() As String
        vHash = Split(sHash, ":")
        If UBound(vHash) <> 2 Then CPUPoW = -13: Exit Function
        Dim iProjectId As Integer
        Dim lUserId As Long
        Dim sGRCAddress As String
        iProjectId = vHash(0): lUserId = vHash(1): sGRCAddress = vHash(2)
        Dim dCredits As Double
        Dim sErr As String
        dCredits = APICPUPoW(iProjectId, lUserId, sGRCAddress)
        CPUPoW = dCredits
        
ErrTrap:
  
 End Function
Public Property Get MinedHash() As String
On Error GoTo ErrTrap

            MinedHash = mMinedHash
            Exit Function
            
ErrTrap:

  End Property
    Public Property Get SourceBlock() As String
        On Error GoTo ErrTrap
        SourceBlock = linux.mSourceBlock
        Exit Function
ErrTrap:
        
    End Property
    
    Public Function BoincDeltaOverTime() As String
        BoincDeltaOverTime = "0:0"
    End Function
 Public Sub SetLastBlockHash(ByVal data As String)
        linux.LastBlockHash = Trim(data)
        
    End Sub
    Public Sub SetPublicWalletAddress(ByVal data As String)
        msPublicWalletAddress = Trim(data)
    End Sub
    Public Sub SetBestBlock(ByVal nBlock As Long)
        nBestBlock = nBlock
    End Sub
    Public Sub SetNodes(sData As String)
    
    End Sub
    Public Function APICPUPoW(ByVal iProjectId As Integer, ByVal lUserId As Long, ByVal sGRCAddress As String) As Double
        'Implement HTTP API Checks
        'CPUPoW = sha1.CPUPoW(iProjectId, lUserId, sGRCAddress)
    End Function
    
    Public Property Get BoincProjectCount() As Double
        BoincProjectCount = linux.mdBoincProjects
    End Property
    Public Property Get BoincTotalHostAverageCredits() As Double
        BoincTotalHostAverageCredits = mBoincTotalHostAvg
    End Property
   
