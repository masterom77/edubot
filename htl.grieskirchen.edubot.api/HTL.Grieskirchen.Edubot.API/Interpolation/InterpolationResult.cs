using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class InterpolationResult
    {
        public InterpolationResult(float[,] angles, float[] speeds) {
            this.angles = angles;
            this.speeds = speeds;
        }

        float[,] angles;

        public long[,] Ticks
        {
            get
            {
                long[,] ticks = new long[2, angles.Length];
                for(int i = 0; i < ticks.Length; i++){
                    ticks[0,i] = (long) angles[0,i] * 6400;
                    ticks[1,i] = (long) angles[1,i] * 6400;
                }
                return ticks;
            }
        }

        float[] speeds;

        public float[] Speeds
        {
            get { return speeds; }
        }

        public float[,] Angles {
            get { return angles; }
        }


    }
}
