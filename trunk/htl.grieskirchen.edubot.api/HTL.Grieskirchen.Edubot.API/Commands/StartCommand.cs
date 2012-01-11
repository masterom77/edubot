using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class StartCommand : ICommand
    {
        public StartCommand()
        {
        }


        public void Execute(IAdapter adapter)
        {
            if (adapter.State == State.READY)
                throw new InvalidStateException("Start-Command kann nicht ausgeführt werden, da sich der Roboter bereits im READY-Zustand befindet");            
            adapter.State = State.STARTING;
            new System.Threading.Thread(adapter.Start).Start();
        }
    }
}
