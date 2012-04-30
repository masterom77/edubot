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

        volatile State newState;

        public State NewState
        {
            get { return newState; }
        }

        //public FailureEventArgs(System.Exception thrownException)
        //{
        //    this.newState = null;
        //    this.thrownException = thrownException;
        //}

        public FailureEventArgs(State newState, System.Exception thrownException) {
            this.newState = newState;
            this.thrownException = thrownException;
        }
    }
}
