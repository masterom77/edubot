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
    public class KebaAdapter : IAdapter
    {
        Socket senderSocket;
        Socket receiverSocket;
        IPEndPoint senderEndpoint;
        IPEndPoint receiverEndpoint;
        KebaStateListener listener;
        

        public KebaAdapter(Tool tool, float length, float length2, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle, IPAddress ipAdress, int senderPort, int receiverPort)
            : base(tool, length, length2, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            SetNetworkConfiguration(ipAdress,receiverPort,senderPort);
            //Connect();
        }

        public KebaAdapter(Tool tool,float length, float length2, float verticalToolRange, int transmission, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle, IPAddress ipAdress, int senderPort, int receiverPort)
            : base(tool, length, length2, verticalToolRange, transmission, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
            
            SetNetworkConfiguration(ipAdress,receiverPort,senderPort);
            
            //Connect();
        }


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

        public void Connect()
        {
            senderSocket.Connect(receiverEndpoint);
            receiverSocket.Connect(senderEndpoint);
            listener = new KebaStateListener(this, receiverSocket);
            listener.Start();
        }

        public void Disconnect()
        {
                listener.Stop();
                senderSocket.Disconnect(true);
                receiverSocket.Disconnect(true);
        }

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

        public override void MoveCircularTo(object param)
        {
            try
            {
                object[] parameters = (object[])param;
                Point3D target = (Point3D)parameters[0];
                Point3D center = (Point3D)parameters[1];
                senderSocket.Send(Encoding.UTF8.GetBytes("mvc:" + target.ToString() + "&" + center.ToString()));
                toolCenterPoint = target;
            }
            catch (Exception e)
            {
                RaiseFailureEvent(new FailureEventArgs(e));
            }
        }

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

        public override bool IsStateUpdateAllowed()
        {
            return false;
        }

        public override bool UsesIntegratedPathCalculation()
        {
            return true;
        }
    }
}
