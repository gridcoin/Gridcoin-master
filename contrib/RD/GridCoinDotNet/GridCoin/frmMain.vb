Option Explicit On
Option Strict On

Imports System.Security.Cryptography
Imports Finisar.SQLite
Imports System.Text
Imports System.IO
Imports Microsoft.VisualBasic



Public Class frmGridCoin



    Private Sub InitListView()
        ' Set ListView Properties
        lvRecent.View = View.Details
        lvRecent.GridLines = True
        lvRecent.FullRowSelect = True
        lvRecent.HideSelection = False
        lvRecent.MultiSelect = False

        ' Create Columns Headers
        lvRecent.Columns.Add("Incoming/Outgoing")
        lvRecent.Columns.Add("Recipient")
        lvRecent.Columns.Add("Amount")


        Dim tempValue As Integer = 0

        For i As Integer = 0 To 5
            Dim lvi As New ListViewItem
            lvi.Text = "I"
            tempValue = tempValue + 1
            lvi.SubItems.Add("Me")
            lvi.SubItems.Add(tempValue.ToString)
            lvRecent.Items.Add(lvi)
        Next
    End Sub
  
    
    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        lblCPU.Text = BoincProcessorUtilization.ToString

    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub frmGridCoin_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim trd As System.Threading.Thread
        trd = New System.Threading.Thread(AddressOf modFunctions.MonitorProcessorUtilization)
        trd.IsBackground = True
        trd.Start()

        'Start the listener to accept new connections
        Dim mP2p As New p2p
        mData = New data

        mData.AddIP("127.0.0.1", "me")

        'Application.Run(mP2p)
        'Application.Run(mP2p.MainForm)


    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click


        frmMail.Show()
        frmMail.RetrievePop3Emails()


    End Sub


    'Boinc
    'Elevated command prompt
    'Cd to gridcoin directory
    'RegAsm.exe boinc.dll
    'regtlibv12.exe boinc.tlb.
    '//Note, on Windows, if the performance counters are corrupted, rebuild them by going to an elevated command prompt and 
    'issue the command:
    'lodctr /r 

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub BackupWalletToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BackupWalletToolStripMenuItem.Click

    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click

    End Sub
End Class
Class Alice

    Public Shared Sub Main()
        Dim bob As New Bob()
        If (True) Then
            Using dsa As New ECDsaCng()
                dsa.HashAlgorithm = CngAlgorithm.Sha256
                bob.key = dsa.Key.Export(CngKeyBlobFormat.EccPublicBlob)
                Dim data() As Byte = {21, 5, 8, 12, 207}
                Dim signature As Byte() = dsa.SignData(data)
                bob.Receive(data, signature)
            End Using
        End If

    End Sub 'Main
End Class 'Alice 


Public Class Bob
    Public key() As Byte

    Public Sub Receive(ByVal data() As Byte, ByVal signature() As Byte)
        Using ecsdKey As New ECDsaCng(CngKey.Import(key, CngKeyBlobFormat.EccPublicBlob))
            If ecsdKey.VerifyData(data, signature) Then
                Console.WriteLine("Data is good")
            Else
                Console.WriteLine("Data is bad")
            End If
        End Using

    End Sub 'Receive
End Class 'Bob 