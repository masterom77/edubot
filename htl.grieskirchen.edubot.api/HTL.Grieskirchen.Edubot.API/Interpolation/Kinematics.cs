using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Exceptions;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{

    /// <summary>
    /// Helps calculating axis angles for a specific point respectively a point from specific angles
    /// </summary>
    public class Kinematics
    {


        /// <summary>
        /// Calculates the point of the tool, if axes have adopted the given angles
        /// </summary>
        /// <param name="alpha1">The angle of the primary axis</param>
        /// <param name="alpha2">The angle of the secondary axis</param>
        /// <param name="alpha3">The angle of the third axis</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="verticalToolRange">The distance between the toolcenter point and the working area, measured when the tool has adopted the highest position</param>
        /// <param name="transmission">The transmission, which defines how many degrees the third engine has to rotate, to move the tertiary axis down one unit</param>
        /// <returns>Returns the point of the tool as Point3D object, if axes have adopted the given angles</returns>
        public static Point3D CalculateDirect(float alpha1, float alpha2, float alpha3, float length, float length2, float verticalToolRange, float transmission) { 
            double beta = 180 - Math.Abs(alpha2);
            double d = Math.Sqrt((length * length + length2 * length2) - 2 * length * length2 * Math.Cos(MathHelper.ConvertToRadians(beta)));
            
            double alpha = MathHelper.ConvertToDegrees(Math.Acos((d * d + length * length - length2 * length2) / (2 * d * length)));
            double temp;
            if(alpha2 <= 0)
                temp = alpha1 - alpha;
            else
                temp = alpha1 + alpha;
            float x = (float)(d * Math.Cos(MathHelper.ConvertToRadians(temp))); 
            float y = (float) (d * Math.Sin(MathHelper.ConvertToRadians(temp)));
            float z = (float)(verticalToolRange - (alpha3 / transmission));
          
            
            return new Point3D(x,y,0);
        }

        /// <summary>
        /// Calculates the possible angles of the axes, which are necessary to reach the given target point.
        /// </summary>
        /// <param name="target">The target point</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="verticalToolRange">The distance between the toolcenter point and the working area, measured when the tool has adopted the highest position</param>
        /// <param name="transmission">The transmission, which defines how many degrees the third engine has to rotate, to move the tertiary axis down one unit</param>
        /// <returns>Returns an array of InterpolationStep objects, containing the possible angles</returns>
        public static InterpolationStep[] CalculateInverse(Point3D target, float length, float length2, float verticalToolRange, float transmission)
        {
            InterpolationStep[] result = new InterpolationStep[2];
            float x = target.X;
            float y = target.Y;
            float z = target.Z;
            float d = (float)Math.Round(Math.Sqrt(x * x + y * y), 2);
            
            double alpha = MathHelper.ConvertToDegrees(Math.Acos((d * d + length * length - length2 * length2) / (2 * d * length)));
            double beta = MathHelper.ConvertToDegrees(Math.Acos((length2 * length2 + length * length - d * d) / (2 * length2 * length)));
            double gamma = MathHelper.ConvertToDegrees(Math.Acos((d * d + length2 * length2 - length * length) / (2 * d * length2)));

            double cosine = x / d;
            if (Math.Abs(cosine) > 1 && Math.Abs(cosine) - 1 <= 0.005)
            {
                cosine = Math.Round(cosine, 0);
            }
            double temp = MathHelper.ConvertToDegrees(Math.Acos(cosine));

            int quadrant = MathHelper.GetQuadrant(x, y);
            if (quadrant == 3 || quadrant == 4)
            {
                temp = -temp;

            }
            result[0] = new InterpolationStep(target, (float)(temp - alpha), (float)(180 - beta), (verticalToolRange - z) * transmission);
            result[1] = new InterpolationStep(target, (float)(temp + alpha), (float)-(180 - beta), (verticalToolRange - z) * transmission);

            return result;
        }
    }
}
