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
using System.Windows.Controls.Primitives;
using HTL.Grieskirchen.Edubot.API.Commands;
using HTL.Grieskirchen.Edubot.Controls;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.Settings;
using System.ComponentModel;

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
        //List<IAdapter> registeredAdapters;
        List<string> commands;
        Settings.Settings settings;


        static MainWindow()
        {
            saveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            openCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            undoCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            redoCommand.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
            executeCommand.InputGestures.Add(new KeyGesture(Key.F5, ModifierKeys.None));
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeEdubot();
            InitializeEnvironmentVariables();
            InitializeLists();
            LoadSettings();
            //Kinematics.DisplayResults = false;
            //windowVisualisation = new VisualisationExternal();
            //VirtualAdapter visualisationAdapter = new VirtualAdapter(new VirtualTool(), 150f, 150f);
            //visualisationAdapter.OnMovementStarted += ShowEventArgsInfo;
            //visualisation3D.VisualisationAdapter = visualisationAdapter;
            //visualisation2D.VisualisationAdapter = visualisationAdapter;
            //edubot.RegisterAdapter("2de", visualisationAdapter);
            //IAdapter adapter;
            //edubot.RegisteredAdapters.TryGetValue(VisualizationConfig.NAME2D, out adapter);
            //adapter.OnMovementStarted += ShowEventArgsInfo;
            //visualisation2D.VisualisationAdapter = (VirtualAdapter)adapter;
            //edubot.RegisteredAdapters.TryGetValue(VisualizationConfig.NAME3D, out adapter);
            //adapter.OnMovementStarted += ShowEventArgsInfo;
            //visualisation3D.VisualisationAdapter = (VirtualAdapter)adapter;
            //edubot.RegisterAdapter(new DefaultAdapter(new VirtualTool(), 250, 150, IPAddress.Parse("127.0.0.1"), 12000));
            InitializeLists();
//ReplaceVisualisationAdapterWithLongest();
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, new ExecutedRoutedEventHandler(icDrawing.UndoExecuted), new CanExecuteRoutedEventHandler(icDrawing.CanUndoDelegate)));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, new ExecutedRoutedEventHandler(icDrawing.RedoExecuted), new CanExecuteRoutedEventHandler(icDrawing.CanRedoDelegate)));
            puAutocomplete.KeyDown += AppendText;

            puAutocomplete.Visibility = Visibility.Visible;
            puAutocomplete.KeyDown += AppendText;
            //edubot.RegisterAdapter(API.Adapters.AdapterType.DEFAULT);
        }


        #region -----------------------------Static Properties-----------------------------

        private static RoutedCommand saveCommand = new RoutedCommand();

        public static RoutedCommand SaveCommand
        {
            get { return MainWindow.saveCommand; }
            set { MainWindow.saveCommand = value;}
        }

        private static RoutedCommand openCommand = new RoutedCommand();

        public static RoutedCommand OpenCommand
        {
            get { return MainWindow.openCommand; }
            set { MainWindow.openCommand = value; }
        }

        private static RoutedCommand undoCommand = new RoutedCommand();

        public static RoutedCommand UndoCommand
        {
            get { return MainWindow.undoCommand; }
            set { MainWindow.undoCommand = value; }
        }

        private static RoutedCommand redoCommand = new RoutedCommand();

        public static RoutedCommand RedoCommand
        {
            get { return MainWindow.redoCommand; }
            set { MainWindow.redoCommand = value; }
        }

        private static RoutedCommand executeCommand = new RoutedCommand();

        public static RoutedCommand ExecuteCommand
        {
            get { return MainWindow.executeCommand; }
            set { MainWindow.executeCommand = value; }
        }

        #endregion

        #region -----------------------------Initialization-----------------------------

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
            //lbRegisteredAdapters.ItemsSource = edubot.RegisteredAdapters.Keys;
            List<AdapterType> allTypes = new List<AdapterType>();
            foreach (AdapterType type in (Enum.GetValues(typeof(AdapterType)).AsQueryable()))
                allTypes.Add(type);
            //lbAvailableAdapters.ItemsSource = allTypes.Except(edubot.RegisteredAdapters.Keys);
            cbVisualizedAdapter.ItemsSource = edubot.RegisteredAdapters.Keys;
            cbDASelectedTool.ItemsSource = new List<string>() { "Virtuell" };
            cbKESelectedTool.ItemsSource = new List<string>() { "Virtuell" };
            //cbVisualizedAdapter.Items.Add(AdapterType.KEBA);
        }

        public void LoadSettings()
        {
            this.settings = Settings.Settings.Load();
            if (settings == null)
                settings = new Settings.Settings();
            settings.VisualizationConfig.PropertyChanged += ReplaceVisualisationAdapters;
            settings.Apply();
            //if(settings.DefaultConfig.AutoConnect){
            //    settings.DefaultConfig.Apply();
            //}
            //if (settings.KebaConfig.AutoConnect) {
            //    settings.KebaConfig.Apply();
            //}
            //if (settings.VisualizationConfig.VisualizationEnabled) {
            //    settings.VisualizationConfig.Apply();
            //}
            tiDASettings.DataContext = settings.DefaultConfig;
            tiKESettings.DataContext = settings.KebaConfig;
            tiVisualization.DataContext = settings.VisualizationConfig;
            visualisation2D.Configuration = settings.VisualizationConfig;
            visualisation3D.Configuration = settings.VisualizationConfig;
            visualisation2D.DataContext = settings.VisualizationConfig;
            visualisation3D.DataContext = settings.VisualizationConfig;
            //tbDALength.SetBinding(TextBox.TextProperty, "DefaultConfig.Length");
            //tiDASettings.DataContext = settings.DefaultConfig;
            //ActualizeDefaultSettings();
        }

       


        public void ActualizeDefaultSettings() {
            tbDALength.Text = settings.DefaultConfig.Length.ToString();
            tbDALength2.Text = settings.DefaultConfig.Length2.ToString();
            tbDAIpAddress.Text = settings.DefaultConfig.IpAddress;
            tbDAPort.Text = settings.DefaultConfig.Port.ToString();
        }

        #endregion

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
                if (sender is DefaultAdapter) {
                    Console.WriteLine("DEFAULT!");
                }
                if (sender is VirtualAdapter)
                {
                    //visualisation3D.Angles = mse.Result.Angles;
                    visualisation2D.Angles = mse.Result.Angles;
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

        #region -----------------------------Toolbar Commands-----------------------------

        public bool UndoPossible
        {
            get
            {
                switch (tcNavigation.SelectedIndex)
                {
                    case 0: return tbCodeArea.CanUndo;
                    case 1: return icDrawing.CanUndo;
                    default: return false;
                }
            }
        }

        public bool RedoPossible
        {
            get
            {
                switch (tcNavigation.SelectedIndex)
                {
                    case 0: return tbCodeArea.CanRedo;
                    case 1: return icDrawing.CanRedo;
                    default: return false;
                }
            }
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            if (saved)
            {
                tbCodeArea.Clear();
                currentFile = null;
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (currentFile == null)
            {
                SaveAs(sender,e);
            }
            else
            {
                Save(currentFile);
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
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

        private void Open(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            if (saved)
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
                        tbCodeArea.AppendText(reader.ReadLine() + Environment.NewLine);
                    }
                    reader.Close();
                    currentFile = dialog.FileName;
                }
            }
        }

        private void SaveChanges() {
            if (!saved) {
                MessageBoxResult response = MessageBox.Show("Möchten Sie die Änderungen in \"" + currentFile.Substring(currentFile.LastIndexOf('\\')+1)+"\" speichern?", "Edubot", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (response == MessageBoxResult.Yes) {
                    if (currentFile == null)
                    {
                        SaveAs(null,null);
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

        private void Execute(object sender, RoutedEventArgs e)
        {
            switch (tcNavigation.SelectedIndex)
            {
                case 0:
                    try
                    {
                        tbConsole.Clear();
                        tbConsole.AppendText(">Building...\n");
                        List<HTL.Grieskirchen.Edubot.API.Commands.ICommand> commands = CommandParser.Parse(tbCodeArea.Text);
                        tbConsole.AppendText(">Build succeeded\n");
                        tbConsole.AppendText(">Executing...\n");
                        foreach (HTL.Grieskirchen.Edubot.API.Commands.ICommand command in commands)
                        {
                            edubot.Execute(command);
                        }
                    }
                    catch (Exception ex)
                    {
                        tbConsole.AppendText(">Build failed\n");
                        tbConsole.AppendText(">" + ex.Message + "\n");
                    }
                    break;
                case 1:
                    try
                    {
                        edubot.Execute(new StartCommand());
                    }
                    catch (Exception) { };
                    foreach (MVSCommand command in icDrawing.GenerateMovementCommands())
                    {
                        edubot.Execute(command);
                    }
                    edubot.Execute(new ShutdownCommand());
                    break;
            }

        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            switch (tcNavigation.SelectedIndex)
            {
                case 0:
                    tbCodeArea.Undo();
                    break;
                case 1:
                    icDrawing.Undo();
                    break;
            }
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            switch (tcNavigation.SelectedIndex)
            {
                case 0:
                    tbCodeArea.Redo();
                    break;
                case 1:
                    icDrawing.Redo();
                    break;
            }
        }

        #endregion

        private void ExtVis_Click(object sender, RoutedEventArgs e)
        {
            windowVisualisation.Show();
        }

        private void tbCodeArea_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            saved = false;
        }

        private void ChangeTool(object sender, RoutedEventArgs e) {
            string operation = ((RadioButton)sender).Tag.ToString();
            switch (operation) { 
                case "select":
                    icDrawing.EditingMode = InkCanvasEditingMode.Select;
                    break;
                case "draw":
                    icDrawing.EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case "erase":
                    icDrawing.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;
                case "eraseShape":
                    icDrawing.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;
                case "line":
                    icDrawing.EditingMode = InkCanvasEditingMode.None;
                    icDrawing.DrawingMode = InkCanvasDrawingMode.Line;
                    break;
                case "rect":
                    icDrawing.EditingMode = InkCanvasEditingMode.None;
                    icDrawing.DrawingMode = InkCanvasDrawingMode.Rectangle;
                    break;
                case "ellipse":
                    icDrawing.EditingMode = InkCanvasEditingMode.None;
                    icDrawing.DrawingMode = InkCanvasDrawingMode.Ellipse;
                    break;
            }
        }

        #region -----------------------------Autocomplete-----------------------------

        private void ClosePopup(object sender, EventArgs e) {
            puAutocomplete.IsOpen = false;
            tbCodeArea.Focus();
        }

        private void AppendText(object sender, KeyEventArgs e) {
            if (puAutocomplete.IsOpen && e.Key == Key.Enter)
            {
                tbCodeArea.Focus();
                string line = tbCodeArea.GetLineText(tbCodeArea.GetLineIndexFromCharacterIndex(tbCodeArea.CaretIndex));
                if (line == string.Empty)
                    tbCodeArea.Text.Insert(0, lbAutocomplete.SelectedItem.ToString());
                else
                    tbCodeArea.Text.Replace(line, lbAutocomplete.SelectedItem.ToString());
                tbCodeArea.InvalidateVisual();
                puAutocomplete.IsOpen = false;
            }
        }

        private void tbCodeArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
               string currentLine = tbCodeArea.GetLineText(tbCodeArea.GetLineIndexFromCharacterIndex(tbCodeArea.CaretIndex));
               currentLine = currentLine.Trim();
               List<string> possibleCommands = null;
               if (currentLine != string.Empty)
               {
                   possibleCommands = (from cmd in commands
                                          where cmd.StartsWith(currentLine)
                                          select cmd).ToList();
               }
               else {
                   possibleCommands = commands.ToList();
               }

               lbAutocomplete.Items.Clear();
               foreach (string cmd in possibleCommands) {
                   lbAutocomplete.Items.Add(cmd);
               }
               lbAutocomplete.SelectedIndex = 0;
               puAutocomplete.IsOpen = true;
               //puAutocomplete.Focus();
               e.Handled = true;
            }
            //if (e.Key == Key.Down) {
            //    lbAutocomplete.Items.MoveCurrentToNext();
            //}

            //if (e.Key == Key.Up)
            //{
            //    lbAutocomplete.Items.MoveCurrentToPrevious();
            //}
            

        }

        private void tbCodeArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (puAutocomplete.IsOpen && e.Key == Key.Down && lbAutocomplete.Items.Count > 0
   && !(e.OriginalSource is ListBoxItem))
            {
                lbAutocomplete.Focus();
                lbAutocomplete.SelectedIndex = 0;
                ListBoxItem lbi = lbAutocomplete.ItemContainerGenerator.ContainerFromIndex(lbAutocomplete.SelectedIndex) as ListBoxItem;
                lbi.Focus();
                e.Handled = true;
            }
        }

        #endregion

        #region -----------------------------Settings Tab-----------------------------

        #region -----------------------------Default Adapter-----------------------------

        #region ---------------Properties---------------

        

        #endregion
        private void btRegister_Click(object sender, RoutedEventArgs e)
        {

            //if (lbAvailableAdapters.SelectedItem != null)
            //{
            //    AdapterType selected = (AdapterType)lbAvailableAdapters.SelectedItem;
            //    switch (selected)
            //    {
            //        case AdapterType.DEFAULT:
            //            Controls.NetworkInputDialog dialog = new Controls.NetworkInputDialog();
            //            dialog.ShowDialog();
            //            if (dialog.DialogResult == true)
            //                //edubot.RegisterAdapter(new DefaultAdapter(new VirtualTool(), 155f, 155f, IPAddress.Parse(dialog.IpAdress), int.Parse(dialog.Port)));
            //                InitializeLists();
            //            break;
            //    }
            //}
        }
        #endregion

        #region -----------------------------Visualization-----------------------------

        private void ReplaceVisualisationAdapter(VirtualAdapter newAdapter)
        {
            //visualisation2D.VisualisationAdapter = newAdapter;
            //visualisation3D.VisualisationAdapter = newAdapter;
        }

        private void ReplaceVisualisationAdapterWithLongest()
        {
            if (edubot != null && edubot.RegisteredAdapters.Count > 0)
            {
                IAdapter longestAdapter = GetLongestAdapter();
                ReplaceVisualisationAdapter(new VirtualAdapter(new VirtualTool(), longestAdapter.Length, longestAdapter.Length2));
            }
        }

        private IAdapter GetLongestAdapter() {
            return (from adapter in edubot.RegisteredAdapters.Values
                    where adapter.Length + adapter.Length2 == edubot.RegisteredAdapters.Values.Max(x => x.Length + x.Length2)
                    select adapter).FirstOrDefault();
        }

        

        private void rbUseLongestAdapter_Checked(object sender, RoutedEventArgs e)
        {
            //ReplaceVisualisationAdapterWithLongest();
        
        }

        private void rbUseSpecificAdapter_Checked(object sender, RoutedEventArgs e)
        {
            if (edubot != null && edubot.RegisteredAdapters.Count > 0 && cbVisualizedAdapter.SelectedItem != null)
            {
                IAdapter selectedAdapter = null;
                //edubot.RegisteredAdapters.TryGetValue((AdapterType)cbVisualizedAdapter.SelectedItem, out selectedAdapter);
                if (selectedAdapter != null)
                {
                    ReplaceVisualisationAdapter( new VirtualAdapter(new VirtualTool(), selectedAdapter.Length, selectedAdapter.Length2));
                }
            }
        }

        private void cbVisualizedAdapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (edubot != null && edubot.RegisteredAdapters.Count > 0 && cbVisualizedAdapter.SelectedItem != null)
            {
                IAdapter selectedAdapter = null;
                //edubot.RegisteredAdapters.TryGetValue((AdapterType)cbVisualizedAdapter.SelectedItem, out selectedAdapter);
                if (selectedAdapter != null)
                {
                    ReplaceVisualisationAdapter(new VirtualAdapter(new VirtualTool(), selectedAdapter.Length, selectedAdapter.Length2));
                }
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //AdapterSetup setup = new AdapterSetup();
            //setup.ShowDialog();
        }

        #endregion

        private void btDASaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                settings.DefaultConfig.Apply();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            Settings.Settings.Save(settings);
        }

        private void ReplaceVisualisationAdapters(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "VisualizedAdapter") {
                IAdapter adapter;
                edubot.RegisteredAdapters.TryGetValue(VisualizationConfig.NAME2D, out adapter);
                adapter.OnMovementStarted += Notify2DVisualization;
                visualisation2D.VisualisationAdapter = (VirtualAdapter)adapter;
                IAdapter adapter2;
                edubot.RegisteredAdapters.TryGetValue(VisualizationConfig.NAME3D, out adapter2);
                adapter2.OnMovementStarted += Notify3DVisualization;
                visualisation3D.VisualisationAdapter = (VirtualAdapter)adapter2;
            }
        }

        private void Notify2DVisualization(object sender, EventArgs e) {
             
            visualisation2D.Angles = ((MovementStartedEventArgs)e).Result.Angles;
        }

        private void Notify3DVisualization(object sender, EventArgs e)
        {
            visualisation3D.Angles = ((MovementStartedEventArgs)e).Result.Angles;
        }

        private void Abort(object sender, RoutedEventArgs e)
        {
            edubot.Execute(new AbortCommand());
        }
    }
}
