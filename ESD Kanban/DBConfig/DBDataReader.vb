Imports System.Data.SqlClient
Imports System.Data.OleDb

Namespace DBConfig

    Public Class DBDataReader
        Inherits sqlConnection

        ''' <summary>
        ''' �ڹ��캯����ָ��������Ϣ�ַ���
        ''' </summary>
        ''' <param name="str"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal str As String)
            ConnStr = str
        End Sub

#If OLEDB Then
        ''' <summary>
        ''' �����ݿ����ӷ���DataReader
        ''' </summary>
        ''' <param name="strSQL"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateDataReader(ByVal strSQL As String) As OleDbDataReader

            '�����ݿ�����
            Open()

            '����OleDbCommand�Ķ���
            Dim cmd As OleDbCommand = New OleDbCommand()

            'ExecuteReaderִ��SQL��䲢����OleDbDataReader
            Dim dr As OleDbDataReader = cmd.ExecuteReader()

            '����DataReader
            Return dr

        End Function
#Else
        Public Function CreateDataReader(ByVal strSQL As String) As SqlDataReader

            '�����ݿ�����
            Open()

            '����SqlCommand�Ķ���
            Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)

            'ExecuteReaderִ��SQL��䲢����SqlDataReader
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            '����DataReader
            Return dr

        End Function
#End If

    End Class

End Namespace
