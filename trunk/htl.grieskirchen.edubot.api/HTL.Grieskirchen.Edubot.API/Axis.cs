using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API
{
    /// <summary>
    /// This class represents an axis of the robot, containing most of the information used for path calculation.
    /// </summary>
    public class Axis
    {

        public Axis(int x, int y, int z, float angle) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.angle = angle;
            this.length = 150;
            engine = new Engine();
        }

        int x;
        /// <summary>
        /// Gets the x-coordinate of the axis
        /// </summary>
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        
        int y;
        /// <summary>
        /// Gets the y-coordinate of the axis
        /// </summary>
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        int z;
        /// <summary>
        /// Gets the z-coordinate of the axis
        /// </summary>
        public int Z
        {
            get { return z; }
            set { z = value; }
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
            set { angle = value; }
        }

        Engine engine;

        /// <summary>
        /// Rotates the axis for the specified amount of ticks by using the specified 
        /// speed
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="speed"></param>
        public void Rotate(InterpolationResult result, bool isConnected) {
            this.angle += result.Angle;
            
            if (isConnected) {
                engine.TurnAngle(result.Ticks, result.Speed);
            }
        }
    }
}
