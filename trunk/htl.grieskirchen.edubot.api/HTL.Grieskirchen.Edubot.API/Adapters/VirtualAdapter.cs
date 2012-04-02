using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class VirtualAdapter : IAdapter
    {
        InterpolationResult result;

        public VirtualAdapter()
            : base()
        {
            type = AdapterType.VIRTUAL;
            requiresPrecalculation = true;
            Listener = new VirtualStateListener();
        }

        public VirtualAdapter(ITool tool, float length, float length2) : base(tool, length, length2)
        {
            this.tool = tool;
            this.length = length;
            this.requiresPrecalculation = true;
            type = AdapterType.VIRTUAL;
            Listener = new VirtualStateListener();
        }

        public override void MoveStraightTo(object param)
        {
            Point3D target = (Point3D)param;
            tool.ToolCenterPoint = target;
        }

        public override void MoveCircularTo(object param)
        {
            object[] parameters = (object[])param;
            Point3D target = (Point3D)parameters[0];
            Point3D center = (Point3D)parameters[1];
            tool.ToolCenterPoint = target;
        }

        public override void UseTool(object param)
        {
        }

        public override void SetInterpolationResult(InterpolationResult result)
        {
            this.result = result;  
        }

        public override void Start(object param)
        {
            tool.ToolCenterPoint = new Point3D(length + length2, 0, 0);
            //State = State.READY;
        }

        public override void Shutdown()
        {
            State = State.SHUTDOWN;
        }

        public override void Abort()
        {
            Shutdown();
        }
    }
}
