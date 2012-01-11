using System;
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

namespace HTL.Grieskirchen.Edubot.Controls
{
    /// <summary>
    /// Interaction logic for NetworkInputDialog.xaml
    /// </summary>
    public partial class NetworkInputDialog : Window
    {
        private string ipAdress;

        public string IpAdress
        {
            get { return ipAdress; }
            set { ipAdress = value; }
        }

        private string port;

        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        public bool Valid {
            get { return !System.Windows.Controls.Validation.GetHasError(tbIpAdress) && !System.Windows.Controls.Validation.GetHasError(tbPort); }
        }

        public NetworkInputDialog()
        {
            InitializeComponent();
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
           int i = tbIpAdress.BindingGroup.BindingExpressions.Count;
                this.DialogResult = true;
                this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbIpAdress.DataContext = this;
            tbIpAdress.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbPort.DataContext = this;
            tbPort.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            btApply.DataContext = this;
            btApply.GetBindingExpression(Button.IsEnabledProperty).UpdateSource();
        }
    }
}
