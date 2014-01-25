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

Stop

x.ShowMiningConsole


Dim sResult As Long
sResult = x.CheckWork("1", "2", "3", "4", "555555555")

Debug.Print x.BoincThreads, x.BoincUtilization






End Sub

Private Sub Command2_Click()
Set x = New boinc.Utilization

End Sub

Private Sub Timer1_Timer()

End Sub
