using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.Commands
{
    class StartCommand : ICommand
    {
        public void Execute()
        {
            API.Edubot.GetInstance().Start();
        }

        public void SetArguments(string[] args)
        {
            
        }
    }
}
