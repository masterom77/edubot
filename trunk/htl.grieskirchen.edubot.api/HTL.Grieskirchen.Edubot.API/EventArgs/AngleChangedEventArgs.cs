﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    public class AngleChangedEventArgs : System.EventArgs
    {
        
        InterpolationResult result;

        public InterpolationResult Result
        {
            get { return result; }
            set { result = value; }
        }

        //AxisType axisType;

        //public AxisType AxisType
        //{
        //    get { return axisType; }
        //}

        public AngleChangedEventArgs(InterpolationResult result) : base() {
            //this.axisType = axisType;
            this.result = result;
        }
    }
}
