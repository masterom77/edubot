﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EduBotAPI;
using System.Threading;

namespace EduBot
{
    public partial class Form1 : Form
    {
        Engine engine;
        Thread t;

        public Form1()
        {
            InitializeComponent();
            engine = new Engine();
            if (engine.Initiate())
            {
                MessageBox.Show("SUCCESS");
            }
            else
            {
                MessageBox.Show("FAILED");
            }
        }

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
            label6.Text = trackBar1.Value.ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            engine.Dispose();
           
        }

        

    }
}
