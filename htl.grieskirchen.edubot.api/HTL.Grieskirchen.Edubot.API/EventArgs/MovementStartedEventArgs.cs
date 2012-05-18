using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    /// <summary>
    /// Used for passing details about a started movement
    /// </summary>
    public class MovementStartedEventArgs : System.EventArgs
    {

        InterpolationResult result;
        /// <summary>
        /// Gets the interpolation result, which contains details about the movement itself
        /// </summary>
        public InterpolationResult Result
        {
            get { return result; }
        }

        /// <summary>
        /// Creates a new instance of the MovementStartedEventArgs using the given values
        /// </summary>
        /// <param name="result">The interpolation result, which contains details about the movement itself</param>
        public MovementStartedEventArgs(InterpolationResult result)
            : base()
        {
            this.result = result;
        }
    }
}
