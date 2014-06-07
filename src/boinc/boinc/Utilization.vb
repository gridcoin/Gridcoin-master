Imports Microsoft.VisualBasic
Imports System.Timers

Public Class Utilization
    Implements IGridCoinMining
    'Private mSharedGPUStat() As SharedMemory
    Public _timerSnapshot As System.Timers.Timer
    Private _nBestBlock As Long
    Private _lLeaderboard As Long
    Private _lLeaderUpdates As Long
    Private _boincmagnitude As Double

    Public Structure CgSumm
        Public Mhs As Double
        Public Accepted As Double
        Public Rejected As Double
        Public Invalid As Double
        Public Stale As Double
        Public Message As String
    End Structure

    Public ReadOnly Property Version As Double
        Get
            Return 158



        End Get
    End Property

    Private lfrmMiningCounter As Long = 0
    Public ReadOnly Property BoincUtilization As Double
        Get
            Return Val(clsGVM.BoincUtilization)
        End Get
    End Property

    Public Property BoincMagnitude As Double
        Get
            Return _boincmagnitude
        End Get
        Set(value As Double)
            _boincmagnitude = value
        End Set
    End Property


    Public ReadOnly Property ClientNeedsUpgrade As Double
        Get
            Dim bNeedsUp As Boolean = NeedsUpgrade()
            If bNeedsUp Then
                Log("Client outdated; needs upgraded.")

                If KeyValue("suppressupgrade") = "true" Then
                    Log("Client needs upgraded; Not upgrading due to key.")

                    Return 0
                End If
                Return 1
            End If
            Log("Client up to date")

            Return 0
        End Get
    End Property
    Public ReadOnly Property BoincThreads As Double
        Get
            Return Val(clsGVM.BoincThreads)
        End Get
    End Property
    Sub New()
        UpdateKey("UpdatingLeaderboard", "false")
        Try
            If Not DatabaseExists("gridcoin_leaderboard") Then ReplicateDatabase("gridcoin_leaderboard")

        Catch ex As Exception
            Log("New:" + ex.Message)
        End Try
        If clsGVM Is Nothing Then
            clsGVM = New GridcoinVirtualMachine.GVM

        End If
        mclsUtilization = Me

    End Sub
    Sub New(bLoadMiningConsole As Boolean)
        If clsGVM Is Nothing Then clsGVM = New GridcoinVirtualMachine.GVM
        If bLoadMiningConsole Then ShowMiningConsole()
    End Sub
    Public Function CgMinerDeviceSummary(lDevId As Long) As CgSumm
        Return modCgMiner.CgSummary(lDevId)
    End Function
    Public ReadOnly Property CalcApiUrl(lProj As Long, sUserId As String) As String
        Get
            Return Trim(clsGVM.CalcApiUrl(lProj, sUserId))
        End Get
    End Property
    Public Function GetGRCSleepStatus(sAddress As String, sBlockhash As String) As Boolean
        Dim bSL As Boolean = False
        Dim dGRCSleepLevel As Double = 0
        Dim dNetLevel As Double = 0
        bSL = GetSleepLevelByAddress(sAddress, dGRCSleepLevel, sBlockhash, dNetLevel)
        Log("Checking sleep level for " + Trim(sAddress) + " level = " + Trim(dGRCSleepLevel) + ", blockhash " + Trim(sBlockhash) _
            + " net level " + Trim(dNetLevel))

        Return bSL
    End Function
    Public Function RefLeaderPos()
        Call mfrmMining.RefreshLeaderboardPosition()

    End Function
    Public Sub RestartWallet()
        Call RestartWallet1("")
    End Sub
    Public Sub UpgradeWallet()
        Call RestartWallet1("upgrade")
    End Sub
    Public Sub UpgradeWalletTestnet()
        Call RestartWallet1("testnetupgrade")
    End Sub


    Public Sub ReindexWallet()
        Call RestartWallet1("reindex")
    End Sub
    Public Sub DownloadBlocks()
        Log("Downloading blocks")

        Call RestartWallet1("downloadblocks")
    End Sub
    Public Sub ReindexWalletTestNet()
        Call RestartWallet1("reindextestnet")
    End Sub
    Public Sub RestoreSnapshot()
        Call RestartWallet1("restoresnapshot")
    End Sub
    Public Sub CreateRestorePoint()
        Call RestartWallet1("createrestorepoint")
    End Sub
    Public Sub CreateRestorePointTestNet()
        Call RestartWallet1("createrestorepointtestnet")
    End Sub
    Public Function xRetrieveGridCoinMiningValue(iDeviceID As Integer, sProperty As String) As String Implements IGridCoinMining.RetrieveGridCoinMiningValue
        Dim sOut As String = modGRC.xReturnMiningValue(iDeviceID, sProperty, True)
        Return sOut
    End Function
    Public Function xRetrieveGridCoinMiningValueWithoutPrefix(iDeviceID As Integer, _
                                                             sProperty As String) As String Implements IGridCoinMining.RetrieveGridCoinMiningValueWithoutPrefix
        Dim sOut As String = modGRC.xReturnMiningValue(iDeviceID, sProperty, False)
        Return sOut
    End Function
    Public ReadOnly Property BoincMD5 As String
        Get
            Return clsGVM.BoincMD5()
        End Get
    End Property
    Public Function StrToMd5Hash(s As String) As String
        Return CalcMd5(s)
    End Function
    Public ReadOnly Property RetrieveWin32BoincHash() As String
        Get
            Dim sHash As String
            sHash = Trim(BoincMD5) + ",CRD_win32," + PoolMode() + "," + clsGVM.PublicWalletAddress + "," _
                + Trim(Val(Version)) _
                + "," + clsGVM.BoincDeltaOverTime + "," + ("" & clsGVM.MinedHash) _
                + "," + Trim("" & clsGVM.SourceBlock) + "," + ("" & Val(clsGVM.BoincProjects)) + "," + Trim(Val(clsGVM.BoincTotalHostAvg))
            Return sHash
        End Get
    End Property
    Public Function AuthenticateToPool() As Boolean
        Dim bResult As Boolean
        Try

            Dim sPoolURL As String = KeyValue("poolurl")


            Dim sPoolUser As String = KeyValue("pooluser")
            Dim sPoolPass As String = KeyValue("poolpassword")
            Dim sMinerName As String = KeyValue("miner")

            If Len(sPoolURL) < 6 Then Return False
            If Len(sPoolUser) = 0 Or Len(sPoolPass) = 0 Then Return False

        bResult = ValidatePoolURL(sPoolURL)
        bResult = True

        If bResult = False Then
            Log("Pool " + sPoolURL + " site certificate invalid.")
            Return False
        End If
            'Authenticate 5-30-2014

            Dim p As String

            '    = mapArgs["-pooluser"];
            '	std::string strAuth2 = mapArgs["-poolpassword"];
            '	std::string strAuth3 = mapArgs["-miner"];

            p = sPoolUser + "<;>" + sPoolPass + "<;>" + sMinerName + "<;>projectname<;>1<;>bpk<;>cpid<;>0<;>0<;>0<;>2<;>AUTHENTICATE"

            '//string http = GridcoinHttpPost(messagetype,boincauth,"GetPoolKey.aspx",true);
            '	msPubKey = ExtractXML(http,key,keystop);


        Dim sURL As String = sPoolURL
        If Mid(sURL, Len(sURL), 1) <> "/" Then sURL = sURL + "/"
            sURL = sURL + "GetPoolKey.aspx"
            Using wc As New MyWebClient

                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
                wc.Headers.Add("Miner", p)


                ' Upload the input string using the HTTP 1.0 POST method.
                Dim PostData As String
                'PostData = "user=" + Trim(sPoolUser) + "&pass=" + Trim(sPoolPass) + "&boinchash=" + Trim(RetrieveWin32BoincHash)
                PostData = p

                Dim byteArray As Byte() = System.Text.Encoding.ASCII.GetBytes(PostData)
                Dim byteResult As Byte() = wc.UploadData(sURL, "POST", byteArray)
                Dim sResult As String = System.Text.Encoding.ASCII.GetString(byteResult)

                If sResult.Contains("AUTHORIZED") Then
                    Log("Authenticate to Pool: Success")
                    Return True
                Else
                    MsgBox("Error while authenticating to pool : " + sResult, vbCritical)
                    Return False

                End If
            End Using

            Log("Authenticate to Pool: Fail - Username or Password invalid.")

        Return False
        Exit Function

        Catch ex As Exception
            Log("AuthenticateToPool Error: " + ex.Message)
            Return False
        End Try

    End Function
    Public Function PoolMode() As String
        Dim sPool As String = KeyValue("poolmining")
        Dim sCPU As String = KeyValue("cpumining")
        If LCase(sPool) = "true" Then PoolMode = "POOL_MINING" Else PoolMode = "SOLO_MINING"
    End Function
    Public ReadOnly Property RetrieveSqlHighBlock As Double
        Get
            Dim lBlock As Long = 0
            If KeyValue("disablesql") = "true" Then Return 10
            If KeyValue("UpdatingLeaderboard") = "true" Then Return mlSqlBestBlock

            Try
                Dim data As New Sql
                lBlock = data.HighBlockNumber
                data = Nothing
                Return lBlock + 1
            Catch ex As Exception
                Log(ex.Message)
            End Try
            Log("High Block " + Trim(lBlock))
            Return lBlock + 1
        End Get
    End Property
    Public ReadOnly Property BoincDeltaOverTime As String
        Get
            Return clsGVM.BoincDeltaOverTime()
        End Get
    End Property
    Public ReadOnly Property BoincTotalCreditsAvg As Double
        Get
            Return clsGVM.BoincCreditsAvg
        End Get
    End Property
    Public ReadOnly Property BoincTotalCredits As Double
        Get
            Return clsGVM.BoincCredits
        End Get
    End Property
    Public Function Des3Encrypt(ByVal s As String) As String
        Return clsGVM.Des3Encrypt(s)
    End Function
    Public Function Des3Decrypt(ByVal sData As String) As String
        Return clsGVM.Des3Decrypt(sData)
    End Function
    Public Function mForceBoincToUseGpus(sSleepDirective As String)
        mfrmMining.ForceBoincToUseGPUs(sSleepDirective)
    End Function
    Public Function ShowProjects()
    End Function
    Public Function ShowSql()
        mfrmSql = New frmSQL
        mfrmSql.Show()
    End Function
    Public Function ShowLeaderboard()
        mfrmLeaderboard = New frmLeaderboard
        mfrmLeaderboard.Show()
    End Function

    Public Function ShowGridcoinMiner()
        If mfrmGridcoinMiner Is Nothing Then
            mfrmGridcoinMiner = New frmGridcoinMiner
            mfrmGridcoinMiner.Show()
        Else
            mfrmGridcoinMiner.Show()
        End If
    End Function
    Public Function CPUPoW(ByVal sHash As String) As Double
        Return clsGVM.CPUPoW(sHash)
    End Function

    Public Function ShowMiningConsole()
        Try

            lfrmMiningCounter = lfrmMiningCounter + 1

            If mfrmMining Is Nothing Then
                mfrmMining = New frmMining
                mfrmMining.SetClsUtilization(Me)

            End If
           
            If lfrmMiningCounter = 1 Then
                If KeyValue("suppressminingconsole") = "true" Then Exit Function
                mfrmMining.Show()
                mfrmMining.Refresh2(False)
            End If

            If KeyValue("suppressminingconsole") <> "true" Then
                mfrmMining.Visible = True


            End If

          
        Catch ex As Exception
        End Try
    End Function
    Public ReadOnly Property MinedHash As String
        Get
            Return clsGVM.MinedHash
        End Get
    End Property
    Public ReadOnly Property SourceBlock As String
        Get
            Return clsGVM.SourceBlock
        End Get
    End Property

    Public Sub SetNodes(ByVal data As String)
        'Fix expression cannot be converted to dbNull
        Exit Sub

        Log(data)
        'Insert the Nodes
        modGRC.LoadNodes(data)
        Log("Loaded successfully")
    End Sub
    Public Function TestOutdated(ByVal sdata As String, ByVal mins As Long) As Boolean
        Return Outdated(sdata, mins)
    End Function
    Public Function TestKeyValue(ByVal sKey As String) As String
        Return KeyValue(sKey)
    End Function
    Public Function TestUpdateKey(ByVal sKey As String, ByVal sValue As String)
        Call UpdateKey(sKey, sValue)
    End Function
    Public Sub SetSqlBlock(ByVal data As String)
        
        Try

            'Log("Updating Leaderboard: " & Trim(KeyValue("UpdatingLeaderboard")) & ", Sql in sync " + Trim(SQLInSync) + " outdated " + Trim(Outdated(KeyValue("UpdatedLeaderboard"), 90)))



        If ("" & KeyValue("disablesql")) = "true" Then Exit Sub
       
        If ("" & KeyValue("UpdatingLeaderboard")) = "true" Then Exit Sub

        Catch ex As Exception
            Log("SetSqlBlockError:" + Err.Description + ":" + Err.Source)

        End Try


        If SQLInSync() And Outdated(KeyValue("UpdatedLeaderboard"), 90) Then
            Log("In sync and outdated, updating leaderboard")
            UpdateLeaderBoard() : Exit Sub
        End If


        Dim s As New Sql

        Try
            s.InsertBlocks(data)
            s.Close()
        Catch ex As Exception
            Log("SetSqlBlock:" + ex.Message)
            Try
                s.Close()

            Catch exx As Exception
                Log("setsqlblock" + exx.Message)
            End Try
        End Try
    End Sub
    Public Sub UpdateLeaderBoard()
        If KeyValue("disablesql") = "true" Then Exit Sub
        Try
            Dim thUpdateLeaderboard As New System.Threading.Thread(AddressOf modBoincLeaderboard.RefreshLeaderboard)
            Log("Starting background update leaderboard thread.")
            thUpdateLeaderboard.IsBackground = False
            thUpdateLeaderboard.Start()
        Catch ex As Exception
            Log("UpdateLeaderboard:" + ex.Message)
        End Try
    End Sub
    Public Sub SetLastBlockHash(ByVal data As String)
        clsGVM.LastBlockHash = Trim(data)
    End Sub
    Public Sub SetPublicWalletAddress(ByVal data As String)
        clsGVM.PublicWalletAddress = Trim(data)
    End Sub
    Public Sub SetBestBlock(ByVal nBlock As Integer)

        Try

        _nBestBlock = nBlock
        nBestBlock = nBlock
        clsGVM.BestBlock = nBlock
        Catch ex As Exception
            Log("Error setting Best block height " + Trim(nBlock) + " " + ex.Message)


        End Try

    End Sub


    Public Function CPUPoW(ByVal iProjectId As Integer, ByVal lUserId As Long, ByVal sGRCAddress As String) As Double
        Return clsGVM.CPUPoW(iProjectId, lUserId, sGRCAddress)
    End Function
    Public ReadOnly Property BoincProjectCount As Double
        Get
            Return clsGVM.BoincProjects
        End Get
    End Property
    Public ReadOnly Property BoincTotalHostAverageCredits As Double
        Get
            Return clsGVM.BoincTotalHostAvg
        End Get
    End Property
    Public Sub Snapshot()
        modGRC.Snapshot()
    End Sub
    Public Sub RestoreSnapshotInternal()
        modGRC.RestoreFromSnapshot()
    End Sub
    Public Sub ShowEmailModule()
        Dim e As New frmMail
        e.Show()
        e.RetrievePop3Emails()
    End Sub
    Public Function RetrieveGPUStats(iDevID As Integer, ByRef dMH As Double, ByRef dStales As Double, ByRef dAccepted As Double, _
                                     ByRef Invalid As Double, ByRef sMsg As String)

        Try
            Dim c As New CgSumm
            c = modCgMiner.CgSummary(iDevID)
            dAccepted = c.Accepted
            dStales = c.Stale
            dMH = c.Mhs * 1000
            Invalid = c.Invalid
            sMsg = c.Message

        Catch ex As Exception

        End Try

    End Function
    Private Sub xSnapshotTimerElapsed()
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Exit Sub
        ''''''NOTE: In order to turn this back on, we will need to remove the KillGuiMiner code from GRCRestarter!!!!!!!!!!
        'Once every 20 minutes this timer fires
    End Sub

    Public Function CloseGUIMiner()
        If Trim(KeyValue("closemineronexit")) <> "false" Then
            KillGuiMiner()
        End If

    End Function
    Protected Overrides Sub Finalize()
        If Not mfrmMining Is Nothing Then
            mfrmMining.bDisposing = True
            mfrmMining.Close()
            mfrmMining.Dispose()
            mfrmMining = Nothing
        End If
        MyBase.Finalize()
    End Sub

End Class

Public Interface IGridCoinMining

    Function RetrieveGridCoinMiningValueWithoutPrefix(iDeviceID As Integer, sProperty As String) As String

    Function RetrieveGridCoinMiningValue(iDeviceID As Integer, sProperty As String) As String

    'Function GetWork() As String

    'Sub SetWork(sData As String)

    'Function TestWork(sData As String) As String

    'Sub SetStats(iDevId As Integer, dValid As Double, dInvalid As Double, dSpeed As Double)

End Interface