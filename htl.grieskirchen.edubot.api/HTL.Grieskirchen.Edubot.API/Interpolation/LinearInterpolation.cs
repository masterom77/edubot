using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class LinearInterpolation : IInterpolationType
    {
        public float[,] CalculatePath(Axis primaryAxis, Axis secondaryAxis, Axis verticalAxis, Axis toolAxis, int x, int y, int z)
        {
            float toolX = secondaryAxis.X;
            float toolY = secondaryAxis.Y;
            float length = secondaryAxis.Length;

            //Calc distance between 0/0 and tool-Point
            float distance = (float) Math.Sqrt(x * x + y * y);
            //Calc angle between primary and secondary axis
            float tmpAlpha2 = MathHelper.ConvertToDegrees(Math.Acos(-((Math.Pow(distance, 2) - Math.Pow(length, 2) - Math.Pow(length, 2)) / (2 * length * length))));
            
            //float tmpAlpha2 = (float)(Math.Asin((distance / 2) / secondaryAxis.Length) * 180 / Math.PI);

            float alpha2 = 180 - tmpAlpha2;
            float tmpAlpha1 = 90 - (tmpAlpha2 / 2);

            float alpha1 = MathHelper.ConvertToDegrees(Math.Atan(y/x)) - tmpAlpha1;

            float difAlpha1;
            if (alpha1 > primaryAxis.Angle)
            {
                difAlpha1 = alpha1 - primaryAxis.Angle;
            }
            else {
                difAlpha1 = primaryAxis.Angle - alpha1;
            }

            float difAlpha2;
            if (alpha2 > secondaryAxis.Angle)
            {
                difAlpha2 = alpha2 - secondaryAxis.Angle;
            }
            else
            {
                difAlpha2 = secondaryAxis.Angle - alpha2;
            }

            float primSpeed = difAlpha1 / difAlpha2;
            float secSpeed = difAlpha2 / difAlpha1;

            float[,] results = Interpolate(primaryAxis.Angle, secondaryAxis.Angle, difAlpha1, difAlpha2, secondaryAxis.Length, 100);

            primaryAxis.Angle = alpha1;
            secondaryAxis.Angle = alpha2;
            secondaryAxis.X = x;
            secondaryAxis.Y = y;
            secondaryAxis.Z = z;

            return results;

            /*long primTicks = MathHelper.ConvertToTicks(alpha1);
            long secTicks = MathHelper.ConvertToTicks(alpha2);
            */

            //Point p = MathHelper.CalculateCoordinates(alpha1, alpha2, length);
            
            
        }

        private float[,] Interpolate(float startAngle1, float startAngle2, float dif1, float dif2, float length, int steps) {
            float[,] results = new float[2, steps];
            float alpha1Incr = dif1 / steps;
            float alpha2Incr = dif2 / steps;
            float alpha1 = startAngle1;
            float alpha2 = startAngle2;
            float tmpAlpha1;
            float tmpAlpha2;
            float distance;

            for (int i = 0; i < steps; i++)
            {
                alpha1 += alpha1Incr;
                alpha2 += alpha2Incr;
                Point nextPoint = MathHelper.CalculateCoordinates(alpha1, alpha2, length);
                distance = (float)Math.Sqrt(Math.Pow(nextPoint.X, 2) + Math.Pow(nextPoint.Y, 2));
                //Calc angle between primary and secondary axis
                tmpAlpha2 = MathHelper.ConvertToDegrees(Math.Acos(-((Math.Pow(distance, 2) - Math.Pow(length, 2) - Math.Pow(length, 2)) / (2 * length * length))));

                //float tmpAlpha2 = (float)(Math.Asin((distance / 2) / secondaryAxis.Length) * 180 / Math.PI);

                tmpAlpha1 = 90 - (tmpAlpha2 / 2);
                results[0, i] = MathHelper.ConvertToDegrees(Math.Atan(nextPoint.Y / nextPoint.X)) - tmpAlpha1;
                results[1, i] = 180 - tmpAlpha2;
            }

            return results;
        }
    }
}
