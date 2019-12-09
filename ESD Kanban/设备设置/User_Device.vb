Imports System.IO.Ports
Public Class User_Device
    Public Property DNumber() As String
        Get
            Return m_DNumber
        End Get
        Set
            m_DNumber = Value
        End Set
    End Property
    Private m_DNumber As String


    Public Property Adress() As String
        Get
            Return m_Adress
        End Get
        Set
            m_Adress = Value
        End Set
    End Property
    Private m_Adress As String

    Public Property DeviceName() As String
        Get
            Return m_DeviceName
        End Get
        Set
            m_DeviceName = Value
        End Set
    End Property
    Private m_DeviceName As String

    Public Property COMName() As String
        Get
            Return m_COMName
        End Get
        Set
            m_COMName = Value
        End Set
    End Property
    Private m_COMName As String

    Dim Cpath As String = My.Application.Info.DirectoryPath & "\" & "Parameter.ini"

    Private Sub User_Device_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.TextBox1.Enabled = False
        Me.ComboBox2.Enabled = False
        Me.ComboBox1.Enabled = False



        If ComboBox1.Items.Count = 0 Then
            Dim D_name As String() = {"扬声器", "TTK-1锁附机", "TTK-2锁附机", "TTK-3锁附机", "螺丝机", "贴膜机"}
            For index = 0 To D_name.Length - 1
                Me.ComboBox1.Items.Add(D_name(index))
            Next
            Me.ComboBox1.SelectedItem = D_name(0)

            'DeviceName = D_name(0)
        End If


        If ComboBox2.Items.Count = 0 Then
            Dim port As String() = {"A", "B"}
            For index = 0 To port.Length - 1
                Me.ComboBox2.Items.Add(port(index))
            Next
            Me.ComboBox2.SelectedItem = port(0)
            'COMName = port(0)

        End If


        'Dim num = Convert.ToInt32(Model_set.GetINI("Device", "Num", "", Cpath))
        'For i = 1 To num
        '    Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", Cpath)
        '    Dim DeviceList As String() = DeviceValue.Split(",")
        'Next




        TextBox1.Text = Adress
        ComboBox1.Text = DeviceName
        ComboBox2.Text = COMName
        Label5.Text = DNumber

    End Sub




    Dim Bn_updata As Boolean = True
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If Bn_updata Then
            Me.TextBox1.Enabled = True
            Me.ComboBox1.Enabled = True
            Me.ComboBox2.Enabled = True
            Button1.Text = "保存"
            Bn_updata = False
        Else

            Me.TextBox1.Enabled = False
            Me.ComboBox1.Enabled = False
            Me.ComboBox2.Enabled = False
            Button1.Text = "编辑"
            Bn_updata = True



            DNumber = Me.Label5.Text
            Adress = TextBox1.Text
            DeviceName = Me.ComboBox1.SelectedItem.ToString()
            COMName = Me.ComboBox2.SelectedItem.ToString()

            Dim DeviceStr As String = DNumber + "," + DeviceName + "," + Adress + "," + COMName

            Model_set.WriteINI("Device", "Device" + DNumber, DeviceStr, Cpath)


            DNumber = Me.Label5.Text
            Adress = TextBox1.Text
            DeviceName = Me.ComboBox1.SelectedItem.ToString()
            COMName = Me.ComboBox2.SelectedItem.ToString()

            'MainFrom.IsUpdate = True

        End If


    End Sub





End Class
