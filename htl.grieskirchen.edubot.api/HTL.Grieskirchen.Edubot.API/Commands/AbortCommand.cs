using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    /// <summary>
    /// Used for aborting a robot's current action
    /// </summary>
    public class AbortCommand : ICommand

    {
        /// <summary>
        /// Executes the command with the given adapter
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
        public void Execute(Adapters.IAdapter adapter)
        {
            adapter.CmdQueue.Clear();  
            adapter.RaiseAbortEvent(null);
            new System.Threading.Thread(adapter.Abort).Start();
        }

        /// <summary>
        /// Determines wether the command can be executed or not by the given the adapter
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
        /// <returns>Returns a FailureEventArgs object if execution is not possible else returns null</returns>
        public FailureEventArgs CanExecute(Adapters.IAdapter adapter)
        {
            return null;
        }
    }
}
