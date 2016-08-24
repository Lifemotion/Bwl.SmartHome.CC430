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

namespace Remote_GUI
{
    public partial class Form1 : Form
    {
        RadioController rc = null;
        public Form1()
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

        private void bOpen_Click(object sender, EventArgs e)
        {
            String port = ports_list.SelectedItem.ToString();
            rc = new RadioController(port);
        }

        private void devices_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            String device_id = devices_list.SelectedItem.ToString();
            Device d = rc.devices[device_id];
            link_id.Text = "" + d.link_ID;
            Switch sw = (Switch)rc.devices[device_id];
            label5.Text = sw.isOn()+"";
        }

        private void bOn_Click(object sender, EventArgs e)
        {
            String device_id = devices_list.SelectedItem.ToString();
            Switch sw = (Switch)rc.devices[device_id];
            sw.SwitchOn();
        }

        private void bOff_Click(object sender, EventArgs e)
        {
            String device_id = devices_list.SelectedItem.ToString();
            Switch sw = (Switch)rc.devices[device_id];
            sw.SwitchOff();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            devices_list.Items.Clear();
            foreach(String dev in rc.devices.Keys)
            {
                devices_list.Items.Add(dev);
            }
            devices_list.Refresh();
        }
    }
}
