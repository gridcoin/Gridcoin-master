VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Security"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True

   Public Function CheckWork(ByVal sGRCHash1 As String, ByVal sGRCHash2 As String, _
            ByVal sGRCHash3 As String, ByVal sGRCHash4 As String, ByVal sBoinchash As String) As Double
            On Error GoTo ErrTrap
            Log "Checking work " + sGRCHash1
            CheckWork = Val(CInt(mCheckWork(sGRCHash1, sGRCHash2, sGRCHash3, sGRCHash4, sBoinchash)))
            Exit Function
ErrTrap:
            Log Err.Message
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

Public Function Version() As Double
Version = 8

End Function
