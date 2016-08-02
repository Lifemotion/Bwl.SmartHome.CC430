using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
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

        private void button2_Click(object sender, EventArgs e)
        {
            string hexValues = bytes_to_send.Text;
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
        }
    }
}
