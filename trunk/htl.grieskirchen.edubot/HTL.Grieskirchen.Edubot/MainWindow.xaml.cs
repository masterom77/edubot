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
        int executedCommands;
        List<API.Commands.ICommand> currentCommands;
        VisualisationExternal windowVisualisation;
        API.Edubot edubot;
        //List<IAdapter> registeredAdapters;
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
            
            InitializeLists();
            string path = Environment.CurrentDirectory.Replace('\\', '/');
            wbManual.Source = new Uri(path+"/doc/Edubot.html");
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
            //edubot.OnAxisAngleChanged += ShowEventArgsInfo;
            //edubot.OnStateChanged += ShowEventArgsInfo;
            //edubot.OnInterpolationChanged += ShowEventArgsInfo;
            //edubot.OnToolUsed += ShowEventArgsInfo;
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
            tiDASettings.DataContext = settings.EdubotConfig;
            tiKESettings.DataContext = settings.KebaConfig;
            tiVisualization.DataContext = settings.VisualizationConfig;
            visualisation2D.Configuration = settings.VisualizationConfig;
            visualisation3D.Configuration = settings.VisualizationConfig;
            visualisation2D.DataContext = settings.VisualizationConfig;
            visualisation3D.DataContext = settings.VisualizationConfig;
        }

        public void ActualizeEdubotSettings() {
            tbDALength.Text = settings.EdubotConfig.Length.ToString();
            tbDALength2.Text = settings.EdubotConfig.Length2.ToString();
            tbDAIpAddress.Text = settings.EdubotConfig.IpAddress;
            tbDAPort.Text = settings.EdubotConfig.Port.ToString();
        }

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
                    Open(dialog.FileName);
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
                        currentCommands = CommandParser.Parse(tbCodeArea.Text);
                        tbConsole.AppendText(">Build succeeded\n");
                        tbConsole.AppendText(">Executing...\n");
                        executedCommands = 0;
                        tbbExecute.IsEnabled = false;
                        tbbAbort.IsEnabled = true;
                        foreach (API.Commands.ICommand command in currentCommands)
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
                    currentCommands = new List<API.Commands.ICommand>();
                    try
                    {
                        currentCommands.Add(new StartCommand());
                        currentCommands.AddRange(icDrawing.GenerateMovementCommands());
                        currentCommands.Add(new ShutdownCommand());
                    }
                    catch (Exception) {
                    };
                    executedCommands = 0;
                    tbbExecute.IsEnabled = false;
                    tbbAbort.IsEnabled = true;
                    foreach (API.Commands.ICommand command in currentCommands)
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
                    tbCodeArea.Text = tbCodeArea.Text.Insert(0, lbAutocomplete.SelectedItem.ToString());
                else
                    tbCodeArea.Text = tbCodeArea.Text.Replace(line, lbAutocomplete.SelectedItem.ToString());
                tbCodeArea.InvalidateVisual();
                puAutocomplete.IsOpen = false;
            }
        }

        private void UpdatePossibleCommands() {

            string lineText = tbCodeArea.GetLineText(tbCodeArea.GetLineIndexFromCharacterIndex(tbCodeArea.CaretIndex));
            //mvc mv|s start
            //67891234567890
            //linetext = mvc mvs start
            //phrase = mvc mv
            //word = mvs
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

        

        //private void ReplaceVisualisationAdapterWithLongest()
        //{
        //    if (edubot != null && edubot.RegisteredAdapters.Count > 0)
        //    {
        //        IAdapter longestAdapter = GetLongestAdapter();
        //        ReplaceVisualisationAdapter(new VirtualAdapter(new VirtualTool(), longestAdapter.Length, longestAdapter.Length2));
        //    }
        //}

        private IAdapter GetLongestAdapter() {
            return (from adapter in edubot.RegisteredAdapters.Values
                    where adapter.Length + adapter.Length2 == edubot.RegisteredAdapters.Values.Max(x => x.Length + x.Length2)
                    select adapter).FirstOrDefault();
        }

        

        private void rbUseLongestAdapter_Checked(object sender, RoutedEventArgs e)
        {
            //ReplaceVisualisationAdapterWithLongest();
        
        }

        //private void rbUseSpecificAdapter_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (edubot != null && edubot.RegisteredAdapters.Count > 0 && cbVisualizedAdapter.SelectedItem != null)
        //    {
        //        IAdapter selectedAdapter = null;
        //        //edubot.RegisteredAdapters.TryGetValue((AdapterType)cbVisualizedAdapter.SelectedItem, out selectedAdapter);
        //        if (selectedAdapter != null)
        //        {
        //            ReplaceVisualisationAdapter( new VirtualAdapter(new VirtualTool(), selectedAdapter.Length, selectedAdapter.Length2));
        //        }
        //    }
        //}

        private void cbVisualizedAdapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (edubot != null && edubot.RegisteredAdapters.Count > 0 && cbVisualizedAdapter.SelectedItem != null)
            {
                IAdapter selectedAdapter = null;
                //edubot.RegisteredAdapters.TryGetValue((AdapterType)cbVisualizedAdapter.SelectedItem, out selectedAdapter);
                if (selectedAdapter != null)
                {
                    settings.VisualizationConfig.Length = selectedAdapter.Length.ToString();
                    settings.VisualizationConfig.Length2 = selectedAdapter.Length2.ToString();
                    settings.VisualizationConfig.Apply();
                    //ReplaceVisualisationAdapter(new VirtualAdapter(new VirtualTool(), selectedAdapter.Length, selectedAdapter.Length2));
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
                settings.EdubotConfig.Apply();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void NotifyCommandExecuted(object sender, EventArgs e) {
            StateChangedEventArgs args = (StateChangedEventArgs) e;
            if (args.NewState == State.READY || args.NewState == State.SHUTDOWN)
            {
                executedCommands++;
            }
            if (executedCommands == currentCommands.Count)
            {
                tbbExecute.IsEnabled = true;
                tbbAbort.IsEnabled = false;
            }
        }

        private void Notify2DVisualization(object sender, EventArgs e) {
             
            
            visualisation2D.Animate(((MovementStartedEventArgs)e).Result);
            
        }

        private void Notify3DVisualization(object sender, EventArgs e)
        {
            visualisation3D.Angles = ((MovementStartedEventArgs)e).Result.Angles;
        }

        private void Abort(object sender, RoutedEventArgs e)
        {
            edubot.Execute(new AbortCommand());
        }

        private void btDAConnect_Click(object sender, RoutedEventArgs e)
        {
            settings.EdubotConfig.Apply();
        }

       

    }
}
