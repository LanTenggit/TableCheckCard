Public Class DeviceSet

    Public num As Int32

    Dim DevicenumClick As Boolean = False
    Dim Device As Boolean = False
    Dim Cpath As String = My.Application.Info.DirectoryPath & "\" & "Parameter.ini"
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim form1 As Form = New MainFrom()

        'FlowLayoutPanel1.Controls.Clear()
        bn_click(Button2, DevicenumClick)
        DevicenumClick = bn_click(Button2, DevicenumClick)

        If DevicenumClick Then

            Me.TextBox2.Enabled = True
            Me.TextBox3.Enabled = True
            Me.TextBox4.Enabled = True

        Else

            Me.TextBox2.Enabled = False
            Me.TextBox3.Enabled = False
            Me.TextBox4.Enabled = False

        End If

        num = Convert.ToInt32(Me.TextBox4.Text)
        Quantity_equipment = num.ToString
        Dim NumSave As String = Model_set.GetINI("Device", "num", "", Cpath)
        Dim SaveNum As Integer = Convert.ToInt32(NumSave)

        If num > SaveNum Then

            For index = SaveNum + 1 To num

                Dim UD As New User_Device With {
                                .Name = "Device" & index.ToString,
                                .DNumber = index.ToString
                            }
                FlowLayoutPanel1.Controls.Add(UD)
            Next
        Else
            For index = num + 1 To SaveNum

                FlowLayoutPanel1.Controls.RemoveAt(num)

                Model_set.WriteINI("Device", "Device" + index.ToString, "", Cpath)




            Next

            Me.TextBox4.Text = num

        End If


        'Model_set.WriteINI("Device", "Num", num.ToString, Cpath)
    End Sub

    Private Sub DeviceSet_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MainFrom.IsUpdate = False
        FlowLayoutPanel1.Controls.Clear()
        Me.TextBox2.Enabled = False
        Me.TextBox3.Enabled = False
        Me.TextBox4.Enabled = False
        If TextBox4.Text = String.Empty Then
            Me.TextBox4.Text = num
        End If
        Me.TextBox4.Text = Model_set.GetINI("Device", "Num", num.ToString, Cpath)
        num = Model_set.GetINI("Device", "Num", num.ToString, Cpath)
        'AddDevice(num)

        Try
            For i = 1 To num

                Dim DeviceValue As String = Model_set.GetINI("Device", "Device" + i.ToString, "", Cpath)
                Dim DeviceList As String() = DeviceValue.Split(",")
                Dim UD As New User_Device With {
                      .Name = DeviceList(1) & i.ToString,
                      .DeviceName = DeviceList(1),
                      .DNumber = DeviceList(0),
                      .Adress = DeviceList(2),
                      .COMName = DeviceList(3)
                }
                FlowLayoutPanel1.Controls.Add(UD)
            Next
        Catch ex As Exception

        End Try






    End Sub

    ''' <summary>
    ''' button点击事件
    ''' </summary>

    Private Function bn_click(but As Button, Isclick As Boolean) As Boolean

        '第一次点击
        If Isclick = False Then
            but.Text = "保存"
            Isclick = True
            Return True
        Else

            Isclick = False
            but.Text = "编辑"
            Return False
        End If
    End Function

    Private Sub AddDevice(Num As Integer)
        If Num > 0 Then
            For i = 1 To Num

                Dim UD As New User_Device With {
                    .Name = "UD" & i.ToString,
                    .DNumber = i.ToString}
                FlowLayoutPanel1.Controls.Add(UD)
            Next
        End If

    End Sub

    Private Sub DeviceSet_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        MainFrom.IsUpdate = True
    End Sub
End Class