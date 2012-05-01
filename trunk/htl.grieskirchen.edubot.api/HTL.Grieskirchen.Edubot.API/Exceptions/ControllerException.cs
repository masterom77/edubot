using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    public class ControllerException : Exception
    {
        public ControllerException(string msg)
            : base(msg)
        {
        }
    }
}
