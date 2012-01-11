using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    public class OutOfRangeException : Exception
    {
        Point3D point;

        public OutOfRangeException(Point3D point, string msg)
            : base(msg)
        {
            this.point = point;
        }
    }
}
