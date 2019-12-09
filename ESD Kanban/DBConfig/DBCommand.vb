Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Web
Imports System.Data

Namespace DBConfig

    Public Class DBCommand
        Inherits sqlConnection

        '�ڹ��캯����ָ��������Ϣ�ַ���
        Public Sub New(ByVal str As String)
            ConnStr = str
        End Sub

        ''' <summary>
        ''' �������ݵ�SQL���ݿ����
        ''' </summary>
        ''' <param name="strSQL">SQL����</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Insert(ByVal strSQL As String) As Integer
            Try
                '�������ݿ�
                Open()

                '����SqlCommandʵ��
#If OLEDB Then
            Dim cmd As OleDbCommand = New OleDbCommand(strSQL, conn)
#Else
                Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)
#End If
                'Log("sql[�������ݵ�SQL���ݿ����]    >   " & strSQL)
                'count��ʾ��Ӱ�����������ʼ��Ϊ0
                Dim count As Integer = 0

                'cmd.CommandTimeout = 5
                'ִ��SQL����
                count = cmd.ExecuteNonQuery()

                '�ر����ݿ�
                Close()
                Return count
            Catch ex As Exception
                Error_record.Geterr(ex.Message, ex.StackTrace)
                Close()
            End Try
        End Function

        ''' <summary>
        ''' ɾ��SQL���ݿ���е�����
        ''' </summary>
        ''' <param name="table">������</param>
        ''' <param name="row">����</param>
        ''' <param name="value">ֵ</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal table As String, ByVal row As String, ByVal value As String) As Integer
            Dim strSQL As String = String.Empty
            Try


                '�������ݿ�
                Open()

                '����SQL����
                strSQL = "Delete From " + table + " Where " + row + "=" + value
                '����SqlCommandʵ��
#If OLEDB Then
            Dim cmd As OleDbCommand = New OleDbCommand(strSQL, conn)
#Else
                Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)
#End If
                'Log("sql[ɾ��SQL���ݿ���е�����]    >   " & strSQL)
                'count��ʾ��Ӱ�����������ʼ��Ϊ0
                Dim count As Integer = 0

                'cmd.CommandTimeout = 5
                'ִ��SQL����
                count = cmd.ExecuteNonQuery()

                '�ر����ݿ�
                Close()

                Return count
            Catch ex As Exception
                Error_record.Geterr(ex.Message, ex.StackTrace)
                Close()
                'MessageBox.Show(ex.Message.ToString, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End Try
        End Function

        ''' <summary>
        ''' ����SQL���ݿ���е�����
        ''' </summary>
        ''' <param name="table">������</param>
        ''' <param name="strContent">���ݿ������ַ���</param>
        ''' <param name="row">����</param>
        ''' <param name="value">ֵ</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Update(ByVal table As String, ByVal strContent As String, ByVal row As String, ByVal value As String) As Integer
            Dim strSQL As String = String.Empty
            Try
                '�������ݿ�
                Open()

                '����SQL����
                strSQL = "Update " + table + " Set " + strContent + " Where " + row + "=" + value

                '����SqlCommandʵ��
#If OLEDB Then
            Dim cmd As OleDbCommand = New OleDbCommand(strSQL, conn)
#Else
                Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)
#End If
                'Log("sql[����SQL���ݿ���е�����]    >   " & strSQL)
                'count��ʾ��Ӱ�����������ʼ��Ϊ0
                Dim count As Integer = 0

                'cmd.CommandTimeout = 5
                'ִ��SQL����
                count = cmd.ExecuteNonQuery()

                '�ر����ݿ�
                Close()

                Return count
            Catch ex As Exception
                Error_record.Geterr(ex.Message, ex.StackTrace)
                Close()
                'MessageBox.Show(ex.Message.ToString, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End Try
        End Function

        ''' <summary>
        ''' ����SQL���ݿ���е�����
        ''' </summary>
        ''' <param name="sql">sql���</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Update(ByVal sql As String) As Integer
            Dim strSQL As String = String.Empty
            Try
                '�������ݿ�
                Open()

                '����SQL����
                strSQL = sql

                '����SqlCommandʵ��
#If OLEDB Then
            Dim cmd As OleDbCommand = New OleDbCommand(strSQL, conn)
#Else
                Dim cmd As SqlCommand = New SqlCommand(strSQL, conn)
#End If
                'Log("sql[����SQL���ݿ���е�����]    >   " & strSQL)
                'count��ʾ��Ӱ�����������ʼ��Ϊ0
                Dim count As Integer = 0

                'cmd.CommandTimeout = 5
                'ִ��SQL����
                count = cmd.ExecuteNonQuery()

                '�ر����ݿ�
                Close()

                Return count
            Catch ex As Exception
                Error_record.Geterr(ex.Message, ex.StackTrace)
                Close()
                'MessageBox.Show(ex.Message.ToString, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End Try
        End Function

    End Class

End Namespace
