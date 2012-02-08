using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using HTL.Grieskirchen.Edubot.API.Exceptions;


namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    /// <summary>
    /// An implementation of the IInterpolationType interfaces, which uses linear interpolation for path calculation
    /// </summary>
    public class CircularInterpolation
    {
        /// <summary>
        /// dCalculates the path by using the specified parameters and linear interpolation.
        /// </summary>
        /// <param name="toolCenterPoint">A point, representing the current location of the tool</param>
        /// <param name="target">The target point of the movement</param>
        /// <param name="length">The length of the first axis</param>
        /// <param name="length2">The length of the second axis</param>
        /// <returns></returns>
        public InterpolationResult CalculatePath(Point3D toolCenterPoint, Point3D target, Point3D center, float length, float length2)
        {
            int steps;// = Configuration.InterpolationSteps;
            float toolX = toolCenterPoint.X;
            float toolY = toolCenterPoint.Y;
            float difX = target.X - toolX;
            float difY = target.Y - toolY;
            float distance = (float) Math.Sqrt(difX * difX + difY * difY);
            steps = (int) Math.Round(distance,1);
            float incrX = difX / steps;
            float incrY = difY / steps;

            InterpolationResult result = new InterpolationResult();
            InterpolationStep prevStep = Kinematics.CalculateInverse(new Point3D(toolX,toolY,0),length,length2);
            InterpolationStep step;
            result.PrimarySpeed = incrX / incrY;
            result.SecondarySpeed = incrY / incrX;

            for (int i = 0; i < steps; i++)
            {
                
                toolX += incrX;
                toolY += incrY;
                step = Kinematics.CalculateInverse(new Point3D(toolX,toolY,0), length,length2);// -step;
                result.Angles.Add(step);
                result.Steps.Add(step-prevStep);
                prevStep = step;
            }
            
            return result;
            
        }

      
        
    }
}
