using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API
{
    public abstract class ITool
    {
        Point3D toolCenterPoint;

        public Point3D ToolCenterPoint
        {
            get { return toolCenterPoint; }
            set { toolCenterPoint = value; }
        }


        //public abstract void Activate();
        //public abstract void Deactivate();

    }
}
