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
        /// Initializes the Kinematics class
        /// </summary>
        public Kinematics() {
            displayResults = false;
        }

        private static bool displayResults;

        /// <summary>
        /// Gets or sets a value, indicating wether the results should be printed in console.
        /// CAUTION: Should only be used for debugging since it slows the calculation drastically.
        /// </summary>
        public static bool DisplayResults
        {
            get { return Kinematics.displayResults; }
            set { Kinematics.displayResults = value; }
        }

        public static Point3D CalculateDirect(float beta, float gamma, float temp, float length, float length2) {

            double d = (length * (Math.Tan(MathHelper.ConvertToRadians((beta - gamma) / 2)) + Math.Tan(MathHelper.ConvertToRadians((beta + gamma) / 2)))) / ((Math.Tan(MathHelper.ConvertToRadians((beta + gamma) / 2)) - Math.Tan(MathHelper.ConvertToRadians((beta - gamma) / 2))));
            float y = (float) (d * Math.Sin(MathHelper.ConvertToRadians(temp)));
            float x = (float) Math.Sqrt(d * d - y * y);
            
            return new Point3D(x,y,0);
        }

        /// <summary>
        /// Calculates the angles using the specified point and lengths
        /// </summary>
        /// <param name="point">A specific point</param>
        /// <param name="length">The length of the first axis</param>
        /// <param name="length2">The length of the second axis</param>
        /// <returns></returns>
        public static InterpolationStep CalculateInverse(Point3D point, float length, float length2)
        {
            float x = point.X;
            float y = point.Y;
            float z = point.Z;
            float d = (float)Math.Sqrt(x * x + y * y);

            #region ------------------------Point Validation------------------------
            //if (Math.Round(d) > (length + length2) ||
            //    (d < Math.Abs(length - length2))
            //    )
            //    throw new OutOfRangeException(new Point3D(Convert.ToInt32(x), Convert.ToInt32(y), 0), "Der Punkt (" + Math.Round(x) + "/" + Math.Round(y) + "/0) befindet sich nicht im Arbeitsbereichs des Roboters");
            //if (length != length2) {
            //    float difLength = Math.Abs(length - length2);
            //    if (r <= difLength)
            //        throw new OutOfRangeException(new Point3D(Convert.ToInt32(x), Convert.ToInt32(y), 0), "Der Punkt (" + Math.Round(x) + "," + Math.Round(y) + ",0) befindet sich nicht im Arbeitsbereichs des Roboters");
            //}
            #endregion

            #region ------------------------Special Cases------------------------
            if (Math.Round(x) == length + length2 && Math.Round(y) == 0)
            {
                return new InterpolationStep() { Alpha1 = 0, Alpha2 = 0 };
            }
            if (Math.Round(x) == -(length + length2) && Math.Round(y) == 0)
            {
                return new InterpolationStep() { Alpha1 = 180, Alpha2 = 0 };
            }
            if (Math.Round(x) == 0 && Math.Round(y) == length + length2)
            {
                return new InterpolationStep() { Alpha1 = 90, Alpha2 = 0 };
            }
            if (Math.Round(x) == 0 && Math.Round(y) == -(length + length2))
            {
                return new InterpolationStep() { Alpha1 = 270, Alpha2 = 0 };
            }
            #endregion

            double alpha = MathHelper.ConvertToDegrees(Math.Acos((d * d + length * length - length2 * length2) / (2 * d * length)));
            double beta = MathHelper.ConvertToDegrees(Math.Acos((length2 * length2 + length * length - d * d) / (2 * length2 * length)));
            double gamma = MathHelper.ConvertToDegrees(Math.Acos((d * d + length2 * length2 - length * length) / (2 * d * length2)));


            double temp = MathHelper.ConvertToDegrees(Math.Acos(x / d));
            int quadrant = MathHelper.GetQuadrant(x, y);
            if (quadrant == 3 || quadrant == 4) {
                temp = -temp;
            }

            //Console.WriteLine("Soll-Wert :" + x + "/" + y);
            //Point3D calculated = CalculateDirect((float)beta, (float)gamma, (float)temp, length, length2);
            //Console.WriteLine("Ist-Wert :" + calculated.X + "/" + calculated.Y);
            return new InterpolationStep() { Alpha1 = (float)(temp - alpha), Alpha2 = (float)(180 - beta) }; 
        }
    }
}
