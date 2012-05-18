using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using System.Net.Sockets;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    /// <summary>
    /// The exception that is thrown if an error in the network communication occured.
    /// </summary>
    public class NetworkException : Exception
    {
        /// <summary>
        /// Creates a new instance of the the NetworkException class
        /// </summary>
        /// <param name="msg">The message which describes the error</param>
        public NetworkException(string msg) : base(msg) {
        
        }
    }
}
