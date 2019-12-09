Imports System.Data.SqlClient
Imports System.Data.OleDb

Namespace DBConfig

    Public Class sqlConnection

        ''' <summary>
        ''' 声明一个受保护变量存储连接数据库的信息
        ''' </summary>
        ''' <remarks></remarks>
        Protected ConnStr As String


#If OLEDB Then
           ''' <summary>
        ''' 声明用于数据库连接的私有成员
        ''' </summary>
        ''' <remarks></remarks>
        Protected conn As OleDbConnection
#Else
        Protected conn As Data.SqlClient.SqlConnection
#End If
        ''' <summary>
        ''' 打开数据库
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub Open()
            Try
                '判断连接字符串是否为空
                If ConnStr Is Nothing Or ConnStr = "" Then
                    MsgBox("连接字符串为指定，请指定连接字符串")
                    Return
                End If

                '实例化SqlConnection类
#If OLEDB Then
            conn = New OleDbConnection(ConnStr)
#Else
                conn = New Data.SqlClient.SqlConnection(ConnStr)
#End If
                '打开数据库
                conn.Open()

            Catch ex As Exception
            End Try

        End Sub

        ''' <summary>
        ''' 关闭连接
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub Close()
            '
            conn.Close()
        End Sub

    End Class

End Namespace

