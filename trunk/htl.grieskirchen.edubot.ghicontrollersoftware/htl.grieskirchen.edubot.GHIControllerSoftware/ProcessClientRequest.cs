using System;
using Microsoft.SPOT;

using Socket = System.Net.Sockets.Socket;
using System.Text;

namespace htl.grieskirchen.edubot.GHIControllerSoftware
{
    /// <summary>
    /// Processes a client request.
    /// </summary>
    internal sealed class ProcessClientRequest
    {


        private Socket m_clientSocket;
        int[] currentSteps = new int[2];

        private Engine primaryEngine;
        private Engine secondaryEngine;

        /// <summary>
        /// The constructor calls another method to handle the request, but can 
        /// optionally do so in a new thread.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="asynchronously"></param>
        public ProcessClientRequest()
        {
            this.m_clientSocket = Executer.clientSocket;
            this.primaryEngine = Executer.primaryEngine;
            this.secondaryEngine = Executer.secondaryEngine;
            
           
            
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        public void ProcessRequest()
        {

            // 'using' ensures that the client's socket gets closed.
            //using (m_clientSocket)
            //{
                
                while(!Executer.acceptNewClient){
                Byte[] buffer = new Byte[100000];
                Byte[] readBuffer = new Byte[512];
                int curBufferIndex = 0;
                try
                {

                    m_clientSocket.Send(Encoding.UTF8.GetBytes("hello"));
                }
                catch (System.Net.Sockets.SocketException) {
                    m_clientSocket.Close();
                    Executer.acceptNewClient = true;
                    continue;
                }

                    string message = "";
                    //int available = m_clientSocket.Available;
                    int bytesRead = 0;
                    double[] primStepBuffer = new double[2];
                    double[] secStepBuffer = new double[2];

                    while (m_clientSocket.Available > 0)
                    {

                        bytesRead = m_clientSocket.Receive(readBuffer);
                        for (int i = 0; i < bytesRead; i++) {
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
                        }
                        else
                        {
                            if (message.Substring(0, 3) == "mvc")
                            {
                                Debug.Print("CircularMovementInitiates");

                                message = message.Substring(4, message.Length - 4);
                            }
                            else
                            {
                                if (message.Substring(0, 3) == "hom")
                                {
                                    Debug.Print("HomingInitiated");
                                    m_clientSocket.Send(Encoding.UTF8.GetBytes("ready"));
                                    message = "";
                                }
                                else
                                {
                                    if (message.Substring(0, 3) == "sht")
                                    {
                                        Debug.Print("ShutDown");
                                        m_clientSocket.Send(Encoding.UTF8.GetBytes("shutdown"));
                                        m_clientSocket.Close();
                                        Executer.acceptNewClient = true;
                                        message = "";
                                    }
                                }
                            }
                        }
                    }


                    if (message != "")
                    {
                        //MySocketServer.calculationManager.Message = message;
                        //new System.Threading.Thread(MySocketServer.calculationManager.Calculate).Start();
                        //string[] positions = message.Split('&');
                        //string[] stepcontainer;
                        //int[] primarySteps = new int[positions.Length];
                        //int[] secondarySteps = new int[positions.Length];
                        //bool[] primaryDir = new bool[positions.Length];
                        //bool[] secondaryDir = new bool[positions.Length];

                        //for (int i = 0; i < positions.Length; i++)
                        //{
                        //    string s = positions[i];

                        //    stepcontainer = s.Split(';');

                        //    if (stepcontainer.Length < 1)
                        //    {
                        //        break;
                        //    }



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
                        //    secStepBuffer[1] = secStepBuffer[0]%1;
                        //    if (stepcontainer[1].Substring(0, 1) == "-" && secStepBuffer[0] > 0)
                        //    {
                        //        secondaryDir[i] = false;
                        //        secStepBuffer[1] *= -1;

                        //    }
                        //    else {
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




                        //}
                        //double speed = 100000;
                        //long primaryEngineLastTick;
                        //long secondaryEngineLastTick;
                        //double rel;

                        //for (int i = 0; i < positions.Length; i++)
                        //{
                        //    primaryEngine.engineDir.Write(primaryDir[i]);
                        //    secondaryEngine.engineDir.Write(secondaryDir[i]);

                            
                        //        if (primarySteps[i] > secondarySteps[i])
                        //        {
                        //            if (secondarySteps[i] > 0)
                        //            {
                        //                rel = (double)primarySteps[i] / (double)secondarySteps[i];

                        //            }
                        //            else {
                        //                rel = Double.MaxValue;
                        //            }
                        //            primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                        //            secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;

                        //            while (primarySteps[i] > 0 || secondarySteps[i] > 0)
                        //            {
                        //                if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - primaryEngineLastTick > speed && primarySteps[i] > 0)
                        //                {
                        //                    primaryEngine.engineFreq.Write(true);
                        //                    primaryEngine.engineFreq.Write(false);
                        //                    primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                        //                    primarySteps[i]--;

                        //                }

                        //                if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - secondaryEngineLastTick > speed * rel && secondarySteps[i] > 0)
                        //                {
                        //                    secondaryEngine.engineFreq.Write(true);
                        //                    secondaryEngine.engineFreq.Write(false);

                        //                    secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                        //                    secondarySteps[i]--;
                        //                }
                        //            }
                        //        }
                        //        else {

                        //            if (primarySteps[i] > 0)
                        //            {
                        //                rel = (double)secondarySteps[i] / (double)primarySteps[i];

                        //            }
                        //            else
                        //            {
                        //                rel = Double.MaxValue;
                        //            }
                                
                        //            primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                        //            secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;

                        //            while (primarySteps[i] > 0 || secondarySteps[i] > 0)
                        //            {
                        //                if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - primaryEngineLastTick > speed*rel && primarySteps[i] > 0)
                        //                {
                        //                    primaryEngine.engineFreq.Write(true);
                        //                    primaryEngine.engineFreq.Write(false);
                        //                    primaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                        //                    primarySteps[i]--;

                        //                }

                        //                if (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - secondaryEngineLastTick > speed && secondarySteps[i] > 0)
                        //                {
                        //                    secondaryEngine.engineFreq.Write(true);
                        //                    secondaryEngine.engineFreq.Write(false);

                        //                    secondaryEngineLastTick = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
                        //                    secondarySteps[i]--;
                        //                }
                        //            }
                        //        }
                                

                        //    }
                       
                        m_clientSocket.Send(Encoding.UTF8.GetBytes("ready"));
                        }
                    
                    }
                
                //        double rel;
                //        double small_count = 0;

                //        bool primaryStepsNegative = false;
                //        bool secondaryStepsNegative = false;

                //        for (int i = 0; i < positions.Length; i++)
                //        {
                //            primaryStepsNegative = false;
                //            secondaryStepsNegative = false;

                //            if (primarySteps[i] < 0)
                //            {
                //                primarySteps[i] *= -1;
                //                primaryEngine.engineDir.Write(true);
                //                primaryStepsNegative = true;
                //            }
                //            else
                //            {
                //                primaryEngine.engineDir.Write(false);

                //            }

                //            if (secondarySteps[i] < 0)
                //            {
                //                secondarySteps[i] *= -1;
                //                secondaryEngine.engineDir.Write(false);
                //                secondaryStepsNegative = true;
                //            }
                //            else
                //            {
                //                secondaryEngine.engineDir.Write(true);
                //            }


                //            small_count = 0;

                //            if (primarySteps[i] > secondarySteps[i])
                //            {
                //                rel = secondarySteps[i] / primarySteps[i];

                //                for (int j = 0; j < primarySteps[i]; j++)
                //                {

                //                    Thread.Sleep(10);
                //                    small_count += rel;
                //                    if ((int)(small_count / 1) > 0)
                //                    {
                //                        small_count = small_count % 1;
                //                        secondaryEngine.engineFreq.Write(true);
                //                        secondaryEngine.engineFreq.Write(false);
                //                    }

                //                    primaryEngine.engineFreq.Write(true);
                //                    primaryEngine.engineFreq.Write(false);

                //                }
                //                if (i + 1 < positions.Length)
                //                {
                //                    if (secondaryStepsNegative)
                //                        secondarySteps[i + 1] += small_count * -1;
                //                    else
                //                        secondarySteps[i + 1] += small_count;
                //                    if (primaryStepsNegative)
                //                        primarySteps[i + 1] += (primarySteps[i] % 1) * -1;
                //                    else
                //                        primarySteps[i + 1] += primarySteps[i] % 1;
                //                }
                //            }

                //            else
                //            {
                //                rel = primarySteps[i] / secondarySteps[i];

                //                for (int j = 0; j < secondarySteps[i]; j++)
                //                {
                //                    Thread.Sleep(10);
                //                    small_count += rel;
                //                    if ((int)(small_count / 1) > 0)
                //                    {
                //                        small_count = small_count % 1;
                //                        primaryEngine.engineFreq.Write(true);
                //                        primaryEngine.engineFreq.Write(false);
                //                    }

                //                    secondaryEngine.engineFreq.Write(true);
                //                    secondaryEngine.engineFreq.Write(false);

                //                }

                //                if (i + 1 < positions.Length)
                //                {
                //                    if (primaryStepsNegative)
                //                        primarySteps[i + 1] += small_count * -1;
                //                    else
                //                        primarySteps[i + 1] += small_count;

                //                    if (secondaryStepsNegative)
                //                        secondarySteps[i + 1] += (secondarySteps[i] % 1) * -1;
                //                    else
                //                        secondarySteps[i + 1] += secondarySteps[i] % 1;
                //                }
                //            }
                //        }
                    
                








                //    if (message != "")
                //    {

                //        string[] positions = message.Split('&');
                //        string[] stepcontainer;


                //        for (int i = 0; i < positions.Length; i++)
                //        {
                //            string s = positions[i];

                //            stepcontainer = s.Split(';');

                //            if (stepcontainer.Length < 1)
                //            {
                //                break;
                //            }

                            

                //            primaryEngine.Steps = Double.Parse(stepcontainer[0]) / 0.1125;
                //            if (stepcontainer[0].Substring(0,1)== "-" && primaryEngine.Steps > 0) {
                //                primaryEngine.Steps *= -1; 
                //            }

                //            primaryEngine.Steps += primaryEngine.StepsLeft;

                //            secondaryEngine.Steps = Double.Parse(stepcontainer[1]) / 0.1125;
                //            if (stepcontainer[1].Substring(0, 1) == "-" && secondaryEngine.Steps > 0)
                //            {
                //                secondaryEngine.Steps *= -1;
                //            }

                //            secondaryEngine.Steps += secondaryEngine.StepsLeft;
                            


                            

                //            bool primaryStepsNegative = false;
                //            bool secondaryStepsNegative = false;

                //            if (primaryEngine.Steps < 0)
                //            {
                //                primaryEngine.Steps = primaryEngine.Steps * -1;
                //                primaryEngine.engineDir.Write(true);
                //                primaryStepsNegative = true;
                //            }
                //            else
                //            {
                //                primaryEngine.engineDir.Write(false);
                                
                //            }

                //            if (secondaryEngine.Steps < 0)
                //            {
                //                secondaryEngine.Steps = secondaryEngine.Steps * -1;
                //                secondaryEngine.engineDir.Write(false);
                //                secondaryStepsNegative = true;
                //            }
                //            else
                //            {
                //                secondaryEngine.engineDir.Write(true);
                //            }

                //            double rel;
                //            double small_count = 0;

                //            if (primaryEngine.Steps > secondaryEngine.Steps)
                //            {
                //                rel = (double)secondaryEngine.Steps / (double)primaryEngine.Steps;
                //                small_count = 0;
                //                for (int j = 0; j < primaryEngine.Steps; j++)
                //                {

                //                    Thread.Sleep(4);
                //                    small_count += rel;
                //                    if ((int)(small_count / 1) > 0)
                //                    {
                //                        small_count = small_count % 1;
                //                        secondaryEngine.engineFreq.Write(true);
                //                        secondaryEngine.engineFreq.Write(false);
                //                    }

                //                    primaryEngine.engineFreq.Write(true);
                //                    primaryEngine.engineFreq.Write(false);

                //                }

                //                if (secondaryStepsNegative)
                //                    secondaryEngine.StepsLeft = small_count * -1;
                //                else
                //                    secondaryEngine.StepsLeft = small_count;
                //                if (primaryStepsNegative)
                //                    primaryEngine.StepsLeft = (primaryEngine.Steps % 1) * -1;
                //                else
                //                    primaryEngine.StepsLeft = primaryEngine.Steps % 1;

                //            }

                //            else
                //            {
                //                rel = primaryEngine.Steps / secondaryEngine.Steps;
                //                small_count = 0;

                                

                //                for (int j = 0; j < secondaryEngine.Steps; j++)
                //                {
                //                    Thread.Sleep(4);
                //                    small_count += rel;
                //                    if ((int)(small_count / 1) > 0)
                //                    {
                //                        small_count = small_count % 1;
                //                        primaryEngine.engineFreq.Write(true);
                //                        primaryEngine.engineFreq.Write(false);
                //                    }

                //                    secondaryEngine.engineFreq.Write(true);
                //                    secondaryEngine.engineFreq.Write(false);

                //                }

                                
                //                if (primaryStepsNegative)
                //                    primaryEngine.StepsLeft = small_count*-1;
                //                else
                //                    primaryEngine.StepsLeft = small_count;

                //                if (secondaryStepsNegative)
                //                    secondaryEngine.StepsLeft = (secondaryEngine.Steps % 1)*-1;
                //                else
                //                    secondaryEngine.StepsLeft = secondaryEngine.Steps % 1;


                                
                                

                //            }

                //        }
                //    }
                //}

                //m_clientSocket.Close();    
            }
        }
    
}
