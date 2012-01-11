using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Adapters.Listeners
{
    public abstract class IStateListener
    {
        protected IAdapter adapter;

        public IAdapter Adapter
        {
            get { return adapter; }
            set { adapter = value; }
        }

        public abstract void Start();
        public abstract void Stop();
        public abstract void UpdateState(State state);
    }
}
