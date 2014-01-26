Imports Microsoft.VisualBasic
Imports System.Timers

Public Class Utilization
    Implements IGridCoinMining
    'Private mSharedGPUStat() As SharedMemory
    Public _timerSnapshot As System.Timers.Timer
    Private _nBestBlock As Long
    Private _lLeaderboard As Long
    Public Structure CgSumm
        Public Mhs As Double
        Public Accepted As Double
        Public Rejected As Double
        Public Invalid As Double
        Public Stale As Double
        Public Message As String
    End Structure
    Private lfrmMiningCounter As Long = 0
    Public ReadOnly Property BoincUtilization As Double
        Get
            Return Val(clsGVM.BoincUtilization)
        End Get
    End Property

    Public ReadOnly Property BoincThreads As Double
        Get
            Return Val(clsGVM.BoincThreads)
        End Get
    End Property
    Sub New()
        If clsGVM Is Nothing Then clsGVM = New GridcoinVirtualMachine.GVM
    End Sub
    Sub New(bLoadMiningConsole As Boolean)
        If clsGVM Is Nothing Then clsGVM = New GridcoinVirtualMachine.GVM
        If bLoadMiningConsole Then ShowMiningConsole()
    End Sub
    Public Function CgMinerDeviceSummary(lDevId As Long) As CgSumm
        Return modCgMiner.CgSummary(lDevId)
    End Function
    Public ReadOnly Property Version As Double
        Get
            Return 50
        End Get
    End Property
    Public Sub RestartWallet()
        Call RestartWallet1("")
    End Sub
    Public Sub UpgradeWallet()
        Call RestartWallet1("upgrade")
    End Sub
    Public Sub ReindexWallet()
        Call RestartWallet1("reindex")
    End Sub
    Public Sub RestoreSnapshot()
        Call RestartWallet1("restoresnapshot")
    End Sub
    Public Sub CreateRestorePoint()
        Call RestartWallet1("createrestorepoint")
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
    Public ReadOnly Property CheckWork(ByVal sGRCHash1 As String, ByVal sGRCHash2 As String, ByVal sGRCHash3 As String, ByVal sBoinchash As String) As Double
        Get
            Return clsGVM.CheckWork(sGRCHash1, sGRCHash2, sGRCHash3, sBoinchash)
        End Get
    End Property
    Public ReadOnly Property RetrieveSqlHighBlock As Double
        Get
            Dim lBlock As Long = 0
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
    Public ReadOnly Property BoincAuthenticityString As String
        Get
            Return Trim(clsGVM.BoincAuthenticityString)
        End Get
    End Property
    Public ReadOnly Property BoincTotalCreditsAvg As Double
        Get
            Return clsGVM.BoincCreditsAvg
        End Get
    End Property
    Public ReadOnly Property BoincAuthenticity As Double
        Get
            Return Val(clsGVM.BoincAuthenticityString)
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
    Public Function ShowProjects()
        If mfrmProjects Is Nothing Then
            mfrmProjects = New frmProjects
            mfrmProjects.Show()
        Else
            mfrmProjects.Show()
        End If
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
            If mfrmMining Is Nothing Or lfrmMiningCounter = 1 Then
                Try
                    mfrmMining = New frmMining
                    mfrmMining.SetClsUtilization(Me)
                    mfrmMining.Show()
                    mfrmMining.Refresh2(False)

                Catch ex As Exception
                End Try
            Else
                mfrmMining.Show()
                mfrmMining.BringToFront()
                mfrmMining.Focus()
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
        Log(data)
        'Insert the Nodes
        modGRC.LoadNodes(data)
        Log("Loaded successfully")
    End Sub
    Public Sub SetSqlBlock(ByVal data As String)
        _lLeaderboard = _lLeaderboard + 1
        If _lLeaderboard > 6 Then
            _lLeaderboard = 0
            UpdateLeaderBoard()
        End If
        Try
            Dim s As New Sql
            s.InsertBlocks(data)
            s.Close()
        Catch ex As Exception
            Log("SetSqlBlock:" + ex.Message)
        End Try
    End Sub
    Public Sub UpdateLeaderBoard()
        Try
            modBoincLeaderboard.RefreshLeaderboard()
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
    Public Sub SetBestBlock(ByVal nBlock As Long)
        _nBestBlock = nBlock
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