using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;

namespace htl.grieskirchen.edubot.GHIControllerSoftware
{
    class CalculationManager
    {
        const Int32 c_port = 12000;
        int currentIndex;
        string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public CalculationManager()
        {
            currentIndex = 0;

        }

        public void Calculate()
        {
            string[] positions = message.Split('&');
            string[] stepcontainer;
            double[] primStepBuffer = new double[2];
            double[] secStepBuffer = new double[2];
            int[] primarySteps = Executer.primarySteps;
            int[] secondarySteps = Executer.secondarySteps;
            bool[] primaryDir = Executer.primaryDir;
            bool[] secondaryDir = Executer.secondaryDir;

            //Split the message into steps
            for (int i = 0; i < positions.Length; i++)
            {
                string s = positions[i];

                stepcontainer = s.Split(';');

                if (stepcontainer.Length < 1)
                {
                    break;
                }



                primStepBuffer[0] = Double.Parse(stepcontainer[0]) / 0.1125;
                primStepBuffer[0] += primStepBuffer[1];
                primStepBuffer[1] = primStepBuffer[0] % 1;
                if (stepcontainer[0].Substring(0, 1) == "-" && primStepBuffer[0] > 0)
                {
                    primaryDir[i] = true;
                    primStepBuffer[1] *= -1;

                }
                else
                {
                    if (primStepBuffer[0] < 0)
                    {
                        primStepBuffer[0] *= -1;
                        primaryDir[currentIndex] = true;
                    }
                    else
                    {
                        primaryDir[currentIndex] = false;
                    }

                }
                primarySteps[currentIndex] = (int)primStepBuffer[0];



                secStepBuffer[0] = Double.Parse(stepcontainer[1]) / 0.1125;
                secStepBuffer[0] += secStepBuffer[1];
                secStepBuffer[1] = secStepBuffer[0] % 1;
                if (stepcontainer[1].Substring(0, 1) == "-" && secStepBuffer[0] > 0)
                {
                    secondaryDir[i] = false;
                    secStepBuffer[i] *= -1;

                }
                else
                {
                    if (secStepBuffer[0] < 0)
                    {
                        secStepBuffer[0] *= -1;
                        secondaryDir[i] = false;
                    }
                    else
                    {
                        secondaryDir[i] = true;
                    }

                }
                secondarySteps[i] = (int)secStepBuffer[0];
                
            }
            //clientSocket.Send(Encoding.UTF8.GetBytes("ready"));

        }

        
        
    }
}
