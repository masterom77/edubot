﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    /// <summary>
    /// Used for a linear or PTP movement
    /// </summary>
    public class MVSCommand : ICommand
    {
        Point3D target;
        /// <summary>
        /// Initializes a new instance of the MVSCommand class
        /// </summary>
        /// <param name="target">The target point of the movement</param>
        public MVSCommand(Point3D target)
        {
            this.target = target;
        }

        /// <summary>
        /// Gets or sets the target point of the movement
        /// </summary>
        public Point3D Target
        {
            get { return target; }
            set { target = value; }
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
                result = Interpolation.Interpolation.InterpolateLinear(adapter, target);
                if (result != null)
                    adapter.InterpolationResult = result;
                else
                    return;
            }
            adapter.RaiseMovementStartedEvent(new MovementStartedEventArgs(result));
            new System.Threading.Thread(adapter.MoveStraightTo).Start(target);
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
                return new FailureEventArgs(new InvalidStateException("MVS-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet"));
            }
            
            return null;
        }
    }
}
