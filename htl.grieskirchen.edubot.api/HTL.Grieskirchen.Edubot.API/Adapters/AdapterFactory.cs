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
                case AdapterType.KEBA: return new KebaAdapter(500, false, null,0);
                    break;
                case AdapterType.VIRTUAL: return new VirtualAdapter(150, true);
                    break;
                case AdapterType.DEFAULT: return new DefaultAdapter(150,true,null,0);
            }
            return null;
        }
    }
}
