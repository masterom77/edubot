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
    /// The UseToolCommand class represents a command which tells the robot to wether activate or deactivate its tool
    /// </summary>
    public class UseToolCommand : ICommand
    {
        bool activate;
        /// <summary>
        /// The Activate Property tells if the tool should be activated or deactivated
        /// </summary>
        public bool Activate
        {
            get { return activate; }
            set { activate = value; }
        }

        /// <summary>
        /// Creates a new UseToolCommand-Object using the given parameters
        /// </summary>
        /// <param name="activate">Indicates if the tool should be activated of deactivated</param>
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
            adapter.State = State.MOVING;
            adapter.RaiseToolUsed(new ToolUsedEventArgs(activate));
            new System.Threading.Thread(adapter.UseTool).Start(activate);
        }


        public FailureEventArgs CanExecute(IAdapter adapter)
        {
            if (adapter.State == State.SHUTDOWN){
                return new FailureEventArgs(State.SHUTDOWN,new InvalidStateException("UseTool-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet"));
            }
        return null;
        }
    }
}
