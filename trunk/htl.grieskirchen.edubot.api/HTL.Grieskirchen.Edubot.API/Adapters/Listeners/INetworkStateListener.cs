using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace HTL.Grieskirchen.Edubot.API.Adapters.Listeners
{
    public abstract class INetworkStateListener
    {

        Thread listener;

        Socket socket;

        public Socket Socket
        {
            get { return socket; }
            set { socket = value; }
        }

        public INetworkStateListener(Socket socket) {
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
                        ProcessData(data.ToArray());                        
                        data.Clear();
                    }
                }
            }
            catch (ThreadAbortException tae) { }
        }       

        public void Start()
        {
            listener = new Thread(ListenOnState);
            listener.Start();
        }

        public void Stop()
        {
            listener.Abort();
            listener = null;
        }

        public abstract void ProcessData(byte[] receivedData);
    }
}
