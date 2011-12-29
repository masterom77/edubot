using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class InterpolationResult
    {
        List<InterpolationStep> steps;

        public List<InterpolationStep> Steps
        {
            get { return steps; }
            set { steps = value; }
        }

        float primarySpeed;

        public float PrimarySpeed
        {
            get { return primarySpeed; }
            set { primarySpeed = value; }
        }

        float secondarySpeed;

        public float SecondarySpeed
        {
            get { return secondarySpeed; }
            set { secondarySpeed = value; }
        }

        float tertiarySpeed;

        public float TertiarySpeed
        {
            get { return tertiarySpeed; }
            set { tertiarySpeed = value; }
        }
        

        public InterpolationResult()
        {
            steps = new List<InterpolationStep>();
        }

        public override string ToString()
        {
            string result = "";
            foreach (InterpolationStep step in steps) {
                result += step.Alpha1 + ";" + step.Alpha2 + ";" + step.Alpha3 + "&";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }



       


    }
}
