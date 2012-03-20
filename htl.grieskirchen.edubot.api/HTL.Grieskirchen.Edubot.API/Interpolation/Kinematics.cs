﻿using System;
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

        /// <summary>
        /// Calculates a point from the specified angles and lengths
        /// </summary>
        /// <param name="angle1"></param>
        /// <param name="angle2"></param>
        /// <param name="length"></param>
        /// <param name="length2"></param>
        /// <returns></returns>
        public static Point3D CalculateDirect(float angle1, float angle2, float length, float length2) {

            double tmpAlpha = MathHelper.ConvertToDegrees(Math.Atan(length2 / length));
            double helpAlpha = angle1 + tmpAlpha;
            double tmpBeta = MathHelper.ConvertToDegrees(Math.Atan(length / length2));

            double r = Math.Sqrt(length * length + length2 * length2 - 2 * length * length2 * Math.Cos(MathHelper.ConvertToRadians(180 - tmpAlpha - tmpBeta)));

            double x = Math.Sin(MathHelper.ConvertToRadians(angle1)) * r;
            double y = Math.Cos(MathHelper.ConvertToRadians(angle1)) * r;
            double x2 = Math.Sin(MathHelper.ConvertToRadians(angle2)) * r;
            double y2 = Math.Cos(MathHelper.ConvertToRadians(angle2)) * r;
      
            return new Point3D(0, 0, 0);
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
            float r = (float) Math.Sqrt(x * x + y * y);

            #region ------------------------Point Validation------------------------
            if ((Math.Sqrt(Math.Round(x) * Math.Round(x) + Math.Round(y) * Math.Round(y)) > (length + length2))||
                (r < Math.Abs(length - length2))
                )
                throw new OutOfRangeException(new Point3D(Convert.ToInt32(x), Convert.ToInt32(y), 0), "Der Punkt (" + Math.Round(x) + "," + Math.Round(y) + ",0) befindet sich nicht im Arbeitsbereichs des Roboters");
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

            #region ------------------------Axis Triangle Calculation------------------------

            List<float> lengths = new List<float> { r, length, length2 };
            float c = lengths.Max();
            lengths.Remove(c);
            float b = lengths.Max();
            lengths.Remove(b);
            float a = lengths.First();

            float alpha = MathHelper.ConvertToDegrees(Math.Acos((b * b + c * c - a * a) / (2 * b * c)));
            float beta = MathHelper.ConvertToDegrees(Math.Acos((a * a + c * c - b * b) / (2 * a * c)));
            float gamma = 180 - alpha - beta;

            #endregion

            #region ------------------------Distance Triangle Calculation------------------------
            float helpAngle = MathHelper.ConvertToDegrees(Math.Acos(x / r));
            #endregion        

            #region ------------------------Angle Transformation and Distribution------------------------
            int quadrant = MathHelper.GetQuadrant(x, y);
            float alpha1 = float.NaN;
            float alpha2 = float.NaN;

            if (length >= length2 && length >= r) {
                 switch (quadrant)
                {
                         //BETA = GAMMA 
                        //GAMMA & ALPHA
                    case 1: alpha1 = helpAngle - beta;
                        break;
                    case 2: alpha1 = helpAngle - beta; //180-
                        break;
                    case 3: alpha1 = -helpAngle - beta; //180+
                        break;
                    case 4: alpha1 = -helpAngle - beta; //360-
                        break;
                }
                alpha2 = 180 - alpha;
                if(displayResults)
                    Console.WriteLine("QDR: " + quadrant + " LMAX alpha:\t" + Math.Round(alpha, 2) + " beta:\t" + Math.Round(beta, 2) + " gamma:\t" + Math.Round(gamma, 2) + "\t" + Math.Round(alpha1, 2) + "\t" + Math.Round(alpha2, 2));
                return new InterpolationStep() { Alpha1 = alpha1, Alpha2 = alpha2 };
            }
            if (length2 >= length && length2 >= r)
            {
                switch (quadrant)
                {
                    case 1: alpha1 = helpAngle - gamma;
                        break;
                    case 2: alpha1 = helpAngle - gamma;
                        break;
                    case 3: alpha1 = -helpAngle - gamma;
                        break;
                    case 4: alpha1 = -helpAngle - gamma;
                        break;
                }
                alpha2 = 180 - alpha;
                if (displayResults)
                    Console.WriteLine("QDR: " + quadrant + " L2MAX alpha:\t" + Math.Round(alpha, 2) + " beta:\t" + Math.Round(beta, 2) + " gamma:\t" + Math.Round(gamma, 2) + "\t" + Math.Round(alpha1, 2) + "\t" + Math.Round(alpha2, 2));
                return new InterpolationStep() { Alpha1 = alpha1, Alpha2 = alpha2 };
            }
            else
            {
                //ALPHA = BETA
                //DEFAULT: BETA + GAMMA
                switch (quadrant)
                {
                    case 1: alpha1 = helpAngle - alpha;
                        break;
                    case 2: alpha1 = helpAngle - alpha;
                        break;
                    case 3: alpha1 = -helpAngle - alpha;
                        break;
                    case 4: alpha1 = -helpAngle - alpha;
                        break;
                }
                alpha2 = 180 - gamma;
                if (displayResults)
                    Console.WriteLine("QDR: " + quadrant + " RMAX alpha:\t" + Math.Round(alpha, 2) + " beta:\t" + Math.Round(beta, 2) + " gamma:\t" + Math.Round(gamma, 2) + "\t" + Math.Round(alpha1, 2) + "\t" + Math.Round(alpha2, 2));
                return new InterpolationStep() { Alpha1 = alpha1, Alpha2 = alpha2 };
            }
            #endregion
        }
    }
}