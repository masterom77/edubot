using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;

namespace htl.grieskirchen.edubot.GHIControllerSoftware
{
    class NetworkManager
    {
        Socket server;
        Socket client;
        const Int32 c_port = 12000;



        public Socket Server
        {
            get { return server; }
            set { server = value; }
        }

        public NetworkManager() {
            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, c_port);
            server.Bind(localEndPoint);
            server.Listen(Int32.MaxValue);


        }

        public void Listen() {
            while (true) {

                if (Executer.acceptNewClient)
                {

                    Executer.clientSocket = server.Accept();

                    //if(Executer.clientSocket.)
                    Executer.newClientRequest = true;
                    Executer.acceptNewClient = false;
                }
            }


        }

        
        
    }
}
