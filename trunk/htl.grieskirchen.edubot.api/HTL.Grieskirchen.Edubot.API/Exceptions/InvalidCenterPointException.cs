using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    /// <summary>
    /// The exception the is thrown if the center point of MVCCommand is not valid. This can happen if the distance between center and target as well as center and current toolcenter point is not the same.
    /// </summary>
    public class InvalidCenterPointException : System.Exception
    {
        Point3D toolCenterPoint;
        /// <summary>
        /// Get the current toolcenter point
        /// </summary>
        public Point3D ToolCenterPoint
        {
            get { return toolCenterPoint; }
        }

        Point3D targetPoint;
        /// <summary>
        /// Gets the target point of the movement
        /// </summary>
        public Point3D TargetPoint
        {
            get { return targetPoint; }
        }

        Point3D center;
        /// <summary>
        /// Gets the center point of the movement
        /// </summary>
        public Point3D Center
        {
            get { return center; }
        }

        /// <summary>
        /// Creates a new instance of the the InvalidCenterPointException class
        /// </summary>
        /// <param name="toolCenterPoint">The current toolcenter point</param>
        /// <param name="targetPoint">The target point of the movement</param>
        /// <param name="center">The center point of the movement</param>
        /// <param name="msg">The message which describes the error</param>
        public InvalidCenterPointException(Point3D toolCenterPoint, Point3D targetPoint, Point3D center, string msg) : base(msg){
            this.toolCenterPoint = toolCenterPoint;
            this.targetPoint = targetPoint;
            this.center = center;
        }
    }
}
