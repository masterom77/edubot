using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EduBotAPI
{
    public class Edubot
    {


        Axis primaryAxis;
        Axis secondaryAxis;
        Axis verticalAxis;
        Axis toolAxis;

        IInterpolationType interpolation;
        /// <summary>
        /// Gets or sets the interpolation type, which is used for calculating the path of
        /// the robot's movements
        /// </summary>
        public IInterpolationType Interpolation
        {
            get { return interpolation; }
            set { interpolation = value; }
        }

        /// <summary>
        /// Moves the tool to the specified 3D-Point (x,y,z)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void MoveTo(int x, int y, int z) {
            Console.WriteLine("Calculating rotation-movements by using current interpolation type...");
            Console.WriteLine("Rotating primary engine");
            Console.WriteLine("Rotating secondary engine");
            Console.WriteLine("Rotating vertical engine");
        }

        /// <summary>
        /// Activates or deactivates the robot's tool
        /// </summary>
        /// <param name="activate">True if the tool should be activated, false if the tool should be deactivate</param>
        public void ActivateTool(bool activate) {
            Console.WriteLine("Setting pin of tool to: " + (activate ? "1" : "0"));
        }


        /// <summary>
        /// Turns the robot on
        /// </summary>
        public void Start() {
            Console.WriteLine("Starting engines...");
            Console.WriteLine("Setting enabled-pin of primary engine to 0");
            Console.WriteLine("Setting enabled-pin of secondary engine to 0");
            Console.WriteLine("Setting enabled-pin of vertical engine to 0");
            Console.WriteLine("Setting enabled-pin of tool engine to 0");
        }

        /// <summary>
        /// Turns the robot off
        /// </summary>
        public void Shutdown()
        {
            Console.WriteLine("Shutting down engines...");
            Console.WriteLine("Setting enabled-pin of primary engine to 1");
            Console.WriteLine("Setting enabled-pin of secondary engine to 1");
            Console.WriteLine("Setting enabled-pin of vertical engine to 1");
            Console.WriteLine("Setting enabled-pin of tool engine to 1");
        
        }
    }
}
