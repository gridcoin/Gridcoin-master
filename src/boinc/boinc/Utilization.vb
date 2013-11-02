Imports Microsoft.VisualBasic
Imports System.Timers





Public Class Utilization
    Private _boincutilization As Long
    Private _boincthreads As Long

    Private _timer As Timer
    Public Ticker As Long



    Public ReadOnly Property BoincUtilization As Double
        Get
            Return Val(_boincutilization)


        End Get

    End Property
    
    Public ReadOnly Property BoincThreads As Double
        Get
            Return Val(_boincthreads)

        End Get

    End Property
    Sub New()
        _timer = New Timer(5000)

        AddHandler _timer.Elapsed, New ElapsedEventHandler(AddressOf Elapsed)
        _timer.Enabled = True


    End Sub
    Public ReadOnly Property Version As Double
        Get
            Return 8
        End Get
    End Property
    Public ReadOnly Property BoincMD5 As String
        Get
            Return modUtilization.BoincMD5()
        End Get
    End Property
    Public ReadOnly Property BoincAuthenticityString As String
        Get
            Return Trim(modUtilization.VerifyBoincAuthenticity.ToString())
        End Get
    End Property

    Public ReadOnly Property BoincAuthenticity As Double
        Get
            Return modUtilization.VerifyBoincAuthenticity()
        End Get

    End Property
    Sub Elapsed()
        Ticker = Ticker + 1
        Try
            _timer.Enabled = False
            'Stop the timer in case this function takes a long time
            modUtilization.ReturnBoincCPUUsage()

        Catch ex As Exception

        End Try
        _timer.Enabled = True
        _boincutilization = modUtilization.BoincProcessorUtilization
        _boincthreads = modUtilization.BoincThreads

    End Sub


    Public Sub ShowEmailModule()
        Dim e As New frmMail
        e.Show()
        e.RetrievePop3Emails()

    End Sub
  
End Class
