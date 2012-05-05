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
            edubotConfig = new EdubotAdapterConfig();
            kebaConfig = new KebaAdapterConfig();
            visualizationConfig = new VisualizationConfig();
        }

        public void Save() {
            Settings.Save(this);
        }

        public void Reset()
        {
            edubotConfig.Reset();
            kebaConfig.Reset();
            visualizationConfig.Reset();
            Settings.Save(this);
        }

        EdubotAdapterConfig edubotConfig;

        public EdubotAdapterConfig EdubotConfig
        {
            get { return edubotConfig; }
            set { edubotConfig = value; }
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
            System.IO.FileStream stream = null;
            Settings settings = null;
            try
            {
                stream = new System.IO.FileStream("settings.xml", System.IO.FileMode.Open);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                settings = (Settings)serializer.Deserialize(stream);
            }
            catch (Exception e)
            {
                Console.WriteLine("LOADING ERROR:" + e.Message);
            }
            finally {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return settings;
        }


    }
}
