using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Adapters.Listeners
{
    class VirtualStateListener : IStateListener
    {


        public override void UpdateState(State state)
        {
            adapter.State = state;
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}
