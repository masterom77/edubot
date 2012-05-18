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
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    /// <summary>
    /// Used to control a robot using the SPS of the KEBA company
    /// </summary>
    public class KebaAdapter : IAdapter
    {
        Socket senderSocket;
        Socket receiverSocket;
        IPEndPoint senderEndpoint;
        IPEndPoint receiverEndpoint;
        KebaStateListener listener;

        /// <summary>
        /// Initializes a new instance of the IAdapter for robots with RR-Kinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="maxPrimaryAngle">The maximum angle, which the primary axis can adopt</param>
        /// <param name="minPrimaryAngle">The minimum angle, which the primary axis can adopt</param>
        /// <param name="maxSecondaryAngle">The maximum angle, which the secondary axis can adopt</param>
        /// <param name="minSecondaryAngle">The minimum angle, which the secondary axis can adopt</param>
        /// <param name="ipAdress">The IP address of the SPS</param>
        /// <param name="receiverPort">The port where the SPS software listens for data</param>
        /// <param name="senderPort">The port where the SPS sends data</param>
        public KebaAdapter(Tool equippedTool, float length, float length2, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle, IPAddress ipAdress, int senderPort, int receiverPort)
            : base(equippedTool, length, length2, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            SetNetworkConfiguration(ipAdress,receiverPort,senderPort);
            //Connect();
        }

        /// <summary>
        /// Initializes a new instance of the IAdapter for robots with RRT-Kinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="verticalToolRange">The distance between the toolcenter point and the working area, measured when the tool has adopted the highest position</param>
        /// <param name="transmission">The transmission, which defines how many degrees the third engine has to rotate, to move the tertiary axis down one unit</param>
        /// <param name="maxPrimaryAngle">The maximum angle, which the primary axis can adopt</param>
        /// <param name="minPrimaryAngle">The minimum angle, which the primary axis can adopt</param>
        /// <param name="maxSecondaryAngle">The maximum angle, which the secondary axis can adopt</param>
        /// <param name="minSecondaryAngle">The minimum angle, which the secondary axis can adopt</param>
        /// <param name="ipAdress">The IP address of the SPS</param>
        /// <param name="receiverPort">The port where the SPS software listens for data</param>
        /// <param name="senderPort">The port where the SPS sends data</param>
        public KebaAdapter(Tool equippedTool, float length, float length2, float verticalToolRange, int transmission, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle, IPAddress ipAdress, int senderPort, int receiverPort)
            : base(equippedTool, length, length2, verticalToolRange, transmission, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            
            SetNetworkConfiguration(ipAdress,receiverPort,senderPort);
            
            //Connect();
        }

        /// <summary>
        /// Sets the network configuration of this adapter. If there is a connection existing, it will be disconnected.
        /// </summary>
        /// <param name="ipAddress">The IP address of the SPS</param>
        /// <param name="receiverPort">The port where the SPS software listens for data</param>
        /// <param name="senderPort">The port where the SPS sends data</param>
        public void SetNetworkConfiguration(IPAddress ipAddress, int receiverPort, int senderPort)
        {
            if (receiverSocket != null)
            {
                if (receiverSocket.Connected)
                {
                    listener.Stop();
                    receiverSocket.Disconnect(false);
                }
            }
            if (senderSocket != null)
            {
                if (senderSocket.Connected)
                {
                    senderSocket.Disconnect(false);
                }
            }

            senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            senderEndpoint = new IPEndPoint(ipAddress, senderPort);
            receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            receiverEndpoint = new IPEndPoint(ipAddress, receiverPort);
            listener = new KebaStateListener(this, receiverSocket);
        }

        /// <summary>
        /// Trys to connect to the controller, using the given network configuration.
        /// </summary>
        /// <returns>Returns true if a connection is possible, else returns false.</returns>
        public bool TestConnectivity()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
                socket.SendTimeout = 5000;
                socket.ReceiveTimeout = 5000;
                socket.Connect(receiverEndpoint);
                socket.Close(-1);
                socket.Connect(senderEndpoint);
                socket.Close(-1);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Trys to connect to the controller and starts the internal network listener.
        /// </summary>
        public void Connect()
        {
            senderSocket.Connect(receiverEndpoint);
            receiverSocket.Connect(senderEndpoint);
            listener = new KebaStateListener(this, receiverSocket);
            listener.Start();
        }

        /// <summary>
        /// Closes the connection to the controller and stops the running EdubotStateListener.
        /// </summary>
        public void Disconnect()
        {
                listener.Stop();
                senderSocket.Disconnect(true);
                receiverSocket.Disconnect(true);
        }

        /// <summary>
        /// Starts a linear movement, by sending a "mvs:" and an array of steps to acquire this type of movement to the controller
        /// </summary>
        /// <param name="param">An object, which contains the target point as Point3D object</param>
        public override void MoveStraightTo(object param)
        {
            try{
            Point3D target = (Point3D)param;
            senderSocket.Send(Encoding.UTF8.GetBytes("mvs:" + target.ToString()));
            toolCenterPoint = target;}
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
                senderSocket.Send(Encoding.UTF8.GetBytes("mvc:" + target.ToString() + "&" + center.ToString()));
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
                senderSocket.Send(Encoding.UTF8.GetBytes("ust:" + param.ToString()));
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
            //socket.Disconnect(true);

        }

        /// <summary>
        /// Initializes the robot, so the position of the tool is at the home point by sending "hom:" and the IP address of this computer to the controller.
        /// </summary>
        /// <param name="param">An object, which contains the correction angle as a float value</param>
        public override void Initialize(object param)
        {
            try
            {
                Connect();
                senderSocket.Send(Encoding.UTF8.GetBytes("hom:" + senderSocket.LocalEndPoint.ToString()));
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }

        // <summary>
        /// Shuts the robot down by sending "sht" to the controller
        /// </summary>
        public override void Shutdown()
        {
            try
            {
                senderSocket.Send(Encoding.UTF8.GetBytes("sht"));
                senderSocket.Disconnect(true);
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }

        /// <summary>
        /// Aborts the robot's current action, by sending "abo" to the controller
        /// </summary>
        public override void Abort()
        {
            try
            {
                senderSocket.Send(Encoding.UTF8.GetBytes("abo"));
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
                senderSocket.Send(content);
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }
    }
}
