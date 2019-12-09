Imports System.Drawing.Drawing2D

Public Class ESD_STATE
    Implements IButtonControl

    ''' <summary>
    ''' ESD状态
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property t_state As String

    Private _state As String
    ''' <summary>
    ''' ESD编号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property t_number As String
        Get
            Return _state
        End Get
        Set(value As String)
            _state = value
            Label2.Text = value
        End Set
    End Property

    ''' <summary>
    ''' 刷新时间
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property t_getdate As DateTime

    ''' <summary>
    ''' 状态枚举
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum State
        正常 = 0
        错误 = 1
        离线 = 3
        初始化 = 4
        禁用 = 5
    End Enum

    ''' <summary>
    ''' 设置
    ''' </summary>
    ''' <param name="state"></param>
    ''' <remarks></remarks>
    Public Sub ESDSTATE(ByVal state As State)
        Select Case state
            Case ESD_STATE.State.错误
                '红色：未开报警器、开了报警器但没有戴好/没有戴手环、
                PictureBox1.Image = My.Resources.灯泡_红色_
                t_state = "ESD防护失效"
            Case ESD_STATE.State.正常
                '绿色：报警器OK & 员工佩戴好手环 & 电路线路OK
                PictureBox1.Image = My.Resources.灯泡_绿色_
                t_state = "ESD防护正常"
            Case ESD_STATE.State.离线
                '红色：未开报警器、开了报警器但没有戴好/没有戴手环、
                PictureBox1.Image = My.Resources.灯泡_红色_
                t_state = "ESD报警器未开"
            Case ESD_STATE.State.初始化
                '灰色：班长维护取消ESD监控 & 电路线路NG
                '线路故障和维护个别工位暂停监控还是要有所区分，比如用闪烁灰色代表线路故障；
                '这里不采取灰色闪耀,如果出现线路异常默认就是灰色.
                PictureBox1.Image = My.Resources.灯泡_灰色_
                t_state = "不使用"
            Case ESD_STATE.State.禁用
                PictureBox1.Image = My.Resources.灯泡_灰色_
                t_state = "禁用"
        End Select
        Label4.Text = t_state

        If t_state = "禁用" Then
            Panel2.BackColor = Color.Yellow
        Else
            Panel2.BackColor = Color.White
        End If

    End Sub

    Dim C1 As String = ""
    ''' <summary>
    ''' 只读,ESD是否被启用
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsEnable As String
        Get
            Select Case t_position
                Case Choice.Left
                    C1 = "Left"
                Case Choice.right
                    C1 = "right"
            End Select
            Return GetINI("ESDEnable_" & C1, "ESD" & t_number, "", Configurationpath)
        End Get
    End Property


    Public Property DialogResult As DialogResult Implements IButtonControl.DialogResult

    Public Sub NotifyDefault(value As Boolean) Implements IButtonControl.NotifyDefault
    End Sub

    ''' <summary>
    ''' 点击事件
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PerformClick() Implements IButtonControl.PerformClick

        RaiseEvent ESD_Event(t_number, t_state, t_position)

    End Sub


    ''' <summary>
    ''' 模式选择
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum Model
        ''' <summary>
        ''' 左
        ''' </summary>
        ''' <remarks></remarks>
        Left = 0
        ''' <summary>
        ''' 右
        ''' </summary>
        ''' <remarks></remarks>
        right = 1
    End Enum

    ''' <summary>
    ''' 左边采集器还是右边采集器
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property t_position As Model


    ''' <summary>
    ''' ESD点击事件
    ''' </summary>
    ''' <param name="esd_num">ESD编号</param>
    ''' <param name="esd_state">ESD状态</param>
    ''' <remarks></remarks>
    Public Delegate Sub ESD_click(ByVal esd_num As String, ByVal esd_state As String, ByVal Model As Model)


    ''' <summary>
    ''' esd点击事件
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Event ESD_Event As ESD_click

    Dim state_num_int As Integer = 0

    ''' <summary>
    ''' 按钮弹起事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ESD_STATE_MouseUp(sender As Object, e As MouseEventArgs) Handles _
        PictureBox1.MouseUp, _
        Panel1.MouseUp, _
        Panel2.MouseUp, _
        Label1.MouseUp, _
        Label2.MouseUp, _
        Label3.MouseUp, _
        Label4.MouseUp, _
        TableLayoutPanel1.MouseUp
        Me.BackColor = Color.White
        PerformClick()
    End Sub

    ''' <summary>
    ''' 单击控件事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles _
        PictureBox1.MouseDown, _
        Panel1.MouseDown, _
        Panel2.MouseDown, _
        Label1.MouseDown, _
        Label2.MouseDown, _
        Label3.MouseDown, _
        Label4.MouseDown, _
        TableLayoutPanel1.MouseDown
        Me.BackColor = Color.PowderBlue
    End Sub

    Private Sub ESD_STATE_MouseEnter(sender As Object, e As EventArgs) Handles _
        PictureBox1.MouseEnter, _
        Panel1.MouseEnter, _
        Panel2.MouseEnter, _
        Label1.MouseEnter, _
        Label2.MouseEnter, _
        Label3.MouseEnter, _
        Label4.MouseEnter, _
        TableLayoutPanel1.MouseEnter

        Panel1.BackColor = Color.CornflowerBlue



        'Using g As Graphics = Graphics.FromHwnd(TableLayoutPanel1.Handle)
        '    '数据刷新后边框变色
        '    g.SmoothingMode = SmoothingMode.AntiAlias
        '    g.DrawRectangle(Pens.Red, 0, 0, Me.Width - 1, Me.Height - 1)
        'End Using

    End Sub

    Private Sub ESD_STATE_MouseLeave(sender As Object, e As EventArgs) Handles _
        PictureBox1.MouseLeave, _
        Panel1.MouseLeave, _
        Panel2.MouseLeave, _
        Label1.MouseLeave, _
        Label2.MouseLeave, _
        Label3.MouseLeave, _
        Label4.MouseLeave, _
         TableLayoutPanel1.MouseLeave

        Panel1.BackColor = Color.White

        'Using g As Graphics = Graphics.FromHwnd(TableLayoutPanel1.Handle)
        '    '数据刷新后边框变色
        '    g.SmoothingMode = SmoothingMode.AntiAlias
        '    g.DrawRectangle(Pens.Gainsboro, 0, 0, Me.Width + 1, Me.Height + 1)
        'End Using

    End Sub


End Class
