Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Web
Imports System.Data

Namespace DBConfig

    Public Class DBCommand
        Inherits sqlConnection

        '在构造函数中指定连接信息字符串
        Public Sub New(ByVal str As String)
            ConnStr = str
        End Sub

        ''' <summary>
        ''' 插入数据到SQL数据库表中
        ''' </summary>
        ''' <param name="strSQL">SQL代码</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Insert(ByVal strSQL As String) As Integer
            Try
                '连接数据库
                Open()

                '创建SqlCommand实例
#If OLEDB Then
            Dim cmd As OleDbCommand = New OleDbCommand(strSQL, conn)
#Else
                Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)
#End If
                'Log("sql[插入数据到SQL数据库表中]    >   " & strSQL)
                'count表示受影响的行数，初始化为0
                Dim count As Integer = 0

                'cmd.CommandTimeout = 5
                '执行SQL命令
                count = cmd.ExecuteNonQuery()

                '关闭数据库
                Close()
                Return count
            Catch ex As Exception
                Error_record.Geterr(ex.Message, ex.StackTrace)
                Close()
            End Try
        End Function

        ''' <summary>
        ''' 删除SQL数据库表中的数据
        ''' </summary>
        ''' <param name="table">表名称</param>
        ''' <param name="row">条件</param>
        ''' <param name="value">值</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal table As String, ByVal row As String, ByVal value As String) As Integer
            Dim strSQL As String = String.Empty
            Try


                '连接数据库
                Open()

                '创建SQL命令
                strSQL = "Delete From " + table + " Where " + row + "=" + value
                '创建SqlCommand实例
#If OLEDB Then
            Dim cmd As OleDbCommand = New OleDbCommand(strSQL, conn)
#Else
                Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)
#End If
                'Log("sql[删除SQL数据库表中的数据]    >   " & strSQL)
                'count表示受影响的行数，初始化为0
                Dim count As Integer = 0

                'cmd.CommandTimeout = 5
                '执行SQL命令
                count = cmd.ExecuteNonQuery()

                '关闭数据库
                Close()

                Return count
            Catch ex As Exception
                Error_record.Geterr(ex.Message, ex.StackTrace)
                Close()
                'MessageBox.Show(ex.Message.ToString, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End Try
        End Function

        ''' <summary>
        ''' 更新SQL数据库表中的数据
        ''' </summary>
        ''' <param name="table">表名称</param>
        ''' <param name="strContent">数据库连接字符串</param>
        ''' <param name="row">条件</param>
        ''' <param name="value">值</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Update(ByVal table As String, ByVal strContent As String, ByVal row As String, ByVal value As String) As Integer
            Dim strSQL As String = String.Empty
            Try
                '连接数据库
                Open()

                '创建SQL命令
                strSQL = "Update " + table + " Set " + strContent + " Where " + row + "=" + value

                '创建SqlCommand实例
#If OLEDB Then
            Dim cmd As OleDbCommand = New OleDbCommand(strSQL, conn)
#Else
                Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)
#End If
                'Log("sql[更新SQL数据库表中的数据]    >   " & strSQL)
                'count表示受影响的行数，初始化为0
                Dim count As Integer = 0

                'cmd.CommandTimeout = 5
                '执行SQL命令
                count = cmd.ExecuteNonQuery()

                '关闭数据库
                Close()

                Return count
            Catch ex As Exception
                Error_record.Geterr(ex.Message, ex.StackTrace)
                Close()
                'MessageBox.Show(ex.Message.ToString, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End Try
        End Function

        ''' <summary>
        ''' 更新SQL数据库表中的数据
        ''' </summary>
        ''' <param name="sql">sql语句</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Update(ByVal sql As String) As Integer
            Dim strSQL As String = String.Empty
            Try
                '连接数据库
                Open()

                '创建SQL命令
                strSQL = sql

                '创建SqlCommand实例
#If OLEDB Then
            Dim cmd As OleDbCommand = New OleDbCommand(strSQL, conn)
#Else
                Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)
#End If
                'Log("sql[更新SQL数据库表中的数据]    >   " & strSQL)
                'count表示受影响的行数，初始化为0
                Dim count As Integer = 0

                'cmd.CommandTimeout = 5
                '执行SQL命令
                count = cmd.ExecuteNonQuery()

                '关闭数据库
                Close()

                Return count
            Catch ex As Exception
                Error_record.Geterr(ex.Message, ex.StackTrace)
                Close()
                'MessageBox.Show(ex.Message.ToString, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End Try
        End Function

    End Class

End Namespace
