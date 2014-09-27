
Imports System.Security.Cryptography
Imports Finisar.SQLite
Imports System.Text
Imports System.IO
Imports Microsoft.VisualBasic

Module modGridCoinShaFunctions

    Function getSHA256Hash(ByVal Txt As String) As String
        Try
            Dim sha As New SHA256Managed()
            Dim ae As New ASCIIEncoding()
            Dim Hash() As Byte = sha.ComputeHash(ae.GetBytes(Txt))
            Dim sb As New StringBuilder(Hash.Length * 2)
            Dim ndx As Integer
            Dim h As String

            For ndx = 0 To Hash.Length - 1
                h = "0" & Hex(Hash(ndx))
                sb.Append(Microsoft.VisualBasic.Right(h, 2))
            Next
            Return sb.ToString
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function CreateGRCKeyWithProjectAndAccount(ByVal sKey As String)
        Dim kp As Casascius.Bitcoin.KeyPair
        kp = Casascius.Bitcoin.KeyPair.Create(0, sKey, False, 37)
        Return kp.AddressBase58
    End Function
    Public Function CreateBitcoinWallet() As String

        Dim h As String
        h = getSHA256Hash("Test")

        'Create a bitcoin wallet
        Dim kp As Casascius.Bitcoin.KeyPair
        kp = Casascius.Bitcoin.KeyPair.Create(1, "none", False, 37)

        Dim s As String
        Dim sOut As String
        

        For zz = 1 To 10
            s = CreateGRCKeyWithProjectAndAccount(Trim(zz))

            sOut = sOut + s + vbCrLf


            Application.DoEvents()

        Next

        Stop




        Dim privKeyHex As String
        privKeyHex = kp.PrivateKeyHex
        Dim pubKeyHex As String
        pubKeyHex = kp.PublicKeyHex
        Dim btcaddr As String

        Dim ab As New Casascius.Bitcoin.AddressBase(kp, 1)


        btcaddr = ab.AddressBase58
        Return btcaddr

    End Function

End Module
