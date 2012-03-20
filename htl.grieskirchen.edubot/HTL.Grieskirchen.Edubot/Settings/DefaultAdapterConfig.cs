using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API;

namespace HTL.Grieskirchen.Edubot.Settings
{
    public class DefaultAdapterConfig : IConfiguration
    {
        public const string NAME = "Default";

        string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value;
            NotifyPropertyChanged("IpAddress");
            }
        }

        int port;

        public int Port
        {
            get { return port; }
            set { port = value;
            NotifyPropertyChanged("Port");
            }
        }

        bool autoConnect;

        public bool AutoConnect
        {
            get { return autoConnect; }
            set { autoConnect = value;
            NotifyPropertyChanged("AutoConnect");
            }
        }

        public string ConnectionState
        {
            get
            {
                IAdapter adapter;
                string state = "Disconnected";
                if (API.Edubot.GetInstance().RegisteredAdapters.TryGetValue(NAME, out adapter)) {
                    if (((DefaultAdapter)adapter).TestConnectivity()) {
                        state = "Connected";
                    }
                }                
                return state;
            }
        }

        public override void ApplyTo(API.Edubot edubot)
        {
            edubot.DeregisterAdapter(NAME);
            ITool realTool = null;
            switch (tool) {
                case "Virtuell": realTool = new VirtualTool();
                    break;
            }
            edubot.RegisterAdapter(NAME, new DefaultAdapter(realTool, float.Parse(length), float.Parse(length2), System.Net.IPAddress.Parse(ipAddress),port));
        }
    }
}
