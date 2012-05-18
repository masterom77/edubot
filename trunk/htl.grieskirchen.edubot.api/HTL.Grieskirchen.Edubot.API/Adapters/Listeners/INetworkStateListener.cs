using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace HTL.Grieskirchen.Edubot.API.Adapters.Listeners
{
    /// <summary>
    /// An abstract class, used to receive data from a specified socket and process it in a user-specified way.
    /// </summary>
    public abstract class INetworkStateListener
    {

        private Thread listener;
        private Socket socket;

        /// <summary>
        /// Gets or sets the socket, which is used to receive information
        /// </summary>
        public Socket Socket
        {
            get { return socket; }
            set { socket = value; }
        }

        /// <summary>
        /// Initializes a new instance of the INetworkStateListener using the given values
        /// </summary>
        /// <param name="socket">An already connected socket, which is used to receive data</param>
        protected INetworkStateListener(Socket socket) {
            this.socket = socket;
        }

        /// <summary>
        /// Listens on incoming data and passes it to the ProcessData method.
        /// </summary>
        private void ListenOnState()
        {
            try
            {
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
            catch (ThreadAbortException) { }
        }       

        /// <summary>
        /// Starts to listen for incoming data
        /// </summary>
        public void Start()
        {
            listener = new Thread(ListenOnState);
            listener.Start();
        }

        /// <summary>
        /// Stops listening on incoming data
        /// </summary>
        public void Stop()
        {
            listener.Abort();
            //listener = null;
        }

        /// <summary>
        /// Processes the received data in a user specified way. This method is called within the ListenOnState method
        /// </summary>
        /// <param name="receivedData">The data, which has been received</param>
        public abstract void ProcessData(byte[] receivedData);
    }
}
