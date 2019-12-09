Imports System.IO.Ports
Imports System.Threading
Imports ESD_Kanban.DBConfig
Imports ESD_Kanban.ESD_STATE
Imports ESD_Kanban.GetSYSTEMTIME
Imports System.Messaging


Public Class ESD

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
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ESD_DIC_Left.Clear()
        ESD_DIC_right.Clear()

        'Button2.Enabled = False
        Synchronization_time() '将本地计算机与服务器时间进行同步
        Me.Text = "ESD在线状态看板 [ESD on line protection system for electrostatic alarm] " & getver


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
            FlowLayoutPanel1.Controls.Add(esd)
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
            FlowLayoutPanel2.Controls.Add(esd)
            esdcontrols_right.Add(esd)
        Next

        Timer1.Interval = 500
        Timer1.Enabled = True




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
        'ToolStripLabel1.Text = "Left ESD:" & ESD_DIC_Left.Count 'ESD_NUM_Left
        'ToolStripLabel15.Text = "right ESD:" & ESD_DIC_right.Count 'ESD_NUM_right

        'Button3.Text = t_Monitor
        'Button4.Text = t_Line_body

        'Dim y As Integer = ESD_DIC.Count

        'ToolStripLabel7.Text = Scanning_interval
        'ToolStripLabel9.Text = Scanning_timeout

        Coltrols_Call_police_Left()

        Button6_Click(Nothing, Nothing)


        Statistical_thread_start()

        'Panel8.Size = New Point(Panel5.Size.Width, 200)
        Panel8.AutoSize = True

        Dim Glabel1 As New Label With {
            .AutoSize = True,
            .Location = New Point(0, 0),
            .Text = "线体左侧→",
            .Name = "XA1",
            .Font = New Font("宋体", 10.0F, FontStyle.Regular),
            .BackColor = Color.Orange}
        Panel8.Controls.Add(Glabel1)

        For i = 1 To 30
            Dim GButton As New Button With {
                .BackColor = Color.Gainsboro,
                .Name = "A" & i,
                .Text = i.ToString("A-000"),
                .AutoSize = False,
                .Size = New Point(23, 95),
                .Font = New Font("宋体", 10.0F, FontStyle.Regular)
            }
            GButton.Location = New Point(23 * (i - 1), 15)
            Panel8.Controls.Add(GButton)
        Next


        For i = 1 To 30
            Dim GButton As New Button With {
                .BackColor = Color.Gainsboro,
                .Name = "B" & i,
                .Text = i.ToString("B-000"),
                .AutoSize = False,
                .Size = New Point(23, 95),
                .Font = New Font("宋体", 10.0F, FontStyle.Regular)
            }
            GButton.Location = New Point(23 * （i - 1), 110)
            Panel8.Controls.Add(GButton)
        Next

        Dim Glabel2 As New Label With {
            .AutoSize = True,
            .Location = New Point(0, 207),
            .Text = "线体右侧→",
            .Name = "XA2",
            .Font = New Font("宋体", 10.0F, FontStyle.Regular),
            .BackColor = Color.Orange}
        Panel8.Controls.Add(Glabel2)


        Label13.Text = "ESD报警器工位排布示意图"

        Label13.Location = New Point((Panel5.Width / 2) - (Label13.Width / 2), 5) '左右居中

        Panel8.Location = New Point((Panel5.Width / 2) - (Panel8.Width / 2), (Panel5.Height / 2) - (Panel8.Height / 2)) '左右居中

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
        'Label1.Location = New Point((Panel1.Width / 2) - (Label1.Width / 2) - 60, (Panel1.Height / 2) - (Label1.Height / 2)) '左右居中
        FlowLayoutPanel1.AutoScroll = True
    End Sub

    ''' <summary>
    ''' 左边采集器计数
    ''' </summary>
    ''' <remarks></remarks>
    Dim esd_int_Left As Integer = 0
    ''' <summary>
    ''' 右边采集器计数
    ''' </summary>
    ''' <remarks></remarks>
    Dim esd_int_right As Integer = 0
    Dim t_num As Integer = 0

    ''' <summary>
    ''' 自动更新倒计时
    ''' </summary>
    Dim Update_time As Long
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        'ToolStripLabel11.Text = Format(Now, "yyyy-MM-dd HH:mm:ss")

        'If CheckBox1.Checked = False Then
        '    Exit Sub
        'End If
        t_num += 1
        If t_num >= 5 Then '调节控件滚动速度
            t_num = 0

            esd_int_Left += 1
            If esd_int_Left = Val(ESD_NUM_Left) Then
                esd_int_Left = 1
            End If
            FlowLayoutPanel1.ScrollControlIntoView(esdcontrols_Left(esd_int_Left))

            esd_int_right += 1
            If esd_int_right = Val(ESD_NUM_right) Then
                esd_int_right = 1
            End If
            FlowLayoutPanel2.ScrollControlIntoView(esdcontrols_right(esd_int_right))

        End If

        '更新倒计时
        Update_time += 1
        'ToolStripButton1.Text = String.Format("程序更新[{0}s]", 1800 - Update_time)
        If Update_time >= 1800 Then
            Update_time = 0
            tpUpdate()
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)

        from_Setting.Hide()
        'from_Setting.Show()
        from_Setting.ShowDialog()
        'ToolStripLabel1.Text = "ESD数量:" & ESD_NUM_Left

        'ToolStripLabel7.Text = Scanning_interval
        'ToolStripLabel9.Text = Scanning_timeout
    End Sub

    ''' <summary>
    ''' 数采控制器串口
    ''' </summary>
    ''' <remarks></remarks>
    Dim S1 As Boolean = False
    Private Sub Button9_Click(sender As Object, e As EventArgs)
        '/*串口事件注册
        '/*************************************************************************************************************/
        Try
            Select Case S1
                Case False

                    'Relay_control_source_Left = ComboBox1.Text '串口名称
                    'Relay_control_source_Baud_Left = ComboBox2.Text '串口波特率

                    'SC_Relay_control_Left.setSerialPort(
                    'ComboBox1.Text, ComboBox2.Text, 8, IO.Ports.StopBits.One)
                    'Control.CheckForIllegalCrossThreadCalls = False '线程之间进行通讯
                    'AddHandler SC_Relay_control_Left.DataReceived,
                    'New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_Left)
                    'If SC_Relay_control_Left.openPort() = False Then
                    '    Exit Sub
                    'End If
                    'S1 = True
                    'Button9.Text = "停止(&Q)"
                    'ComboBox1.Enabled = False
                    'ComboBox2.Enabled = False
                    'Button9.BackColor = Color.DarkSeaGreen '绿色

                    thread_run_Left()

                Case True
                    ' Relay_control_source_Left = ComboBox1.Text '串口名称
                    ' Relay_control_source_Baud_Left = ComboBox2.Text '串口波特率

                    ' SC_Relay_control_Left.closePort() '关闭串口
                    ' '卸载事件关联
                    ' RemoveHandler SC_Relay_control_Left.DataReceived,
                    'New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_Left)

                    ' S1 = False
                    ' Button9.Text = "启动(&S)"
                    ' ComboBox1.Enabled = True
                    ' ComboBox2.Enabled = True
                    ' Button9.BackColor = Color.Gainsboro '浅灰色

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

    ''' <summary>
    ''' ESD实时状态计数
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure t_State_num
        ''' <summary>
        ''' 错误
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_Invalid As Integer

        ''' <summary>
        ''' 正常
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_normal As Integer

        ''' <summary>
        ''' 离线
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_Off_line As Integer

        ''' <summary>
        ''' 初始化
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_Initialization As Integer
    End Structure




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
        Dim hexs() As String = BitConverter.ToString(bits).Replace("-", " ").Trim.Split

        Dim hex As New System.Text.StringBuilder
        For Each s In hexs
            hex.Append(s.ToString & " ")
        Next

        Me.Invoke(New Action(Sub()
                                 If bits.Length > 6 Then
                                     Log("A串口接收指令:" & hex.ToString & ",长度:" & bits.Length)
                                 End If
                                 'ToolStripTextBox1.Text = hex.ToString




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

        Me.Invoke(New Action(Sub()
                                 If bits.Length > 7 Then
                                     Log("B串口接收指令:" & hex.ToString & ",长度:" & bits.Length)
                                 End If
                                 'ToolStripTextBox1.Text = hex.ToString
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

#Region "线体左边采集器串口状态采集"

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



    ''' <summary>
    '''================================================================== 设备可视化发送===========================================
    ''' </summary>
    Dim send As String
    Public Sub Thread_voice(port As SerialPort)

        If port.IsOpen = False Then
            port.Open()
        End If
        port.Handshake = Handshake.None
        port.StopBits = StopBits.One
        port.Parity = Parity.None
        port.WriteTimeout = 1000
        port.ReadTimeout = 2000
        port.DataBits = 8

        send = "F1 00 00"

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
        Dim bufW As Byte() = CRCClass.HexString2Bytes(send)

        Try

            port.Write(bufW, 0, bufW.Length)
        Catch ex As Exception

            MessageBox.Show(ex.ToString())
        End Try
    End Sub



    ''' <summary>
    ''' [采集器A]线程循环发送串口指令
    ''' </summary>
    ''' <remarks></remarks>
    ''' 


    Dim po As String = 0
    Private Sub thread_senddate_Left()
        Log("A线程循环发送串口指令 启动")
        Do



            '==============设备可视化发送指令==================

            'Thread_voice（SC_Relay_control_Left.Serialport()）

            '=================================================


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

                            'ToolStripProgressBar1.Maximum = ESD_DIC_Left.Count
                            'ToolStripProgressBar1.Minimum = 0
                            'ToolStripProgressBar1.Value = i

                            Threading.Thread.Sleep(Val(Scanning_interval)) '发送数据进行延时

                            If ESD_DIC_Left(i)(0).ESD_isEnable.ToUpper = "FALSE" Then
                                Continue For  'ESD被禁用,跳到下一条执行
                            End If

                            Me.Invoke(New Action(Sub()
                                                     'ToolStripTextBox2.Text = ESD_DIC_Left(i)(0).ESD_query
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



                            SC_Relay_control_Left.SendData(send_cmd, 0, send_cmd.Count) '循环查询ESD的状态










                            Me.Invoke(New Action(Sub()
                                                     'ToolStripLabel4.Text = "左" & ESD_DIC_Left(i)(0).ESD_NUM
                                                 End Sub))
                        Next

                        '一个扫描周期以后才开始更新控件状态
                        For Each get_Controls In FlowLayoutPanel1.Controls
                            Dim sen As New ESD_STATE
                            sen = get_Controls
                            '获取到对应的控件对象
                            If sen.IsEnable.ToUpper = "FALSE" Then
                                sen.ESDSTATE(ESD_STATE.State.禁用)
                                Dim t_num As Int16 = Int(sen.t_number.Replace("A-", "").Trim)
                                Dim control As Control = Controls.Find("A" & t_num, True)(0)
                                control.BackColor = Color.Silver 'Yellow ESD 禁用改为灰色  ,不要黄色
                                control.ForeColor = Color.Black
                                Continue For '判断为禁用状态就跳过,执行下一次循环
                            End If
                            Dim Rice1 As Long = DateDiff(DateInterval.Minute, Convert.ToDateTime(sen.t_getdate), Convert.ToDateTime(Now))
                            If Rice1 >= Val(Scanning_timeout) Then '超时时间不能太短,会造成状态不停在 [灰色与绿色] 之间闪耀
                                '超过设置的分钟数没有更新状态代表设备可能连接失败了
                                '状态设置为初始化
                                sen.ESDSTATE(ESD_STATE.State.初始化)

                                '搜寻排布控件,并修改颜色
                                Dim t_num As Int16 = Int(sen.t_number.Replace("A-", "").Trim)
                                Dim control As Control = Controls.Find("A" & t_num, True)(0)
                                control.BackColor = Color.Silver
                                control.ForeColor = Color.Black
                            End If
                        Next

                    End If
                Catch ex As Exception
                    Log("A线程循环发送串口指令 " & ex.Message)
                End Try
            End If
        Loop
        Log("A线程循环发送串口指令 停止")
    End Sub

#End Region

    ''' <summary>
    ''' 是否退出程序
    ''' </summary>
    Dim t_Cancel_bol As Boolean = True
    ''' <summary>
    ''' 关闭系统
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Me.WindowState = FormWindowState.Minimized '最小化程序
        e.Cancel = t_Cancel_bol '不退出程序
    End Sub


#Region "报警红灯闪耀逻辑"

    Dim S2 As Boolean = False
    ''' <summary>
    ''' 报警灯控制
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button2_Click(sender As Object, e As EventArgs)
        '/*串口事件注册
        '/*************************************************************************************************************/
        Try
            Select Case S2
                Case False

                    'Relay_control_Cell_police = ComboBox4.Text '串口名称
                    'Relay_control_Cell_police_Baud = ComboBox3.Text '串口波特率

                    'SC_Cell_police_control.setSerialPort(
                    'ComboBox4.Text, ComboBox3.Text, 8, IO.Ports.StopBits.One)
                    'Control.CheckForIllegalCrossThreadCalls = False '线程之间进行通讯
                    'AddHandler SC_Cell_police_control.DataReceived,
                    'New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_call_police)
                    'If SC_Cell_police_control.openPort() = False Then
                    '    Exit Sub
                    'End If
                    'S2 = True
                    'Button2.Text = "停止(&Q)"
                    'ComboBox4.Enabled = False
                    'ComboBox3.Enabled = False
                    'Button2.BackColor = Color.DarkSeaGreen '绿色

                Case True
                    ' Relay_control_Cell_police = ComboBox4.Text '串口名称
                    ' Relay_control_Cell_police_Baud = ComboBox3.Text '串口波特率

                    ' SC_Cell_police_control.closePort() '关闭串口
                    ' '卸载事件关联 
                    ' RemoveHandler SC_Cell_police_control.DataReceived,
                    'New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_call_police)

                    ' S2 = False
                    ' Button2.Text = "启动(&S)"
                    ' ComboBox4.Enabled = True
                    ' ComboBox3.Enabled = True
                    ' Button2.BackColor = Color.Gainsboro '浅灰色

            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message & ex.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
        End Try

        '关闭串口连接
        'SC.closePort() 
        '/*************************************************************************************************************/

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
                Dim State_num As New t_State_num

                '左边线体
                For Each get_Controls In FlowLayoutPanel1.Controls
                    Dim sen As New ESD_STATE
                    sen = get_Controls
                    '获取到对应的控件对象
                    Select Case sen.t_state
                        Case "ESD防护正常"
                            State_num.ESD_normal += 1
                        Case "ESD防护失效"
                            State_num.ESD_Invalid += 1
                        Case "ESD报警器未开"
                            State_num.ESD_Off_line += 1
                        Case "初始化"
                            State_num.ESD_Initialization += 1
                    End Select
                Next

                '右边线体
                For Each get_Controls In FlowLayoutPanel2.Controls
                    Dim sen As New ESD_STATE
                    sen = get_Controls
                    '获取到对应的控件对象
                    Select Case sen.t_state
                        Case "ESD防护正常"
                            State_num.ESD_normal += 1
                        Case "ESD防护失效"
                            State_num.ESD_Invalid += 1
                        Case "ESD报警器未开"
                            State_num.ESD_Off_line += 1
                        Case "初始化"
                            State_num.ESD_Initialization += 1
                    End Select
                Next

                If State_num.ESD_Invalid > 0 Then
                    '11月30  应老板要求修改为直接闪耀
                    '出现异常 <ESD0>
                    'Scanning_times += 1 '红灯计数延时自增
                    'If Scanning_times > 5 Then '5次扫描还是NG的话就亮线头红灯
                    'Scanning_times = 6 '为避免无止境的自增导致溢出问题,给计数器赋值6
                    Dim hex As String = "<ESD0>"
                    mq.Send(hex)

                    'SC_Cell_police_control.SendData(hex)

                    'Else
                    'Threading.Thread.Sleep(1000) '小于5秒就将过程延迟1秒
                    'End If
                Else
                    '正常 <ESD1>
                    Scanning_times = 0 '红灯计数延迟归零
                    Dim hex As String = "<ESD1>"
                    mq.Send(hex)

                    Threading.Thread.Sleep(5000) '正常的话就延时5秒,再循环下一次
                    'SC_Cell_police_control.SendData(hex)
                End If

                '反馈异常数量到UI界面
                Dim ngnum As New t_Ng_num(AddressOf Ng_num)
                Me.Invoke(ngnum, State_num.ESD_Invalid)

            Catch ex As Exception
            End Try
        Loop
        Log("报警灯(红灯)闪耀 停止")
        controls_do_bol = False
    End Sub

    ''' <summary>
    ''' 反馈异常数据到UI的委托实现
    ''' </summary>
    ''' <param name="val"></param>
    ''' <remarks></remarks>
    Private Delegate Sub t_Ng_num(ByVal val As Integer)

    ''' <summary>
    ''' 异常数量
    ''' </summary>
    ''' <param name="val"></param>
    ''' <remarks></remarks>
    Private Sub Ng_num(ByVal val As Integer)
        'ToolStripLabel14.Text = "异常数量:" & val
    End Sub


#End Region

    Private Sub Button3_Click_1(sender As Object, e As EventArgs)
        t_state = t_Pattern_selection.班长选择

        from_Squad_leader_selection.Hide()
        from_Squad_leader_selection.ShowDialog()

        'Button3.Text = t_Monitor
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs)
        t_state = t_Pattern_selection.线体选择

        from_Squad_leader_selection.Hide()
        from_Squad_leader_selection.ShowDialog()

        'Button4.Text = t_Line_body
    End Sub

#Region "线体右边采集器逻辑"

    Dim S3 As Boolean = False
    Private Sub Button5_Click(sender As Object, e As EventArgs)
        '/*串口事件注册
        '/*************************************************************************************************************/
        Try
            Select Case S3
                Case False

                    'Relay_control_source_right = ComboBox6.Text '串口名称
                    'Relay_control_source_Baud_right = ComboBox5.Text '串口波特率

                    'SC_Relay_control_right.setSerialPort(
                    'ComboBox6.Text, ComboBox5.Text, 8, IO.Ports.StopBits.One)
                    'Control.CheckForIllegalCrossThreadCalls = False '线程之间进行通讯
                    'AddHandler SC_Relay_control_right.DataReceived,
                    'New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_right)
                    'If SC_Relay_control_right.openPort() = False Then
                    '    Exit Sub
                    'End If
                    'S3 = True
                    'Button5.Text = "停止(&Q)"
                    'ComboBox6.Enabled = False
                    'ComboBox5.Enabled = False
                    'Button5.BackColor = Color.DarkSeaGreen '绿色

                    thread_run_right()

                Case True
                    'Relay_control_source_right = ComboBox6.Text '串口名称
                    'Relay_control_source_Baud_right = ComboBox5.Text '串口波特率

                    SC_Relay_control_right.closePort() '关闭串口
                    '卸载事件关联 
                    RemoveHandler SC_Relay_control_right.DataReceived,
                   New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived_right)

                    S3 = False
                    'Button5.Text = "启动(&S)"
                    'ComboBox6.Enabled = True
                    'ComboBox5.Enabled = True
                    'Button5.BackColor = Color.Gainsboro '浅灰色

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

#End Region

#Region "线体右边采集器串口状态采集"

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

                            'ToolStripProgressBar2.Maximum = ESD_DIC_right.Count
                            'ToolStripProgressBar2.Minimum = 0
                            'ToolStripProgressBar2.Value = i

                            Threading.Thread.Sleep(Val(Scanning_interval))

                            If ESD_DIC_right(i)(0).ESD_isEnable.ToUpper = "FALSE" Then
                                'ESD被禁用,跳到下一条执行

                                Continue For
                            End If

                            Me.Invoke(New Action(Sub()
                                                     'ToolStripTextBox2.Text = ESD_DIC_right(i)(0).ESD_query
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
                                                     'ToolStripLabel4.Text = "右" & ESD_DIC_right(i)(0).ESD_NUM
                                                 End Sub))

                        Next

                        For Each get_Controls In FlowLayoutPanel2.Controls
                            Dim sen As New ESD_STATE
                            sen = get_Controls
                            '获取到对应的控件对象
                            If sen.IsEnable.ToUpper = "FALSE" Then
                                sen.ESDSTATE(ESD_STATE.State.禁用)
                                Dim t_num As Int16 = Int(sen.t_number.Replace("B-", "").Trim)
                                Dim control As Control = Controls.Find("B" & t_num, True)(0)
                                control.BackColor = Color.Silver 'Yellow ESD 禁用改为灰色  ,不要黄色
                                control.ForeColor = Color.Black
                                Continue For
                            End If
                            Dim Rice1 As Long = DateDiff(DateInterval.Minute, Convert.ToDateTime(sen.t_getdate), Convert.ToDateTime(Now))
                            If Rice1 >= Val(Scanning_timeout) Then
                                '超过设置的分钟数没有更新状态代表设备可能连接失败了
                                '状态设置为初始化
                                sen.ESDSTATE(ESD_STATE.State.初始化)

                                '搜寻排布控件,并修改颜色
                                Dim t_num As Int16 = Int(sen.t_number.Replace("B-", "").Trim)
                                Dim control As Control = Controls.Find("B" & t_num, True)(0)
                                control.BackColor = Color.Silver
                                control.ForeColor = Color.Black

                            End If
                        Next

                    End If
                Catch ex As Exception
                    Log("B线程循环发送串口指令" & ex.Message)
                End Try
            End If
        Loop
        Log("[采集器B]线程循环发送串口指令 停止")
    End Sub

#End Region

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
    Private Sub Button6_Click(sender As Object, e As EventArgs)
        Select Case thread_bol
            'Case False
            '    thread_bol = True
            '    Button6.BackColor = Color.DarkSeaGreen '绿色
            '    Button6.Text = "停止采集线程"
            'Case True
            '    thread_bol = False
            '    Button6.BackColor = Color.Gainsboro '浅灰色
            '    Button6.Text = "启动采集线程"
        End Select
    End Sub



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
                CreateGraph_GradientByZBars(ZedGraphControl1, Now.Month & "月" & Now.Day & "日ESD状态统计", GetDic, 1, 2, 13)
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

#End Region

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs)


        'Dim control As Control = Controls.Find("A1", True)(0)
        'Dim o As Object = control.[GetType]().GetProperty("Text").GetValue(control, Nothing) '获取获取属性
        'Dim ev As System.Reflection.EventInfo = control.[GetType]().GetEvent("Click") '获取事件
        'Control.Text = "hello world!"

        '不带参数的多线程传值
        'Dim th As New Threading.Thread(Sub()
        '                                   '委托的简单写法
        '                                   Me.Invoke(New Action(Sub()
        '                                                            Dim msg As String = "这是委托"
        '                                                            MsgBox(msg)
        '                                                        End Sub))
        '                               End Sub) With {
        '                      .Name = "简化线程写法",
        '                      .IsBackground = True,
        '                      .Priority = Threading.ThreadPriority.Highest}
        'th.Start()


        '带参数的线程
        'Dim Thd As Threading.Thread = New Threading.Thread(Function() TestMethod(Obj))
        'Thd.SetApartmentState(Threading.ApartmentState.MTA)

    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs)
        tpUpdate()
    End Sub

    Dim Upbol As Boolean = False
    ''' <summary>
    ''' 程序升级
    ''' </summary>
    Private Sub tpUpdate()
        If Upbol = False Then
            Upbol = True
            Try
                If IO.File.Exists(Environ("temp") & "\Update_Detection.exe") = True Then
                    IO.File.Delete(Environ("temp") & "\Update_Detection.exe") '删除
                End If
                Threading.Thread.Sleep(300)
                If IO.File.Exists(Environ("temp") & "\Update_Detection.exe") = False Then
                    Release_file(My.Resources.Update_Detection, Environ("temp"), "Update_Detection.exe")
                End If

                Dim p As New System.Diagnostics.Process()
                p.StartInfo.UseShellExecute = True
                p.StartInfo.FileName = Environ("temp") & "\Update_Detection.exe"
                p.StartInfo.Arguments = "/r ftp://10.114.113.4/2.Yuan.suo/1.update/ESDKanBan/ /u fe /p yuanshuai /f " & IO.Path.GetDirectoryName(Application.ExecutablePath) & " /v " & getver & " /s " & IO.Path.GetFileName(Application.ExecutablePath)
                p.Start()
                Upbol = False
            Catch ex As Exception
                Upbol = False
            End Try
        End If

    End Sub

    ''' <summary>
    ''' 将资源文件释放到本地磁盘中
    ''' </summary>
    ''' <param name="My_ResouName">资源文件名称</param>
    ''' <param name="path">释放的地址</param>
    ''' <param name="FileName">文件的名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Release_file(ByVal My_ResouName() As Byte, ByVal path As String, ByVal FileName As String) As Integer
        '将资源文件释放到本地磁盘
        On Error Resume Next
        If Not IO.File.Exists(path & FileName) = True Then '对已经释放过的不再重复写入
            Dim bufbytes() As Byte '声明一个2进制数组
            Dim fs As System.IO.Stream
            bufbytes = My_ResouName '将资源赋值到数组中
            fs = System.IO.File.Create(path & "\" & FileName) '释放path
            fs.Write(bufbytes, 0, bufbytes.Length) '开始写入数据到path
            fs.Close()
        End If
    End Function

    ''' <summary>
    ''' 服务器地址
    ''' </summary>
    Private Sub Server_address()
        Dim Server_Address_Selection As New Server_Address_Selection
        Dim IP As String = Server_Address_Selection.Scanning()
        If IP IsNot Nothing Then
            WriteINI("Connect", "server", IP & ",1433", My.Application.Info.DirectoryPath & "\Parameter.ini")
        End If

    End Sub

    'Private Sub 关闭ToolStripMenuItem_Click(sender As Object, e As EventArgs)
    '    Me.WindowState = FormWindowState.Maximized

    '    If MessageBox.Show("Process id : " & Process_pid & vbCrLf & vbCrLf _
    '                        & "确认要退出系统", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) = Windows.Forms.DialogResult.Yes Then
    '        thread_bol = False
    '        t_Cancel_bol = False
    '        Process.GetProcessById(Process_pid).Kill()
    '    Else
    '        t_Cancel_bol = True
    '    End If
    'End Sub

    'Private Sub 打开ToolStripMenuItem_Click(sender As Object, e As EventArgs)
    '    Me.WindowState = FormWindowState.Maximized
    'End Sub

    Private Sub Form1_MaximumSizeChanged(sender As Object, e As EventArgs) Handles Me.MaximumSizeChanged

    End Sub

    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint

    End Sub
End Class


