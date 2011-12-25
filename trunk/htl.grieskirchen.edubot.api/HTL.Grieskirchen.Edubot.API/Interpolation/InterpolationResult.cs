using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class InterpolationResult
    {
        int[, ,] angles;
        public InterpolationResult()
        {
            result = new Dictionary<AxisType, AxisData>();
        }

        Dictionary<AxisType, AxisData> result;

        public Dictionary<AxisType, AxisData> Result
        {
            get { return result; }
            set { result = value; }
        }


       


    }
}
