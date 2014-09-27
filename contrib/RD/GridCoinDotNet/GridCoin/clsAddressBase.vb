
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

Namespace Casascius.Bitcoin



    ''' <summary>
    ''' Represents a single Bitcoin address, assumes knowledge only of a Hash160.
    ''' </summary>
    Public Class AddressBase

        Protected Sub New()
        End Sub

        ''' <summary>
        ''' Constructs a Bitcoin address from a 20 byte array representing a Hash160.
        ''' If 21 bytes are provided, the extra byte denotes address type.
        ''' </summary>
        Public Sub New(ByVal addressBytes As Byte())
            ' Hash160 setter validates length and throws exception if needed
            Hash160 = addressBytes
        End Sub

        ''' <summary>
        ''' Constructs a Bitcoin address from a 20 byte array representing a Hash160,
        ''' and also denoting a specific address type.
        ''' </summary>
        Public Sub New(ByVal addressBytes As Byte(), ByVal addressType As Byte)
            ' Hash160 setter validates length and throws exception if needed
            Hash160 = addressBytes
            Me.AddressType = addressType
        End Sub

        ''' <summary>
        ''' Allows calculation of address with a different AddressType
        ''' </summary>
        Public Sub New(ByVal otheraddress As AddressBase, ByVal addressType As Byte)
            ' Hash160 setter validates length and throws exception if needed
            Hash160 = otheraddress.Hash160
            Me.AddressType = addressType
        End Sub

        ''' <summary>
        ''' Constructs an Address from an address string
        ''' </summary>
        Public Sub New(ByRef address As String, bForceMutiliation As Boolean)

            Dim hex As Byte() = Util.Base58CheckToByteArray(address, bForceMutiliation)
            If hex Is Nothing Then Return


            If hex.Length <> 21 Then
                Throw New ArgumentException("Not a valid or recognized address")
            End If
            ' Hash160 setter validates length and throws exception if needed
            Hash160 = hex
        End Sub



        ''' <summary>
        ''' Returns the address type.  For example, 0=Bitcoin
        ''' </summary>
        Public Property AddressType() As Byte
            Get
                Return _addressType
            End Get
            Protected Set(ByVal value As Byte)
                _addressType = value
                _address = Nothing
            End Set
        End Property


        Protected _addressType As Byte = 0

        Private _hash160 As Byte() = Nothing

        ''' <summary>
        ''' Overridden in descendant classes allowing Hash160 to be computed on an as-needed
        ''' basis (since it's CPU-costly if it comes from a private key)
        ''' </summary>
        Protected Overridable Function ComputeHash160() As Byte()
            Return Nothing
        End Function

        ''' <summary>
        ''' Returns a copy of the 20-byte Hash160 of the Bitcoin address
        ''' </summary>
        Public Property Hash160() As Byte()
            Get
                If _hash160 Is Nothing Then
                    _hash160 = ComputeHash160()
                End If

                ' make a copy for the caller
                Dim rv As Byte() = New Byte(19) {}
                Array.Copy(_hash160, rv, 20)
                Return rv
            End Get
            Protected Set(ByVal value As Byte())
                If value.Length = 20 Then
                    _hash160 = New Byte(19) {}
                    value.CopyTo(_hash160, 0)
                ElseIf value.Length = 21 Then
                    _hash160 = New Byte(19) {}
                    Array.Copy(value, 1, _hash160, 0, 20)
                    AddressType = value(0)
                Else
                    Throw New ArgumentException("Address constructor with byte array requires 20 or 21 bytes")
                End If
            End Set
        End Property

        Public ReadOnly Property Hash160Hex() As String
            Get
                Return Util.ByteArrayToString(Hash160)
            End Get
        End Property


        ''' <summary>
        ''' Get the Bitcoin address in Base58 format as it would be seen by the user.
        ''' </summary>
        Public ReadOnly Property AddressBase58() As String
            Get
                If _address Is Nothing Then
                    ' compute the base58 but cache it for subsequent references.
                    Dim hex2 As Byte() = New Byte(20) {}
                    Array.Copy(Hash160, 0, hex2, 1, 20)
                    hex2(0) = AddressType
                    _address = Util.ByteArrayToBase58Check(hex2)
                    Return _address
                End If
                Return _address
            End Get
        End Property

        Protected _address As String = Nothing
    End Class
End Namespace