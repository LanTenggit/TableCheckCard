Imports ESD_Kanban.DBConfig
Imports Newtonsoft.Json
''' <summary>
''' 信息上传到SQL
''' </summary>
Public Class Info_uploading

    Public Sub New()
        '空构造
    End Sub

    ''' <summary>
    ''' 线体
    ''' </summary>
    ''' <returns></returns>
    Public Property Line_body As String

    ''' <summary>
    ''' 班长
    ''' </summary>
    ''' <returns></returns>
    Public Property Monitor As String

    ''' <summary>
    ''' ESD编号
    ''' </summary>
    ''' <returns></returns>
    Public Property ESD_number As String

    ''' <summary>
    ''' ESD状态
    ''' </summary>
    ''' <returns></returns>
    Public Property ESD_state As String

    ''' <summary>
    ''' 更新时间
    ''' </summary>
    ''' <returns></returns>
    Public Property Update_time As DateTime

    ''' <summary>
    ''' GUID
    ''' </summary>
    ''' <returns></returns>
    Public Property getGUID As String

    ''' <summary>
    ''' 提交信息到服务器
    ''' </summary>
    Public Sub Update()

        '将sql请求提交给线程池运行
        System.Threading.ThreadPool.QueueUserWorkItem(New Threading.WaitCallback(AddressOf tbUpdate))

    End Sub


    ''' <summary>
    ''' 提交信息到服务器
    ''' </summary>
    Public Sub tbUpdate()
        '问题记录,线程数超过1000个,出现卡死问题. - 2019-03-11
        '确认问题是由于网络不停闪断造成的,数据库操作超时,造成数据库操作线程阻塞.
        '解决办法,将数据库操作与线程分离,单独一个线程循环执行数据库操作.

        '这里可能是要线程池来解决上传数据的问题

        'Log("线程池 -> tbUpdate 执行")

        If String.Compare(t_Line_body.Trim, "", False) = 0 Or String.Compare(t_Line_body.Trim, "未选择", False) = 0 Then
            '为选择线体的直接不上传数据
            Exit Sub
        End If

        Try
            '处理避免数据出现大量的重复值
            'log(Line_body & Monitor & ESD_number & ESD_state)

            Dim T As New T_Update With {
            .ESD_number = ESD_number,
            .ESD_state = ESD_state,
            .Line_body = Line_body,
            .Monitor = Monitor,
            .Update_time = Now,
            .GetGUID = Guid.NewGuid.ToString,
            .t_Creation_time = Now
            }

            If ESDINFO.Keys.Contains(ESD_number) = True Then
                '如果键存在,就进行比对,不一致就进行修改操作,一致就跳过
                If ESDINFO(ESD_number).ESD_state = ESD_state Then
                    'Log("线程池 -> 状态未变化退出:" & ESD_number & "|" & ESD_state)
                    '如果结果相等就跳过
                    Exit Sub
                Else
                    '修改上一笔数据的结束时间
                    Error_record.Geterr("", "") '初始化错误捕获信息
                    Dim t_Second As Long = DateDiff(DateInterval.Second, ESDINFO(ESD_number).t_Creation_time, Now)

                    'Log("线程池 -> 修改上一笔数据的结束时间:" & ESD_number & "|" & t_Second)

                    Dim strContent As String = "t_End_time='" & Format(Now, "yyyy-MM-dd HH:mm:ss") & "',t_Duration_time='" & t_Second & "'"


                    Log("更新|" & "update t_ESD_status_record set " & strContent & " where t_guid='" & ESDINFO(ESD_number).GetGUID & "'")

                    'Dim cmd1 As DBCommand = New DBCommand(New ConnectionString().ConnectionInfo)
                    'If cmd1.Update("t_ESD_status_record ", strContent, "t_guid", "'" & ESDINFO(ESD_number).GetGUID & "'") > 0 Then
                    '    '更新成功!
                    '    Log("更新成功|" & "update t_ESD_status_record set " & strContent & " where t_guid='" & ESDINFO(ESD_number).GetGUID & "'")
                    'Else
                    '    Log("更新失败|" & "update t_ESD_status_record set " & strContent & " where t_guid='" & ESDINFO(ESD_number).GetGUID & "'")
                    '    'Dim failed_data As New Submit_failed_data With {.Sign = 0,
                    '    '    .SQL = "update t_ESD_status_record set " & strContent & " where t_guid='" & ESDINFO(ESD_number).GetGUID & "'",
                    '    '    .GetErrString = Error_record.GetErrString,
                    '    '    .GetStackTrace = Error_record.GetStackTrace,
                    '    '    .GetretryCount = 0}
                    '    'Journal(failed_data)
                    'End If

                    'Log("线程池 -> 更新键表集合:" & ESD_number)

                    '结果不相等就更新集合
                    ESDINFO(ESD_number) = T

                    '插入数据
                    Dim sql As String = "insert into t_ESD_status_record " &
                    " (t_Creation_time,t_modify_time,t_Line_body,t_Monitor,t_ESD_number,t_ESD_state," &
                    "t_Computer_name,t_IP_address,t_MAC_address,t_System_version,t_guid)" &
                    " values (getdate(),getdate(),'" &
                    ESDINFO(ESD_number).Line_body & "','" &
                    ESDINFO(ESD_number).Monitor & "','" &
                    ESDINFO(ESD_number).ESD_number & "','" &
                    ESDINFO(ESD_number).ESD_state & "','" &
                    hostName & "','" &
                    IPadd & "','" &
                    GetMacAddress() & "','" &
                    getver() & "','" &
                    ESDINFO(ESD_number).GetGUID & "')"
                    Error_record.Geterr("", "") '初始化错误捕获信息
                    Dim cmd As DBCommand = New DBCommand(New ConnectionString().ConnectionInfo)

                    'Log("线程池 -> 插入状态到数据库:" & ESD_number & "|" & sql)

                    If cmd.Insert(sql) > 0 Then
                        '更新成功!
                        'Log("插入成功|" & sql)
                    Else
                        'Log("插入失败|" & sql)
                        '更新失败的话就记录信息,后面忘了正常以后再更新
                        Dim failed_data As New Submit_failed_data With {.Sign = 1,
                                .SQL = sql,
                                .GetErrString = Error_record.GetErrString,
                                .GetStackTrace = Error_record.GetStackTrace,
                                .GetretryCount = 0}
                        Journal(failed_data)
                        'Error_record.Geterr(ex.Message, ex.StackTrace)
                    End If

                End If
            Else

                'Log("线程池 -> 添加新键值:" & ESD_number)

                '如果键不存在就添加到集合
                ESDINFO.Add(ESD_number, T)


                '插入数据
                Dim sql As String = "insert into t_ESD_status_record " &
                " (t_Creation_time,t_modify_time,t_Line_body,t_Monitor,t_ESD_number,t_ESD_state," &
                "t_Computer_name,t_IP_address,t_MAC_address,t_System_version,t_guid)" &
                " values (getdate(),getdate(),'" &
                ESDINFO(ESD_number).Line_body & "','" &
                ESDINFO(ESD_number).Monitor & "','" &
                ESDINFO(ESD_number).ESD_number & "','" &
                ESDINFO(ESD_number).ESD_state & "','" &
                hostName & "','" &
                IPadd & "','" &
                GetMacAddress() & "','" &
                getver() & "','" &
                ESDINFO(ESD_number).GetGUID & "')"
                Error_record.Geterr("", "") '初始化错误捕获信息
                Dim cmd As DBCommand = New DBCommand(New ConnectionString().ConnectionInfo)

                'Log("线程池 -> 插入状态到数据库:" & ESD_number & "|" & sql)

                If cmd.Insert(sql) > 0 Then
                    '更新成功!
                    'Log("插入成功|" & sql)
                Else
                    'Log("插入失败|" & sql)
                    '更新失败的话就记录信息,后面忘了正常以后再更新
                    Dim failed_data As New Submit_failed_data With {.Sign = 1,
                            .SQL = sql,
                            .GetErrString = Error_record.GetErrString,
                            .GetStackTrace = Error_record.GetStackTrace,
                            .GetretryCount = 0}
                    Journal(failed_data)
                    'Error_record.Geterr(ex.Message, ex.StackTrace)
                End If
            End If

        Catch ex As Exception
            Log("提交信息到服务器 " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' 生成json日志
    ''' </summary>
    ''' <param name="data"></param>
    Public Shared Sub Journal(ByVal data As Object)
        On Error Resume Next
        '生成json数据
        Dim json = JsonConvert.SerializeObject(data) '(ESDINFO(ESD_number))
        '写入json文档
        Dim path As String = IO.Path.GetDirectoryName(Application.ExecutablePath) & "\abnormal\" & Format(Now, "yyyyMMdd")
        If My.Computer.FileSystem.DirectoryExists(path) = False Then
            My.Computer.FileSystem.CreateDirectory(path)
        End If
        IO.File.AppendAllText(
            path & "\" & Guid.NewGuid.ToString & ".json",
            json,
            System.Text.Encoding.GetEncoding("GB2312"))
    End Sub

    ''' <summary>
    ''' 失败的数据
    ''' </summary>
    Private Structure Submit_failed_data
        ''' <summary>
        ''' SQL语句
        ''' </summary>
        ''' <returns></returns>
        Public Property SQL As String

        ''' <summary>
        ''' 是否处理
        ''' </summary>
        ''' <returns>0为未处理，1为处理</returns>
        Public Property Sign As Integer

        ''' <summary>
        ''' 错误信息
        ''' </summary>
        ''' <returns></returns>
        Public Property GetErrString As String

        ''' <summary>
        ''' 详细的错误信息
        ''' </summary>
        ''' <returns></returns>
        Public Property GetStackTrace As String

        ''' <summary>
        ''' 重试次数
        ''' </summary>
        ''' <returns></returns>
        Public Property GetretryCount As Integer

    End Structure

    ''' <summary>
    ''' ESD数据结构
    ''' </summary>
    Private Structure T_Update
        ''' <summary>
        ''' 线体
        ''' </summary>
        ''' <returns></returns>
        Public Property Line_body As String

        ''' <summary>
        ''' 班长
        ''' </summary>
        ''' <returns></returns>
        Public Property Monitor As String

        ''' <summary>
        ''' ESD编号
        ''' </summary>
        ''' <returns></returns>
        Public Property ESD_number As String

        ''' <summary>
        ''' ESD状态
        ''' </summary>
        ''' <returns></returns>
        Public Property ESD_state As String

        ''' <summary>
        ''' 更新时间
        ''' </summary>
        ''' <returns></returns>
        Public Property Update_time As DateTime

        ''' <summary>
        ''' GUID
        ''' </summary>
        ''' <returns></returns>
        Public Property GetGUID As String

        ''' <summary>
        ''' 创建时间
        ''' </summary>
        ''' <returns></returns>
        Public Property t_Creation_time As DateTime

    End Structure

    ''' <summary>
    ''' ESD详细情况
    ''' </summary>
    Private ESDINFO As Dictionary(Of String, T_Update) = New Dictionary(Of String, T_Update)

End Class
