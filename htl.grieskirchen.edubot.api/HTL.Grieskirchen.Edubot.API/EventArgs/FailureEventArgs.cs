using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    public class FailureEventArgs : System.EventArgs
    {
        volatile Exception thrownException;

        public Exception ThrownException
        {
            get { return thrownException; }
        }

        //public FailureEventArgs(System.Exception thrownException)
        //{
        //    this.newState = null;
        //    this.thrownException = thrownException;
        //}

        public FailureEventArgs(System.Exception thrownException) {
            this.thrownException = thrownException;
        }
    }
}
