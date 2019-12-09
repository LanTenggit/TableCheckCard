Imports System.Data

Namespace DBConfig

    Public Class DataBinding

        ''' <summary>
        ''' ��ָ�����ָ���е�ֵ���ComboBox�ؼ�
        ''' </summary>
        ''' <param name="combo">ComboBox�ؼ�</param>
        ''' <param name="tableName">������</param>
        ''' <param name="column">����</param>
        ''' <param name="connStr">SQL�����ַ���</param>
        ''' <param name="field">����1</param>
        ''' <param name="op">����2</param>
        ''' <param name="value">����3</param>
        ''' <remarks></remarks>
        Public Shared Sub BindComboBox(ByRef combo As ComboBox, ByVal tableName As String, _
                                        ByVal column As String, ByVal connStr As String, _
                                        Optional ByVal field As String = "", _
                                        Optional ByVal op As String = "", _
                                        Optional ByVal value As String = "")
            Dim SQLStr As String = String.Empty
            Try
                '����DBDataTable����
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '����SQL���
                SQLStr = "Select distinct " & column & " from " & tableName
                '���ָ���˲�ѯ��������Ѳ�ѯ����׷�ӵ�SQL���
                If Not field = "" Then
                    SQLStr += " Where " & field & " " & op & " " & value
                End If
                'log("sql    >   " & SQLStr)
                '����DBDataTable��CreateDataTable�������õ�DataTable��
                Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
                'ָ��ComboBox��ʾDataTable����һ��
                combo.DisplayMember = column
                'ָ��DataTableΪComboBox������Դ
                combo.DataSource = table
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>
        ''' ����ָ�����Index�ֶε�ֵ����ʾ��TextBox��
        ''' </summary>
        ''' <param name="txt"></param>
        ''' <param name="tableName"></param>
        ''' <param name="Column"></param>
        ''' <param name="connStr"></param>
        ''' <remarks></remarks>
        Public Shared Sub FillIndexTextBox(ByRef txt As TextBox, ByVal tableName As String, ByVal Column As String, _
                                        ByVal connStr As String)

            '����DBDataTable����
            Dim dt As DBDataTable = New DBDataTable(connStr)
            '����SQL���
            Dim SQLStr As String = "Select TOP 1 * from " & tableName & "  ORDER BY " & Column & " DESC"
            '����DBDataTable��CreateDataTable�������õ�DataTable��
            'log("sql[����ָ�����Index�ֶε�ֵ����ʾ��TextBox��]    >   " & SQLStr)
            Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
            '������Index��ֵ��һ����TextBox����ʾ����
            txt.Text = CType(Integer.Parse(table.Rows(0).Item(Column)) + 1, String)

        End Sub

        ''' <summary>
        ''' ��ʾ����ָ�����ָ���е�ֵ��������ֵ�ļ�¼�������Ӿ����
        ''' </summary>
        ''' <param name="txt"></param>
        ''' <param name="tableName"></param>
        ''' <param name="Column"></param>
        ''' <param name="connStr"></param>
        ''' <param name="field"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Shared Sub FillTextBox(ByRef txt As TextBox, ByVal tableName As String, ByVal Column As String, _
                                        ByVal connStr As String, ByVal field As String, ByVal value As String)
            Dim SQLStr As String = String.Empty
            Try
                '����DBDataTable����
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '����SQL���
                SQLStr = "Select " & Column & " from " & tableName & " WHERE " & field & " = " & value
                '����DBDataTable��CreateDataTable�������õ�DataTable��
                'log("sql[ʾ����ָ�����ָ���е�ֵ��������ֵ�ļ�¼�������Ӿ����]    >   " & SQLStr)
                Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
                '��ʾ��ѯ�õ������У�һ��Ҳֻ��һ�У�ָ���е�ֵ
                If table.Rows.Count = 0 Then Exit Sub '�������0����û�����ݷ���
                txt.Text = table.Rows(0).Item(Column).ToString().Trim()
            Catch ex As Exception
                txt.Text = String.Empty
            End Try
        End Sub

        ''' <summary>
        ''' ����ָ���Ĳ�ѯ�����õ�һ����ѯ��¼������ComboBox��ѡ����Ϊ������¼ĳ�е�ֵ
        ''' </summary>
        ''' <param name="combo"></param>
        ''' <param name="tableName"></param>
        ''' <param name="Column"></param>
        ''' <param name="connStr"></param>
        ''' <param name="field"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Shared Sub SetComboSelectedIndex(ByRef combo As ComboBox, ByVal tableName As String, ByVal Column As String, _
                             ByVal connStr As String, ByVal field As String, ByVal value As String)
            Dim SQLStr As String = String.Empty
            Try
                '����DBDataTable����
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '����SQL���
                SQLStr = "Select " & Column & " from " & tableName & " WHERE " & field & " = " & value
                '����DBDataTable��CreateDataTable�������õ�DataTable��
                'Log("sql[����ָ���Ĳ�ѯ�����õ�һ����ѯ��¼������ComboBox��ѡ����Ϊ������¼ĳ�е�ֵ]    >   " & SQLStr)
                Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
                If table.Rows.Count = 0 Then Exit Sub '�������0����û�����ݷ���
                '�õ�ָ���ֶ���ComboBox�е����
                Dim nIndex = combo.FindStringExact(table.Rows(0).Item(Column))
                '����ComboBox��ѡ�е���
                combo.SelectedIndex = nIndex
            Catch ex As Exception
            End Try

        End Sub

        ''' <summary>
        ''' �����ݿ����ݰ󶨵�DataGridView����
        ''' </summary>
        ''' <param name="Grid"></param>
        ''' <param name="tableName"></param>
        ''' <param name="connStr"></param>
        ''' <param name="SQLStr"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SetDataGridView(ByRef Grid As DataGridView, ByVal tableName As String, ByVal connStr As String, _
                            ByVal SQLStr As String) As DataTable
            Try
                '����DBDataTable����
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '����SQL���
                'Dim SQLStr As String = "Select " & Column & " from " & tableName & " WHERE " & field & " " & op & " " & value
                '����DBDataTable��CreateDataTable�������õ�DataTable��
                'log("sql[�����ݿ����ݰ󶨵�DataGridView����]    >   " & SQLStr)
                Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
                Grid.DataSource = table
                Return table
            Catch ex As Exception
            End Try
        End Function



        ''' <summary>
        ''' ���DataGridView
        ''' </summary>
        ''' <param name="dgv">�ؼ����� ��: DataGridView</param>
        ''' <remarks></remarks>
        '''
        Public Shared Sub DataGridViewReset(ByVal dgv As DataGridView)
            Try
                '��DataGridView�󶨵�����ԴΪDataTable
                If dgv.DataSource.GetType.ToString = GetType(DataTable).ToString Then

                    Dim dt As DataTable = dgv.DataSource
                    dt.Clear()
                End If
                '��DataGridView�󶨵�����ԴΪBindingSource
                If dgv.DataSource.GetType.ToString = GetType(BindingSource).ToString Then
                    Dim bs As BindingSource = dgv.DataSource
                    Dim dt As DataTable = bs.DataSource
                    dt.Clear()
                End If
            Catch ex As Exception
            End Try
        End Sub




        ''' <summary>
        ''' ����ָ�����ָ����ѯ�������ListView
        ''' </summary>
        ''' <param name="lsv"></param>
        ''' <param name="tableName"></param>
        ''' <param name="liename"></param>
        ''' <param name="num"></param>
        ''' <param name="connStr"></param>
        ''' <param name="field"></param>
        ''' <param name="op"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Overloads Shared Sub FillListView(ByRef lsv As ListView, ByVal tableName As String, ByVal liename As String, ByVal num As Integer, _
                                        ByVal connStr As String, Optional ByVal field As String = "", _
                                        Optional ByVal op As String = " = ", Optional ByVal value As String = "")
            Dim SQLString As String = String.Empty
            Try
                '���ListView
                lsv.Items.Clear()
                '����SQL���
                SQLString = "SELECT " & liename & " FROM " & tableName
                '���ָ���˲�ѯ��������Ѳ�ѯ����׷�ӵ�SQL���
                If field <> "" Then
                    SQLString += " Where " & field & op & value
                End If
                'SQLString += "  order by t_Line_body"
                'log("sql[����ָ�����ָ����ѯ�������ListView]    >   " & SQLString)
                '����DBDataTable����
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '����DBDataTable��CreateDataTable�������õ�DataTable��
                Dim table As DataTable = dt.CreateDataTable(SQLString, tableName)

                '��ѭ���б���DataTable���������аѱ��е����ݼ��뵽ListView�ؼ���
                Dim UserRow As DataRow
                Dim LItem As ListViewItem
                '����ÿһ��
                For Each UserRow In table.Rows
                    LItem = New ListViewItem(UserRow(0).ToString())
                    Dim i As Integer
                    '����һ���е�������
                    For i = 1 To num - 1
                        Dim a As String = lsv.Name
                        Dim text As String = UserRow(i).ToString.Trim
                        If text = "" Then
                            LItem.SubItems.Add(" ")
                        Else
                            LItem.SubItems.Add(UserRow(i).ToString.Trim)
                        End If
                    Next
                    lsv.Items.Add(LItem)
                Next
            Catch ex As Exception
            End Try
        End Sub

        Public Overloads Shared Sub FillListView(ByRef lsv As ListView, ByVal SQLString As String, ByVal num As Integer, _
                                       ByVal connStr As String, Optional ByVal field As String = "", _
                                       Optional ByVal op As String = " = ", Optional ByVal value As String = "")
            Try
                '���ListView
                lsv.Items.Clear()

                'log("sql[����ָ�����ָ����ѯ�������ListView]    >   " & SQLString)
                '����DBDataTable����
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '����DBDataTable��CreateDataTable�������õ�DataTable��
                Dim table As DataTable = dt.CreateDataTable(SQLString, Nothing)

                '��ѭ���б���DataTable���������аѱ��е����ݼ��뵽ListView�ؼ���
                Dim UserRow As DataRow
                Dim LItem As ListViewItem
                '����ÿһ��
                For Each UserRow In table.Rows
                    LItem = New ListViewItem(UserRow(0).ToString())
                    Dim i As Integer
                    '����һ���е�������
                    For i = 1 To num - 1
                        Dim a As String = lsv.Name
                        Dim text As String = UserRow(i).ToString.Trim
                        If text = "" Then
                            LItem.SubItems.Add(" ")
                        Else
                            LItem.SubItems.Add(UserRow(i).ToString.Trim)
                        End If
                    Next
                    lsv.Items.Add(LItem)
                Next
            Catch ex As Exception
            End Try
        End Sub
    End Class

End Namespace
