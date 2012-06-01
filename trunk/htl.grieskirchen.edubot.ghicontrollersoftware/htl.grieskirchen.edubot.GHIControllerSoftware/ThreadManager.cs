using System;
using Microsoft.SPOT;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace htl.grieskirchen.edubot.GHIControllerSoftware
{
    public class ThreadManager
    {
        Thread clientListener;
        Thread dataListener;
        Thread calculator;
        Socket server;
        string message;
        const Int32 c_port = 12000;
        int currentIndex;

        public ThreadManager() {
            Executer.acceptNewClient = true;
            Executer.newClientRequest = false;
            currentIndex = 0;
            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, c_port);
            server.Bind(localEndPoint);
            server.Listen(Int32.MaxValue);
            
        }

        public void StartThreads() {
            clientListener = new Thread(ListenForClients);
            clientListener.Priority = ThreadPriority.BelowNormal;
            clientListener.Start();
        }

        private void Calculate() {
            string[] positions = message.Split('&');
            string[] stepcontainer;
            double[] primStepBuffer = new double[2];
            double[] secStepBuffer = new double[2];
            Executer.primarySteps = new int[positions.Length];
            Executer.secondarySteps = new int[positions.Length];
            Executer.primaryDir = new bool[positions.Length];
            Executer.secondaryDir = new bool[positions.Length];
            int[] primarySteps = Executer.primarySteps;
            int[] secondarySteps = Executer.secondarySteps;
            bool[] primaryDir = Executer.primaryDir;
            bool[] secondaryDir = Executer.secondaryDir;
            

            //Split the message into steps
            for (int i = 0; i < positions.Length; i++)
            {
                string s = positions[i];

                stepcontainer = s.Split(';');

                if (stepcontainer.Length <= 1)
                {
                    break;
                }

                primarySteps[i] = int.Parse(stepcontainer[0]);
                if (primarySteps[i] < 0) {
                    primaryDir[i] = true;
                    primarySteps[i] *= -1;
                }
                else
                {
                    primaryDir[i] = false;
                }

                secondarySteps[i] = int.Parse(stepcontainer[1]);
                if (secondarySteps[i] < 0)
                {
                    secondaryDir[i] = false;
                    secondarySteps[i] *= -1;
                }
                else {
                    secondaryDir[i] = true;
                }



                //primStepBuffer[0] = Double.Parse(stepcontainer[0]) / 0.1125;
                //if (stepcontainer[0].Substring(0, 1) == "-" && primStepBuffer[0] > 0)
                //{
                //    primaryDir[i] = true;
                //    primStepBuffer[1] *= -1;

                //}
                //else
                //{
                //    if (primStepBuffer[0] < 0)
                //    {
                //        primStepBuffer[0] *= -1;
                //        primaryDir[i] = true;
                //    }
                //    else
                //    {
                //        primaryDir[i] = false;
                //    }
                //}
                //if (primStepBuffer[0] % 1 > 0.5) { 
                //    primarySteps[i] = (int)primStepBuffer[]
                //}



            //    primStepBuffer[0] = Double.Parse(stepcontainer[0]) / 0.1125;
            //    primStepBuffer[0] += primStepBuffer[1];
            //    primStepBuffer[1] = primStepBuffer[0] % 1;
            //    if (stepcontainer[0].Substring(0, 1) == "-" && primStepBuffer[0] > 0)
            //    {
            //        primaryDir[i] = true;
            //        primStepBuffer[1] *= -1;

            //    }
            //    else
            //    {
            //        if (primStepBuffer[0] < 0)
            //        {
            //            primStepBuffer[0] *= -1;
            //            primaryDir[i] = true;
            //        }
            //        else
            //        {
            //            primaryDir[i] = false;
            //        }

            //    }
            //    primarySteps[i] = (int)primStepBuffer[0];



            //    secStepBuffer[0] = Double.Parse(stepcontainer[1]) / 0.1125;
            //    secStepBuffer[0] += secStepBuffer[1];
            //    secStepBuffer[1] = secStepBuffer[0] % 1;
            //    if (stepcontainer[1].Substring(0, 1) == "-" && secStepBuffer[0] > 0)
            //    {
            //        secondaryDir[i] = false;
            //        secStepBuffer[1] *= -1;

            //    }
            //    else
            //    {
            //        if (secStepBuffer[0] < 0)
            //        {
            //            secStepBuffer[0] *= -1;
            //            secondaryDir[i] = false;
            //        }
            //        else
            //        {
            //            secondaryDir[i] = true;
            //        }

            //    }
            //    secondarySteps[i] = (int)secStepBuffer[0];

            }
            

        }

        private void ListenForData() {
            try
            {
                Socket m_clientSocket = Executer.clientSocket;
                while (!Executer.acceptNewClient)
                {
                    Byte[] buffer = new Byte[100000];
                    Byte[] readBuffer = new Byte[512];
                    int curBufferIndex = 0;
                    //try
                    //{

                    //    m_clientSocket.Send(Encoding.UTF8.GetBytes("hello"));
                    //}
                    //catch (System.Net.Sockets.SocketException)
                    //{
                    //    m_clientSocket.Close();
                    //    MySocketServer.acceptNewClient = true;
                    //    continue;
                    //}

                    //string message = "";
                    //int available = m_clientSocket.Available;
                    int bytesRead = 0;
                    double[] primStepBuffer = new double[2];
                    double[] secStepBuffer = new double[2];

                    while (m_clientSocket.Available > 0)
                    {

                        bytesRead = m_clientSocket.Receive(readBuffer);
                        for (int i = 0; i < bytesRead; i++)
                        {
                            buffer[curBufferIndex] = readBuffer[i];
                            curBufferIndex++;
                        }
                    }

                    if (curBufferIndex > 0)
                    {
                        message = new String(System.Text.UTF8Encoding.UTF8.GetChars(buffer));
                        Debug.Print(message);


                        if (message.Substring(0, 3) == "mvs")
                        {
                            Debug.Print("LinearMovementInitiated");
                            message = message.Substring(4, message.Length - 4);
                            calculator = new Thread(Calculate);
                            calculator.Priority = ThreadPriority.BelowNormal;
                            calculator.Start();
                            Executer.action = 3;
                        }
                        else
                        {
                            if (message.Substring(0, 3) == "mvc")
                            {
                                Debug.Print("CircularMovementInitiates");
                                message = message.Substring(4, message.Length - 4);
                                calculator = new Thread(Calculate);
                                calculator.Priority = ThreadPriority.BelowNormal;
                                calculator.Start();
                                Executer.action = 3;
                            }
                            else
                            {
                                if (message.Substring(0, 3) == "hom")
                                {
                                    Debug.Print("HomingInitiated");
                                    Executer.action = 1;
                                    //m_clientSocket.Send(Encoding.UTF8.GetBytes("ready"));
                                    message = "";
                                }
                                else
                                {
                                    if (message.Substring(0, 3) == "sht")
                                    {
                                        Debug.Print("ShutDown");
                                        Executer.action = 2;
                                        m_clientSocket.Send(Encoding.UTF8.GetBytes("shutdown"));
                                        m_clientSocket.Close();
                                        Executer.acceptNewClient = true;
                                        message = "";
                                    }
                                    else
                                    {
                                        if (message.Substring(0, 3) == "abo")
                                        {
                                            Debug.Print("Aborting");
                                            Executer.action = 6;
                                            m_clientSocket.Send(Encoding.UTF8.GetBytes("shutdown"));
                                            Executer.abort = true;
                                            m_clientSocket.Close();
                                            Executer.acceptNewClient = true;
                                            message = "";
                                        }
                                        else
                                        {
                                            if (message.Substring(0, 3) == "ust")
                                            {
                                                Debug.Print("USETool");
                                                Executer.action = 5;
                                                m_clientSocket.Send(Encoding.UTF8.GetBytes("ready"));
                                                message = "";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }catch(SocketException ex){
                Executer.clientSocket.Close();
                Executer.acceptNewClient = true;
            }
        }

        private void ListenForClients() {
            while (true)
            {

                if (Executer.acceptNewClient)
                {
                    Executer.clientSocket = server.Accept();

                    //if(Executer.clientSocket.
                    Executer.newClientRequest = true;
                    Executer.acceptNewClient = false;
                    
                        dataListener = new Thread(ListenForData);
                    
                    dataListener.Priority = ThreadPriority.BelowNormal;
                    dataListener.Start();
                }
            }
        }
    }
}
