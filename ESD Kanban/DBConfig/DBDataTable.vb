Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb

Namespace DBConfig

    Public Class DBDataTable
        Inherits sqlConnection

        ''' <summary>
        ''' 在构造函数中指定连接信息字符串
        ''' </summary>
        ''' <param name="str"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal str As String)
            ConnStr = str
        End Sub

        Public Function CreateDataTable(ByVal strSQL As String, ByVal table As String) As DataTable
            Try
                '连接数据库
                Open()

                '使用连接字符串和SqlConnection创建SqlDataAdapter的实例
#If OLEDB Then
            Dim da As OleDbDataAdapter = New OleDbDataAdapter(strSQL, conn)
#Else
                Dim da As SqlDataAdapter = New SqlDataAdapter(strSQL, conn)
#End If

                '创建DataSet对象
                Dim ds As New Data.DataSet()
                '填充DataSet
                da.Fill(ds)
                '关闭数据库
                Close()
                '返回DataTable
                Return ds.Tables(0)
            Catch ex As Exception
            End Try
        End Function
    End Class

End Namespace
