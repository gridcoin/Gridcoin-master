Module modBoincLeaderboard

    Public Structure BoincProject
        Public URL As String
        Public Name As String
        Public Credits As Double


    End Structure
    Public bSqlHouseCleaningComplete As Boolean = False

    Public vProj() As String

    Sub New()
        ReDim vProj(100)
        vProj(0) = "http://boinc.bakerlab.org/rosetta/   |rosetta@home"
        vProj(1) = "http://docking.cis.udel.edu/         |Docking"
        vProj(2) = "http://www.malariacontrol.net/       |malariacontrol.net"
        vProj(3) = "http://www.worldcommunitygrid.org/   |World Community Grid"
        vProj(4) = "http://asteroidsathome.net/boinc/    |Asteroids@home"
        vProj(5) = "http://climateprediction.net/        |climateprediction.net"
        vProj(6) = "http://pogs.theskynet.org/pogs/      |pogs"
        vProj(7) = "http://boinc.umiacs.umd.edu/         |The Lattice Project"
        vProj(8) = "http://setiathome.berkeley.edu/      |SETI@home"
        vProj(9) = "http://radioactiveathome.org/boinc/  |Radioactive@Home"
        vProj(10) = "http://qcn.stanford.edu/sensor/     |Quake-Catcher Network"
        vProj(11) = "http://boinc.gorlaeus.net/          |Leiden Classical"
        vProj(12) = "http://home.edges-grid.eu/home/     |EDGeS@Home"
        vProj(13) = "http://milkyway.cs.rpi.edu/milkyway/|Milkyway@Home"
        vProj(14) = "http://spin.fh-bielefeld.de/        |Spinhenge@home"
    End Sub
    Public Function CodeToProject(sCode As String) As BoincProject
        Dim bp As New BoincProject

        Dim vRow() As String
        sCode = Trim(LCase(sCode))
        If sCode = "" Then Return bp

        For y As Integer = 0 To UBound(vProj)
            If Len(vProj(y)) > 10 Then
                vRow = Split(vProj(y), "|")
                If UBound(vRow) = 1 Then
                    If Left(LCase(vRow(1)), Len(sCode)) = sCode Then
                        bp.Name = Trim(vRow(1))
                        bp.URL = Trim(vRow(0))
                        Return bp
                    End If

                End If
            End If
        Next
        Return bp
    End Function

    Public Function RefreshLeaderboard()
        Log("Updating Leaderboard")
        Dim sql As String
        Dim d As New Sql
        Dim dBlock As Double = d.HighBlockNumber
        Dim lBlock As Double = dBlock - 17856 '30 days back
        If lBlock < 1 Then lBlock = 1
        d.Close()
        sql = "Delete from Leaderboard" 'Truncate Table
        d.Exec(sql)
        '''''''''Fake temporary data useful for sample queries until all the clients sync blocks into sql server: (1-1-2014)
        If True Then
            Dim sHash2 As String
            sHash2 = "xa3,1, 98, CRD_V, SOLO_MIN, GBZkHyR7sKXfdh1Z7FMxbsLB,  23, 2854:2963:2969  ,483CB1696,310830774fefd00fb888761f0e\1:3a94913164b731f5c712e4a7852575a3\50\2969\3\58842\World_1000_175:MILKY_2000_275:SETI_3000_375\1386004003\2\270722"
            For x = 1 To 10
                sql = "Update Blocks set boinchash = '" + sHash2 + "' where height = '" + Trim(x) + "';"
                d.Exec(sql)
            Next
            lBlock = 0
        Else
            If lBlock < 100 Then Exit Function
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        sql = "Select height,Boinchash From blocks where height > " + Trim(lBlock) + ";"
        Dim dr As Finisar.SQLite.SQLiteDataReader
        dr = d.Query(sql)

        'MD5, Avg_Credits, UTILIZATION,  CRD_V, pool_mode, DefaultWalletAddress() ,RegVer,BoincDeltaOverTime,MinedHash, sSourceBlock;
        'xa3, 1,           98,           CRD_V, SOLO_MIN, GBZkHyR7sKXfdh1Z7FMxbsLB,    23,2854:2963:2969 ,483CB1696,00fb888761f0e\1:3a94913164b731f5c712e4a7852575a3\50\2969\3\58842\roset:fight\1386004003\2\270722
        '310838f9d793552\1:3a94913164b731f5c712e4\50    \2969 \3  \58842\roset:fight \1386004003\2  \270722
        'BoincSolvedHash\Credits:OriginalSha1Hash\Utiliz\AvgCr\thr\totcr\ProjExpanded\UnixTime  \pCo\Nonce
        Dim sqlI As String = ""
        Dim sHash As String
        Dim vHash() As String
        While dr.Read
            sHash = dr("boinchash")
            vHash = Split(sHash, ",")
            Dim sSourceBlock As String
            Dim vSourceBlock() As String
            If UBound(vHash) >= 9 Then
                sSourceBlock = vHash(9)
                vSourceBlock = Split(sSourceBlock, "\")
                Dim sExpandedProjects As String
                Dim vExpandedProjects() As String
                If UBound(vSourceBlock) > 8 Then
                    sExpandedProjects = vSourceBlock(6)
                    'sExpandedProjects = "World_1000_100:MILKY_2000_200:SETI_3000_300"
                    If sExpandedProjects.Contains("_") Then
                        vExpandedProjects = Split(sExpandedProjects, ":")
                        Dim sProjData As String
                        Dim vProjData() As String
                        For x As Integer = 0 To UBound(vExpandedProjects)
                            sProjData = vExpandedProjects(x)
                            vProjData = Split(sProjData, "_")
                            If UBound(vProjData) = 2 Then
                                Dim dCredits As Double
                                Dim sProject As String
                                Dim sHost As String
                                sHost = vProjData(2)
                                sProject = vProjData(0)
                                dCredits = Val(vProjData(1))
                                Dim sGRCAddress As String
                                sGRCAddress = vHash(5)
                                Dim bp As New BoincProject
                                bp = CodeToProject(sProject)
                                If Len(bp.URL) > 1 Then
                                    sqlI = sqlI + "Insert into LeaderBoard (Added, Address, Host, Project, Credits, ProjectName, ProjectURL) VALUES " _
                                                          & "(date('now'),'" + sGRCAddress + "','" + sHost + "','" + sProject + "','" + Trim(dCredits) + "','" + bp.Name _
                                                          + "','" + bp.URL + "');"



                                End If
                            End If
                        Next
                    End If
                End If
            End If
        End While
        d.Close()
        d.Exec(sqlI)
        d = Nothing
    End Function

    'Copy the prod database to the read only database:
    Public Function ReplicateDatabase()
        Dim sPath As String = GetGridFolder() + "Sql\gridcoin"
    Dim sROPath As String = GetGridFolder() + "Sql\gridcoin_ro"
        Try
            FileCopy(sPath, sROPath)
        Catch ex As Exception
        End Try
    End Function

End Module
