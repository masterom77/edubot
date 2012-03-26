using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API;
using System.ComponentModel;
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot.Settings
{
    [Serializable]
    public class Settings
    {

        public Settings() {
            defaultConfig = new DefaultAdapterConfig() { Length = "150", Length2 = "150", IpAddress = "192.168.0.100", Port = 3000, SelectedTool = "Virtuell" };
            kebaConfig = new KebaAdapterConfig() { Length = "150", Length2 = "150", IpAddress = "192.168.0.101", ReceiverPort = 3000, SenderPort = 3001, SelectedTool = "Virtuell" };
            visualizationConfig = new VisualizationConfig() { Length = "150", Length2 = "150", VisualizationEnabled = true, ShowGrid = false, ShowLabels = false, Speed = 50, Steps = 10, UseLongest = true, UseFollowing = false, SelectedTool = "Virtuell" };
        }

        DefaultAdapterConfig defaultConfig;

        public DefaultAdapterConfig DefaultConfig
        {
            get { return defaultConfig; }
            set { defaultConfig = value; }
        }

        KebaAdapterConfig kebaConfig;

        public KebaAdapterConfig KebaConfig
        {
            get { return kebaConfig; }
            set { kebaConfig = value; }
        }

        VisualizationConfig visualizationConfig;

        public VisualizationConfig VisualizationConfig
        {
            get { return visualizationConfig; }
            set { visualizationConfig = value; }
        }

        public static void Save(Settings settings) {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            System.IO.FileStream stream = new System.IO.FileStream("settings.xml", System.IO.FileMode.Create);
            serializer.Serialize(stream, settings);
            stream.Close();
        }

        public static Settings Load() {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                Settings settings = (Settings)
                serializer.Deserialize(new System.IO.FileStream("settings.xml", System.IO.FileMode.Create));
                return settings;
            }
            catch (Exception e) {
                Console.WriteLine("LOADING ERROR:" + e.Message);
            }
            return null;
        }

        public void Apply() {
            if (defaultConfig.AutoConnect)
            {
                defaultConfig.Apply();
            }
            if (kebaConfig.AutoConnect)
            {
                kebaConfig.Apply();
            }
            visualizationConfig.Apply();
        
        }
    }
}
