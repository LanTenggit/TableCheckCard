Imports System.IO.Ports
Imports System.Threading
Imports ESD_Kanban.DBConfig
Imports ESD_Kanban.ESD_STATE
Imports ESD_Kanban.GetSYSTEMTIME
Imports System.Messaging
Imports ZedGraph
Imports System.IO
Imports System.Data.SqlClient


Public Class MainFrom

    Dim inipath As String = My.Application.Info.DirectoryPath & "\" & "Parameter.ini"
    Dim send As String
    Dim controller As String

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        from_Setting.Hide()
        'from_Setting.Show()
        from_Setting.ShowDialog()
        ToolStripLabel1.Text = "ESD数量:" & ESD_NUM_Left

        ToolStripLabel7.Text = Scanning_interval
        ToolStripLabel9.Text = Scanning_timeout
    End Sub

    ''' <summary>
    ''' 采集器线程运行开关
    ''' </summary>
    ''' <remarks></remarks>
    Dim thread_bol As Boolean = False
    ''' <summary>
    ''' 采集器工作开关
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Select Case thread_bol
            Case False
                thread_bol = True
                Button6.BackColor = Color.DarkSeaGreen '绿色
                Button6.Text = "停止采集线程"
            Case True
                thread_bol = False
                Button6.BackColor = Color.Gainsboro '浅灰色
                Button6.Text = "启动采集线程"
        End Select
    End Sub


    ''' <summary>
    ''' 数采控制器串口
    ''' </summary>
    ''' <remarks></remarks>
    Dim S1 As Boolean = False
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        '/*串口事件注册
        '/*************************************************************************************************************/
        Try
            Select Case S1
                Case False

                    Relay_control_source_Left = ComboBox1.Text '串口名称
                    Relay_control_source_Baud_Left = ComboBox2.Text '串口波特率

                    SC_Relay_control_Left.setSerialPort(
                    ComboBox1.Text, ComboBox2.Text, 8, IO.Ports.StopBits.One)
                    Control.CheckForIllegalCrossThreadCalls = False '线程之间进行通讯
                    AddHandler SC_Relay_control_Left.DataReceived,
                    New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_Left)
                    If SC_Relay_control_Left.openPort() = False Then
                        Exit Sub
                    End If
                    S1 = True
                    Button9.Text = "停止(&Q)"
                    ComboBox1.Enabled = False
                    ComboBox2.Enabled = False
                    Button9.BackColor = Color.DarkSeaGreen '绿色

                    thread_run_Left()



                Case True
                    Relay_control_source_Left = ComboBox1.Text '串口名称
                    Relay_control_source_Baud_Left = ComboBox2.Text '串口波特率

                    SC_Relay_control_Left.closePort() '关闭串口
                    '卸载事件关联
                    RemoveHandler SC_Relay_control_Left.DataReceived,
                   New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_Left)

                    S1 = False
                    Button9.Text = "启动(&S)"
                    ComboBox1.Enabled = True
                    ComboBox2.Enabled = True
                    Button9.BackColor = Color.Gainsboro '浅灰色

                    If thread_esd_send_Left IsNot Nothing Then
                        thread_esd_send_Left.Abort() '结束线程
                    End If

            End Select
        Catch ex As Exception
            MessageBox.Show("A采集器" & ex.Message & ex.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
        End Try

        '关闭串口连接
        'SC.closePort() 
        '/*************************************************************************************************************/

    End Sub
    Dim S3 As Boolean = False
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        '/*串口事件注册
        '/*************************************************************************************************************/
        Try
            Select Case S3
                Case False

                    Relay_control_source_right = ComboBox6.Text '串口名称
                    Relay_control_source_Baud_right = ComboBox5.Text '串口波特率

                    SC_Relay_control_right.setSerialPort(
                    ComboBox6.Text, ComboBox5.Text, 8, IO.Ports.StopBits.One)
                    Control.CheckForIllegalCrossThreadCalls = False '线程之间进行通讯
                    AddHandler SC_Relay_control_right.DataReceived,
                    New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_right)
                    If SC_Relay_control_right.openPort() = False Then
                        Exit Sub
                    End If
                    S3 = True
                    Button5.Text = "停止(&Q)"
                    ComboBox6.Enabled = False
                    ComboBox5.Enabled = False
                    Button5.BackColor = Color.DarkSeaGreen '绿色

                    thread_run_right()

                Case True
                    Relay_control_source_right = ComboBox6.Text '串口名称
                    Relay_control_source_Baud_right = ComboBox5.Text '串口波特率

                    SC_Relay_control_right.closePort() '关闭串口
                    '卸载事件关联 
                    RemoveHandler SC_Relay_control_right.DataReceived,
                   New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_right)

                    S3 = False
                    Button5.Text = "启动(&S)"
                    ComboBox6.Enabled = True
                    ComboBox5.Enabled = True
                    Button5.BackColor = Color.Gainsboro '浅灰色

                    If thread_esd_send_right IsNot Nothing Then
                        thread_esd_send_right.Abort() '结束线程
                    End If

            End Select
        Catch ex As Exception
            MessageBox.Show("B采集器," & ex.Message & ex.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
        End Try

        '关闭串口连接
        'SC.closePort() 
        '/*************************************************************************************************************/
    End Sub

    ''' <summary>
    ''' 线体右边线程
    ''' </summary>
    ''' <remarks></remarks>
    Dim thread_esd_send_right As Threading.Thread
    ''' <summary>
    ''' ESD线程启动 右边
    ''' </summary>
    ''' <remarks></remarks>


    Private Sub thread_run_right()
        thread_esd_send_right = New Threading.Thread(AddressOf thread_senddate_right)
        Control.CheckForIllegalCrossThreadCalls = False '不检测线程安全
        thread_esd_send_right.Name = "ESD串口通信线程"
        thread_esd_send_right.IsBackground = True '后台线程
        thread_esd_send_right.Start() '启动线程
    End Sub
    ''' <summary>
    ''' [采集器B]线程循环发送串口指令
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub thread_senddate_right()
        Log("[采集器B]线程循环发送串口指令 启动")
        Do
            If thread_bol = False Then
                '延迟1秒
                Threading.Thread.Sleep(1000)
                Continue Do
            End If
            If S3 = True Then
                Try
                    If thread_esd_send_right IsNot Nothing Then
                        '执行代码

                        If String.Equals(Scanning_interval.Trim, String.Empty) = True And IsNumeric(Scanning_interval.Trim) = True Then
                            Scanning_interval = 150
                        End If
                        If String.Equals(Scanning_timeout.Trim, String.Empty) = True And IsNumeric(Scanning_timeout.Trim) = True Then
                            Scanning_interval = 3
                        End If

                        'Threading.Thread.Sleep(Val(Scanning_interval))

                        For i = 1 To Val(ESD_DIC_right.Count)

                            ToolStripProgressBar2.Maximum = ESD_DIC_right.Count
                            ToolStripProgressBar2.Minimum = 0
                            ToolStripProgressBar2.Value = i

                            Threading.Thread.Sleep(Val(Scanning_interval))

                            If ESD_DIC_right(i)(0).ESD_isEnable.ToUpper = "FALSE" Then
                                'ESD被禁用,跳到下一条执行

                                Continue For
                            End If

                            Me.Invoke(New Action(Sub()
                                                     ToolStripTextBox2.Text = ESD_DIC_right(i)(0).ESD_query
                                                 End Sub))


                            Dim hex As String() = ESD_DIC_right(i)(0).ESD_query.Split(" ")

                            If Not hex.Count = 8 Then
                                Continue For '不足八位就跳过执行下一行指令
                            End If

                            Dim send_cmd As Byte() = New Byte() _
                                                     {"&H" & hex(0),
                                                      "&H" & hex(1),
                                                      "&H" & hex(2),
                                                      "&H" & hex(3),
                                                      "&H" & hex(4),
                                                      "&H" & hex(5),
                                                      "&H" & hex(6),
                                                      "&H" & hex(7)} '八位串口指令

                            SC_Relay_control_right.SendData(send_cmd, 0, send_cmd.Count) '循环查询ESD的状态

                            Me.Invoke(New Action(Sub()
                                                     ToolStripLabel4.Text = "右" & ESD_DIC_right(i)(0).ESD_NUM
                                                 End Sub))

                        Next



                    End If
                Catch ex As Exception
                    Log("B线程循环发送串口指令" & ex.Message)
                End Try
            End If
        Loop
        Log("[采集器B]线程循环发送串口指令 停止")
    End Sub

    ''' <summary>
    ''' 报警灯信息接收
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <param name="bits"></param>
    ''' <remarks></remarks>
    Private Sub sc_DataReceived_call_police(ByVal sender As System.Object, ByVal e As SerialDataReceivedEventArgs, ByVal bits As Byte())
        'ESD报警灯反馈的串口信号
    End Sub

    Dim S2 As Boolean = False
    ''' <summary>
    ''' 报警灯控制
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '/*串口事件注册
        '/*************************************************************************************************************/
        Try
            Select Case S2
                Case False

                    Relay_control_Cell_police = ComboBox4.Text '串口名称
                    Relay_control_Cell_police_Baud = ComboBox3.Text '串口波特率

                    SC_Cell_police_control.setSerialPort(
                    ComboBox4.Text, ComboBox3.Text, 8, IO.Ports.StopBits.One)
                    Control.CheckForIllegalCrossThreadCalls = False '线程之间进行通讯
                    AddHandler SC_Cell_police_control.DataReceived,
                    New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_call_police)
                    If SC_Cell_police_control.openPort() = False Then
                        Exit Sub
                    End If
                    S2 = True
                    Button2.Text = "停止(&Q)"
                    ComboBox4.Enabled = False
                    ComboBox3.Enabled = False
                    Button2.BackColor = Color.DarkSeaGreen '绿色

                Case True
                    Relay_control_Cell_police = ComboBox4.Text '串口名称
                    Relay_control_Cell_police_Baud = ComboBox3.Text '串口波特率

                    SC_Cell_police_control.closePort() '关闭串口
                    '卸载事件关联 
                    RemoveHandler SC_Cell_police_control.DataReceived,
                   New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_call_police)

                    S2 = False
                    Button2.Text = "启动(&S)"
                    ComboBox4.Enabled = True
                    ComboBox3.Enabled = True
                    Button2.BackColor = Color.Gainsboro '浅灰色

            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message & ex.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
        End Try

        '关闭串口连接
        'SC.closePort() 
        '/*************************************************************************************************************/

    End Sub

#Region "采集器B 串口接收事件"
    ''' <summary>
    ''' 右侧线体-串口接收数据委托事件集合(数采控制器)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <param name="bits"></param>
    ''' <remarks></remarks>
    Private Sub sc_DataReceived_right(ByVal sender As System.Object, ByVal e As SerialDataReceivedEventArgs, ByVal bits As Byte())

        '发送和接收逻辑都出现了速度过快导致的丢包问题,解决办法是增加延时
        Dim hexs() As String = BitConverter.ToString(bits).Replace("-", " ").Trim.Split

        Dim hex As New System.Text.StringBuilder
        For Each s In hexs
            hex.Append(s.ToString & " ")
        Next
        Thread.Sleep(2000)

        Me.Invoke(New Action(Sub()
                                 If bits.Length > 7 Then
                                     Log("B串口接收指令:" & hex.ToString & ",长度:" & bits.Length)
                                 End If
                                 ToolStripTextBox1.Text = hex.ToString
                                 If controller = "Right" Then
                                     sc_DataReceived(hex.ToString)
                                 End If



                             End Sub))

        If bits.Length Mod 6 = 0 Then
            '验证数据是否正确
            '避免因为接收延迟导致一次出现大量信息,下面的将接收到的信息进行拆分,然后进行数据更新
            Dim t_position_start As Int64 = 0, t_position_end As Int64 = 0, gethex As String = String.Empty
            For k = 0 To (bits.Length / 6) - 1
                t_position_start = k * 18 '开始位置
                t_position_end = 18 '结束位置
                gethex = Mid(hex.ToString.Trim, t_position_start + 1, t_position_end)

                For i = 1 To ESD_DIC_right.Count '遍历指令集合
                    Try
                        If ESD_DIC_right(i)(0).ESD_isEnable.ToUpper = "FALSE" Then
                            '被禁用就不显示状态
                            Dim control1 As Control = Controls.Find("ESDR" & i.ToString, True)(0)
                            TryCast(control1, ESD_STATE).ESDSTATE(ESD_STATE.State.禁用)
                            TryCast(control1, ESD_STATE).t_getdate = Now

                            TryCast(control1, ESD_STATE).BackColor = Color.Silver 'Yellow ESD 禁用改为灰色  ,不要黄色
                            TryCast(control1, ESD_STATE).ForeColor = Color.Black

                            Continue For
                        End If

                        Select Case gethex.Trim'hex.ToString.Trim
                            Case ESD_DIC_right(i)(0).ESD_Invalid.Trim
                                '错误

                                '获取到对应的控件对象
                                Dim control1 As Control = Controls.Find("ESDR" & i.ToString, True)(0)
                                TryCast(control1, ESD_STATE).ESDSTATE(ESD_STATE.State.错误)
                                TryCast(control1, ESD_STATE).t_getdate = Now


                                esdUpdateR.ESD_number = TryCast(control1, ESD_STATE).t_number
                                esdUpdateR.ESD_state = "ESD未佩戴好(防护失败)"
                                esdUpdateR.Monitor = t_Monitor
                                esdUpdateR.Line_body = t_Line_body
                                esdUpdateR.Update()

                                Dim control2 As Control = Controls.Find("B" & i.ToString, True)(0)
                                TryCast(control2, Button).BackColor = Color.Red
                                TryCast(control2, Button).ForeColor = Color.Blue

                            Case ESD_DIC_right(i)(0).ESD_normal.Trim
                                '正常

                                '获取到对应的控件对象
                                Dim control1 As Control = Controls.Find("ESDR" & i.ToString, True)(0)
                                TryCast(control1, ESD_STATE).ESDSTATE(ESD_STATE.State.正常)
                                TryCast(control1, ESD_STATE).t_getdate = Now


                                esdUpdateR.ESD_number = TryCast(control1, ESD_STATE).t_number
                                esdUpdateR.ESD_state = "防护正常"
                                esdUpdateR.Monitor = t_Monitor
                                esdUpdateR.Line_body = t_Line_body
                                esdUpdateR.Update()

                                Dim control2 As Control = Controls.Find("B" & i.ToString, True)(0)
                                TryCast(control2, Button).BackColor = Color.LawnGreen
                                TryCast(control2, Button).ForeColor = Color.Blue

                            Case ESD_DIC_right(i)(0).ESD_Off_line.Trim
                                '离线
                                '获取到对应的控件对象
                                Dim control1 As Control = Controls.Find("ESDR" & i.ToString, True)(0)
                                TryCast(control1, ESD_STATE).ESDSTATE(ESD_STATE.State.离线)
                                TryCast(control1, ESD_STATE).t_getdate = Now

                                '上传ESD信息到SQL
                                esdUpdateR.ESD_number = TryCast(control1, ESD_STATE).t_number
                                esdUpdateR.ESD_state = "ESD报警器未开(防护失败)"
                                esdUpdateR.Monitor = t_Monitor
                                esdUpdateR.Line_body = t_Line_body
                                esdUpdateR.Update()

                                Dim control2 As Control = Controls.Find("B" & i.ToString, True)(0)
                                TryCast(control2, Button).BackColor = Color.Red
                                TryCast(control2, Button).ForeColor = Color.Blue

                        End Select
                    Catch ex As Exception
                        Log("采集器B 串口接收事件" & ex.Message & vbCrLf & ex.StackTrace)
                    End Try
                Next
            Next
        End If



    End Sub
#End Region

    '============================================================设备可视化接收=============================================
    Dim num As Integer = 0
    Dim Cpath As String = My.Application.Info.DirectoryPath & "\" & "Parameter.ini"
    ''' <summary>
    ''' 设备可视化接收
    ''' </summary>
    ''' <param name="hex"></param>
    Private Sub sc_DataReceived(hex As String)

        Dim con As Control = TabPage2.Controls.Find("FlowLayoutPanel1", True).First
        For Each item As Control In con.Controls
            If TypeOf item Is UserDeviceInfo Then
                Dim UDI As UserDeviceInfo = CType(item, UserDeviceInfo)
                UDI.pb_01.Image = My.Resources.灯泡_灰色_
            End If
        Next


        Dim pathtxt = Path.GetFullPath("dataalltxt.txt")
        txtWrite(pathtxt, hex)
        Dim indata As String = hex.ToString
        'Dim devicesplit As String() = indata.Split("F")
        'Dim buff As Byte()
        'Dim receivestr_split As String()
        'For Each D In devicesplit
        '    If D.Length > 5 Then
        '        buff = CRCClass.HexString2Bytes(D)

        '        Dim byteF As Byte()

        '        Dim receivestr As String = CRCClass.ByteArrayToHexString(buff)
        '        receivestr_split = receivestr.Split(" "c)
        '        If receivestr_split(0) = 3 Then
        '            receivestr_split(0) = "F3"
        '            byteF = CRCClass.HexString2Bytes("F3")
        '            buff(0) = byteF(0)
        '        ElseIf receivestr_split(0) = 2 Then
        '            receivestr_split(0) = "F2"
        '            byteF = CRCClass.HexString2Bytes("F2")
        '            buff(0) = byteF(0)
        '        ElseIf receivestr_split(0) = 1 Then
        '            receivestr_split(0) = "F1"
        '            byteF = CRCClass.HexString2Bytes("F1")
        '            buff(0) = byteF(0)
        '        ElseIf receivestr_split(0) = 4 Then
        '            receivestr_split(0) = "F4"
        '            byteF = CRCClass.HexString2Bytes("F4")
        '            buff(0) = byteF(0)
        '        End If


        '    End If
        'Next



        Dim buf As Byte()
        buf = CRCClass.HexString2Bytes(indata)
        If indata.Length > 0 Then
            Dim receivestr As String = CRCClass.ByteArrayToHexString(buf)
            Dim receivestr_split As String() = receivestr.Split(" "c)


            Select Case receivestr_split(0)
                Case "F1"
                    Dim Device = DeviceReceiveDataClass.Voice(buf, receivestr_split)
                    Me.Invoke(New Action(Sub()
                                             Dim c As Control = TabPage2.Controls.Find("FlowLayoutPanel1", True).First
                                             For Each item As Control In c.Controls
                                                 If TypeOf item Is UserDeviceInfo Then
                                                     Dim UDI As UserDeviceInfo = CType(item, UserDeviceInfo)
                                                     If UDI.DeviceName = "F1" And UDI.DeviceAdrees = Device.address Then
                                                         UDI.pb_01.Image = My.Resources.灯泡_绿色_
                                                         UDI.Lb_DYieds.Text = Device.gross_product
                                                         UDI.Lb_DTime.Text = "保压时间：" + Device.Powertimme + "S"
                                                         UDI.Lb_DBedNum.Text = "螺丝不良数：" + Device.bad_num
                                                         UDI.Lb_DPower.Text = "后负压值：" + Device.Powervalue + "N"
                                                         Try

                                                             DeviceReceiveDataClass.DataUpLoad(Device.DeviceName, Device.address, Device.gross_product,
                                                                     Device.bad_num, Device.Powertimme, Device.Powervalue)


                                                         Catch ex As Exception

                                                         End Try

                                                     End If

                                                 End If
                                             Next
                                         End Sub))


                Case "F2"
                    Dim Device = DeviceReceiveDataClass.Lock_Machine_TTK1(buf, receivestr_split)
                    Try
                        Me.Invoke(New Action(Sub()
                                                 Dim c As Control = TabPage2.Controls.Find("FlowLayoutPanel1", True).First
                                                 For Each item As Control In c.Controls
                                                     If TypeOf item Is UserDeviceInfo Then
                                                         Dim UDI As UserDeviceInfo = CType(item, UserDeviceInfo)
                                                         If UDI.DeviceName = "F2" And UDI.DeviceAdrees = Device.address Then
                                                             'UDI.m_DeviceBedNum = Device.bad_num
                                                             'UDI.DeviceSumYields = Device.gross_product
                                                             'UDI.DeviceTime = Device.Powertimme
                                                             'UDI.DevicePower = Device.Powervalue
                                                             UDI.pb_01.Image = My.Resources.灯泡_绿色_
                                                             UDI.Lb_DYieds.Text = Device.gross_product
                                                             UDI.Lb_DTime.Text = "螺丝总数：" + Device.Powertimme
                                                             UDI.Lb_DBedNum.Text = "螺丝不良数：" + Device.bad_num
                                                             UDI.Lb_DPower.Text = "负压值：" + Device.Powervalue + "N"

                                                             Try
                                                                 DeviceReceiveDataClass.DataUpLoad(Device.DeviceName, Device.address, Device.gross_product,
                                                                     Device.bad_num, Device.Powertimme, Device.Powervalue)

                                                             Catch ex As Exception

                                                             End Try


                                                         End If

                                                     End If
                                                 Next
                                             End Sub))
                    Catch ex As Exception

                    End Try
                Case "F3"
                    Dim Device = DeviceReceiveDataClass.Lock_Machine_TTK2(buf, receivestr_split)
                    Me.Invoke(New Action(Sub()
                                             Dim c As Control = TabPage2.Controls.Find("FlowLayoutPanel1", True).First
                                             For Each item As Control In c.Controls
                                                 If TypeOf item Is UserDeviceInfo Then
                                                     Dim UDI As UserDeviceInfo = CType(item, UserDeviceInfo)
                                                     If UDI.DeviceName = "F3" And UDI.DeviceAdrees = Device.address Then
                                                         UDI.pb_01.Image = My.Resources.灯泡_绿色_
                                                         UDI.Lb_DYieds.Text = Device.gross_product
                                                         UDI.Lb_DTime.Text = "前负压值：" + Device.Powertimme + "N"
                                                         UDI.Lb_DBedNum.Text = "不 良 率：" + Device.bad_num + "%"
                                                         UDI.Lb_DPower.Text = "后负压值：" + Device.Powervalue + "N"

                                                         Try
                                                             DeviceReceiveDataClass.DataUpLoad(Device.DeviceName, Device.address, Device.gross_product,
                                                                     Device.bad_num, Device.Powertimme, Device.Powervalue)

                                                         Catch ex As Exception

                                                         End Try
                                                     End If
                                                 End If
                                             Next
                                         End Sub))
                Case "F4"

                    Dim Device = DeviceReceiveDataClass.Lock_Machine_TTK3(buf, receivestr_split)
                    Me.Invoke(New Action(Sub()
                                             Dim c As Control = TabPage2.Controls.Find("FlowLayoutPanel1", True).First
                                             For Each item As Control In c.Controls
                                                 If TypeOf item Is UserDeviceInfo Then
                                                     Dim UDI As UserDeviceInfo = CType(item, UserDeviceInfo)
                                                     If UDI.DeviceName = "F4" And UDI.DeviceAdrees = Device.address Then
                                                         UDI.pb_01.Image = My.Resources.灯泡_绿色_
                                                         UDI.Lb_DYieds.Text = Device.gross_product
                                                         UDI.Lb_DTime.Text = "前负压值：" + Device.Powertimme + "N"
                                                         UDI.Lb_DBedNum.Text = "不 良 数：" + Device.bad_num
                                                         UDI.Lb_DPower.Text = "后负压值：" + Device.Powervalue + "N"

                                                         Try
                                                             DeviceReceiveDataClass.DataUpLoad(Device.DeviceName, Device.address, Device.gross_product,
                                                                     Device.bad_num, Device.Powertimme, Device.Powervalue)

                                                         Catch ex As Exception

                                                         End Try
                                                     End If
                                                 End If
                                             Next
                                         End Sub))

            End Select
        End If
    End Sub

#Region "采集器A 串口接收事件"

    ''' <summary>
    ''' 左侧线体-串口接收数据委托事件集合(数采控制器)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <param name="bits"></param>
    ''' <remarks></remarks>
    Private Sub sc_DataReceived_Left(ByVal sender As System.Object, ByVal e As SerialDataReceivedEventArgs, ByVal bits As Byte())
        '发送和接收逻辑都出现了速度过快导致的丢包问题,解决办法是增加延时

        Thread.Sleep(1000)

        Dim hexs() As String = BitConverter.ToString(bits).Replace("-", " ").Trim.Split

        Dim hex As New System.Text.StringBuilder
        For Each s In hexs
            hex.Append(s.ToString & " ")
        Next
        Sleep(1000)

        Me.Invoke(New Action(Sub()
                                 If bits.Length > 6 Then
                                     Log("A串口接收指令:" & hex.ToString & ",长度:" & bits.Length)
                                 End If
                                 ToolStripTextBox1.Text = hex.ToString
                                 If controller = "Left" Then
                                     sc_DataReceived(hex.ToString)
                                 End If







                             End Sub))
        '问题记录,ESD被禁用以后还会出现报错防护失败,不应该有这种情况的,
        '前面已经屏蔽了状态为false的设备发送查询指令,不可能会收回反馈才对.

        If bits.Length Mod 6 = 0 Then
            '验证数据是否正确
            '避免因为接收延迟导致一次出现大量信息,下面的将接收到的信息进行拆分,然后进行数据更新
            Dim t_position_start As Int64 = 0, t_position_end As Int64 = 0, gethex As String = String.Empty
            For k = 0 To (bits.Length / 6) - 1
                t_position_start = k * 18 '开始位置
                t_position_end = 18 '结束位置
                gethex = Mid(hex.ToString.Trim, t_position_start + 1, t_position_end)

                For i = 1 To ESD_DIC_Left.Count '遍历指令集合
                    Try
                        If ESD_DIC_Left(i)(0).ESD_isEnable.ToUpper = "FALSE" Then
                            '被禁用就不显示状态
                            Dim control1 As Control = Controls.Find("ESDL" & i.ToString, True)(0)
                            TryCast(control1, ESD_STATE).ESDSTATE(ESD_STATE.State.禁用)
                            TryCast(control1, ESD_STATE).t_getdate = Now

                            TryCast(control1, ESD_STATE).BackColor = Color.Silver 'Yellow ESD 禁用改为灰色  ,不要黄色
                            TryCast(control1, ESD_STATE).ForeColor = Color.Black

                            Continue For
                        End If

                        Select Case gethex.Trim 'hex.ToString.Trim
                            Case ESD_DIC_Left(i)(0).ESD_Invalid.Trim
                                '错误
                                '获取到对应的控件对象
                                Dim control1 As Control = Controls.Find("ESDL" & i.ToString, True)(0)
                                TryCast(control1, ESD_STATE).ESDSTATE(ESD_STATE.State.错误)
                                TryCast(control1, ESD_STATE).t_getdate = Now


                                esdUpdateL.ESD_number = TryCast(control1, ESD_STATE).t_number
                                esdUpdateL.ESD_state = "ESD未佩戴好(防护失败)"
                                esdUpdateL.Monitor = t_Monitor
                                esdUpdateL.Line_body = t_Line_body
                                esdUpdateL.Update()

                                Dim control2 As Control = Controls.Find("A" & i.ToString, True)(0)
                                TryCast(control2, Button).BackColor = Color.Red
                                TryCast(control2, Button).ForeColor = Color.Blue

                            Case ESD_DIC_Left(i)(0).ESD_normal.Trim
                                '正常
                                '获取到对应的控件对象
                                Dim control1 As Control = Controls.Find("ESDL" & i.ToString, True)(0)
                                TryCast(control1, ESD_STATE).ESDSTATE(ESD_STATE.State.正常)
                                TryCast(control1, ESD_STATE).t_getdate = Now


                                esdUpdateL.ESD_number = TryCast(control1, ESD_STATE).t_number
                                esdUpdateL.ESD_state = "防护正常"
                                esdUpdateL.Monitor = t_Monitor
                                esdUpdateL.Line_body = t_Line_body
                                esdUpdateL.Update()

                                Dim control2 As Control = Controls.Find("A" & i.ToString, True)(0)
                                TryCast(control2, Button).BackColor = Color.LawnGreen
                                TryCast(control2, Button).ForeColor = Color.Blue

                            Case ESD_DIC_Left(i)(0).ESD_Off_line.Trim
                                '离线
                                '获取到对应的控件对象
                                Dim control1 As Control = Controls.Find("ESDL" & i.ToString, True)(0)
                                TryCast(control1, ESD_STATE).ESDSTATE(ESD_STATE.State.离线)
                                TryCast(control1, ESD_STATE).t_getdate = Now


                                esdUpdateL.ESD_number = TryCast(control1, ESD_STATE).t_number
                                esdUpdateL.ESD_state = "ESD报警器未开(防护失败)"
                                esdUpdateL.Monitor = t_Monitor
                                esdUpdateL.Line_body = t_Line_body
                                esdUpdateL.Update()

                                Dim control2 As Control = Controls.Find("A" & i.ToString, True)(0)
                                TryCast(control2, Button).BackColor = Color.Red
                                TryCast(control2, Button).ForeColor = Color.Blue

                        End Select
                    Catch ex As Exception
                        Log("采集器A 串口接收事件" & ex.Message & vbCrLf & ex.StackTrace)
                    End Try
                Next
            Next
        End If
    End Sub
#End Region

#Region "统计图"

    ''' <summary>
    ''' 统计图表线程对象
    ''' </summary>
    Public thread_Statistical_chart As Threading.Thread

    ''' <summary>
    ''' 启动统计图表线程
    ''' </summary>
    Private Sub Statistical_thread_start()
        thread_Statistical_chart = New Threading.Thread(AddressOf Statistical_chart)
        thread_Statistical_chart.IsBackground = False '后台线程
        thread_Statistical_chart.Name = "统计图表线程"
        thread_Statistical_chart.Start()
    End Sub

    ''' <summary>
    ''' 终止线程
    ''' </summary>
    Private Sub Statistical_thread_stop()
        If thread_Statistical_chart IsNot Nothing Then
            thread_Statistical_chart.Abort() '结束线程
        End If
    End Sub

    ''' <summary>
    ''' 指示线程是否被启动
    ''' </summary>
    Dim chart_bol As Boolean = False
    ''' <summary>
    ''' 统计图表方法
    ''' </summary>
    Private Sub Statistical_chart()
        If chart_bol = True Then
            '避免重复启动线程
            Exit Sub
        End If
        chart_bol = True
        Log("统计图表方法线程 启动")
        Do
            Try
                If thread_Statistical_chart IsNot Nothing Then
                    If thread_Statistical_chart.IsAlive = False Then
                        Exit Do '线程结束就退出循环
                    End If
                End If

                Dim data1 As String = ""
                Dim data2 As String = ""

                If Now.Hour < 8 Then
                    data1 = Format(Now.AddDays(-1), "yyyy-MM-dd")
                    data2 = Format(Now, "yyyy-MM-dd")
                Else
                    data1 = Format(Now, "yyyy-MM-dd")
                    data2 = Format(Now.AddDays(1), "yyyy-MM-dd")
                End If

                '只抓取前异常最多的前30个设备
                Dim sql As String = "select top 30 t_ESD_number as ESD编号 ,count(t_Duration_time) as 次数,sum(t_Duration_time) as 持续时间, (sum(t_Duration_time) / count(t_Duration_time)) as 平均持续时长 from t_ESD_status_record where t_Creation_time between '" & data1 &
                    " 08:00:00' and '" & data2 & " 08:00:00' and t_Line_body = '" & t_Line_body & "' and t_Duration_time is not null and t_ESD_state like '%失败%' group by t_ESD_number order by count(t_Duration_time) desc"

                Dim GetDic As Dictionary(Of String, Set_indexes) = New Dictionary(Of String, Set_indexes)

                Dim Getlist As List(Of Object()) = Data_query(sql, New ConnectionString().ConnectionInfo)

                Dim getcount As Integer = 0 '计数

                For Each T In Getlist
                    If T.Count = 4 Then
                        Dim tlist As New Set_indexes With {
                                          .Get_ESD_number = T(0).ToString,
                                          .Get_Abnormality_times = T(1).ToString,
                                          .Get_Abnormal_time = T(2).ToString,
                                          .Get_average_time = T(3).ToString}
                        getcount += 1

                        If GetDic.Keys.Contains(getcount) = True Then
                            '存在就更新
                            GetDic(getcount) = tlist
                        Else
                            '不存在就新增
                            GetDic.Add(getcount, tlist)
                        End If
                    End If
                Next

                '绘图
                'CreateGraph_GradientByZBars(ZedGraphControl, Now.Month & "月" & Now.Day & "日ESD状态统计", GetDic, 1, 2, 13)
            Catch ex As Exception
                Log("统计图表方法刷新线程 | " & ex.Message & "|" & ex.StackTrace)
            End Try
            Threading.Thread.Sleep(1000 * 60 * 5) '10分钟刷新一次图表
        Loop
        Log("统计图表方法线程 停止")
        chart_bol = False
    End Sub

    ''' <summary>
    ''' 柱子图,异常次数日看板
    ''' </summary>
    ''' <param name="z1">图表控件名称</param>
    ''' <param name="Title_name">标题名称</param>
    ''' <param name="getdic">数据源</param>
    ''' <param name="displacement">标签偏移量</param>
    ''' <param name="MAX">Y1最大值</param>
    ''' <param name="XFontSpec">X左边字体大小</param>
    ''' <remarks></remarks>
    Private Sub CreateGraph_GradientByZBars(ByVal z1 As ZedGraph.ZedGraphControl, ByVal Title_name As String, ByVal getdic As Dictionary(Of String, Set_indexes), ByVal displacement As Integer, ByVal MAX As Integer, ByVal XFontSpec As Integer)

        Dim myPane As ZedGraph.GraphPane = z1.GraphPane
        myPane.CurveList.Clear()
        myPane.GraphObjList.Clear()
        myPane.Title.Text = Title_name ' Format(Now(), "MM") & "月维修分布"
        myPane.XAxis.Title.Text = "ESD编号"
        myPane.YAxis.Title.Text = "异常数次"
        myPane.Y2Axis.Title.Text = "异常时间/小时"

        Dim loss_list As New ZedGraph.PointPairList '损耗时长 折线

        Dim Sum_values As Integer = 0 '合计数量
        Dim lower As New ZedGraph.PointPairList() '折线
        Dim x_val As New List(Of Double)
        Dim t_Max As Integer = 0 '最大值
        Dim Y2_MAX As Integer = 0 'Y2坐标最大值

        Dim list As New ZedGraph.PointPairList()


        For i = 1 To getdic.Count
            Dim time As String = getdic.Item(i).Get_ESD_number 'ESD编号
            Dim values As Double = getdic.Item(i).Get_Abnormality_times '异常次数
            Dim values2 As Double = 0
            Try
                If String.Compare(getdic.Item(i).Get_Abnormal_time.Trim(), String.Empty, False) = 0 Then
                    Continue For
                End If

                values2 = Math.Round(getdic.Item(i).Get_Abnormal_time / 3600, 2) '损耗时长
                If Y2_MAX <= values2 Then
                    Y2_MAX = values2 + MAX
                End If

            Catch ex As Exception
                Log("柱子图,异常次数日看板" & ex.Message & ex.StackTrace.ToString)
            End Try

            Dim x_start As Double '= New ZedGraph.XDate(time) 'X轴时间
            list.Add(x_start, CDbl(values), CDbl(values) & "次")

            loss_list.Add(x_start, CDbl(values2), time & "," & CDbl(values2) & "/h")

            Sum_values += CDbl(values)
            x_val.Add(CDbl(values))
            If t_Max < values Then
                t_Max = values + MAX
            End If
        Next

        ZedGraph.BarItem.CreateBarLabels(myPane, False, "0")
        Dim myCurvea = myPane.AddCurve("平均次数", lower, Color.Red, ZedGraph.SymbolType.HDash) '画折线 平均
        Dim myCurvea2 = myPane.AddCurve("损耗时长", loss_list, Color.MediumSlateBlue, ZedGraph.SymbolType.HDash) '画折线 损耗
        '定义折线线条粗度
        myCurvea2.Symbol.Size = 8.0F
        myCurvea2.Symbol.Fill = New ZedGraph.Fill(Color.White)
        myCurvea2.Line.Width = 2.0F
        Dim myBara As ZedGraph.BarItem = myPane.AddBar("维修次数", Nothing, x_val.ToArray, Color.MediumSeaGreen) '画柱状图

        myCurvea2.IsY2Axis = True '手动改为按【Y2Axis】的刻度描画 
        myCurvea2.Line.IsSmooth = True '曲线平滑

        For i = 0 To list.Count - 1
            Dim pt As ZedGraph.PointPair = myBara.Points(i)

            Dim text As ZedGraph.TextObj = New ZedGraph.TextObj(pt.Y.ToString("f2"),
                                                                pt.X,
                                                                pt.Y + displacement,
                                                                ZedGraph.CoordType.AxisXYScale,
                                                                ZedGraph.AlignH.Left - 1,
                                                                ZedGraph.AlignV.Center)
            text.ZOrder = ZedGraph.ZOrder.A_InFront
            '// 隐藏标注的边框和填充
            text.FontSpec.Border.IsVisible = False
            text.FontSpec.Fill.IsVisible = False
            '// 选择标注字体90°
            text.FontSpec.Angle = 0
            text.Text = Int(text.Text) & "次" '柱子上标记字符
            myPane.GraphObjList.Add(text)

            Dim values As Double = (Sum_values / list.Count)
            lower.Add(CDbl(values), CDbl(values), Math.Round(CDbl(values), 2) & "次/平均") '平均值折线
        Next


        '定义折线线条粗度
        myCurvea.Symbol.Size = 8.0F
        myCurvea.Symbol.Fill = New ZedGraph.Fill(Color.White)
        myCurvea.Line.Width = 2.0F


        myPane.Chart.Fill = New ZedGraph.Fill(Color.White, Color.FromArgb(255, Color.ForestGreen), 90.0F)

        myPane.XAxis.MajorGrid.IsVisible = True
        myPane.Legend.Position = ZedGraph.LegendPos.Top
        myPane.Legend.FontSpec.Size = 10.0F
        myPane.Legend.FontSpec.FontColor = Color.Red
        myPane.Legend.IsHStack = True

        myPane.XAxis.Type = ZedGraph.AxisType.Text
        Dim PairList_new As New List(Of String)

        For i = 1 To getdic.Count
            PairList_new.Add(getdic.Item(i).Get_ESD_number.Trim())
        Next

        myPane.XAxis.Scale.TextLabels = PairList_new.ToArray '设置坐标名称


        myPane.XAxis.Scale.MajorStep = 1
        myPane.Legend.IsVisible = True
        myPane.XAxis.CrossAuto = True
        myPane.Legend.FontSpec.Size = 13 '标签的字体大小
        myPane.XAxis.Scale.Min = 0
        myPane.XAxis.Scale.FontSpec.Size = XFontSpec 'X坐标字体大小


        myPane.XAxis.Scale.FontSpec.Angle = 90 '旋转X左边文字的角度
        myPane.XAxis.MajorGrid.IsVisible = True
        myPane.YAxis.MajorGrid.IsVisible = True
        myPane.YAxis.Scale.Max = Double.Parse(10) '定义网格线的密集程度
        'Make the Y axis scale red
        myPane.YAxis.Scale.FontSpec.FontColor = Color.Red
        myPane.YAxis.Title.FontSpec.FontColor = Color.Red
        'turn off the opposite tics so the Y tics don't show up on the Y2 axis
        myPane.YAxis.MajorTic.IsOpposite = False
        myPane.YAxis.MinorTic.IsOpposite = False
        'Don() 't display the Y zero line
        myPane.YAxis.MajorGrid.IsZeroLine = False
        'Align the Y axis labels so they are flush to the axis
        myPane.YAxis.Scale.Align = ZedGraph.AlignP.Inside
        myPane.YAxis.Scale.Min = 0
        myPane.YAxis.Scale.Max = t_Max + 5 'Y轴最大值 
        myPane.YAxis.Scale.FontSpec.Size = 9
        myPane.YAxis.Scale.FormatAuto = True


        ' Enable the Y2 axis display
        myPane.Y2Axis.IsVisible = True
        ' Make the Y2 axis scale blue
        myPane.Y2Axis.Scale.FontSpec.FontColor = Color.Blue
        myPane.Y2Axis.Title.FontSpec.FontColor = Color.Blue
        ' turn off the opposite tics so the Y2 tics don't show up on the Y axis
        myPane.Y2Axis.MajorTic.IsOpposite = False
        myPane.Y2Axis.MinorTic.IsOpposite = False
        ' Display the Y2 axis grid lines
        'myPane.Y2Axis.MajorGrid.IsVisible = True
        ' Align the Y2 axis labels so they are flush to the axis
        myPane.Y2Axis.Scale.Align = ZedGraph.AlignP.Inside
        myPane.Y2Axis.Scale.Min = 0
        myPane.Y2Axis.Scale.Max = Y2_MAX + 5
        'myPane.Y2Axis.Scale.FormatAuto = True
        myPane.Y2Axis.Scale.FontSpec.Size = 9


        'Fill the axis background with a gradient
        myPane.Chart.Fill = New ZedGraph.Fill(Color.White, Color.White, 40.0F) '图表背景颜色

        z1.IsShowPointValues = True '鼠标经过图表上的点时是否气泡显示该点所对应的值。默认为false

        z1.AxisChange() '修改图表
        z1.Refresh()

    End Sub
    ''' <summary>
    ''' ESD关键指标
    ''' </summary>
    Private Structure Set_indexes
        ''' <summary>
        ''' ESD编号
        ''' </summary>
        ''' <returns></returns>
        Public Property Get_ESD_number As String
        ''' <summary>
        ''' 异常次数
        ''' </summary>
        ''' <returns></returns>
        Public Property Get_Abnormality_times As String
        ''' <summary>
        ''' 异常时间
        ''' </summary>
        ''' <returns></returns>
        Public Property Get_Abnormal_time As String
        ''' <summary>
        ''' 平均时间
        ''' </summary>
        ''' <returns></returns>
        Public Property Get_average_time As String

    End Structure

    ''' <summary>
    ''' 设置扬声器可视化地址
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub bn_DeviceSet_Click(sender As Object, e As EventArgs) Handles bn_DeviceSet.Click
        DeviceSet.Hide()
        DeviceSet.ShowDialog()
    End Sub

#End Region


    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        t_state = t_Pattern_selection.线体选择

        from_Squad_leader_selection.Hide()
        from_Squad_leader_selection.ShowDialog()

        Button4.Text = t_Line_body
    End Sub




    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        t_state = t_Pattern_selection.班长选择

        from_Squad_leader_selection.Hide()
        from_Squad_leader_selection.ShowDialog()

        Button3.Text = t_Monitor
    End Sub

    ''' <summary>
    ''' 线体左边线程
    ''' </summary>
    ''' <remarks></remarks>
    Dim thread_esd_send_Left As Threading.Thread
    ''' <summary>
    ''' ESD线程启动左边
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub thread_run_Left()
        thread_esd_send_Left = New Threading.Thread(AddressOf thread_senddate_Left)
        Control.CheckForIllegalCrossThreadCalls = False '不检测线程安全
        thread_esd_send_Left.Name = "ESD串口通信线程"
        thread_esd_send_Left.IsBackground = True '后台线程
        thread_esd_send_Left.Start() '启动线程
    End Sub

    '''' <summary>
    '''' 设备线程
    '''' </summary>
    'Private Sub thread_run_voice()
    '    Dim thread_send_voice As Thread = New Threading.Thread(AddressOf Thread_voice())
    '    Control.CheckForIllegalCrossThreadCalls = False '不检测线程安全
    '    thread_send_voice.Name = "设备串口通信线程"
    '    thread_send_voice.IsBackground = True '后台线程
    '    thread_send_voice.Start() '启动线程

    'End Sub


    ''' <summary>
    '''================================================================== 设备可视化发送===========================================
    ''' </summary>
    ''' <param name="port"></param>
    ''' <param name="send"></param>
    Public Sub Thread_voice(port As SerialPort, send As String)

        If port.IsOpen = False Then
            port.Open()
        End If
        port.Handshake = Handshake.None
        port.StopBits = StopBits.One
        port.Parity = Parity.None
        port.WriteTimeout = 1000
        port.ReadTimeout = 2000
        port.DataBits = 8

        Dim bty As Byte() = CRCClass.HexString2Bytes(send)
        '校验位
        Dim qw As String = CRCClass.CRCCalc(send)
        send &= " " + qw
        Dim sendsplit As String() = send.Split(" "c)

        Dim buff As [Byte]() = New [Byte](sendsplit.Length - 1) {}
        send = ""
        Dim i As Integer = 0
        While i < sendsplit.Length

            send += sendsplit(i) + " "
            i += 1
        End While

        Me.Invoke(New Action(Sub()
                                 ToolStripTextBox2.Text = send
                             End Sub))





        Dim bufW As Byte() = CRCClass.HexString2Bytes(send)

        Try

            port.Write(bufW, 0, bufW.Length)
        Catch ex As Exception

            MessageBox.Show(ex.ToString())
        End Try
    End Sub


    Private Sub thread_senddate_Left()
        Log("A线程循环发送串口指令 启动")
        Do


            '==========================================================设备可视化发送指令=========================================================

            Dim num = Convert.ToInt32(Model_set.GetINI("Device", "Num", "", inipath))
            For i = 1 To num
                Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", inipath)
                If DeviceValue IsNot "" Then

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
                        Case "螺丝机"
                            DeviceList(1) = "F5"
                        Case "贴膜机"
                            DeviceList(1) = "F6"



                    End Select
                    send = DeviceList(1) + " " + DeviceList(2)
                    controller = DeviceList(3)
                    Threading.Thread.Sleep(1000)
                    Select Case controller
                        Case "A"
                            controller = "Left"
                            Thread_voice（SC_Relay_control_Left.Serialport(), send）

                        Case "B"
                            controller = "Right"
                            Thread_voice（SC_Relay_control_right.Serialport(), send）

                    End Select



                End If
            Next





            '========================================================================================================


            If thread_bol = False Then
                '延迟1秒
                Threading.Thread.Sleep(1000)
                Continue Do
            End If
            If S1 = True Then
                Try
                    If thread_esd_send_Left IsNot Nothing Then
                        '执行代码

                        If String.Equals(Scanning_interval.Trim, String.Empty) = True Or IsNumeric(Scanning_interval.Trim) = False Then
                            Scanning_interval = 1000
                        End If
                        If String.Equals(Scanning_timeout.Trim, String.Empty) = True Or IsNumeric(Scanning_timeout.Trim) = False Then
                            Scanning_timeout = 3
                        End If

                        'Threading.Thread.Sleep(Val(Scanning_interval))

                        For i = 1 To Val(ESD_DIC_Left.Count)

                            ToolStripProgressBar1.Maximum = ESD_DIC_Left.Count
                            ToolStripProgressBar1.Minimum = 0
                            ToolStripProgressBar1.Value = i

                            Threading.Thread.Sleep(Val(Scanning_interval)) '发送数据进行延时

                            If ESD_DIC_Left(i)(0).ESD_isEnable.ToUpper = "FALSE" Then
                                Continue For  'ESD被禁用,跳到下一条执行
                            End If

                            Me.Invoke(New Action(Sub()
                                                     ToolStripTextBox2.Text = ESD_DIC_Left(i)(0).ESD_query
                                                 End Sub))

                            Dim hex As String() = ESD_DIC_Left(i)(0).ESD_query.Split(" ")

                            If Not hex.Count = 8 Then
                                Continue For '不足八位就跳过执行下一行指令
                            End If

                            Dim send_cmd As Byte() = New Byte() _
                                                     {"&H" & hex(0),
                                                      "&H" & hex(1),
                                                      "&H" & hex(2),
                                                      "&H" & hex(3),
                                                      "&H" & hex(4),
                                                      "&H" & hex(5),
                                                      "&H" & hex(6),
                                                      "&H" & hex(7)} '八位串口指令


                            'Voice.Thread_voice(SC_Relay_control_Left.Serialport())

                            SC_Relay_control_Left.SendData(send_cmd, 0, send_cmd.Count) '循环查询ESD的状态




                            Me.Invoke(New Action(Sub()
                                                     ToolStripLabel4.Text = "左" & ESD_DIC_Left(i)(0).ESD_NUM
                                                 End Sub))
                        Next



                    End If
                Catch ex As Exception
                    Log("A线程循环发送串口指令 " & ex.Message)
                End Try
            End If
        Loop
        Log("A线程循环发送串口指令 停止")
    End Sub


    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If TabControl1.SelectedTab.Name = "TabPage1" Then

            Dim Tab As TabPage = TabPage1


            'Dim Tab As TabPage = New TabPage
            Dim From As ESD = New ESD
            From.TopLevel = False
            Tab.Controls.Add(From)
            'TabControl1.TabPages.Add(Tab)
            From.Show()

            'Form1.Show()
            'Panel2.Show()
            'Panel5.Hide()

        Else
            'Panel5.Show()
            'Panel2.Hide()
            ESD.Hide()
        End If



    End Sub

#Region "临时数据导入"
    Private Sub XG()
        For Each l In My.Resources.String1.Split(vbCrLf)
            Dim k As String() = l.Split(",")


            'Dim t_Equipment_number As String = k(0).Trim
            'Dim t_Device_type As String = k(1).Trim

            'Dim strContent As String = "t_Remarks='" & t_Device_type & "'"

            'Dim cmd As DBCommand = New DBCommand(New ConnectionString().ConnectionInfo)
            'If cmd.Update("tab_Equipment_number ", strContent, "t_Equipment_number", "'" & t_Equipment_number & "'") > 0 Then

            'Else
            '    log("NG")
            'End If

            Dim sql As String = "insert into t_Monitor_info (t_Creation_time,t_modify_time,t_Work_number,t_name,t_Commissioner) values (getdate(),getdate(),'" & k(0).Trim & "','" & k(1).Trim & "','" & k(2).Trim & "')"

            Dim cmd As DBCommand = New DBCommand(New ConnectionString().ConnectionInfo)
            If cmd.Insert(sql) > 0 Then
            Else
                Log(sql)
            End If
        Next
    End Sub
#End Region
    ''' <summary>
    ''' 消息队列
    ''' </summary>
    Private mq As MessageQueue

    ''' <summary>
    ''' ESD控件集合(左边)
    ''' </summary>
    ''' <remarks></remarks>
    Dim esdcontrols_Left As New List(Of ESD_STATE)

    ''' <summary>
    ''' ESD控件集合(右边)
    ''' </summary>
    ''' <remarks></remarks>
    Dim esdcontrols_right As New List(Of ESD_STATE)

    ''' <summary>
    ''' 程序启动,初始化
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>

    Private Sub MainFrom_Load(sender As Object, e As EventArgs) Handles MyBase.Load



        ToolStripLabel10.Text = "Date:" + Date.Now.ToString
        Dim Tab As TabPage = TabPage1
        Dim From As ESD = New ESD
        From.TopLevel = False
        Tab.Controls.Add(From)
        From.Show()
        'Panel5.Hide()

        ESD_DIC_Left.Clear()
        ESD_DIC_right.Clear()

        Button2.Enabled = False
        Synchronization_time() '将本地计算机与服务器时间进行同步
        Me.Text = "设备可视化 " & getver

        NotifyIcon1.Text = "设备可视化"
        '关联ESD点击事件
        AddHandler ESD_STATE.ESD_Event, AddressOf ESD_click

        T_Retry_Count = 3

        Scanning_timeout = 3 '定义超时时间/分钟

        Me.WindowState = FormWindowState.Maximized

        'TableLayoutPanel1.SetColumnSpan(Panel1, 2)

        For i = 1 To Val(ESD_NUM_Left)
            Dim esd As New ESD_Kanban.ESD_STATE
            esd = New ESD_Kanban.ESD_STATE With {.t_number = "A-" & i.ToString("000")}
            esd.ESDSTATE(ESD_STATE.State.初始化)
            esd.t_position = Model.Left '左侧
            If esd.IsEnable.ToUpper = "FALSE" Then
                esd.ESDSTATE(State.禁用)
            End If
            esd.Name = "ESDL" & i.ToString
            'FlowLayoutPanel1.Controls.Add(esd)
            esdcontrols_Left.Add(esd)
        Next

        For i = 1 To Val(ESD_NUM_right)
            Dim esd As New ESD_Kanban.ESD_STATE
            esd = New ESD_Kanban.ESD_STATE With {.t_number = "B-" & i.ToString("000")}
            esd.ESDSTATE(ESD_STATE.State.初始化)
            esd.t_position = Model.right '右侧
            If esd.IsEnable.ToUpper = "FALSE" Then
                esd.ESDSTATE(State.禁用)
            End If
            esd.Name = "ESDR" & i.ToString
            'FlowLayoutPanel2.Controls.Add(esd)
            esdcontrols_right.Add(esd)
        Next

        Timer1.Interval = 500
        Timer1.Enabled = True




        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox4.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox5.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox6.DropDownStyle = ComboBoxStyle.DropDownList

        ComboBox1.Items.Clear()
        ComboBox4.Items.Clear()
        ComboBox5.Items.Clear()
        ComboBox6.Items.Clear()

        Dim ports As String() = SerialPort.GetPortNames() '必须用命名空间，用SerialPort,获取计算机的有效串口
        Dim port As String
        For Each port In ports
            ComboBox1.Items.Add(port) '向combobox中添加项
            ComboBox4.Items.Add(port)
            ComboBox6.Items.Add(port)
        Next port
        ComboBox1.SelectedIndex = 0
        ComboBox4.SelectedIndex = 0
        ComboBox6.SelectedIndex = 0

        ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox3.DropDownStyle = ComboBoxStyle.DropDownList

        Dim Baud As Integer() = {300, 600, 1200, 4800, 9600, 19200, 38400, 43000, 56000, 57600, 115200} '定义波特率
        ComboBox2.Items.Clear()
        ComboBox3.Items.Clear()
        For Each t_Baud As Integer In Baud
            ComboBox2.Items.Add(t_Baud.ToString)
            ComboBox3.Items.Add(t_Baud.ToString)
            ComboBox5.Items.Add(t_Baud.ToString)
        Next
        ComboBox2.SelectedIndex = 4
        ComboBox3.SelectedIndex = 4
        ComboBox5.SelectedIndex = 4

        If Not Relay_control_source_Left = String.Empty Then
            ComboBox1.Text = Relay_control_source_Left  '数采控制器串口名称A
        End If

        If Not Relay_control_source_Baud_Left = String.Empty Then
            ComboBox2.Text = Relay_control_source_Baud_Left  '数采控制器串口波特率A
        End If

        If Not Relay_control_source_right = String.Empty Then
            ComboBox6.Text = Relay_control_source_right  '数采控制器串口名称B
        End If

        If Not Relay_control_source_Baud_right = String.Empty Then
            ComboBox5.Text = Relay_control_source_Baud_right  '数采控制器串口波特率B
        End If

        If Not Relay_control_Cell_police = String.Empty Then
            ComboBox4.Text = Relay_control_Cell_police  '报警灯串口名称
        End If

        If Not Relay_control_Cell_police_Baud = String.Empty Then
            ComboBox3.Text = Relay_control_Cell_police_Baud  '报警灯串口波特率
        End If

        Button9_Click(Nothing, Nothing)
        Button5_Click(Nothing, Nothing)
        'Button2_Click(Nothing, Nothing)


        For i = 1 To Val(ESD_NUM_Left)
            Dim listadd As New List(Of ESDConfig)
            '如果没有找到配置文件就不添加到集合中
            Dim ESDCon As ESDConfig = ESD_NUM(i.ToString("0000"), Choice.Left)
            ESDCon.ESD_isEnable = GetINI("ESDEnable_Left", "ESDA-" & i.ToString("000"), "", Configurationpath) '获取控件启用状态
            If ESDCon.ESD_query Is Nothing Then
                Continue For
            End If
            listadd.Add(ESDCon)
            ESD_DIC_Left.Add(i, listadd)
        Next

        For i = 1 To Val(ESD_NUM_right)
            Dim listadd As New List(Of ESDConfig)
            '如果没有找到配置文件就不添加到集合中
            Dim ESDCon As ESDConfig = ESD_NUM(i.ToString("0000"), Choice.right)
            ESDCon.ESD_isEnable = GetINI("ESDEnable_right", "ESDB-" & i.ToString("000"), "", Configurationpath) '获取控件启用状态
            If ESDCon.ESD_query Is Nothing Then
                Continue For
            End If
            listadd.Add(ESDCon)
            ESD_DIC_right.Add(i, listadd)
        Next





        '这里不采用预设值的数量作为ESD数来显示，而采取有效数为基础来显示
        ToolStripLabel1.Text = "Left ESD:" & ESD_DIC_Left.Count 'ESD_NUM_Left
        ToolStripLabel15.Text = "right ESD:" & ESD_DIC_right.Count 'ESD_NUM_right

        Button3.Text = t_Monitor
        Button4.Text = t_Line_body

        'Dim y As Integer = ESD_DIC.Count

        ToolStripLabel7.Text = Scanning_interval
        ToolStripLabel9.Text = Scanning_timeout

        Coltrols_Call_police_Left()

        Button6_Click(Nothing, Nothing)


        Statistical_thread_start()





        Try
            Dim path As String = ".\private$\killf"
            If MessageQueue.Exists(path) Then
                mq = New MessageQueue(path)
            Else
                mq = MessageQueue.Create(path)
            End If
            mq.Formatter = New XmlMessageFormatter(New Type() {GetType(String)})
        Catch ex As Exception
        End Try

        Try
            '程序开机启动
            My.Computer.Registry.SetValue("HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run",
                                          IO.Path.GetFileName(Application.ExecutablePath),
                                          Application.StartupPath & "\" & IO.Path.GetFileName(Application.ExecutablePath))
        Catch ex As Exception
        End Try


        '处理未捕获的异常
        'Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        '处理UI线程异常
        AddHandler Application.ThreadException, New Threading.ThreadExceptionEventHandler(AddressOf Application_ThreadException)
        '处理非UI线程异常
        AddHandler AppDomain.CurrentDomain.UnhandledException, New UnhandledExceptionEventHandler(AddressOf CurrentDomain_UnhandledException)

        '因为数据库存在死锁的问题,暂停使用失败更新 - 2019-03-22
        Dim threadSQLUpdate As New Threading.Thread(AddressOf SqlUpdate)
        threadSQLUpdate.IsBackground = True
        threadSQLUpdate.Name = "数据库更新操作线程"
        'threadSQLUpdate.Start()





        Dim NumSave As String = Model_set.GetINI("Device", "num", "", Cpath)

        For i = 1 To Convert.ToInt32(NumSave)

            Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", Cpath)
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



            Dim UD As New UserDeviceInfo With {
                                                .Name = DeviceList(1) & DeviceList(0).ToString,
                                                .DeviceName = DeviceList(1),
                                                .DeviceAdrees = DeviceList(2),
                                                .DeviceBedNum = "不 良 数：",
                                                .DeviceTime = "保压时间：",
                                                .DevicePower = "保压压力："
                                                }
            FlowLayoutPanel1.Controls.Add(UD)

        Next





        Timer1.Interval = 1000
        Timer1.Start()




    End Sub

    ''' <summary>
    ''' 数据库更新操作线程
    ''' </summary>
    Private Sub SqlUpdate()
        Do
            Try
                Retry_uploading.Retry_update() '检查并更新因网络问题导致数据上传失败的数据文件
                Threading.Thread.Sleep(1000 * 5)
            Catch ex As Exception
            End Try
        Loop

    End Sub


    ''' <summary>
    ''' 报警状态轮询
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Coltrols_Call_police_Left()
        Dim thread_controls_do As New Threading.Thread(AddressOf controls_do)
        thread_controls_do.IsBackground = False
        thread_controls_do.Name = "报警状态轮询"
        thread_controls_do.Start()
    End Sub

    ''' <summary>
    ''' 扫描次数
    ''' </summary>
    ''' <remarks></remarks>
    Dim Scanning_times As Integer = 0

    ''' <summary>
    ''' 避免线程重复启动
    ''' </summary>
    Dim controls_do_bol As Boolean = False
    ''' <summary>
    ''' 报警灯(红灯)闪耀
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub controls_do()
        If controls_do_bol = True Then
            Exit Sub
        End If
        Log("报警灯(红灯)闪耀 启动")
        Do
            controls_do_bol = True
            Try
                If S1 = False Then
                    Threading.Thread.Sleep(1000)
                    Continue Do
                End If

                If Alarm_detection = 0 And Alarm_detection = "" Then
                    Alarm_detection = 1000 '默认1000毫秒
                End If
                Threading.Thread.Sleep(Alarm_detection)
                'Dim State_num As New t_State_num

            Catch ex As Exception
            End Try
        Loop
        Log("报警灯(红灯)闪耀 停止")
        controls_do_bol = False
    End Sub


    ''' <summary>
    ''' 处理UI线程异常
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Shared Sub Application_ThreadException(sender As Object, e As System.Threading.ThreadExceptionEventArgs)
        Log("Application_ThreadException:" + e.Exception.Message)
        Log(e.Exception.StackTrace)
    End Sub

    ''' <summary>
    ''' 处理非UI线程异常
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Shared Sub CurrentDomain_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs)

        Log("CurrentDomain_UnhandledException")
        Log("IsTerminating : " + e.IsTerminating.ToString())
        Log(e.ExceptionObject.ToString())
        System.Threading.Thread.Sleep(100) '延时100毫秒
    End Sub


    ''' <summary>
    ''' ESD按钮点击事件
    ''' </summary>
    ''' <param name="esd_num"></param>
    ''' <param name="esd_state"></param>
    ''' <remarks></remarks>
    Public Sub ESD_click(ByVal esd_num As String, ByVal esd_state As String, ByVal model As ESD_STATE.Model)
        'MsgBox("ESD编号:" & esd_num & vbCrLf & "ESD状态:" & esd_state)

        Dim t_ESD_Maintain As New ESD_Maintain
        t_ESD_Maintain.ESD_NUM = esd_num
        t_ESD_Maintain.ESD_STATE = esd_state
        t_ESD_Maintain.t_position = model

        t_ESD_Maintain.Hide()
        t_ESD_Maintain.ShowDialog()

    End Sub
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        '将窗体设置为最大化
        If Me.WindowState = FormWindowState.Normal Then
            Me.WindowState = FormWindowState.Maximized
        End If

        TableLayoutPanel1.Width = Me.Width


    End Sub

    Private Sub 关闭ToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Application.Exit()
        'If MessageBox.Show("Process id : " & Process_pid & vbCrLf & vbCrLf _
        '          & "确认要退出系统", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) = Windows.Forms.DialogResult.Yes Then
        '    '结束当前线程
        Process.GetProcessById(Process_pid).Kill()
    End Sub

    Private Sub MainFrom_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Me.WindowState = FormWindowState.Minimized '最小化程序

        e.Cancel = True
        Me.Hide()

    End Sub

    Private Sub 打开ToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub NotifyIcon1_MouseDown(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDown
        'If e.Button = MouseButtons.Left Then
        '    Me.WindowState = FormWindowState.Maximized '最大化程序
        '    Me.Show()
        'Else
        '    'If MessageBox.Show("Process id : " & Process_pid & vbCrLf & vbCrLf _
        '    '          & "确认要退出系统", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) = Windows.Forms.DialogResult.Yes Then
        '    '    '结束当前线程
        '    '    Process.GetProcessById(Process_pid).Kill()
        '    'End If
        'End If
    End Sub



    ''' <summary>
    ''' 写入txt
    ''' </summary>
    ''' <param name="txtPath"></param>
    Private Sub WriteTXT(txtPath As String, de As Device)
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
    ''' 写入TXT文件
    ''' </summary>
    ''' <param name="txtPath"></param>
    ''' <param name="str"></param>
    Private Sub txtWrite(txtPath As String, str As String)
        Dim sw As New StreamWriter(txtPath, True) 'System.Text.Encoding.GetEncoding("GB2312") = 

        str += Date.Now()
        sw.WriteLine(str)
        sw.Close()

        'Dim strArr As String() = {str}
        'File.WriteAllLines(txtPath, strArr, System.Text.Encoding.GetEncoding("GB2312")) '写入到新文件中


    End Sub


    Public Class Device
        Public DeviceName As String
        Public address As String
        Public gross_product As String
        Public bad_num As String
        Public Powertimme As String
        Public Powervalue As String
    End Class


    ''' <summary>
    ''' 等待
    ''' </summary>
    ''' <param name="r"></param>
    Private Declare Sub Sleep Lib "kernel32" (ByVal r As Integer)


    Public IsUpdate As Boolean = True

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        'While IsUpdate



        Dim NumSave As String = Model_set.GetINI("Device", "num", "", Cpath)

        If IsUpdate Then



            If Convert.ToInt32(Val(Quantity_equipment)) = Convert.ToInt32(NumSave) AndAlso IsUpdate = False Then
            Else

                FlowLayoutPanel1.Controls.Clear()


                For i = 1 To Convert.ToInt32(Val(Quantity_equipment))

                    Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", Cpath)
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



                    Dim UD As New UserDeviceInfo With {
                                                        .Name = DeviceList(1) & DeviceList(0).ToString,
                                                        .DeviceName = DeviceList(1),
                                                        .DeviceAdrees = DeviceList(2),
                                                        .DeviceBedNum = "不 良 数：",
                                                        .DeviceTime = "保压时间：",
                                                        .DevicePower = "保压压力："
                                                        }
                    FlowLayoutPanel1.Controls.Add(UD)



                Next

                Model_set.WriteINI("Device", "Num", Quantity_equipment, Cpath)
                'Quantity_equipment = NumSave
                IsUpdate = False


            End If


        End If




    End Sub

    Private Sub 打开ToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles 打开ToolStripMenuItem.Click
        Me.WindowState = FormWindowState.Maximized '最大化程序
        Me.Show()
    End Sub

    Private Sub 关闭ToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles 关闭ToolStripMenuItem.Click
        Process.GetProcessById(Process_pid).Kill()
    End Sub

    Private Sub ToolStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ToolStrip1.ItemClicked

    End Sub

    Private Sub bn_mttr_Click(sender As Object, e As EventArgs) Handles bn_mttr.Click
        MTTR.Hide()

        MTTR.ShowDialog()


    End Sub
End Class

