using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    /// <summary>
    /// Used for initializing a robot
    /// </summary>
    public class InitCommand : ICommand
    {
        /// <summary>
        /// Executes the command with the given adapter
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
        public void Execute(IAdapter adapter)
        {
            adapter.SetState(State.HOMING, true);
            adapter.AxisConfiguration = AxisConfiguration.Lefty;
            adapter.CmdQueue.Clear();
            adapter.RaiseHomingEvent(new HomingEventArgs(0.1125f));
            new System.Threading.Thread(adapter.Initialize).Start();
        }

        /// <summary>
        /// Determines wether the command can be executed or not by the given the adapter
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
        /// <returns>Returns a FailureEventArgs object if execution is not possible else returns null</returns>
        public FailureEventArgs CanExecute(IAdapter adapter)
        {
            return null;            
        }
    }
}
