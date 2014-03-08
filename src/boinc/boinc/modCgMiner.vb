
Imports System.Net.HttpWebRequest
Imports System.Text
Imports System.IO
Imports System.Net

Module modCgMiner

    Public mCGMinerHwnd(10) As IntPtr
    Public mCGMinerAPIDown(10) As Double
    Public mbScryptSleep As Boolean

    Public Function StringToByte(value As String) As Byte()
        Dim array() As Byte = System.Text.Encoding.ASCII.GetBytes(value)
        Return array
    End Function
    Public Function CgSend(command As String, devid As Integer) As String
        Try

            If mCGMinerHwnd(devid) = 0 Then
                Return "CGMiner not started."
            End If
            If mCGMinerAPIDown(devid) > 0 Then
                mCGMinerAPIDown(devid) = mCGMinerAPIDown(devid) - 0.1
                Return "API Not responding; Restart CGMiner"
            End If
            Dim tcpclient As New System.Net.Sockets.TcpClient
            tcpclient.Connect("127.0.0.1", 4000 + devid)
            Dim s As System.Net.Sockets.NetworkStream
            Dim b() As Byte
            b = StringToByte(command)
            s = tcpclient.GetStream()
            s.Write(b, 0, b.Length)
            Dim responseData As String = ""
            Dim data As [Byte]() = New [Byte](4096) {}
            Dim bytes As Int32 = s.Read(data, 0, data.Length)
            responseData = Encoding.ASCII.GetString(data, 0, bytes)
            tcpclient.Close()
            tcpclient = Nothing
            mCGMinerAPIDown(devid) = 0
            Return responseData
        Catch ex As Exception
            Return "Not Connected"
            '12-21-2013
            mCGMinerAPIDown(devid) = mCGMinerAPIDown(devid) + 1

        End Try

    End Function
    Public Function UpdateIntensity(lDevId As Long, Intensity As Long) As String
        Dim sOut As String
        sOut = CgSend("gpuintensity|" + Trim(lDevId) + "," + Trim(Intensity), lDevId)
        Return sOut
    End Function
    Public Function DisableGPU(lDevId As Long) As String
        Dim sOut As String
        sOut = CgSend("gpudisable|" + Trim(lDevId), lDevId)
        Return sOut
    End Function
    Public Function EnableGPU(lDevId As Long) As String
        Dim sOut As String
        sOut = CgSend("gpuenable|" + Trim(lDevId), lDevId)
        Return sOut
    End Function
    Public Function EnableAllGPUs()
        For x = 0 To 4
            EnableGPU(x)
        Next
    End Function
    Public Function DisableAllGPUs()
        For x = 0 To 4
            DisableGPU(x)
        Next
    End Function
    Private Function CgFieldValue(vFields() As String, sKey As String) As String
        Try

            Dim vVal() As String
            For x = 0 To UBound(vFields)
                vVal = Split(vFields(x), "=")
                If LCase(vVal(0)) = LCase(sKey) Then
                    Return vVal(1)
                End If
            Next

        Catch ex As Exception

        End Try
    End Function
    Public Function CgSummary(lDevId As Long) As Utilization.CgSumm
        Dim c As New Utilization.CgSumm

        Try

            Dim sOut As String = CgSend("summary", lDevId)
            Dim vOut() As String
            vOut = Split(sOut, "|")

            If UBound(vOut) = 0 And Len(sOut) > 0 Then
                c.Message = sOut
                Exit Function
            End If
            Dim sFields As String = "SUMMARY,Elapsed=15,MHS av=0.65,Found Blocks=0,Getworks=6,Accepted=0,Rejected=0,Hardware Errors=0,Utility=0.00,Discarded=0,Stale=0,Get Failures=0,Local Work=0,Remote Failures=0,Network Blocks=1,Total MH=9.9697,Work Utility=0.00,Difficulty Accepted=0.00000000,Difficulty Rejected=0.00000000,Difficulty Stale=0.00000000,Best Share=0"
            Dim sInterim As String = vOut(1) 'summary
            Dim vFields() As String
            vFields = Split(sInterim, ",")

            c.Mhs = Val(CgFieldValue(vFields, "MHS av"))
            c.Accepted = Val(CgFieldValue(vFields, "Accepted"))
            c.Rejected = Val(CgFieldValue(vFields, "Rejected"))
            c.Invalid = Val(CgFieldValue(vFields, "Hardware Errors"))
            c.Stale = Val(CgFieldValue(vFields, "Stale"))
            Return c
        Catch ex As Exception

        End Try


    End Function
    Public Function WriteCgMinerFile(lDevId As Long, sPort As String, sUserName As String, sPass As String, lIntensity As Long, lWorkSize As Long, lLookupGap As Long, lConcurrency As Long, sCGPath As String) As String
        Try

            Dim c As New StringBuilder
            Dim lPortId As Long
            lPortId = 4000 + lDevId
            c.AppendLine("{")
            c.AppendLine("  'pools' : [")
            c.AppendLine("      {")
            c.AppendLine("          'url' : 'http://127.0.0.1:" + Trim(sPort) + "/', ")
            c.AppendLine("          'user' : '" + sUserName + "',")
            c.AppendLine("          'pass' : '" + sPass + "'")
            c.AppendLine("  	}")
            c.AppendLine("],")
            c.AppendLine("'api-listen' : true,")
            c.AppendLine("'api-port' : '" + Trim(lPortId) + "',")
            c.AppendLine("'api-allow' : 'W:127.0.0.1',")
            c.AppendLine("'intensity' : '" + Trim(lIntensity) + "',")
            c.AppendLine("'vectors' : '1',")
            c.AppendLine("'worksize' : '" + Trim(lWorkSize) + "',")
            c.AppendLine("'scantime' : '7',")

            c.AppendLine("'kernel' : 'scrypt',")
            c.AppendLine("'scrypt' : true,")
            c.AppendLine("'lookup-gap' : '" + Trim(lLookupGap) + "',")
            c.AppendLine("'thread-concurrency' : '" + Trim(lConcurrency) + "',")
            c.AppendLine("'gpu-engine' : '0',")
            c.AppendLine("'temp-cutoff' : '80',")
            c.AppendLine("'temp-overheat' : '70',")
            c.AppendLine("'temp-target' : '60',")
            c.AppendLine("'gpu-threads' : '1',")
            'c.AppendLine("'no-pool-disable' : false,") 'doesnt work
            c.AppendLine("'temp-hysteresis' : '3',")
            c.AppendLine("'gpu-platform' : '0',")
            c.AppendLine("'device' : '" + Trim(lDevId) + "'")
            c.AppendLine("}")
            Dim sw As New StreamWriter(sCGPath)
            Dim sOut As String
            sOut = c.ToString()
            sOut = Replace(sOut, "'", Chr(34))
            sw.Write(sOut)
            sw.Close()
            Return "Success"
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Public Function GetSleepLevelByAddress(sAddress As String, ByRef dScryptSleep As Double, sBlockhash As String, _
                                            ByRef dNetLevel As Double) As Boolean
        'Retrieve User level
        dScryptSleep = 0
        dNetLevel = 0
        Try
            Dim sql As String
            Dim mData As New Sql("gridcoin_leaderboard")
            sql = "Select ScryptSleepChance from leaderboard Where Address='" + sAddress + "'"
            Dim gr As New GridcoinReader
            gr = mData.GetGridcoinReader(sql)
            'Dim grr As GridcoinReader.GridcoinRow
            dScryptSleep = gr.Value(1, "ScryptSleepChance")
            mData = Nothing
        Catch ex As Exception
            Log("GetSleepLevelByAddress: " + ex.Message)
        End Try


        ''''''''''''''''Newbie Sleep Level Patch:
        If dScryptSleep = 0 Then dScryptSleep = NewbieSleepLevel()



        If dScryptSleep = 0 Then dScryptSleep = 0.5

        'Calculate Net Level
        Dim sBlockSuffix As String

        If Len(sBlockhash) > 3 Then
            sBlockSuffix = Mid(sBlockhash, Len(sBlockhash) - 3, 3)
        End If
        Dim dDecSuffix = CDbl("&h" + Trim(sBlockSuffix))
        Dim dCalc = dDecSuffix / 40.96
        dCalc = dCalc / 100
        Dim sSleepStatus As String = "WORK"
        If dScryptSleep >= dCalc Then sSleepStatus = "WORK" Else sSleepStatus = "SLEEP"
        dNetLevel = dCalc
        If sSleepStatus = "SLEEP" Then
            Return False
        Else
            Return True
        End If

    End Function


End Module
