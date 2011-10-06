using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API
{
    /// <summary>
    /// This class represents an axis of the robot, containing most of the information used for path calculation.
    /// </summary>
    class Axis
    {
        int x;
        /// <summary>
        /// Gets the x-coordinate of the axis
        /// </summary>
        public int X
        {
            get { return x; }
        }
        
        int y;
        /// <summary>
        /// Gets the y-coordinate of the axis
        /// </summary>
        public int Y
        {
            get { return y; }
        }

        int z;
        /// <summary>
        /// Gets the z-coordinate of the axis
        /// </summary>
        public int Z
        {
            get { return z; }
        }

        float length;
        /// <summary>
        /// Gets the length of the axis
        /// </summary>
        public float Length
        {
            get { return length; }
        }

        float angle;
        /// <summary>
        /// Gets the angle of the engine, which is related to this axis
        /// </summary>
        public float Angle
        {
            get { return angle; }
        }

        Engine engine;

        /// <summary>
        /// Rotates the axis for the specified amount of ticks by using the specified 
        /// speed
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="speed"></param>
        public void Rotate(long ticks, float speed) {
            
        }
    }
}
