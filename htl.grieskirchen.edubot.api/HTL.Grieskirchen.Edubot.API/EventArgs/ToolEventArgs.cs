using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    public class ToolEventArgs : System.EventArgs
    {
        bool activated;

        public bool Activated
        {
            get { return activated; }
        }

        public ToolEventArgs(bool activated) : base() {
            this.activated = activated;
        }
    }
}
