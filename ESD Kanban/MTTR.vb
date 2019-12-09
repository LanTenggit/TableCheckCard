Imports ESD_Kanban.DBConfig
Imports ZedGraph

Public Class MTTR




    Private Sub MTTR_Load(sender As Object, e As EventArgs) Handles MyBase.Load



        Try
            Dim sql As String = String.Empty
            sql = "select top 10 * from Device"

            'sql = "exec dbo.PressureValue '2018-11-30','2018-11-30','BE46','10 0B'"


            Me.Invoke(New Action(Sub()
                                     'ToolStripLabel14.Text = "图表设备地址:" & equipment
                                 End Sub))

            Debug.Print("sql>" & sql)

            Dim db As List(Of Object()) = Data_query(sql, New ConnectionString().ConnectionInfo)

            If db IsNot Nothing Then
                If db.Count = 0 Then
                    'Select_device_count = 65 '立即刷新控件列表
                Else
                    '绘图
                    CreateGraph_GradientByZBars(ZedGraphControl1,
                                                Now.Month & "月" & Now.Day &
                                                " 设备MTTR",
                                                db, 0.7, 5, 10)

                End If
            Else
                'Select_device_count = 65 '立即刷新控件列表
            End If

        Catch ex As Exception
            Error_record.Geterr("绘制曲线图表线程>" & ex.Message, ex.StackTrace)
        End Try

    End Sub

    '''' <summary>
    '''' 柱子图,异常次数日看板
    '''' </summary>
    '''' <param name="z1">图表控件名称</param>
    '''' <param name="Title_name">标题名称</param>
    '''' <param name="getdic">数据源</param>
    '''' <param name="displacement">标签偏移量</param>
    '''' <param name="MAX">坐标轴增加点数</param>
    '''' <param name="XFontSpec">X左边字体大小</param>
    '''' <remarks></remarks>
    Private Sub CreateGraph_GradientByZBars(ByVal z1 As ZedGraph.ZedGraphControl,'图表控件名称
                                            ByVal Title_name As String,'标题名称
                                            ByVal getdic As List(Of Object()),'数据源
                                            ByVal displacement As Integer,'标签偏移量
                                            ByVal MAX As Integer,'坐标轴增加点数
                                            ByVal XFontSpec As Integer'X左边字体大小
                                            )

        Dim myPane As ZedGraph.GraphPane = z1.GraphPane
        myPane.CurveList.Clear()
        myPane.GraphObjList.Clear()
        myPane.Title.Text = Title_name
        myPane.XAxis.Title.Text = "机型"
        myPane.YAxis.Title.Text = "压力"
        'myPane.Y2Axis.Title.Text = "保压时间"

        Dim MaximumPressure As New ZedGraph.PointPairList '压力最大值 折线

        Dim limit As New ZedGraph.PointPairList '上限
        Dim lower_limit As New ZedGraph.PointPairList '下限



        Dim Sum_values As Integer = 0 '合计数量
        'Dim MeanPressure As New ZedGraph.PointPairList() '折线
        Dim x_val As New List(Of Double)
        Dim t_Max As Double = 0 '最大值
        Dim Y2_MAX As Double = 0 'Y2坐标最大值
        Dim Y2_MIN As Double = 9999 '最小值

        Dim y2max As Double = 0 'Y2保压时间最大值

        Dim list As New ZedGraph.PointPairList()


        For i = 0 To getdic.Count - 1
            Dim Address As String = getdic(i)(5).ToString '设备编号
            Dim values As Double = getdic(i)(9).ToString '压力平均值
            Dim values2 As Double = 0
            Try
                values2 = getdic(i)(9).ToString '压力最大值

                If Y2_MAX <= values2 Then
                    Y2_MAX = values2
                End If
                If Y2_MIN >= getdic(i)(9) Then
                    Y2_MIN = getdic(i)(9).ToString
                End If
            Catch ex As Exception
                Debug.Print("柱子图>" & ex.Message & ex.StackTrace.ToString)
            End Try
            Dim x_start As Double  'X轴时间
            list.Add(x_start, CDbl(values), CDbl(values))
            MaximumPressure.Add(x_start, CDbl(getdic(i)(9)),
                                "机型: " & Address & vbCrLf &
                                "最大压力值: " & getdic(i)(9) & "/N")
            Sum_values += CDbl(values)
            x_val.Add(CDbl(values))
            If t_Max <= values Then
                t_Max = values
            End If

            limit.Add(x_start, CDbl(getdic(3)(9) + 2))

            lower_limit.Add(x_start, CDbl(getdic(0)(9) - 2))

        Next
        ZedGraph.BarItem.CreateBarLabels(myPane, False, "0")
        '压力最大值
        Dim myCurvea_MaximumPressure = myPane.AddCurve("最大压力", MaximumPressure, Color.Blue, ZedGraph.SymbolType.Circle) '画折线 
        '定义折线线条粗度
        myCurvea_MaximumPressure.Symbol.Size = 4.0F
        myCurvea_MaximumPressure.Symbol.Fill = New ZedGraph.Fill(Color.White)
        myCurvea_MaximumPressure.Line.Width = 2.0F
        myCurvea_MaximumPressure.IsY2Axis = False '手动改为按【Y2Axis】的刻度描画 
        myCurvea_MaximumPressure.Line.IsSmooth = True '曲线平滑
        myCurvea_MaximumPressure.Symbol.Fill = New ZedGraph.Fill(Color.Blue) '点的颜色
        '压力最大值
        For i = 0 To list.Count - 1
            Dim pt As ZedGraph.PointPair = myCurvea_MaximumPressure.Points(i)
            Dim text As ZedGraph.TextObj = New ZedGraph.TextObj(pt.Y.ToString("f2"),
                                                                pt.X + i + 1,
                                                                pt.Y + displacement + 0.1,
                                                                ZedGraph.CoordType.AxisXYScale,
                                                                ZedGraph.AlignH.Left - 1,
                                                                ZedGraph.AlignV.Center) With {
                .ZOrder = ZedGraph.ZOrder.A_InFront
                                                                }
            If Convert.ToDouble(text.Text) > CDbl(getdic(i)(9).ToString) Then
                text.FontSpec.FontColor = Color.Red
            End If
            '// 隐藏标注的边框和填充
            text.FontSpec.Border.IsVisible = False
            text.FontSpec.Fill.IsVisible = False
            '// 选择标注字体90°
            text.FontSpec.Angle = 0
            text.Text = text.Text & "/N" '柱子上标记字符
            myPane.GraphObjList.Add(text)
        Next


        '压力上限
        Dim myCurvea_limit = myPane.AddCurve("上限", limit, Color.Crimson, ZedGraph.SymbolType.Circle) '画折线 
        '定义折线线条粗度
        myCurvea_limit.Symbol.Size = 4.0F
        myCurvea_limit.Symbol.Fill = New ZedGraph.Fill(Color.White)
        myCurvea_limit.Line.Width = 2.0F
        myCurvea_limit.IsY2Axis = False '手动改为按【Y2Axis】的刻度描画 
        myCurvea_limit.Line.IsSmooth = True '曲线平滑
        myCurvea_limit.Symbol.Fill = New Fill(Color.Crimson) '点的颜色
        '压力上限
        For i = 0 To list.Count - 1
            Dim pt As ZedGraph.PointPair = myCurvea_limit.Points(i)
            Dim text As ZedGraph.TextObj = New ZedGraph.TextObj(pt.Y.ToString("f2"),
                                                                pt.X + i + 1,
                                                                pt.Y + displacement,
                                                                ZedGraph.CoordType.AxisXYScale,
                                                                ZedGraph.AlignH.Left - 1,
                                                                ZedGraph.AlignV.Center) With {
                .ZOrder = ZedGraph.ZOrder.A_InFront
                                                                }
            If Convert.ToDouble(text.Text) < CDbl(getdic(i)(9).ToString) Then
                text.FontSpec.FontColor = Color.Red
            Else
                '// 隐藏标注的边框和填充
                text.FontSpec.Border.IsVisible = False
                text.FontSpec.Fill.IsVisible = False
            End If
            '// 选择标注字体90°
            text.FontSpec.Angle = 0
            text.Text = "" '柱子上标记字符
            myPane.GraphObjList.Add(text)
        Next



        '压力下限
        Dim myCurvea_lower_limit = myPane.AddCurve("下限", lower_limit, Color.Crimson, ZedGraph.SymbolType.Circle) '画折线 
        '定义折线线条粗度
        myCurvea_lower_limit.Symbol.Size = 4.0F
        myCurvea_lower_limit.Symbol.Fill = New ZedGraph.Fill(Color.White)
        myCurvea_lower_limit.Line.Width = 2.0F
        myCurvea_lower_limit.IsY2Axis = False '手动改为按【Y2Axis】的刻度描画 
        myCurvea_lower_limit.Line.IsSmooth = True '曲线平滑
        myCurvea_lower_limit.Symbol.Fill = New Fill(Color.Crimson) '点的颜色
        '压力下限
        For i = 0 To list.Count - 1
            Dim pt As ZedGraph.PointPair = myCurvea_lower_limit.Points(i)
            Dim text As ZedGraph.TextObj = New ZedGraph.TextObj(pt.Y.ToString("f2"),
                                                                pt.X + i + 1,
                                                                pt.Y + displacement,
                                                                ZedGraph.CoordType.AxisXYScale,
                                                                ZedGraph.AlignH.Left - 1,
                                                                ZedGraph.AlignV.Center) With {
                .ZOrder = ZedGraph.ZOrder.A_InFront
                                                                }
            If Convert.ToDouble(text.Text) < CDbl(getdic(i)(9).ToString) Then
                text.FontSpec.FontColor = Color.Red
            Else
                '// 隐藏标注的边框和填充
                text.FontSpec.Border.IsVisible = False
                text.FontSpec.Fill.IsVisible = False
            End If
            '// 选择标注字体90°
            text.FontSpec.Angle = 0
            text.Text = ""  '柱子上标记字符
            myPane.GraphObjList.Add(text)
        Next

        myPane.Chart.Fill = New ZedGraph.Fill(Color.White, Color.FromArgb(255, Color.ForestGreen), 90.0F)
        myPane.XAxis.MajorGrid.IsVisible = True
        myPane.Legend.Position = ZedGraph.LegendPos.Top '标题注释的位置
        myPane.Legend.FontSpec.Size = 10.0F
        myPane.Legend.FontSpec.FontColor = Color.Black '标题注释格子内文字的颜色
        myPane.Legend.IsHStack = True
        myPane.XAxis.Type = ZedGraph.AxisType.Text
        Dim PairList_new As New List(Of String)
        'For i = 0 To getdic.Count - 1
        '    PairList_new.Add(Format(Convert.ToDateTime(getdic(i)(0).ToString), "HH:mm"))
        'Next
        For i = 1 To 10
            PairList_new.Add(i * 10)
        Next
        myPane.XAxis.Scale.TextLabels = PairList_new.ToArray '设置坐标名称
        myPane.XAxis.Scale.MajorStep = 1
        myPane.Legend.IsVisible = True
        myPane.XAxis.CrossAuto = True
        myPane.Legend.FontSpec.Size = 9 '标签的字体大小
        myPane.XAxis.Scale.Min = 0 '刷新图表后,图表数据靠左出现位置
        myPane.XAxis.Scale.FontSpec.Size = XFontSpec 'X坐标字体大小
        myPane.XAxis.Scale.FontSpec.Angle = 90 '旋转X左边文字的角度
        myPane.XAxis.MajorGrid.IsVisible = True
        myPane.YAxis.MajorGrid.IsVisible = True
        'myPane.YAxis.Scale.Max = Double.Parse(10) '定义网格线的密集程度
        'Make the Y axis scale red
        myPane.YAxis.Scale.FontSpec.FontColor = Color.Black
        myPane.YAxis.Title.FontSpec.FontColor = Color.Black
        'turn off the opposite tics so the Y tics don't show up on the Y2 axis
        myPane.YAxis.MajorTic.IsOpposite = False
        'Don() 't display the Y zero line
        myPane.YAxis.MajorGrid.IsZeroLine = False
        'Align the Y axis labels so they are flush to the axis
        myPane.YAxis.Scale.Align = ZedGraph.AlignP.Inside
        myPane.YAxis.Scale.Min = Y2_MIN - 3 ' 0
        myPane.YAxis.Scale.Max = t_Max + MAX 'Y轴最大值 
        myPane.YAxis.Scale.FontSpec.Size = 9 '标签的字体大小
        myPane.YAxis.Scale.FormatAuto = True

        'Y2坐标
        ' Enable the Y2 axis display
        myPane.Y2Axis.IsVisible = True
        ' Make the Y2 axis scale blue
        myPane.Y2Axis.Scale.FontSpec.FontColor = Color.Black
        myPane.Y2Axis.Title.FontSpec.FontColor = Color.Black
        ' turn off the opposite tics so the Y2 tics don't show up on the Y axis
        myPane.Y2Axis.MajorTic.IsOpposite = False
        myPane.Y2Axis.Scale.Align = ZedGraph.AlignP.Inside
        myPane.Y2Axis.Scale.Min = 0
        myPane.Y2Axis.Scale.Max = y2max + 5
        'myPane.Y2Axis.Scale.FormatAuto = True
        myPane.Y2Axis.Scale.FontSpec.Size = 9
        'Fill the axis background with a gradient
        myPane.Chart.Fill = New ZedGraph.Fill(Color.White, Color.White, 40.0F) '图表背景颜色
        z1.IsShowPointValues = True '鼠标经过图表上的点时是否气泡显示该点所对应的值。默认为false
        z1.AxisChange() '修改图表
        z1.Refresh()
    End Sub




End Class