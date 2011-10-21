using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class LinearInterpolation : IInterpolationType
    {
        public InterpolationResult CalculatePath(Axis primaryAxis, Axis secondaryAxis, Axis verticalAxis, Axis toolAxis, int x, int y, int z)
        {
            int steps = 100;
            float toolX = secondaryAxis.X+150;
            float toolY = secondaryAxis.Y;
            float incrX = (x-toolX)/steps;
            float incrY = (y-toolY)/steps;
            float length = secondaryAxis.Length;

            float[,] angles = new float[2, steps];
            float[] speeds = new float[2];

            speeds[0] = incrX / incrY;
            speeds[1] = incrY / incrX;
            
            for (int i = 0; i < steps; i++) {
                toolX += incrX;
                toolY += incrY;
              
                CalculateAngleForPoint(toolX, toolY, length,out angles[0, i],out angles[1, i]);
            }
            return new InterpolationResult(angles,speeds);
            
        }

        private void CalculateAngleForPoint(float x, float y, float length, out float alpha1, out float alpha2) {
            float distance = (float)Math.Sqrt(x * x + y * y);
            float tmpAlpha2 = MathHelper.ConvertToDegrees(Math.Acos(-((Math.Pow(distance, 2) - Math.Pow(length, 2) - Math.Pow(length, 2)) / (2 * length * length))));
            alpha2 = 180 - tmpAlpha2;
            float tmpAlpha1 = 90 - (tmpAlpha2 / 2);

            alpha1 = MathHelper.ConvertToDegrees(Math.Atan(y / x)) - tmpAlpha1;

        }

        
    }
}
