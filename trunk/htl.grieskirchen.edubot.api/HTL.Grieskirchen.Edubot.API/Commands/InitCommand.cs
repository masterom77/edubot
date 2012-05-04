using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class InitCommand : ICommand
    {
        public InitCommand()
        {
        }

        public void Execute(IAdapter adapter)
        {
            adapter.SetState(State.HOMING, true);
            adapter.CmdQueue.Clear();
            adapter.RaiseHomingEvent(new HomingEventArgs(Configuration.AnglePerStep));
            new System.Threading.Thread(adapter.Initialize).Start();
        }

        public FailureEventArgs CanExecute(IAdapter adapter)
        {
            return null;            
        }
    }
}
