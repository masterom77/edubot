using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    /// <summary>
    /// Used to represent a three dimensional point
    /// </summary>
    public class Point3D
    {
        /// <summary>
        /// Creates a new instance of the Point3D class, where all coordinates are zero.
        /// </summary>
        public Point3D()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        /// <summary>
        /// Creates a new instance of the Point3D class using the given values
        /// </summary>
        /// <param name="x">The x-coordinate</param>
        /// <param name="y">The y-coordinate</param>
        /// <param name="z">The z-coordinate</param>
        public Point3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        float x;
        /// <summary>
        /// Gets or sets the x-coordinate
        /// </summary>
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        float y;
        /// <summary>
        /// Gets or sets the y-coordinate
        /// </summary>
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        float z;
        /// <summary>
        /// Gets or sets the z-coordinate
        /// </summary>
        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        /// <summary>
        /// Adds two points together
        /// </summary>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <returns>A Point3D containing the sum of the given two points</returns>
        public static Point3D operator + (Point3D point1, Point3D point2){
            return new Point3D(point1.x + point2.x, point1.y + point2.y, point1.z + point2.z);
        }

        /// <summary>
        /// Subtracts the second point from the first one
        /// </summary>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <returns>A Point3D containing the difference between the given two points</returns>
        public static Point3D operator -(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.x - point2.x, point1.y - point2.y, point1.z - point2.z);
        }

        /// <summary>
        /// Multiplies the two points
        /// </summary>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <returns>A Point3D containing the product of the given two points</returns>
        public static Point3D operator *(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.x * point2.x, point1.y * point2.y, point1.z * point2.z);
        }

        /// <summary>
        /// Divides first point through the second point
        /// </summary>
        /// <param name="point1">The first point</param>
        /// <param name="point2">The second point</param>
        /// <returns>A Point3D containing the quotient of the given two points</returns>
        public static Point3D operator *(Point3D point1, float factor)
        {
            return new Point3D(point1.x*factor, point1.y*factor, point1.z*factor);
        }


        /// <summary>
        /// Converts the Point3D object into a string
        /// </summary>
        /// <returns>Returns a string with format "x/y/z"</returns>
        public override string ToString()
        {
            return x + "/" + y + "/" + z;
        }
    }
}
