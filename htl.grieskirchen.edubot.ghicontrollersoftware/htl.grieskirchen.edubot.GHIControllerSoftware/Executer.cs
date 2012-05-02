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
       
        public static InputPort secondaryHoming;
        public static InputPort primaryHoming;

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
            
            primaryHoming = new InputPort(EMX.Pin.IO9, false,Port.ResistorMode.PullDown);
            secondaryHoming = new InputPort(EMX.Pin.IO8, false, Port.ResistorMode.PullDown);

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
            while (true)
            {

                switch (action)
                {
                    case 0: //IDLE
                        break;
                    case 1: //Homing
                        StartHoming();

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

        public static void StartHoming() {
            //while (true)
            //{
            //    Thread.Sleep(200);
            //    Debug.Print("primaryHoming" + primaryHoming.Read());
            //    Debug.Print("secondaryHoming" + secondaryHoming.Read());
            //}
            long lasttick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            int delay = 50000;
            int primaryHomeSteps = 1250;
            int secondaryHomeSteps = 1330;

            primaryEngine.engineDir.Write(false);
            while (!secondaryHoming.Read())
            {
                if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - lasttick > delay)
                {
                    secondaryEngine.engineFreq.Write(true);
                    secondaryEngine.engineFreq.Write(false);
                    lasttick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                }
            }

            secondaryEngine.engineDir.Write(false);
            while (!primaryHoming.Read())
            {
                if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - lasttick > delay)
                {

                    primaryEngine.engineFreq.Write(true);
                    primaryEngine.engineFreq.Write(false);
                    lasttick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                }
            }

            primaryEngine.engineDir.Write(true);
            secondaryEngine.engineDir.Write(true);
            while (secondaryHomeSteps > 0 || primaryHomeSteps > 0) {
                if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - lasttick > delay)
                {
                    if (secondaryHomeSteps > 0) {
                        secondaryEngine.engineFreq.Write(true);
                        secondaryEngine.engineFreq.Write(false);
                        secondaryHomeSteps--;
                    }
                    if (primaryHomeSteps > 0) {
                        primaryEngine.engineFreq.Write(true);
                        primaryEngine.engineFreq.Write(false);
                        primaryHomeSteps--;
                    }

                    lasttick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                }
            }

            clientSocket.Send(Encoding.UTF8.GetBytes("ready"));
        }

        public static void Move()
        {

            double rel;
            double small_count;
            int delay = 50000;
            long lasttick;

            for (int i = 0; i < primarySteps.Length; i++)
            {
                primaryEngine.engineDir.Write(primaryDir[i]);
                secondaryEngine.engineDir.Write(secondaryDir[i]);
                small_count = 0;
                rel = 0;
                lasttick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;

                if (primarySteps[i] > secondarySteps[i])
                {
                    if(primarySteps[i]>0)
                    rel = (double)secondarySteps[i] / (double)primarySteps[i];

                    for (int j = 0; j < primarySteps[i]; )
                    {

                        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - lasttick > delay)
                        {
                            small_count += rel;

                            if (rel > 0.5 && small_count > 1)
                            {
                                small_count = small_count % 1;
                                secondaryEngine.engineFreq.Write(true);
                                secondaryEngine.engineFreq.Write(false);
                            }
                            else
                            {
                                if (rel <= 0.5 && small_count > 0.5)
                                {
                                    small_count = 0;
                                    secondaryEngine.engineFreq.Write(true);
                                    secondaryEngine.engineFreq.Write(false);
                                }
                            }

                            primaryEngine.engineFreq.Write(true);
                            primaryEngine.engineFreq.Write(false);
                            j++;
                            lasttick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                        }
                    }

                }
                else
                {
                    if (secondarySteps[i] > 0)
                    rel = (double)primarySteps[i] / (double)secondarySteps[i];

                    for (int j = 0; j < secondarySteps[i];)
                    {
                        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - lasttick > delay)
                        {
                            small_count += rel;
                            if (rel > 0.5 && small_count > 1)
                            {
                                small_count = small_count % 1;
                                primaryEngine.engineFreq.Write(true);
                                primaryEngine.engineFreq.Write(false);
                            }
                            else
                            {
                                if (rel <= 0.5 && small_count > 0.5)
                                {
                                    small_count = 0;
                                    primaryEngine.engineFreq.Write(true);
                                    primaryEngine.engineFreq.Write(false);
                                }
                            }

                            secondaryEngine.engineFreq.Write(true);
                            secondaryEngine.engineFreq.Write(false);

                            j++;
                            lasttick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                        }
                    }
                }


                //primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                //secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;

                //small_count = 0;

                //        if (primarySteps[i] > secondarySteps[i])
                //        {
                //            rel = secondarySteps[i] / primarySteps[i];

                //            for (int j = 0; j < primarySteps[i]; j++)
                //            {

                //                Thread.Sleep(10);
                //                small_count += rel;
                //                if ((int)(small_count / 1) > 0)
                //                {
                //                    small_count = small_count % 1;
                //                    secondaryEngine.engineFreq.Write(true);
                //                    secondaryEngine.engineFreq.Write(false);
                //                }

                //                primaryEngine.engineFreq.Write(true);
                //                primaryEngine.engineFreq.Write(false);

                //            }
                //            if (i + 1 < positions.Length)
                //            {
                //                if (secondaryStepsNegative)
                //                    secondarySteps[i + 1] += small_count * -1;
                //                else
                //                    secondarySteps[i + 1] += small_count;
                //                if (primaryStepsNegative)
                //                    primarySteps[i + 1] += (primarySteps[i] % 1) * -1;
                //                else
                //                    primarySteps[i + 1] += primarySteps[i] % 1;
                //            }
                //        }

                //        else
                //        {
                //            rel = primarySteps[i] / secondarySteps[i];

                //            for (int j = 0; j < secondarySteps[i]; j++)
                //            {
                //                Thread.Sleep(10);
                //                small_count += rel;
                //                if ((int)(small_count / 1) > 0)
                //                {
                //                    small_count = small_count % 1;
                //                    primaryEngine.engineFreq.Write(true);
                //                    primaryEngine.engineFreq.Write(false);
                //                }

                //                secondaryEngine.engineFreq.Write(true);
                //                secondaryEngine.engineFreq.Write(false);

                //            }


                //    while (primarySteps[i] > 0 || secondarySteps[i] > 0)
                //    {
                //        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - primaryEngineLastTick > delay && primarySteps[i] > 0)
                //        {
                //            primaryEngine.engineFreq.Write(true);
                //            primaryEngine.engineFreq.Write(false);
                //            primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                //            primarySteps[i]--;

                //        }

                //        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - secondaryEngineLastTick > delay * rel && secondarySteps[i] > 0)
                //        {
                //            secondaryEngine.engineFreq.Write(true);
                //            secondaryEngine.engineFreq.Write(false);

                //            secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                //            secondarySteps[i]--;
                //        }
                //    }
                //}
                //else
                //{

                //    if (primarySteps[i] > 0)
                //    {
                //        rel = (double)secondarySteps[i] / (double)primarySteps[i];

                //    }
                //    else
                //    {
                //        rel = Double.MaxValue;
                //    }

                //    primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                //    secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;

                //    while (primarySteps[i] > 0 || secondarySteps[i] > 0)
                //    {
                //        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - primaryEngineLastTick > delay * rel && primarySteps[i] > 0)
                //        {
                //            primaryEngine.engineFreq.Write(true);
                //            primaryEngine.engineFreq.Write(false);
                //            primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                //            primarySteps[i]--;

                //        }

                //        if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - secondaryEngineLastTick > delay && secondarySteps[i] > 0)
                //        {
                //            secondaryEngine.engineFreq.Write(true);
                //            secondaryEngine.engineFreq.Write(false);

                //            secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                //            secondarySteps[i]--;
                //        }
                //    }
                //}


            }
            clientSocket.Send(Encoding.UTF8.GetBytes("ready"));
        }
    }
}

   
    
