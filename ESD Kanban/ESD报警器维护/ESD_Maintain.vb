Public Class ESD_Maintain

    ''' <summary>
    ''' ESD编号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ESD_NUM As String

    ''' <summary>
    ''' ESD状态
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ESD_STATE As String

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

    Private Sub ESD_Maintain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "ESD报警器维护"

        Dim t_pos As String = ""
        Select Case t_position
            Case Model.Left
                t_pos = "线体左边"
            Case Model.right
                t_pos = "线体右边"
        End Select

        'Label1.Text =
        '    "ESD编号: " & ESD_NUM & vbCrLf &
        '    "ESD状态: " & ESD_STATE & vbCrLf &
        '    "位置: " & t_pos

        TextBox1.Text = ESD_NUM
        TextBox2.Text = ESD_STATE
        TextBox3.Text = t_pos


        Dim t_p As String = ESD_Enable(ESD_NUM, t_position)

        Select Case t_p.ToUpper
            Case "False".ToUpper
                RadioButton2.Checked = True
            Case "True".ToUpper
                RadioButton1.Checked = True
        End Select

        RadioButton1.Location = New Point((Panel3.Width / 2) - (RadioButton1.Width / 2), (Panel3.Height / 2) - (RadioButton1.Height / 2))

        RadioButton2.Location = New Point((Panel4.Width / 2) - (RadioButton2.Width / 2), (Panel4.Height / 2) - (RadioButton2.Height / 2))

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim isopen As Boolean = False
        If RadioButton1.Checked = True Then
            isopen = True
        End If
        If RadioButton2.Checked = True Then
            isopen = False
        End If
        ESD_Enable(ESD_NUM, t_position) = isopen
        'MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
        Close()
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.Click, PictureBox1.Click
        Panel3.BackColor = Color.Yellow
        Panel4.BackColor = Color.White
        RadioButton1.Checked = True
        RadioButton2.Checked = False
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.Click, PictureBox2.Click
        Panel3.BackColor = Color.White
        Panel4.BackColor = Color.Yellow
        RadioButton2.Checked = True
        RadioButton1.Checked = False
    End Sub
End Class