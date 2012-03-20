using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API;

namespace HTL.Grieskirchen.Edubot.Settings
{
    public class Settings
    {
        public Settings() {
            defaultConfig = new DefaultAdapterConfig() { Length = "300", Length2 = "300", IpAddress = "192.168.0.100", Port = 3000, Tool = "Virtuell" };
            kebaConfig = new KebaAdapterConfig() { Length = "300", Length2 = "300", IpAddress = "192.168.0.101", ReceiverPort = 3000, SenderPort = 3001, Tool = "Virtuell" };
            visualizationConfig = new VisualizationConfig() { Length = "300", Length2 = "300", VisualizationEnabled = true, ShowGrid = false, ShowLabels = false, Speed = 50, Steps = 5, UseLongest = true, UseFollowing = false, Tool = "Virtuell" };
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
            serializer.Serialize(new System.IO.FileStream("settings.xml", System.IO.FileMode.Create), settings);            
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
    }
}
