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


        protected string length;

        public string Length
        {
            get { return length; }
            set
            {
                length = value;
                NotifyPropertyChanged("Length");
            }
        }

        protected string length2;

        public string Length2
        {
            get { return length2; }
            set
            {
                length2 = value;
                NotifyPropertyChanged("Length2");
            }
        }

        private string verticalToolRange;

        public string VerticalToolRange
        {
            get { return verticalToolRange; }
            set
            {
                verticalToolRange = value;
                NotifyPropertyChanged("VerticalToolRange");
            }
        }

        private string transmission;

        public string Transmission
        {
            get { return transmission; }
            set { transmission = value;
            NotifyPropertyChanged("Transmission");
            }
        } 

        private string maxPrimaryAngle;

        public string MaxPrimaryAngle
        {
            get { return maxPrimaryAngle; }
            set
            {
                maxPrimaryAngle = value;
                NotifyPropertyChanged("MaxPrimaryAngle");
            }
        }

        private string minPrimaryAngle;

        public string MinPrimaryAngle
        {
            get { return minPrimaryAngle; }
            set
            {
                minPrimaryAngle = value;
                NotifyPropertyChanged("MinPrimaryAngle");
            }
        }

        private string maxSecondaryAngle;

        public string MaxSecondaryAngle
        {
            get { return maxSecondaryAngle; }
            set
            {
                maxSecondaryAngle = value;
                NotifyPropertyChanged("MaxSecondaryAngle");
            }
        }

        private string minSecondaryAngle;

        public string MinSecondaryAngle
        {
            get { return minSecondaryAngle; }
            set
            {
                minSecondaryAngle = value;
                NotifyPropertyChanged("MinSecondaryAngle");
            }
        }


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

        bool animateHoming;

        public bool AnimateHoming
        {
            get { return animateHoming; }
            set
            {
                animateHoming = value;
                NotifyPropertyChanged("AnimateHoming");
            }
        }

        bool isEdubotModelSelected;

        public bool IsEdubotModelSelected
        {
            get { return isEdubotModelSelected; }
            set { isEdubotModelSelected = value;
                NotifyPropertyChanged("IsEdubotModelSelected");
            }
        }

        public void Reset() {
            Length = "300";
            Length2 = "300";
            VerticalToolRange = "50";
            MaxPrimaryAngle = float.MaxValue.ToString();
            MinPrimaryAngle = float.MinValue.ToString();
            MaxSecondaryAngle = float.MaxValue.ToString();
            minSecondaryAngle = float.MinValue.ToString();
            VisualizationEnabled = true;
            AnimateHoming = false;
            ShowGrid = false;
            ShowLabels = false;
            Speed = 50;
            Steps = 10;
            SelectedTool = "Virtuell";
            IsEdubotModelSelected = true;
        }
        //[NonSerialized]
        //VirtualAdapter visualizationAdapter2D;

        //public VirtualAdapter VisualizationAdapter2D
        //{
        //    get { return visualizationAdapter2D; }
        //    set
        //    {
        //        visualizationAdapter2D = value;
        //        NotifyPropertyChanged("VisualizationAdapter2D");
        //    }
        //}

        //[NonSerialized]
        //VirtualAdapter visualizationAdapter3D;

        //public VirtualAdapter VisualizationAdapter3D
        //{
        //    get { return visualizationAdapter3D; }
        //    set
        //    {
        //        visualizationAdapter3D = value;
        //        NotifyPropertyChanged("VisualizationAdapter3D");
        //    }
        //}

        public override void Apply()
        {
            NotifyPropertyChanged("VisualizationEnabled");
        }
        //string visualizedAdapter;

        //public string VisualizedAdapter
        //{
        //    get { return visualizedAdapter; }
        //    set
        //    {
        //        visualizedAdapter = value;
        //        IAdapter adapter = null;
        //        API.Edubot edubot = API.Edubot.GetInstance();
        //        if (edubot.RegisteredAdapters.TryGetValue(value, out adapter))
        //        {
        //            length = adapter.Length.ToString();
        //            length2 = adapter.Length2.ToString();
        //            if (adapter.EquippedTool == Tool.VIRTUAL)
        //            {
        //                selectedTool = "Virtuell";
        //            }
        //        }
        //        else
        //        {
        //            edubot.RegisteredAdapters.TryGetValue(NAME2D, out adapter);
        //        }
        //        Apply();
        //    }
        //}

        //public List<string> VisualizableAdapters {
        //    get
        //    {
        //        edubot = Edubot.API.Edubot.GetInstance(); 
        //        return edubot.RegisteredAdapters.Keys.ToList(); }
        //}


        //private string GetLongestAdapter()
        //{
        //    Edubot.API.Edubot edubot = Edubot.API.Edubot.GetInstance();
        //    float maxLength = edubot.RegisteredAdapters.Values.Max(x => x.Length + x.Length2);
        //    foreach (KeyValuePair<string, IAdapter> entry in edubot.RegisteredAdapters)
        //    {
        //        if (entry.Value.Length + entry.Value.Length2 == maxLength)
        //        {
        //            return entry.Key;
        //        }
        //    }
        //    return null;

        //}

        //public override void Apply()
        //{
        //    if (visualizationEnabled)
        //    {
        //        API.Edubot edubot = API.Edubot.GetInstance();
               
        //        edubot.DeregisterAdapter(NAME2D);
        //        edubot.DeregisterAdapter(NAME3D);

        //        visualizationAdapter2D = new VirtualAdapter(Tool.VIRTUAL, float.Parse(length), float.Parse(length2));
        //        edubot.RegisterAdapter(NAME2D, visualizationAdapter2D);

        //        visualizationAdapter3D = new VirtualAdapter(Tool.VIRTUAL, float.Parse(length), float.Parse(length2));
        //        edubot.RegisterAdapter(NAME3D, new VirtualAdapter(Tool.VIRTUAL, float.Parse(length), float.Parse(length2)));
               
        //        //NotifyPropertyChanged("VisualizedAdapter");
        //    }
        //}

        //public VirtualAdapter GetVisualizedAdapter() {
        //    IAdapter adapter = null;
        //    API.Edubot edubot = API.Edubot.GetInstance();
        //    if (edubot.RegisteredAdapters.TryGetValue(VisualizedAdapter, out adapter))
        //        return new VirtualAdapter(new VirtualTool(), adapter.Length, adapter.Length2);
        //    else
        //        return null;
        //}

    }
}
