using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.Commands
{
    class ShutdownCommand : ICommand
    {
        public void Execute()
        {
            API.Edubot.GetInstance().Shutdown();
        }

        public void SetArguments(string[] args)
        {
            
        }
    }
}
