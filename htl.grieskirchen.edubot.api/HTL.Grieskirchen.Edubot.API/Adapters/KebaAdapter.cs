using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class KebaAdapter : IAdapter
    {
        Socket socket;
        IPEndPoint endpoint;

        public KebaAdapter(float length, bool requiresPrecalculation, IPAddress ipAdress, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endpoint = new IPEndPoint(ipAdress, port);
            this.length = length;
            this.requiresPrecalculation = requiresPrecalculation;
        }

        public override void MoveTo(int x, int y, int z)
        {
            socket.Connect(endpoint);
            socket.Send(new byte[]{(byte)x,(byte)y,(byte)z});
            socket.Disconnect(true);
        }

        public override void UseTool(bool activate)
        {
            socket.Connect(endpoint);
            socket.Send(new byte[] {Convert.ToByte(activate)});
            socket.Disconnect(true);
        }

        public override void SetInterpolationResult(Interpolation.InterpolationResult result)
        {
           
        }
    }
}
