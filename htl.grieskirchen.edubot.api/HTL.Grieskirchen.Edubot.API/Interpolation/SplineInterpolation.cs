using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class SplineInterpolation : IInterpolationType
    {
        public List<InterpolationResult> CalculatePath(Axis primaryAxis, Axis secondaryAxis, Axis verticalAxis, Axis toolAxis, int x, int y, int z)
        {
            List<InterpolationResult> results = new List<InterpolationResult>();
            results.Add(new InterpolationResult(primaryAxis.Angle, 500));
            results.Add(new InterpolationResult(secondaryAxis.Angle, 500));
            results.Add(new InterpolationResult(verticalAxis.Angle, 500));
            return results;
        }
    }
}
