using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class SplineInterpolation : IInterpolationType
    {
        public InterpolationResult CalculatePath(Axis primaryAxis, Axis secondaryAxis, Axis verticalAxis, Axis toolAxis, int x, int y, int z)
        {
            List<InterpolationResult> results = new List<InterpolationResult>();
            return null;
        }
    }
}
