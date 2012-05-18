using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    /// <summary>
    /// Used for managing and converting data about a calculated path
    /// </summary>
    public class InterpolationResult
    {
        List<InterpolationStep> angles;
        
        /// <summary>
        /// The calculated interpolation-steps, represented through absolute angles
        /// </summary>
        public List<InterpolationStep> Angles
        {
            get { return angles; }
            set { angles = value; }
        }

        List<InterpolationStep> steps;

        /// <summary>
        /// The calculated interpolation-steps, represented through relative angles
        /// </summary>
        public List<InterpolationStep> Steps
        {
            get { return steps; }
            set { steps = value; }
        }


        List<Point3D> points;
        /// <summary>
        /// The calculated interpolation points
        /// </summary>
        public List<Point3D> Points
        {
            get { return points; }
            set { points = value; }
        }

        InterpolationType interpolationType;
        /// <summary>
        /// Gets or sets the type of interpolation used for calculating this result
        /// </summary>
        public InterpolationType InterpolationType
        {
            get { return interpolationType; }
            set { interpolationType = value; }
        }

        Hashtable metaData;
        /// <summary>
        /// Gets or sets a table of additional data concerning the calculation. Contains the keys "Clockwise" and "Radius", when circular interpolation is used
        /// </summary>
        public Hashtable MetaData
        {
            get { return metaData; }
            set { metaData = value; }
        }


        /// <summary>
        /// A constructor with no parameters, which only initalizes the intern lists
        /// </summary>
        public InterpolationResult(InterpolationType interpolationType)
        {
            this.interpolationType = interpolationType;
            angles = new List<InterpolationStep>();
            steps = new List<InterpolationStep>();
            points = new List<Point3D>();
            metaData = new Hashtable();
        }


        /// <summary>
        /// Converts the content into a sendable format, representing the angles in degrees
        /// </summary>
        /// <returns>A string with format "step1&step2&step3..."</returns>
        public override string ToString()
        {
            string result = "";
            
            foreach (InterpolationStep angle in angles) {
                result += angle.ToString() + "&";
            }
            if (result.Length > 0)
            {
                result = result.Remove(result.Length - 1);
            }
            return result;
        }


        /// <summary>
        /// Converts Angles into Steps and returns a sendable String, representing the angles in steps, required by the edubot model's stepper motor
        /// </summary>
        /// <returns>A string with format "step1&step2&step3..."</returns>
        public string ConverToStepString() {
            string result = "";

            double[] curSteps = new double[3];
            foreach (InterpolationStep step in steps) {
                curSteps[0] += step.Alpha1 / 0.1125;
                curSteps[1] += step.Alpha2 / 0.1125;
                curSteps[2] += step.Alpha3 / 0.1125;
                result += ((int)curSteps[0]) + ";" + ((int)curSteps[1]) + ";" + ((int)curSteps[2]) + "&";

                curSteps[0] = curSteps[0] % 1;
                curSteps[1] = curSteps[1] % 1;
                curSteps[2] = curSteps[2] % 1;
            }

            //foreach (InterpolationStep step in steps) {
            //    curSteps[0] += step.Alpha1 / 0.1125;
            //    curSteps[1] += step.Alpha2 / 0.1125;
            //    curSteps[2] += step.Alpha3 / 0.1125;

            //    if (curSteps[0]%1 > 0.5) {
            //        curSteps[0] += 1;
            //    }
            //    if (curSteps[1] % 1 > 0.5)
            //    {
            //        curSteps[1] += 1;
            //    }
            //    if (curSteps[2] % 1 > 0.5)
            //    {
            //        curSteps[2] += 1;
            //    }

            //    result += ((int)curSteps[0]) + ";" + ((int)curSteps[1]) + ";" + ((int)curSteps[2])+"&";

            //    if (curSteps[0] % 1 > 0.5)
            //    {
            //        curSteps[0] = (curSteps[0] % 1) - 1;
            //    }
            //    else {
            //        curSteps[0] = curSteps[0] % 1;
            //    }

            //    if (curSteps[1] % 1 > 0.5)
            //    {
            //        curSteps[1] = (curSteps[1] % 1) - 1;
            //    }
            //    else
            //    {
            //        curSteps[1] = curSteps[1] % 1;
            //    }

            //    if (curSteps[2] % 1 > 0.5)
            //    {
            //        curSteps[2] = (curSteps[2] % 1) - 1;
            //    }
            //    else
            //    {
            //        curSteps[2] = curSteps[2] % 1;
            //    }
                
            //}

            //if (result.Length > 0)
            //{
            //    result = result.Remove(result.Length - 1);
            //}
            //Console.WriteLine(ToString());
            //Console.WriteLine(result);
            return result;
        }
    }
}
