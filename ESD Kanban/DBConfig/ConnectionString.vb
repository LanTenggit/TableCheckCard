Namespace DBConfig
    Public Class ConnectionString
        ''' <summary>
        ''' SQL登录密码
        ''' </summary>
        ''' <remarks></remarks>
        Dim pass As String = "1qaz!QAZ"
        ''' <summary>
        ''' 服务器地址
        ''' </summary>
        ''' <remarks></remarks>
        Dim Comname As String = "10.124.149.29"
        ''' <summary>
        ''' 数据库表名称
        ''' </summary>
        ''' <remarks></remarks>
        Dim Bname As String = "tab_ESD_MonitorDB"
        ''' <summary>
        ''' 默认ini配置文件名称
        ''' </summary>
        ''' <remarks></remarks>
        Dim ini_name As String = "Parameter.ini" 'ini配置文件的名称
        ''' <summary>
        ''' ini配置文件输出位置
        ''' </summary>
        ''' <remarks></remarks>
        Dim Cpath As String = My.Application.Info.DirectoryPath & "\" & ini_name '
        Public ConnectionInfo As String = info()
        ''' <summary>
        ''' 返回配置文件中的SQL登录数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function info() As String
            On Error Resume Next
            Dim _Comname As String = GetINI("Connect", "server", "", Cpath) '服务器名称
            Dim _Bname As String = GetINI("Connect", "Table", "", Cpath) '表名称
            If _Comname <> "" And _Bname <> "" Then
                '文件存在就读取文件内容
                Comname = GetINI("Connect", "server", "", Cpath) '服务器名称
                Bname = GetINI("Connect", "Table", "", Cpath) '表名称
            Else
                '文件不存在就写入默认值
                WriteINI("Connect", "server", Comname, Cpath)
                WriteINI("Connect", "Table", Bname, Cpath)
            End If

            Comname = "10.124.149.29"

            Dim _ConnectionInfo As String =
        "Data Source=" & Comname & ";Initial Catalog=" & Bname & ";" &
     "Persist Security Info=True;User ID=sa;Password=" & pass
            'Log("sql[配置文件中的SQL登录数据]    >   " & _ConnectionInfo)
            Return _ConnectionInfo
        End Function
    End Class
End Namespace
