VERSION 5.00
Object = "{65E121D4-0C60-11D2-A9FC-0000F8754DA1}#2.0#0"; "MSCHRT20.OCX"
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.1#0"; "mscomctl.OCX"
Begin VB.Form frmMining 
   BackColor       =   &H00000000&
   Caption         =   "Gridcoin Mining 1.12"
   ClientHeight    =   9645
   ClientLeft      =   120
   ClientTop       =   450
   ClientWidth     =   14175
   ForeColor       =   &H0000FF00&
   Icon            =   "frmMining.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   9645
   ScaleWidth      =   14175
   StartUpPosition =   3  'Windows Default
   Begin VB.Timer timerCloser 
      Enabled         =   0   'False
      Interval        =   55555
      Left            =   12360
      Top             =   6000
   End
   Begin VB.Timer TimerOneMinute 
      Interval        =   65535
      Left            =   11520
      Top             =   120
   End
   Begin VB.Timer TimerBoinc 
      Interval        =   1000
      Left            =   12480
      Top             =   0
   End
   Begin VB.CommandButton cmdTestMiner 
      BackColor       =   &H00000000&
      Caption         =   "Test CPUMiner"
      Height          =   195
      Left            =   12960
      MaskColor       =   &H0000FF00&
      Style           =   1  'Graphical
      TabIndex        =   3
      Top             =   480
      UseMaskColor    =   -1  'True
      Width           =   135
   End
   Begin MSComctlLib.ProgressBar pbCPUMiner 
      Height          =   75
      Left            =   1080
      TabIndex        =   2
      Top             =   3360
      Width           =   12135
      _ExtentX        =   21405
      _ExtentY        =   132
      _Version        =   393216
      Appearance      =   0
   End
   Begin MSChart20Lib.MSChart chartCredits 
      Height          =   2655
      Left            =   3600
      OleObjectBlob   =   "frmMining.frx":0442
      TabIndex        =   1
      Top             =   480
      Width           =   9495
   End
   Begin MSChart20Lib.MSChart chartUtilization 
      Height          =   2535
      Left            =   1080
      OleObjectBlob   =   "frmMining.frx":260C
      TabIndex        =   0
      Top             =   480
      Width           =   2175
   End
End
Attribute VB_Name = "frmMining"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private mLabels(20) As Label
Private mLabelValues(20) As Label

Private Sub cmdHide_Click()
Me.Visible = False

End Sub

Private Sub cmdTestMiner_Click()

mclsGui.msGuiMessage = "TESTCPUMINER"


End Sub
Public Function Scribe(data As String, sSuffix As String) As String
Dim sName As String
 sName = "lbl" + sSuffix + Replace(data, " ", "_")
 Scribe = sName
End Function
Public Function UndoScribe(data As String) As String
Dim sName As String
 sName = Replace(data, "_", " ")
 UndoScribe = sName
End Function

Private Sub Form_Load()

DoEvents

 
         '   Dim sPath As String
         '   sPath = linux.BoincDataDir + "gridcoin.dat"
         '   Open sPath For Output As #1
         '   Close #1
            
   
'Create the labels
Dim sLabels As String
sLabels = "Version|Boinc Utilization|Boinc Thread Count|Boinc Component A|Boinc Component B|Boinc Avg Credits|Boinc MD5|Boinc Project Data" _
& "|Boinc KHPS|Last Block|Last Solved Hash"
Dim vLabels() As String
vLabels = Split(sLabels, "|")
msMD5 = md52


For I = 0 To UBound(vLabels)
    Dim L As VB.Label
    Set L = Controls.Add("VB.Label", Scribe(vLabels(I), ""), Me)
     

  Set mLabels(I) = L
    L.Caption = vLabels(I) + ": "
    L.Visible = True
    L.Left = Me.pbCPUMiner.Left
    L.Font.Size = 14
    L.Font.Name = "Arial"
    L.BackColor = 0
    L.ForeColor = &HFF00&
    
    
    L.Top = pbCPUMiner.Top + ((I + 1) * 450)
    L.Width = 2600
    
    
    Dim l1 As VB.Label
    Set l1 = Controls.Add("VB.Label", Scribe(vLabels(I), "Value"), Me)
    
    Set mLabelValues(I) = l1
    l1.Caption = ". . . "
    l1.Visible = True
    l1.Font.Size = 14
    l1.Font.Name = "Arial"
    
    l1.Left = L.Left + L.Width + 3300
    l1.Top = pbCPUMiner.Top + ((I + 1) * 450)
    l1.Width = 7000
    l1.BackColor = 0
    l1.ForeColor = &HFF00&
   
    
Next I

Call TimerOneMinute_Timer

End Sub

Public Sub SetLabel(sName As String, sValue As String)

Dim sCN As String
sCN = Scribe(sName, "Value")

For I = 0 To UBound(mLabelValues)
 If TypeName(mLabelValues(I)) = "Label" Then
  If LCase(mLabelValues(I).Name) = LCase(sCN) Then
    mLabelValues(I).Caption = sValue
  End If
  End If
  
Next I
End Sub

Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
Cancel = True
Me.Visible = False


End Sub

Private Sub TimerBoinc_Timer()

Call SetLabel("boinc khps", Trim(mclsGui.KHPS))

Call SetLabel("last solved hash", Trim(mclsGui.MinedHash))

Call SetLabel("last block", Trim(mclsGui.LastBlockHash))
If mclsGui.CPUMinerStatus = True Then
 If mclsGui.CPUMinerNonce > pbCPUMiner.Max Then pbCPUMiner.Max = mclsGui.CPUMinerNonce * 1.5
    pbCPUMiner.Value = mclsGui.CPUMinerNonce
    
    pbCPUMiner.Visible = True
    Else
    pbCPUMiner.Visible = False
    
End If

Call SetLabel("boinc utilization", Trim(mclsGui.BoincUtilization))
Call SetLabel("boinc project data", Trim(mclsGui.BoincProjectData))
Call SetLabel("boinc projects", Trim(mclsGui.BoincProjects))
Call SetLabel("boinc avg credits", GlobalizedDecimal(mclsGui.BoincAvgCredits))
Call SetLabel("Version", Trim(mclsGui.Version))
Call SetLabel("Boinc Thread Count", Trim(mclsGui.BoincThreads))
Call SetLabel("boinc component A", GlobalizedDecimal(mclsGui.mdBoincComponentA))
Call SetLabel("boinc component B", GlobalizedDecimal(mclsGui.mdBoincComponentB))

End Sub
Private Function GlobalizedDecimal(data As Variant)
Dim sOut As String
sOut = Trim(Math.Round(Val(data), 2))

GlobalizedDecimal = sOut
End Function

Private Sub timerCloser_Timer()
Me.Hide

End Sub

Private Sub TimerOneMinute_Timer()

Call SetLabel("boinc utilization", Trim(mclsGui.BoincUtilization))
Call SetLabel("boinc project data", Trim(mclsGui.BoincProjectData))
Call SetLabel("boinc projects", Trim(mclsGui.BoincProjects))
Call SetLabel("boinc avg credits", GlobalizedDecimal(mclsGui.BoincAvgCredits))
Call SetLabel("Version", Trim(mclsGui.Version))
Call SetLabel("Boinc Thread Count", Trim(mclsGui.BoincThreads))
Call SetLabel("boinc component A", GlobalizedDecimal(mclsGui.mdBoincComponentA))
Call SetLabel("boinc component B", GlobalizedDecimal(mclsGui.mdBoincComponentB))
Call SetLabel("boinc Md5", GetMd52)

UpdateCharts


End Sub
Private Sub UpdateCharts()

On Error GoTo ErrTrap
  Dim sPath As String
  sPath = BoincDataDir
  sPath = sPath + "gridcoin.dat"
  
  FileCopy sPath, sPath + ".bak"
  
          


chartCredits.chartType = VtChChartType2dLine

chartCredits.ColumnCount = 3

Dim arrValues(1 To 30, 1 To 4)


Dim I As Integer
  
Dim x As Long
Dim xtop As Long
xtop = 30
    
Dim lookback As Double
            For x = xtop To 1 Step -1.2
            
                DoEvents
                
                lookback = x * 3600 * 24
                ReturnBoincCreditsAtPointInTime (lookback)
                Dim l1 As Double
                Dim l2 As Double
                Dim l3 As Double
                l1 = BoincCreditsAvgAtPointInTime
                ReturnBoincCreditsAtPointInTime (lookback - (3600# * 24#))
                l2 = BoincCreditsAtPointInTime
                l3 = Math.Abs(l2 - last_total)
                last_total = l2
                If l3 > (l1 * 5) Then l3 = l1
                dProj = BoincProjects
                Dim d1 As Date
                d1 = DateAdd("d", -x, Now)
                arrValues(x, 1) = Format(d1, "mm/dd/yyyy")
                arrValues(x, 2) = l3
                arrValues(x, 3) = l2
               
                arrValues(x, 4) = dProj * (l1 / 10)
              
            Next x

  
  
chartCredits.ChartData = arrValues
chartCredits.Column = 1
chartCredits.ColumnLabel = "Avg Credits"
chartCredits.Column = 2
chartCredits.ColumnLabel = "Daily Credits"
chartCredits.Column = 3
chartCredits.ColumnLabel = "Projects"
chartUtilization.Column = 1
chartUtilization.data = mclsGui.BoincUtilization
chartUtilization.Column = 2
chartUtilization.data = 100 - mclsGui.BoincUtilization

Exit Sub

ErrTrap:
End Sub




