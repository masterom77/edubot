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
using System.Net.Sockets;
using System.Net;
using System.Windows.Ink;
using System.Threading;
using HTL.Grieskirchen.Edubot.API.Adapters;

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
        bool running;
        VisualisationExternal windowVisualisation;
        
        API.Edubot edubot;

        public MainWindow()
        {
            InitializeComponent();
            InitializeEdubot();
            InitializeEnvironmentVariables();
            InitializeLists();
            windowVisualisation = new VisualisationExternal();
            VirtualAdapter visualisationAdapter = new VirtualAdapter(new VirtualTool(), 150f);
            visualisationAdapter.OnMovementStarted += ShowEventArgsInfo;
            visualisation3D.VisualisationAdapter = visualisationAdapter;
            edubot.RegisterAdapter(visualisationAdapter);
            InitializeLists();
            //edubot.RegisterAdapter(API.Adapters.AdapterType.DEFAULT);
        }

        public void InitializeEdubot() {
            edubot = API.Edubot.GetInstance();
            edubot.OnAxisAngleChanged += ShowEventArgsInfo;
            edubot.OnStateChanged += ShowEventArgsInfo;
            edubot.OnInterpolationChanged += ShowEventArgsInfo;
            edubot.OnToolUsed += ShowEventArgsInfo;
        }

        public void InitializeEnvironmentVariables() {
            parser = new CommandParser();
            currentFile = null;
            saved = true;
            running = false;
        }

        public void InitializeLists() {
            lbRegisteredAdapters.ItemsSource = edubot.RegisteredAdapters.Keys;
            List<AdapterType> allTypes = new List<AdapterType>();
            foreach (AdapterType type in (Enum.GetValues(typeof(AdapterType)).AsQueryable()))
                allTypes.Add(type);
            lbAvailableAdapters.ItemsSource = allTypes.Except(edubot.RegisteredAdapters.Keys);
        }

        private void ShowEventArgsInfo(object sender, EventArgs e) {
            Console.WriteLine("--------------------------");
            Console.WriteLine(e.GetType().Name);
            if (e is MovementStartedEventArgs) {
                MovementStartedEventArgs mse = e as MovementStartedEventArgs;
                
                
                //Console.WriteLine("Ticks: " + );
                //Console.WriteLine("Speed: " + ace.Result.Speed);
                //Console.WriteLine("Angle: " + ace.Result.Angle+"°");
                //Console.WriteLine("AxisType: " + ace.AxisType.ToString());

                //AxisData d;

                //Update Visualisation

                if (true)
                {
                    visualisation3D.Angles = mse.Result.Angles;
                    //visualisationAbove.Angles = ace.Result.Steps;

                    

                    //Socket sock = new Socket(AddressFamily.InterNetwork,
                    //     SocketType.Stream,
                    //     ProtocolType.Tcp);
                    //const int Port = 12000;
                    //const string IPv4 = "192.168.0.40";

                    //IPAddress ipo = IPAddress.Parse(IPv4);
                    //IPEndPoint ipEo = new IPEndPoint(ipo, Port);
                    //sock.Connect(ipEo);
                    //sock.Send(Encoding.UTF8.GetBytes(ace.Result.ToString()));
                    //sock.Close();

                    //    windowVisualisation.visualisation3D.MoveAnglePrimaryAxis(ace.Result.Ticks, ace.Result.Speed);
                    //    windowVisualisation.visualisationAbove.MoveAnglePrimaryAxis(ace.Result.Ticks, ace.Result.Speed);

                }
                //if (ace.AxisType == AxisType.SECONDARY) {
                //    visualisation3D.MoveAngleSecondaryAxis(ace.Ticks, ace.Speed);
                    
                //    //    windowVisualisation.visualisation3D.MoveAngleSecondaryAxis(ace.Result.Ticks, ace.Result.Speed);
                //    //    windowVisualisation.visualisationAbove.MoveAngleSecondaryAxis(ace.Result.Ticks, ace.Result.Speed);
                    
                //}

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
            
            try
            {
                tbConsole.Clear();
                tbConsole.AppendText(">Building...\n");
                List<HTL.Grieskirchen.Edubot.API.Commands.ICommand> commands = CommandParser.Parse(tbCodeArea.Text);
                tbConsole.AppendText(">Build succeeded\n");
                tbConsole.AppendText(">Executing...");
                foreach (HTL.Grieskirchen.Edubot.API.Commands.ICommand command in commands) {
                    edubot.Execute(command);
                }
            }
            catch (Exception ex)
            {
                tbConsole.AppendText(">Build failed\n");
                tbConsole.AppendText(">"+ex.Message + "\n");
            }
          
        }

        private void Execute(object cmd) {
            
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btRegister_Click(object sender, RoutedEventArgs e)
        {
            AdapterType selected = (AdapterType) lbAvailableAdapters.SelectedItem;
            if (selected != null) {
                switch (selected) {
                    case AdapterType.DEFAULT: new Controls.NetworkInputDialog().ShowDialog();
                        break;
                }
            }
        }

        

        
    }
}
