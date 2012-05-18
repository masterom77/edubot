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
    /// <summary>
    /// Used for changing the axis configuration of a robot
    /// </summary>
    public class ChangeConfigurationCommand : ICommand
    {
        AxisConfiguration newConfiguration;
        /// <summary>
        /// Initializes a new instance of the ChangeConfigurationCommand class
        /// </summary>
        /// <param name="newMode">The new axis configuration</param>
        public ChangeConfigurationCommand(AxisConfiguration newMode)
        {
            this.newConfiguration = newMode;
        }


        /// <summary>
        /// Executes the command with the given adapter
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
        public void Execute(IAdapter adapter)
        {

            adapter.SetState(State.MOVING, true);
            InterpolationResult result = null;
            if (adapter.UsesIntegratedPathCalculation()) {
                result = Interpolation.Interpolation.InterpolateConfigurationChange(adapter, newConfiguration);
                if (result != null)
                    adapter.InterpolationResult = result;
                else
                    return;
            }
            adapter.AxisConfiguration = newConfiguration;
           
            adapter.RaiseMovementStartedEvent(new MovementStartedEventArgs(result));
            new System.Threading.Thread(adapter.ChangeConfiguration).Start(newConfiguration);
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
