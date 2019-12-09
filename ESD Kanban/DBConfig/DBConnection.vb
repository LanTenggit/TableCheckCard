Imports System.Data.SqlClient
Imports System.Data.OleDb

Namespace DBConfig

    Public Class sqlConnection

        ''' <summary>
        ''' ����һ���ܱ��������洢�������ݿ����Ϣ
        ''' </summary>
        ''' <remarks></remarks>
        Protected ConnStr As String


#If OLEDB Then
           ''' <summary>
        ''' �����������ݿ����ӵ�˽�г�Ա
        ''' </summary>
        ''' <remarks></remarks>
        Protected conn As OleDbConnection
#Else
        Protected conn As Data.SqlClient.SqlConnection
#End If
        ''' <summary>
        ''' �����ݿ�
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub Open()
            Try
                '�ж������ַ����Ƿ�Ϊ��
                If ConnStr Is Nothing Or ConnStr = "" Then
                    MsgBox("�����ַ���Ϊָ������ָ�������ַ���")
                    Return
                End If

                'ʵ����SqlConnection��
#If OLEDB Then
            conn = New OleDbConnection(ConnStr)
#Else
                conn = New Data.SqlClient.SqlConnection(ConnStr)
#End If
                '�����ݿ�
                conn.Open()

            Catch ex As Exception
            End Try

        End Sub

        ''' <summary>
        ''' �ر�����
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub Close()
            '
            conn.Close()
        End Sub

    End Class

End Namespace

