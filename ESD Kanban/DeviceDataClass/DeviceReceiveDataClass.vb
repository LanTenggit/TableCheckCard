Imports System.IO
Imports ESD_Kanban.DBConfig

Public Class DeviceReceiveDataClass

    Shared inipath As String = My.Application.Info.DirectoryPath & "\" & "Parameter.ini"
    Shared send As String
    Dim controller As String
    Public Class Device
        Public DeviceName As String
        Public address As String
        Public gross_product As String
        Public bad_num As String
        Public Powertimme As String
        Public Powervalue As String
    End Class

    ''' <summary>
    ''' 扬声器接收数据处理
    ''' </summary>
    ''' <param name="buf"></param>
    ''' <param name="receivestr_split"></param>
    Public Shared Function Voice(buf As Byte(), receivestr_split As String())

        Dim device As Device = New Device
        If buf.Length >= 14 Then
            Dim receivestrhex As String = ""
            Dim j As Integer = 0
            While j < 11
                receivestrhex += receivestr_split(j) + " "
                j += 1
            End While
            receivestrhex += receivestr_split(11)

            Dim num = Convert.ToInt32(Model_set.GetINI("Device", "Num", "", inipath))

            For i = 1 To num

                Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", inipath)
                Dim DeviceList As String() = DeviceValue.Split(",")
                Select Case DeviceList(1)
                    Case "扬声器"
                        DeviceList(1) = "F1"
                    Case "TTK-1锁附机"
                        DeviceList(1) = "F2"
                    Case "TTK-2锁附机"
                        DeviceList(1) = "F3"
                    Case "TTK-3锁附机"
                        DeviceList(1) = "F4"
                End Select
                send = DeviceList(1) + " " + DeviceList(2)


                If send IsNot String.Empty And send IsNot Nothing Then

                    Dim sendsplit As String() = send.Split(" "c)

                    Dim ＲeceivestrCRC As String = receivestr_split(12) + " " + receivestr_split(13)
                    Dim crcstr As String = CRCClass.CRCCalc(receivestrhex)


                    If receivestr_split(0) = sendsplit(0) AndAlso receivestr_split(1) = sendsplit(1) AndAlso receivestr_split(2) = sendsplit(2) AndAlso ＲeceivestrCRC = crcstr Then


                        device.address = receivestr_split(1) + " " + receivestr_split(2)
                        device.DeviceName = receivestr_split(0)
                        device.gross_product = (buf(3) * 256 + buf(4))
                        device.bad_num = buf(5) * 256 + buf(6)
                        device.Powertimme = (buf(7) * 256 + buf(8)) / 100
                        device.Powervalue = (buf(10) * 256 + buf(11)) / 100
                        If buf(9) = 1 Then
                            device.Powervalue = "-" + （(buf(10) * 256 + buf(11)) / 100）.ToString()
                        End If

                        Dim txtpath = Path.GetFullPath("datatxt.txt")
                        WriteTXT(txtpath, device)
                    End If
                End If
            Next
        End If

        Return device
    End Function

    ''' <summary>
    ''' TTK-1锁附机一代
    ''' </summary>
    ''' <param name="buf"></param>
    ''' <param name="receivestr_split"></param>
    Public Shared Function Lock_Machine_TTK1(buf As Byte(), receivestr_split As String())
        Dim device As Device = New Device
        '设备名称
        Dim DeviceName As String
        '地址
        Dim address As String
        '手机总产量
        Dim gross_product As Integer = 0
        '螺丝不良数
        Dim bad_num As Integer = 0
        '螺丝总产量
        Dim ScrewSum As String = ""
        '负压值
        Dim Powervalue As String = ""

        If buf.Length >= 14 Then

            Dim receivestrhex As String = ""
            Dim j As Integer = 0
            While j < 11
                receivestrhex += receivestr_split(j) + " "
                j += 1
            End While
            receivestrhex += receivestr_split(11)

            Dim num = Convert.ToInt32(Model_set.GetINI("Device", "Num", "", inipath))

            For i = 1 To num

                Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", inipath)
                Dim DeviceList As String() = DeviceValue.Split(",")
                Select Case DeviceList(1)
                    Case "扬声器"
                        DeviceList(1) = "F1"
                    Case "TTK-1锁附机"
                        DeviceList(1) = "F2"
                    Case "TTK-2锁附机"
                        DeviceList(1) = "F3"
                    Case "TTK-3锁附机"
                        DeviceList(1) = "F4"
                End Select
                send = DeviceList(1) + " " + DeviceList(2)


                If send IsNot String.Empty And send IsNot Nothing Then

                    Dim sendsplit As String() = send.Split(" "c)

                    Dim ＲeceivestrCRC As String = receivestr_split(12) + " " + receivestr_split(13)
                    Dim crcstr As String = CRCClass.CRCCalc(receivestrhex)


                    If receivestr_split(0) = sendsplit(0) AndAlso receivestr_split(1) = sendsplit(1) AndAlso receivestr_split(2) = sendsplit(2) AndAlso ＲeceivestrCRC = crcstr Then

                        DeviceName = receivestr_split(0)
                        address = receivestr_split(1) + " " + receivestr_split(2)
                        gross_product = (buf(3) * 256 + buf(4))
                        bad_num = buf(5) * 256 + buf(6)
                        ScrewSum = (buf(7) * 256 + buf(8))
                        Powervalue = (buf(10) * 256 + buf(11)) / 100
                        If buf(9) = 1 Then
                            Powervalue = "-" + （(buf(10) * 256 + buf(11)) / 100）.ToString()
                        End If


                        device.address = address
                        device.DeviceName = DeviceName
                        device.gross_product = gross_product
                        device.bad_num = bad_num
                        device.Powertimme = ScrewSum
                        device.Powervalue = Powervalue
                        Dim txtpath = Path.GetFullPath("datatxt.txt")
                        WriteTXT(txtpath, device)

                    End If
                End If
            Next
        End If
        Return device

    End Function
    ''' <summary>
    ''' TTK-1锁附机二代
    ''' </summary>
    ''' <param name="buf"></param>
    ''' <param name="receivestr_split"></param>
    Public Shared Function Lock_Machine_TTK2(buf As Byte(), receivestr_split As String())

        Dim device As Device = New Device
        '设备名称
        Dim DeviceName As String
        '地址
        Dim address As String
        '手机总产量
        Dim gross_product As Integer = 0
        '螺丝不良率
        Dim bad_rate As Double = 0
        '前负压值
        Dim FrontPowervalue As String = ""
        '后负压值
        Dim BackPowervalue As String = ""
        If buf.Length >= 15 Then

            Dim receivestrhex As String = ""
            Dim j As Integer = 0
            While j < 12
                receivestrhex += receivestr_split(j) + " "
                j += 1
            End While
            receivestrhex += receivestr_split(12)

            Dim num = Convert.ToInt32(Model_set.GetINI("Device", "Num", "", inipath))

            For i = 1 To num

                Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", inipath)
                Dim DeviceList As String() = DeviceValue.Split(",")
                Select Case DeviceList(1)
                    Case "扬声器"
                        DeviceList(1) = "F1"
                    Case "TTK-1锁附机"
                        DeviceList(1) = "F2"
                    Case "TTK-2锁附机"
                        DeviceList(1) = "F3"
                    Case "TTK-3锁附机"
                        DeviceList(1) = "F4"
                End Select
                send = DeviceList(1) + " " + DeviceList(2)


                If send IsNot String.Empty And send IsNot Nothing Then

                    Dim sendsplit As String() = send.Split(" "c)

                    Dim ＲeceivestrCRC As String = receivestr_split(13) + " " + receivestr_split(14)
                    Dim crcstr As String = CRCClass.CRCCalc(receivestrhex)


                    If receivestr_split(0) = sendsplit(0) AndAlso receivestr_split(1) = sendsplit(1) AndAlso receivestr_split(2) = sendsplit(2) AndAlso ＲeceivestrCRC = crcstr Then

                        DeviceName = receivestr_split(0)
                        address = receivestr_split(1) + " " + receivestr_split(2)
                        gross_product = (buf(3) * 256 + buf(4))
                        bad_rate = (buf(5) * 256 + buf(6)) / 100
                        FrontPowervalue = (buf(8) * 256 + buf(9)) / 100
                        BackPowervalue = (buf(11) * 256 + buf(12)) / 100

                        If buf(7) = 1 Then
                            FrontPowervalue = "-" + （(buf(8) * 256 + buf(9)) / 100）.ToString()
                        End If
                        If buf(10) = 1 Then
                            BackPowervalue = "-" + （(buf(11) * 256 + buf(12)) / 100）.ToString()
                        End If

                        device.address = address
                        device.DeviceName = DeviceName
                        device.gross_product = gross_product
                        device.bad_num = bad_rate
                        device.Powertimme = FrontPowervalue
                        device.Powervalue = BackPowervalue
                        Dim txtpath = Path.GetFullPath("datatxt.txt")
                        WriteTXT(txtpath, device)


                    End If
                End If
            Next
        End If
        Return device

    End Function
    ''' <summary>
    ''' TTK-1锁附机三代
    ''' </summary>
    ''' <param name="buf"></param>
    ''' <param name="receivestr_split"></param>
    Public Shared Function Lock_Machine_TTK3(buf As Byte(), receivestr_split As String())
        Dim device As Device = New Device
        '设备名称
        Dim DeviceName As String
        '地址
        Dim address As String
        '手机总产量
        Dim gross_product As Integer = 0
        '螺丝不良数
        Dim bad_sum As Double = 0
        '前负压值
        Dim FrontPowervalue As String = ""
        '后负压值
        Dim BackPowervalue As String = ""
        If buf.Length >= 15 Then

            Dim receivestrhex As String = ""
            Dim j As Integer = 0
            While j < 12
                receivestrhex += receivestr_split(j) + " "
                j += 1
            End While
            receivestrhex += receivestr_split(12)

            Dim num = Convert.ToInt32(Model_set.GetINI("Device", "Num", "", inipath))

            For i = 1 To num

                Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", inipath)
                Dim DeviceList As String() = DeviceValue.Split(",")
                Select Case DeviceList(1)
                    Case "扬声器"
                        DeviceList(1) = "F1"
                    Case "TTK-1锁附机"
                        DeviceList(1) = "F2"
                    Case "TTK-2锁附机"
                        DeviceList(1) = "F3"
                    Case "TTK-3锁附机"
                        DeviceList(1) = "F4"
                End Select
                send = DeviceList(1) + " " + DeviceList(2)


                If send IsNot String.Empty And send IsNot Nothing Then

                    Dim sendsplit As String() = send.Split(" "c)

                    Dim ＲeceivestrCRC As String = receivestr_split(13) + " " + receivestr_split(14)
                    Dim crcstr As String = CRCClass.CRCCalc(receivestrhex)


                    If receivestr_split(0) = sendsplit(0) AndAlso receivestr_split(1) = sendsplit(1) AndAlso receivestr_split(2) = sendsplit(2) AndAlso ＲeceivestrCRC = crcstr Then

                        DeviceName = receivestr_split(0)
                        address = receivestr_split(1) + " " + receivestr_split(2)
                        gross_product = (buf(3) * 256 + buf(4))
                        bad_sum = buf(5) * 256 + buf(6)
                        FrontPowervalue = (buf(8) * 256 + buf(9)) / 100
                        BackPowervalue = (buf(11) * 256 + buf(12)) / 100

                        If buf(7) = 1 Then
                            FrontPowervalue = "-" + （(buf(8) * 256 + buf(9)) / 100）.ToString()
                        End If
                        If buf(10) = 1 Then
                            BackPowervalue = "-" + （(buf(11) * 256 + buf(12)) / 100）.ToString()
                        End If


                        device.address = address
                        device.DeviceName = DeviceName
                        device.gross_product = gross_product
                        device.bad_num = bad_sum
                        device.Powertimme = FrontPowervalue
                        device.Powervalue = BackPowervalue
                        Dim txtpath = Path.GetFullPath("datatxt.txt")
                        WriteTXT(txtpath, device)

                    End If
                End If
            Next
        End If
        Return device

    End Function





    ''' <summary>
    ''' 写入txt
    ''' </summary>
    ''' <param name="txtPath"></param>
    Private Shared Sub WriteTXT(txtPath As String, de As Device)
        Dim sw As New StreamWriter(txtPath, True) 'System.Text.Encoding.GetEncoding("GB2312") = 
        Dim str As String = "时间：" + Date.Now.ToString + "," + "名称：" +
            de.DeviceName + "," + "地址：" + de.address + "," + "总产量：" +
            de.gross_product + "," + "不良数：" + de.bad_num + "," + "保压时间：" +
            de.Powertimme + "," + "保压值：" + de.Powervalue
        sw.WriteLine(str)
        sw.Close()

        'Dim strArr As String() = {str}
        'File.WriteAllLines(txtPath, strArr, System.Text.Encoding.GetEncoding("GB2312")) '写入到新文件中

    End Sub



    ''' <summary>
    ''' 设备数据上传
    ''' </summary>
    Public Shared Function DataUpLoad(V_Name As String, V_adress As String, V_Yiled As Integer, V_BedNum As Integer,
           V_Power_Time As Double, V_Power As Double)
        Dim line As String = Model_set.t_Line_body
        Dim Monitor As String = Model_set.t_Monitor
        Dim Computer As String = Model_set.hostName
        Dim IP As String = Model_set.IPadd
        Dim MAC As String = Model_set.GetMacAddress


        Dim sql = String.Format("insert into Device (D_Creadte_Time,D_Line,D_Monitor,D_Name,D_Adress,D_Yieds,
             D_Bednum,D_Power_Time,D_Power,D_Computer_Name,D_IP_adress,D_MAC_adress)
              values(GETDATE(),'{0}','{1}','{2}','{3}',{4},{5},{6},{7},'{8}','{9}','{10}')",
              line, Monitor, V_Name, V_adress, V_Yiled, V_BedNum, V_Power_Time, V_Power, Computer, IP, MAC)
        Dim cmd As DBCommand = New DBCommand(New ConnectionString().ConnectionInfo)
        If cmd.Insert(sql) > 0 Then
        Else
            Log(sql)
        End If




        Return 1


    End Function












End Class
