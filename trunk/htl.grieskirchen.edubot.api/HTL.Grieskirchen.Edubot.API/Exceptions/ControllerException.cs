using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    /// <summary>
    /// The exception that is thrown if an error on the controller occured. Try restarting the controller when you get this type of exception.
    /// </summary>
    public class ControllerException : Exception
    {
        /// <summary>
        /// Creates a new instance of the the ControllerException class
        /// </summary>
        /// <param name="msg">The message which describes the error</param>
        public ControllerException(string msg)
            : base(msg)        
        {
           
        }
    }
}
