Imports System.IO
Imports ESD_Kanban
Imports ESD_Kanban.DBConfig
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Retry_uploading
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
    ''' 是否启动
    ''' </summary>
    Private Shared IsBol As Boolean = False
    ''' <summary>
    ''' 重新加载失败的SQL语句
    ''' </summary>
    Public Shared Sub Retry_update()
        If IsBol = True Then
            Exit Sub '如果已经启动就结束
        End If
        IsBol = True
        Dim path As String = IO.Path.GetDirectoryName(Application.ExecutablePath) & "\abnormal\" & Format(Now, "yyyyMMdd")
        If My.Computer.FileSystem.DirectoryExists(path) = False Then
            My.Computer.FileSystem.CreateDirectory(path)
        End If

        Dim getfiles As List(Of String) = FindFile(path, "*.json")

        For Each mFileInfo In getfiles
            Try

                'Dim ip As String = GetINI("Connect", "server", "", My.Application.Info.DirectoryPath & "\Parameter.ini")
                'Dim ipadd As String() = ip.Split(",")
                'If ipadd.Count = 2 Then
                '    ip = ipadd(0).Trim
                'End If

                'Dim siteResponds = My.Computer.Network.Ping(ip)
                'If siteResponds = False Then
                '    Log("服务器ping不通,跳过...")
                '    Continue For
                'End If

                Dim jsontext As String = Text_read(mFileInfo).Replace(vbCrLf, "")

                Dim json As JObject = CType(JsonConvert.DeserializeObject(jsontext), JObject)

                If json Is Nothing Then
                    '没有数据就跳过执行下一次循环
                    Continue For
                End If

                Dim failed_data As New Submit_failed_data With {.SQL = json("SQL").ToString(),
                    .GetErrString = json("GetErrString").ToString(),
                    .GetStackTrace = json("GetStackTrace").ToString(),
                    .Sign = json("Sign").ToString(),
                    .GetretryCount = json("GetretryCount").ToString()}

                'Dim t As New System.Text.StringBuilder
                't.AppendLine(failed_data.Sign)
                't.AppendLine(failed_data.SQL)
                't.AppendLine(failed_data.GetErrString)
                't.AppendLine(failed_data.GetStackTrace)
                'MessageBox.Show(t.ToString, IO.Path.GetFileNameWithoutExtension(mFileInfo), MessageBoxButtons.OK, MessageBoxIcon.Asterisk)

                failed_data.GetretryCount += 1 '重试次数加1,当累计到指定数后就放弃数据

                Dim cmd As DBCommand = New DBCommand(New ConnectionString().ConnectionInfo)
                Select Case failed_data.Sign
                    Case 1
                        '插入记录
                        Error_record.Geterr("", "") '初始化错误捕获信息
                        Dim sql As String = failed_data.SQL
                        If cmd.Insert(sql) > 0 Then
                            Log("失败数据重提交->插入成功|" & sql)
                            '更新成功!
                        Else
                            Log("失败数据重提交->插入失败|" & sql)
                            '更新失败的话就记录信息,后面忘了正常以后再更新
                            Dim failed_data1 As New Submit_failed_data With {.Sign = 1,
                                        .SQL = sql,
                                        .GetErrString = Error_record.GetErrString,
                                        .GetStackTrace = Error_record.GetStackTrace,
                                        .GetretryCount = failed_data.GetretryCount}
                            If failed_data1.GetretryCount <= T_Retry_Count Then
                                Info_uploading.Journal(failed_data1)
                            End If
                        End If
                    Case 0
                        '修改记录
                        Error_record.Geterr("", "") '初始化错误捕获信息
                        Dim sql As String = failed_data.SQL
                        If cmd.Update(sql) > 0 Then
                            Log("失败数据重提交->更新成功" & sql)
                            '更新成功!
                        Else
                            Log("失败数据重提交->更新失败" & sql)
                            Dim failed_data2 As New Submit_failed_data With {.Sign = 0,
                                .SQL = sql,
                                .GetErrString = Error_record.GetErrString,
                                .GetStackTrace = Error_record.GetStackTrace,
                                .GetretryCount = failed_data.GetretryCount}

                            If failed_data2.GetretryCount <= T_Retry_Count Then
                                Info_uploading.Journal(failed_data2)
                            End If
                        End If

                End Select

                Threading.Thread.Sleep(1000) '延迟50毫秒

                If IO.File.Exists(mFileInfo) = True Then
                    '处理完毕后删除记录
                    Try
                        IO.File.Delete(mFileInfo)
                    Catch ex As Exception
                    End Try
                End If

            Catch ex As Exception
                Log("重新加载失败的SQL语句 " & ex.StackTrace)
            End Try
        Next
        IsBol = False
    End Sub


    ''' <summary>
    ''' 读取文本内容
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Text_read(ByVal path As String) As String
        If IO.File.Exists(path) = True Then
            Dim wz As New StreamReader(path, System.Text.Encoding.GetEncoding("GB2312"))
            Dim text As New System.Text.StringBuilder
            Do While wz.Peek >= 0
                Dim str As String = wz.ReadLine.ToString
                text.AppendLine(str)
            Loop
            wz.Close()
            Return text.ToString
        Else
            Return String.Empty
        End If
    End Function

    ''' <summary>
    ''' 遍历文件夹下面的全部文件
    ''' </summary>
    ''' <param name="sSourcePath">文件夹</param>
    ''' <param name="sType">类型 例如 *.*</param>
    ''' <returns></returns>
    Public Shared Function FindFile(ByVal sSourcePath As String, ByVal sType As String) As List(Of String)
        Dim list As List(Of String) = New List(Of String)()
        Dim theFolder As DirectoryInfo = New DirectoryInfo(sSourcePath)
        Dim thefileInfo As FileInfo() = theFolder.GetFiles(sType, SearchOption.TopDirectoryOnly)

        For Each NextFile As FileInfo In thefileInfo
            Try
                list.Add(NextFile.FullName)
            Catch ex As Exception
            End Try
        Next

        Dim dirInfo As DirectoryInfo() = theFolder.GetDirectories()

        For Each NextFolder As DirectoryInfo In dirInfo
            Try
                Dim fileInfo As FileInfo() = NextFolder.GetFiles("*.*", SearchOption.AllDirectories)
                For Each NextFile As FileInfo In fileInfo
                    Try
                        list.Add(NextFile.FullName)
                    Catch ex As Exception
                    End Try
                Next
            Catch ex As Exception
            End Try
        Next

        Return list
    End Function

End Class
