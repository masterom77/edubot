﻿using System;
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
        VirtualAdapter adapter = new VirtualAdapter(Tool.VIRTUAL, 150f, 150f,50f,1);
        VirtualAdapter adapter2 = new VirtualAdapter(Tool.VIRTUAL, 150f, 150f, 50f, 1);

        public Form1()
        {
            InitializeComponent();
            Edubot edubot = Edubot.GetInstance();
            //Kinematics.DisplayResults = true;
            
            //edubot.RegisterAdapter("virtual", adapter);
            //edubot.RegisterAdapter("virtual2", adapter2);
            //adapter.OnMovementStarted += OnUpdate1;
            //adapter2.OnMovementStarted += OnUpdate2;
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            new Thread(ControllerCircle).Start(); 
            //EdubotAdapter a = new EdubotAdapter(Tool.VIRTUAL, 150,150, 135,-135, 160,-160,System.Net.IPAddress.Parse("127.0.0.1"), 12000);
            //edubot.RegisterAdapter("default",a);
            ////bool conTest = a.TestConnectivity();
            //List<ICommand> commands = new List<ICommand>();
            //commands.Add(new InitCommand());
            //commands.Add(new MVSCommand(new Point3D(100, 0, 0)));
            //commands.Add(new MVSCommand(new Point3D(200, 100, 0)));
            //commands.Add(new MVSCommand(new Point3D(150, 150, 0)));
            //commands.Add(new ShutdownCommand());
            ////commands.Add(new StartCommand());
            ////commands.Add(new MVSCommand(new Point3D(100, 150, 0)));
            ////edubot.OnAxisAngleChanged += React; 
            //foreach (ICommand cmd in commands) {
            //    edubot.Execute(cmd);
            //}
            
        }

        public void OnUpdate1(object sender, EventArgs args) {
            Console.WriteLine("Update first adapter:"+((MovementStartedEventArgs)args).Result.Angles.Count);
            adapter.SetState(State.READY);
        }
        public void OnUpdate2(object sender, EventArgs args)
        {
            Console.WriteLine("Update second adapter:" + ((MovementStartedEventArgs)args).Result.Angles.Count);
            adapter2.SetState(State.READY);
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
                    if (message.StartsWith("hom:")) {
                        Console.WriteLine("Controller: start received");
                        Thread.Sleep(12000);
                        writeBuffer = Encoding.UTF8.GetBytes("ready");
                        client.GetStream().Write(Encoding.UTF8.GetBytes("ready"),0,writeBuffer.Length);
                    }
                    if (message.StartsWith("mvs:") || message.StartsWith("mvc:"))
                    {
                        Console.WriteLine("Controller: move received");
                        Thread.Sleep(new Random().Next(7000,20000));
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
            Console.WriteLine((args as MovementStartedEventArgs).Result.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HTL.Grieskirchen.Edubot.API.Edubot.GetInstance().Execute(new AbortCommand());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HTL.Grieskirchen.Edubot.API.Edubot.GetInstance().Execute(new AbortCommand());
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
