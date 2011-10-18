using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API
{
    /// <summary>
    /// This interface represents an interpolation type. Any class implementing this
    /// interface can be used for path calculation.
    /// </summary>
    public interface IInterpolationType
    {
        float[,] CalculatePath(Axis primaryAxis, Axis secondaryAxis, Axis verticalAxis, Axis toolAxis, int x, int y, int z);
    }
}
