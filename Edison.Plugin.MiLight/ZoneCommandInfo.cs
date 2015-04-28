using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Plugin.MiLight
{
    internal class ZoneCommandInfo
    {
        public ZoneCommandInfo(byte @on, byte off, byte? night, byte? full, byte? white)
        {
            On = @on;
            Off = off;
            Night = night;
            Full = full;
            White = white;
        }

        public byte On { get; private set; }
        public byte Off { get; private set; }
        public byte? Night { get; private set; }
        public byte? Full { get; private set; }
        public byte? White { get; private set; }
    }
}
