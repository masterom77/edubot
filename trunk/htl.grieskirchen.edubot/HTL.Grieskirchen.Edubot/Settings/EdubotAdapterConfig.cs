using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API;

namespace HTL.Grieskirchen.Edubot.Settings
{
    [Serializable]
    public class EdubotAdapterConfig : IConfiguration
    {
        public const string NAME = "Default";

        public EdubotAdapterConfig() {
            availableTools = new Dictionary<string, ITool>();
            availableTools.Add("Virtuell", new VirtualTool());
        }

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

        [NonSerialized]
        string connectionState = "Nicht verbunden";
        
        public string ConnectionState
        {
            get
            {
                return connectionState;
            }
            set {
                connectionState = value;
                NotifyPropertyChanged("ConnectionState");
                NotifyPropertyChanged("FieldsEnabled");
            }
        }

        public bool FieldsEnabled {
            get {
                return connectionState == "Nicht verbunden" || connectionState == "Verbindung fehlgeschlagen";
            }
        }

        private void ActualizeConnectionState() {
            IAdapter adapter;
            ConnectionState = "Nicht verbunden";
            if (API.Edubot.GetInstance().RegisteredAdapters.TryGetValue(NAME, out adapter))
            {
                ConnectionState = "Verbindung testen...";
                if (((EdubotAdapter)adapter).TestConnectivity())
                {
                    ConnectionState = "Bereit";
                }
                else {
                    ConnectionState = "Verbindung fehlgeschlagen";
                }
            }   
        }

        public override void Apply()
        {
            API.Edubot edubot = API.Edubot.GetInstance();
            edubot.DeregisterAdapter(NAME);
            Tool realTool = Tool.VIRTUAL;
            //switch (selectedTool) {
            //    case "Virtuell": realTool = Tool.VIRTUAL;
            //        break;
            //}
            edubot.RegisterAdapter(NAME, new EdubotAdapter(realTool,System.Net.IPAddress.Parse(ipAddress), port));
            new System.Threading.Thread(ActualizeConnectionState).Start();
        }
    }
}
