
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
                'sb.Append(Mid(h, Len(h) - 2, 2))
                sb.Append(Microsoft.VisualBasic.Right(h, 2))



            Next
            Return sb.ToString
        Catch ex As Exception
            Return ""
        End Try
    End Function




    Public Function CreateBitcoinWallet() As String

        Dim h As String
        h = getSHA256Hash("Crapola.")
        h = getSHA256Hash("Crap")
        'Create a bitcoin wallet
        Dim kp As Casascius.Bitcoin.KeyPair
        kp = Casascius.Bitcoin.KeyPair.Create("salt", False)
        Dim privKeyHex As String
        privKeyHex = kp.PrivateKeyHex
        Dim pubKeyHex As String
        pubKeyHex = kp.PublicKeyHex
        Dim btcaddr As String

        Dim ab As New Casascius.Bitcoin.AddressBase(kp, 0)

        btcaddr = ab.AddressBase58
        Return btcaddr

    End Function

End Module
