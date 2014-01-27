VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   3030
   ClientLeft      =   120
   ClientTop       =   450
   ClientWidth     =   4560
   LinkTopic       =   "Form1"
   ScaleHeight     =   3030
   ScaleWidth      =   4560
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton Command2 
      Caption         =   "Command2"
      Height          =   195
      Left            =   2160
      TabIndex        =   1
      Top             =   2280
      Width           =   855
   End
   Begin VB.CommandButton Command1 
      Caption         =   "Command1"
      Height          =   495
      Index           =   0
      Left            =   1080
      TabIndex        =   0
      Top             =   840
      Width           =   1215
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Public x As New boinc.Utilization

Private Sub Command1_Click(Index As Integer)
'Figure out sha

Dim z As Long
z = x.version


x.ShowMiningConsole


Dim sResult As Long
Dim sHash As String
sHash = "17134a21fe25d39dcb6afcf544aab8cc3748c1f1e250efd2acf55738fa6dd3a3"
Dim sBoinc As String
sBoinc = "3a94913164b731f5c712e4a7852575a3,1,100,CRD_V,SOLO_MINING,Fv9pvrJ5UPFseMXvfpFFYMHHhnH69nuF2L,48,3357:3125:2826,E2C9999BA1006308E45ED23381C44DF02108D2EF,17134a21fe25d39dcb6afcf544aab8cc3748c1f1e250efd2acf55738fa6dd3a3\\1:3a94913164b731f5c712e4a7852575a3\\100\\3381\\2\\842692\\roset:fight\\1388417242\\2\\322334"

sResult = x.CheckWork(sHash, sHash, sHash, sHash, sBoinc)


Debug.Print x.BoincThreads, x.BoincUtilization






End Sub

Private Sub Command2_Click()
Set x = New boinc.Utilization

End Sub

Private Sub Timer1_Timer()

End Sub
