using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    public class AdapterException : Exception
    {
        AdapterType adapter;

        public AdapterException(AdapterType adapter, string msg) : base(msg) {
            this.adapter = adapter;
        }
    }
}
