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

namespace EduBot
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            Edubot.GetInstance().MoveTo(150, 150,0);
            
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
