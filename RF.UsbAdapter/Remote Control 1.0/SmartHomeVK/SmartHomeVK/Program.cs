using SmartHomeVK.DeviceWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeVK
{
    class Program
    {
        static void Main(string[] args)
        {
            new RadioController("COM9");
            new MessageProcessor("89370794325", "gusto757").Auth();
        }
    }
}
