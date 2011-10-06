using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.Exceptions
{
    class UnknownCommandException : Exception
    {
        public UnknownCommandException(string msg) : base(msg) {
        
        }

    }
}
