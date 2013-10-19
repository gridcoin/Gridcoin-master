Imports Finisar.SQLite
Imports System.Text
Imports System.IO
Imports Microsoft.VisualBasic



Public Class data

    Const CONNECTION_STR As String = "Data Source=gridcoin.db;Version=3;"




    Private mobjConn As SQLiteConnection
    Private mobjCommand As SQLiteCommand
    Private mobjReader As SQLiteDataReader




    Private Sub CreateDatabase()

        Dim objConn As SQLiteConnection
        Dim objCommand As SQLiteCommand

        If DatabaseExists() = True Then Exit Sub

        'Note - use New=True to create a new database
        objConn = New SQLiteConnection(CONNECTION_STR & "New=True;")
        objConn.Open()
        objCommand = objConn.CreateCommand()
        sql = "create table system (id integer primary key, key varchar(30),value varchar(30));"
        objCommand.CommandText = sql
        objCommand.ExecuteNonQuery()

        sql = "Insert into System (id,key,value) values (1,'Version','1.0');"
        objCommand.CommandText = sql
        objCommand.ExecuteNonQuery()
        'Setup and execute the command SQL to create a new table
        objCommand.CommandText = "CREATE TABLE Peers (id integer primary key, ip varchar(30), computer varchar(40));"
        objCommand.ExecuteNonQuery()
        objCommand.CommandText = "INSERT INTO Peers (id, ip, computer) VALUES (1,'127.0.0.1','rachalupa-pc');"
        objCommand.ExecuteNonQuery()

        'Cleanup and close the connection
        If Not IsNothing(objConn) Then
            objConn.Close()
        End If

    End Sub



    Public Function Query(ByVal ssql As String) As SQLiteDataReader

       
        Try
            mobjConn = New SQLiteConnection(CONNECTION_STR)
            mobjConn.Open()

            mobjCommand = mobjConn.CreateCommand()
            mobjCommand.CommandText = ssql

            mobjReader = mobjCommand.ExecuteReader()


        Catch ex As Exception
            Stop

        Finally
            'Cleanup and close the connection
           

        End Try

        Return mobjReader

    End Function
    Public Sub New()


        CreateDatabase()


    End Sub



    Private Function DatabaseExists() As Boolean


        Dim z As String


        Try

            'Create a new database connection
            mobjConn = New SQLiteConnection(CONNECTION_STR)

            mobjConn.Open()

            'Create a new SQL command to read all records from the customer table
            mobjCommand = mobjConn.CreateCommand()

            Dim sql As String
            sql = "Select Value From System where key='Version';"


            mobjCommand.CommandText = sql

            'Execute the command returning a reader object
            mobjReader = mobjCommand.ExecuteReader()


            mobjReader.Read()

            z = CStr(mobjReader.Item("Value"))

            


        Catch ex As Exception
            Return False
        Finally

            If Not IsNothing(mobjConn) Then
                mobjConn.Close()
            End If


        End Try



        If Val(z) > 0 Then Return True

    End Function




    Public Function Exec(ByVal sSQL As String) As Boolean
        Try

        mobjConn = New SQLiteConnection(CONNECTION_STR)
        mobjConn.Open()
        mobjCommand = mobjConn.CreateCommand()
        mobjCommand.CommandText = sSQL
        mobjCommand.ExecuteNonQuery()
        Catch ex As Exception
            Stop

        End Try

    End Function


    Public Function IPExists(ByVal sIP As String) As Boolean

        sql = "Select IP from Peers where IP='" & sIP & "'"
        mobjReader = Query(sql)

        If Not mobjReader Is Nothing Then
            mobjReader.Read()
            Dim sReadIP As String
            sReadIP = mobjReader("IP")
            If sReadIP = sIP Then
                mobjConn.Close()

                Return True
            End If
        End If
        mobjConn.Close()

        Return False

    End Function



    Public Function AddIP(ByVal sIP As String, ByVal sComputer As String) As Boolean
        If IPExists(sIP) Then Exit Function
        sql = "Insert into Peers (ip,computer) values ('" & sIP & "','" & sComputer & "');"
        Exec(sql)

    End Function
End Class
