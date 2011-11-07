using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API
{
    public class AxisData
    {
        public AxisData(float[] angles, float speed)
        {
            this.angles = angles;
            this.speed = speed;
        }

        float[] angles;

        public float[] Angles
        {
            get { return angles; }
        }

        long[] ticks;

        public long[] Ticks
        {
            get { return (from angle in angles
                             select (long) angle *3200).ToArray<long>(); }
        }

        float speed;

        public float Speed
        {
            get { return speed; }
        }

    }
}
