using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;
using System.Threading;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    /// <summary>
    /// Used for communication to the Edubot
    /// </summary>
    public class DefaultAdapter : IAdapter
    {
        Socket socket;
        IPEndPoint endpoint;
        InterpolationResult result;

        public DefaultAdapter()
            : base()
        {
            type = AdapterType.DEFAULT;
            requiresPrecalculation = true;
            //Connect();
        }

        /// <summary>
        /// Initializes a new instance of the DefaultAdapter class.
        /// </summary>
        /// <param name="tool">An instance of the tool, that is currently installed on the robot</param>
        /// <param name="length">The length of the first Axis in millimeters</param>
        /// <param name="length2">The length of the second Axis in millimeters</param>
        /// <param name="ipAdress">The IP address of controller</param>
        /// <param name="port">The port, on which the program of the controller is listening</param>
        public DefaultAdapter(ITool tool, float length,float length2, IPAddress ipAdress, int port)
            : base(tool, length, length2)
        {
            type = AdapterType.DEFAULT;
            requiresPrecalculation = true;
            SetNetworkConfiguration(ipAdress, port);
            //Connect();
        }

        public void SetNetworkConfiguration(IPAddress ipAddress, int port)
        {
            if (socket.Connected)
            {
                listener.Stop();
                socket.Disconnect(false);
            }
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endpoint = new IPEndPoint(ipAddress, port);
            listener = new NetworkStateListener(this, socket);
        }

        /// <summary>
        /// Trys to connect to the controller and starts the internal network listener.
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        public void Connect() {
            socket.Connect(endpoint);            
            listener.Start();
        }

        public bool TestConnectivity() {
            try
            {
                socket.Connect(endpoint);
                socket.Disconnect(true);
                return true;
            }
            catch(Exception e) {
                return false;
            }
        }

        public void Disconnect()
        {
            //listener.Stop();
            socket.Disconnect(true);
        }

        /// <summary>
        /// Sends the command
        /// </summary>
        /// <param name="param"></param>



        public override void MoveStraightTo(object param)
        {
            //socket.SendBufferSize = Int32.MaxValue;
            Point3D target = (Point3D)param;
            byte[] content = Encoding.UTF8.GetBytes("mvs:" + result.ToString());
            socket.Send(content);

            tool.ToolCenterPoint = target;
        }

        public override void MoveCircularTo(object param)
        {

            object[] parameters = (object[])param;
            Point3D target = (Point3D)parameters[0];
            Point3D center = (Point3D)parameters[1];
            byte[] content = Encoding.UTF8.GetBytes("mvc:" + result.ToString());
            socket.Send(content);
            //socket.Disconnect(true);

            tool.ToolCenterPoint = target;
        }

        public override void UseTool(object param)
        {
            
            //byte[] buffer = new byte[1];
            //buffer[0] = Convert.ToByte(activate);
            //socket.Connect(endpoint);
            socket.Send(Encoding.UTF8.GetBytes("usetool:" + param.ToString()));
            //socket.Disconnect(true);

        }

        public override void Start()
        {
            //if (!socket.Connected)
            //    Connect();
            Connect();
            socket.Send(Encoding.UTF8.GetBytes("start"));
        }

        public override void Shutdown()
        {
            
            socket.Send(Encoding.UTF8.GetBytes("shutdown"));
            //Disconnect();
        }

        public override void SetInterpolationResult(Interpolation.InterpolationResult result)
        {
            this.result = result;
        }


    }
}
