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
using HTL.Grieskirchen.Edubot.API.Interpolation;

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
            VirtualAdapter adapter = new VirtualAdapter(Tool.VIRTUAL, 200, 200);
            adapter.OnFailure += ShowError;
            EdubotAdapter adapter2 = new EdubotAdapter(Tool.VIRTUAL,System.Net.IPAddress.Parse("192.168.0.40"), 12000);
           
            v2d.VisualisationAdapter = adapter;
            edubot.RegisterAdapter("demo", adapter);
            //edubot.RegisterAdapter("demo2", adapter2);
            adapter.OnMovementStarted += NotifyVisualisation;
            edubot.Execute(new StartCommand());
            //edubot.Execute(new MVSCommand(new Point3D(150, 250, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(250, 250, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(250, 150, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(150, 150, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(150, 250, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));

            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, 50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(50, -50, 0)));
            //edubot.Execute(new MVSCommand(new Point3D(-50, -50, 0)));
        }

        public void ShowError(object sender, EventArgs e) {
            Console.WriteLine(((FailureEventArgs)e).ThrownException.ToString());
        }

        public void NotifyVisualisation(object sender, EventArgs e){
            v2d.Angles = ((MovementStartedEventArgs)e).Result.Angles;
            //((MovementStartedEventArgs)e).Result.ToString();
        }

        private void ExecuteStart(object sender, RoutedEventArgs e)
        {try
            {
                edubot.Execute(new StartCommand());
            }
        catch (Exception ex)
        {
            MessageBox.Show("Exception while executing:\n" + ex.ToString());
        }
        }

        private void ExecuteMVS(object sender, RoutedEventArgs e)
        {try
            {
            edubot.Execute(new MVSCommand(new Point3D(float.Parse(tbMVSX.Text),float.Parse(tbMVSY.Text),float.Parse(tbMVSZ.Text))));}
            catch (Exception ex) {
                MessageBox.Show("Exception while executing:\n"+ex.ToString());
            }
        }

        private void ExecuteMVC(object sender, RoutedEventArgs e)
        {
            try
            {
                edubot.Execute(new MVCCommand(new Point3D(float.Parse(tbMVCX.Text), float.Parse(tbMVCY.Text), float.Parse(tbMVCZ.Text)), new Point3D(float.Parse(tbMVCXHelp.Text), float.Parse(tbMVCYHelp.Text), float.Parse(tbMVCZHelp.Text))));
            }
            catch (Exception ex) {
                MessageBox.Show("Exception while executing:\n"+ex.ToString());
            }
        }

        private void ExecuteAbort(object sender, RoutedEventArgs e)
        {try
            {
            edubot.Execute(new AbortCommand());}
            catch (Exception ex) {
                MessageBox.Show("Exception while executing:\n"+ex.ToString());
            }
        }

        private void ExecuteShutdown(object sender, RoutedEventArgs e)
        {try
            {
            edubot.Execute(new ShutdownCommand());}
            catch (Exception ex) {
                MessageBox.Show("Exception while executing:\n"+ex.ToString());
            }
        }

        private void SendAngle(object sender, RoutedEventArgs e)
        {
            EdubotAdapter adapter = new EdubotAdapter(Tool.VIRTUAL, 200, 225, 135,-135,160,-160,System.Net.IPAddress.Parse("192.168.0.40"), 12000);
            adapter.InterpolationResult = new InterpolationResult() { Steps = new List<InterpolationStep> { new InterpolationStep(new Point3D(0, 0, 0), float.Parse(tbAlpha1.Text), float.Parse(tbAlpha2.Text), 0) } };
            adapter.Start(0.1125f);
            System.Threading.Thread.Sleep(1000);
            adapter.MoveStraightTo(new Point3D(300,0,0));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

    }
}
