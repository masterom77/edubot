using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
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

        public static Point3D operator + (Point3D point1, Point3D point2){
            return new Point3D(point1.x + point2.x, point1.y + point2.y, point1.z + point2.z);
        }

        public static Point3D operator -(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.x - point2.x, point1.y - point2.y, point1.z - point2.z);
        }

        public static Point3D operator *(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.x * point2.x, point1.y * point2.y, point1.z * point2.z);
        }

        public static Point3D operator *(Point3D point1, float factor)
        {
            return new Point3D(point1.x*factor, point1.y*factor, point1.z*factor);
        }



        public override string ToString()
        {
            return x + ";" + y + ";" + z;
        }
    }
}
