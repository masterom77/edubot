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
        Socket socket;
        IPEndPoint endpoint;
        Thread stateListener;

        public KebaAdapter(ITool tool, float length, IPAddress ipAdress, int port)
            : base()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endpoint = new IPEndPoint(ipAdress, port);
            this.tool = tool;
            tool.X = (int)length * 2;
            tool.Y = 0;
            this.length = length;
            requiresPrecalculation = false;
            type = AdapterType.KEBA;
            listener = new NetworkStateListener(this, socket);
        }

        public override void MoveTo(object param)
        {
            Point3D target = (Point3D)param;
            socket.Connect(endpoint);
            socket.Send(Encoding.UTF8.GetBytes(target.ToString()));
            socket.Disconnect(true);
            tool.X = target.X;
            tool.Y = target.Y;
            tool.Z = target.Z;
        }

        public override void UseTool(object param)
        {
            byte[] buffer = new byte[1];
            buffer[0] = Convert.ToByte(param);
            socket.Connect(endpoint);
            socket.Send(buffer);
            socket.Disconnect(true);
        }

        public override void SetInterpolationResult(Interpolation.InterpolationResult result)
        {
           
        }

        public override void Start()
        {
            //socket.Connect(endpoint);
            socket.Connect(endpoint);
            stateListener = new Thread(ListenOnState);
            stateListener.Start();
            socket.Send(Encoding.UTF8.GetBytes("start"));
            //socket.Disconnect(true);
        }

        public override void Shutdown()
        {
            //socket.Connect(endpoint);
            socket.Send(Encoding.UTF8.GetBytes("shutdown"));
            socket.Disconnect(true);
            //socket.Disconnect(true);
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
                    case "READY": State = State.READY;
                        break;
                }
                data.Clear();
                message = "";
            }
        }
    }
}
