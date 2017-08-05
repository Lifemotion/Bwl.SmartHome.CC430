<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CC430ControlApp

    'Форма переопределяет dispose для очистки списка компонентов.
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

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CC430ControlApp))
        Me.lbDrivers = New System.Windows.Forms.ListBox()
        Me.lbDevices = New System.Windows.Forms.ListBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tStates = New System.Windows.Forms.Timer(Me.components)
        Me.tPoll = New System.Windows.Forms.Timer(Me.components)
        Me.tUpdateObjects = New System.Windows.Forms.Timer(Me.components)
        Me.tFind = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'logWriter
        '
        Me.logWriter.Location = New System.Drawing.Point(2, 207)
        Me.logWriter.Size = New System.Drawing.Size(781, 179)
        '
        'lbDrivers
        '
        Me.lbDrivers.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbDrivers.FormattingEnabled = True
        Me.lbDrivers.Location = New System.Drawing.Point(552, 50)
        Me.lbDrivers.Name = "lbDrivers"
        Me.lbDrivers.Size = New System.Drawing.Size(220, 147)
        Me.lbDrivers.TabIndex = 2
        '
        'lbDevices
        '
        Me.lbDevices.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lbDevices.FormattingEnabled = True
        Me.lbDevices.Location = New System.Drawing.Point(12, 50)
        Me.lbDevices.Name = "lbDevices"
        Me.lbDevices.Size = New System.Drawing.Size(520, 147)
        Me.lbDevices.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 34)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Devices:"
        '
        'tStates
        '
        Me.tStates.Enabled = True
        Me.tStates.Interval = 1000
        '
        'tPoll
        '
        Me.tPoll.Enabled = True
        Me.tPoll.Interval = 500
        '
        'tUpdateObjects
        '
        Me.tUpdateObjects.Enabled = True
        Me.tUpdateObjects.Interval = 1000
        '
        'tFind
        '
        Me.tFind.Enabled = True
        Me.tFind.Interval = 60000
        '
        'AvrControlApp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 387)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lbDevices)
        Me.Controls.Add(Me.lbDrivers)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "AvrControlApp"
        Me.Text = "Bwl SmartHome CC430"
        Me.Controls.SetChildIndex(Me.logWriter, 0)
        Me.Controls.SetChildIndex(Me.lbDrivers, 0)
        Me.Controls.SetChildIndex(Me.lbDevices, 0)
        Me.Controls.SetChildIndex(Me.Label1, 0)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lbDrivers As ListBox
    Friend WithEvents lbDevices As ListBox
    Friend WithEvents Label1 As Label
    Friend WithEvents tStates As Timer
    Friend WithEvents tPoll As Timer
    Friend WithEvents tUpdateObjects As Timer
    Friend WithEvents tFind As Timer
End Class
