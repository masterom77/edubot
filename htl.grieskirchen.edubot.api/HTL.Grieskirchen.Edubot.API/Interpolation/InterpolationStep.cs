using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Converts the content into a sendable format
        /// </summary>
        /// <returns>A string with format "alpha1;alpha2;alpha3"</returns>
        public override string ToString()
        {
            return Convert.ToInt32(alpha1 / 0.1125) + ";" + Convert.ToInt32(alpha2 / 0.1125) + ";" + Convert.ToInt32(alpha3 / 0.1125);
        }

    }
}
