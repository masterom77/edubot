﻿using System;
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
        public const string NAME = "Edubot";

        public EdubotAdapterConfig() {
            //availableTools = new Dictionary<string, ITool>();
            //availableTools.Add("Virtuell", new VirtualTool());
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

        public void Reset() {
            IpAddress = "192.168.0.40"; 
            Port = 12000; 
            SelectedTool = "Virtuell";
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
            //new System.Threading.Thread(ActualizeConnectionState).Start();
        }
    }
}
