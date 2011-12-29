using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    /// <summary>
    /// An implementation of the IInterpolationType interfaces, which uses linear interpolation for path calculation
    /// </summary>
    public class LinearInterpolation : IInterpolationType
    {
        /// <summary>
        /// Calculates the path by using the specified parameters
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public InterpolationResult CalculatePath(ITool tool, int x, int y, int z, float length)
        {
            int steps = 100;
            float toolX = tool.X;
            float toolY = tool.Y;
            float difX = x - toolX;
            if ((x >= 0 && toolX <= 0)||(x <= 0 && toolX >= 0)){
                difX = Math.Abs(x) + Math.Abs(toolX);
                if (x < toolX) {
                    difX *= -1;
                }
            }
            float difY = y - toolY;
            if ((y >= 0 && toolY <= 0)||(y <= 0 && toolY >= 0)){
                difY = Math.Abs(y) + Math.Abs(toolY);
                if (y < toolY)
                {
                    difY *= -1;
                }
            }

            float incrX = difX / steps;
            float incrY = difY / steps;

            float[] primaryAngles = new float[steps];
            float[] secondaryAngles = new float[steps];
            float[] speeds = new float[2];

            float primaryVelocity = incrX / incrY;
            float secondaryVelocity = incrY / incrX;

            InterpolationResult result = new InterpolationResult(steps,2);
            

            for (int i = 0; i < steps; i++) {
                toolX += incrX;
                toolY += incrY;
                CalculateAngleForPoint(toolX, toolY, length, out result.Angles[i, 0], out result.Angles[i,1]);
            }

            result.Velocities[0] = primaryVelocity;
            result.Velocities[1] = secondaryVelocity;
            //result.Result.Add(AxisType.PRIMARY,new AxisData(primaryAngles,primarySpeed));
            //result.Result.Add(AxisType.SECONDARY,new AxisData(secondaryAngles,secondarySpeed));
            return result;
            
        }

        /// <summary>
        /// Calculates the angles alpha1 and alpha2, when the tool is on a specific point
        /// </summary>
        /// <param name="x">The x-coordinate of the tool</param>
        /// <param name="y">The y-coordinate of the tool</param>
        /// <param name="length">The length of both axis</param>
        /// <param name="alpha1">The calculated alpha1 angle</param>
        /// <param name="alpha2">The calculated alpha2 angle</param>
        private void CalculateAngleForPoint(float x, float y, float length, out float alpha1, out float alpha2) {
            //float distance = (float)Math.Sqrt(x * x + y * y);
            //float tmpAlpha2 = MathHelper.ConvertToDegrees(Math.Acos(-((Math.Pow(distance, 2) - Math.Pow(length, 2) - Math.Pow(length, 2)) / (2 * length * length))));
            //alpha2 = 180 - tmpAlpha2;
            //float tmpAlpha1 = 90 - (tmpAlpha2 / 2);

            //alpha1 = MathHelper.ConvertToDegrees(Math.Atan(y / x)) - tmpAlpha1;
            float distance = (float)Math.Sqrt(x * x + y * y);
            float tmpAlpha2 = MathHelper.ConvertToDegrees(2 * Math.Asin((distance / 2) / length));
            alpha2 = 180 - tmpAlpha2;
            float tmpAlpha1 = 90 - (tmpAlpha2 / 2);

            alpha1 = MathHelper.ConvertToDegrees(Math.Asin(y / distance)) - tmpAlpha1;

        }

        //private int CalculateDistance(int toolX, int toolY, int x, int y) {
        //    int originQuadrant = MathHelper.GetQuadrant(toolX, toolY);
        //    int targetQuadrant = MathHelper.GetQuadrant(x, y);
        //    if (originQuadrant == targetQuadrant) {
        //        return x - toolX;
        //    }
        //    switch (originQuadrant) {
        //        case 1: if (targetQuadrant == 2 || targetQuadrant == 3)
        //                return x + toolX;
        //            else return x - toolX;
        //            break;
        //        case 2: if (targetQuadrant == 

              
        //    }
        //}

        
        
    }
}
