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

        public VirtualAdapter(IAdapter adapter)
            : base(adapter.EquippedTool, adapter.Length, adapter.Length2, adapter.VerticalToolRange, adapter.Transmission, adapter.MaxPrimaryAngle, adapter.MinPrimaryAngle, adapter.MaxPrimaryAngle, adapter.MinSecondaryAngle)
        {
            requiresPrecalculation = true;
        }

        public VirtualAdapter(Tool tool, float length, float length2)
            : base(tool, length, length2)
        {
            requiresPrecalculation = true;
        }

        public VirtualAdapter(Tool tool, float length, float length2, float verticalToolRange, float transmission)
            : base(tool, length, length2, verticalToolRange, transmission)
        {
            requiresPrecalculation = true;
        }

        public VirtualAdapter(Tool equippedTool, float length, float length2, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle)
            : base(equippedTool, length, length2, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            requiresPrecalculation = true;
        }

        public VirtualAdapter(Tool equippedTool, float length, float length2, float verticalToolRange, int transmission, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle)
            : base(equippedTool, length, length2, verticalToolRange, transmission, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            requiresPrecalculation = true;
        }
        //public VirtualAdapter(Tool tool, float length, float length2) : base(tool, length, length2)
        //{
        //    this.tool = tool;
        //    this.length = length;
        //    this.requiresPrecalculation = true;
        //    type = AdapterType.VIRTUAL;
        //    Listener = new VirtualStateListener();
        //}

        public override void MoveStraightTo(object param)
        {
            Point3D target = (Point3D)param;
            toolCenterPoint = target;
        }

        public override void MoveCircularTo(object param)
        {
            object[] parameters = (object[])param;
            Point3D target = (Point3D)parameters[0];
            Point3D center = (Point3D)parameters[1];
            toolCenterPoint = target;
        }

        public override void UseTool(object param)
        {
        }

        public override void Initialize(object param)
        {
            toolCenterPoint = new Point3D(length + length2, 0, verticalToolRange);
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


        public override bool IsStateUpdateAllowed()
        {
            return true;
        }
    }
}
