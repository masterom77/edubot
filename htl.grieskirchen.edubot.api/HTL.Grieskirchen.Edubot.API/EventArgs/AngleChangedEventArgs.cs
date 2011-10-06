using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    public class AngleChangedEventArgs : System.EventArgs
    {
        
        public float Angle
        {
            get { return ticks/2*0.1125f; }
        }

        long ticks;

        public long Ticks
        {
            get { return ticks; }
        }

        float speed;

        public float Speed
        {
            get { return speed; }
        }

        AxisType axisType;

        public AxisType AxisType
        {
            get { return axisType; }
        }

        public AngleChangedEventArgs(AxisType axisType, long ticks, float speed) : base() {
            this.axisType = axisType;
            this.ticks = ticks;
            this.speed = speed;
        }
    }
}
