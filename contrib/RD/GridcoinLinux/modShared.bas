Attribute VB_Name = "modShared"
Option Explicit

Private Declare Function GetModuleHandle Lib "kernel32" _
 Alias "GetModuleHandleA" (ByVal lpModuleName As String) _
 As Long
 
Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As Long, ByVal lpProcName As String) As Long





Public Function GetMd5(sPath As String) As String
GetMd5 = Trim(FileLen(sPath)) & "FF" & Trim(FileLen(sPath))
If Len(GetMd5) < 7 Then GetMd5 = Trim(Timer) & "AA" & Trim(Timer) & "BB" & Trim(Timer) & "CC" & Trim(Timer) & "DD"
linux.msBoincMD5 = GetMd5

End Function



Function MyASC(OneChar)
  If OneChar = "" Then MyASC = 0 Else MyASC = Asc(OneChar)
End Function


Public Function GetMd52() As String
If Len(msMD5cache) > 0 Then GetMd52 = msMD5cache: Exit Function

            Dim sFolder As String: sFolder = GetBoincProgFolder()
            Dim sPath As String:
            If IsWine Then
                sPath = sFolder + "boinc"
                Else
                sPath = sFolder + "boinc.exe"
                End If
            GetMd52 = GetMd5(sPath)
 
End Function

  Public Function ReturnBoincCreditsAtPointInTime(ByVal lLookbackSecs) As Double
       On Error GoTo ErrTrap
       
            Dim dtEnd As Date
            dtEnd = Now
            Dim dtStart As Date
            dtStart = DateAdd("s", -lLookbackSecs, dtEnd)
            Dim sPath As String
            sPath = BoincDataDir
            sPath = sPath + "gridcoin.dat"
            
            Dim iFF As Integer
            iFF = FreeFile
            Open sPath For Input As #iFF
            Dim sTemp As String
            Dim dTotalCreditsStart As Double
            Dim dTotalAvgStart As Double
            Dim dProjects As Double
            Dim dtEntry As Date
            Dim iRows As Long
            
            Do While Not EOF(iFF)
                Line Input #iFF, sTemp
                
                sTemp = FromBase64(sTemp)
                Dim vTemp() As String
                vTemp = Split(sTemp, ",")
                iRows = iRows + 1
                
                If UBound(vTemp) > 3 Then
                    If vTemp(1) = "TOTAL" Then
                        dtEntry = vTemp(0)
                        If dtEntry > dtStart And dTotalCreditsStart = 0 Then
                            dTotalCreditsStart = vTemp(2)
                            dProjects = vTemp(3)
                        End If
                    End If

                    If vTemp(1) = "AVG" Then
                        dtEntry = vTemp(0)
                        If dtEntry > dtStart And dTotalAvgStart = 0 Then
                            dTotalAvgStart = vTemp(2)
                            
                            
                        End If
                    End If
                    
                    If dtEntry > dtStart And dTotalAvgStart > 0 And dTotalCreditsStart > 0 Then Exit Do
                    
                End If
                DoEvents
                
            Loop
            Close #iFF
            
            mdBoincProjects = dProjects
            If mdBoincProjects < 0 Then mdBoincProjects = 0
            mdBoincCreditsAvgAtPointInTime = dTotalAvgStart
            mBoincCreditsAvg = dTotalAvgStart
            
            mdBoincCreditsAtPointInTime = dTotalCreditsStart
            mBoincCredits = dTotalCreditsStart
            
            ReturnBoincCreditsAtPointInTime = dTotalAvgStart
           
           Exit Function
ErrTrap:

           
    End Function

Public Function GetBoincProgFolder()
Dim sPath As String
'1-20-2014
If IsWine() Then
    GetBoincProgFolder = "z:\usr\bin\"
    Else
    GetBoincProgFolder = "c:\program files\boinc\"
End If

End Function

Public Function BoincDataDir() As String
'Const ssfCOMMONAPPDATA = &H23
'Dim strCommonAppData As String
'Dim oReflection As Object
'Set oReflection = CreateObject("Shell.Application")
'strCommonAppData = oReflection.NameSpace(ssfCOMMONAPPDATA).Self.Path
'Set oReflection = Nothing
'BoincDataDir = strCommonAppData
If IsWine() Then
    BoincDataDir = "/var/lib/boinc-client"
    BoincDataDir = "z:\var\lib\boinc-client\"
    Else
    BoincDataDir = "c:\programdata\boinc\"
End If
End Function
Public Function ToBase64(sData As String) As String
ToBase64 = Base64Encode(sData)
End Function

Public Function FromBase64(sData As String) As String
FromBase64 = Base64Decode(sData)
End Function

Property Get IsWine() As Boolean
    IsWine = (GetProcAddress(GetModuleHandle("kernel32"), "wine_get_unix_file_name") <> 0)
End Property





Function Base64Encode(inData)
  'rfc1521
  '2001 Antonin Foller, Motobit Software, http://Motobit.cz
  Const Base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
  Dim cOut, sOut, I
  
  'For each group of 3 bytes
  For I = 1 To Len(inData) Step 3
    Dim nGroup, pOut, sGroup
    
    'Create one long from this 3 bytes.
    nGroup = &H10000 * Asc(Mid(inData, I, 1)) + _
      &H100 * MyASC(Mid(inData, I + 1, 1)) + MyASC(Mid(inData, I + 2, 1))
    
    'Oct splits the long To 8 groups with 3 bits
    nGroup = Oct(nGroup)
    
    'Add leading zeros
    nGroup = String(8 - Len(nGroup), "0") & nGroup
    
    'Convert To base64
    pOut = Mid(Base64, CLng("&o" & Mid(nGroup, 1, 2)) + 1, 1) + _
      Mid(Base64, CLng("&o" & Mid(nGroup, 3, 2)) + 1, 1) + _
      Mid(Base64, CLng("&o" & Mid(nGroup, 5, 2)) + 1, 1) + _
      Mid(Base64, CLng("&o" & Mid(nGroup, 7, 2)) + 1, 1)
    
    'Add the part To OutPut string
    sOut = sOut + pOut
    
    'Add a new line For Each 76 chars In dest (76*3/4 = 57)
    'If (I + 2) Mod 57 = 0 Then sOut = sOut + vbCrLf
  Next
  Select Case Len(inData) Mod 3
    Case 1: '8 bit final
      sOut = Left(sOut, Len(sOut) - 2) + "=="
    Case 2: '16 bit final
      sOut = Left(sOut, Len(sOut) - 1) + "="
  End Select
  Base64Encode = sOut
End Function




Function Base64Decode(ByVal base64String)
  'rfc1521
  '1999 Antonin Foller, Motobit Software, http://Motobit.cz
  Const Base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
  Dim dataLength, sOut, groupBegin
  
  'remove white spaces, If any
  base64String = Replace(base64String, vbCrLf, "")
  base64String = Replace(base64String, vbTab, "")
  base64String = Replace(base64String, " ", "")
  
  'The source must consists from groups with Len of 4 chars
  dataLength = Len(base64String)
  If dataLength Mod 4 <> 0 Then
    Err.Raise 1, "Base64Decode", "Bad Base64 string."
    Exit Function
  End If

  
  ' Now decode each group:
  For groupBegin = 1 To dataLength Step 4
    Dim numDataBytes, CharCounter, thisChar, thisData, nGroup, pOut
    ' Each data group encodes up To 3 actual bytes.
    numDataBytes = 3
    nGroup = 0

    For CharCounter = 0 To 3
      ' Convert each character into 6 bits of data, And add it To
      ' an integer For temporary storage.  If a character is a '=', there
      ' is one fewer data byte.  (There can only be a maximum of 2 '=' In
      ' the whole string.)

      thisChar = Mid(base64String, groupBegin + CharCounter, 1)

      If thisChar = "=" Then
        numDataBytes = numDataBytes - 1
        thisData = 0
      Else
        thisData = InStr(1, Base64, thisChar, vbBinaryCompare) - 1
      End If
      If thisData = -1 Then
        Err.Raise 2, "Base64Decode", "Bad character In Base64 string."
        Exit Function
      End If

      nGroup = 64 * nGroup + thisData
    Next
    
    'Hex splits the long To 6 groups with 4 bits
    nGroup = Hex(nGroup)
    
    'Add leading zeros
    nGroup = String(6 - Len(nGroup), "0") & nGroup
    
    'Convert the 3 byte hex integer (6 chars) To 3 characters
    pOut = Chr(CByte("&H" & Mid(nGroup, 1, 2))) + _
      Chr(CByte("&H" & Mid(nGroup, 3, 2))) + _
      Chr(CByte("&H" & Mid(nGroup, 5, 2)))
    
    'add numDataBytes characters To out string
    sOut = sOut & Left(pOut, numDataBytes)
  Next

  Base64Decode = sOut
End Function



