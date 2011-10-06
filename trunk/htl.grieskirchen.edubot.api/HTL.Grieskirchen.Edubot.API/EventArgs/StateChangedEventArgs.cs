using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    public class StateChangedEventArgs : System.EventArgs
    {
        State oldState;

        public State OldState
        {
            get { return oldState; }
        }

        State newState;

        public State NewState
        {
            get { return newState; }
        }

        public StateChangedEventArgs(State oldState, State newState) : base() {
            this.newState = newState;
            this.oldState = oldState;
        }
    }
}
