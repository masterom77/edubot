using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class DefaultAdapter : IAdapter
    {
        Socket socket;
        IPEndPoint endpoint;
        InterpolationResult result;

        public DefaultAdapter(ITool tool, float length, bool requiresPrecalculation, IPAddress ipAdress, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endpoint = new IPEndPoint(ipAdress, port);
            this.tool = tool;
            tool.X = (int)length * 2;
            tool.Y = 0;
            this.length = length;
            this.requiresPrecalculation = requiresPrecalculation;
        }

        public override void MoveTo(int x, int y, int z)
        {
            socket.Connect(endpoint);
            socket.Send(Encoding.UTF8.GetBytes(result.ToString()));
            socket.Disconnect(true);
        }

        public override void UseTool(bool activate)
        {
            
            byte[] buffer = new byte[1];
            buffer[0] = Convert.ToByte(activate);
            socket.Connect(endpoint);
            socket.Send(buffer);
            socket.Disconnect(true);

        }


        public override void SetInterpolationResult(Interpolation.InterpolationResult result)
        {
            this.result = result;
        }



        public override void Start()
        {
        }

        public override void Shutdown()
        {
        }
    }
}
