using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class InterpolationResult
    {
        float[,] angles;

        public float[,] Angles
        {
            get { return angles; }
            set { angles = value; }
        }

        float[] velocities;

        public float[] Velocities
        {
            get { return velocities; }
            set { velocities = value; }
        }


        public InterpolationResult(int steps, int dimensions)
        {
            angles = new float[steps, dimensions];
            velocities = new float[dimensions];
            //result = new Dictionary<AxisType, AxisData>();
        }

        //Dictionary<AxisType, AxisData> result;

        //public Dictionary<AxisType, AxisData> Result
        //{
        //    get { return result; }
        //    set { result = value; }
        //}


       


    }
}
