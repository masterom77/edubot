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

        public static Point3D CalculateDirect(float alpha1, float alpha2, float length, float length2) {
            double beta = 180 - alpha2;
            double d = Math.Sqrt((length * length + length2 * length2) - 2 * length * length2 * Math.Cos(MathHelper.ConvertToRadians(beta)));
            //double d = (length * (Math.Tan(MathHelper.ConvertToRadians((beta - gamma) / 2)) + Math.Tan(MathHelper.ConvertToRadians((beta + gamma) / 2)))) / ((Math.Tan(MathHelper.ConvertToRadians((beta + gamma) / 2)) - Math.Tan(MathHelper.ConvertToRadians((beta - gamma) / 2))));
            double alpha = MathHelper.ConvertToDegrees(Math.Acos((d * d + length * length - length2 * length2) / (2 * d * length)));
            double temp = alpha + alpha1;

            float x = (float)(d * Math.Cos(MathHelper.ConvertToRadians(temp))); 
            float y = (float) (d * Math.Sin(MathHelper.ConvertToRadians(temp)));
            
            
            return new Point3D(x,y,0);
        }

        /// <summary>
        /// Calculates the angles using the specified point and lengths
        /// </summary>
        /// <param name="target">A specific point</param>
        /// <param name="length">The length of the first axis</param>
        /// <param name="length2">The length of the second axis</param>
        /// <returns></returns>
        public static InterpolationStep CalculateInverse(Point3D target, float length, float length2)
        {
            float x = target.X;
            float y = target.Y;
            float z = target.Z;
            float d = (float)Math.Round(Math.Sqrt(x * x + y * y),4);

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
            if (Math.Round(x) == 0 && Math.Round(y) == 0)
            {
                return new InterpolationStep(target, 90, 180, float.NaN);
            }
            if (Math.Round(x) == length + length2 && Math.Round(y) == 0)
            {
                return new InterpolationStep(target, 0, 0, float.NaN);
            }
            if (Math.Round(x) == -(length + length2) && Math.Round(y) == 0)
            {
                return new InterpolationStep(target, 180, 0, float.NaN);
            }
            if (Math.Round(x) == 0 && Math.Round(y) == length + length2)
            {
                return new InterpolationStep(target, 90, 0, float.NaN);
            }
            if (Math.Round(x) == 0 && Math.Round(y) == -(length + length2))
            {
                return new InterpolationStep(target, 270, 0, float.NaN);
            }
            #endregion

            double alpha = MathHelper.ConvertToDegrees(Math.Acos((d * d + length * length - length2 * length2) / (2 * d * length)));
            double beta = MathHelper.ConvertToDegrees(Math.Acos((length2 * length2 + length * length - d * d) / (2 * length2 * length)));
            double gamma = MathHelper.ConvertToDegrees(Math.Acos((d * d + length2 * length2 - length * length) / (2 * d * length2)));


            double temp = MathHelper.ConvertToDegrees(Math.Acos(x / d));
            if (double.IsNaN(temp)) {
                temp = MathHelper.ConvertToDegrees(Math.Acos(Math.Round(x / d,4)));
            }
            int quadrant = MathHelper.GetQuadrant(x, y);
            if (quadrant == 3 || quadrant == 4) {
                temp = -temp;
            }

            return new InterpolationStep(target,(float)(temp - alpha),(float)(180 - beta),float.NaN); 
        }

        public static InterpolationStep CalculateInverse(Point3D target, float length, float length2, float transmission)
        {
            float x = target.X;
            float y = target.Y;
            float z = target.Z;
            float d = (float)Math.Round(Math.Sqrt(x * x + y * y),2);
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
            if (Math.Round(x) == 0 && Math.Round(y) == 0)
            {
                return new InterpolationStep(target, -90, 180, z * transmission);
            }
            if (Math.Round(x) == length + length2 && Math.Round(y) == 0)
            {
                return new InterpolationStep(target, 0, 0, z*transmission);
            }
            if (Math.Round(x) == -(length + length2) && Math.Round(y) == 0)
            {
                return new InterpolationStep(target, 180, 0, z * transmission);
            }
            if (Math.Round(x) == 0 && Math.Round(y) == length + length2)
            {
                return new InterpolationStep(target, 90, 0, z * transmission);
            }
            if (Math.Round(x) == 0 && Math.Round(y) == -(length + length2))
            {
                return new InterpolationStep(target, -90, 0, z * transmission);
            }
            #endregion

            double alpha = MathHelper.ConvertToDegrees(Math.Acos((d * d + length * length - length2 * length2) / (2 * d * length)));
            double beta = MathHelper.ConvertToDegrees(Math.Acos((length2 * length2 + length * length - d * d) / (2 * length2 * length)));
            double gamma = MathHelper.ConvertToDegrees(Math.Acos((d * d + length2 * length2 - length * length) / (2 * d * length2)));


            double temp = MathHelper.ConvertToDegrees(Math.Acos(x / d));
            if (double.IsNaN(temp))
            {
                temp = MathHelper.ConvertToDegrees(Math.Acos(Math.Round(x / d, 4)));
            }
            int quadrant = MathHelper.GetQuadrant(x, y);
            if (quadrant == 3 || quadrant == 4)
            {
                temp = -temp;
            }


            return new InterpolationStep(target, (float)(temp - alpha), (float)(180 - beta), z * transmission);
        }
    }
}
