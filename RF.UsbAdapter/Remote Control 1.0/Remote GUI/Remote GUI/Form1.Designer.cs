namespace Remote_GUI
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.devices_list = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.link_id = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ports_list = new System.Windows.Forms.ComboBox();
            this.bOpen = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.bOn = new System.Windows.Forms.Button();
            this.bOff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // devices_list
            // 
            this.devices_list.FormattingEnabled = true;
            this.devices_list.Location = new System.Drawing.Point(12, 38);
            this.devices_list.Name = "devices_list";
            this.devices_list.Size = new System.Drawing.Size(140, 69);
            this.devices_list.Sorted = true;
            this.devices_list.TabIndex = 0;
            this.devices_list.SelectedIndexChanged += new System.EventHandler(this.devices_list_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Link ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "State:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Last act:";
            // 
            // link_id
            // 
            this.link_id.AutoSize = true;
            this.link_id.Location = new System.Drawing.Point(79, 110);
            this.link_id.Name = "link_id";
            this.link_id.Size = new System.Drawing.Size(10, 13);
            this.link_id.TabIndex = 4;
            this.link_id.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(79, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "-";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(79, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "-";
            // 
            // ports_list
            // 
            this.ports_list.FormattingEnabled = true;
            this.ports_list.Location = new System.Drawing.Point(12, 11);
            this.ports_list.Name = "ports_list";
            this.ports_list.Size = new System.Drawing.Size(64, 21);
            this.ports_list.TabIndex = 7;
            // 
            // bOpen
            // 
            this.bOpen.Location = new System.Drawing.Point(82, 11);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(70, 21);
            this.bOpen.TabIndex = 8;
            this.bOpen.Text = "Open";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(168, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Device: ";
            // 
            // bOn
            // 
            this.bOn.Location = new System.Drawing.Point(171, 54);
            this.bOn.Name = "bOn";
            this.bOn.Size = new System.Drawing.Size(106, 23);
            this.bOn.TabIndex = 11;
            this.bOn.Text = "ON";
            this.bOn.UseVisualStyleBackColor = true;
            this.bOn.Click += new System.EventHandler(this.bOn_Click);
            // 
            // bOff
            // 
            this.bOff.Location = new System.Drawing.Point(171, 84);
            this.bOff.Name = "bOff";
            this.bOff.Size = new System.Drawing.Size(107, 23);
            this.bOff.TabIndex = 12;
            this.bOff.Text = "OFF";
            this.bOff.UseVisualStyleBackColor = true;
            this.bOff.Click += new System.EventHandler(this.bOff_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 163);
            this.Controls.Add(this.bOff);
            this.Controls.Add(this.bOn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.ports_list);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.link_id);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.devices_list);
            this.Name = "Form1";
            this.Text = "Remote GUI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox devices_list;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label link_id;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox ports_list;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bOn;
        private System.Windows.Forms.Button bOff;
    }
}

