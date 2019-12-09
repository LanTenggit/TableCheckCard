Imports ESD_Kanban.DBConfig
Imports System.Threading

Public Class from_Squad_leader_selection

    Private Sub from_Squad_leader_selection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        t_Refresh()
    End Sub

    ''' <summary>
    ''' 刷新
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub t_Refresh()
        'Panel2.Controls.Clear() '清空控件
        Select Case t_state
            Case t_Pattern_selection.班长选择
                Squad_leader_selection()
            Case t_Pattern_selection.线体选择
                'Dim t_Line_selection As New task(Sub() Line_selection())
                't_Line_selection.Start()
                Line_selection()
        End Select
    End Sub

    ''' <summary>
    ''' 班长选择
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Squad_leader_selection()
        'buttarr.Clear()
        'Me.Text = "班长选择"
        'Dim butt1 As New Button
        'butt1.Text = "未选择"
        'butt1.BackColor = Color.Goldenrod
        'butt1.Name = "buttx"
        'butt1.AutoSize = True
        'butt1.Size = New Point(200, 20)
        'butt1.Font = New Font("微软雅黑", 12.0F, FontStyle.Regular)
        'AddHandler butt1.Click, AddressOf Button_Click

        'buttarr.Add(butt1)

        Dim X As Integer = 0 '行
        Dim Y As Integer = 0
        Dim sql As String = "select t_name as 姓名 from t_Monitor_info  where t_name is not null order by t_Work_number desc"
        Dim t_name As List(Of Object()) = Data_query(sql, New ConnectionString().ConnectionInfo)
        If t_name Is Nothing Then
            Exit Sub
        End If

        ListView1.BeginUpdate()
        ListView1.Items.Clear()
        ListView1.View = View.LargeIcon
        'ImageList1.ImageSize = New Point(90, 75)
        ListView1.LargeImageList = ImageList1
        ListView1.Scrollable = True

        Dim lvi1 As ListViewItem = New ListViewItem()
        lvi1.ImageIndex = 0
        lvi1.Text = "未选择"
        ListView1.Items.Add(lvi1)

        For Each T In t_name
            If T.Count = 1 Then
                '        Dim butt As New Button
                '        butt.Text = T(0).Trim
                '        If String.Compare(t_Monitor.Trim, butt.Text, False) = 0 Then
                '            butt.BackColor = Color.LawnGreen
                '        End If
                '        butt.Name = "butt"
                '        butt.AutoSize = True
                '        butt.Size = New Point(200, 20)
                '        X += 1
                '        If X = 4 Then
                '            X = 0
                '            Y += 1
                '        End If
                '        butt.Location = New Point(210 * X, 35 * Y) '按钮排布 一行4个  一列一列显示出来
                '        buttarr.Add(butt)
                '        butt.Font = New Font("微软雅黑", 12.0F, FontStyle.Regular)
                '        AddHandler butt.Click, AddressOf Button_Click
                '        'Dim t1 As New t_addbutt(AddressOf addbutt)
                '        'Me.Invoke(t1, buttarr)

                Dim lvi As ListViewItem = New ListViewItem()
                lvi.ImageIndex = 0
                If T(0).ToString.Trim = t_Monitor Then
                    lvi.ImageIndex = 3
                End If
                lvi.Text = T(0).Trim
                ListView1.Items.Add(lvi)
            End If
        Next
        'addbutt(buttarr)
        'buttarr.Clear()

        ListView1.EndUpdate()

    End Sub

    ''' <summary>
    ''' 线体选择
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Line_selection()
        buttarr.Clear()
        Me.Text = "线体选择"
        Dim butt1 As New Button
        butt1.Text = "未选择"
        butt1.BackColor = Color.Goldenrod
        butt1.Name = "buttx"
        butt1.AutoSize = True
        butt1.Size = New Point(200, 20)
        butt1.Font = New Font("微软雅黑", 12.0F, FontStyle.Regular)
        AddHandler butt1.Click, AddressOf Button_Click

        buttarr.Add(butt1)

        Dim X As Integer = 0 '行
        Dim Y As Integer = 0
        Dim sql As String = "select t_line_name from tab_info_Repair where t_type = '1' group by t_line_name"
        Dim t_name As List(Of Object()) = Data_query(sql, "Data Source=10.114.113.10;Initial Catalog=PMS;Persist Security Info=True;User ID=sa;Password=")
        If t_name Is Nothing Then
            Exit Sub
        End If

        ListView1.BeginUpdate()
        ListView1.Items.Clear()
        ListView1.View = View.LargeIcon
        'ImageList1.ImageSize = New Point(100, 75)
        ListView1.LargeImageList = ImageList1
        ListView1.Scrollable = True

        Dim lvi1 As ListViewItem = New ListViewItem()
        lvi1.ImageIndex = 1
        lvi1.Text = "未选择"
        ListView1.Items.Add(lvi1)

        For Each T In t_name
            If T.Count = 1 Then
                'Dim butt As New Button
                'butt.Text = T(0).Trim
                'If String.Compare(t_Monitor.Trim, butt.Text, False) = 0 Then
                '    butt.BackColor = Color.LawnGreen
                'End If
                'butt.Name = "butt"
                'butt.AutoSize = True
                'butt.Size = New Point(200, 20)
                'X += 1
                'If X = 4 Then
                '    X = 0
                '    Y += 1
                'End If
                'butt.Location = New Point(210 * X, 35 * Y) '按钮排布 一行4个  一列一列显示出来
                'buttarr.Add(butt)
                'butt.Font = New Font("微软雅黑", 12.0F, FontStyle.Regular)
                'AddHandler butt.Click, AddressOf Button_Click
                'Dim t1 As New t_addbutt(AddressOf addbutt)
                'Me.Invoke(t1, buttarr)

                Dim lvi As ListViewItem = New ListViewItem()
                lvi.ImageIndex = 1
                lvi.Text = T(0).Trim

                If T(0).ToString.Trim = t_Line_body Then
                    lvi.ImageIndex = 2
                End If


                ListView1.Items.Add(lvi)
            End If
        Next
        'addbutt(buttarr)
        'buttarr.Clear()
        ListView1.EndUpdate()
    End Sub

    Private buttarr As New List(Of Button)

    Private Delegate Sub t_addbutt(ByVal butt As List(Of Button))
    Private Sub addbutt(ByVal butt As List(Of Button))
        If butt.Count < 0 Then Exit Sub
        Dim tasks = New Task(butt.Count - 1) {}
        For t = 0 To butt.Count - 1
            Dim Bu As New Button
            Bu = butt(t)
            'log(t & " butt.Count>" & butt.Count & "  name>" & Bu.Text)
            tasks(t) = Task.Run(Sub()
                                    Dim t1 As New t_addbut(AddressOf addbut)
                                    Me.BeginInvoke(t1, Bu)
                                    'Invoke 同步调用委托
                                    'BeginInvoke 异步调用委托
                                End Sub)
        Next
    End Sub

    Private Delegate Sub t_addbut(ByVal butt As Button)

    Private Sub addbut(ByVal butt As Button)
        'Panel2.Controls.Add(butt)
    End Sub


    ''' <summary>
    ''' 按钮事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button_Click(sender As Object, e As EventArgs)
        Select Case t_state
            Case t_Pattern_selection.班长选择
                Dim butt As Button = sender
                t_Monitor = butt.Text.Trim
            Case t_Pattern_selection.线体选择
                Dim butt As Button = sender
                t_Line_body = butt.Text.Trim
        End Select
        Me.Close() '关闭窗体
    End Sub

    Dim itm As ListViewItem
    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        Try
            itm = ListView1.Items(ListView1.SelectedIndices(0))
            Select Case t_state
                Case t_Pattern_selection.班长选择
                    t_Monitor = itm.SubItems(0).Text.Trim
                Case t_Pattern_selection.线体选择
                    t_Line_body = itm.SubItems(0).Text.Trim
            End Select
            Me.Close() '关闭窗体
        Catch ex As Exception
        End Try
    End Sub
End Class


