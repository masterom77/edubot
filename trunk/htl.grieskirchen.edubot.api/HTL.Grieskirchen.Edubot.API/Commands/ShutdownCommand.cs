using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class ShutdownCommand : ICommand
    {
        public ShutdownCommand()
        {
        }

        public void Execute(IAdapter adapter)
        {
            if (adapter.State == State.SHUTDOWN)
                throw new InvalidStateException("Shutdown-Command kann nicht ausgeführt werden, da sich der Roboter bereits im SHUTDOWN-Zustand befindet");
            adapter.State = State.SHUTTING_DOWN;
            new System.Threading.Thread(adapter.Shutdown).Start();
           
        }
    }
}
