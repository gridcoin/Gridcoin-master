Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Security.Cryptography
Imports System.Diagnostics
Imports System.IO
Imports Org.BouncyCastle.Asn1
Imports Org.BouncyCastle.Crypto
Imports Org.BouncyCastle.Crypto.Digests
Imports Org.BouncyCastle.Crypto.Generators
Imports Org.BouncyCastle.Crypto.Parameters
Imports Org.BouncyCastle.Security
Imports Org.BouncyCastle.Math.EC
Imports Org.BouncyCastle.Math
Imports CryptSharp.Utility

Namespace Casascius.Bitcoin

    ''' <summary>
    ''' A KeyPair represents a Bitcoin address and its known private key.
    ''' </summary>
    Public Class KeyPair
        Inherits PublicKey
        Protected Sub New()
        End Sub

        Private Shared nonce As Int64

        ''' <summary>
        ''' Creates a new key pair using the SHA256 hash of a given string as the private key.
        ''' </summary>
        Public Shared Function CreateFromString(ByVal tohash As String) As KeyPair
            Dim utf8 As New UTF8Encoding(False)
            Dim forsha As Byte() = utf8.GetBytes(tohash)
            Dim sha256 As New Sha256Digest()
            sha256.BlockUpdate(forsha, 0, forsha.Length)
            Dim thehash As Byte() = New Byte(31) {}
            sha256.DoFinal(thehash, 0)
            Return New KeyPair(thehash)
        End Function

        ''' <summary>
        ''' Creates a new random key pair, using a user-provided string to add entropy to the
        ''' SecureRandom generator provided by the .NET Framework.
        ''' </summary>
        Public Shared Function Create(ByVal usersalt As String, Optional ByVal compressed As Boolean = False, Optional ByVal addressType As Byte = 0) As KeyPair
            If usersalt Is Nothing Then
                usersalt = "ok, whatever"
            End If
            usersalt += DateTime.UtcNow.Ticks.ToString()

            Dim sr As New SecureRandom()

            Dim poop As Byte() = Util.ComputeSha256(usersalt & nonce.ToString())
            nonce += 1

            Dim newkey As Byte() = New Byte(31) {}

            For i As Integer = 0 To 31
                Dim x As Long = sr.NextLong() And Long.MaxValue
                x += poop(i)
                newkey(i) = CByte(x And &HFF)
            Next
            Return New KeyPair(newkey, compressed:=compressed, addressType:=addressType)
        End Function

        ''' <summary>
        ''' Generates a KeyPair using a BigInteger as a private key.
        ''' BigInteger is checked for appropriate range.
        ''' </summary>
        Public Sub New(ByVal bi As BigInteger, Optional ByVal compressed As Boolean = False, Optional ByVal addressType As Byte = 0)
            Me.IsCompressedPoint = compressed
            Me._addressType = addressType

            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            If bi.CompareTo(ps.N) >= 0 OrElse bi.SignValue <= 0 Then
                Throw New ArgumentException("BigInteger is out of range of valid private keys")
            End If
            Dim bb As Byte() = Util.Force32Bytes(bi.ToByteArrayUnsigned())
            PrivateKeyBytes = bb
        End Sub


        ''' <summary>
        ''' Create a Bitcoin address from a 32-byte private key
        ''' </summary>
        Public Sub New(ByVal bytes As Byte(), Optional ByVal compressed As Boolean = False, Optional ByVal addressType As Byte = 0)
            If bytes.Length = 32 Then
                PrivateKeyBytes = bytes
                Me.IsCompressedPoint = compressed
                Me._addressType = addressType
            Else
                Throw New ArgumentException("Byte array provided to KeyPair constructor must be 32 bytes long")
            End If
        End Sub

        ''' <summary>
        ''' Create a Bitcoin address from a key represented in a string.
        ''' </summary>
        Public Sub New(ByVal key As String, Optional ByVal compressed As Boolean = False, Optional ByVal addressType As Byte = 0)
            Me._addressType = addressType
            Dim result As String = constructWithKey(key, compressed)
            If result IsNot Nothing Then
                Throw New ArgumentException(result)

            End If
        End Sub

        ''' <summary>
        ''' Constructs the object with string key, returning any intended exception as a string.
        ''' </summary>
        Private Function constructWithKey(ByVal key As String, ByVal compressed As Boolean) As String
            Dim hex As Byte() = Util.Base58CheckToByteArray(key)
            If hex Is Nothing Then
                hex = Util.HexStringToBytes(key, True)
                If hex Is Nothing Then
                    ' tolerate a minikey
                    If MiniKeyPair.IsValidMiniKey(key) > 0 Then
                        PrivateKeyBytes = New MiniKeyPair(key).PrivateKeyBytes
                        Return Nothing
                    Else
                        Return "Invalid private key"
                    End If
                End If
            End If
            If hex.Length = 32 Then
                _privKey = New Byte(31) {}
                Array.Copy(hex, 0, _privKey, 0, 32)
                IsCompressedPoint = compressed
            ElseIf hex.Length = 33 AndAlso hex(0) = &H80 Then
                ' normal private key
                _privKey = New Byte(31) {}
                Array.Copy(hex, 1, _privKey, 0, 32)
                IsCompressedPoint = False
            ElseIf hex.Length = 34 AndAlso hex(0) = &H80 AndAlso hex(33) = &H1 Then
                ' compressed private key
                _privKey = New Byte(31) {}
                Array.Copy(hex, 1, _privKey, 0, 32)
                IsCompressedPoint = True
            ElseIf key.StartsWith("6") Then
                Return "Key is encrypted, decrypt first."
            Else
                Return "Not a recognized private key format"
            End If
            Return validateRange()

        End Function

        ''' <summary>
        ''' Returns error message in a string if private key is not within the valid range (2 ... N-1)
        ''' </summary>
        Private Function validateRange() As String
            Dim Db As New Org.BouncyCastle.Math.BigInteger(1, _privKey)
            Dim N As BigInteger = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1").N
            '            If Db = BigInteger.Zero OrElse Db.CompareTo(N) >= 0 Then
            If Val(Db) = 0 OrElse Db.CompareTo(N) >= 0 Then
                Return "Not a valid private key"
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Returns true if a given string can be turned into a private key.
        ''' </summary>
        Public Shared Function IsValidPrivateKey(ByVal key As String) As Boolean
            Dim kp As New KeyPair()
            Dim result As String = kp.constructWithKey(key, False)
            Return (result Is Nothing)
        End Function


        ''' <summary>
        ''' Provides access to the private key.
        ''' </summary>
        Public Property PrivateKeyBytes() As Byte()
            Get
                Dim rv As Byte() = New Byte(_privKey.Length - 1) {}
                _privKey.CopyTo(rv, 0)
                Return rv
            End Get
            Protected Set(ByVal value As Byte())
                If value Is Nothing OrElse value.Length < 32 OrElse value.Length > 33 Then
                    Throw New ArgumentException("Must be 32 bytes")
                End If

                _privKey = New Byte(31) {}

                Array.Copy(value, value.Length - 32, _privKey, 0, 32)
            End Set
        End Property

        Private _privKey As Byte()

        ''' <summary>
        ''' Computes the public key from the private key.
        ''' </summary>
        Protected Overrides Function ComputePublicKey() As Byte()
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            Dim point As ECPoint = ps.G

            Dim Db As New Org.BouncyCastle.Math.BigInteger(1, _privKey)
            Dim dd As ECPoint = point.Multiply(Db)


            If IsCompressedPoint Then
                dd = ps.Curve.CreatePoint(dd.X.ToBigInteger(), dd.Y.ToBigInteger(), True)
                Return dd.GetEncoded()
            Else
                Dim pubaddr As Byte() = New Byte(64) {}
                Dim Y As Byte() = dd.Y.ToBigInteger().ToByteArray()
                Array.Copy(Y, 0, pubaddr, 64 - Y.Length + 1, Y.Length)
                Dim X As Byte() = dd.X.ToBigInteger().ToByteArray()
                Array.Copy(X, 0, pubaddr, 32 - X.Length + 1, X.Length)
                pubaddr(0) = 4
                Return pubaddr
            End If
        End Function

        ''' <summary>
        ''' Returns the private key as a string of hexadecimal digits.
        ''' </summary>
        Public Property PrivateKeyHex() As String
            Get
                Return Util.ByteArrayToString(PrivateKeyBytes)
            End Get
            Protected Set(ByVal value As String)
                Dim hex As Byte() = Util.ValidateAndGetHexPrivateKey(&H80, value, 32)
                If hex Is Nothing Then
                    Throw New ApplicationException("Invalid private hex key")
                End If
                _privKey = hex
            End Set
        End Property

        ''' <summary>
        ''' Returns the private key in the most preferred display format for the type.
        ''' </summary>
        Public Overridable ReadOnly Property PrivateKey() As String
            Get
                Return PrivateKeyBase58
            End Get
        End Property


        ''' <summary>
        ''' Getter: Returns the private key, either unencrypted if no password was set, or encrypted
        ''' if a password is set or if we do not have it unencrypted.
        ''' Setter: Accepts a private key in wallet import format.  If the private key is encrypted, the
        ''' correct Passphrase must have been set, or else an ApplicationException will be thrown.
        ''' </summary>
        Public Property PrivateKeyBase58() As String
            Get
                If _privKey.Length <> 32 Then
                    Throw New ApplicationException("Not a valid private key")
                End If

                If IsCompressedPoint Then
                    Dim rv As Byte() = New Byte(33) {}
                    Array.Copy(_privKey, 0, rv, 1, 32)
                    rv(0) = &H80
                    rv(33) = 1
                    Return Util.ByteArrayToBase58Check(rv)
                Else
                    Dim rv As Byte() = New Byte(32) {}
                    Array.Copy(_privKey, 0, rv, 1, 32)
                    rv(0) = &H80
                    Return Util.ByteArrayToBase58Check(rv)
                End If
            End Get
            Protected Set(ByVal value As String)

                Dim hex As Byte() = Util.Base58CheckToByteArray(value)

                If hex Is Nothing Then
                    Throw New ApplicationException("WIF private key is not valid.")
                End If


                ' pywallet seems to produce and accept keys like this... they are private keys starting with 00 or 0000 and
                ' they pass the base58 check but are missing leading byte(s) that were never put into the original payload.
                ' We will simply fill in the missing bytes with 00's.
                If hex.Length = 29 OrElse hex.Length = 30 OrElse hex.Length = 31 OrElse hex.Length = 32 Then
                    Dim hex2 As Byte() = New Byte(32) {}
                    hex2(0) = hex(0)
                    Array.Copy(hex, 1, hex2, 34 - hex.Length, hex.Length - 1)
                    hex = hex2
                End If

                If hex.Length <> 33 Then
                    Throw New ApplicationException("WIF private key is not valid (wrong byte count, should be 33, was " & hex.Length & ")")
                End If

                If hex(0) = &H82 Then
                    Me.IsCompressedPoint = True
                ElseIf hex(0) <> &H80 Then
                    Throw New ApplicationException("This is a valid base58 string but it has no Wallet Import Format identifier.")
                End If

                _privKey = New Byte(31) {}
                Array.Copy(hex, 1, _privKey, 0, 32)


                _address = Nothing
            End Set
        End Property
    End Class
End Namespace