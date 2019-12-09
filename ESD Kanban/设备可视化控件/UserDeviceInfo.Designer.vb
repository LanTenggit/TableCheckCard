<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UserDeviceInfo
    Inherits System.Windows.Forms.UserControl

    'UserControl 重写释放以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UserDeviceInfo))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Lb_DPower = New System.Windows.Forms.Label()
        Me.Lb_DTime = New System.Windows.Forms.Label()
        Me.Lb_DBedNum = New System.Windows.Forms.Label()
        Me.Lb_DYieds = New System.Windows.Forms.Label()
        Me.lb_DName = New System.Windows.Forms.Label()
        Me.Lb_DAdrees = New System.Windows.Forms.Label()
        Me.label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pb_01 = New System.Windows.Forms.PictureBox()
        Me.Panel2.SuspendLayout()
        CType(Me.pb_01, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "灯泡(红色).png")
        Me.ImageList1.Images.SetKeyName(1, "灯泡(灰色).png")
        Me.ImageList1.Images.SetKeyName(2, "灯泡(绿色).png")
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Lb_DPower)
        Me.Panel2.Controls.Add(Me.Lb_DTime)
        Me.Panel2.Controls.Add(Me.Lb_DBedNum)
        Me.Panel2.Controls.Add(Me.Lb_DYieds)
        Me.Panel2.Controls.Add(Me.lb_DName)
        Me.Panel2.Controls.Add(Me.Lb_DAdrees)
        Me.Panel2.Controls.Add(Me.label3)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.pb_01)
        Me.Panel2.Location = New System.Drawing.Point(0, 2)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(156, 253)
        Me.Panel2.TabIndex = 3
        '
        'Lb_DPower
        '
        Me.Lb_DPower.AutoSize = True
        Me.Lb_DPower.Location = New System.Drawing.Point(18, 236)
        Me.Lb_DPower.Name = "Lb_DPower"
        Me.Lb_DPower.Size = New System.Drawing.Size(53, 12)
        Me.Lb_DPower.TabIndex = 4
        Me.Lb_DPower.Text = "压   力:"
        '
        'Lb_DTime
        '
        Me.Lb_DTime.AutoSize = True
        Me.Lb_DTime.Location = New System.Drawing.Point(19, 222)
        Me.Lb_DTime.Name = "Lb_DTime"
        Me.Lb_DTime.Size = New System.Drawing.Size(53, 12)
        Me.Lb_DTime.TabIndex = 5
        Me.Lb_DTime.Text = "时   间:"
        '
        'Lb_DBedNum
        '
        Me.Lb_DBedNum.AutoSize = True
        Me.Lb_DBedNum.Location = New System.Drawing.Point(18, 206)
        Me.Lb_DBedNum.Name = "Lb_DBedNum"
        Me.Lb_DBedNum.Size = New System.Drawing.Size(65, 12)
        Me.Lb_DBedNum.TabIndex = 3
        Me.Lb_DBedNum.Text = "不 良 数："
        '
        'Lb_DYieds
        '
        Me.Lb_DYieds.AutoSize = True
        Me.Lb_DYieds.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Lb_DYieds.Location = New System.Drawing.Point(74, 189)
        Me.Lb_DYieds.Name = "Lb_DYieds"
        Me.Lb_DYieds.Size = New System.Drawing.Size(29, 12)
        Me.Lb_DYieds.TabIndex = 2
        Me.Lb_DYieds.Text = "产量"
        '
        'lb_DName
        '
        Me.lb_DName.AutoSize = True
        Me.lb_DName.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.lb_DName.Location = New System.Drawing.Point(73, 157)
        Me.lb_DName.Name = "lb_DName"
        Me.lb_DName.Size = New System.Drawing.Size(29, 12)
        Me.lb_DName.TabIndex = 0
        Me.lb_DName.Text = "名称"
        '
        'Lb_DAdrees
        '
        Me.Lb_DAdrees.AutoSize = True
        Me.Lb_DAdrees.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Lb_DAdrees.Location = New System.Drawing.Point(73, 172)
        Me.Lb_DAdrees.Name = "Lb_DAdrees"
        Me.Lb_DAdrees.Size = New System.Drawing.Size(29, 12)
        Me.Lb_DAdrees.TabIndex = 1
        Me.Lb_DAdrees.Text = "地址"
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.label3.Location = New System.Drawing.Point(19, 189)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(59, 12)
        Me.label3.TabIndex = 9
        Me.label3.Text = "产    量:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label2.Location = New System.Drawing.Point(17, 174)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(59, 12)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "设备地址:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label1.Location = New System.Drawing.Point(17, 157)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(59, 12)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "设备名称:"
        '
        'pb_01
        '
        Me.pb_01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pb_01.Image = Global.ESD_Kanban.My.Resources.Resources.灯泡_灰色_
        Me.pb_01.Location = New System.Drawing.Point(2, 0)
        Me.pb_01.Name = "pb_01"
        Me.pb_01.Size = New System.Drawing.Size(152, 150)
        Me.pb_01.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pb_01.TabIndex = 6
        Me.pb_01.TabStop = False
        '
        'UserDeviceInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.Panel2)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Name = "UserDeviceInfo"
        Me.Size = New System.Drawing.Size(158, 258)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.pb_01, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Lb_DBedNum As Label
    Friend WithEvents Lb_DYieds As Label
    Friend WithEvents Lb_DAdrees As Label
    Friend WithEvents lb_DName As Label
    Friend WithEvents Lb_DTime As Label
    Friend WithEvents Lb_DPower As Label
    Friend WithEvents pb_01 As PictureBox
    Friend WithEvents label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
End Class
