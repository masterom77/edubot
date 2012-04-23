////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using GHIElectronics.NETMF.Hardware;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.IO;
using Socket = System.Net.Sockets.Socket;

namespace htl.grieskirchen.edubot.GHIControllerSoftware
{
    /// <summary>
    /// </para>
    /// </summary>
    public static class Executer
    {
        public static Engine primaryEngine;
        public static Engine secondaryEngine;

        public static bool newClientRequest;
        public static bool acceptNewClient;
        public static Socket clientSocket;

        public static int[] primarySteps;
        public static int[] secondarySteps;
        public static bool[] primaryDir;
        public static bool[] secondaryDir;

        public static int bufferSize;
        public static int action;


        public static void Main()
        {
            acceptNewClient = true;
            primaryEngine = new Engine(1);
            secondaryEngine = new Engine(2);
            //bufferSize = 4096;
            //primarySteps = new int[bufferSize];
            //secondarySteps = new int[bufferSize];
            //primaryDir = new bool[bufferSize];
            //secondaryDir = new bool[bufferSize];
            const Int32 c_port = 12000;

            Debug.EnableGCMessages(false);

            // Create a socket, bind it to the server's port, and listen for client 
            // connections.
            GHIElectronics.NETMF.Net.Ethernet.Enable();


            ThreadManager manager = new ThreadManager();
            manager.StartThreads();
            
            //NetworkManager network = new NetworkManager();
            //calculationManager = new CalculationManager();
            

            //    // Wait for a client to connect.

            //    Thread networkThread = new System.Threading.Thread(new ThreadStart(network.Listen));
            //    networkThread.Priority = ThreadPriority.BelowNormal;
            //    networkThread.Start();
                while (true) {
                    
                        switch (action) {
                            case 0: //IDLE
                                break;
                            case 1: //Homing
                                
                                
                                action = 0;
                                break;
                            case 2: //SHUTDOWN
                                action = 0;
                                break;
                            case 3:
                                Thread.Sleep(20);
                                Move();

                                action = 0;
                                break;
                            case 4: //MVC
                                action = 0;
                                break;
                            case 5: //USE TOOL
                                action = 0;
                                break;
                        }
                        //new ProcessClientRequest().ProcessRequest();
                     
                }
                // Process the client request.  true means asynchronous.

            
        }

        public static void Move() {
            double speed = 10000;
            long primaryEngineLastTick;
            long secondaryEngineLastTick;
            double rel;

            for (int i = 0; i < primarySteps.Length; i++)
            {
                primaryEngine.engineDir.Write(primaryDir[i]);
                secondaryEngine.engineDir.Write(secondaryDir[i]);


                if (primarySteps[i] > secondarySteps[i])
                {
                    if (secondarySteps[i] > 0)
                    {
                        rel = (double)primarySteps[i] / (double)secondarySteps[i];

                    }
                    else
                    {
                        rel = Double.MaxValue;
                    }
                    primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                    secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;

                    while (primarySteps[i] > 0 || secondarySteps[i] > 0)
                    {
                        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - primaryEngineLastTick > speed && primarySteps[i] > 0)
                        {
                            primaryEngine.engineFreq.Write(true);
                            primaryEngine.engineFreq.Write(false);
                            primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                            primarySteps[i]--;

                        }

                        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - secondaryEngineLastTick > speed * rel && secondarySteps[i] > 0)
                        {
                            secondaryEngine.engineFreq.Write(true);
                            secondaryEngine.engineFreq.Write(false);

                            secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                            secondarySteps[i]--;
                        }
                    }
                }
                else
                {

                    if (primarySteps[i] > 0)
                    {
                        rel = (double)secondarySteps[i] / (double)primarySteps[i];

                    }
                    else
                    {
                        rel = Double.MaxValue;
                    }

                    primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                    secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;

                    while (primarySteps[i] > 0 || secondarySteps[i] > 0)
                    {
                        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - primaryEngineLastTick > speed * rel && primarySteps[i] > 0)
                        {
                            primaryEngine.engineFreq.Write(true);
                            primaryEngine.engineFreq.Write(false);
                            primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                            primarySteps[i]--;

                        }

                        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - secondaryEngineLastTick > speed && secondarySteps[i] > 0)
                        {
                            secondaryEngine.engineFreq.Write(true);
                            secondaryEngine.engineFreq.Write(false);

                            secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                            secondarySteps[i]--;
                        }
                    }
                }


            }
            clientSocket.Send(Encoding.UTF8.GetBytes("ready"));
        }
    }

   
    }
