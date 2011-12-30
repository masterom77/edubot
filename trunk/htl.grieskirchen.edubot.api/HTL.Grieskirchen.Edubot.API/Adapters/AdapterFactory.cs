using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class AdapterFactory
    {
        public static IAdapter GetAdapter(AdapterType adapter) {
            switch (adapter) {
                case AdapterType.KEBA: return new KebaAdapter(new VirtualTool(), 500, false, null,0);
                    break;
                case AdapterType.VIRTUAL: return new VirtualAdapter(new VirtualTool(), 150, true);
                    break;
                case AdapterType.DEFAULT: return new DefaultAdapter(new VirtualTool(), 150, true, null, 0);
            }
            return null;
        }
    }
}
