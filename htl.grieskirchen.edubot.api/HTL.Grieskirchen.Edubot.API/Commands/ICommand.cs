using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public interface ICommand
    {
        void Execute(IAdapter adapter);
    }
}
