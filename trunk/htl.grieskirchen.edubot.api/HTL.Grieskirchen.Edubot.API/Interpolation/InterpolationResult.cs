using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class InterpolationResult
    {
        List<InterpolationStep> angles;

        /// <summary>
        /// The calculated interpolation-steps
        /// </summary>
        public List<InterpolationStep> Angles
        {
            get { return angles; }
            set { angles = value; }
        }

        List<InterpolationStep> steps;

        /// <summary>
        /// The calculated interpolation-steps
        /// </summary>
        public List<InterpolationStep> Steps
        {
            get { return steps; }
            set { steps = value; }
        }

        float primarySpeed;

        /// <summary>
        /// The suggested speed for the primary axis
        /// </summary>
        public float PrimarySpeed
        {
            get { return primarySpeed; }
            set { primarySpeed = value; }
        }
        
        float secondarySpeed;

        /// <summary>
        /// The suggested speed for the secondary axis
        /// </summary>
        public float SecondarySpeed
        {
            get { return secondarySpeed; }
            set { secondarySpeed = value; }
        }

        float tertiarySpeed;

        /// <summary>
        /// The suggested speed for the tertiary axis
        /// </summary>
        public float TertiarySpeed
        {
            get { return tertiarySpeed; }
            set { tertiarySpeed = value; }
        }
        
        /// <summary>
        /// A constructor with no parameters, which only initalizes the intern lists
        /// </summary>
        public InterpolationResult()
        {
            angles = new List<InterpolationStep>();
            steps = new List<InterpolationStep>();
        }

        private void GenerateAccelerationData() {
            for (int pos = 0; pos < steps.Count - 1; pos++)
            {
                int ticksAlpha1 = 0;
                AccelerationArea area = new AccelerationArea();
                area.StartPos = pos;
                if (((steps[pos].Alpha1 > 0 && steps[pos + 1].Alpha1 > 0) || (steps[pos].Alpha1 < 0 && steps[pos + 1].Alpha1 < 0)) && ((steps[pos].Alpha1 > 0 && steps[pos + 1].Alpha2 > 0) || (steps[pos].Alpha1 < 0 && steps[pos + 1].Alpha2 < 0)))
                {
                    
                }
                area.EndPos = 1;
            }
        }

        /// <summary>
        /// Converts the content into a sendable format
        /// </summary>
        /// <returns>A string with format "primarySpeed;secondarySpeed|step1&step2&step3..."</returns>
        public override string ToString()
        {
            
            float varianceAlpha1 = 0f;
            float varianceAlpha2 = 0f;
            string result = "";//primarySpeed+";"+secondarySpeed+"|";
            
            foreach (InterpolationStep step in steps) {
                if (Math.Abs(step.Alpha1) < Configuration.AnglePerStep)
                {
                    varianceAlpha1 += step.Alpha1;
                }
                if (Math.Abs(varianceAlpha1) >= Configuration.AnglePerStep)
                {
                    step.Alpha1 += varianceAlpha1;
                    varianceAlpha1 = 0f;
                }
                if (Math.Abs(step.Alpha2) < Configuration.AnglePerStep)
                {
                    varianceAlpha2 += step.Alpha2;
                }
                if (varianceAlpha2 >= Configuration.AnglePerStep)
                {
                    step.Alpha2 += varianceAlpha2;
                    varianceAlpha2 = 0f;
                }
                result += step.ToString() + "&";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }



       


    }
}
