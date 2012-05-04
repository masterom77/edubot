using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using HTL.Grieskirchen.Edubot.API.Exceptions;

namespace HTL.Grieskirchen.Edubot.API.Adapters.Listeners
{
    class KebaStateListener : INetworkStateListener
    {
        Thread stateListener;
        Socket socket;
        KebaAdapter adapter;

        public KebaStateListener(KebaAdapter adapter, Socket socket)
            : base(socket)
        {
            this.adapter = adapter;
        }

        public override void ProcessData(byte[] receivedData)
        {
            string message = Encoding.UTF8.GetString(receivedData.ToArray());
            switch (message)
            {
                case "ready":
                    adapter.SetState(State.READY, true);
                    break;
                case "shutdown":
                    adapter.SetState(State.SHUTDOWN, true);
                    socket.Disconnect(false);
                    break;
                case "err":
                    adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new ControllerException(message.Substring(4))));
                    break;
            }
        }
    }
}
