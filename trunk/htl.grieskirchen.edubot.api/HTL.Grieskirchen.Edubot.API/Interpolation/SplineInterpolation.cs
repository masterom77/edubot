using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class SplineInterpolation : IInterpolationType
    {
        public InterpolationResult CalculatePath(ITool tool, int x, int y, int z, float length)
        {
            List<InterpolationResult> results = new List<InterpolationResult>();
            return null;
        }
    }
}
