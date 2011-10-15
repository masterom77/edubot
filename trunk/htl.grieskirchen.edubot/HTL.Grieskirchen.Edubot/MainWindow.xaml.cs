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
using System.IO;
using Microsoft.Win32;

namespace HTL.Grieskirchen.Edubot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CommandParser parser;
        string currentFile;
        bool saved;
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
            currentFile = null;
            saved = true;
        }

        private void ShowEventArgsInfo(object sender, EventArgs e) {
            Console.WriteLine("--------------------------");
            Console.WriteLine(e.GetType().Name);
            if (e is AngleChangedEventArgs) {
                AngleChangedEventArgs ace = e as AngleChangedEventArgs;
                Console.WriteLine("Ticks: " + ace.Result.Ticks);
                Console.WriteLine("Speed: " + ace.Result.Speed);
                Console.WriteLine("Angle: " + ace.Result.Angle+"°");
                Console.WriteLine("AxisType: " + ace.AxisType.ToString());


                //Update Visualisation

                if (ace.AxisType == AxisType.PRIMARY) {
                    visualisation3D.MoveAnglePrimaryAxis(ace.Result.Ticks, ace.Result.Speed);
                    
                        windowVisualisation.visualisation3D.MoveAnglePrimaryAxis(ace.Result.Ticks, ace.Result.Speed);
                        windowVisualisation.visualisationAbove.MoveAnglePrimaryAxis(ace.Result.Ticks, ace.Result.Speed);
                    
                }
                if (ace.AxisType == AxisType.SECONDARY) {
                    visualisation3D.MoveAngleSecondaryAxis(ace.Result.Ticks, ace.Result.Speed);
                    
                        windowVisualisation.visualisation3D.MoveAngleSecondaryAxis(ace.Result.Ticks, ace.Result.Speed);
                        windowVisualisation.visualisationAbove.MoveAngleSecondaryAxis(ace.Result.Ticks, ace.Result.Speed);
                    
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

        private void SaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";

            if ((bool)dialog.ShowDialog())
            {
                Save(dialog.FileName);
                currentFile = dialog.FileName;
            }
        }

        private void Save(string fileName)
        {
            StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write));
            writer.WriteLine(tbCodeArea.Text);
            writer.Close();
            saved = true;
        }

        private void Open()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";

            if ((bool)dialog.ShowDialog())
            {
                StreamReader reader = new StreamReader(new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read));
                while (!reader.EndOfStream)
                {
                    tbCodeArea.AppendText(reader.ReadLine()+Environment.NewLine);
                }
                reader.Close();
                currentFile = dialog.FileName;
            }
        }

        private void SaveChanges() {
            if (!saved) {
                MessageBoxResult response = MessageBox.Show("Möchten Sie die Änderungen in \"" + currentFile.Substring(currentFile.LastIndexOf('\\')+1)+"\" speichern?", "Edubot", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (response == MessageBoxResult.Yes) {
                    if (currentFile == null)
                    {
                        SaveAs();
                    }
                    else {
                        Save(currentFile);
                    }
                }
                if (response == MessageBoxResult.No) {
                    saved = true;
                }
                if (response == MessageBoxResult.Cancel) {
                    saved = false;
                }
            }
        }

        private void Create()
        {
            tbCodeArea.Clear();
            currentFile = null;
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

        private void miSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
            
        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentFile == null)
            {
                SaveAs();
            }
            else {
                Save(currentFile);
            }
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            if (saved)
            {
                Open();
            }

        }

        private void miNew_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            if (saved)
            {
                Create();
            }
        }

       

        private void tbCodeArea_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            saved = false;
        }

        

        
    }
}
