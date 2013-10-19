
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

    Public Class MiniKeyPair
        Inherits KeyPair

        Public Shared Function CreateDeterministic(ByVal seed As String) As MiniKeyPair
            '
            ' flow: 
            ' 1. take SHA256 of seed to yield 32 bytes
            ' 2. base58-encode those 32 bytes as though it were a regular private key. now we have 51 characters.
            ' 3. remove all instances of the digit 1. (likely source of typos)
            ' 4. take 29 characters starting with position 4
            '    (this is to skip those first characters of a base58check-encoded private key with low entropy)
            ' 5. test to see if it matches the typo check.  while it does not, increment and try again.
            Dim utf8 As New UTF8Encoding(False)
            Dim sha256ofseed As Byte() = Util.ComputeSha256(seed)
            Dim sOut2 As String

            Dim asbase58 As String = New KeyPair(sha256ofseed).PrivateKeyBase58.Replace("1", "")

            Dim keytotry As String = "S" & asbase58.Substring(4, 29)
            Dim chars As Char() = keytotry.ToCharArray()
            Dim charstest As Char() = (keytotry & "?").ToCharArray()

            While Util.ComputeSha256(utf8.GetBytes(charstest))(0) <> 0
                ' As long as key doesn't pass typo check, increment it.
                For i As Integer = chars.Length - 1 To 0 Step -1
                    Dim c As Char = chars(i)
                    If c = "9"c Then
                        charstest(i) = InlineAssignHelper(chars(i), "A"c)
                        Exit For
                    ElseIf c = "H"c Then
                        charstest(i) = InlineAssignHelper(chars(i), "J"c)
                        Exit For
                    ElseIf c = "N"c Then
                        charstest(i) = InlineAssignHelper(chars(i), "P"c)
                        Exit For
                    ElseIf c = "Z"c Then
                        charstest(i) = InlineAssignHelper(chars(i), "a"c)
                        Exit For
                    ElseIf c = "k"c Then
                        charstest(i) = InlineAssignHelper(chars(i), "m"c)
                        Exit For
                    ElseIf c = "z"c Then
                        ' No break - let loop increment prior character.
                        charstest(i) = InlineAssignHelper(chars(i), "2"c)
                    Else
                        Dim zzz As Integer
                        zzz = AscW(c)
                        System.Threading.Interlocked.Increment(zzz)
                        Dim yyy As Char
                        yyy = Chr(zzz)


                        charstest(i) = InlineAssignHelper(chars(i), yyy)

                        Exit For
                    End If
                Next
            End While
            Return New MiniKeyPair(New [String](chars))
        End Function

        ''' <summary>
        ''' Create a new random MiniKey.
        ''' Entropy is taken from .NET's SecureRandom, the system clock,
        ''' and any optionally provided salt.
        ''' </summary>
        Public Shared Function CreateRandom(ByVal usersalt As String) As MiniKeyPair
            If usersalt Is Nothing Then
                usersalt = "ok, whatever"
            End If
            usersalt += DateTime.UtcNow.Ticks.ToString()
            Dim sr As New SecureRandom()
            Dim chars As Char() = New Char(63) {}
            For i As Integer = 0 To 63
                chars(i) = ChrW(32 + (sr.NextInt() Mod 64))
            Next
            Return CreateDeterministic(usersalt & New [String](chars))
        End Function


        Public Sub New(ByVal key As String)
            MiniKey = key
        End Sub

        ''' <summary>
        ''' Returns the private key in the most preferred display format for the type.
        ''' </summary>
        Public Overrides ReadOnly Property PrivateKey() As String
            Get
                Return MiniKey
            End Get
        End Property

        Public Property MiniKey() As String
            Get
                Return _minikey
            End Get
            Protected Set(ByVal value As String)
                _minikey = value
                If value Is Nothing Then
                    PrivateKeyBytes = Nothing
                Else
                    If IsValidMiniKey(value) <= 0 Then
                        Throw New ApplicationException("Not a valid minikey")
                    End If
                    _minikey = value
                    ' Setting PrivateKeyBytes sets up delegates so the public key, hash160, and
                    ' bitcoin address can be computed upon demand.
                    PrivateKeyBytes = Util.ComputeSha256(value)
                End If
            End Set
        End Property

        Private _minikey As String

        ''' <summary>
        ''' Returns 1 if candidate is a valid Mini Private Key per rules described in
        ''' Bitcoin Wiki article "Mini private key format".
        ''' Zero or negative indicates not a valid Mini Private Key.
        ''' -1 means well formed but fails typo check.
        ''' </summary>
        Public Shared Function IsValidMiniKey(ByVal candidate As String) As Integer
            If candidate.Length <> 22 AndAlso candidate.Length <> 26 AndAlso candidate.Length <> 30 Then
                Return 0
            End If
            If candidate.StartsWith("S") = False Then
                Return 0
            End If
            Dim reg As New System.Text.RegularExpressions.Regex("^S[1-9A-HJ-NP-Za-km-z]{21,29}$")
            If reg.IsMatch(candidate) = False Then
                Return 0
            End If
            Dim ahash As Byte() = Util.ComputeSha256(candidate & "?")
            ' first round
            If ahash(0) = 0 Then
                Return 1
            End If
            ' for (int ct = 0; ct < 716; ct++) ahash = sha256.ComputeHash(ahash); // second thru 717th
            ' if (ahash[0] == 0) return 1;
            Return -1
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function

    End Class
End Namespace