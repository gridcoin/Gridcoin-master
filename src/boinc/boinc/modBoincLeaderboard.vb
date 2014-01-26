Module modBoincLeaderboard
    Public mlSqlBestBlock As Long = 0
    Public nBestBlock As Long = 0
    Public mlLeaderboardPosition As Long = 0
    Public mdScryptSleep As Double = 0
    Public msBlockSuffix As String = ""
    Public msSleepStatus As String = ""
    Public mdBlockSleepLevel As Double = 0

    Public Structure BoincProject
        Public URL As String
        Public Name As String
        Public Credits As Double


    End Structure
    Public bSqlHouseCleaningComplete As Boolean = False

    Public vProj() As String
    Public Function SQLInSync() As Boolean
        If mlSqlBestBlock < 40000 Then Return False
        If nBestBlock < 40000 Then Return False

        If mlSqlBestBlock > nBestBlock - 6 Then Return True
        Return False
    End Function
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
        vProj(15) = "http://casathome.ihep.ac.cn/        |CAS@home"
        vProj(16) = "http://aerospaceresearch.net/constellation/|Constellation"
        vProj(17) = "http://www.cosmologyathome.org/     |Cosmology@Home"
        vProj(18) = "http://boinc.freerainbowtables.com/ |DistrRTgen"
        vProj(19) = "http://einstein.phys.uwm.edu/       |Einstein@Home"
        vProj(20) = "http://www.enigmaathome.net/        |Enigma@Home"
        vProj(21) = "http://boinc.ucd.ie/fmah/           |fightmalaria@home"
        vProj(22) = "http://registro.ibercivis.es/       |ibercivis"
        vProj(23) = "http://lhcathomeclassic.cern.ch/sixtrack/|LHC@home 1.0"
        vProj(24) = "http://lhcathome2.cern.ch/test4theory|Test4Theory@Home"
        vProj(25) = "http://mindmodeling.org/            |MindModeling@Beta"
        vProj(26) = "http://escatter11.fullerton.edu/nfs/|NFS@Home"
        vProj(27) = "http://numberfields.asu.edu/NumberFields/|NumberFields@home"
        vProj(28) = "http://oproject.info/               |OProject@Home"
        vProj(29) = "http://boinc.fzk.de/poem/           |Poem@Home"
        vProj(30) = "http://www.primegrid.com/           |PrimeGrid"
        vProj(31) = "http://www.rnaworld.de/rnaworld/    |RNA World"
        vProj(32) = "http://sat.isa.ru/pdsat/            |SAT@home"
        vProj(33) = "http://boincsimap.org/boincsimap/   |boincsimap"
        vProj(34) = "http://szdg.lpds.sztaki.hu/szdg/    |SZTAKI Desktop Grid"
        vProj(35) = "http://mmgboinc.unimi.it/           |SimOne@home"
        vProj(36) = "http://volunteer.cs.und.edu/subset_sum/|SubsetSum@Home"
        vProj(37) = "http://boinc.vgtu.lt/vtuathome/     |VGTU project@Home"
        vProj(38) = "http://volpex.cs.uh.edu/VCP/        |VolPEx"
        vProj(39) = "http://www.rechenkraft.net/yoyo/    |yoyo@home"
        vProj(40) = "http://eon.ices.utexas.edu/eon2/    |eon2"


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
    Public Function GlobalizedDecimal(ByVal data As Object) As String
        Try
            Dim sOut As String
            sOut = Trim(data)
            If sOut.Contains(",") Then
                sOut = Replace(sOut, ",", "|")
                sOut = Replace(sOut, ".", "")
                sOut = Replace(sOut, "|", ".")

            End If

            Return sOut
        Catch ex As Exception
            Return Trim(data)
        End Try
    End Function
    Public Function RefreshLeaderboardFactors(d As Sql)

        Dim sql As String = ""


        Try

            Dim vRow() As String
            Dim dAvg As Double
            '''''''''''''''''''''''''''1-22-2014 D37D: Purge duplicate Host Records before trusting data:
            sql = "Select id,host,address      from leaderboard    group by address     order by host,id"

            Dim gr1 As New GridcoinReader
            gr1 = d.GetGridcoinReader(sql)
            Dim grr1 As GridcoinReader.GridcoinRow
            Dim grForwardRow As GridcoinReader.GridcoinRow
            Dim lPurged As Long

            For y1 As Integer = 1 To gr1.Rows - 1
                grr1 = gr1.GetRow(y1)
                grForwardRow = gr1.GetRow(y1 + 1)
                Dim sHost As String
                sHost = gr1.Value(y1, "Host")
                Dim sForwardHost As String
                sForwardHost = gr1.Value(y1 + 1, "Host")
                If sForwardHost = sHost And gr1.Value(y1, "Address") <> gr1.Value(y1 + 1, "Address") Then
                    'This host changed GRC address during the month! Purge older records:
                    sql = "Delete from Leaderboard where id = '" + Trim(gr1.Value(y1, "id")) + "'"
                    lPurged = lPurged + 1
                    d.Exec(sql)
                End If

            Next y1
            Log("Purged " + Trim(lPurged) + "records.")

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            sql = "Select  avg(credits) as [Credits] from  leaderboard  "
            dAvg = Val(d.QueryFirstRow(sql, "Credits"))

            For y As Integer = 0 To UBound(vProj)
                If Len(vProj(y)) > 10 Then
                    vRow = Split(vProj(y), "|")
                    If UBound(vRow) = 1 Then
                        Dim sProjName As String = vRow(1)
                        sql = "Update Leaderboard set Factor = (Select (" + GlobalizedDecimal(dAvg) _
                            + "/avg(credits)) as [Credits] from  leaderboard LL " _
                            & " where LL.projectname='" + Trim(sProjName) + "') WHERE ProjectName = '" + sProjName + "'"
                        d.Exec(sql)
                    End If
                End If
            Next y
            sql = "Update leaderboard set Factor = 4 where Factor > 4"
            d.Exec(sql)
            sql = "Update Leaderboard set AdjCredits = Factor*Credits where 1=1 "
            d.Exec(sql)
            'Update scrypt sleep for the network:
            '1-8-2014
            sql = " Select avg(credits*factor*projectcount) as [AdjCredits], Address from " _
                & " leaderboard group by Address order by avg(credits*factor*projectcount) desc "
            Dim gr As New GridcoinReader
            gr = d.GetGridcoinReader(sql)
            Dim grr As GridcoinReader.GridcoinRow
            Dim sleepfactor = gr.Rows / 50
            Dim chance As Double = 0
            Dim sleeppercent As Double = 0

            For y As Integer = 1 To gr.Rows
                grr = gr.GetRow(y)
                chance = y / sleepfactor
                sleeppercent = 100 - chance
                If sleeppercent > 94.5 Then sleeppercent = 100 'This allows the top 10% to hash 100% of the time
                If sleeppercent < 50 Then sleeppercent = 50
                'Globalizaton: store as a decimal from 0 - 1 (meaning 0-100%)
                sleeppercent = sleeppercent / 100
                '1-19-2013
                sql = "Update Leaderboard Set ScryptSleepChance = " + GlobalizedDecimal(sleeppercent) _
                    + " where address = '" + gr.Value(y, "Address") + "'"
                d.Exec(sql)
            Next y

            gr = Nothing
            grr = Nothing
        Catch ex As Exception
            Log("Refresh Leaderboard factors: " + ex.Message + " " + sql + ex.InnerException.ToString)
        End Try


    End Function
    Public Function RefreshLeaderboard()

        If KeyValue("UpdatingLeaderboard") = "true" Then Exit Function

        Try

            UpdateKey("UpdatingLeaderboard", "true")



            Log("Updating Leaderboard")
            Dim sql As String
            Dim d As New Sql
            d.CreateLeaderboardTable()

            Dim dBlock As Double = d.HighBlockNumber
            Dim lBlock As Double = dBlock - 11520 '20 days back
            If lBlock < 1 Then lBlock = 1
          
            sql = "Delete from Leaderboard" 'Truncate Table
            d.Exec(sql)
            '''''''''Fake temporary data useful for sample queries until all the clients sync blocks into sql server: (1-1-2014)
            If False Then
                Dim sHash2 As String
                sHash2 = "xa3,1, 98, CRD_V, SOLO_MIN, GBZkHyR7sKXfdh1Z7FMxbsLB,  23, 2854:2963:2969  ,483CB1696,310830774fefd00fb888761f0e\1:3a94913164b731f5c712e4a7852575a3\50\2969\3\58842\World_1000_175:MILKY_2000_275:SETI_3000_375\1386004003\2\270722"
                For x = 1 To 10
                    sql = "Update Blocks set boinchash = '" + sHash2 + "' where height = '" + GlobalizedDecimal(x) + "';"
                    d.Exec(sql)
                Next
                lBlock = 0
            Else
                If lBlock < 100 Then Exit Function
            End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            sql = "Select height, Boinchash From blocks where height > " + GlobalizedDecimal(lBlock) + " order by height;"
            Dim dr As GridcoinReader
            Log("Gathering data for Leaderboard: " + Trim(sql))

            dr = d.GetGridcoinReader(sql)


            'MD5, Avg_Credits, UTILIZATION,  CRD_V, pool_mode, DefaultWalletAddress() ,RegVer,BoincDeltaOverTime,MinedHash, sSourceBlock;
            'xa3, 1,           98,           CRD_V, SOLO_MIN, GBZkHyR7sKXfdh1Z7FMxbsLB,    23,2854:2963:2969 ,483CB1696,00fb888761f0e\1:3a94913164b731f5c712e4a7852575a3\50\2969\3\58842\roset:fight\1386004003\2\270722
            '310838f9d793552\1:3a94913164b731f5c712e4\50    \2969 \3  \58842\roset:fight \1386004003\2  \270722
            'BoincSolvedHash\Credits:OriginalSha1Hash\Utiliz\AvgCr\thr\totcr\ProjExpanded\UnixTime  \pCo\Nonce
            Dim sqlI As String = ""
            Dim sHash As String
            Dim vHash() As String
            Dim sbi As New System.Text.StringBuilder
            Dim grr As GridcoinReader.GridcoinRow

            For y = 1 To dr.Rows
                grr = dr.GetRow(y)

                sHash = dr.Value(y, "boinchash")

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
                            'Precalculate active project count:
                            Dim lProjCount As Long = 0

                            For pr As Integer = 0 To UBound(vExpandedProjects)
                                sProjData = vExpandedProjects(pr)
                                vProjData = Split(sProjData, "_")
                                If UBound(vProjData) = 2 Then
                                    Dim dCredits As Double
                                    Dim sProject As String
                                    Dim sHost As String
                                    sHost = vProjData(2)
                                    sProject = vProjData(0)

                                    dCredits = Val(vProjData(1))
                                    If dCredits > 0 Then lProjCount += 1

                                End If

                            Next pr
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
                                        If dCredits > 0 Then
                                            ' ProjectCount integer, Factor numeric(12,2), AdjCredits
                                            sbi.AppendLine("Insert into LeaderBoard (Added, Address, Host, Project, Credits, ProjectName, ProjectURL, ProjectCount, Factor, AdjCredits) VALUES " _
                                                              & "(date('now'),'" + sGRCAddress + "','" + sHost + "','" + sProject + "','" _
                                                              + GlobalizedDecimal(dCredits) + "','" + bp.Name _
                                                              + "','" + bp.URL + "'," + GlobalizedDecimal(lProjCount) + ",'0','0');")
                                           
                                        End If

                                    End If
                                End If
                            Next
                        End If
                    End If
                End If
            Next y
            
            d.ExecHugeQuery(sbi)
            'Update Project Count
            Log("Updating factors")

            RefreshLeaderboardFactors(d)
            'd.UpdateUserSummary()
            Log("Updated Leaderboard")

            Try
                ReplicateDatabase("gridcoin_leaderboard")
            Catch ex As Exception

            End Try

            'Update the sync key

            UpdateKey("UpdatedLeaderboard", Trim(Now))
            d = Nothing
            UpdateKey("UpdatingLeaderboard", "false")

        Catch ex As Exception
            Log("Refresh leaderboard: " + ex.Message)
            UpdateKey("UpdatingLeaderboard", "false")

        End Try
        UpdateKey("UpdatingLeaderboard", "false")

    End Function
    Public Function Outdated(ByVal data As String, ByVal mins As Long) As Boolean
        Try

        If Trim(data) = "" Then Return True
        If IsDate(data) = False Then Return True
        Dim lMins As Long
            lMins = Math.Abs(DateDiff(DateInterval.Minute, Now, CDate(data)))

        If lMins > mins Then Return True
            Return False
        Catch ex As Exception
            Return True
        End Try

    End Function
    Public Function DatabaseExists(ByVal sDatabaseName As String) As Boolean
        Return System.IO.File.Exists(GetGridFolder() + "Sql\" + sDatabaseName)

    End Function
    'Copy the prod database to the read only database:
    Public Function ReplicateDatabase(ByVal sTargetDatabaseName As String)
        Dim sPath As String = GetGridFolder() + "Sql\gridcoin"
        Dim sROPath As String = GetGridFolder() + "Sql\" + sTargetDatabaseName
        Try
            FileCopy(sPath, sROPath)
        Catch ex As Exception
        End Try
    End Function
    Public Function xUnlockDatabase()
        Dim sPath As String = GetGridFolder() + "Sql\gridcoin"
        Dim sROPath As String = GetGridFolder() + "Sql\gridcoin_copy"
        Try
            If System.IO.File.Exists(sPath) = False Then Exit Function
            FileCopy(sPath, sROPath)
            System.IO.File.Delete(sPath)
            FileCopy(sROPath, sPath)
        Catch ex As Exception
            Log("UnlockDatabase:" + ex.Message)
        End Try
    End Function

End Module
