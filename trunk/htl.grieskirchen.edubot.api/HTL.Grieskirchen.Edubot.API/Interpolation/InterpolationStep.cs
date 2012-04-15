using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class InterpolationStep
    {
        public static InterpolationStep operator +(InterpolationStep step1, InterpolationStep step2) {
            InterpolationStep step = new InterpolationStep();
            step.alpha1 = step1.alpha1 + step2.alpha1;
            step.alpha2 = step1.alpha2 + step2.alpha2;
            return step;
        }

        public static InterpolationStep operator -(InterpolationStep step1, InterpolationStep step2)
        {
            InterpolationStep step = new InterpolationStep();
            step.alpha1 = step1.alpha1 - step2.alpha1;
            step.alpha2 = step1.alpha2 - step2.alpha2;
            return step;
        }

        float alpha1;
        /// <summary>
        /// The alpha1-angle
        /// </summary>
        public float Alpha1
        {
            get { return alpha1; }
            set { alpha1 = value; }
        }

        float alpha2;
        /// <summary>
        /// The alpha2-angle
        /// </summary>
        public float Alpha2
        {
            get { return alpha2; }
            set { alpha2 = value; }
        }

        float alpha3;
        /// <summary>
        /// The alpha3-angle
        /// </summary>
        public float Alpha3
        {
            get { return alpha3; }
            set { alpha3 = value; }
        }

        int alpha1Sleep;

        public int Alpha1Sleep
        {
            get { return alpha1Sleep; }
            set { alpha1Sleep = value; }
        }

        int alpha2Sleep;

        public int Alpha2Sleep
        {
            get { return alpha2Sleep; }
            set { alpha2Sleep = value; }
        }
        
        /// <summary>
        /// Converts the content into a sendable format
        /// </summary>
        /// <returns>A string with format "alpha1;alpha2;alpha3"</returns>
        public override string ToString()
        {
            NumberFormatInfo info = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
            info.NumberDecimalSeparator = ".";
            //return Convert.ToInt32(Math.Round(alpha1 / 0.1125)) + ";" + Convert.ToInt32(Math.Round(alpha2 / 0.1125)) + ";" + Convert.ToInt32(Math.Round(alpha3 / 0.1125));
            return alpha1.ToString(info) + ";" + alpha2.ToString(info) + ";" + alpha3.ToString(info);
        }

    }
}
