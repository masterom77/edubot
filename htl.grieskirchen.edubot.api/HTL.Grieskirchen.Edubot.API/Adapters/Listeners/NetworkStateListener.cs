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
            try
            {
                string message;
                List<byte> data = new List<byte>();
                byte[] buffer = new byte[512];
                while (socket.Connected)
                {
                    while (socket.Available > 0)
                    {
                        int bytesRead = socket.Receive(buffer);
                        for (int i = 0; i < bytesRead; i++)
                        {
                            data.Add(buffer[i]);
                        }
                    }
                    if (data.Count > 0)
                    {
                        message = Encoding.UTF8.GetString(data.ToArray());
                        switch (message)
                        {
                            case "ready":

                                adapter.State = State.READY;

                                break;
                            case "shutdown":
                                adapter.State = State.SHUTDOWN;
                                socket.Disconnect(true);
                                break;
                        }
                        data.Clear();
                        message = "";
                    }
                }
            }
            catch (ThreadAbortException tae) { }
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
            //stateListener.Abort();
            //stateListener = null;
        }
    }
}
