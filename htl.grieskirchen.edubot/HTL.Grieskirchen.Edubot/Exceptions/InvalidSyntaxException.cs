
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.Exceptions
{
    class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException(string msg) : base(msg) {
        
        }

    }
}
