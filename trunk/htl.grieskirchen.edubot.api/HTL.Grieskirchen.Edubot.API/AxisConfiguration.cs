using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API
{
    /// <summary>
    /// An enumeration containing the possible axis configurations for a robot with SCARA architecture
    /// </summary>
    public enum AxisConfiguration
    {
        /// <summary>
        /// Represents a lefty axis configuration, where the angle of the secondary axis is always positive
        /// </summary>
        Lefty = 0,
        /// <summary>
        /// Represents a righty axis configuration, where the angle of the secondary axis is always negative.
        /// </summary>
        Righty = 1
    }
}
