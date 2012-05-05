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
            adapter.SetState(State.MOVING,true);
            InterpolationResult result = null;
            
            if (adapter.UsesIntegratedPathCalculation()) {
                result = Interpolation.Interpolation.InterpolateCircular(adapter, target,center);
                if (result != null)
                    adapter.InterpolationResult = result;
                else
                    return;
            }
            adapter.RaiseMovementStartedEvent(new MovementStartedEventArgs(result));
            new System.Threading.Thread(adapter.MoveCircularTo).Start(new object[]{target,center});
        }

        public FailureEventArgs CanExecute(IAdapter adapter)
        {
            if (adapter.GetState() == State.SHUTDOWN)
            {
                return new FailureEventArgs(new InvalidStateException("MVC-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet"));
            }

            return null;  
        }
    }
}
