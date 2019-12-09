Imports System.Data.SqlClient
Imports System.Data.OleDb

Namespace DBConfig

    Public Class DBDataReader
        Inherits sqlConnection

        ''' <summary>
        ''' 在构造函数中指定连接信息字符串
        ''' </summary>
        ''' <param name="str"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal str As String)
            ConnStr = str
        End Sub

#If OLEDB Then
        ''' <summary>
        ''' 打开数据库连接返回DataReader
        ''' </summary>
        ''' <param name="strSQL"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateDataReader(ByVal strSQL As String) As OleDbDataReader

            '打开数据库连接
            Open()

            '创建OleDbCommand的对象
            Dim cmd As OleDbCommand = New OleDbCommand()

            'ExecuteReader执行SQL语句并返回OleDbDataReader
            Dim dr As OleDbDataReader = cmd.ExecuteReader()

            '返回DataReader
            Return dr

        End Function
#Else
        Public Function CreateDataReader(ByVal strSQL As String) As SqlDataReader

            '打开数据库连接
            Open()

            '创建SqlCommand的对象
            Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)

            'ExecuteReader执行SQL语句并返回SqlDataReader
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            '返回DataReader
            Return dr

        End Function
#End If

    End Class

End Namespace
