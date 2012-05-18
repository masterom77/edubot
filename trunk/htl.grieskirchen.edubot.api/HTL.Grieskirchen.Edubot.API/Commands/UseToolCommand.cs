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
    /// Used for activating or deactivating a tool
    /// </summary>
    public class UseToolCommand : ICommand
    {
        bool activate;
        /// <summary>
        /// Gets or sets the value, indicating wether the tool should activated or deactivated
        /// </summary>
        public bool Activate
        {
            get { return activate; }
            set { activate = value; }
        }

        /// <summary>
        /// Creates a new instance of the UseToolCommand with the given values
        /// </summary>
        /// <param name="activate">A value indicating wether the tool should be activated or deactivated</param>
        public UseToolCommand(bool activate)
        {
            this.activate = activate;
        }

        /// <summary>
        /// Executes the command using the given parameters
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
        public void Execute(IAdapter adapter)
        {
            adapter.SetState(State.MOVING, true);
            adapter.RaiseToolUsed(new ToolUsedEventArgs(activate));
            new System.Threading.Thread(adapter.UseTool).Start(activate);
        }

        /// <summary>
        /// Determines wether the command can be executed or not by the given the adapter
        /// </summary>
        /// <param name="adapter">The adapter which should be used for execution</param>
        /// <returns>Returns a FailureEventArgs object if execution is not possible else returns null</returns>
        public FailureEventArgs CanExecute(IAdapter adapter)
        {
            if (adapter.GetState() == State.SHUTDOWN){
                return new FailureEventArgs(new InvalidStateException("UseTool-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet"));
            }
        return null;
        }
    }
}
