using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using HTL.Grieskirchen.Edubot.API;
using HTL.Grieskirchen.Edubot.API.EventArgs;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Commands;

namespace EduBot
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            Edubot edubot = Edubot.GetInstance();
            edubot.RegisterAdapter(new VirtualAdapter(new VirtualTool(), 150f));
            List<ICommand> commands = new List<ICommand>();
            commands.Add(new StartCommand());
            commands.Add(new MoveCommand(new Point3D(100, 50, 0)));
            commands.Add(new MoveCommand(new Point3D(100, 100, 0)));
            commands.Add(new MoveCommand(new Point3D(50, 100, 0)));
            commands.Add(new UseToolCommand(false));
            commands.Add(new MoveCommand(new Point3D(50, 50, 0)));
            commands.Add(new ShutdownCommand());
            commands.Add(new StartCommand());
            commands.Add(new UseToolCommand(true));
            commands.Add(new MoveCommand(new Point3D(100, 50, 0)));
            commands.Add(new ShutdownCommand());
            edubot.OnAxisAngleChanged += React; 
            foreach (ICommand cmd in commands) {
                edubot.Execute(cmd);
            }
            
        }

        private void React(object src,  EventArgs args) {
            Console.WriteLine((args as AngleChangedEventArgs).Result.ToString());
        }
        /*
        private void button1_Click(object sender, EventArgs e)
        {
            engine.Enabled = !engine.Enabled;
            label1.Text = engine.Enabled.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            engine.Direction = !engine.Direction;
            label2.Text = engine.Direction.ToString();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            engine.CurrencyProtection = !engine.CurrencyProtection;
            label4.Text = engine.CurrencyProtection.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (engine.IsRunning) {
                engine.Stop();
            }else{
                engine.Start();
            }
            //label3.Text = con.Freq.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            engine.Speed = trackBar1.Value;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            engine.Dispose();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            engine.TurnAngle(Convert.ToInt32(textBox1.Text));
        }

        */

    }
}
