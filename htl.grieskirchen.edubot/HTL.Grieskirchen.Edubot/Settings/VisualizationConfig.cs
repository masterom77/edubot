using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters;
using System.ComponentModel;

namespace HTL.Grieskirchen.Edubot.Settings
{
    public class VisualizationConfig : IConfiguration
    {
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
            }
        }

        IAdapter visualizedAdapter;

        public IAdapter VisualizedAdapter
        {
            get { return visualizedAdapter; }
            set
            {
                visualizedAdapter = value;
                NotifyPropertyChanged("VisualizedAdapter");
            }
        }

        public override void ApplyTo(API.Edubot edubot)
        {
            
        }
    }
}
