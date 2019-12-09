Imports System.IO.Ports

''' <summary>
''' 设置
''' </summary>
''' <remarks></remarks>
Public Class from_Setting

    Private Sub from_Setting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "设置"

        TextBox1.Text = ESD_NUM_Left
        FlowLayoutPanel1.Controls.Clear()

        For i = 1 To Val(ESD_NUM_Left)
            Dim t_ESD_Agreement As New ESD_Agreement
            t_ESD_Agreement.Name = "ESD" & i.ToString("0000")
            t_ESD_Agreement.get_ESD_NUM = i.ToString("0000")
            t_ESD_Agreement.ESD_Choice = ESD_Agreement.model.Left '定义左边采集器
            FlowLayoutPanel1.Controls.Add(t_ESD_Agreement)
        Next

        TextBox8.Text = ESD_NUM_right
        FlowLayoutPanel2.Controls.Clear()

        For i = 1 To Val(ESD_NUM_right)
            Dim t_ESD_Agreement As New ESD_Agreement
            t_ESD_Agreement.Name = "ESD" & i.ToString("0000")
            t_ESD_Agreement.get_ESD_NUM = i.ToString("0000")
            t_ESD_Agreement.ESD_Choice = ESD_Agreement.model.right '定义左边采集器
            FlowLayoutPanel2.Controls.Add(t_ESD_Agreement)
        Next


        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        TextBox4.Enabled = False
        TextBox8.Enabled = False

        TextBox2.Text = Scanning_interval
        TextBox3.Text = Scanning_timeout
        TextBox4.Text = Alarm_detection

    End Sub

    Dim bc_bol As Boolean = False
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Select Case bc_bol
            Case False
                bc_bol = True
                TextBox1.Enabled = True
                Button1.Text = "保存"
            Case True
                Dim t_ESD_NUM As String = TextBox1.Text.Trim
                If String.Equals(t_ESD_NUM, String.Empty) = True And IsNumeric(t_ESD_NUM) = True Then
                    MessageBox.Show("保存失败,请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Exit Sub
                End If
                ESD_NUM_Left = TextBox1.Text
                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                bc_bol = False
                TextBox1.Enabled = False
                Button1.Text = "编辑"
        End Select
    End Sub

    Dim e_bol As Boolean = False
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Select Case e_bol
            Case False
                e_bol = True
                Button2.Text = "保存"
                TextBox2.Enabled = True
                TextBox3.Enabled = True
                TextBox4.Enabled = True

            Case True
                If String.Equals(TextBox2.Text.Trim, String.Empty) = True And IsNumeric(TextBox2.Text.Trim) = True Then
                    MessageBox.Show("扫描间隔时间保存失败,请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Exit Sub
                End If
                If String.Equals(TextBox3.Text.Trim, String.Empty) = True And IsNumeric(TextBox3.Text.Trim) = True Then
                    MessageBox.Show("超时时间保存失败,请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Exit Sub
                End If

                If String.Equals(TextBox4.Text.Trim, String.Empty) = True And IsNumeric(TextBox4.Text.Trim) = True Then
                    MessageBox.Show("警报检测时间保存失败,请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Exit Sub
                End If

                Try
                    '扫描间隔必须大于或等于1000毫秒
                    If Int(TextBox2.Text.Trim) < 1000 Then
                        TextBox2.Text = 1000
                    End If
                Catch ex As Exception
                End Try

                Scanning_interval = TextBox2.Text.Trim
                Scanning_timeout = TextBox3.Text.Trim
                Alarm_detection = TextBox4.Text.Trim
                MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                e_bol = False
                Button2.Text = "编辑"

                TextBox2.Enabled = False
                TextBox3.Enabled = False
                TextBox4.Enabled = False

        End Select
    End Sub

    Dim bc_bol_R As Boolean = False
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Select Case bc_bol_R
            Case False
                bc_bol_R = True
                TextBox8.Enabled = True
                Button4.Text = "保存"
            Case True
                Dim t_ESD_NUM As String = TextBox8.Text.Trim
                If String.Equals(t_ESD_NUM, String.Empty) = True And IsNumeric(t_ESD_NUM) = True Then
                    MessageBox.Show("保存失败,请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Exit Sub
                End If
                ESD_NUM_right = TextBox8.Text
                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                bc_bol_R = False
                TextBox8.Enabled = False
                Button4.Text = "编辑"
        End Select
    End Sub
End Class