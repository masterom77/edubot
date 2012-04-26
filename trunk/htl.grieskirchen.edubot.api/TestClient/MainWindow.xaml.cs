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
            VirtualAdapter adapter = new VirtualAdapter(new VirtualTool(), 200, 230);
            EdubotAdapter adapter2 = new EdubotAdapter(new VirtualTool(), 200, 230, System.Net.IPAddress.Parse("192.168.0.40"), 12000);
           
            v2d.VisualisationAdapter = adapter;
            edubot.RegisterAdapter("demo", adapter);
            edubot.RegisterAdapter("demo2", adapter2);
            adapter.OnMovementStarted += NotifyVisualisation;
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
            EdubotAdapter adapter = new EdubotAdapter(new VirtualTool(), 200, 225, System.Net.IPAddress.Parse("192.168.0.40"), 12000);
            adapter.SetInterpolationResult(new InterpolationResult() { Steps = new List<InterpolationStep> { new InterpolationStep() { Alpha1 = float.Parse(tbAlpha1.Text), Alpha2 = float.Parse(tbAlpha2.Text) } } });
            adapter.Start(0.1125f);
            System.Threading.Thread.Sleep(1000);
            adapter.MoveStraightTo(new Point3D(300,0,0));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

    }
}