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
    public class Settings : INotifyPropertyChanged
    {

        public Settings() {
            edubotConfig = new EdubotAdapterConfig() { Length = "150", Length2 = "150", IpAddress = "192.168.0.100", Port = 3000, SelectedTool = "Virtuell" };
            kebaConfig = new KebaAdapterConfig() { Length = "150", Length2 = "150", IpAddress = "192.168.0.101", ReceiverPort = 3000, SenderPort = 3001, SelectedTool = "Virtuell" };
            visualizationConfig = new VisualizationConfig() { Length = "150", Length2 = "150", VisualizationEnabled = true, ShowGrid = false, ShowLabels = false, Speed = 50, Steps = 10, SelectedTool = "Virtuell", VisualizedAdapter = VisualizationConfig.NAME2D };
            edubotConfig.PropertyChanged += Save;
            kebaConfig.PropertyChanged += Save;
            visualizationConfig.PropertyChanged += Save;
        }

        private void Save(object sender, EventArgs e) {
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
                stream = new System.IO.FileStream("settings.xml", System.IO.FileMode.Create);
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

        public void Apply() {
            if (edubotConfig.AutoConnect)
            {
                edubotConfig.Apply();
            }
            if (kebaConfig.AutoConnect)
            {
                kebaConfig.Apply();
            }
            visualizationConfig.Apply();
            Settings.Save(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
