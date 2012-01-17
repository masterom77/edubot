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
    public class LinearInterpolation : IInterpolationType
    {
        /// <summary>
        /// Calculates the path by using the specified parameters and linear interpolation.
        /// </summary>
        /// <param name="tool">The tool of the robot</param>
        /// <param name="x">The target-X coordinate</param>
        /// <param name="y">The target-Y coordinate</param>
        /// <param name="z">The target-Z coordinate</param>
        /// <param name="length">The length of both axis</param>
        /// <returns></returns>
        public InterpolationResult CalculatePath(ITool tool, Point3D target, float length)
        {
            int steps = Configuration.InterpolationSteps;
            float toolX = tool.X;
            float toolY = tool.Y;
            float difX = target.X - toolX;
            //if ((x >= 0 && toolX <= 0)||(x <= 0 && toolX >= 0)){
            //    difX = Math.Abs(x) + Math.Abs(toolX);
            //    if (x < toolX) {
            //        difX *= -1;
            //    }
            //}
            float difY = target.Y - toolY;
            //if ((y >= 0 && toolY <= 0)||(y <= 0 && toolY >= 0)){
            //    difY = Math.Abs(y) + Math.Abs(toolY);
            //    if (y < toolY)
            //    {
            //        difY *= -1;
            //    }
            //}

            float incrX = difX / steps;
            float incrY = difY / steps;

            float[] primaryAngles = new float[steps];
            float[] secondaryAngles = new float[steps];
            float[] speeds = new float[2];

            InterpolationResult result = new InterpolationResult();
            InterpolationStep prevStep = CalculateStepForPoint(toolX, toolY, length);
            InterpolationStep step;
            result.PrimarySpeed = incrX / incrY;
            result.SecondarySpeed = incrY / incrX;

            for (int i = 0; i < steps; i++)
            {
                
                toolX += incrX;
                toolY += incrY;
                step = CalculateStepForPoint(toolX, toolY, length);// -step;
                result.Angles.Add(step);
                result.Steps.Add(step-prevStep);
                prevStep = step;
            }

            Console.WriteLine("a1 \t a2");
            int count = 0;
            foreach (InterpolationStep s in result.Angles)
            {
                Console.WriteLine("[" + count + "]\t" + s.Alpha1 + "\t" + s.Alpha2);
                count++;
            }
            
            return result;
            
        }

        /// <summary>
        /// Calculates the angles alpha1 and alpha2, when the tool is on a specific point
        /// </summary>
        /// <param name="x">The x-coordinate of the tool</param>
        /// <param name="y">The y-coordinate of the tool</param>
        /// <param name="length">The length of both axis</param>
        private InterpolationStep CalculateStepForPoint(float x, float y, float length) {
            //float distance = (float)Math.Sqrt(x * x + y * y);
            //float tmpAlpha2 = MathHelper.ConvertToDegrees(Math.Acos(-((Math.Pow(distance, 2) - Math.Pow(length, 2) - Math.Pow(length, 2)) / (2 * length * length))));
            //alpha2 = 180 - tmpAlpha2;
            //float tmpAlpha1 = 90 - (tmpAlpha2 / 2);
            if (Math.Sqrt(Math.Round(x) * Math.Round(x) + Math.Round(y) * Math.Round(y)) > (length * 2))
                throw new OutOfRangeException(new Point3D(Convert.ToInt32(x), Convert.ToInt32(y), 0), "Der Punkt (" + x + "," + y + ",0) befindet sich außerhalb der Reichweite des Roboters");

            int quadrant = MathHelper.GetQuadrant(x, y);
            //alpha1 = MathHelper.ConvertToDegrees(Math.Atan(y / x)) - tmpAlpha1;
            float distance = (float)Math.Sqrt(x * x + y * y);
            float tmpAlpha2 = MathHelper.ConvertToDegrees(2 * Math.Asin((distance / 2) / length));
            float alpha2 = 180 - tmpAlpha2;
            float tmpAlpha1 = 90 - (tmpAlpha2 / 2);

            switch (quadrant)
            {
                case 2: tmpAlpha1 = 180 - tmpAlpha1;
                    break;
                case 3: tmpAlpha1 = 180 + tmpAlpha1;
                    break;
                //case 4: tmpAlpha1 = 360 - tmpAlpha1;
                //    break;
            }
            float alpha1 = MathHelper.ConvertToDegrees(Math.Asin(y / distance)) - tmpAlpha1;
            switch (quadrant)
            {
                case 2: alpha1 *= -1;
                    break;
                case 3: alpha1 =  (330+(180 + alpha1))*-1;
                    break;
                //case 4: alpha1 = (360 + alpha1) * -1;
                //    break;
            }
            if (alpha1 == float.NaN || alpha2 == float.NaN)
                throw new NotFiniteNumberException();

           

            return new InterpolationStep() { Alpha1 = alpha1, Alpha2 = alpha2 };
        }


        
        
    }
}
