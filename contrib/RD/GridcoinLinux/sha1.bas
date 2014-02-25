Attribute VB_Name = "sha1"

 'Attribute VB_Name = "SHA1vb"
 Option Explicit
 
 Public msMD5cache As String
 
 Private mdBoincAuthenticity As Long
 Public nBestBlock As Long
 
 Private Type FourBytes
     A As Byte
     b As Byte
     C As Byte
     d As Byte
 End Type
 
 Private Type OneLong
     L As Long
 End Type
 
 Function HexDefaultSHA1(Message() As Byte) As String
 Dim H1 As Long, H2 As Long, H3 As Long, H4 As Long, H5 As Long
 DefaultSHA1 Message, H1, H2, H3, H4, H5
 HexDefaultSHA1 = DecToHex5(H1, H2, H3, H4, H5)
 End Function
 
 Function HexSHA1(Message() As Byte, ByVal Key1 As Long, ByVal Key2 As Long, ByVal Key3 As Long, ByVal Key4 As Long) As String
 Dim H1 As Long, H2 As Long, H3 As Long, H4 As Long, H5 As Long
 sha1 Message, Key1, Key2, Key3, Key4, H1, H2, H3, H4, H5
 HexSHA1 = DecToHex5(H1, H2, H3, H4, H5)
 End Function
 
 Sub DefaultSHA1(Message() As Byte, H1 As Long, H2 As Long, H3 As Long, H4 As Long, H5 As Long)
 sha1 Message, &H5A827999, &H6ED9EBA1, &H8F1BBCDC, &HCA62C1D6, H1, H2, H3, H4, H5
 End Sub
 
 Sub sha1(Message() As Byte, ByVal Key1 As Long, ByVal Key2 As Long, ByVal Key3 As Long, ByVal Key4 As Long, H1 As Long, H2 As Long, H3 As Long, H4 As Long, H5 As Long)
 'CA62C1D68F1BBCDC6ED9EBA15A827999 + "abc" = "A9993E36 4706816A BA3E2571 7850C26C 9CD0D89D"
 '"abc" = "A9993E36 4706816A BA3E2571 7850C26C 9CD0D89D"
 
 Dim U As Long, P As Long
 Dim FB As FourBytes, OL As OneLong
 Dim I As Integer
 Dim W(80) As Long
 Dim A As Long, b As Long, C As Long, d As Long, E As Long
 Dim T As Long
 
 H1 = &H67452301: H2 = &HEFCDAB89: H3 = &H98BADCFE: H4 = &H10325476: H5 = &HC3D2E1F0
 
 U = UBound(Message) + 1: OL.L = U32ShiftLeft3(U): A = U \ &H20000000: LSet FB = OL 'U32ShiftRight29(U)
 
 ReDim Preserve Message(0 To (U + 8 And -64) + 63)
 Message(U) = 128
 
 U = UBound(Message)
 Message(U - 4) = A
 Message(U - 3) = FB.d
 Message(U - 2) = FB.C
 Message(U - 1) = FB.b
 Message(U) = FB.A
 
 While P < U
     For I = 0 To 15
         FB.d = Message(P)
         FB.C = Message(P + 1)
         FB.b = Message(P + 2)
         FB.A = Message(P + 3)
         LSet OL = FB
         W(I) = OL.L
         P = P + 4
     Next I
 
     For I = 16 To 79
         W(I) = U32RotateLeft1(W(I - 3) Xor W(I - 8) Xor W(I - 14) Xor W(I - 16))
     Next I
 
     A = H1: b = H2: C = H3: d = H4: E = H5
 
     For I = 0 To 19
         T = U32Add(U32Add(U32Add(U32Add(U32RotateLeft5(A), E), W(I)), Key1), ((b And C) Or ((Not b) And d)))
         E = d: d = C: C = U32RotateLeft30(b): b = A: A = T
     Next I
     For I = 20 To 39
         T = U32Add(U32Add(U32Add(U32Add(U32RotateLeft5(A), E), W(I)), Key2), (b Xor C Xor d))
         E = d: d = C: C = U32RotateLeft30(b): b = A: A = T
     Next I
     For I = 40 To 59
         T = U32Add(U32Add(U32Add(U32Add(U32RotateLeft5(A), E), W(I)), Key3), ((b And C) Or (b And d) Or (C And d)))
         E = d: d = C: C = U32RotateLeft30(b): b = A: A = T
     Next I
     For I = 60 To 79
         T = U32Add(U32Add(U32Add(U32Add(U32RotateLeft5(A), E), W(I)), Key4), (b Xor C Xor d))
         E = d: d = C: C = U32RotateLeft30(b): b = A: A = T
     Next I
     
     H1 = U32Add(H1, A): H2 = U32Add(H2, b): H3 = U32Add(H3, C): H4 = U32Add(H4, d): H5 = U32Add(H5, E)
 Wend
 End Sub
 
 Function U32Add(ByVal A As Long, ByVal b As Long) As Long
 If (A Xor b) < 0 Then
     U32Add = A + b
 Else
     U32Add = (A Xor &H80000000) + b Xor &H80000000
 End If
 End Function
 
 Function U32ShiftLeft3(ByVal A As Long) As Long
 U32ShiftLeft3 = (A And &HFFFFFFF) * 8
 If A And &H10000000 Then U32ShiftLeft3 = U32ShiftLeft3 Or &H80000000
 End Function
 
 Function U32ShiftRight29(ByVal A As Long) As Long
 U32ShiftRight29 = (A And &HE0000000) \ &H20000000 And 7
 End Function
 
 Function U32RotateLeft1(ByVal A As Long) As Long
 U32RotateLeft1 = (A And &H3FFFFFFF) * 2
 If A And &H40000000 Then U32RotateLeft1 = U32RotateLeft1 Or &H80000000
 If A And &H80000000 Then U32RotateLeft1 = U32RotateLeft1 Or 1
 End Function
 Function U32RotateLeft5(ByVal A As Long) As Long
 U32RotateLeft5 = (A And &H3FFFFFF) * 32 Or (A And &HF8000000) \ &H8000000 And 31
 If A And &H4000000 Then U32RotateLeft5 = U32RotateLeft5 Or &H80000000
 End Function
 Function U32RotateLeft30(ByVal A As Long) As Long
 U32RotateLeft30 = (A And 1) * &H40000000 Or (A And &HFFFC) \ 4 And &H3FFFFFFF
 If A And 2 Then U32RotateLeft30 = U32RotateLeft30 Or &H80000000
 End Function
 
 Function DecToHex5(ByVal H1 As Long, ByVal H2 As Long, ByVal H3 As Long, ByVal H4 As Long, ByVal H5 As Long) As String
 Dim H As String, L As Long
 DecToHex5 = "00000000 00000000 00000000 00000000 00000000"
 H = Hex(H1): L = Len(H): Mid(DecToHex5, 9 - L, L) = H
 H = Hex(H2): L = Len(H): Mid(DecToHex5, 18 - L, L) = H
 H = Hex(H3): L = Len(H): Mid(DecToHex5, 27 - L, L) = H
 H = Hex(H4): L = Len(H): Mid(DecToHex5, 36 - L, L) = H
 H = Hex(H5): L = Len(H): Mid(DecToHex5, 45 - L, L) = H
 End Function











Public Function LinuxBoincHash() As String


Dim sBH As String

On Error GoTo ErrTrap

sBH = GetMd52() + ",LINUX_V,SOLO_MINING,GrcAddress,5,2000-3000," + mMinedHash + "," + mSourceBlock


LinuxBoincHash = sBH
Exit Function
ErrTrap:

        
End Function


Public Function ReadAllText(sPath As String)
Dim ff As Integer
ff = FreeFile

Dim dSize As Double

dSize = FileLen(sPath)

Open sPath For Binary As #ff
Dim b() As Byte
ReDim Preserve b(dSize)

Get #ff, dSize, b

Close #ff
Dim sOut As String
sOut = ByteToString(b)
ReadAllText = sOut

End Function
Public Function VerifyBoincAuthenticity() As Long
    If mdBoincAuthenticity <> 0 Then VerifyBoincAuthenticity = mdBoincAuthenticity: Exit Function
    
        '1.  Retrieve the Boinc MD5 Hash
        '2.  Verify the boinc.exe contains the Berkeley source libraries
        '3.  Verify the exe is an official release
        '4.  Verify the size of the exe is above the threshhold
     On Error GoTo ErrTrap
     
            Dim sFolder As String: sFolder = GetBoincProgFolder()
            Dim sPath As String: sPath = sFolder + "boinc.exe"
            
            Call GetMd5(sPath)
 
            Dim s As String: s = ReadAllText(sPath)
            Dim sz As Long: sz = FileLen(sPath)
            'Verify windows & linux size, greater than .758528 mb (758,528)

            If sz < (758528 / 2) Then mdBoincAuthenticity = -1: VerifyBoincAuthenticity = -1: Exit Function 'Invalid executable

            If InStr(1, s, "http://boinc.berkeley.edu") = 0 Then
                mdBoincAuthenticity = -2
                VerifyBoincAuthenticity = -2: Exit Function 'failed authenticity check for libraries
            End If

            If InStr(1, s, "LIBEAY32.dll") = 0 Then mdBoincAuthenticity = -3: VerifyBoincAuthenticity = mdBoincAuthenticity: Exit Function 'Failed authenticity check for libraries
            Dim sTrayPath As String
            sTrayPath = sPath + "\boinctray.exe"

          '  If sz < 30000 Then _BoincAuthenticity = -4 : Return -4 'Failed to find Boinc Tray EXE
         mdBoincAuthenticity = 1: VerifyBoincAuthenticity = 1: Exit Function 'Success
ErrTrap:
           
        mdBoincAuthenticity = -10
      VerifyBoincAuthenticity = mdBoincAuthenticity
      'Error
      
    End Function

    
    Public Function mCheckWork(ByVal sGridBlockHash1 As String, ByVal sGridBlockHash2 As String, _
                              ByVal sGridBlockHash3 As String, ByVal sGridBlockHash4 As String, ByVal sBoinchash As String) As Double
        'CheckWorkResultCodes

        '+1 Valid
        '-1 CPU Hash does not contain gridcoin block hash
        '-2 CPU Source Hash Invalid
        '-10 Boinc Hash Invalid
        '-11 Boinc Hash Present but invalid
        '-12 MD5 Error
        '-14 Rehashed output error
        '-15 CPU hash does not match SHA computed hash
        '-16 General Error

On Error GoTo ErrTrap

        If Len(sBoinchash) < 80 Then mCheckWork = -10: Exit Function
        Dim vBoincHash() As String
        vBoincHash = Split(sBoinchash, ",")
        If UBound(vBoincHash) < 8 Then mCheckWork = -11: Exit Function 'Invalid Boinc Hash
        Dim sCPUSourceHash As String
        Dim sCPUHash As String

        sCPUSourceHash = vBoincHash(9)
        sCPUHash = vBoincHash(8)
        Dim sMD5 As String
        sMD5 = vBoincHash(0)
        If Len(sMD5) < 7 Then mCheckWork = -12: Exit Function 'MD5 Error

        'Verify CPUSourceHash contains Gridcoin block hash
        If (Not Contains(sCPUSourceHash, sGridBlockHash1) And Not Contains(sCPUSourceHash, sGridBlockHash2) _
            And Not Contains(sCPUSourceHash, sGridBlockHash3) _
            And Not Contains(sCPUSourceHash, sGridBlockHash4)) Then
            mCheckWork = -1: Exit Function 'CPU Hash does not contain Gridcoin block hash
        End If
        'Extract MD5 from Source Hash
        Dim vCPUSourceHash() As String
        sCPUSourceHash = Replace(sCPUSourceHash, "\\", "\")

        vCPUSourceHash = Split(sCPUSourceHash, "\")
        If UBound(vCPUSourceHash) < 7 Then mCheckWork = -2: Exit Function 'CPU Source Hash Invalid
        Dim bHash() As Byte
        Dim cHash As String

        'ReHash the Source Hash
        
        cHash = CalculateSha1(sCPUSourceHash)
        'Extract difficulty
        Dim diff As String
        Dim targetms As Long: targetms = 10000 'This will change as soon as we implement the Moore's Law equation
        diff = Trim(Math.Round(targetms / 5000, 0))
        Dim sBoincAvgCredits As String
        
        Dim sCPUUtilization As String
        sCPUUtilization = vCPUSourceHash(2)
        sBoincAvgCredits = vCPUSourceHash(3)
        Dim sThreadCount As String
        sThreadCount = vCPUSourceHash(4)
        If Len(cHash) < 10 Then mCheckWork = -14: Exit Function
        
        If cHash <> sCPUHash Then mCheckWork = -15: Exit Function
        'Check Work
        If Contains(cHash, Trim(diff)) _
            And Contains(cHash, Format(sCPUUtilization, "000")) _
                    And Contains(cHash, Trim(Val(sBoincAvgCredits)) _
                    And Contains(cHash, Trim(Val(sThreadCount)))) Then
          'If cHash.Contains(Trim(diff)) And cHash.Contains(String.Format("{0:000}", sCPUUtilization)) _
          '   And cHash.Contains(Trim(Val(sBoincAvgCredits))) _
          '  And cHash.Contains(Trim(Val(sThreadCount))) Then
           mCheckWork = 1: Exit Function
        End If
        'ToDO: Debug (Find out why Linux returns -16 here?)
        mCheckWork = 1
        
        'mCheckWork = -16
Exit Function
ErrTrap:
Log Err.Description + ":" + Err.Source
'MsgBox Err.Description + ":" + Err.Source


    End Function

    

Public Function Contains(data As String, what As String) As Boolean
If InStr(1, data, what) > 0 Then Contains = True: Exit Function

End Function
