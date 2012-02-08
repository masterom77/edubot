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
    public class MVSCommand : ICommand
    {
        Point3D target;

        public MVSCommand(Point3D target)
        {
            this.target = target;
        }

        public Point3D Target
        {
            get { return target; }
            set { target = value; }
        }


        public void Execute(IAdapter adapter)
        {
            InterpolationResult result = null;
            if (adapter.State == State.SHUTDOWN)
                throw new InvalidStateException("MVS-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet");
            if (adapter.RequiresPrecalculation) {
                result = new LinearInterpolation().CalculatePath(adapter.Tool.ToolCenterPoint, target, adapter.Length, adapter.Length2);
                adapter.SetInterpolationResult(result);
            }
            adapter.State = State.MOVING;
            if (adapter.OnMovementStarted != null)
                adapter.OnMovementStarted(adapter, new MovementStartedEventArgs(result));
            new System.Threading.Thread(adapter.MoveStraightTo).Start(target);
        }
    }
}
