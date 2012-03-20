using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API;
using System.ComponentModel;

namespace HTL.Grieskirchen.Edubot.Settings
{
    public abstract class IConfiguration : INotifyPropertyChanged
    {
        protected string tool;

        public string Tool
        {
            get { return tool; }
            set { tool = value;
            NotifyPropertyChanged("Tool");
            }
        }

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

        protected void NotifyPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void ApplyTo(API.Edubot edubot);
    }
}
