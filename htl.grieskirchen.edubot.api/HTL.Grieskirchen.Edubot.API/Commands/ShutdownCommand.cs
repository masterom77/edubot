using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class ShutdownCommand : ICommand
    {
        public ShutdownCommand()
        {
        }

        public void Execute(IAdapter adapter)
        {
            
            adapter.State = State.SHUTTING_DOWN;
            new System.Threading.Thread(adapter.Shutdown).Start();
           
        }


        public FailureEventArgs CanExecute(IAdapter adapter)
        {
            if (adapter.State == State.SHUTDOWN)
            {
                return new FailureEventArgs(State.SHUTDOWN, new InvalidStateException("Shutdown-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet"));
            }
            return null;
        }
    }
}
