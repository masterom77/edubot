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
using HTL.Grieskirchen.Edubot.API.Adapters;

namespace HTL.Grieskirchen.Edubot.Controls
{
    /// <summary>
    /// Interaction logic for ConnectingScreen.xaml
    /// </summary>
    public partial class ConnectingScreen : Window
    {

        System.Threading.Thread thread;
        public ConnectingScreen(EdubotAdapter adapter)
        {
            InitializeComponent();
            if (adapter != null) {
                thread = new System.Threading.Thread(TestEdubotConnectivity);
                thread.Start(adapter);
            }
        }

        public ConnectingScreen(KebaAdapter adapter)
        {
            InitializeComponent();
            if (adapter != null)
            {
                thread = new System.Threading.Thread(TestKebaConnectivity);
                thread.Start(adapter);
            }
        }

        public void TestEdubotConnectivity(object adapter) {
            EdubotAdapter edubotAdapter = (EdubotAdapter)adapter;
            if (edubotAdapter.TestConnectivity())
            {
                this.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    DialogResult = true;
                    this.Close();
                }));
            }
            else {
                this.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    DialogResult = false;
                    this.Close();
                }));
            }
        }

        public void TestKebaConnectivity(object adapter)
        {
            KebaAdapter kebaAdapter = (KebaAdapter)adapter;
            if (kebaAdapter.TestConnectivity())
            {
                this.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    DialogResult = true;
                    this.Close();
                }));
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    DialogResult = false;
                    this.Close();
                }));
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                thread.Abort();
            }
            catch (Exception) { 
            
            }
        }

        
    }
}
