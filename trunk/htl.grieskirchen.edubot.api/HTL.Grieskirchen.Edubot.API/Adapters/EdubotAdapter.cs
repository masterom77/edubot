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
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    /// <summary>
    /// Used for communication to the Edubot
    /// </summary>
    public class EdubotAdapter : IAdapter
    {
        Socket socket;
        IPEndPoint endpoint;
        EdubotStateListener listener;

        /// <summary>
        /// Creates a new instance of the EdubotAdapter using the values of the original Edubot model.
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="ipAddress">The IP address of the GHI controller</param>
        /// <param name="port">The port where controller-software runs</param>
        public EdubotAdapter(Tool equippedTool, IPAddress ipAddress, int port)
            : base(equippedTool, 200, 200, 80, 10, 145, -145, 135, -135)
        {
            SetNetworkConfiguration(ipAddress, port);
        }

        /// <summary>
        /// Constructor for RR-Kinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="verticalToolRange">The distance between the toolcenter point and the working area, measured when the tool has adopted the highest position</param>
        /// <param name="transmission">The transmission, which defines how many degrees the third engine has to rotate, to move the tertiary axis down one unit</param>
        /// <param name="ipAddress">The IP address of the GHI controller</param>
        /// <param name="port">The port where controller-software runs</param>
        /// <param name="maxPrimaryAngle">The maximum angle, which the primary axis can adopt</param>
        /// <param name="minPrimaryAngle">The minimum angle, which the primary axis can adopt</param>
        /// <param name="maxSecondaryAngle">The maximum angle, which the secondary axis can adopt</param>
        /// <param name="minSecondaryAngle">The minimum angle, which the secondary axis can adopt</param>
        public EdubotAdapter(Tool equippedTool, float length,float length2, float verticalToolRange, int transmission, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle, IPAddress ipAddress, int port)
            : base(equippedTool, length, length2, verticalToolRange, transmission, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            SetNetworkConfiguration(ipAddress, port);
            //Connect();
        }

        /// <summary>
        /// Constructor for RR-Kinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="ipAddress">The IP address of the GHI controller</param>
        /// <param name="port">The port where controller-software runs</param>
        /// <param name="maxPrimaryAngle">The maximum angle, which the primary axis can adopt</param>
        /// <param name="minPrimaryAngle">The minimum angle, which the primary axis can adopt</param>
        /// <param name="maxSecondaryAngle">The maximum angle, which the secondary axis can adopt</param>
        /// <param name="minSecondaryAngle">The minimum angle, which the secondary axis can adopt</param>
        public EdubotAdapter(Tool equippedTool, float length, float length2, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle, IPAddress ipAddress, int port)
            : base(equippedTool, length, length2,maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            SetNetworkConfiguration(ipAddress, port);
            //Connect();
        }

        /// <summary>
        /// Sets the network configuration of this adapter. If there is a connection existing, it will be disconnected.
        /// </summary>
        /// <param name="ipAddress">The IP address of the GHI controller</param>
        /// <param name="port">The port where controller-software runs</param>
        public void SetNetworkConfiguration(IPAddress ipAddress, int port)
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    listener.Stop();
                    socket.Disconnect(false);
                }
            }
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endpoint = new IPEndPoint(ipAddress, port);
            listener = new EdubotStateListener(this, socket);
        }

        /// <summary>
        /// Trys to connect to the controller and starts the internal network listener.
        /// </summary>
        public void Connect() {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
                socket.Connect(endpoint);
                listener = new EdubotStateListener(this, socket);
                listener.Start();
        }

        /// <summary>
        /// Trys to connect to the controller, using the given network configuration.
        /// </summary>
        /// <returns>Returns true if a connection is possible, else returns false.</returns>
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
        /// Closes the connection to the controller and stops the running EdubotStateListener.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                listener.Stop();
                socket.Disconnect(true);
            }
            catch (Exception e) {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }

        /// <summary>
        /// Starts a linear movement, by sending a "mvs:" and an array of steps to acquire this type of movement to the controller
        /// </summary>
        /// <param name="param">An object, which contains the target point as Point3D object</param>
        public override void MoveStraightTo(object param)
        {
            //socket.SendBufferSize = Int32.MaxValue;
            try{
            Point3D target = (Point3D)param;
            byte[] content = Encoding.UTF8.GetBytes("mvs:" + InterpolationResult.ConverToStepString());
            socket.Send(content);

            toolCenterPoint = target;
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }

        /// <summary>
        /// Starts a circular movement, by sending a "mvc:" and an array of steps to acquire this type of movement to the controller
        /// </summary>
        /// <param name="param">An object, which contains the target and center point as Point3D array</param>
        public override void MoveCircularTo(object param)
        {
            try
            {
                Point3D[] parameters = (Point3D[])param;
                Point3D target = parameters[0];
                Point3D center = parameters[1];
                byte[] content = Encoding.UTF8.GetBytes("mvc:" + InterpolationResult.ToString());
                socket.Send(content);
                toolCenterPoint = target;
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }

        /// <summary>
        /// Activates or deactivates the tool, by sending "ust:" and a boolean value to the controller
        /// </summary>
        /// <param name="param">An object, which contains a boolean value indicating whether tool should be activated or deactivated</param>
        public override void UseTool(object param)
        {
            try
            {
                socket.Send(Encoding.UTF8.GetBytes("ust:" + ((bool)param).ToString()));
            }
            catch (Exception e) {
                RaiseFailureEvent(new FailureEventArgs(e));
            }

        }

        /// <summary>
        /// Initializes the robot, so the position of the tool is at the home point by sending "hom:" and the IP address of this computer to the controller.
        /// </summary>
        /// <param name="param">An object, which contains the correction angle as a float value</param>
        public override void Initialize(object param)
        {
            try
            {
                if (socket == null || !socket.Connected)
                {
                    Connect();
                }
                socket.Send(Encoding.UTF8.GetBytes("hom:" + socket.LocalEndPoint.ToString()));
                toolCenterPoint = HomePoint;
            }
            catch (Exception e) {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }

        /// <summary>
        /// Shuts the robot down by sending "sht" to the controller
        /// </summary>
        public override void Shutdown()
        {
            try
            {
                socket.Send(Encoding.UTF8.GetBytes("sht"));
            }
            catch (Exception e) {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }

        /// <summary>
        /// Aborts the robot's current action, by sending "abo" to the controller
        /// </summary>
        public override void Abort()
        {
            try{
            socket.Send(Encoding.UTF8.GetBytes("abo"));
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }


        /// <summary>
        /// Determines wether manual status updates are allowed or not
        /// </summary>
        /// <returns>A boolean value, which indicates wether manual status updates are allowed or not</returns>
        public override bool IsStateUpdateAllowed()
        {
            return false;
        }

        /// <summary>
        /// Determines wether the kinematics and interpolation methods of the API should be used
        /// </summary>
        /// <returns>A boolean value, which indicates wether the kinematics and interpolation methods of the API should be used</returns>
        public override bool UsesIntegratedPathCalculation()
        {   
            return true;
        }

        /// <summary>
        /// Changes the current axis configuration by sending "mvs:" and the required steps to the controller.
        /// </summary>
        /// <param name="param">An object, which contains the new configuration as AxisConfiguration object</param>
        public override void ChangeConfiguration(object param)
        {
            try
            {
                byte[] content = Encoding.UTF8.GetBytes("mvs:" + InterpolationResult.ConverToStepString());
                socket.Send(content);
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }
    }
}
