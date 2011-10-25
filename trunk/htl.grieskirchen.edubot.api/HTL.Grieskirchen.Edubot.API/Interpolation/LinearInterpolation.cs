using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class LinearInterpolation : IInterpolationType
    {
        
        public InterpolationResult CalculatePath(ITool tool, int x, int y, int z, float length)
        {
            int steps = 100;
            float toolX = tool.X;
            float toolY = tool.Y;
            float incrX = (x-toolX)/steps;
            float incrY = (y-toolY)/steps;

            float[] primaryAngles = new float[steps];
            float[] secondaryAngles = new float[steps];
            float[] speeds = new float[2];

            float primarySpeed = incrX / incrY;
            float secondarySpeed = incrY / incrX;

            InterpolationResult result = new InterpolationResult();
            

            for (int i = 0; i < steps; i++) {
                toolX += incrX;
                toolY += incrY;              
                CalculateAngleForPoint(toolX, toolY, length,out primaryAngles[i],out secondaryAngles[i]);
            }

            result.Result.Add(AxisType.PRIMARY,new AxisData(primaryAngles,primarySpeed));
            result.Result.Add(AxisType.SECONDARY,new AxisData(secondaryAngles,secondarySpeed));
            return result;
            
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
