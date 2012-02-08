using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class MVCCommand : ICommand
    {
        

        public MVCCommand(Point3D target, Point3D center)
        {
            this.target = target;
            this.center = center;
        }

        Point3D target;
        public Point3D Target
        {
            get { return target; }
            set { target = value; }
        }

        Point3D center;
        public Point3D Center
        {
            get { return center; }
            set { center = value; }
        }

        public void Execute(IAdapter adapter)
        {
            InterpolationResult result = null;
            if (adapter.State == State.SHUTDOWN)
                throw new InvalidStateException("MVC-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet");
            if (adapter.RequiresPrecalculation) {
                result = new CircularInterpolation().CalculatePath(adapter.Tool.ToolCenterPoint, target,center, adapter.Length, adapter.Length2);
                adapter.SetInterpolationResult(result);
            }
            adapter.State = State.MOVING;
            if (adapter.OnMovementStarted != null)
                adapter.OnMovementStarted(adapter, new MovementStartedEventArgs(result));
            new System.Threading.Thread(adapter.MoveCircularTo).Start(target);
        }
    }
}
