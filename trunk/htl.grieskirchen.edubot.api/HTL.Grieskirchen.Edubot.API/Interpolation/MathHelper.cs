using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    /// <summary>
    /// Used for different mathemical conversions and calculations
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Converts the given angle from radians to degrees
        /// </summary>
        /// <param name="rad">The angle in radians</param>
        /// <returns>Return the given angle in degrees</returns>
        public static float ConvertToDegrees(double rad)
        {
            return (float)(rad * 180 / Math.PI);
        }

        /// <summary>
        /// Converts the given angle from degrees to radians
        /// </summary>
        /// <param name="rad">The angle in degrees</param>
        /// <returns>Return the given angle in radians</returns>
        public static float ConvertToRadians(double degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        /// <summary>
        /// Determines in which quadrant of a cartesian coordinate system the given point lies
        /// </summary>
        /// <param name="x">The x-coordinate</param>
        /// <param name="y">The y-coordinate</param>
        /// <returns>An integer between 1 and 4, indicating the quadrant</returns>
        public static int GetQuadrant(float x, float y)
        {
            
            if (x >= 0 && y >= 0)
                return 1;
            if (x <= 0 && y >= 0)
                return 2;
            if (x <= 0 && y <= 0)
                return 3;
            return 4;
        }
    }
}
