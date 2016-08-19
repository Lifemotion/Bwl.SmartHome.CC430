using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote_GUI
{
    class Switch : Device
    {
        private Boolean state = false;

        public Switch()
        {
            this.timeout = 10; //10 секунд дл€ выключател€
        }

        public override void data_process(byte[] data)
        {
            this.last_active = Environment.TickCount / 1000;
            if (data[5] == 0x01)
            {
                //пришел пакет состо€ни€
                if (data[6] == 0x00) state = false;
                else state = true;                
            }
        }

        public Boolean isOn()
        {
            return state;
        }

        public void SwitchOn()
        {
            byte[] msg = new byte[2];
            msg[0] = 0x01;
            msg[1] = 0x01;
            RadioController.CmdQueue.Add(dev_id_hex, msg);
        }

        public void SwitchOff()
        {
            byte[] msg = new byte[2];
            msg[0] = 0x01;
            msg[1] = 0x00;
            RadioController.CmdQueue.Add(dev_id_hex, msg);
        }
    }
}
