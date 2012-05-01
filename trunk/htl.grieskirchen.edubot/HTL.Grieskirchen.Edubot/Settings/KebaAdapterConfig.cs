using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API;

namespace HTL.Grieskirchen.Edubot.Settings
{
    [Serializable]
    public class KebaAdapterConfig : IConfiguration
    {
        public const string NAME = "Keba";

        string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                ipAddress = value;
                NotifyPropertyChanged("IpAddress");
            }
        }

        int senderPort;

        public int SenderPort
        {
            get { return senderPort; }
            set
            {
                senderPort = value;
                NotifyPropertyChanged("SenderPort");
            }
        }

        int receiverPort;

        public int ReceiverPort
        {
            get { return receiverPort; }
            set
            {
                receiverPort = value;
                NotifyPropertyChanged("ReceiverPort");
            }
        }

        //bool autoConnect;

        //public bool AutoConnect
        //{
        //    get { return autoConnect; }
        //    set { autoConnect = value;
        //    NotifyPropertyChanged("AutoConnect");
        //    }
        //}

        //public string ConnectionState
        //{
        //    get
        //    {
        //        IAdapter adapter;
        //        string state = "Disconnected";
        //        if (API.Edubot.GetInstance().RegisteredAdapters.TryGetValue(NAME, out adapter)) {
        //            if (((EdubotAdapter)adapter).TestConnectivity())
        //            {
        //                state = "Connected";
        //            }
        //        }                
        //        return state;
        //    }
        //}

        public void Reset() {
            IpAddress = "192.168.0.101";
            ReceiverPort = 3000;
            SenderPort = 3001;
            SelectedTool = "Virtuell";
        }

        public override void Apply()
        {
            //API.Edubot edubot = API.Edubot.GetInstance();
            //edubot.DeregisterAdapter(NAME);

            //ITool realTool = null;
            //switch (selectedTool)
            //{
            //    case "Virtuell": realTool = new VirtualTool();
            //        break;
            //}
            //edubot.RegisterAdapter(NAME, new KebaAdapter(realTool, float.Parse(length), float.Parse(length2), System.Net.IPAddress.Parse(ipAddress), senderPort, receiverPort));
        }
    }
}
