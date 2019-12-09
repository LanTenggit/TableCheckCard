Imports System.Data

Namespace DBConfig

    Public Class DataBinding

        ''' <summary>
        ''' 用指定表的指定列的值填充ComboBox控件
        ''' </summary>
        ''' <param name="combo">ComboBox控件</param>
        ''' <param name="tableName">表名称</param>
        ''' <param name="column">数量</param>
        ''' <param name="connStr">SQL连接字符串</param>
        ''' <param name="field">条件1</param>
        ''' <param name="op">条件2</param>
        ''' <param name="value">条件3</param>
        ''' <remarks></remarks>
        Public Shared Sub BindComboBox(ByRef combo As ComboBox, ByVal tableName As String, _
                                        ByVal column As String, ByVal connStr As String, _
                                        Optional ByVal field As String = "", _
                                        Optional ByVal op As String = "", _
                                        Optional ByVal value As String = "")
            Dim SQLStr As String = String.Empty
            Try
                '创建DBDataTable对象
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '设置SQL语句
                SQLStr = "Select distinct " & column & " from " & tableName
                '如果指定了查询条件，则把查询条件追加到SQL语句
                If Not field = "" Then
                    SQLStr += " Where " & field & " " & op & " " & value
                End If
                'log("sql    >   " & SQLStr)
                '调用DBDataTable的CreateDataTable函数，得到DataTable表
                Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
                '指定ComboBox显示DataTable的哪一列
                combo.DisplayMember = column
                '指定DataTable为ComboBox的数据源
                combo.DataSource = table
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>
        ''' 生成指定表的Index字段的值，显示在TextBox上
        ''' </summary>
        ''' <param name="txt"></param>
        ''' <param name="tableName"></param>
        ''' <param name="Column"></param>
        ''' <param name="connStr"></param>
        ''' <remarks></remarks>
        Public Shared Sub FillIndexTextBox(ByRef txt As TextBox, ByVal tableName As String, ByVal Column As String, _
                                        ByVal connStr As String)

            '创建DBDataTable对象
            Dim dt As DBDataTable = New DBDataTable(connStr)
            '设置SQL语句
            Dim SQLStr As String = "Select TOP 1 * from " & tableName & "  ORDER BY " & Column & " DESC"
            '调用DBDataTable的CreateDataTable函数，得到DataTable表
            'log("sql[生成指定表的Index字段的值，显示在TextBox上]    >   " & SQLStr)
            Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
            '把最大的Index的值加一后在TextBox上显示出来
            txt.Text = CType(Integer.Parse(table.Rows(0).Item(Column)) + 1, String)

        End Sub

        ''' <summary>
        ''' 显示根据指定表的指定列的值，包含该值的记录由条件子句决定
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
                '创建DBDataTable对象
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '设置SQL语句
                SQLStr = "Select " & Column & " from " & tableName & " WHERE " & field & " = " & value
                '调用DBDataTable的CreateDataTable函数，得到DataTable表
                'log("sql[示根据指定表的指定列的值，包含该值的记录由条件子句决定]    >   " & SQLStr)
                Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
                '显示查询得到的首行（一般也只有一行）指定列的值
                If table.Rows.Count = 0 Then Exit Sub '如果等于0代表没有数据返回
                txt.Text = table.Rows(0).Item(Column).ToString().Trim()
            Catch ex As Exception
                txt.Text = String.Empty
            End Try
        End Sub

        ''' <summary>
        ''' 根据指定的查询条件得到一条查询记录，设置ComboBox被选中项为该条记录某列的值
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
                '创建DBDataTable对象
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '设置SQL语句
                SQLStr = "Select " & Column & " from " & tableName & " WHERE " & field & " = " & value
                '调用DBDataTable的CreateDataTable函数，得到DataTable表
                'Log("sql[根据指定的查询条件得到一条查询记录，设置ComboBox被选中项为该条记录某列的值]    >   " & SQLStr)
                Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
                If table.Rows.Count = 0 Then Exit Sub '如果等于0代表没有数据返回
                '得到指定字段在ComboBox中的序号
                Dim nIndex = combo.FindStringExact(table.Rows(0).Item(Column))
                '设置ComboBox被选中的项
                combo.SelectedIndex = nIndex
            Catch ex As Exception
            End Try

        End Sub

        ''' <summary>
        ''' 将数据库数据绑定到DataGridView表中
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
                '创建DBDataTable对象
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '设置SQL语句
                'Dim SQLStr As String = "Select " & Column & " from " & tableName & " WHERE " & field & " " & op & " " & value
                '调用DBDataTable的CreateDataTable函数，得到DataTable表
                'log("sql[将数据库数据绑定到DataGridView表中]    >   " & SQLStr)
                Dim table As DataTable = dt.CreateDataTable(SQLStr, tableName)
                Grid.DataSource = table
                Return table
            Catch ex As Exception
            End Try
        End Function



        ''' <summary>
        ''' 清空DataGridView
        ''' </summary>
        ''' <param name="dgv">控件名称 如: DataGridView</param>
        ''' <remarks></remarks>
        '''
        Public Shared Sub DataGridViewReset(ByVal dgv As DataGridView)
            Try
                '若DataGridView绑定的数据源为DataTable
                If dgv.DataSource.GetType.ToString = GetType(DataTable).ToString Then

                    Dim dt As DataTable = dgv.DataSource
                    dt.Clear()
                End If
                '若DataGridView绑定的数据源为BindingSource
                If dgv.DataSource.GetType.ToString = GetType(BindingSource).ToString Then
                    Dim bs As BindingSource = dgv.DataSource
                    Dim dt As DataTable = bs.DataSource
                    dt.Clear()
                End If
            Catch ex As Exception
            End Try
        End Sub




        ''' <summary>
        ''' 根据指定表和指定查询条件填充ListView
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
                '清空ListView
                lsv.Items.Clear()
                '设置SQL语句
                SQLString = "SELECT " & liename & " FROM " & tableName
                '如果指定了查询条件，则把查询条件追加到SQL语句
                If field <> "" Then
                    SQLString += " Where " & field & op & value
                End If
                'SQLString += "  order by t_Line_body"
                'log("sql[根据指定表和指定查询条件填充ListView]    >   " & SQLString)
                '创建DBDataTable对象
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '调用DBDataTable的CreateDataTable函数，得到DataTable表
                Dim table As DataTable = dt.CreateDataTable(SQLString, tableName)

                '在循环中遍历DataTable表，逐行逐列把表中的内容加入到ListView控件中
                Dim UserRow As DataRow
                Dim LItem As ListViewItem
                '遍历每一行
                For Each UserRow In table.Rows
                    LItem = New ListViewItem(UserRow(0).ToString())
                    Dim i As Integer
                    '遍历一行中的所有列
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
                '清空ListView
                lsv.Items.Clear()

                'log("sql[根据指定表和指定查询条件填充ListView]    >   " & SQLString)
                '创建DBDataTable对象
                Dim dt As DBDataTable = New DBDataTable(connStr)
                '调用DBDataTable的CreateDataTable函数，得到DataTable表
                Dim table As DataTable = dt.CreateDataTable(SQLString, Nothing)

                '在循环中遍历DataTable表，逐行逐列把表中的内容加入到ListView控件中
                Dim UserRow As DataRow
                Dim LItem As ListViewItem
                '遍历每一行
                For Each UserRow In table.Rows
                    LItem = New ListViewItem(UserRow(0).ToString())
                    Dim i As Integer
                    '遍历一行中的所有列
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
