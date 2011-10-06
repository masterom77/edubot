using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.Commands
{
    interface ICommand
    {
        /// <summary>
        /// Executes the command using the given arguments
        /// </summary>
        /// <param name="args">Arguments to be used for execution</param>
        void Execute();
        void SetArguments(string[] args);

    }
}
