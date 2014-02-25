VERSION 5.00
Object = "{65E121D4-0C60-11D2-A9FC-0000F8754DA1}#2.0#0"; "MSCHRT20.OCX"
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.1#0"; "mscomctl.OCX"
Begin VB.Form frmMining 
   BackColor       =   &H00000000&
   Caption         =   "Gridcoin Mining 1.12"
   ClientHeight    =   7485
   ClientLeft      =   120
   ClientTop       =   450
   ClientWidth     =   12885
   ForeColor       =   &H0000FF00&
   Icon            =   "frmMining.frx":0000
   LinkTopic       =   "Form1"
   ScaleHeight     =   7485
   ScaleWidth      =   12885
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdRefresh 
      BackColor       =   &H00404040&
      Caption         =   "Refresh"
      Height          =   375
      Left            =   10800
      MaskColor       =   &H00000000&
      Style           =   1  'Graphical
      TabIndex        =   4
      Top             =   3720
      Width           =   1215
   End
   Begin VB.Timer timerCloser 
      Enabled         =   0   'False
      Interval        =   55555
      Left            =   5520
      Top             =   240
   End
   Begin VB.Timer TimerOneMinute 
      Interval        =   65535
      Left            =   9000
      Top             =   240
   End
   Begin VB.Timer TimerBoinc 
      Interval        =   1000
      Left            =   9840
      Top             =   120
   End
   Begin VB.CommandButton cmdTestMiner 
      BackColor       =   &H00000000&
      Caption         =   "Test CPUMiner"
      Height          =   195
      Left            =   0
      MaskColor       =   &H0000FF00&
      Style           =   1  'Graphical
      TabIndex        =   3
      Top             =   0
      UseMaskColor    =   -1  'True
      Width           =   135
   End
   Begin MSComctlLib.ProgressBar pbCPUMiner 
      Height          =   75
      Left            =   1080
      TabIndex        =   2
      Top             =   3360
      Width           =   10935
      _ExtentX        =   19288
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
      Width           =   8415
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
Option Explicit

Private mLabels(20) As Label
Private mLabelValues(20) As Label
Private mYPosition As Long

Private Sub cmdHide_Click()
Me.Visible = False

End Sub

Private Sub cmdRefresh_Click()
 Call TimerOneMinute_Timer
 


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
Private Sub DrawComposite(sCaption As String, sValue As String, iOrdinal As Long, vLabels() As String, i As Integer)

    Dim lHeight As Long
    lHeight = 400
    
    mYPosition = pbCPUMiner.Top + (iOrdinal * lHeight)
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Caption
    
    Dim L As VB.Label
    Set L = Controls.Add("VB.Label", sCaption, Me)
    Set mLabels(i) = L
    L.Caption = vLabels(i) + ": "
    L.Visible = True
    L.Left = Me.pbCPUMiner.Left
    L.Font.Size = 10
    L.Font.Name = "Arial"
    L.BackColor = 0
    L.ForeColor = &HFF00&
    L.Top = mYPosition
    L.Height = lHeight - 50
    L.Width = 2400
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''Value:
    ''''''''
    Dim l1 As VB.Label
    Set l1 = Controls.Add("VB.Label", sValue, Me)
    Set mLabelValues(i) = l1
    l1.Caption = ". . . "
    l1.Visible = True
    l1.Font.Size = 10
    l1.Font.Name = "Arial"
    l1.Left = L.Left + L.Width + 3000
    l1.Top = mYPosition
    l1.Height = lHeight - 50
    
    l1.Width = 6000
    l1.BackColor = 0
    l1.ForeColor = &HFF00&
  
    Me.Height = l1.Top + 1200
    
    
End Sub
Private Sub Form_Load()

DoEvents

   
'Create the labels
Dim sLabels As String
sLabels = "Version|Boinc Utilization|Boinc Thread Count|Boinc Component A|Boinc Component B|Boinc Avg Credits|Boinc MD5|Boinc Project Data" _
& "|Boinc KHPS|Last Block|Last Solved Hash"
Dim vLabels() As String
vLabels = Split(sLabels, "|")

Dim i As Integer

For i = 0 To UBound(vLabels)
    Call DrawComposite(Scribe(vLabels(i), ""), Scribe(vLabels(i), "Value"), i + 1, vLabels, Val(i))
Next i

Call TimerOneMinute_Timer
Me.Show
Me.SetFocus
Me.Refresh


End Sub

Public Sub SetLabel(sName As String, sValue As String)

Dim sCN As String
sCN = Scribe(sName, "Value")
Dim i As Integer

For i = 0 To UBound(mLabelValues)
 If TypeName(mLabelValues(i)) = "Label" Then
  If LCase(mLabelValues(i).Name) = LCase(sCN) Then
    mLabelValues(i).Caption = sValue
  End If
  End If
  
Next i
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
  sPath = sPath + "gridcoin2.dat"
   
    chartCredits.chartType = VtChChartType2dLine
    chartCredits.ColumnCount = 3
    Dim arrValues(1 To 30, 1 To 4)


Dim i As Integer
Dim x As Long
Dim xtop As Long
xtop = 30
Log "Traversing back to n-30"
Dim last_total As Double
Dim dProj As Double

Dim lookback As Double
            For x = xtop To 1 Step -1
                DoEvents
                lookback = x * 3600 * 24
                ReturnBoincCreditsAtPointInTime (lookback)
                Dim l1 As Double
                Dim l2 As Double
                Dim l3 As Double
                l1 = mdBoincCreditsAvgAtPointInTime
                
                ReturnBoincCreditsAtPointInTime (lookback - (3600# * 24#))
                l2 = mdBoincCreditsAtPointInTime
                                
                l3 = Math.Abs(l2 - last_total)
                last_total = l2
                If l3 > (l1 * 5) Then l3 = l1
                dProj = mdBoincProjects
                
                Dim d1 As Date
                d1 = DateAdd("d", -x, Now)
                Log "credits " + Trim(l1) + "," + Trim(l2) + "," + Trim(l3)
                
                
                arrValues(x, 1) = Format(d1, "mm/dd/yyyy")
                arrValues(x, 2) = l3
                arrValues(x, 3) = l2
                Log Trim(dProj)
                
                arrValues(x, 4) = dProj * (l1 / 10)
                Log "Charting " + Trim(x)
            Next x

    
chartCredits.ChartData = arrValues
chartCredits.Column = 1
chartCredits.ColumnLabel = "Avg Credits"
chartCredits.Column = 2
chartCredits.ColumnLabel = "Daily Credits"
chartCredits.Column = 3
chartCredits.ColumnLabel = "Projects"
Log "Setting utilization @1"

chartUtilization.Column = 1

chartUtilization.data = mclsGui.BoincUtilization
chartUtilization.Column = 2
Log "Updating Utilization 100-bu"

chartUtilization.data = 100 - mclsGui.BoincUtilization
chartUtilization.Refresh

Exit Sub

ErrTrap:
Log "Update Charts:" + Err.Description
End Sub




