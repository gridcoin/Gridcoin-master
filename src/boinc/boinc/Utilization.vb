Imports Microsoft.VisualBasic
Imports System.Timers

Public Class Utilization
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
        ShowMiningConsole()
    End Sub
    Public ReadOnly Property Version As Double
        Get
            Return 25
        End Get
    End Property
    Public ReadOnly Property BoincMD5 As String
        Get
            Return clsGVM.BoincMD5()
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
            mfrmProjects.show()
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
                    mfrmMining.clsUtilization = Me
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
    Public Sub SetLastBlockHash(ByVal data As String)
        clsGVM.LastBlockHash = Trim(data)
    End Sub
    Public Sub SetPublicWalletAddress(ByVal data As String)
        clsGVM.PublicWalletAddress = Trim(data)
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

    Public Sub ShowEmailModule()
        Dim e As New frmMail
        e.Show()
        e.RetrievePop3Emails()
    End Sub

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
