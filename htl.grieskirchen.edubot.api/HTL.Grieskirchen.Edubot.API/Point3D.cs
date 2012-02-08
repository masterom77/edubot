using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API
{
    public class Point3D
    {
        public Point3D()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public Point3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        float x;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        float y;

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        float z;

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public override string ToString()
        {
            return x + ";" + y + ";" + z;
        }
    }
}
