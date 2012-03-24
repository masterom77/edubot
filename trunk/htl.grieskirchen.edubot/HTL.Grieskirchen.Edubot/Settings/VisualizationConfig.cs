using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using System.ComponentModel;
using HTL.Grieskirchen.Edubot.API;

namespace HTL.Grieskirchen.Edubot.Settings
{
    public class VisualizationConfig : IConfiguration, INotifyPropertyChanged
    {
        public const string NAME2D = "2D";
        public const string NAME3D = "3D";

        public VisualizationConfig() {
            steps = 10;
            speed = 50;
            edubot = API.Edubot.GetInstance();
            length = "300";
            length2 = "300";
            selectedTool = "Virtuell";
            useLongest = true;
            useFollowing = false;
            visualizationEnabled = true;
            availableTools = new Dictionary<string, ITool>();
            availableTools.Add("Virtuell", new VirtualTool());
            Apply();
        }

        API.Edubot edubot;

        int steps;

        public int Steps
        {
            get { return steps; }
            set { steps = value;
            NotifyPropertyChanged("Steps");
            }
        }

        int speed;

        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                NotifyPropertyChanged("Speed");
            }
        }

        bool visualizationEnabled;

        public bool VisualizationEnabled
        {
            get { return visualizationEnabled; }
            set
            {
                visualizationEnabled = value;
                NotifyPropertyChanged("VisualizationEnabled");
            }
        }

        bool showGrid;

        public bool ShowGrid
        {
            get { return showGrid; }
            set { showGrid = value;
            NotifyPropertyChanged("ShowGrid");
            }
        }

        bool showLabels;

        public bool ShowLabels
        {
            get { return showLabels; }
            set
            {
                showLabels = value;
                NotifyPropertyChanged("ShowLabels");
            }
        }

        bool useLongest;

        public bool UseLongest
        {
            get { return useLongest; }
            set
            {
                useLongest = value;
                NotifyPropertyChanged("UseLongest");
                if(useLongest)
                VisualizedAdapter = GetLongestAdapter();
            }
        }

        bool useFollowing;

        public bool UseFollowing
        {
            get { return useFollowing; }
            set
            {
                useFollowing = value;
                NotifyPropertyChanged("UseFollowing");
                if (useFollowing)
                {
                    NotifyPropertyChanged("VisualizableAdapters");
                    VisualizedAdapter = visualizedAdapter;
                }
            }
        }

        string visualizedAdapter;

        public string VisualizedAdapter
        {
            get { return visualizedAdapter; }
            set
            {
                visualizedAdapter = value;
                IAdapter adapter = null;
                API.Edubot edubot = API.Edubot.GetInstance();
                if (edubot.RegisteredAdapters.TryGetValue(VisualizedAdapter, out adapter))
                {
                    length = adapter.Length.ToString();
                    length2 = adapter.Length2.ToString();
                    if (adapter.Tool is VirtualTool) {
                        selectedTool = "Virtuell";
                    }
                }
                else
                {
                    throw new Exception("No adapter found");
                }
                NotifyPropertyChanged("VisualizedAdapter");
                Apply();
            }
        }

        public List<string> VisualizableAdapters {
            get { return edubot.RegisteredAdapters.Keys.ToList(); }
        }


        private string GetLongestAdapter()
        {
            Edubot.API.Edubot edubot = Edubot.API.Edubot.GetInstance();
            float maxLength = edubot.RegisteredAdapters.Values.Max(x => x.Length + x.Length2);
            foreach (KeyValuePair<string, IAdapter> entry in edubot.RegisteredAdapters)
            {
                if (entry.Value.Length + entry.Value.Length2 == maxLength)
                {
                    return entry.Key;
                }
            }
            return null;

        }

        public override void Apply()
        {
            API.Edubot edubot = API.Edubot.GetInstance();
            edubot.DeregisterAdapter(NAME2D);
            edubot.DeregisterAdapter(NAME3D);
            ITool realTool = null;
            switch (selectedTool)
            {
                case "Virtuell": realTool = new VirtualTool();
                    break;
            }

            edubot.RegisterAdapter(NAME2D, new VirtualAdapter(realTool, float.Parse(length), float.Parse(length2)));
            edubot.RegisterAdapter(NAME3D, new VirtualAdapter(realTool, float.Parse(length), float.Parse(length2)));
        }

        public VirtualAdapter GetVisualizedAdapter() {
            IAdapter adapter = null;
            API.Edubot edubot = API.Edubot.GetInstance();
            if (edubot.RegisteredAdapters.TryGetValue(VisualizedAdapter, out adapter))
                return new VirtualAdapter(new VirtualTool(), adapter.Length, adapter.Length2);
            else
                return null;
        }

    }
}
