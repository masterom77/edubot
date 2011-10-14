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
using HTL.Grieskirchen.Edubot.Commands;
using HTL.Grieskirchen.Edubot.API;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CommandParser parser;
        VisualisationExternal windowVisualisation;

        public MainWindow()
        {
            InitializeComponent();
            windowVisualisation = new VisualisationExternal();
            parser = new CommandParser();
            API.Edubot edubot = API.Edubot.GetInstance();
           
            edubot.OnAxisAngleChanged += ShowEventArgsInfo;
            edubot.OnStateChanged += ShowEventArgsInfo;
            edubot.OnInterpolationChanged += ShowEventArgsInfo;
            edubot.OnToolUsed += ShowEventArgsInfo;
            
        }

        private void ShowEventArgsInfo(object sender, EventArgs e) {
            Console.WriteLine("--------------------------");
            Console.WriteLine(e.GetType().Name);
            if (e is AngleChangedEventArgs) {
                AngleChangedEventArgs ace = e as AngleChangedEventArgs;
                Console.WriteLine("Ticks: " + ace.Ticks);
                Console.WriteLine("Speed: " + ace.Speed);
                Console.WriteLine("Angle: " + ace.Angle+"°");
                Console.WriteLine("AxisType: " + ace.AxisType.ToString());


                //Update Visualisation

                if (ace.AxisType == AxisType.PRIMARY) {
                    visualisation3D.MoveAnglePrimaryAxis(ace.Ticks, ace.Speed);
                    
                        windowVisualisation.visualisation3D.MoveAnglePrimaryAxis(ace.Ticks, ace.Speed);
                        windowVisualisation.visualisationAbove.MoveAnglePrimaryAxis(ace.Ticks, ace.Speed);
                    
                }
                if (ace.AxisType == AxisType.SECONDARY) {
                    visualisation3D.MoveAngleSecondaryAxis(ace.Ticks, ace.Speed);
                    
                        windowVisualisation.visualisation3D.MoveAngleSecondaryAxis(ace.Ticks, ace.Speed);
                        windowVisualisation.visualisationAbove.MoveAngleSecondaryAxis(ace.Ticks, ace.Speed);
                    
                }

            } 
            if (e is StateChangedEventArgs)
            {
                StateChangedEventArgs sce = e as StateChangedEventArgs;
                Console.WriteLine("Old State: " + sce.OldState.ToString());
                Console.WriteLine("New State: " + sce.NewState.ToString());
            } if (e is InterpolationChangedEventArgs)
            {
                InterpolationChangedEventArgs ice = e as InterpolationChangedEventArgs;
                Console.WriteLine("Old State: " + ice.OldValue != null ? ice.OldValue.ToString():"null");
                Console.WriteLine("New State: " + ice.NewValue != null ? ice.NewValue.ToString():"null");

            } if (e is ToolEventArgs)
            {
                ToolEventArgs tce = e as ToolEventArgs;
                Console.WriteLine("Activated: " + tce.Activated);
            }
            Console.WriteLine("--------------------------");
            
           
        }

        private void btExecute_Click(object sender, RoutedEventArgs e)
        {
            tbConsole.Clear();
            try
            {
                tbConsole.AppendText(">Building...\n");
                parser.Parse(tbCodeArea.Text);
                tbConsole.AppendText(">Build succeeded\n");
                tbConsole.AppendText(">Executing...");
            }
            catch (Exception ex)
            {
                tbConsole.AppendText(">Build failed\n");
                tbConsole.AppendText(">"+ex.Message + "\n");
            }
          
        }

        private void ExtVis_Click(object sender, RoutedEventArgs e)
        {
            windowVisualisation.Show();
        }

        

        
    }
}
