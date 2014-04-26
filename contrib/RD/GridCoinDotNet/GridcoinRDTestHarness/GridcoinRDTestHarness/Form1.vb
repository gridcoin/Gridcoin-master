
Imports boinc

Imports System.Net.HttpWebRequest
Imports System.Text
Imports System.IO
Imports System.Data
Imports System.Object
Imports System.Security.Cryptography


Imports System.Net

Public Class Form1

    'Public m As New Utilization



    Public Structure Crypt
        Dim Success As Double
        Dim [Return] As Object

    End Structure
    Public Structure Symbol
        Public marketid As String
        Public label As String
        Public lasttradeprice As Double
        Public volume As Double
        Public lasttradetime As String
        Public primaryname As String
        Public primarycode As String
        Public secondaryname As String
        Public secondarycode As String
        Public rencenttrades As Object
    End Structure
    Public Function ByteToString(b() As Byte)
        Dim sReq As String
        sReq = System.Text.Encoding.UTF8.GetString(b)
        Return sReq
    End Function
    Public Function RetrieveSiteSecurityInformation(sURL As String) As String
        Dim u As New Uri(sURL)
        Dim sp As ServicePoint = ServicePointManager.FindServicePoint(u)
        Dim groupName As String = Guid.NewGuid().ToString()
        Dim req As HttpWebRequest = TryCast(HttpWebRequest.Create(u), HttpWebRequest)
        req.ConnectionGroupName = groupName
        Try

        Using resp As WebResponse = req.GetResponse()
        End Using
        sp.CloseConnectionGroup(groupName)
        Dim key As Byte() = sp.Certificate.GetPublicKey()
        Dim sOut As String
        sOut = ByteArrayToHexString(key)
            Return sOut
        Catch ex As Exception
            'Usually due to either HTTP, 501, Not Implemented...etc.
            Return ""
        End Try

    End Function
    Public Function ValidatePoolURL(sURL As String) As Boolean
        Dim Pools(100) As String
        Pools(1) = "gridcoin.us,30 82 01 0a 02 82 01 01 00 e1 91 3f 65 da 2b cc de 81 10 be 21 bd 8a 22 00 c5 8d 5f d6 72 5d 1c 3c e4 0b 3a 03 c8 07 c1 e1 69 54 22 d3 ff 9e d7 55 55 c2 2e 62 bd 5c bc f5 3f 93 3d f1 2c 39 0b 66 04 a8 50 7e f5 19 ca 97 a5 99 02 0b 11 39 37 5e df a2 74 14 f1 ed be eb af 4b 53 c2 cc a9 ea 5f c0 0a cb 92 cf 7f 21 fc 96 4f 79 47 e9 15 97 58 65 ef 10 a3 3e 46 6a 1d 5b 34 ea ff 6d c6 10 08 b8 60 dd 40 d5 b3 43 73 96 70 9f ce f1 2c 3b 8e 09 e0 14 97 9e b3 c6 6c a2 d9 81 4d d4 71 f1 46 ae ec b9 cf 0b 59 bd 7a 85 88 48 0f aa fa 6e f5 1a 75 18 f0 c9 94 79 6c 8b 11 86 de 3f ab 76 62 77 99 5a c4 fb 10 79 35 3d 61 33 15 ed a8 0c ce 45 cd 3e fc 64 62 72 07 a2 05 b4 df 3c f8 97 7c f9 20 43 b6 93 c2 2a 67 b7 9c 64 36 2f 9f 2d c3 d1 82 1a 9c 85 bb 3f d6 b7 07 aa 23 a3 a9 6a 49 18 f1 46 b5 b3 11 b6 61 02 03 01 00 01"

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''Verify SSL Certificate'''''''''''''''''''''''''''
        Dim sHexSSLPublicKey As String = RetrieveSiteSecurityInformation(sURL)
        If Len(sHexSSLPublicKey) = 0 Then Return False

        For x As Integer = 0 To 100
            If Len(Pools(x)) > 0 Then
                Dim vPools() As String = Split(Pools(x), ",")
                If UBound(vPools) = 1 Then
                    Dim sPoolPublicKey As String = vPools(1)
                    If Len(sPoolPublicKey) > 0 Then
                        sPoolPublicKey = Replace(sPoolPublicKey, " ", "")
                        If LCase(sPoolPublicKey) = LCase(sHexSSLPublicKey) Then
                            Return True
                        End If
                    End If
                End If
            End If
        Next
        Return False

    End Function

    Public Function ByteArrayToHexString(ByVal ba As Byte()) As String
        Dim hex As StringBuilder
        hex = New StringBuilder(ba.Length * 2)
        For Each b As Byte In ba
            hex.AppendFormat("{0:x2}", b)
        Next
        Return hex.ToString()
    End Function


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim sOut As String
        Dim z As New Utilization

        sOut = z.Des3Decrypt(sOut)

        Stop




        sOut = Trim(False)


        Dim bResult As Boolean

        
        Stop

        z.ShowMiningConsole()
        Stop


        Exit Sub




        

    End Sub
    Public Sub Log(sData As String)
        Try
            Dim sPath As String
            sPath = "c:\cryptsy.txt"
            Dim sw As New System.IO.StreamWriter(sPath, True)
            sw.WriteLine(Trim(Now) + ", " + sData)
            sw.Close()
        Catch ex As Exception
        End Try

    End Sub
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim m As New Utilization
        m.UpdateLeaderBoard()

        Dim d As New Sql
        d.UpdateUserSummary()
        d = Nothing

    End Sub
End Class
