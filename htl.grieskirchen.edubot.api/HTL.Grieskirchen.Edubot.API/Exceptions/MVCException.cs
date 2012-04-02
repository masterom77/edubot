using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot.API.Exceptions
{
    public class MVCException : Exception
    {
        Point3D toolCenterPoint;

        public Point3D ToolCenterPoint
        {
            get { return toolCenterPoint; }
            set { toolCenterPoint = value; }
        }

        Point3D targetPoint;

        public Point3D TargetPoint
        {
            get { return targetPoint; }
            set { targetPoint = value; }
        }

        Point3D center;

        public Point3D Center
        {
            get { return center; }
            set { center = value; }
        }

        public MVCException(string msg, Point3D toolCenterPoint, Point3D targetPoint, Point3D center) : base(msg){
            this.toolCenterPoint = toolCenterPoint;
            this.targetPoint = targetPoint;
            this.center = center;
        }
    }
}
