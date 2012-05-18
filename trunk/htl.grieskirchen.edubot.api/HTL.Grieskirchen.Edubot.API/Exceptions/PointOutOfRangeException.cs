using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    /// <summary>
    /// The exception that is thrown if a point given by the user or calculated by the interpolation is not within the working area of the robot.
    /// </summary>
    public class PointOutOfRangeException : Exception
    {
        Point3D point;
        /// <summary>
        /// Gets the point which can't be reached
        /// </summary>
        public Point3D Point
        {
            get { return point; }
        }

        /// <summary>
        /// Creates a new instance of the the PointOutOfRangeException class
        /// </summary>
        /// <param name="point">The point which can't be reached</param>
        /// <param name="msg">The message which describes the error</param>
        public PointOutOfRangeException(Point3D point, string msg)
            : base(msg)
        {
            this.point = point;
        }
    }
}
