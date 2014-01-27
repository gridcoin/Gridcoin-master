Attribute VB_Name = "modGUI"

Public msBoincMd5 As String
Public mdBoincProjects As Double
Public mdBoincCreditsAvgAtPointInTime As Double
Public mBoincCreditsAvg As Double
Public mdBoincCreditsAtPointInTime             As Double
Public mBoincCredits              As Double

Public mFrmMining As frmMining
Public mclsGui As GridcoinLinuxGUI.gui

Public Sub Log(sData As String)
Dim sPath As String
sPath = AppPath + "debuggui.txt"
Dim ff As Long
ff = FreeFile
Open sPath For Append As #ff
Print #ff, Trim(Now) + " - " + Trim(sData)
Close #ff
End Sub
            
           
