using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_GUI
{
    
    abstract class Device
    {
        public byte link_ID = 0;
        public String dev_id_hex = "";
        public byte[] device_id = null;
        public String group = "";
        public int last_active;
        public int timeout;
        abstract public void data_process(byte[] data);

    }
}
