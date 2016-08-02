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
    }
}
