Imports System.Collections.Generic
Imports System.Text
Imports System.IO.Ports
Imports System.Globalization
Imports System.Windows.Forms

Namespace SerialClass
    Public Class SerialClass
        'Inherits SerialPort
        Private _serialPort As SerialPort = Nothing
        '定义委托
        Public Delegate Sub SerialPortDataReceiveEventArgs(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs, ByVal bits As Byte())
        '定义接收数据事件
        Public Event DataReceived As SerialPortDataReceiveEventArgs
        '定义接收错误事件
        'Public Event SerialErrorReceivedEventHandler As Error
        '接收事件是否有效 false表示有效
        Public ReceiveEventFlag As Boolean = False

#Region "获取串口名"

        Private protName As String

        Public Property PortName() As String
            Get
                Return _serialPort.PortName
            End Get
            Set(ByVal value As String)
                _serialPort.PortName = value
                protName = value
            End Set
        End Property
#End Region

#Region "获取比特率"

        Private m_baudRate As Integer

        Public Property BaudRate() As Integer
            Get
                Return _serialPort.BaudRate
            End Get
            Set(ByVal value As Integer)
                _serialPort.BaudRate = value
                m_baudRate = value
            End Set
        End Property

#End Region

#Region "默认构造函数"

        ''' <summary>
        ''' 默认构造函数，操作COM1，速度为9600，没有奇偶校验，8位字节，停止位为1 "COM1", 9600, Parity.None, 8, StopBits.One
        ''' </summary>
        Public Sub New()
            _serialPort = New SerialPort()
        End Sub

#End Region

#Region "构造函数"

        ''' <summary>
        ''' 构造函数,
        ''' </summary>
        ''' <param name="comPortName"></param>
        Public Sub New(ByVal comPortName As String)


            _serialPort = New SerialPort(comPortName)

            _serialPort.BaudRate = 9600

            _serialPort.Parity = Parity.Even

            _serialPort.DataBits = 8

            _serialPort.StopBits = StopBits.One

            _serialPort.Handshake = Handshake.None

            _serialPort.RtsEnable = True

            _serialPort.ReadTimeout = 500 'SerialPort.InfiniteTimeout '1000 '读取超时

            _serialPort.WriteTimeout = 500 'SerialPort.InfiniteTimeout ' 100 '写入超时

            setSerialPort()
        End Sub

#End Region

#Region "构造函数,可以自定义串口的初始化参数"

        ''' <summary>
        ''' 构造函数,可以自定义串口的初始化参数
        ''' </summary>
        ''' <param name="comPortName">需要操作的COM口名称</param>
        ''' <param name="baudRate">COM的速度</param>
        ''' <param name="parity">奇偶校验位</param>
        ''' <param name="dataBits">数据长度</param>
        ''' <param name="stopBits">停止位</param>
        Public Sub New(ByVal comPortName As String, ByVal baudRate As Integer, ByVal parity As Parity, ByVal dataBits As Integer, ByVal stopBits As StopBits)


            _serialPort = New SerialPort(comPortName, baudRate, parity, dataBits, stopBits)
            _serialPort.RtsEnable = True
            '自动请求
            _serialPort.ReadTimeout = 500 ' SerialPort.InfiniteTimeout '3000
            '超时

            setSerialPort()
        End Sub

#End Region

#Region "析构函数"

        ''' <summary>
        ''' 析构函数，关闭串口
        ''' </summary>
        Protected Overrides Sub Finalize()
            Try

                If _serialPort.IsOpen Then

                    _serialPort.Close()

                End If
            Finally
                MyBase.Finalize()
            End Try
        End Sub

#End Region

#Region "设置串口参数"

        ''' <summary>
        ''' 设置串口参数
        ''' </summary>
        ''' <param name="comPortName">需要操作的COM口名称</param>
        ''' <param name="baudRate">COM的速度</param>
        ''' <param name="dataBits">数据长度</param>
        ''' <param name="stopBits">停止位</param>
        Public Sub setSerialPort(ByVal comPortName As String, ByVal baudRate As Integer, ByVal dataBits As Integer, ByVal stopBits As Integer)


            If _serialPort.IsOpen Then

                _serialPort.Close()
            End If

            _serialPort.PortName = comPortName

            _serialPort.BaudRate = baudRate

            _serialPort.Parity = Parity.None

            _serialPort.DataBits = dataBits

            _serialPort.StopBits = DirectCast(stopBits, StopBits)

            _serialPort.Handshake = Handshake.None

            _serialPort.RtsEnable = False

            _serialPort.ReadTimeout = 500 ' SerialPort.InfiniteTimeout ' 3000

            _serialPort.NewLine = "/r/n"

            setSerialPort()

        End Sub

#End Region

#Region "设置接收函数"

        ''' <summary>
        ''' 设置串口资源,还需重载多个设置串口的函数
        ''' </summary>
        Private Sub setSerialPort()


            If _serialPort IsNot Nothing Then


                '设置触发DataReceived事件的字节数为1
                _serialPort.ReceivedBytesThreshold = 1

                '接收到一个字节时，也会触发DataReceived事件
                AddHandler _serialPort.DataReceived, New SerialDataReceivedEventHandler(AddressOf _serialPort_DataReceived)
                '_serialPort.DataReceived += New SerialDataReceivedEventHandler(AddressOf _serialPort_DataReceived)

                '接收数据出错,触发事件

                '打开串口
                'openPort();
                AddHandler _serialPort.ErrorReceived, New SerialErrorReceivedEventHandler(AddressOf _serialPort_ErrorReceived)
                '_serialPort.ErrorReceived += New SerialErrorReceivedEventHandler(AddressOf _serialPort_ErrorReceived)
            End If

        End Sub

#End Region

#Region "串口打开状态"
        ''' <summary>
        ''' 串口打开状态
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsOpen() As Boolean
            Get
                Return _serialPort.IsOpen
            End Get
        End Property
#End Region

#Region "缓冲区的字节数量"
        ''' <summary>
        ''' 缓冲区的字节数量
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property BytesToRead() As Integer
            Get
                Return _serialPort.BytesToRead
            End Get
        End Property
#End Region

#Region "打开串口资源"

        ''' <summary>
        ''' 打开串口资源
        ''' <returns>返回bool类型</returns>
        ''' </summary>
        Public Function openPort() As Boolean
            Dim ok As Boolean = False
            '如果串口是打开的，先关闭
            If _serialPort.IsOpen Then
                _serialPort.Close()
            End If
            Try
                '打开串口
                _serialPort.Open()
                ok = True
            Catch Ex As Exception
                Return False
            End Try
            Return ok
        End Function
#End Region

#Region "关闭串口"

        ''' <summary>
        ''' 关闭串口资源,操作完成后,一定要关闭串口
        ''' </summary>
        Public Sub closePort()


            '如果串口处于打开状态,则关闭

            If _serialPort.IsOpen Then

                _serialPort.Close()
            End If

        End Sub

#End Region

#Region "接收串口数据事件"

        ''' <summary>
        ''' 接收串口数据事件
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub _serialPort_DataReceived(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
            '禁止接收事件时直接退出
            If ReceiveEventFlag Then
                Return
            End If

            Try

                System.Threading.Thread.Sleep(20)

                Dim _data As Byte() = New Byte(_serialPort.BytesToRead - 1) {}

                If _data.Length = 0 Then
                    Return
                End If

                _serialPort.Read(_data, 0, _data.Length)

                '_serialPort.DiscardInBuffer()  '清空接收缓冲区  

                RaiseEvent DataReceived(sender, e, _data)

            Catch ex As Exception
                Log("接收串口数据事件 " & ex.Message & ex.StackTrace)
                'MessageBox.Show("接收串口数据事件 " & ex.Message & ex.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End Try

        End Sub

#End Region

#Region "接收数据出错事件"

        ''' <summary>
        ''' 接收数据出错事件
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub _serialPort_ErrorReceived(ByVal sender As Object, ByVal e As SerialErrorReceivedEventArgs)


        End Sub

#End Region

#Region "发送数据string类型"

        Public Sub SendData(ByVal data As String)
            '发送数据

            '禁止接收事件时直接退出
            If ReceiveEventFlag Then
                Return
            End If

            If _serialPort.IsOpen Then
                _serialPort.Write(data)
            End If

        End Sub

#End Region

#Region "发送数据byte类型"

        ''' <summary>
        ''' 数据发送
        ''' </summary>
        ''' <param name="data">数据</param>
        ''' <param name="offset">一般为0</param>
        ''' <param name="count">数据长度</param>
        ''' <remarks></remarks>
        Public Sub SendData(ByVal data As Byte(), ByVal offset As Integer, ByVal count As Integer)
            '禁止接收事件时直接退出
            If ReceiveEventFlag Then
                Return
            End If

            Try
                If _serialPort.IsOpen Then
                    '问题记录, 连接到系统上的设备没有发挥作用！
                    '_serialPort.DiscardInBuffer();//清空接收缓冲区
                    _serialPort.Write(data, offset, count)

                End If
            Catch ex As Exception
                Log("数据发送 " & ex.Message & ex.StackTrace)
                'MessageBox.Show("数据发送 " & ex.Message & ex.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End Try

        End Sub


        Public Function Serialport()
            Return _serialPort
        End Function

#End Region

#Region "发送命令"

        ''' <summary>
        ''' 发送命令
        ''' </summary>
        ''' <param name="SendData">发送数据</param>
        ''' <param name="ReceiveData">接收数据</param>
        ''' <param name="Overtime">超时时间</param>
        ''' <returns></returns>
        Public Function SendCommand(ByVal SendData As Byte(), ByRef ReceiveData As Byte(), ByVal Overtime As Integer) As Integer
            If _serialPort.IsOpen Then
                Try
                    ReceiveEventFlag = True  '关闭接收事件
                    _serialPort.DiscardInBuffer() '清空接收缓冲区     

                    _serialPort.Write(SendData, 0, SendData.Length)

                    Dim num As Integer = 0, ret As Integer = 0

                    System.Threading.Thread.Sleep(50)

                    ReceiveEventFlag = False
                    '打开事件
                    While System.Math.Max(System.Threading.Interlocked.Increment(num), num - 1) < Overtime

                        If _serialPort.BytesToRead >= ReceiveData.Length Then

                            Exit While
                        End If

                        System.Threading.Thread.Sleep(20)
                    End While

                    If _serialPort.BytesToRead >= ReceiveData.Length Then
                        ret = _serialPort.Read(ReceiveData, 0, ReceiveData.Length)
                    Else
                        ret = _serialPort.Read(ReceiveData, 0, _serialPort.BytesToRead)
                    End If
                    ReceiveEventFlag = False
                    '打开事件
                    Return ret
                Catch ex As Exception
                    ReceiveEventFlag = False
                    'MessageBox.Show("发送命令 " & ex.Message & ex.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)

                End Try
            End If
            Return -1
        End Function

#End Region

#Region "获取串口"

        ''' <summary>
        ''' 获取所有已连接电脑的设备的串口
        ''' </summary>
        ''' <returns></returns>
        Public Function serialsIsConnected() As String()
            Dim lists As New List(Of String)()
            Dim seriallist As String() = getSerials()
            For Each s As String In seriallist
                lists.Add(s.Trim)
            Next
            Return lists.ToArray()
        End Function

#End Region

#Region "获取当前全部串口资源"

        ''' <summary>
        ''' 获得当前电脑上的所有串口资源
        ''' </summary>
        ''' <returns></returns>
        Public Function getSerials() As String()
            Return SerialPort.GetPortNames()
        End Function

#End Region

#Region "字节型转换16"

        ''' <summary>
        ''' 把字节型转换成十六进制字符串
        ''' </summary>
        ''' <param name="InBytes"></param>
        ''' <returns></returns>
        Public Shared Function ByteToString(ByVal InBytes As Byte()) As String


            Dim StringOut As String = ""

            For Each InByte As Byte In InBytes



                StringOut = StringOut & [String].Format("{0:X2} ", InByte)
            Next

            Return StringOut

        End Function

#End Region

#Region "十六进制字符串转字节型"

        ''' <summary>
        ''' 把十六进制字符串转换成字节型(方法1)
        ''' </summary>
        ''' <param name="InString"></param>
        ''' <returns></returns>
        Public Shared Function StringToByte(ByVal InString As String) As Byte()


            Dim ByteStrings As String()

            ByteStrings = InString.Split(" ".ToCharArray())

            Dim ByteOut As Byte()

            ByteOut = New Byte(ByteStrings.Length - 1) {}

            For i As Integer = 0 To ByteStrings.Length - 1


                'ByteOut[i] = System.Text.Encoding.ASCII.GetBytes(ByteStrings[i]);


                'ByteOut[i] =Convert.ToByte("0x" + ByteStrings[i]);

                ByteOut(i) = [Byte].Parse(ByteStrings(i), System.Globalization.NumberStyles.HexNumber)
            Next

            Return ByteOut

        End Function

#End Region

#Region "十六进制字符串转字节型"

        ''' <summary>
        ''' 字符串转16进制字节数组(方法2)
        ''' </summary>
        ''' <param name="hexString"></param>
        ''' <returns></returns>
        Public Shared Function strToToHexByte(ByVal hexString As String) As Byte()


            hexString = hexString.Replace(" ", "")

            If (hexString.Length Mod 2) <> 0 Then

                hexString += " "
            End If

            Dim returnBytes As Byte() = New Byte(hexString.Length / 2 - 1) {}

            For i As Integer = 0 To returnBytes.Length - 1

                returnBytes(i) = Convert.ToByte(hexString.Substring(i * 2, 2), 16)
            Next

            Return returnBytes

        End Function

#End Region

#Region "字节型转十六进制字符串"

        ''' <summary>
        ''' 字节数组转16进制字符串
        ''' </summary>
        ''' <param name="bytes"></param>
        ''' <returns></returns>
        Public Shared Function byteToHexStr(ByVal bytes As Byte()) As String


            Dim returnStr As String = ""

            If bytes IsNot Nothing Then


                For i As Integer = 0 To bytes.Length - 1



                    returnStr += bytes(i).ToString("X2")

                Next
            End If

            Return returnStr

        End Function

#End Region

    End Class

End Namespace



'======================================================================================

'调用方法：
'                   Public WithEvents SC_Relay_control As New SerialClass.SerialClass
'                   SC_Relay_control.setSerialPort( _
'                   ComboBox7.Text, ComboBox6.Text, 8, IO.Ports.StopBits.One)
'                   Control.CheckForIllegalCrossThreadCalls = False '线程之间进行通讯
'                   AddHandler SC_Relay_control.DataReceived, _
'                   New SerialClass.SerialClass.SerialPortDataReceiveEventArgs(AddressOf sc_DataReceived)
'
'======================================================================================
