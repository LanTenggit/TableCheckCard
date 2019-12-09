Public Class UserDeviceInfo
    ''' <summary>
    ''' 设备名称
    ''' </summary>
    ''' <returns></returns>
    Public Property DeviceName() As String
        Get
            Return m_DeviceName
        End Get
        Set
            m_DeviceName = Value
        End Set
    End Property
    Private m_DeviceName As String
    ''' <summary>
    ''' 设备地址
    ''' </summary>
    ''' <returns></returns>
    Public Property DeviceAdrees() As String
        Get
            Return m_DeviceAdrees
        End Get
        Set
            m_DeviceAdrees = Value
        End Set
    End Property
    Private m_DeviceAdrees As String

    ''' <summary>
    '''  设备产量
    ''' </summary>
    ''' <returns></returns>
    Public Property DeviceSumYields() As String
        Get
            Return m_DeviceSumYields
        End Get
        Set
            m_DeviceSumYields = Value
        End Set
    End Property
    Private m_DeviceSumYields As String

    ''' <summary>
    ''' 不良数
    ''' </summary>
    ''' <returns></returns>
    Public Property DeviceBedNum() As String
        Get
            Return m_DeviceBedNum
        End Get
        Set
            m_DeviceBedNum = Value
        End Set
    End Property
    Public m_DeviceBedNum As String
    ''' <summary>
    ''' 压力
    ''' </summary>
    ''' <returns></returns>
    Public Property DevicePower() As String
        Get
            Return m_DevicePower
        End Get
        Set
            m_DevicePower = Value
        End Set
    End Property
    Private m_DevicePower As String
    ''' <summary>
    ''' 时间
    ''' </summary>
    ''' <returns></returns>
    Public Property DeviceTime() As String
        Get
            Return m_DeviceTime
        End Get
        Set
            m_DeviceTime = Value
        End Set
    End Property
    Private m_DeviceTime As String


    Private Sub UserDeviceInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        pb_01.SizeMode = PictureBoxSizeMode.StretchImage
        ImageList1.ColorDepth = ColorDepth.Depth32Bit

        lb_DName.Text = DeviceName
        Lb_DAdrees.Text = DeviceAdrees
        Lb_DYieds.Text = DeviceSumYields
        Lb_DBedNum.Text = DeviceBedNum
        Lb_DPower.Text = DevicePower
        Lb_DTime.Text = DeviceTime

        'Me.Invoke(New Action(Sub()

        '                         Select Case DeviceName
        '                             Case "F1"
        '                                 pb_01.Image = Me.ImageList1.Images(0)

        '                             Case "F2", "F3", "F4"
        '                                 pb_01.Image = Me.ImageList1.Images(0)

        '                         End Select

        '                     End Sub))


    End Sub
End Class
