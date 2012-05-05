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


        protected float length;

        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                SetRelations();
                NotifyPropertyChanged("Length");
            }
        }

        protected float length2;

        public float Length2
        {
            get { return length2; }
            set
            {
                length2 = value;
                SetRelations();
                NotifyPropertyChanged("Length2");
            }
        }

        protected string relationLength;

        public string RelationLength
        {
            get { return relationLength; }
            set
            {
                relationLength = value;
                SetLengths();
                NotifyPropertyChanged("Length");
            }
        }

        public void SetLengths() {
            float relation1 = float.Parse(relationLength);
            float relation2 = float.Parse(relationLength2);
            float incr = (length + length2) / (relation1 + relation2);
            length = incr * relation1;
            length2 = incr * relation2;
            NotifyPropertyChanged("Length");
            NotifyPropertyChanged("Length2");
        }

        public void SetRelations()
        {
            relationLength = "1";
            relationLength2 = (length2/length).ToString();
            NotifyPropertyChanged("RelationLength");
            NotifyPropertyChanged("RelationLength2");
        }

        protected string relationLength2;

        public string RelationLength2
        {
            get { return relationLength2; }
            set
            {
                relationLength2 = value;
                SetLengths();
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

        bool isVirtualModelSelected;

        public bool IsVirtualModelSelected
        {
            get { return isVirtualModelSelected; }
            set
            {
                isVirtualModelSelected = value;
                NotifyPropertyChanged("IsVirtualModelSelected");
            }
        }

        public float GetPrimaryScaleFactor() {
            return length / (length + length2) * 2;
        }

        public float GetSecondaryScaleFactor()
        {
            return length2 / (length + length2) * 2;
        }

        public void Reset() {
            Length = 200;
            Length2 = 230;
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
            IsVirtualModelSelected = false;
        }

        public void NotifiyAllPropertiesChanged()
        {
            NotifyPropertyChanged("VisualizationEnabled");
            NotifyPropertyChanged("HomingAnimated");
            NotifyPropertyChanged("ShowGrid");
            NotifyPropertyChanged("ShowLabels");
            //NotifyPropertyChanged("IsEdubotModelSelected");
            NotifyPropertyChanged("Length");
            NotifyPropertyChanged("Length2");
            NotifyPropertyChanged("VerticalToolRange");
            NotifyPropertyChanged("Transmission");
            NotifyPropertyChanged("RelationLength");
            NotifyPropertyChanged("RelationLength2");
            NotifyPropertyChanged("MaxPrimaryAngle");
            NotifyPropertyChanged("MinPrimaryAngle");
            NotifyPropertyChanged("MaxSecondaryAngle");
            NotifyPropertyChanged("MinSecondaryAngle");
            NotifyPropertyChanged("Speed");
            NotifyPropertyChanged("Steps");
        }
        
        
    }
}
