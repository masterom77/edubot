﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace HTL.Grieskirchen.Edubot.Controls
{
    /// <summary>
    /// Interaction logic for NetworkInputDialog.xaml
    /// </summary>
    public partial class NetworkInputDialog : Window
    {
        public static readonly DependencyProperty LengthProperty =
      DependencyProperty.Register("Length", typeof(string), typeof(NetworkInputDialog));

        public static readonly DependencyProperty IPAddressProperty =
      DependencyProperty.Register("IPAddress", typeof(string), typeof(NetworkInputDialog));

        public static readonly DependencyProperty PortProperty =
      DependencyProperty.Register("Port", typeof(string), typeof(NetworkInputDialog));

        public string IpAdress
        {
            get { return (string) GetValue(IPAddressProperty) ; }
            set
            {
                BindingExpression b;
                
                SetValue(IPAddressProperty, value);
            }
        }

        public string Port
        {
            get { return (string)GetValue(PortProperty); }
            set
            {
                SetValue(PortProperty, value);
            }
        }

        public string Length
        {
            get { return (string)GetValue(LengthProperty); }
            set
            {
                SetValue(LengthProperty, value);
            }
        }

        public NetworkInputDialog()
        {
            InitializeComponent();
            
            //DataContext = this;
            //tbPort.DataContext = this;
            tbIpAdress.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbPort.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbLength.GetBindingExpression(TextBox.TextProperty).UpdateSource();
          
            //btApply.GetBindingExpression(Button.IsEnabledProperty).UpdateSource();
        }


        private void btApply_Click(object sender, RoutedEventArgs e)
        {
                this.DialogResult = true;
                this.Close();
           
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

    }
}
