using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    class InvalidStateException : Exception
    {
        public InvalidStateException(string msg) : base(msg) {
        
        }

    }
}
