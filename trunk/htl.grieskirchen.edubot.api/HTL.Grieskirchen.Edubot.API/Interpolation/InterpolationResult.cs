using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class InterpolationResult
    {
        public InterpolationResult(float angle, float speed) {
            this.angle = angle;
            this.speed = speed;
        }

        float angle;

        public long Ticks
        {
            get { return (long) angle * 6400; }
        }

        float speed;

        public float Speed
        {
            get { return speed; }
        }

        public float Angle {
            get { return angle; }
        }


    }
}
