Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Casascius.Bitcoin
    Public Class Base58
        ''' <summary>
        ''' Converts a base-58 string to a byte array, returning null if it wasn't valid.
        ''' </summary>
        Public Shared Function ToByteArray(ByVal base58 As String) As Byte()
            Dim bi2 As New Org.BouncyCastle.Math.BigInteger("0")
            Dim b58 As String = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"
            Dim sOut As String = "hi"

            For Each c As Char In base58
                If b58.IndexOf(c) <> -1 Then
                    bi2 = bi2.Multiply(New Org.BouncyCastle.Math.BigInteger("58"))
                    bi2 = bi2.Add(New Org.BouncyCastle.Math.BigInteger(b58.IndexOf(c).ToString()))
                Else
                    Return Nothing
                End If
            Next

            Dim bb As Byte() = bi2.ToByteArrayUnsigned()

            ' interpret leading '1's as leading zero bytes
            For Each c As Char In base58
                If c <> "1"c Then
                    Exit For
                End If
                Dim bbb As Byte() = New Byte(bb.Length) {}
                Array.Copy(bb, 0, bbb, 1, bb.Length)
                bb = bbb
            Next

            Return bb
        End Function

        Public Shared Function FromByteArray(ByVal ba As Byte()) As String
            Dim addrremain As New Org.BouncyCastle.Math.BigInteger(1, ba)

            Dim big0 As New Org.BouncyCastle.Math.BigInteger("0")
            Dim big58 As New Org.BouncyCastle.Math.BigInteger("58")

            Dim b58 As String = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"

            Dim rv As String = ""

            While addrremain.CompareTo(big0) > 0
                Dim d As Integer = Convert.ToInt32(addrremain.[Mod](big58).ToString())
                addrremain = addrremain.Divide(big58)
                rv = b58.Substring(d, 1) & rv
            End While

            ' handle leading zeroes
            For Each b As Byte In ba
                If b <> 0 Then
                    Exit For
                End If

                rv = "1" & rv
            Next
            Return rv
        End Function

    End Class
End Namespace