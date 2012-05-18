using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    /// <summary>
    /// Used for passing details about tool related process
    /// </summary>
    public class ToolUsedEventArgs : System.EventArgs
    {
        bool activated;
        /// <summary>
        /// Gets the value indicating wether the tool is being activated or deactivated
        /// </summary>
        public bool Activated
        {
            get { return activated; }
        }

        /// <summary>
        /// Creates a new instance of the ToolUsedEventArgs using the given values
        /// </summary>
        /// <param name="activated">The value indicating wether the tool is being activated or deactivated</param>
        public ToolUsedEventArgs(bool activated) : base() {
            this.activated = activated;
        }
    }
}
