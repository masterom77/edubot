using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    public class PointOutOfRangeException : Exception
    {
        Point3D point;

        public PointOutOfRangeException(Point3D point, string msg)
            : base(msg)
        {
            this.point = point;
        }
    }
}
