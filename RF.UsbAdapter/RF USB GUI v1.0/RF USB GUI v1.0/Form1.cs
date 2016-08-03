using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RF_USB_GUI_v1._0
{
    public partial class bScan : Form
    {
        private SerialPort _serial = null;

        public bScan()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String[] ports = SerialPort.GetPortNames();
            status.Text = "";
            foreach(String port in ports)
            {
                ports_list.Items.Add(port);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ports_list.Items.Clear();
            String[] ports = SerialPort.GetPortNames();
            foreach (String port in ports)
            {
                ports_list.Items.Add(port);
            }
        }

        private void SerialProcess()
        {
            while (_serial.IsOpen)
            {
                while (_serial.BytesToRead == 0) ;
                int d_len = _serial.ReadByte();
                byte[] data = new byte[d_len];
                while (_serial.BytesToRead < d_len) ;
                _serial.Read(data, 0, d_len);
                string hex = BitConverter.ToString(data).Replace("-", " ")+" | ";
                this.Invoke((MethodInvoker)delegate {
                    incom_data.Text = incom_data.Text + hex;
                });
                SetStatus("receive: ok");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string hexValues = data_length.Text +" "+ desr_addr.Text + " "+ bytes_to_send.Text;
            if (hexValues.Length < 1) return;
            string[] hexValuesSplit = hexValues.Split(' ');
            byte[] data = new byte[hexValuesSplit.Length + 1];
            data[0] = (byte)hexValuesSplit.Length;
            for (int i = 1; i<data.Length; i++) { 
                data[i] =(byte) Convert.ToInt32(hexValuesSplit[i-1], 16);               
            }
            if(_serial != null && _serial.IsOpen)
            {
                _serial.Write(data, 0, data.Length);
            }
            SetStatus("sending: ok");
        }

        private void bOpen_Click(object sender, EventArgs e)
        {
            if (ports_list.SelectedItem == null) return;
            String port = ports_list.SelectedItem.ToString();
            if (port.Length == 0) return;
            _serial = new SerialPort(ports_list.SelectedItem.ToString(), 115200);
            _serial.Open();
            SetStatus("port is opened");
            Thread proc = new Thread(SerialProcess);
            proc.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            incom_data.Text = "";
        }

        private void SetStatus(String st)
        {
            this.Invoke((MethodInvoker)delegate {
                status.Text = st;
            });
        }

        private void bytes_to_send_TextChanged(object sender, EventArgs e)
        {
            int data_len = bytes_to_send.Text.Replace(" ", "").Length / 2;
            byte[] buf = { (byte)data_len };
            data_length.Text = BitConverter.ToString(buf).Replace("-", "");
        }
    }
}
