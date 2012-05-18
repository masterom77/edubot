using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    /// <summary>
    /// Used for passing details about an error
    /// </summary>
    public class FailureEventArgs : System.EventArgs
    {
        volatile Exception thrownException;
        /// <summary>
        /// Gets the exception, which caused the error
        /// </summary>
        public Exception ThrownException
        {
            get { return thrownException; }
        }

        /// <summary>
        /// Initializes a new instance of the FailureEventArgs class
        /// </summary>
        /// <param name="thrownException">The exception, which caused the error</param>
        public FailureEventArgs(System.Exception thrownException) {
            this.thrownException = thrownException;
        }
    }
}
