using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API
{
    /// <summary>
    /// Defines the different robot states
    /// </summary>
    public enum State
    {
        HOMING,
        SHUTTING_DOWN,
        READY,
        SHUTDOWN,
        MOVING,
        FAILURE
    }
}
