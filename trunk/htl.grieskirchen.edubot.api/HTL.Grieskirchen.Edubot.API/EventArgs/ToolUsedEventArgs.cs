using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    public class ToolUsedEventArgs : System.EventArgs
    {
        bool activated;

        public bool Activated
        {
            get { return activated; }
        }

        public ToolUsedEventArgs(bool activated) : base() {
            this.activated = activated;
        }
    }
}
