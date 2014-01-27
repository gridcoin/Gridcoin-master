VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "gui"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True



Public BoincUtilization As Long
Public BoincProjectData As String
Public BoincProjects As Long
Public BoincAvgCredits As Long
Public Version As Long
Public BoincThreads As Long
Public mdBoincComponentA As Long
Public mdBoincComponentB As Long
Public KHPS As Long
Public MinedHash As String
Public CPUMinerStatus As Boolean
Public CPUMinerNonce As Double
Public msGuiMessage As String
Public LastBlockHash As String


Public Function ShowMiningWindow()
mFrmMining.Show
End Function

Public Function DestroyMiningWindow()
Set mFrmMining = Nothing
End
End Function


Private Sub Class_Initialize()
Set modGUI.mclsGui = Me
Set mFrmMining = New frmMining
End Sub


Public Property Let SetBoincUtilization(lBoincUtilization As Long)
             BoincUtilization = lBoincUtilization
End Property

Public Property Let SetBoincProjectData(msBoincProjectData As String)
    BoincProjectData = msBoincProjectData
End Property

