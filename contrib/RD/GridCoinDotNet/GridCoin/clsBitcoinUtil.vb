
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Drawing
Imports System.Security.Cryptography
Imports System.IO
Imports Org.BouncyCastle.Asn1
Imports Org.BouncyCastle.Crypto
Imports Org.BouncyCastle.Crypto.Digests
Imports Org.BouncyCastle.Crypto.Generators
Imports Org.BouncyCastle.Crypto.Parameters
Imports Org.BouncyCastle.Security
Imports Org.BouncyCastle.Math.EC

Namespace Casascius.Bitcoin
    Public Class Util
        Public Shared Function PassphraseToPrivHex(ByVal passphrase As String) As String
            Return ByteArrayToString(ComputeSha256(passphrase))
        End Function

        Public Shared Function ByteArrayToBase58Check(ByVal ba As Byte()) As String

            Dim bb As Byte() = New Byte(ba.Length + 3) {}
            Array.Copy(ba, bb, ba.Length)
            Dim bcsha256a As New Sha256Digest()
            bcsha256a.BlockUpdate(ba, 0, ba.Length)
            Dim thehash As Byte() = New Byte(31) {}
            bcsha256a.DoFinal(thehash, 0)
            bcsha256a.BlockUpdate(thehash, 0, 32)
            bcsha256a.DoFinal(thehash, 0)
            For i As Integer = 0 To 3
                bb(ba.Length + i) = thehash(i)
            Next
            Return Base58.FromByteArray(bb)
        End Function


        Public Shared Function ValidateAndGetHexPublicKey(ByVal PubHex As String) As Byte()
            Dim hex As Byte() = GetHexBytes(PubHex, 64)

            If hex Is Nothing OrElse hex.Length < 64 OrElse hex.Length > 65 Then
                Throw New ApplicationException("Hex is not 64 or 65 bytes.")
            End If

            ' if leading 00, change it to 0x80
            If hex.Length = 65 Then
                If hex(0) = 0 OrElse hex(0) = 4 Then
                    hex(0) = 4
                Else
                    Throw New ApplicationException("Not a valid public key")
                End If
            End If

            ' add 0x80 byte if not present
            If hex.Length = 64 Then
                Dim hex2 As Byte() = New Byte(64) {}
                Array.Copy(hex, 0, hex2, 1, 64)
                hex2(0) = 4
                hex = hex2
            End If
            Return hex
        End Function

        Public Shared Function ValidateAndGetHexPublicHash(ByVal PubHash As String) As Byte()
            Dim hex As Byte() = GetHexBytes(PubHash, 20)

            If hex Is Nothing OrElse hex.Length <> 20 Then
                Throw New ApplicationException("Hex is not 20 bytes.")
            End If
            Return hex
        End Function


        Public Shared Function ValidateAndGetHexPrivateKey(ByVal leadingbyte As Byte, ByVal PrivHex As String, ByVal desiredByteCount As Integer) As Byte()
            If desiredByteCount <> 32 AndAlso desiredByteCount <> 33 Then
                Throw New ApplicationException("desiredByteCount must be 32 or 33")
            End If

            Dim hex As Byte() = GetHexBytes(PrivHex, 32)

            If hex Is Nothing OrElse hex.Length < 32 OrElse hex.Length > 33 Then
                Throw New ApplicationException("Hex is not 32 or 33 bytes.")
            End If

            ' if leading 00, change it to 0x80
            If hex.Length = 33 Then
                If hex(0) = 0 OrElse hex(0) = &H80 Then
                    hex(0) = &H80
                Else
                    Throw New ApplicationException("Not a valid private key")
                End If
            End If

            ' add 0x80 byte if not present
            If hex.Length = 32 AndAlso desiredByteCount = 33 Then
                Dim hex2 As Byte() = New Byte(32) {}
                Array.Copy(hex, 0, hex2, 1, 32)
                hex2(0) = &H80
                hex = hex2
            End If

            If desiredByteCount = 33 Then
                hex(0) = leadingbyte
            End If

            If desiredByteCount = 32 AndAlso hex.Length = 33 Then
                Dim hex2 As Byte() = New Byte(32) {}
                Array.Copy(hex, 1, hex2, 0, 32)
                hex = hex2
            End If

            Return hex

        End Function

        ''' <summary>
        ''' Trims whitespace from within and outside string.
        ''' Whitespace is anything non-alphanumeric that may have been inserted into a string.
        ''' </summary>
        Public Shared Function Base58Trim(ByVal base58 As String) As String
            Dim strin As Char() = base58.ToCharArray()
            Dim cc As Char() = New Char(base58.Length - 1) {}
            Dim pos As Integer = 0
            For i As Integer = 0 To base58.Length - 1
                Dim c As Char = strin(i)
                If (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) Then
                    cc(System.Math.Max(System.Threading.Interlocked.Increment(pos), pos - 1)) = c
                End If
            Next
            Return New [String](cc, 0, pos)
        End Function

        ''' <summary>
        ''' Converts a base-58 string to a byte array, checking the checksum, and
        ''' returning null if it wasn't valid.  Appending "?" to the end of the string skips
        ''' the checksum calculation, but still strips the four checksum bytes from the
        ''' result.
        ''' </summary>
        Public Shared Function Base58CheckToByteArray(ByVal base58__1 As String) As Byte()

            Dim IgnoreChecksum As Boolean = False
            If base58__1.EndsWith("?") Then
                IgnoreChecksum = True
                base58__1 = base58__1.Substring(0, base58__1.Length - 1)
            End If

            Dim bb As Byte() = Base58.ToByteArray(base58__1)
            If bb Is Nothing OrElse bb.Length < 4 Then
                Return Nothing
            End If

            If IgnoreChecksum = False Then
                Dim bcsha256a As New Sha256Digest()
                bcsha256a.BlockUpdate(bb, 0, bb.Length - 4)

                Dim checksum As Byte() = New Byte(31) {}
                'sha256.ComputeHash(bb, 0, bb.Length - 4);
                bcsha256a.DoFinal(checksum, 0)
                bcsha256a.BlockUpdate(checksum, 0, 32)
                bcsha256a.DoFinal(checksum, 0)

                For i As Integer = 0 To 3
                    If checksum(i) <> bb(bb.Length - 4 + i) Then
                        Return Nothing
                    End If
                Next
            End If

            Dim rv As Byte() = New Byte(bb.Length - 5) {}
            Array.Copy(bb, 0, rv, 0, bb.Length - 4)
            Return rv
        End Function

        Public Shared Function ByteArrayToString(ByVal ba As Byte()) As String
            Return ByteArrayToString(ba, 0, ba.Length)
        End Function

        Public Shared Function ByteArrayToString(ByVal ba As Byte(), ByVal offset As Integer, ByVal count As Integer) As String
            Dim rv As String = ""
            Dim usedcount As Integer = 0
            Dim i As Integer = offset
            While usedcount < count
                rv += [String].Format("{0:X2}", ba(i)) & " "
                i += 1
                usedcount += 1
            End While
            Return rv
        End Function




        Public Shared Function GetHexBytes(ByVal source As String, ByVal minimum As Integer) As Byte()
            Dim hex As Byte() = HexStringToBytes(source)
            If hex Is Nothing Then
                Return Nothing
            End If
            ' assume leading zeroes if we're short a few bytes
            If hex.Length > (minimum - 6) AndAlso hex.Length < minimum Then
                Dim hex2 As Byte() = New Byte(minimum - 1) {}
                Array.Copy(hex, 0, hex2, minimum - hex.Length, hex.Length)
                hex = hex2
            End If
            ' clip off one overhanging leading zero if present
            If hex.Length = minimum + 1 AndAlso hex(0) = 0 Then
                Dim hex2 As Byte() = New Byte(minimum - 1) {}
                Array.Copy(hex, 1, hex2, 0, minimum)

                hex = hex2
            End If

            Return hex
        End Function


        ''' <summary>
        ''' Converts a hex string to bytes.  Hex chars can optionally be space-delimited, otherwise,
        ''' any two contiguous hex chars are considered to be a byte.  If testingForValidHex==true,
        ''' then if any invalid characters are found, the function returns null instead of bytes.
        ''' </summary>
        Public Shared Function HexStringToBytes(ByVal source As String, Optional ByVal testingForValidHex As Boolean = False) As Byte()
            Dim bytes As New List(Of Byte)()
            Dim gotFirstChar As Boolean = False
            Dim accum As Byte = 0

            For Each c As Char In source.ToCharArray()
                If c = " "c OrElse c = "-"c OrElse c = ":"c Then
                    ' if we got a space, then accept it as the end if we have 1 character.
                    If gotFirstChar Then
                        bytes.Add(accum)
                        accum = 0
                        gotFirstChar = False
                    End If
                ElseIf (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "A"c AndAlso c <= "F"c) OrElse (c >= "a"c AndAlso c <= "f"c) Then
                    ' get the character's value
                    Dim v As Byte = CByte(AscW(c) - &H30)


                    If c >= "A"c AndAlso c <= "F"c Then
                        v = CByte(AscW(c) + &HA - AscW("A"c))


                    End If
                    If c >= "a"c AndAlso c <= "f"c Then
                        v = CByte(AscW(c) + &HA - AscW("a"c))

                    End If

                    If gotFirstChar = False Then
                        gotFirstChar = True
                        accum = v
                    Else
                        accum <<= 4
                        accum += v
                        bytes.Add(accum)
                        accum = 0
                        gotFirstChar = False
                    End If
                Else
                    If testingForValidHex Then
                        Return Nothing
                    End If
                End If
            Next
            If gotFirstChar Then
                bytes.Add(accum)
            End If
            Return bytes.ToArray()
        End Function



        Public Shared Function PrivHexToPubHex(ByVal PrivHex As String) As String
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            Return PrivHexToPubHex(PrivHex, ps.G)
        End Function

        Public Shared Function PrivHexToPubHex(ByVal PrivHex As String, ByVal point As ECPoint) As String

            Dim hex As Byte() = ValidateAndGetHexPrivateKey(&H0, PrivHex, 33)
            If hex Is Nothing Then
                Throw New ApplicationException("Invalid private hex key")
            End If
            Dim Db As New Org.BouncyCastle.Math.BigInteger(hex)
            Dim dd As ECPoint = point.Multiply(Db)

            Dim pubaddr As Byte() = PubKeyToByteArray(dd)

            Return ByteArrayToString(pubaddr)

        End Function

        Public Shared Function PrivHexToPubKey(ByVal PrivHex As String) As ECPoint
            Dim hex As Byte() = ValidateAndGetHexPrivateKey(&H0, PrivHex, 33)
            If hex Is Nothing Then
                Throw New ApplicationException("Invalid private hex key")
            End If
            Dim Db As New Org.BouncyCastle.Math.BigInteger(1, hex)
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            Return ps.G.Multiply(Db)
        End Function

        Public Shared Function PrivKeyToPubKey(ByVal PrivKey As Byte()) As ECPoint
            If PrivKey Is Nothing OrElse PrivKey.Length > 32 Then
                Throw New ApplicationException("Invalid private hex key")
            End If
            Dim Db As New Org.BouncyCastle.Math.BigInteger(1, PrivKey)
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            Return ps.G.Multiply(Db)
        End Function


        Public Shared Function PubKeyToByteArray(ByVal point As ECPoint) As Byte()
            Dim pubaddr As Byte() = New Byte(64) {}
            Dim Y As Byte() = point.Y.ToBigInteger().ToByteArray()
            Array.Copy(Y, 0, pubaddr, 64 - Y.Length + 1, Y.Length)
            Dim X As Byte() = point.X.ToBigInteger().ToByteArray()
            Array.Copy(X, 0, pubaddr, 32 - X.Length + 1, X.Length)
            pubaddr(0) = 4
            Return pubaddr
        End Function

        Public Shared Function PubHexToPubHash(ByVal PubHex As String) As String
            Dim hex As Byte() = ValidateAndGetHexPublicKey(PubHex)
            If hex Is Nothing Then
                Throw New ApplicationException("Invalid public hex key")
            End If
            Return PubHexToPubHash(hex)
        End Function

        Public Shared Function PubHexToPubHash(ByVal PubHex As Byte()) As String

            Dim shaofpubkey As Byte() = ComputeSha256(PubHex)

            Dim rip As RIPEMD160 = System.Security.Cryptography.RIPEMD160.Create()
            Dim ripofpubkey As Byte() = rip.ComputeHash(shaofpubkey)

            Return ByteArrayToString(ripofpubkey)

        End Function

        Public Shared Function PubHashToAddress(ByVal PubHash As String, ByVal AddressType As String) As String
            Dim hex As Byte() = ValidateAndGetHexPublicHash(PubHash)
            If hex Is Nothing Then
                Throw New ApplicationException("Invalid public hex key")
            End If

            Dim hex2 As Byte() = New Byte(20) {}
            Array.Copy(hex, 0, hex2, 1, 20)

            Dim cointype As Integer = 0
            If Int32.TryParse(AddressType, cointype) = False Then
                cointype = 0
            End If

            If AddressType = "Testnet" Then
                cointype = 111
            End If
            If AddressType = "Namecoin" Then
                cointype = 52
            End If
            If AddressType = "Litecoin" Then
                cointype = 48
            End If
            hex2(0) = CByte(cointype And &HFF)
            Return ByteArrayToBase58Check(hex2)


        End Function

        Public Shared Function PassphraseTooSimple(ByVal passphrase As String) As Boolean

            Dim Lowercase As Integer = 0, Uppercase As Integer = 0, Numbers As Integer = 0, Symbols As Integer = 0, Spaces As Integer = 0
            For Each c As Char In passphrase.ToCharArray()
                If c >= "a"c AndAlso c <= "z"c Then
                    Lowercase += 1
                ElseIf c >= "A"c AndAlso c <= "Z"c Then
                    Uppercase += 1
                ElseIf c >= "0"c AndAlso c <= "9"c Then
                    Numbers += 1
                ElseIf c = " "c Then
                    Spaces += 1
                Else
                    Symbols += 1
                End If
            Next

            ' let mini private keys through - they won't contain words, they are nonsense characters, so their entropy is a bit better per character
            If MiniKeyPair.IsValidMiniKey(passphrase) <> 1 Then
                Return False
            End If

            If passphrase.Length < 30 AndAlso (Lowercase < 10 OrElse Uppercase < 3 OrElse Numbers < 2 OrElse Symbols < 2) Then
                Return True
            End If

            Return False

        End Function

        Public Shared Function ComputeSha256(ByVal ofwhat As String) As Byte()
            Dim utf8 As New UTF8Encoding(False)
            Return ComputeSha256(utf8.GetBytes(ofwhat))
        End Function


        Public Shared Function ComputeSha256(ByVal ofwhat As Byte()) As Byte()
            Dim sha256 As New Sha256Digest()
            sha256.BlockUpdate(ofwhat, 0, ofwhat.Length)
            Dim rv As Byte() = New Byte(31) {}
            sha256.DoFinal(rv, 0)
            Return rv
        End Function

        Public Shared Function ComputeDoubleSha256(ByVal ofwhat As String) As Byte()
            Dim utf8 As New UTF8Encoding(False)
            Return ComputeDoubleSha256(utf8.GetBytes(ofwhat))
        End Function

        Public Shared Function ComputeDoubleSha256(ByVal ofwhat As Byte()) As Byte()
            Dim sha256 As New Sha256Digest()
            sha256.BlockUpdate(ofwhat, 0, ofwhat.Length)
            Dim rv As Byte() = New Byte(31) {}
            sha256.DoFinal(rv, 0)
            sha256.BlockUpdate(rv, 0, rv.Length)
            sha256.DoFinal(rv, 0)
            Return rv
        End Function

        Public Shared nonce As Int64 = 0

        Public Shared Function Force32Bytes(ByVal inbytes As Byte()) As Byte()
            If inbytes.Length = 32 Then
                Return inbytes
            End If
            Dim rv As Byte() = New Byte(31) {}
            If inbytes.Length > 32 Then
                Array.Copy(inbytes, inbytes.Length - 32, rv, 0, 32)
            Else
                Array.Copy(inbytes, 0, rv, 32 - inbytes.Length, inbytes.Length)
            End If
            Return rv
        End Function

        ''' <summary>
        ''' Extension for cloning a byte array
        ''' </summary>
        Public Shared Function CloneByteArray(ByVal ba As Byte()) As Byte()
            If ba Is Nothing Then
                Return Nothing
            End If
            Dim copy As Byte() = New Byte(ba.Length - 1) {}
            Array.Copy(ba, copy, ba.Length)
            Return copy
        End Function

        ''' <summary>
        ''' Extension for cloning a portion of a byte array
        ''' </summary>
        Public Shared Function CloneByteArray(ByVal ba As Byte(), ByVal offset As Integer, ByVal length As Integer) As Byte()
            If ba Is Nothing Then
                Return Nothing
            End If
            Dim copy As Byte() = New Byte(length - 1) {}
            Array.Copy(ba, offset, copy, 0, length)
            Return copy
        End Function

        Public Shared Function ConcatenateByteArrays(ByVal ParamArray bytearrays As Byte()()) As Byte()
            Dim totalLength As Integer = 0
            For i As Integer = 0 To bytearrays.Length - 1
                totalLength += bytearrays(i).Length
            Next
            Dim rv As Byte() = New Byte(totalLength - 1) {}
            Dim idx As Integer = 0
            For i As Integer = 0 To bytearrays.Length - 1
                Array.Copy(bytearrays(i), 0, rv, idx, bytearrays(i).Length)
                idx += bytearrays(i).Length
            Next
            Return rv
        End Function


    End Class



End Namespace