﻿using System;
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
    public class DefaultAdapter : IAdapter
    {
        Socket socket;
        IPEndPoint endpoint;
        InterpolationResult result;

        public DefaultAdapter(ITool tool, float length, IPAddress ipAdress, int port)
            : base()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endpoint = new IPEndPoint(ipAdress, port);
            this.tool = tool;
            tool.X = (int)length * 2;
            tool.Y = 0;
            this.length = length;
            type = AdapterType.DEFAULT;
            requiresPrecalculation = true;
            listener = new NetworkStateListener(this, socket);
            
        }

        public override void MoveTo(object param)
        {
            //socket.Connect(endpoint);
            Point3D target = (Point3D)param;
            socket.SendBufferSize = Int32.MaxValue;
            byte[] content = Encoding.UTF8.GetBytes(result.ToString());
            socket.Send(content);
            tool.X = target.X;
            tool.Y = target.Y;
            tool.Z = target.Z;
            //socket.Disconnect(true);
        }

        public override void UseTool(object param)
        {
            
            //byte[] buffer = new byte[1];
            //buffer[0] = Convert.ToByte(activate);
            //socket.Connect(endpoint);
            socket.Send(Encoding.UTF8.GetBytes("activate:" + param.ToString()));
            //socket.Disconnect(true);

        }




        public override void Start()
        {
            //socket.Connect(endpoint);
            listener.Start();
            socket.Send(Encoding.UTF8.GetBytes("start"));
            //socket.Disconnect(true);
        }

        public override void Shutdown()
        {
            //socket.Connect(endpoint);
            socket.Send(Encoding.UTF8.GetBytes("shutdown"));
            listener.Stop();
            socket.Disconnect(true);
            //socket.Disconnect(true);
        }

        public override void SetInterpolationResult(Interpolation.InterpolationResult result)
        {
            this.result = result;
        }



    }
}