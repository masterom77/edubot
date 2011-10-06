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

namespace HTL.Grieskirchen.Edubot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CommandParser parser;

        public MainWindow()
        {
            InitializeComponent();
            parser = new CommandParser();
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
    }
}
