﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API;
using System.ComponentModel;

namespace HTL.Grieskirchen.Edubot.Settings
{
    public abstract class IConfiguration : INotifyPropertyChanged
    {
        protected string selectedTool;

        public string SelectedTool
        {
            get { return selectedTool; }
            set { selectedTool = value;
            NotifyPropertyChanged("SelectedTool");
                
            }
        }

        //protected string length;

        //public string Length
        //{
        //    get { return length; }
        //    set
        //    {
        //        length = value;
        //        NotifyPropertyChanged("Length");
        //    }
        //}

        //protected string length2;

        //public string Length2
        //{
        //    get { return length2; }
        //    set
        //    {
        //        length2 = value;
        //        NotifyPropertyChanged("Length2");
        //    }
        //}

        //[NonSerialized]
        //protected Dictionary<String, ITool> availableTools;

        //public List<String> AvailableTools {
        //    get
        //    {
        //        if (availableTools == null) {
        //            availableTools = new Dictionary<string, ITool>();
        //        } 
        //        return availableTools.Keys.ToList(); }
        //}

        protected void NotifyPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
