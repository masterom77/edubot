using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Exceptions;

namespace HTL.Grieskirchen.Edubot.API.Commands
{
    public class ChangeInterpolationCommand : ICommand
    {
        IInterpolationType interpolation;

        public IInterpolationType Interpolation
        {
            get { return interpolation; }
            set { interpolation = value; }
        }

        public ChangeInterpolationCommand(IInterpolationType interpolation)
        {
            this.interpolation = interpolation;
        }

        public void Execute(IAdapter adapter)
        {
            if (adapter.State == State.SHUTDOWN)
                throw new InvalidStateException("ChangeInterpolation-Command kann nicht ausgeführt werden, da sich der Roboter im SHUTDOWN-Zustand befindet");
            adapter.Interpolation = interpolation;
            //new System.Threading.Thread(adapter.Shutdown).Start();
           
        }
    }
}
