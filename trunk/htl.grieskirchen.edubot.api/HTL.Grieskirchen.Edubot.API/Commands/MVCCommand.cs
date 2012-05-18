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
        
        /// <summary>
        /// Initializes a new instance of the MVCCommand class
        /// </summary>
        /// <param name="target">The target point of the movement</param>
        /// <param name="center">The center point of the movement</param>
        public MVCCommand(Point3D target, Point3D center)
        {
            this.target = target;
            this.center = center;
        }

        Point3D target;
        /// <summary>
        /// Gets or sets the target point of the movement
        /// </summary>
        public Point3D Target
        {
            get { return target; }
            set { target = value; }
        }

        Point3D center;
        /// <summary>
        /// Gets or sets the center point of the movement
        /// </summary>
        public Point3D Center
        {
            get { return center; }
            set { center = value; }
        }

        /// <summary>
        /// Executes the command with the given adapter
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
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
            new System.Threading.Thread(adapter.MoveCircularTo).Start(new Point3D[]{target,center});
        }

        /// <summary>
        /// Determines wether the command can be executed or not by the given the adapter
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
        /// <returns>Returns a FailureEventArgs object if execution is not possible else returns null</returns>
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
