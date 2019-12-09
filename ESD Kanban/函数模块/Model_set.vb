Imports ESD_Kanban.DBConfig
Imports System.Data
Imports System.Net
Imports System.Management

Module Model_set

#Region "ini读取函数"
    ''' <summary>
    ''' 获取应用程序的安装目录
    ''' </summary>
    ''' <remarks></remarks>
    Public progpath As String = My.Application.Info.DirectoryPath


    ''' <summary>
    ''' 配置文件路径
    ''' </summary>
    ''' <remarks></remarks>
    Public Configurationpath As String = My.Application.Info.DirectoryPath & "\Parameter.ini"

    ''' <summary>
    ''' 声明INI配置文件读写API函数
    ''' </summary>
    ''' <param name="lpApplicationName"></param>
    ''' <param name="lpKeyName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="lpReturnedString"></param>
    ''' <param name="nSize"></param>
    ''' <param name="lpFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Int32, ByVal lpFileName As String) As Int32
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Int32
    ''' <summary>
    ''' 定义读取配置文件函数
    ''' </summary>
    ''' <param name="Section"></param>
    ''' <param name="AppName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As String
        Dim Str As String = LSet(Str, 256)
        GetPrivateProfileString(Section, AppName, lpDefault, Str, Len(Str), FileName)
        Return Microsoft.VisualBasic.Left(Str, InStr(Str, Chr(0)) - 1)
    End Function
    ''' <summary>
    ''' 定义写入配置文件函数
    ''' </summary>
    ''' <param name="Section"></param>
    ''' <param name="AppName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As Long
        WriteINI = WritePrivateProfileString(Section, AppName, lpDefault, FileName)
    End Function

#End Region

    ''' <summary>
    '''程序版本
    ''' </summary>
    ''' <returns></returns>
    Public Property getver As String = "1.0.0.10"

#Region "左边-数采控制器串口参数设置"
    ''' <summary>
    ''' 实例化串口(数采控制器)
    ''' </summary>
    ''' <remarks></remarks>
    Public WithEvents SC_Relay_control_Left As New SerialClass.SerialClass

    ''' <summary>
    ''' 串口名称(数采控制器)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Relay_control_source_Left() As String
        Get
            Return GetINI("Configuration", "Relay_control_source_Left", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Relay_control_source_Left", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 串口波特率(数采控制器)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Relay_control_source_Baud_Left() As String
        Get
            Return GetINI("Configuration", "Relay_control_source_Baud_Left", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Relay_control_source_Baud_Left", value.Trim, Configurationpath)
        End Set
    End Property
#End Region

#Region "右边-数采控制器串口参数设置"
    ''' <summary>
    ''' 实例化串口(数采控制器)
    ''' </summary>
    ''' <remarks></remarks>
    Public WithEvents SC_Relay_control_right As New SerialClass.SerialClass

    ''' <summary>
    ''' 串口名称(数采控制器)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Relay_control_source_right() As String
        Get
            Return GetINI("Configuration", "Relay_control_source_right", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Relay_control_source_right", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 串口波特率(数采控制器)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Relay_control_source_Baud_right() As String
        Get
            Return GetINI("Configuration", "Relay_control_source_Baud_right", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Relay_control_source_Baud_right", value.Trim, Configurationpath)
        End Set
    End Property
#End Region


    ''' <summary>
    ''' 设备数量
    ''' </summary>
    ''' <returns></returns>
    Public Property Quantity_equipment() As String
        Get
            Return GetINI("Connect", "Quantity_equipment", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Connect", "Quantity_equipment", value.Trim, Configurationpath)
        End Set
    End Property





#Region "报警灯设置串口参数"
    ''' <summary>
    ''' 实例化串口(报警灯控制)
    ''' </summary>
    ''' <remarks></remarks>
    Public WithEvents SC_Cell_police_control As New SerialClass.SerialClass

    ''' <summary>
    ''' 串口名称(报警灯设置)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Relay_control_Cell_police() As String
        Get
            Return GetINI("Configuration", "Relay_control_Cell_police", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Relay_control_Cell_police", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 串口波特率(报警灯设置)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Relay_control_Cell_police_Baud() As String
        Get
            Return GetINI("Configuration", "Relay_control_Cell_police_Baud", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Relay_control_Cell_police_Baud", value.Trim, Configurationpath)
        End Set
    End Property
#End Region

    ''' <summary>
    ''' 错误信息记录
    ''' </summary>
    Public Error_record As New Error_record

    ''' <summary>
    ''' 上传ESD信息到SQL 左侧
    ''' </summary>
    Public esdUpdateL As New Info_uploading

    ''' <summary>
    ''' 上传ESD信息到SQL 右侧
    ''' </summary>
    Public esdUpdateR As New Info_uploading

    ''' <summary>
    ''' ESD数量-左边
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ESD_NUM_Left() As String
        Get
            Dim esdnum As String = GetINI("Configuration", "ESD_NUM_Left", "", Configurationpath)
            If esdnum.Trim = "" Then
                esdnum = 0
            End If
            Return esdnum
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "ESD_NUM_Left", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' ESD数量-右边
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ESD_NUM_right() As String
        Get
            Dim esdnum As String = GetINI("Configuration", "ESD_NUM_right", "", Configurationpath)
            If esdnum.Trim = "" Then
                esdnum = 0
            End If
            Return esdnum
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "ESD_NUM_right", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 扫描间隔时间
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Scanning_interval As String
        Get
            Try
                Dim val As String = GetINI("Configuration", "Scanning_interval", "", Configurationpath)
                Dim valint As Int64 = 0
                If String.IsNullOrEmpty(val) = False Then
                    valint = CInt(val)
                Else
                    valint = 1000
                End If
                If valint < 1000 Then
                    valint = 1000
                End If
                Return valint
            Catch ex As Exception
                Return 1000
            End Try
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Scanning_interval", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 超时时间
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Scanning_timeout As String
        Get
            Return GetINI("Configuration", "Scanning_timeout", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Scanning_timeout", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 警报检测时间
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Alarm_detection As String
        Get
            Return GetINI("Configuration", "Alarm_detection", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Alarm_detection", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 返回程序本身的进程PID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Process_pid() As Integer
        Get
            Return Process.GetCurrentProcess().Id
        End Get
        Set(ByVal value As Integer)
            'MsgBox(String.Format("pid = {0}({0:X8})", Process.GetCurrentProcess().Id))
        End Set
    End Property


    ''' <summary>
    ''' ESD串口协议结构
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure ESDConfig
        ''' <summary>
        ''' ESD编号
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_NUM As String

        ''' <summary>
        ''' 查询
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_query As String

        ''' <summary>
        ''' 离线
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_Off_line As String

        ''' <summary>
        ''' 失效
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_Invalid As String

        ''' <summary>
        ''' 正常
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_normal As String

        ''' <summary>
        ''' 模式
        ''' </summary>
        ''' <remarks></remarks>
        Dim ESD_Choice As Choice

        ''' <summary>
        ''' 是否被启用
        ''' </summary>
        Dim ESD_isEnable As String

    End Structure

    ''' <summary>
    ''' 模式选择
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum Choice
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

    Dim ch As String = ""
    ''' <summary>
    ''' ESD编号
    ''' </summary>
    ''' <param name="num"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ESD_NUM(ByVal num As String, ByVal Model As Choice) As ESDConfig
        Get
            Select Case Model
                Case Choice.Left
                    ch = "Left"
                Case Choice.right
                    ch = "right"
            End Select
            Dim str As String() = GetINI("ESDConfig_" & ch, "ESD" & num, "", Configurationpath).Split(",")
            Dim t_ESDConfig As New ESDConfig
            If str.Count = 5 Then
                t_ESDConfig.ESD_NUM = str(0).Trim
                t_ESDConfig.ESD_query = str(1).Trim
                t_ESDConfig.ESD_Off_line = str(2).Trim
                t_ESDConfig.ESD_Invalid = str(3).Trim
                t_ESDConfig.ESD_normal = str(4).Trim
            End If
            Return t_ESDConfig
        End Get
        Set(ByVal value As ESDConfig)
            Dim text As New System.Text.StringBuilder
            text.Append(value.ESD_NUM)
            text.Append(",")
            text.Append(value.ESD_query)
            text.Append(",")
            text.Append(value.ESD_Off_line)
            text.Append(",")
            text.Append(value.ESD_Invalid)
            text.Append(",")
            text.Append(value.ESD_normal)
            WriteINI("ESDConfig_" & ch, "ESD" & value.ESD_NUM, text.ToString.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' A采集器串口指令集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ESD_DIC_Left As New Dictionary(Of Integer, List(Of ESDConfig))


    ''' <summary>
    ''' B采集器串口指令集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ESD_DIC_right As New Dictionary(Of Integer, List(Of ESDConfig))

    ''' <summary>
    ''' 查询SQL,返回数据
    ''' </summary>
    ''' <param name="SQLString">SQL代码</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Data_query(ByVal SQLString As String, ByVal ConnectionString As String) As List(Of Object())
        '输出数据库内容
        Try
            Dim list_data As New List(Of Object())
            Dim list_str As New ArrayList
            '创建DBDataTable对象 
            Dim dt As DBDataTable = New DBDataTable(ConnectionString) '(New ConnectionString().ConnectionInfo)
            '调用DBDataTable的CreateDataTable函数，得到DataTable表
            Dim table As DataTable = dt.CreateDataTable(SQLString, Nothing)
            '在循环中遍历DataTable表，逐行逐列把表中的内容加入到ListView控件中
            Dim UserRow As DataRow
            '遍历每一行
            For Each UserRow In table.Rows
                Dim i As Integer
                list_str.Clear()
                '遍历一行中的所有列
                For i = 0 To UserRow.ItemArray.Count - 1
                    Dim t_data As String = UserRow(i).ToString
                    list_str.Add(t_data)
                Next
                list_data.Add(list_str.ToArray)

            Next
            Return list_data
        Catch ex As Exception

        End Try
    End Function

    ''' <summary>
    ''' 班长
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property t_Monitor() As String
        Get
            Return GetINI("Configuration", "Monitor", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "Monitor", value.Trim, Configurationpath)
        End Set
    End Property


    ''' <summary>
    ''' 线体
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property t_Line_body() As String
        Get
            Return GetINI("Configuration", "t_Line_body", "", Configurationpath)
        End Get
        Set(ByVal value As String)
            WriteINI("Configuration", "t_Line_body", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 信息提交失败后的重试次数
    ''' </summary>
    ''' <returns></returns>
    Public Property T_Retry_Count() As String
        Get

            Dim RetryCount As String = GetINI("Submission", "RetryCount", "", Configurationpath)
            If RetryCount = "" Then
                RetryCount = "30"
            End If
            Return RetryCount
        End Get
        Set(ByVal value As String)
            WriteINI("Submission", "RetryCount", value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 窗体模式选择
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum t_Pattern_selection
        班长选择 = 0
        线体选择 = 1
    End Enum

    ''' <summary>
    ''' 模式选择
    ''' </summary>
    ''' <remarks></remarks>
    Public t_state As New t_Pattern_selection

    Dim C1 As String = ""
    ''' <summary>
    ''' 查询指定ESD报警器是否启用
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ESD_Enable(ByVal ESDNUM As String, ByVal Model As Choice) As String
        Get
            Select Case Model
                Case Choice.Left
                    C1 = "Left"
                Case Choice.right
                    C1 = "right"
            End Select
            Return GetINI("ESDEnable_" & C1, "ESD" & ESDNUM, "", Configurationpath)
        End Get
        Set(ByVal value As String)
            Select Case Model
                Case Choice.Left
                    C1 = "Left"
                Case Choice.right
                    C1 = "right"
            End Select
            WriteINI("ESDEnable_" & C1, "ESD" & ESDNUM, value.Trim, Configurationpath)
        End Set
    End Property

    ''' <summary>
    ''' 计算机名称
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property hostName
        Get
            Return Dns.GetHostName()
        End Get
    End Property

    ''' <summary>
    ''' 获取IP地址
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IPadd As String
        Get
            Return System.Net.Dns.GetHostByName(Net.Dns.GetHostName).AddressList.GetValue(0).ToString
        End Get
    End Property

    ''' <summary>
    ''' 获取MAC地址
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMacAddress() As String
        Try
            Dim mac As String = ""
            Dim mc As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")
            Dim moc As ManagementObjectCollection = mc.GetInstances()

            For Each mo As ManagementObject In moc

                If CBool(mo("IPEnabled")) = True Then
                    mac = mo("MacAddress").ToString()
                    Exit For
                End If
            Next

            moc = Nothing
            mc = Nothing
            Return mac
        Catch
            Return "unknow"
        Finally
        End Try
    End Function

    ''' <summary>
    ''' 返回服务器时间
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Getdate() As String
        Get
            Dim t_date As List(Of Object()) = Data_query("SELECT GETDATE()", New ConnectionString().ConnectionInfo)
            If t_date IsNot Nothing Then
                For Each T In t_date
                    If T.Count = 1 Then
                        Return T(0).ToString.Trim
                    End If
                Next
            End If
        End Get
    End Property


    ''' <summary>
    ''' 生成json日志
    ''' </summary>
    ''' <param name="data"></param>
    Public Sub Log(ByVal data As String)
        On Error Resume Next
        '写入json文档
        Dim path As String = IO.Path.GetDirectoryName(Application.ExecutablePath) & "\Log\" & Format(Now, "yyyyMMdd")
        If My.Computer.FileSystem.DirectoryExists(path) = False Then
            My.Computer.FileSystem.CreateDirectory(path)
        End If



        Dim logstr As String = vbCrLf & Format(Now, "yyyy-MM-dd HH:mm:ss") &
            " - 线程数[" & Process.GetCurrentProcess().Threads.Count & "] " &
            "线程[" & System.Threading.Thread.CurrentThread.Name & "] 程序版本[" &
            getver & "] 线体[" & t_Line_body & "]" & data

        IO.File.AppendAllText(
            path & "\" & Format(Now, "yyyyMMdd").ToString & ".log",
            logstr,
            System.Text.Encoding.GetEncoding("GB2312"))
        Debug.Print(logstr)
    End Sub

End Module
