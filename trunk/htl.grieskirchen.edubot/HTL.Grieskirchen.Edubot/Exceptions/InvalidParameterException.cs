using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.Exceptions
{
    class InvalidParameterException : Exception
    {
        public InvalidParameterException(string msg) : base(msg) { 
        
        }
    }
}
