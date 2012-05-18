using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    /// <summary>
    /// The exception that is used when an action can't take place because the current state of the adapter is not valid. This can happen if the adapter should for example execute a MVSCommand while it is still in the SHUTDOWN-State
    /// </summary>
    public class InvalidStateException : Exception
    {
        /// <summary>
        /// Creates a new instance of the the InvalidStateException class
        /// </summary>
        /// <param name="msg">The message which describes the error</param>
        public InvalidStateException(string msg) : base(msg) {
        
        }

    }
}
