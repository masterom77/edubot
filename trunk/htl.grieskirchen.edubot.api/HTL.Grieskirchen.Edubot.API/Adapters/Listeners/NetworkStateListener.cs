using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using HTL.Grieskirchen.Edubot.API.Exceptions;

namespace HTL.Grieskirchen.Edubot.API.Adapters.Listeners
{
    class NetworkStateListener : IStateListener
    {
        Thread stateListener;
        Socket socket;

        public NetworkStateListener(IAdapter adapter, Socket socket) {
            this.adapter = adapter;
            this.socket = socket;
        }

        private void ListenOnState()
        {
            string message;
            List<byte> data = new List<byte>();
            byte[] buffer = new byte[512];
            while (socket.Connected)
            {
                while (socket.Available > 0)
                {
                    socket.Receive(buffer);
                    data.AddRange(buffer);
                }
                message = Encoding.UTF8.GetString(data.ToArray());
                switch (message)
                {
                    case "READY": adapter.State = State.READY;
                        break;
                }
                data.Clear();
                message = "";
            }
        }

        public override void UpdateState(State state)
        {
            throw new IllegalStateUpdateException(this, "Manuelle Updates des Zustandes sind bei einem NetworkStateListener nicht erlaubt");
        }

        public override void Start()
        {
            stateListener = new Thread(ListenOnState);
            stateListener.Start();
        }

        public override void Stop()
        {
            stateListener.Abort();
            stateListener = null;
        }
    }
}
