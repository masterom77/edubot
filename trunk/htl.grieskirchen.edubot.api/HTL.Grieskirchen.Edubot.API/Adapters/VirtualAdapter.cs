using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class VirtualAdapter : IAdapter
    {
        InterpolationResult result;

        public VirtualAdapter(ITool tool, float length) : base()
        {
            this.tool = tool;
            tool.X = (int)length * 2;
            tool.Y = 0;
            this.length = length;
            this.requiresPrecalculation = true;
            type = AdapterType.VIRTUAL;
            listener = new VirtualStateListener();
        }

        public override void MoveTo(object param)
        {
            Point3D target = (Point3D)param;
            tool.X = target.X;
            tool.Y = target.Y;
            tool.Z = target.Z;
            //Console.WriteLine("Moving...");
            //System.Threading.Thread.Sleep(1000);
            //State = State.READY;
        }

        public override void UseTool(object param)
        {
            //Console.WriteLine((((bool)param) ? "Activating" : "Deactivating") + " Tool...");
            //System.Threading.Thread.Sleep(1000);
            //State = State.READY;
        }

        public override void SetInterpolationResult(InterpolationResult result)
        {
            this.result = result;  
        }

        public override void Start()
        {
            //Console.WriteLine("Starting...");
            //System.Threading.Thread.Sleep(2000);
            State = State.READY;
        }

        public override void Shutdown()
        {

            //Console.WriteLine("Shutting down...");
            //System.Threading.Thread.Sleep(2000);
            State = State.SHUTDOWN;
        }
    }
}
