VERSION 5.00
Begin VB.Form frmCPUMiner 
   Caption         =   "CPU Miner"
   ClientHeight    =   3030
   ClientLeft      =   120
   ClientTop       =   450
   ClientWidth     =   4560
   LinkTopic       =   "Form1"
   ScaleHeight     =   3030
   ScaleWidth      =   4560
   StartUpPosition =   3  'Windows Default
   Begin VB.Timer timerCPUMiner 
      Interval        =   1000
      Left            =   360
      Top             =   2520
   End
End
Attribute VB_Name = "frmCPUMiner"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

Option Explicit


Public nonce As Double
Public myStatus As Boolean


    Public KHPS As Double
    Public Elapsed As Long
    Public MinedHash As String
    Public SourceBlock As String
    Public LastSolvedHash As String
    
    Private MinerSlowCounter As Long
    Private sGVMGuidHash As String

    Private Function GetBlockTemplate(ByVal nonce As Double, ByVal Difficulty As Double) As Block
        Dim b As Block
        b.PREVIOUS_GRC_HASH = LastBlockHash
        b.BLOCK_DATA = BlockData
        b.CPU_UTILIZATION = linux.mlBoincUtilization
        b.BOINC_AVG_CREDITS = Left(Trim(linux.mdBoincAvgCredits), 4)
        b.BOINC_THREAD_COUNT = linux.mlBoincThreads
        b.BOINC_PROJECTS_COUNT = linux.mdBoincProjects
        b.BOINC_PROJECTS_DATA = linux.msBoincProjectData
        b.UNIX_TIME = UnixTimeStamp()
        b.nonce = nonce
        b.Difficulty = Difficulty
        GetBlockTemplate = b
    End Function
    Private Function BlockToGRCString(MiningBlock As Block) As String
     On Error GoTo ErrTrap
     
        Dim s As String
        Dim d As String: d = "\"
            s = MiningBlock.PREVIOUS_GRC_HASH + d + MiningBlock.BLOCK_DATA + d + Trim(MiningBlock.CPU_UTILIZATION) + d _
                + Format(Left(Trim(MiningBlock.BOINC_AVG_CREDITS), 3), "000") + d + Trim(MiningBlock.BOINC_THREAD_COUNT) _
                + d + Trim(MiningBlock.BOINC_PROJECTS_COUNT) + d _
                + Trim(MiningBlock.BOINC_PROJECTS_DATA) + d + Trim(MiningBlock.UNIX_TIME) _
                + d + Trim(MiningBlock.Difficulty) + d + Trim(MiningBlock.nonce)
           BlockToGRCString = s
        Exit Function
ErrTrap:
        Log (Err.Description)
        
    End Function
    Public Sub MineNewBlock()
    
  '      Dim thrMine As New System.Threading.Thread(AddressOf Mine)
   '     thrMine.IsBackground = True
    '    thrMine.Start()
    Mine
    
    End Sub
    Public Function ReturnGVMGuid(data As String) As String
    
    End Function
    Private Sub Mine()

    On Error GoTo ErrTrap
    
        KHPS = 0
        myStatus = True
        
        BlockData = Trim(VerifyBoincAuthenticity()) + ":" + Trim(msBoincMD5)
        
        Dim bHash() As Byte
        Dim cHash As String
        Dim startime As Date: startime = Now
        Dim stoptime As Date: stoptime = Now
        
        Dim diff As String
        Dim targetms As Long: targetms = 10000 'This will change as soon as we implement the Moore's Law equation
        diff = Trim(Math.Round(targetms / 5000, 0))
      
        nonce = 0
        Dim MiningBlock As Block
        Dim sSourceBlock As String
        Dim sStartingBlockHash As String: sStartingBlockHash = LastBlockHash
        MiningBlock = GetBlockTemplate(nonce, diff)

        Do While True
            nonce = nonce + 1
            If LastBlockHash <> sStartingBlockHash Then
                sGVMGuidHash = ReturnGVMGuid(ToBase64(LastBlockHash + ",CPUMiner," + UnixTimeStamp))
                MiningBlock.GVM_BOINC_GUID = sGVMGuidHash
                Exit Do 'New block detected on network
            End If
            MiningBlock = GetBlockTemplate(nonce, diff)
            sSourceBlock = BlockToGRCString(MiningBlock)
            cHash = linux.CalculateSha1(sSourceBlock)
            If (nonce Mod 1000) = 0 Then
                Elapsed = DateDiff("s", startime, Now)
                KHPS = Round((nonce / Elapsed) * 10, 0)
                
                DoEvents
                
                If Elapsed > 600 Then Exit Sub
            End If
                If Contains(cHash, Trim(diff)) And Contains(cHash, Format(MiningBlock.CPU_UTILIZATION, "000")) _
                    And Contains(cHash, Format(Left(Trim(MiningBlock.BOINC_AVG_CREDITS), 3), "000")) _
                    And Contains(cHash, Trim(Val(MiningBlock.BOINC_THREAD_COUNT))) Then
                    Elapsed = DateDiff("s", startime, Now)
                    MinedHash = cHash
                    SourceBlock = sSourceBlock
                    LastSolvedHash = MiningBlock.PREVIOUS_GRC_HASH
                    Dim sForensic As String
                    sForensic = sSourceBlock + "[~]" + cHash
                    Debug.Print sForensic
                    
                    Exit Do
                End If
        Loop
            myStatus = False
Exit Sub


ErrTrap:
            myStatus = False

    End Sub




Private Sub Form_Activate()
Me.Visible = False

End Sub

Private Sub Form_Load()
On Error GoTo ErrTrap
Me.Visible = False


Exit Sub

ErrTrap:


End Sub

Private Sub timerCPUMiner_Timer()
     If mlBoincThreads > 0 Then
            MinerSlowCounter = MinerSlowCounter + 1
            If MinerSlowCounter > 15000 Then MinerSlowCounter = 0: myStatus = False
            If myStatus = False Then
                If LastSolvedHash <> LastBlockHash Then
                    myStatus = True
                    Call MineNewBlock
                End If
            End If
        End If
   
End Sub


