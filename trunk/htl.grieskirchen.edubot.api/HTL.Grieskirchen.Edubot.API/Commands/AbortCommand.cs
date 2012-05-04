using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class AbortCommand : ICommand

    {
        public void Execute(Adapters.IAdapter adapter)
        {
            adapter.CmdQueue.Clear();  
            adapter.RaiseAbortEvent(null);
            new System.Threading.Thread(adapter.Abort).Start();
        }


        public FailureEventArgs CanExecute(Adapters.IAdapter adapter)
        {
            return null;
        }
    }
}
