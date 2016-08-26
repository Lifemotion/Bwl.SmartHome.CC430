using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace SmartHomeVK.DeviceWork
{
    class RadioController
    {
        SerialPort _port = null;
        public static Dictionary<string, Device> devices = new Dictionary<string, Device>();
        public static Dictionary<string, byte[]> CmdQueue = new Dictionary<string, byte[]>();

        public RadioController(String port)
        {
            _port = new SerialPort(port, 19200);
            _port.Open();
            new Thread(SerialProcess).Start();
            //new Thread(DeviceCleaner).Start();
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

                if (!devices.ContainsKey(device_id))
                {
                    //добавляем новое устройство в словарь     
                    if (dev_id[3] > 0x00)
                    {
                        //выключтаель
                        Switch sw = new Switch();
                        sw.link_ID = data[5];
                        sw.dev_id_hex = device_id;
                        sw.device_id = dev_id;
                        devices.Add(device_id, sw);
                        MessageProcessor.SendMsg(MessageProcessor.current_user, "Добавлено новое устройство: " + device_id);
                    }
                    else if (dev_id[3] == 0x02)
                    {
                        //IR управление
                    }
                }

                if (data[0] == 0x02)
                {
                    //пришел пакет от устройства
                    if (!CmdQueue.ContainsKey(device_id))
                    {
                        //нет посылки для данного устройства, отправляем ноль
                        byte[] buf = new byte[1];
                        buf[0] = 0x00;
                        _port.Write(buf, 0, 1);
                    }
                    else
                    {
                        //есть посылка, отправляем ее.
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


        private void DeviceCleaner()
        {
            while (true)
            {
                foreach (String dev_id in devices.Keys)
                {
                    if (Environment.TickCount / 1000 - devices[dev_id].last_active > devices[dev_id].timeout) devices.Remove(dev_id);
                }
                Thread.Sleep(1000);
            }
        }

        private byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, c, 0, a.Length);
            Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }
    }
}
