
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Security.Cryptography
Imports System.IO
Imports Org.BouncyCastle.Asn1
Imports Org.BouncyCastle.Crypto
Imports Org.BouncyCastle.Crypto.Digests
Imports Org.BouncyCastle.Crypto.Generators
Imports Org.BouncyCastle.Crypto.Parameters
Imports Org.BouncyCastle.Security
Imports Org.BouncyCastle.Math.EC
Imports Org.BouncyCastle.Math

Namespace Casascius.Bitcoin

    ''' <summary>
    ''' Bitcoin address extended to include knowledge of public key.
    ''' </summary>
    Public Class PublicKey
        Inherits AddressBase

        Protected Sub New()
        End Sub
     

        Public Sub New(ByVal point As ECPoint)
            Me.IsCompressedPoint = point.IsCompressed
            Me.point = point
            Me.PublicKeyBytes = point.GetEncoded()
            If validatePoint() = False Then
                Throw New ArgumentException("Not a valid public key")
            End If
        End Sub

        Public Sub New(ByVal hex As String)
            Dim pubKeyBytes As Byte() = Util.HexStringToBytes(hex)
            Dim result As String = constructFromBytes(pubKeyBytes)
            If result IsNot Nothing Then
                Throw New ArgumentException(result)
            End If
        End Sub

        ''' <summary>
        ''' Constructor that takes a byte array of 33 or 65 bytes representing a public key.
        ''' </summary>
        Public Sub New(ByVal pubKeyBytes As Byte())
            Dim result As String = constructFromBytes(pubKeyBytes)
            If result IsNot Nothing Then
                Throw New ArgumentException(result)

            End If
        End Sub

        Public Shared Function IsValidPublicKey(ByVal hex As String) As Boolean
            Dim pubKeyBytes As Byte() = Util.HexStringToBytes(hex)
            Dim pk As New PublicKey()
            Dim result As String = pk.constructFromBytes(pubKeyBytes)
            Return (result Is Nothing)
        End Function

        Private Function constructFromBytes(ByVal pubKeyBytes As Byte()) As String
            If pubKeyBytes Is Nothing Then
                Return "PublicKey constructor requires a byte array with 65 bytes."
            End If

            Dim sOut As String = "hi"



            If pubKeyBytes.Length = 65 Then
                If pubKeyBytes(0) <> 4 Then
                    Return "Invalid public key, for 65-byte keys the first byte must be 0x04"

                End If
            ElseIf pubKeyBytes.Length = 33 Then
                If pubKeyBytes(0) <> 2 AndAlso pubKeyBytes(0) <> 3 Then
                    Return "Invalid public key, for 3-byte keys the first byte must be 0x02 or 0x03"
                End If
                IsCompressedPoint = True
            Else
                Return "Invalid public key, must be 33 or 65 bytes"
            End If
            Try
                Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
                point = ps.Curve.DecodePoint(pubKeyBytes)
                If validatePoint() = False Then
                    Return "Not a valid public key"
                End If

                ' todo: ensure X and Y are on the curve
                PublicKeyBytes = pubKeyBytes
            Catch e As Exception
                ' catches errors like "invalid point compression"
                Return "Not a valid public key: " & e.Message
            End Try
            Return Nothing
        End Function

        ''' <summary>
        ''' Returns true if the point coordinates satisfy the elliptic curve equation.
        ''' </summary>
        Private Function validatePoint() As Boolean
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            Dim y2 = point.Y.Multiply(point.Y)
            Dim x3 = point.X.Multiply(point.X).Multiply(point.X)
            Dim ax = point.X.Multiply(ps.Curve.A)
            Dim x3axb = x3.Add(ax).Add(ps.Curve.B)
            Return y2.Equals(x3axb)
        End Function

        Protected point As ECPoint = Nothing

        Private _publicKey As Byte() = Nothing

        Public Property IsCompressedPoint() As Boolean
            Get
                Return m_IsCompressedPoint
            End Get
            Protected Set(ByVal value As Boolean)
                m_IsCompressedPoint = Value
            End Set
        End Property
        Private m_IsCompressedPoint As Boolean


        ''' <summary>
        ''' Virtual method to compute public key on demand when doing so is expensive.
        ''' Not used if we are handed a public key through the constructor, but this is used
        ''' if a descendant class (e.g. KeyPair) has a private key and the only way to know
        ''' the public key is to compute it.
        ''' </summary>
        Protected Overridable Function ComputePublicKey() As Byte()
            Return Nothing
        End Function

        ''' <summary>
        ''' Returns the public key bytes.  This will return 65 bytes for an uncompressed public key
        ''' or 33 bytes for a compressed public key.
        ''' </summary>
        Public Property PublicKeyBytes() As Byte()
            Get
                If _publicKey Is Nothing Then
                    _publicKey = ComputePublicKey()
                End If

                Dim rv As Byte() = New Byte(_publicKey.Length - 1) {}
                _publicKey.CopyTo(rv, 0)
                Return rv
            End Get
            Protected Set(ByVal value As Byte())
                _publicKey = New Byte(value.Length - 1) {}
                value.CopyTo(_publicKey, 0)
            End Set
        End Property

        Public Function GetCompressed() As Byte()
            Return getReencoded(True)
        End Function

        Public Function GetUncompressed() As Byte()
            Return getReencoded(False)
        End Function

        Public Function GetECPoint() As ECPoint
            Dim pubKeyBytes As Byte() = PublicKeyBytes
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            Return ps.Curve.DecodePoint(pubKeyBytes)
        End Function

        Private Function getReencoded(ByVal compressed As Boolean) As Byte()
            Dim pubKeyBytes As Byte() = PublicKeyBytes
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            point = ps.Curve.DecodePoint(pubKeyBytes)
            Dim point2 = ps.Curve.CreatePoint(point.X.ToBigInteger(), point.Y.ToBigInteger(), compressed)
            Return point2.GetEncoded()
        End Function

        Public Shared Function GetUncompressed(ByVal point As ECPoint) As ECPoint
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            Return ps.Curve.CreatePoint(point.X.ToBigInteger(), point.Y.ToBigInteger(), False)
        End Function

        Public Shared Function GetCompressed(ByVal point As ECPoint) As ECPoint
            Dim ps = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1")
            Return ps.Curve.CreatePoint(point.X.ToBigInteger(), point.Y.ToBigInteger(), True)
        End Function


        ''' <summary>
        ''' Computes the Hash160 of the public key upon demand.
        ''' </summary>
        Protected Overrides Function ComputeHash160() As Byte()
            Dim shaofpubkey As Byte() = Util.ComputeSha256(PublicKeyBytes)
            Dim rip As RIPEMD160 = System.Security.Cryptography.RIPEMD160.Create()
            Return rip.ComputeHash(shaofpubkey)
        End Function

        ''' <summary>
        ''' Hexadecimal representation of public key.  Each byte is 2 hex digits, uppercase,
        ''' delimited with spaces.
        ''' </summary>
        Public ReadOnly Property PublicKeyHex() As String

            Get
                Return Util.ByteArrayToString(PublicKeyBytes)
            End Get
        End Property







        Public Function hi()

        End Function

    End Class
End Namespace