using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using System.Net.Sockets;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    public class NetworkException : Exception
    {

        public NetworkException(string msg) : base(msg) {
        
        }
    }
}
