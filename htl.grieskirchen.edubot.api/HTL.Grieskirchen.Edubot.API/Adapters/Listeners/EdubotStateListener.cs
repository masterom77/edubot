using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using HTL.Grieskirchen.Edubot.API.Exceptions;

namespace HTL.Grieskirchen.Edubot.API.Adapters.Listeners
{
    /// <summary>
    /// Listens for incoming information from the GHI controller
    /// </summary>
    public class EdubotStateListener : INetworkStateListener
    {
        
        EdubotAdapter adapter;

        /// <summary>
        /// Creates a new instance of the EdubotStateListener using the given values.
        /// </summary>
        /// <param name="adapter">The adapter, which should be notified on status updates</param>
        /// <param name="socket">The socket, which is already connected to the controller</param>
        public EdubotStateListener(EdubotAdapter adapter, Socket socket) : base(socket) {
            this.adapter = adapter;
        }

        /// <summary>
        /// Processes the data, received from the network, appropriate for the EdubotAdapter.
        /// </summary>
        /// <param name="receivedData">The data, which was received</param>
        public override void ProcessData(byte[] receivedData)
        {
            try
            {
                string message = Encoding.UTF8.GetString(receivedData.ToArray());
                switch (message)
                {
                    case "ready":
                        adapter.SetState(State.READY, true);
                        break;
                    case "shutdown":
                        adapter.SetState(State.SHUTDOWN, true);
                        Socket.Disconnect(false);
                        break;
                    case "err":
                        adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new ControllerException(message.Substring(4))));
                        break;
                }
            }
            catch (Exception e) {
                adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(e));
            }
        }
    }
}
