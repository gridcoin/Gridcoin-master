Imports Finisar.SQLite
Imports System.Text
Imports System.IO
Imports Microsoft.VisualBasic
Public Class Sql

    Private mobjConn As SQLiteConnection
    Private mobjConnQ As SQLiteConnection
    Private mobjConnExec As SQLiteConnection
    Private mobjCommand As SQLiteCommand
    Private mobjReader As SQLiteDataReader
    Private mobjReaderQ As SQLiteDataReader
    Private sDatabaseName As String = "gridcoin"
    Private bClean As Boolean
    Private mSql As String = ""
    Private mStr As String = ""
    Private Function CONNECTION_STR() As String
        Dim sPath As String
        Dim sFolder As String = GetGridFolder() + "Sql\"
        Try
            If Not System.IO.Directory.Exists(sFolder) Then MkDir(sFolder)
        Catch ex As Exception
        End Try
        sPath = sFolder + sDatabaseName
        sPath = Replace(sPath, "\", "//")
        Dim s As String = "Data Source=" + sPath + ";Version=3;"
        Return s
    End Function
    Private Function DatabaseExists() As Boolean
        Try
            'Create a new database connection
            mobjConn = New SQLiteConnection(CONNECTION_STR)
            mobjConn.Open()
            mobjCommand = mobjConn.CreateCommand()
            Dim sql As String
            sql = "Select Value From System where id='1';"
            mobjCommand.CommandText = sql
            'Execute the command returning a reader object
            mobjReader = mobjCommand.ExecuteReader()
            mobjReader.Read()
            mStr = CStr(mobjReader.Item("Value"))
            mobjReader.Close()
        Catch ex As Exception
            Return False
        Finally
            If Not IsNothing(mobjConn) Then
                mobjConn.Close()
            End If
        End Try
        If Val(mStr) > 0 Then Return True

    End Function

    Public Sub SqlHousecleaning()
        If bSqlHouseCleaningComplete Then Exit Sub
        Dim sql As String
        bSqlHouseCleaningComplete = True
        Try
            sql = "Delete from Peers where added < date('now','start of month','+1 month','+1 day');"
            Exec(sql)
            Close()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub CreateDatabase()
        If DatabaseExists() = True Then
            Exit Sub
        End If
        'New=True to create a new database
        Try
            mobjConn.Close()
        mobjConn = New SQLiteConnection(CONNECTION_STR() & "New=True;")
        mobjConn.Open()
        mobjCommand = mobjConn.CreateCommand()
            mSql = "create table System (id integer primary key, key varchar(30),value varchar(30),Added datetime);"
        mobjCommand.CommandText = mSql
        mobjCommand.ExecuteNonQuery()
            mSql = "Insert into System (id,key,value,added) values (1,'Gridcoin_Version','1.0',date('now'));"
        mobjCommand.CommandText = mSql
        mobjCommand.ExecuteNonQuery()
            mobjCommand.CommandText = "CREATE TABLE Peers (id integer primary key, ip varchar(30), version varchar(40), Added datetime);"
            mobjCommand.ExecuteNonQuery()
            mobjCommand.CommandText = "INSERT INTO Peers (id, ip, version, Added) VALUES (1,'127.0.0.1','71002',date('now'));"
            mobjCommand.ExecuteNonQuery()
            mobjCommand.CommandText = "INSERT INTO Peers (id, ip, version, Added) VALUES (2,'127.0.0.1','71002',date('now','start of month','+1 month','-2 day'));"
            mobjCommand.ExecuteNonQuery()
            mobjCommand.CommandText = "CREATE TABLE Leaderboard (id integer primary key, Added datetime, Address varchar(100), Host varchar(50), Project varchar(40), Credits numeric(12,2), ProjectName varchar(100), ProjectURL varchar(150));"
            mobjCommand.ExecuteNonQuery()
            mSql = "CREATE TABLE Blocks (height integer primary key,hash varchar(100),confirmations varchar(30)," _
            & "blocksize varchar(10),version varchar(10), merkleroot varchar(200), tx varchar(900), " _
            & "blocktime varchar(12), nonce varchar(15), bits varchar(15), difficulty varchar(20), boinchash varchar(900), previousblockhash varchar(200), nextblockhash varchar(200)); "
        mobjCommand.CommandText = mSql
        mobjCommand.ExecuteNonQuery()
            'Cleanup
        If Not IsNothing(mobjConn) Then
            mobjConn.Close()
            End If
            Exit Sub
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
  
    Public Function InsertBlocks(data As String)
        Dim vBlocks() As String = Split(data, "{ROW}")
        If UBound(vBlocks) < 5 Then Exit Function
        Try
            For i As Integer = 0 To UBound(vBlocks)
                If Len(vBlocks(i)) > 100 Then

                    InsertBlock(vBlocks(i))
                End If
            Next
        Catch ex As Exception
            Log("InsertBlocks: " + data + ":" + ex.Message)
        End Try

    End Function
    Public Function InsertBlock(data As String)
        Dim sql As String
        sql = "Insert into Blocks (hash,confirmations,blocksize,height,version,merkleroot,tx,blocktime,nonce,bits,difficulty,boinchash,previousblockhash,nextblockhash) VALUES "
        Dim vData() As String
        vData = Split(data, "|")
        Dim sValues As String
        If UBound(vData) < 13 Then Exit Function
        vData(1) = vData(3) 'Confirms = Height since it is dynamic
        vData(11) = Replace(vData(11), "'", "")
        vData(11) = Replace(vData(11), Chr(0), "")
        For x = 0 To UBound(vData)
            sValues = sValues + "'" + vData(x) + "',"
        Next
        If Len(sValues) > 1 Then sValues = Left(sValues, Len(sValues) - 1)
        sql = sql + "(" + sValues + ")"
        Exec(sql)
    End Function
    Public Function HighBlockNumber() As Long
        Try
            mSql = "Select max(height) as HighBlock from Blocks"
        Dim lBlock As Long
            lBlock = Val(QueryFirstRow(mSql, "HighBlock"))
            Return lBlock
        Catch ex As Exception
            Log(ex.Message)
            Return 0
        End Try
    End Function
    Public Function Query(ByVal ssql As String) As SQLiteDataReader
        Try
            If Not mobjConn Is Nothing Then mobjConn.Close()
            mobjConn = New SQLiteConnection(CONNECTION_STR)
            mobjConn.Open()
            mobjCommand = mobjConn.CreateCommand()
            mobjCommand.CommandText = ssql
            mobjReader = mobjCommand.ExecuteReader()
        Catch ex As Exception
            Log("Query:" + CONNECTION_STR() + ":" + ssql + ":" + ex.Message)
        Finally
            mobjCommand = Nothing
        End Try
        Return mobjReader

    End Function
    Public Function QueryFirstRow(ByVal sSql As String, sCol As String) As Object
        Try
            If Not mobjConnQ Is Nothing Then mobjConnQ.Close()
            mobjConnQ = New SQLiteConnection(CONNECTION_STR)
            mobjConnQ.Open()
            mobjCommand = mobjConnQ.CreateCommand()
            mobjCommand.CommandText = sSql
            mobjReaderQ = mobjCommand.ExecuteReader()
            Dim oOut As Object
            mobjReaderQ.Read()
            oOut = mobjReaderQ(sCol)
            Return oOut
        Catch ex As Exception
            Log("Sql.QueryFirstRow:" + CONNECTION_STR() + ":" + sSql + ":" + ex.Message)
        Finally
        End Try
        Return mobjReader
    End Function
    Public Sub New()
        sDatabaseName = "gridcoin"
        CreateDatabase()
        If bClean = False Then SqlHousecleaning()
    End Sub

    Public Sub New(strDatabaseName As String)
        sDatabaseName = strDatabaseName
        CreateDatabase()
        If bClean = False Then SqlHousecleaning()
    End Sub

    Public Function Exec(ByVal sSQL As String) As Boolean
        Try
            If Not mobjConnExec Is Nothing Then mobjConnExec.Close()
            mobjConnExec = New SQLiteConnection(CONNECTION_STR)
            mobjConnExec.Open()
            mobjCommand = mobjConnExec.CreateCommand()
            mobjCommand.CommandText = sSQL
            mobjCommand.ExecuteNonQuery()
            mobjConnExec.Close()
        Catch ex As Exception
            Log("Sql.Exec:" + sSQL + ":" + ex.Message)
            mobjConnExec.Close()

            Return False
        End Try
        Return True
    End Function

    Public Sub Close()
        Try
            If Not mobjConn Is Nothing Then mobjConn.Close()
            If Not mobjConnQ Is Nothing Then mobjConnQ.Close()
            If Not mobjConnExec Is Nothing Then mobjConnExec.Close()
            mobjCommand = Nothing
        Catch ex As Exception
        End Try
    End Sub
    Protected Overrides Sub Finalize()
        Close()
        MyBase.Finalize()
    End Sub
End Class
