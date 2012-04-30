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


        List<Point3D> points;
        /// <summary>
        /// The calculated interplation points
        /// </summary>
        public List<Point3D> Points
        {
            get { return points; }
            set { points = value; }
        }

        float incrZ;

        public float IncrZ
        {
            get { return incrZ; }
            set { incrZ = value; }
        }

        /// <summary>
        /// A constructor with no parameters, which only initalizes the intern lists
        /// </summary>
        public InterpolationResult()
        {
            angles = new List<InterpolationStep>();
            steps = new List<InterpolationStep>();
            points = new List<Point3D>();
            incrZ = 0;
        }

        /// <summary>
        /// A constructor with no parameters, which only initalizes the intern lists
        /// </summary>
        public InterpolationResult(float slopeZ)
        {
            angles = new List<InterpolationStep>();
            steps = new List<InterpolationStep>();
            points = new List<Point3D>();
            this.incrZ = slopeZ;
       } 

        //private void GenerateAccelerationData() {
        //    for (int pos = 0; pos < steps.Count - 1; pos++)
        //    {
        //        int ticksAlpha1 = 0;
        //        AccelerationArea area = new AccelerationArea();
        //        area.StartPos = pos;
        //        if (((steps[pos].Alpha1 > 0 && steps[pos + 1].Alpha1 > 0) || (steps[pos].Alpha1 < 0 && steps[pos + 1].Alpha1 < 0)) && ((steps[pos].Alpha1 > 0 && steps[pos + 1].Alpha2 > 0) || (steps[pos].Alpha1 < 0 && steps[pos + 1].Alpha2 < 0)))
        //        {
                    
        //        }
        //        area.EndPos = 1;
        //    }
        //}

        /// <summary>
        /// Converts the content into a sendable format
        /// </summary>
        /// <returns>A string with format "primarySpeed;secondarySpeed|step1&step2&step3..."</returns>
        public override string ToString()
        {
            string result = "";
            
            foreach (InterpolationStep step in steps) {
                result += step.ToString() + "&";
            }
            if (result.Length > 0)
            {
                result = result.Remove(result.Length - 1);
            }
            return result;
        }



       


    }
}
