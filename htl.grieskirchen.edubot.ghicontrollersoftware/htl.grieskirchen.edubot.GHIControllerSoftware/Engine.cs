using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace htl.grieskirchen.edubot.GHIControllerSoftware
{
    public class Engine
    {
        public OutputPort engineEnabled;
        public OutputPort engineFreq;
        public OutputPort engineDir;
        public OutputPort enginePowerDown;

        private int speed;

        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }



        private double stepsLeft = 0;

        public double StepsLeft
        {
            get { return stepsLeft; }
            set { stepsLeft = value; }
        }

        private double steps;

        public double Steps
        {
            get { return steps; }
            set { steps = value; }
        }

        public Engine(int engineNumber) {
            switch (engineNumber) { 
                case 1:
                   engineEnabled = new OutputPort(EMX.Pin.IO0, false);
                   engineFreq = new OutputPort(EMX.Pin.IO2, false);
                   engineDir = new OutputPort(EMX.Pin.IO1, false);
                   enginePowerDown = new OutputPort(EMX.Pin.IO3, false);
                   break;
                case 2:
                   engineEnabled = new OutputPort(EMX.Pin.IO4, false);
                   engineFreq = new OutputPort(EMX.Pin.IO6, false);
                   engineDir = new OutputPort(EMX.Pin.IO5, false);
                   enginePowerDown = new OutputPort(EMX.Pin.IO7, false);
                   break;
            
            }
        }

        public void GoSteps()
        {
            if (steps < 0)
            {
                steps = steps * -1;
                engineDir.Write(true);
            }
            else
            {
                engineDir.Write(false);
            }

            while (steps > 0)
            {


                engineFreq.Write(true);
                engineFreq.Write(false);
                steps--;
            }
        }
    }
}
