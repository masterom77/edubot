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

        public Point3D(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        int z;

        public int Z
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
