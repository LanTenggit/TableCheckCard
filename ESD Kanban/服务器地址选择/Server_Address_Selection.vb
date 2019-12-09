Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

''' <summary>
''' 服务器地址选择
''' </summary>
Public Class Server_Address_Selection

    ''' <summary>
    ''' 10.124.157.185
    ''' </summary>
    Public Const Address_01 As String = "10.124.157.185"
    ''' <summary>
    ''' 10.114.109.235
    ''' </summary>
    Public Const Address_02 As String = "10.114.109.235"
    ''' <summary>
    ''' 10.114.146.26
    ''' </summary>
    Public Const Address_03 As String = "10.114.146.26"
    ''' <summary>
    ''' 10.114.113.11
    ''' </summary>
    Public Const Address_04 As String = "10.114.113.11" '掉线严重,不建议使用


    ''' <summary>
    ''' 获取能够使用的服务器地址
    ''' </summary>
    ''' <returns></returns>
    Public Function Scanning() As String

        Dim connection_01 As TcpClient = New TcpClientWithTimeout(Address_01, 1433, 1000).Connect()
        If connection_01 IsNot Nothing Then
            Log(Address_01.ToString)
            connection_01.Close()
            Return Address_01.ToString
        End If

        Dim connection_02 As TcpClient = New TcpClientWithTimeout(Address_02, 1433, 1000).Connect()
        If connection_02 IsNot Nothing Then
            Log(Address_02.ToString)
            connection_02.Close()
            Return Address_02.ToString
        End If

        Dim connection_03 As TcpClient = New TcpClientWithTimeout(Address_03, 1433, 1000).Connect()
        If connection_03 IsNot Nothing Then
            Log(Address_03.ToString)
            connection_03.Close()
            Return Address_03.ToString
        End If

        Dim connection_04 As TcpClient = New TcpClientWithTimeout(Address_04, 1433, 1000).Connect()
        If connection_04 IsNot Nothing Then
            Log(Address_04.ToString)
            connection_04.Close()
            Return Address_04.ToString
        End If
        Return Nothing
    End Function


End Class


''' <summary>
''' TcpClientWithTimeout 用来设置一个带连接超时功能的类
''' 使用者可以设置毫秒级的等待超时时间 (1000=1second)
''' 例如:
''' TcpClient connection = new TcpClientWithTimeout('127.0.0.1',80,1000).Connect();
''' </summary>
Public Class TcpClientWithTimeout
    Protected _hostname As String
    Protected _port As Integer
    Protected _timeout_milliseconds As Integer
    Protected connection As TcpClient
    Protected connected As Boolean
    Protected exception As Exception

    Public Sub New(hostname As String, port As Integer, timeout_milliseconds As Integer)
        _hostname = hostname
        _port = port
        _timeout_milliseconds = timeout_milliseconds
    End Sub
    ''' <summary>
    ''' 连接到端口
    ''' </summary>
    ''' <returns></returns>
    Public Function Connect() As TcpClient
        ' kick off the thread that tries to connect
        connected = False
        exception = Nothing
        Dim thread As New Thread(New ThreadStart(AddressOf BeginConnect))
        thread.IsBackground = True
        ' 作为后台线程处理
        ' 不会占用机器太长的时间
        thread.Start()

        ' 等待如下的时间
        thread.Join(_timeout_milliseconds)

        If connected = True Then
            ' 如果成功就返回TcpClient对象
            thread.Abort()
            Return connection
        Else
            thread.Abort()
            Return Nothing
        End If
        'If exception IsNot Nothing Then
        '    ' 如果失败就抛出错误
        '    thread.Abort()
        '    'Throw exception
        'Else
        '    ' 同样地抛出错误
        '    thread.Abort()
        '    Dim message As String = String.Format("TcpClient connection to {0}:{1} timed out", _hostname, _port)
        '    Throw New TimeoutException(message)
        'End If
    End Function
    Protected Sub BeginConnect()
        Try
            connection = New TcpClient(_hostname, _port)
            ' 标记成功，返回调用者
            connected = True
        Catch ex As Exception
            ' 标记失败
            exception = ex
        End Try
    End Sub
End Class
