using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    /// <summary>
    /// An enumeration containing different interpolation types
    /// </summary>
    public enum InterpolationType
    {
        /// <summary>
        /// Used for linear interpolation
        /// </summary>
        Linear,
        /// <summary>
        /// Used for circular interpolation
        /// </summary>
        Circular,
        /// <summary>
        /// Used if there's no specific movement type, e.g. a change of axis configuration
        /// </summary>
        None
    }
}
