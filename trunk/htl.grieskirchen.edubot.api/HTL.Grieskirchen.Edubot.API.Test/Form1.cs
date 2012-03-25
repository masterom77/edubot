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
using System.Net.Sockets;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace EduBot
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            Edubot edubot = Edubot.GetInstance();
            Kinematics.DisplayResults = true;
            VirtualAdapter adapter = new VirtualAdapter(new VirtualTool(), 200f, 150f);
            edubot.RegisterAdapter("virtual",adapter);
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            new Thread(ControllerCircle).Start(); 
            //DefaultAdapter a = new DefaultAdapter(new VirtualTool(), 153,153, System.Net.IPAddress.Parse("127.0.0.1"), 12000);
            //edubot.RegisterAdapter("default",a);
            //bool conTest = a.TestConnectivity();
            List<ICommand> commands = new List<ICommand>();
            commands.Add(new StartCommand());
            commands.Add(new MVSCommand(new Point3D(100, 0, 0)));
            commands.Add(new ShutdownCommand());
            //commands.Add(new StartCommand());
            //commands.Add(new MVSCommand(new Point3D(100, 150, 0)));
            //edubot.OnAxisAngleChanged += React; 
            foreach (ICommand cmd in commands) {
                edubot.Execute(cmd);
            }
            
        }

        private void ControllerCircle() {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"),12000);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            byte[] readBuffer = new byte[512];
            byte[] writeBuffer;
            while(true){
              
                List<byte> bytes = new List<byte>();
                while (client.GetStream().DataAvailable) {
                    int bytesRead = client.GetStream().Read(readBuffer, 0, readBuffer.Length);
                    for (int i = 0; i < bytesRead;i++ )
                    {
                        bytes.Add(readBuffer[i]);
                    }
                }
                if (bytes.Count > 0)
                {
                    string message = Encoding.UTF8.GetString(bytes.ToArray());
                    bytes.Clear();
                    if (message.StartsWith("start")) {
                        Console.WriteLine("Controller: start received");
                        Thread.Sleep(1000);
                        writeBuffer = Encoding.UTF8.GetBytes("ready");
                        client.GetStream().Write(Encoding.UTF8.GetBytes("ready"),0,writeBuffer.Length);
                    }
                    if (message.StartsWith("mvs:") || message.StartsWith("mvc:"))
                    {
                        Console.WriteLine("Controller: move received");
                        Thread.Sleep(message.Length);
                        writeBuffer = Encoding.UTF8.GetBytes("ready");
                        client.GetStream().Write(Encoding.UTF8.GetBytes("ready"), 0, writeBuffer.Length);
                    }
                    if (message.StartsWith("shutdown"))
                    {
                        Console.WriteLine("Controller: shutdown received");
                        Thread.Sleep(1000);
                        writeBuffer = Encoding.UTF8.GetBytes("shutdown");
                        client.GetStream().Write(Encoding.UTF8.GetBytes("shutdown"), 0, writeBuffer.Length);
                    }
                }
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
