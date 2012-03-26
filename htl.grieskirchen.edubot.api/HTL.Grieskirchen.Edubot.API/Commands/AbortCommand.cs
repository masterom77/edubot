using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class AbortCommand : ICommand

    {
        public void Execute(Adapters.IAdapter adapter)
        {
            adapter.CmdQueue.Clear();
        }
    }
}
