Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb

Namespace DBConfig

    Public Class DBDataTable
        Inherits sqlConnection

        ''' <summary>
        ''' �ڹ��캯����ָ��������Ϣ�ַ���
        ''' </summary>
        ''' <param name="str"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal str As String)
            ConnStr = str
        End Sub

        Public Function CreateDataTable(ByVal strSQL As String, ByVal table As String) As DataTable
            Try
                '�������ݿ�
                Open()

                'ʹ�������ַ�����SqlConnection����SqlDataAdapter��ʵ��
#If OLEDB Then
            Dim da As OleDbDataAdapter = New OleDbDataAdapter(strSQL, conn)
#Else
                Dim da As SqlDataAdapter = New SqlDataAdapter(strSQL, conn)
#End If

                '����DataSet����
                Dim ds As New Data.DataSet()
                '���DataSet
                da.Fill(ds)
                '�ر����ݿ�
                Close()
                '����DataTable
                Return ds.Tables(0)
            Catch ex As Exception
            End Try
        End Function
    End Class

End Namespace
