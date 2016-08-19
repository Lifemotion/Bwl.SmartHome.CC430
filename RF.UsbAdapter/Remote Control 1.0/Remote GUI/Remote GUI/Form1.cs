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
        SerialPort _port = null;
        Dictionary<string, Device> devices =  new Dictionary<string, Device>();
        public static Dictionary<string, byte[]> CmdQueue = new Dictionary<string, byte[]>();

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
            if (ports_list.SelectedItem == null || _port != null) return;
            String port = ports_list.SelectedItem.ToString();
            _port = new SerialPort(port, 19200);
            _port.Open();
            var th = new Thread(SerialProcess);
            th.Start();
        }

        public void SerialProcess()
        {
            while (_port.IsOpen)
            {
                while (_port.BytesToRead < 1) ;
                int d_len = _port.ReadByte();
                while (_port.BytesToRead < d_len) ;
                byte[] data = new byte[d_len];
                _port.Read(data, 0, d_len);
                byte[] dev_id = new byte[4];
                Array.Copy(data, 1, dev_id, 0, 4);
                String device_id = BitConverter.ToString(dev_id).Replace("-", string.Empty);

                if (!devices.ContainsKey(device_id)) {
                    //добавляем новое устройство в словарь     
                    devices_list.Invoke(new Action(() =>
                    {
                        devices_list.Items.Add(device_id);
                        devices_list.Refresh();
                    }));

                    if (dev_id[3] > 0x00)
                    {
                            //выключтаель
                            Switch sw = new Switch();
                            sw.link_ID = data[5];
                            sw.dev_id_hex = device_id;
                            sw.device_id  = dev_id;
                            devices.Add(device_id, sw); 
                                           
                    } else if (dev_id[3] == 0x02) {
                        //IR управление
                    }
                }

                if (data[0] == 0x02)      //пришел пакет от устройства
                {
                    if (!CmdQueue.ContainsKey(device_id))
                    {
                        byte[] buf = new byte[1];
                        buf[0] = 0x00;
                        _port.Write(buf, 0, 1);                      
                    } else {
                        byte[] head = new byte[2];
                        head[0] = (byte)(CmdQueue[device_id].Length + 1);
                        head[1] = devices[device_id].link_ID;                                  // идентификатор пакета
                        byte[] buffer = Combine(head, CmdQueue[device_id]);
                        _port.Write(buffer, 0, buffer.Length);
                        CmdQueue.Remove(device_id);                      //подчищаем очередь
                    }
                        devices[device_id].data_process(data);
                }

            }
        }

        private byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
            System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }

        private void devices_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            String device_id = devices_list.SelectedItem.ToString();
            Device d = devices[device_id];
            link_id.Text = "" + d.link_ID;
            Switch sw = (Switch)devices[device_id];
            label5.Text = sw.isOn()+"";
        }

        private void bOn_Click(object sender, EventArgs e)
        {
            String device_id = devices_list.SelectedItem.ToString();
            Switch sw = (Switch)devices[device_id];
            sw.SwitchOn();
        }

        private void bOff_Click(object sender, EventArgs e)
        {
            String device_id = devices_list.SelectedItem.ToString();
            Switch sw = (Switch)devices[device_id];
            sw.SwitchOff();
        }
    }
}
