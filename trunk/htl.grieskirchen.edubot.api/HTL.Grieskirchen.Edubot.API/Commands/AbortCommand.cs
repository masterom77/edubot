﻿using System;
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
            if (adapter.OnAbort != null)
                adapter.OnAbort(adapter, null);
            new System.Threading.Thread(adapter.Abort).Start();
        }
    }
}