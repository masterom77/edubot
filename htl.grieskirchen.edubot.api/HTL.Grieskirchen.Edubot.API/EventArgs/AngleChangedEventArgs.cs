using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    public class AngleChangedEventArgs : System.EventArgs
    {
        
        public float Angle
        {
            get { return result.Ticks/2*0.1125f; }
        }

        InterpolationResult result;

        public InterpolationResult Result
        {
            get { return result; }
            set { result = value; }
        }

        AxisType axisType;

        public AxisType AxisType
        {
            get { return axisType; }
        }

        public AngleChangedEventArgs(AxisType axisType, InterpolationResult result) : base() {
            this.axisType = axisType;
            this.result = result;
        }
    }
}
