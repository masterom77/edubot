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
            if (adapter.State != State.SHUTDOWN)
                throw new InvalidStateException("Start-Command kann nicht ausgeführt werden, da sich der Roboter bereits eingeschaltet wurde");
            adapter.State = State.HOMING;
            if (adapter.OnHoming != null)
                adapter.OnHoming(adapter, new HomingEventArgs(Configuration.AnglePerStep));
            new System.Threading.Thread(adapter.Start).Start();
        }
    }
}
