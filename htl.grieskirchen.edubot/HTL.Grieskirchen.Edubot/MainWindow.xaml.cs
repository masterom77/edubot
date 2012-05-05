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
using System.Xml.Serialization;

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
        int executedCommands;
        //List<API.Commands.ICommand> currentCommands;
        //VisualisationExternal windowVisualisation;
        API.Edubot edubot;
        //List<IAdapter> registeredAdapters;
        //Settings.Settings settings;
        VisualizationConfig visualizationConfig;
        EdubotAdapterConfig edubotConfig;
        KebaAdapterConfig kebaConfig;
        Visualisation3D vVirtual;
        VisualisationEdubot vEdubot;

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
            InitializeVisualization();
            InitializeVariables();
            InitializeLists();
            LoadSettings();
            
            InitializeLists();
            string path = Environment.CurrentDirectory.Replace('\\', '/').Replace("C:", "file://127.0.0.1/c$");
            wbManual.Source = new Uri(path+"/doc/Edubot.html");
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, new ExecutedRoutedEventHandler(icDrawing.UndoExecuted), new CanExecuteRoutedEventHandler(icDrawing.CanUndoDelegate)));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, new ExecutedRoutedEventHandler(icDrawing.RedoExecuted), new CanExecuteRoutedEventHandler(icDrawing.CanRedoDelegate)));
            puAutocomplete.KeyDown += AppendText;

            puAutocomplete.Visibility = Visibility.Visible;
            puAutocomplete.KeyDown += AppendText;
            //edubot.RegisterAdapter(API.Adapters.AdapterType.DEFAULT);
        }

        private void InitializeVisualization() {
            vVirtual = new Visualisation3D();
            vVirtual.Width = 600;
            vVirtual.Height = 600;
            vVirtual.AnglePrimaryAxis = 0;
            vVirtual.AngleSecondaryAxis = 0;
            vVirtual.HorizontalAlignment = HorizontalAlignment.Stretch;

            vEdubot = new VisualisationEdubot();
            vEdubot.Width = 600;
            vEdubot.Height = 600;
            vEdubot.AnglePrimaryAxis = 0;
            vEdubot.AngleSecondaryAxis = 0;
            vEdubot.HorizontalAlignment = HorizontalAlignment.Stretch;

            visualisation2D.AnglePrimaryAxis = 0;
            visualisation2D.AngleSecondaryAxis = 0;
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

        public void InitializeVariables()
        {
            edubot = API.Edubot.GetInstance();
            edubot.OnFailure += ShowError;
            parser = new CommandParser();
            currentFile = null;                      
            saved = true;
        }

        public void InitializeLists() {
            List<AdapterType> allTypes = new List<AdapterType>();
            foreach (AdapterType type in (Enum.GetValues(typeof(AdapterType)).AsQueryable()))
                allTypes.Add(type);
            //cbVisualizedAdapter.ItemsSource = edubot.RegisteredAdapters.Keys;
            //cbDASelectedTool.ItemsSource = new List<string>() { "Virtuell" };
            //cbKESelectedTool.ItemsSource = new List<string>() { "Virtuell" };
            //cbVisualizedAdapter.Items.Add(AdapterType.KEBA);
        }

        public void LoadSettings()
        {
           

            visualizationConfig = new VisualizationConfig();
            edubotConfig = new EdubotAdapterConfig();
            kebaConfig = new KebaAdapterConfig();

            Cursor = Cursors.AppStarting;
            System.IO.FileStream stream = null;
            XmlSerializer serializer;
            try
            {
                stream = new System.IO.FileStream("visualization.config.xml", System.IO.FileMode.Open);
                serializer = new XmlSerializer(typeof(VisualizationConfig));
                visualizationConfig = (VisualizationConfig)serializer.Deserialize(stream);
            }
            catch (Exception)
            {
                visualizationConfig.Reset();
            }
            finally
            {
                if(stream != null)
                stream.Close();
            }
            try
            {
                stream = new System.IO.FileStream("edubot.config.xml", System.IO.FileMode.Open);
                serializer = new XmlSerializer(typeof(EdubotAdapterConfig));
                edubotConfig = (EdubotAdapterConfig)serializer.Deserialize(stream);
            }
            catch (Exception)
            {
                edubotConfig.Reset();
            }
            finally
            {
                if (stream != null)
                stream.Close();
            }
            try
            {
                stream = new System.IO.FileStream("keba.config.xml", System.IO.FileMode.Open);
                serializer = new XmlSerializer(typeof(KebaAdapterConfig));
                kebaConfig = (KebaAdapterConfig)serializer.Deserialize(stream);
            }
            catch (Exception)
            {
                kebaConfig.Reset();
            }
            finally
            {
                if (stream != null)
                stream.Close();
            }

            Cursor = Cursors.Arrow;
            ApplyConfiguration(null, new PropertyChangedEventArgs("IsEdubotModelSelected"));
            visualizationConfig.PropertyChanged += ApplyConfiguration;
            gEdubotData.DataContext = edubotConfig;
            gKebaData.DataContext = kebaConfig;
            gVisualizationData.DataContext = visualizationConfig;

            icDrawing.Configuration = visualizationConfig;
            visualisation2D.Configuration = visualizationConfig;
            vVirtual.Configuration = visualizationConfig;
            vEdubot.Configuration = visualizationConfig;
            visualisation2D.DataContext = visualizationConfig;
            vVirtual.DataContext = visualizationConfig;
            vEdubot.DataContext = visualizationConfig;

            visualizationConfig.NotifiyAllPropertiesChanged();
            //kebaConfig.Reset();
            //visualizationConfig.Reset();
        }




        private void ApplyConfiguration(object sender, PropertyChangedEventArgs args) {
            if (args.PropertyName == "IsEdubotModelSelected") {
                if (visualizationConfig.IsEdubotModelSelected)
                    vb3DVisualisation.Child = vEdubot;
                else
                    vb3DVisualisation.Child = vVirtual;
            }
        }
        //public void ActualizeEdubotSettings() {
        //    //tbDALength.Text = settings.EdubotConfig.Length.ToString();
        //    //tbDALength2.Text = settings.EdubotConfig.Length2.ToString();
        //    //tbEdubotIPAddress.Text = settings.EdubotConfig.IpAddress;
        //    //tbEdubotPort.Text = settings.EdubotConfig.Port.ToString();
        //}

        #endregion


               

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
                   tbCodeArea.Text = Open(dialog.FileName);
                }
            }
        }

        private string Open(string path) {
            StringBuilder builder = new StringBuilder();
            StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
            while (!reader.EndOfStream)
            {
                builder.Append(reader.ReadLine() + Environment.NewLine);
            }
            reader.Close();
            currentFile = path;
            int index = miRecentFiles.Items.Add(path);
            return builder.ToString();
        }

        private void SaveChanges() {
            if (!saved) {
                MessageBoxResult response = MessageBox.Show("Möchten Sie die Änderungen in \"" +(currentFile == null ? "\"Script1\"" : currentFile.Substring(currentFile.LastIndexOf('\\')+1))+"\" speichern?", "Edubot", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

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
                        List<API.Commands.ICommand> commands = CommandParser.Parse(tbCodeArea.Text);
                        tbConsole.AppendText(">Build succeeded\n");
                        tbConsole.AppendText(">Executing...\n");
                        //executedCommands = 0;
                        //tbbExecute.IsEnabled = false;
                        //tbbAbort.IsEnabled = true;
                        foreach (API.Commands.ICommand command in commands)
                        {
                            edubot.Execute(command);
                        }
                    }
                    catch (Exception ex)
                    {
                        tbConsole.AppendText(">Failure.\n");
                        tbConsole.AppendText(">" + ex.Message + "\n");
                    }
                    break;
                case 1:
                      
                    //executedCommands = 0;
                    //tbbExecute.IsEnabled = false;
                    //tbbAbort.IsEnabled = true;
                    edubot.Execute(new InitCommand());
                    foreach (API.Commands.ICommand command in icDrawing.GenerateMovementCommands())
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
            //windowVisualisation.Show();
        }

        private void tbCodeArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (puAutocomplete.IsOpen) {
                UpdatePossibleCommands();
            }
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
                    icDrawing.DrawingMode = InkCanvasDrawingMode.None;
                    break;
                case "erase":
                    icDrawing.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    icDrawing.DrawingMode = InkCanvasDrawingMode.None;
                    break;
                case "eraseShape":
                    icDrawing.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    icDrawing.DrawingMode = InkCanvasDrawingMode.None;
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
                    tbCodeArea.Text = tbCodeArea.Text.Insert(0, lbAutocomplete.SelectedItem.ToString());
                else
                    tbCodeArea.Text = tbCodeArea.Text.Replace(line, lbAutocomplete.SelectedItem.ToString());
                tbCodeArea.InvalidateVisual();
                puAutocomplete.IsOpen = false;
            }
        }

        private void UpdatePossibleCommands() {

            string lineText = tbCodeArea.GetLineText(tbCodeArea.GetLineIndexFromCharacterIndex(tbCodeArea.CaretIndex));
            string phrase = lineText.Substring(0, tbCodeArea.CaretIndex - tbCodeArea.GetCharacterIndexFromLineIndex(tbCodeArea.GetLineIndexFromCharacterIndex(tbCodeArea.CaretIndex)));
            string word = phrase;
                if(phrase.Contains(' '))
                    word = phrase.Substring(phrase.LastIndexOf(' '), phrase.Length - phrase.LastIndexOf(' '));
            word = word.Trim();
            List<string> possibleCommands = null;
            if (word != string.Empty)
            {
                possibleCommands = (from cmd in Enum.GetNames(typeof(Commands.Commands))
                                    where cmd.StartsWith(word.ToUpper())
                                    select cmd).ToList();
            }
            else
            {
                possibleCommands = Enum.GetNames(typeof(Commands.Commands)).ToList();
            }

            lbAutocomplete.Items.Clear();
            foreach (string cmd in possibleCommands)
            {
                lbAutocomplete.Items.Add(cmd);
            }
            lbAutocomplete.SelectedIndex = 0;
        }

        private void tbCodeArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { 
                puAutocomplete.IsOpen = false;
            }
            if (e.Key == Key.Space && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                UpdatePossibleCommands();
                puAutocomplete.Placement = PlacementMode.RelativePoint;
                Rect caretRect = tbCodeArea.GetRectFromCharacterIndex(tbCodeArea.CaretIndex, true);
                puAutocomplete.HorizontalOffset = caretRect.X;
                puAutocomplete.VerticalOffset = caretRect.Y+tbCodeArea.FontSize;
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





        #endregion



        delegate void AppendTextDelegate(String text);
        AppendTextDelegate appendTextDelegate;
        public void AppendText(String text) {
            tbConsole.AppendText(text+Environment.NewLine);
        }

        public void ShowError(object sender, EventArgs args) {
            FailureEventArgs fea = (FailureEventArgs)args;
            appendTextDelegate = AppendText;
            
            tbConsole.Dispatcher.BeginInvoke(appendTextDelegate, fea.ThrownException.Message);
        }

        
        private void Abort(object sender, RoutedEventArgs e)
        {
            edubot.Execute(new AbortCommand());
        }

        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            if (btConnect.Content.ToString() == "Verbinden")
            {
                if ((bool)rbEdubot.IsChecked)
                {
                    EdubotAdapter adapter = new EdubotAdapter(Tool.VIRTUAL, IPAddress.Parse(edubotConfig.IpAddress), edubotConfig.Port);
                    ConnectingScreen screen = new ConnectingScreen(adapter);
                    screen.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    screen.ShowDialog();
                    if (screen.DialogResult == false)
                    {
                        MessageBox.Show("Es konnte keine Verbindung mit der ausgewählten Steuerung hergestellt werden.", "Verbindungstest fehlgeschlagen", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                        btConnect.Content = "Trennen";
                        edubot.RegisterAdapter("Edubot", adapter);
                        //settings.EdubotConfig.Apply();
                        if (!visualizationConfig.IsEdubotModelSelected)
                            visualizationConfig.IsEdubotModelSelected = true;
                        rbVirtualModel.IsEnabled = false;
                }
                else {
                    KebaAdapter adapter = new KebaAdapter(Tool.VIRTUAL,200,200,float.MaxValue,float.MinValue,float.MaxValue,float.MinValue, IPAddress.Parse(kebaConfig.IpAddress), kebaConfig.ReceiverPort, kebaConfig.SenderPort);
                    ConnectingScreen screen = new ConnectingScreen(adapter);
                    screen.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    screen.ShowDialog();
                    if (screen.DialogResult == false)
                    {
                        MessageBox.Show("Es konnte keine Verbindung mit der ausgewählten Steuerung hergestellt werden.","Verbindungstest fehlgeschlagen", MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }
                    btConnect.Content = "Trennen";
                    edubot.RegisterAdapter("Keba", adapter);
                    //settings.EdubotConfig.Apply();
                    if (visualizationConfig.VisualizationEnabled)
                        visualizationConfig.VisualizationEnabled = false;
                    cbEnableVisualization.IsEnabled = false;
                }
                tbEdubotIPAddress.IsEnabled = false;
                tbEdubotPort.IsEnabled = false;
                rbEdubot.IsEnabled = false;
                rbKeba.IsEnabled = false;
            }
            else {
                
                btConnect.Content = "Verbinden";
                if ((bool)rbEdubot.IsChecked)
                {
                    edubot.DeregisterAdapter("Edubot");
                    rbVirtualModel.IsEnabled = true;
                }
                else
                {
                    edubot.DeregisterAdapter("Keba");
                    cbEnableVisualization.IsEnabled = true;
                    visualizationConfig.VisualizationEnabled = true;
                }
                tbEdubotIPAddress.IsEnabled = true;
                tbEdubotPort.IsEnabled = true;
                rbEdubot.IsEnabled = true;
                rbKeba.IsEnabled = true;

            }


        }

        private void btSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();  
            //settings.Save();
        }

        private void SaveSettings() {
            Cursor = Cursors.AppStarting;
            System.IO.FileStream stream = new System.IO.FileStream("visualization.config.xml", System.IO.FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(typeof(VisualizationConfig));
            serializer.Serialize(stream, visualizationConfig);
            stream.Close();
            stream = new System.IO.FileStream("edubot.config.xml", System.IO.FileMode.Create);
            serializer = new XmlSerializer(typeof(EdubotAdapterConfig));
            serializer.Serialize(stream, edubotConfig);
            stream.Close();
            stream = new System.IO.FileStream("keba.config.xml", System.IO.FileMode.Create);
            serializer = new XmlSerializer(typeof(KebaAdapterConfig));
            serializer.Serialize(stream, kebaConfig);
            stream.Close();

            Cursor = Cursors.Arrow;
        }

        private void btResetSettings_Click(object sender, RoutedEventArgs e)
        {
            edubotConfig.Reset();
            kebaConfig.Reset();
            visualizationConfig.Reset();
            SaveSettings();
        }

       

    }
}
