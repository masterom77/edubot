using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class AdapterFactory
    {
        public static IAdapter GetAdapter(AdapterType adapter) {
            switch (adapter) {
                case AdapterType.KEBA: return new KebaAdapter(new VirtualTool(), 500, IPAddress.Parse("192.168.0.40"),12000);
                case AdapterType.VIRTUAL: return new VirtualAdapter(new VirtualTool(), 150);
                case AdapterType.DEFAULT: return new DefaultAdapter(new VirtualTool(), 150, IPAddress.Parse("192.168.0.40"), 12000);
            }
            return null;
        }
    }
}
