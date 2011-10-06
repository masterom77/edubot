using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.Exceptions;

namespace HTL.Grieskirchen.Edubot.Commands
{
    class ToolCommand : ICommand
    {
        bool activate;

        public void Execute()
        {
            Console.WriteLine(activate ? "Activating Tool" : "Deactivating Tool");
        }

        public void SetArguments(string[] args)
        {
            if(!bool.TryParse(args[0],out activate)){
                throw new InvalidParameterException("Ungültiger Parameter: activate. Cannot cast \""+args[0]+"\" into bool");
            }
        }
    }
}
