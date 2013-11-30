﻿Public Class GVM

    Public CPUMiner As CPUMiner

    Sub New()
        CPUMiner = New CPUMiner
        Initialize()
    End Sub

    Public ReadOnly Property BoincCredits As Double
        Get
            Return modBoincCredits.BoincCredits
        End Get
    End Property
    Public ReadOnly Property BoincDeltaOverTime As String
        Get
            Return modUtilization.BoincAvgOverTime
        End Get
    End Property
    Public ReadOnly Property MinedHash As String
        Get
            Return CPUMiner.MinedHash
        End Get
    End Property

    Public ReadOnly Property SourceBlock As String
        Get
            Return CPUMiner.SourceBlock
        End Get
    End Property

    Public ReadOnly Property BoincProjects As Double
        Get
            Return modBoincCredits.BoincProjects
        End Get
    End Property
    Public Function BoincCreditsByProject(ByVal projectid As Long, ByVal dUserId As Double) As Double
        Return modBoincCredits.BoincCreditsByProject(projectid, dUserId)
    End Function
    Public Function Des3Encrypt(ByVal s As String) As String
        Return modCryptography.Des3EncryptData(s)
    End Function
    Public Function Des3Decrypt(ByVal sData As String) As String
        Return modCryptography.Des3DecryptData(sData)
    End Function
    Public ReadOnly Property BoincCreditsAvg As Double
        Get
            Return modBoincCredits.BoincCreditsAvg
        End Get
    End Property
    Public Function CPUPoW(ByVal sHash As String) As Double
        Dim vHash() As String
        vHash = Split(sHash, ":")
        If UBound(vHash) <> 2 Then Return -13
        Dim iProjectId As Integer
        Dim lUserId As Long
        Dim sGRCAddress As String
        iProjectId = vHash(0) : lUserId = vHash(1) : sGRCAddress = vHash(2)
        Dim dCredits As Double
        Dim sErr As String
        dCredits = CPUPoW(iProjectId, lUserId, sGRCAddress)
        Return dCredits
    End Function
    Public Function CPUPoW(ByVal iProjectId As Integer, ByVal lUserId As Long, ByVal sGRCAddress As String) As Long
        Dim dCredits As Double
        Dim sErr As String
        dCredits = ExtractCreditsByProject(iProjectId, lUserId, sGRCAddress, sErr)
        Return dCredits
    End Function
    


    Public Property PublicWalletAddress As String
        Set(ByVal value As String)
            modUtilization.PublicWalletAddress = value
        End Set
        Get
            Return modUtilization.PublicWalletAddress
        End Get
    End Property

    Public Property LastBlockHash As String
        Set(ByVal value As String)
            modUtilization.LastBlockHash = value
        End Set
        Get
            Return modUtilization.LastBlockHash
        End Get
    End Property
    Public ReadOnly Property BoincUtilization As Double
        Get
            Return Val(mBoincProcessorUtilization)

        End Get
    End Property

    Public ReadOnly Property BoincThreads As Double
        Get
            Return Val(mBoincThreads)

        End Get
    End Property
    Public ReadOnly Property BoincTotalHostAvg As Double
        Get
            Return modBoincCredits.BoincTotalHostAvg

        End Get
    End Property
    Public ReadOnly Property BoincAuthenticityString As String
        Get
            Return Trim(VerifyBoincAuthenticity.ToString())
        End Get
    End Property
    Public Function ReturnBoincCreditsAtPointInTime(ByVal dLookback As Double) As Double
        Return modBoincCredits.ReturnBoincCreditsAtPointInTime(dLookback)
    End Function
    Public ReadOnly Property BoincMD5 As String
        Get
            Return Trim(modBoincMD5)
        End Get
    End Property
    Public Function MineBlock()
        Dim c As New CPUMiner
        c.MineNewBlock()
    End Function
End Class
