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
            adapter.SetState(State.MOVING,true);
            InterpolationResult result = null;
            if (adapter.RequiresPrecalculation) {
                result = Interpolation.Interpolation.InterpolateLinear(adapter, target);
                if (result != null)
                    adapter.InterpolationResult = result;
                else
                    return;
            }
            adapter.RaiseMovementStartedEvent(new MovementStartedEventArgs(result));
            new System.Threading.Thread(adapter.MoveStraightTo).Start(target);
        }


        public FailureEventArgs CanExecute(IAdapter adapter)
        {
            if (adapter.GetState() == State.SHUTDOWN)
            {
                return new FailureEventArgs(new InvalidStateException("MVS-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet"));
            }
            
            return null;
        }
    }
}
