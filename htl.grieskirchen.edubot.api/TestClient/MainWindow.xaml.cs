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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HTL.Grieskirchen.Edubot.API;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.EventArgs;
using HTL.Grieskirchen.Edubot.API.Commands;

namespace TestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Edubot edubot;

        public MainWindow()
        {
            InitializeComponent();
            edubot = Edubot.GetInstance();
            VirtualAdapter adapter = new VirtualAdapter(new VirtualTool(), 150, 150);
            v2d.VisualisationAdapter = adapter;
            edubot.RegisterAdapter("demo", adapter);
            adapter.OnMovementStarted += NotifyVisualisation;
        }

        public void NotifyVisualisation(object sender, EventArgs e){
            v2d.Angles = ((MovementStartedEventArgs)e).Result.Angles;
            ((MovementStartedEventArgs)e).Result.ToString();
        }

        private void ExecuteStart(object sender, RoutedEventArgs e)
        {
            edubot.Execute(new StartCommand());
        }

        private void ExecuteMVS(object sender, RoutedEventArgs e)
        {
            edubot.Execute(new MVSCommand(new Point3D(float.Parse(tbMVSX.Text),float.Parse(tbMVSY.Text),float.Parse(tbMVSZ.Text))));
        }

        private void ExecuteMVC(object sender, RoutedEventArgs e)
        {
            edubot.Execute(new MVCCommand(new Point3D(float.Parse(tbMVCX.Text), float.Parse(tbMVCY.Text), float.Parse(tbMVCZ.Text)), new Point3D(float.Parse(tbMVCXHelp.Text), float.Parse(tbMVCYHelp.Text), float.Parse(tbMVCZHelp.Text))));
        }

        private void ExecuteAbort(object sender, RoutedEventArgs e)
        {
            edubot.Execute(new AbortCommand());
        }

        private void ExecuteShutdown(object sender, RoutedEventArgs e)
        {
            edubot.Execute(new ShutdownCommand());
        }

    }
}
