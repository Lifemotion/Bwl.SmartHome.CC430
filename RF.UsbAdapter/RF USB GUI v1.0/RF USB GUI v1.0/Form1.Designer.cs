namespace RF_USB_GUI_v1._0
{
    partial class bScan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(bScan));
            this.ports_list = new System.Windows.Forms.ComboBox();
            this.bOpen = new System.Windows.Forms.Button();
            this.bytes_to_send = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.incom_data = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            this.desr_addr = new System.Windows.Forms.TextBox();
            this.data_length = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rssi_value = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ports_list
            // 
            this.ports_list.FormattingEnabled = true;
            this.ports_list.Location = new System.Drawing.Point(13, 13);
            this.ports_list.Name = "ports_list";
            this.ports_list.Size = new System.Drawing.Size(87, 21);
            this.ports_list.TabIndex = 0;
            // 
            // bOpen
            // 
            this.bOpen.Location = new System.Drawing.Point(106, 13);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(75, 23);
            this.bOpen.TabIndex = 1;
            this.bOpen.Text = "open";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // bytes_to_send
            // 
            this.bytes_to_send.Location = new System.Drawing.Point(107, 71);
            this.bytes_to_send.Name = "bytes_to_send";
            this.bytes_to_send.Size = new System.Drawing.Size(491, 20);
            this.bytes_to_send.TabIndex = 2;
            this.bytes_to_send.TextChanged += new System.EventHandler(this.bytes_to_send_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 97);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(585, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "send";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // incom_data
            // 
            this.incom_data.Location = new System.Drawing.Point(13, 141);
            this.incom_data.Multiline = true;
            this.incom_data.Name = "incom_data";
            this.incom_data.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.incom_data.Size = new System.Drawing.Size(585, 163);
            this.incom_data.TabIndex = 4;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(13, 310);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(585, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "clear";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(187, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "scan";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Location = new System.Drawing.Point(296, 18);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(35, 13);
            this.status.TabIndex = 7;
            this.status.Text = "status";
            // 
            // desr_addr
            // 
            this.desr_addr.Location = new System.Drawing.Point(12, 71);
            this.desr_addr.Name = "desr_addr";
            this.desr_addr.Size = new System.Drawing.Size(42, 20);
            this.desr_addr.TabIndex = 8;
            // 
            // data_length
            // 
            this.data_length.Location = new System.Drawing.Point(60, 71);
            this.data_length.Name = "data_length";
            this.data_length.Size = new System.Drawing.Size(40, 20);
            this.data_length.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "addr";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "length";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(104, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "data";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(381, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "RSSI:";
            // 
            // rssi_value
            // 
            this.rssi_value.AutoSize = true;
            this.rssi_value.Location = new System.Drawing.Point(413, 18);
            this.rssi_value.Name = "rssi_value";
            this.rssi_value.Size = new System.Drawing.Size(10, 13);
            this.rssi_value.TabIndex = 14;
            this.rssi_value.Text = "-";
            // 
            // bScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 345);
            this.Controls.Add(this.rssi_value);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.data_length);
            this.Controls.Add(this.desr_addr);
            this.Controls.Add(this.status);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.incom_data);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bytes_to_send);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.ports_list);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "bScan";
            this.Text = "Rf GUI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ports_list;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.TextBox bytes_to_send;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox incom_data;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.TextBox desr_addr;
        private System.Windows.Forms.TextBox data_length;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label rssi_value;
    }
}

