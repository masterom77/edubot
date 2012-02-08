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
        /// <summary>
        /// Calculates the path by using the specified parameters
        /// </summary>
        /// <param name="tool">The tool of the robot</param>
        /// <param name="x">The target-X coordinate</param>
        /// <param name="y">The target-Y coordinate</param>
        /// <param name="z">The target-Z coordinate</param>
        /// <param name="length">The length of both axis</param>
        /// <returns></returns>
        InterpolationResult CalculatePath(Point3D toolCenterPoint, Point3D target, float length, float length2);
    }
}
