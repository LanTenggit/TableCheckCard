<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MTTR
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ZedGraphControl1 = New ZedGraph.ZedGraphControl()
        Me.SuspendLayout()
        '
        'ZedGraphControl1
        '
        Me.ZedGraphControl1.Location = New System.Drawing.Point(12, 35)
        Me.ZedGraphControl1.Name = "ZedGraphControl1"
        Me.ZedGraphControl1.ScrollGrace = 0R
        Me.ZedGraphControl1.ScrollMaxX = 0R
        Me.ZedGraphControl1.ScrollMaxY = 0R
        Me.ZedGraphControl1.ScrollMaxY2 = 0R
        Me.ZedGraphControl1.ScrollMinX = 0R
        Me.ZedGraphControl1.ScrollMinY = 0R
        Me.ZedGraphControl1.ScrollMinY2 = 0R
        Me.ZedGraphControl1.Size = New System.Drawing.Size(1163, 512)
        Me.ZedGraphControl1.TabIndex = 0
        '
        'MTTR
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1187, 626)
        Me.Controls.Add(Me.ZedGraphControl1)
        Me.Name = "MTTR"
        Me.Text = "MTTR"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ZedGraphControl1 As ZedGraph.ZedGraphControl
End Class
