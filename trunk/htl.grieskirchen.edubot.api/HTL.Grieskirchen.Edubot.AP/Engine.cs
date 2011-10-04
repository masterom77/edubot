using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EduBotAPI
{
    public class Engine
    {
        private Thread clockThread;
        private bool running;
        private LIBADX.LIBADX usbCon;

        public Engine()
        {
            usbCon = new LIBADX.LIBADX();
            
        }

        #region Controller Variables
        public bool Enabled
        {
            get { return usbCon.get_DigitalOutLine(1,0); }
            set { usbCon.set_DigitalOutLine(1,0, value); }
        }

        public bool Direction
        {
            get { return usbCon.get_DigitalOutLine(1, 1); }
            set { usbCon.set_DigitalOutLine(1, 1, value); }
        }

        bool clock;
        public bool Clock
        {
            get {return usbCon.get_DigitalOutLine(1, 2); }
            set { usbCon.set_DigitalOutLine(1, 2, value); }
        }

        public bool CurrencyProtection
        {
            get { return usbCon.get_DigitalOutLine(1, 3); }
            set { usbCon.set_DigitalOutLine(1, 3, value); }
        }
        #endregion

        public bool IsRunning
        {
            get { return running; }
        }

        public long speed;
        public long Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public bool Initiate() {
            if (usbCon.Open("usb-pio"))
            {
                usbCon.set_DigitalDirection(1, 0x0000);
                usbCon.set_DigitalDirection(2, 0x0000);
                usbCon.set_DigitalDirection(3, 0x0000);
                Enabled = false;
                Direction = true;
                CurrencyProtection = false;
                running = false;
                speed = 10;
                return true;
            }
            else {
                return false;
            }
        }

        public bool Dispose() {
            try
            {
                Stop();
                Enabled = true;
                Direction = true;
                CurrencyProtection = false;
                speed = 10;
                usbCon.Close();
                return true;
            }
            catch {
                return false;
            }
        }

        public void Start() {
            if (clockThread == null)
            {
                clockThread = new Thread(Tick);
                running = true;
                clockThread.Start();
            }
        }

        public void TurnAngle(int angle) {
            for (int i = 0; i < angle/0.1125*2; i++)
            {
                Clock = !Clock;

            }
        }

        public void Stop() {
            running = false;
            clockThread = null;
        }

        TimeSpan t = new TimeSpan(1);

        private void Tick()
        {
            for(int i = 0; i<6400; i++){
                Clock = !Clock;

            }
        }
        
    }
}
