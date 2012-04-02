using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    public class HomingEventArgs : System.EventArgs
    {

        float correctionAngle;

        public float CorrectionAngle
        {
            get { return correctionAngle; }
            set { correctionAngle = value; }
        }

        //AxisType axisType;

        //public AxisType AxisType
        //{
        //    get { return axisType; }
        //}

        public HomingEventArgs(float correctionAngle)
            : base()
        {
            //this.axisType = axisType;
            this.correctionAngle = correctionAngle;
        }
    }
}
