using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class StartCommand : ICommand
    {
        public StartCommand()
        {
        }


        public void Execute(IAdapter adapter)
        {
            adapter.RaiseHomingEvent(new HomingEventArgs(Configuration.AnglePerStep));
            new System.Threading.Thread(adapter.Start).Start();
        }


        public FailureEventArgs CanExecute(IAdapter adapter)
        {
            if (adapter.State != State.SHUTDOWN)
            {
                return new FailureEventArgs(adapter.State, new InvalidStateException("Start-Command kann nicht ausgeführt werden, da der Roboter sich im "+adapter.State+"-Zustand befindet"));
            }
            return null;
            
        }
    }
}
