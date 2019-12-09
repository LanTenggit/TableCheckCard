Public Class ESD_Agreement

    Private Sub ESD_Agreement_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        TextBox1.Top = 0
        TextBox1.Left = 45
        TextBox1.Width = Panel2.Width - 45
        TextBox1.Height = Panel2.Height

        TextBox2.Top = 0
        TextBox2.Left = 45
        TextBox2.Width = Panel3.Width - 45
        TextBox2.Height = Panel3.Height

        TextBox3.Top = 0
        TextBox3.Left = 45
        TextBox3.Width = Panel4.Width - 45
        TextBox3.Height = Panel4.Height

        TextBox4.Top = 0
        TextBox4.Left = 45
        TextBox4.Width = Panel5.Width - 45
        TextBox4.Height = Panel5.Height

        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        TextBox4.Enabled = False

        Dim t_ESDConfig As New ESDConfig


        t_ESDConfig = ESD_NUM(get_ESD_NUM, ESD_Choice)

        TextBox1.Text = t_ESDConfig.ESD_query
        TextBox2.Text = t_ESDConfig.ESD_Off_line
        TextBox3.Text = t_ESDConfig.ESD_Invalid
        TextBox4.Text = t_ESDConfig.ESD_normal

    End Sub




    Dim Edit_bol As Boolean = False
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Select Case Edit_bol
            Case False
                Edit_bol = True
                TextBox1.Enabled = True
                TextBox2.Enabled = True
                TextBox3.Enabled = True
                TextBox4.Enabled = True
                Button1.Text = "保存"
            Case True
                Edit_bol = False
                TextBox1.Enabled = False
                TextBox2.Enabled = False
                TextBox3.Enabled = False
                TextBox4.Enabled = False
                Button1.Text = "编辑"

                Dim esdconfig As New ESDConfig
                esdconfig.ESD_NUM = get_ESD_NUM
                esdconfig.ESD_query = TextBox1.Text.Trim
                esdconfig.ESD_Off_line = TextBox2.Text.Trim
                esdconfig.ESD_Invalid = TextBox3.Text.Trim
                esdconfig.ESD_normal = TextBox4.Text.Trim
                esdconfig.ESD_Choice = Choice.Left

                ESD_NUM(esdconfig.ESD_NUM, ESD_Choice) = esdconfig

        End Select
    End Sub

    Private _ESD_NUM As String = String.Empty
    ''' <summary>
    ''' ESD编号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property get_ESD_NUM
        Get
            Return _ESD_NUM
        End Get
        Set(value)
            _ESD_NUM = value
            Label2.Text = value
        End Set
    End Property

    ''' <summary>
    ''' 控件的读取模式
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ESD_Choice As model

    ''' <summary>
    ''' 模式选择
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum model
        Left = 0
        right = 1
    End Enum

End Class
