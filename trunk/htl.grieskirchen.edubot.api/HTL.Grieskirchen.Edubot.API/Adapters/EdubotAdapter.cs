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
    public class EdubotAdapter : IAdapter
    {
        Socket socket;
        IPEndPoint endpoint;
        InterpolationResult result;

        /// <summary>
        /// Initializes a new instance of the DefaultAdapter class.
        /// </summary>
        /// <param name="tool">An instance of the tool, that is currently installed on the robot</param>
        /// <param name="length">The length of the first Axis in millimeters</param>
        /// <param name="length2">The length of the second Axis in millimeters</param>
        /// <param name="ipAdress">The IP address of controller</param>
        /// <param name="port">The port, on which the program of the controller is listening</param>
        public EdubotAdapter(ITool tool, float length,float length2, IPAddress ipAdress, int port)
            : base(tool, length, length2)
        {
            type = AdapterType.DEFAULT;
            requiresPrecalculation = true;
            SetNetworkConfiguration(ipAdress, port);
            //Connect();
        }

        /// <summary>
        /// Sets the network configuration of this adapter. If there is a connection existing, it will be disconnected.
        /// </summary>
        /// <param name="ipAddress">The IP-address of the controller</param>
        /// <param name="port">The port where controller-software runs</param>
        public void SetNetworkConfiguration(IPAddress ipAddress, int port)
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    Listener.Stop();
                    socket.Disconnect(false);
                }
            }
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endpoint = new IPEndPoint(ipAddress, port);
            Listener = new NetworkStateListener(this, socket);
        }

        /// <summary>
        /// Trys to connect to the controller and starts the internal network listener.
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        public void Connect() {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endpoint);
            Listener = new NetworkStateListener(this, socket);            
            Listener.Start();
        }

        /// <summary>
        /// Trys to connect to the controller, using the given network configuration.
        /// </summary>
        /// <returns>Return true if a connection is possible, else returns false.</returns>
        public bool TestConnectivity() {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
                socket.SendTimeout = 5000;
                socket.ReceiveTimeout = 5000;
                socket.Connect(endpoint);
                socket.Close(-1);
                return true;
            }
            catch(Exception e) {
                return false;
            }
        }

        /// <summary>
        /// Closes the connection to the controller and stops the running network-listener.
        /// </summary>
        public void Disconnect()
        {
            Listener.Stop();
            socket.Disconnect(true);
        }

        /// <summary>
        /// Sends a "Move Straight"-Command to the controller, which will be executed by the robot.
        /// </summary>
        /// <param name="param">An object containing the target point of the movement</param>
        public override void MoveStraightTo(object param)
        {
            //socket.SendBufferSize = Int32.MaxValue;
            Point3D target = (Point3D)param;
            byte[] content = Encoding.UTF8.GetBytes("mvs:" + result.ToString());
            socket.Send(content);

            tool.ToolCenterPoint = target;
        }

        /// <summary>
        /// Sends a "Move Circular"-Command to the controller, which will be executed by the robot
        /// </summary>
        /// <param name="param"></param>
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
            socket.Send(Encoding.UTF8.GetBytes("ust:" + param.ToString()));
            //socket.Disconnect(true);

        }

        public override void Start(object param)
        {
            //if (!socket.Connected)
            //    Connect();
            Connect();
                socket.Send(Encoding.UTF8.GetBytes("hom:"+socket.LocalEndPoint.ToString()+";"+MathHelper.ConvertToTicks(Configuration.AnglePerStep)));
        }

        public override void Shutdown()
        {
            
            socket.Send(Encoding.UTF8.GetBytes("sht"));
            //Disconnect();
        }

        public override void SetInterpolationResult(Interpolation.InterpolationResult result)
        {
            this.result = result;
        }



        public override void Abort()
        {
            socket.Send(Encoding.UTF8.GetBytes("abo"));
        }
    }
}
