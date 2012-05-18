using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    /// <summary>
    /// Used for passing details about a status update
    /// </summary>
    public class StateChangedEventArgs : System.EventArgs
    {
        State oldState;
        /// <summary>
        /// Gets the old state of the robot
        /// </summary>
        public State OldState
        {
            get { return oldState; }
        }

        State newState;
        /// <summary>
        /// Gets the new state of the robot
        /// </summary>
        public State NewState
        {
            get { return newState; }
        }

        /// <summary>
        /// Creates a new instance of the StateChangedEventArgs using the given values
        /// </summary>
        /// <param name="oldState">The old state of the robot</param>
        /// <param name="newState">The new state of the robot</param>
        public StateChangedEventArgs(State oldState, State newState) : base() {
            this.newState = newState;
            this.oldState = oldState;
        }
    }
}
