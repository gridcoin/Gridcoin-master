
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

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        '       Dim sSource As String = "706\-2:1236112FF1236112\100\490\7\0\fight_0_25251:malar_909_705879:Leide_515_115841:World_615_2504147:Docki_1558_189058:roset_728_1651516:SETI@_576_7179401\1390603039\2\17546"
        Dim sSource As String = "ABCDE70554751158\-2:1236112FF1236112\100\490\7\0\fight_0_25251:malar_909_705879:Leide_515_115841:World_615_2504147:Docki_1558_189058:roset_728_1651516:SETI@_576_7179401\1390605012\2\6847"
        Dim sResult As String = "1BEE2141A100B82305FED972572AF4904908E061"


        Dim lResult As Long
        Dim bHash() As Byte
        Dim cHash As String

        bHash = System.Text.Encoding.ASCII.GetBytes(sSource)

        Dim objSHA1 As New SHA1CryptoServiceProvider()

        cHash = Replace(BitConverter.ToString(objSHA1.ComputeHash(bHash)), "-", "")

        Dim xx As New Utilization

        Dim l As Long
        l = xx.CheckWork("1BEE2141A100B82305FED972572AF4904908E061", "", "", "", sSource)

        Stop




        Stop




        'Compare .net sha1 to vb6 sha1:
        
        'ReHash the Source Hash
        bHash = System.Text.Encoding.ASCII.GetBytes("HELLO WORLD")
        
        cHash = Replace(BitConverter.ToString(objSHA1.ComputeHash(bHash)), "-", "")

        Stop


        Dim xxx As New frmSQL
        xxx.Show()
        Exit Sub



        Dim x As String
        x = String.Format("{0:p}", 99.32)

        Stop



        Dim g As New frmLeaderboard
        g.Show()

        Exit Sub



        Dim f As New Utilization


        Dim bOutdated As Boolean
        bOutdated = f.TestOutdated(f.TestKeyValue("UpdatedLeaderboard"), 10)
        f.TestUpdateKey("UpdatedLeaderboard", Trim(Now))

        bOutdated = f.TestOutdated(f.TestKeyValue("UpdatedLeaderboard"), 10)

        Stop


        Dim zz As New frmLeaderboard
        zz.Show()


        Dim s As New frmSQL
        s.Show()


        Exit Sub

        'Dim b As New frmLeaderboard
        'b.Show()
        'Exit Sub



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
