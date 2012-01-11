using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    class IllegalStateUpdateException : Exception
    {
        IStateListener listener;

        public IllegalStateUpdateException(IStateListener listener, string msg)
            : base(msg)
        {
            this.listener = listener;
        }
    }
}
