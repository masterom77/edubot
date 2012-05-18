using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    /// <summary>
    /// Represents a single interpolation steps
    /// </summary>
    public class InterpolationStep
    {
        /// <summary>
        /// Adds to interpolation steps together
        /// </summary>
        /// <param name="step1">The first step</param>
        /// <param name="step2">The second step</param>
        /// <returns>An interpolation step containing the sum of the given steps</returns>
        public static InterpolationStep operator +(InterpolationStep step1, InterpolationStep step2) {
            return new InterpolationStep(step1.Target+step2.Target, step1.Alpha1 + step2.Alpha1,step1.Alpha2+step2.Alpha2,step1.Alpha3+step2.Alpha3);
        }

        /// <summary>
        /// Subtracts the second from the first interpolation step
        /// </summary>
        /// <param name="step1">The first step</param>
        /// <param name="step2">The second step</param>
        /// <returns>An interpolation step containing the difference between the first and the second step</returns>
        public static InterpolationStep operator - (InterpolationStep step1, InterpolationStep step2)
        {
            return new InterpolationStep(step1.Target - step2.Target, step1.Alpha1 - step2.Alpha1, step1.Alpha2 - step2.Alpha2, step1.Alpha3 - step2.Alpha3);
        }

        /// <summary>
        /// Creates a new instance of the InterpolationStep class
        /// </summary>
        /// <param name="target">The point, which is reached by using the given angles</param>
        /// <param name="alpha1">The angle of the primary axis</param>
        /// <param name="alpha2">The angle of the secondary axis</param>
        /// <param name="alpha3">The angle of the tertiary axis</param>
        public InterpolationStep(Point3D target, float alpha1, float alpha2, float alpha3) {
            this.target = target;
            this.alpha1 = alpha1;
            this.alpha2 = alpha2;
            this.alpha3 = alpha3;
        }

        Point3D target;
        /// <summary>
        /// Gets or sets the point, which is reached by using the given angles
        /// </summary>
        public Point3D Target
        {
            get { return target; }
            set { target = value; }
        }

        float alpha1;
        /// <summary>
        /// Gets or sets the angle of the primary axis
        /// </summary>
        public float Alpha1
        {
            get { return alpha1; }
            set { alpha1 = value; }
        }

        float alpha2;
        /// <summary>
        /// Gets or sets the angle of the secondary axis
        /// </summary>
        public float Alpha2
        {
            get { return alpha2; }
            set { alpha2 = value; }
        }

        float alpha3;
        /// <summary>
        /// Gets or sets the angle of the tertiary axis
        /// </summary>
        public float Alpha3
        {
            get { return alpha3; }
            set { alpha3 = value; }
        }

        /// <summary>
        /// Converts the content into a sendable format
        /// </summary>
        /// <returns>A string with format "alpha1;alpha2;alpha3"</returns>
        public override string ToString()
        {
            NumberFormatInfo info = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
            info.NumberDecimalSeparator = ".";
            return alpha1.ToString(info) + ";" + alpha2.ToString(info) + ";" + alpha3.ToString(info);
        }



    }
}
