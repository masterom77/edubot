using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class KebaAdapter : IAdapter
    {
        Socket senderSocket;
        Socket receiverSocket;
        IPEndPoint senderEndpoint;
        IPEndPoint receiverEndpoint;
        Thread stateListener;

        public KebaAdapter()
            : base()
        {
            type = AdapterType.KEBA;
            state = State.SHUTDOWN;
            requiresPrecalculation = true;
            //Connect();
        }

        public KebaAdapter(ITool tool, float length, float length2, IPAddress ipAdress, int port)
            : base(tool, length, length2)
        {
            type = AdapterType.KEBA;
            state = State.DISCONNECTED;

            senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            senderEndpoint = new IPEndPoint(ipAdress, port);

            receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            receiverEndpoint = new IPEndPoint(ipAdress, port + 1);

            requiresPrecalculation = false;
            Connect();
        }

        public void SetNetworkConfiguration(IPAddress ipAddress, int port)
        {
            if (receiverSocket.Connected)
            {
                listener.Stop();
                receiverSocket.Disconnect(false);
            }
            if (senderSocket.Connected)
            {
                senderSocket.Disconnect(false);
            }

            senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            senderEndpoint = new IPEndPoint(ipAddress, port);
            receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            receiverEndpoint = new IPEndPoint(ipAddress, port+1);
            listener = new NetworkStateListener(this, receiverSocket);
        }

        public void Connect()
        {
            senderSocket.Connect(senderEndpoint);
            receiverSocket.Connect(receiverEndpoint);
            state = State.SHUTDOWN;
            listener = new NetworkStateListener(this, receiverSocket);
        }

        public override void MoveStraightTo(object param)
        {
            Point3D target = (Point3D)param;
            senderSocket.Connect(senderEndpoint);
            senderSocket.Send(Encoding.UTF8.GetBytes("mvs:" + target.ToString()));
            tool.ToolCenterPoint = target;
        }

        public override void MoveCircularTo(object param)
        {

            object[] parameters = (object[])param;
            Point3D target = (Point3D)parameters[0];
            Point3D center = (Point3D)parameters[1];
            senderSocket.Connect(senderEndpoint);
            senderSocket.Send(Encoding.UTF8.GetBytes("mvc:" + target.ToString() + "&" + center.ToString()));
            tool.ToolCenterPoint = target;
        }

        public override void UseTool(object param)
        {
            byte[] buffer = new byte[1];
            buffer[0] = Convert.ToByte(param);
            senderSocket.Connect(senderEndpoint);
            senderSocket.Send(buffer);
            senderSocket.Disconnect(true);
        }

        public override void SetInterpolationResult(Interpolation.InterpolationResult result)
        {
           
        }

        public override void Start()
        {
            //socket.Connect(endpoint);
            senderSocket.Connect(senderEndpoint);
            stateListener = new Thread(ListenOnState);
            stateListener.Start();
            senderSocket.Send(Encoding.UTF8.GetBytes("start"));
            //socket.Disconnect(true);
        }

        public override void Shutdown()
        {
            //socket.Connect(endpoint);
            senderSocket.Send(Encoding.UTF8.GetBytes("shutdown"));
            senderSocket.Disconnect(true);
            //socket.Disconnect(true);
        }

        private void ListenOnState()
        {
            string message;
            List<byte> data = new List<byte>();
            byte[] buffer = new byte[512];
            while (senderSocket.Connected)
            {
                while (senderSocket.Available > 0)
                {
                    senderSocket.Receive(buffer);
                    data.AddRange(buffer);
                }
                message = Encoding.UTF8.GetString(data.ToArray());
                switch (message)
                {
                    case "READY": State = State.READY;
                        break;
                }
                data.Clear();
                message = "";
            }
        }

        
    }
}
