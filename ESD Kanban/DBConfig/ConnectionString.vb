Namespace DBConfig
    Public Class ConnectionString
        ''' <summary>
        ''' SQL��¼����
        ''' </summary>
        ''' <remarks></remarks>
        Dim pass As String = "1qaz!QAZ"
        ''' <summary>
        ''' ��������ַ
        ''' </summary>
        ''' <remarks></remarks>
        Dim Comname As String = "10.124.149.29"
        ''' <summary>
        ''' ���ݿ������
        ''' </summary>
        ''' <remarks></remarks>
        Dim Bname As String = "tab_ESD_MonitorDB"
        ''' <summary>
        ''' Ĭ��ini�����ļ�����
        ''' </summary>
        ''' <remarks></remarks>
        Dim ini_name As String = "Parameter.ini" 'ini�����ļ�������
        ''' <summary>
        ''' ini�����ļ����λ��
        ''' </summary>
        ''' <remarks></remarks>
        Dim Cpath As String = My.Application.Info.DirectoryPath & "\" & ini_name '
        Public ConnectionInfo As String = info()
        ''' <summary>
        ''' ���������ļ��е�SQL��¼����
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function info() As String
            On Error Resume Next
            Dim _Comname As String = GetINI("Connect", "server", "", Cpath) '����������
            Dim _Bname As String = GetINI("Connect", "Table", "", Cpath) '������
            If _Comname <> "" And _Bname <> "" Then
                '�ļ����ھͶ�ȡ�ļ�����
                Comname = GetINI("Connect", "server", "", Cpath) '����������
                Bname = GetINI("Connect", "Table", "", Cpath) '������
            Else
                '�ļ������ھ�д��Ĭ��ֵ
                WriteINI("Connect", "server", Comname, Cpath)
                WriteINI("Connect", "Table", Bname, Cpath)
            End If

            Comname = "10.124.149.29"

            Dim _ConnectionInfo As String =
        "Data Source=" & Comname & ";Initial Catalog=" & Bname & ";" &
     "Persist Security Info=True;User ID=sa;Password=" & pass
            'Log("sql[�����ļ��е�SQL��¼����]    >   " & _ConnectionInfo)
            Return _ConnectionInfo
        End Function
    End Class
End Namespace
