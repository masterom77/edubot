using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class KebaAdapter : IAdapter
    {
        Socket senderSocket;
        Socket receiverSocket;
        IPEndPoint senderEndpoint;
        IPEndPoint receiverEndpoint;
        Thread stateListener;

        public KebaAdapter(Tool tool, float length, float length2, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle, IPAddress ipAdress, int senderPort, int receiverPort)
            : base(tool, length, length2, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            state = State.SHUTDOWN;
            requiresPrecalculation = false;
            //Connect();
        }

        public KebaAdapter(Tool tool,float length, float length2, float verticalToolRange, int transmission, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle, IPAddress ipAdress, int senderPort, int receiverPort)
            : base(tool, length, length2, verticalToolRange, transmission, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            
            state = State.SHUTDOWN;
            requiresPrecalculation = false;
            //Connect();
        }


        public void SetNetworkConfiguration(IPAddress ipAddress, int port)
        {
            if (receiverSocket.Connected)
            {
                Listener.Stop();
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
            Listener = new NetworkStateListener(this, receiverSocket);
        }

        public void Connect()
        {
            senderSocket.Connect(senderEndpoint);
            receiverSocket.Connect(receiverEndpoint);
            state = State.SHUTDOWN;
            Listener = new NetworkStateListener(this, receiverSocket);
        }

        public override void MoveStraightTo(object param)
        {
            Point3D target = (Point3D)param;
            senderSocket.Connect(senderEndpoint);
            senderSocket.Send(Encoding.UTF8.GetBytes("mvs:" + target.ToString()));
            toolCenterPoint = target;
        }

        public override void MoveCircularTo(object param)
        {

            object[] parameters = (object[])param;
            Point3D target = (Point3D)parameters[0];
            Point3D center = (Point3D)parameters[1];
            senderSocket.Connect(senderEndpoint);
            senderSocket.Send(Encoding.UTF8.GetBytes("mvc:" + target.ToString() + "&" + center.ToString()));
            toolCenterPoint = target;
        }

        public override void UseTool(object param)
        {
            byte[] buffer = new byte[1];
            buffer[0] = Convert.ToByte(param);
            senderSocket.Connect(senderEndpoint);
            senderSocket.Send(buffer);
            senderSocket.Disconnect(true);
        }

        public override void Start(object param)
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



        public override void Abort()
        {
            senderSocket.Send(Encoding.UTF8.GetBytes("abort"));
        }

    }
}
