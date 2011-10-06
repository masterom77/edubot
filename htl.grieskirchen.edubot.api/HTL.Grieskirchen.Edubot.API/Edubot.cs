using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API
{
    /// <summary>
    /// This class represents the robot and can be used for controlling the physical robot, or simulating it's movements by using the specified events. This class is designed as Singleton, which means theres only one instance available and you can't create a new one.
    /// </summary>
    public class Edubot
    {
        private Edubot() {
            state = State.DISCONNECTED;
        }

        /// <summary>
        /// Returns a unique instance of the Edubot object.
        /// </summary>
        /// <returns>A unique Edubot instance.</returns>
        public static Edubot GetInstance() {
            return EdubotCreator.instance;
        }

        /// <summary>
        /// This class is used for creating the instance of Edubot object. Since the
        /// Edubot class is a designed as a Singleton, there is only one instance of the
        /// Edubot available.
        /// </summary>
        class EdubotCreator
        {
            internal static Edubot instance;
            static EdubotCreator() {
                instance = new Edubot();
            }

        }

        public delegate void Event(object sender, System.EventArgs args);

        Event onStartup;
        /// <summary>
        /// Triggered after the robot has started.
        /// </summary>
        public Event OnStartup
        {
            get { return onStartup; }
            set { onStartup = value; }
        }

        Event onConnectionEstablished;
        /// <summary>
        /// Triggered after the connection to the physical robot is established.
        /// </summary>
        public Event OnConnected
        {
            get { return onConnectionEstablished; }
            set { onConnectionEstablished = value; }
        }

        Event onDiconnected;
        /// <summary>
        /// Triggered after the connection to the physical robot is lost or closed.
        /// </summary>
        public Event OnDiconnected
        {
            get { return onDiconnected; }
            set { onDiconnected = value; }
        }

        Event onShutdown;
        /// <summary>
        /// Triggered before the robot is shutting down.
        /// </summary>
        public Event OnShutdown
        {
            get { return onShutdown; }
            set { onShutdown = value; }
        }

        Event onAxisAngleChanged;
        /// <summary>
        /// Triggered if the angle of an axis has changed. The specific axis can be found in the event arguments.
        /// </summary>
        public Event OnAxisAngleChanged
        {
            get { return onAxisAngleChanged; }
            set { onAxisAngleChanged = value; }
        }

        Event onInterpolationChanged;
        /// <summary>
        /// Triggered if the interpolation type of the robot has changed.
        /// </summary>
        public Event OnInterpolationChanged
        {
            get { return onInterpolationChanged; }
            set { onInterpolationChanged = value; }
        }

        Event onToolUsed;
        /// <summary>
        /// Triggered if the robot's tool was activated or deactivated.
        /// </summary>
        public Event OnToolUsed
        {
            get { return onToolUsed; }
            set { onToolUsed = value; }
        }

        Event onStateChanged;
        /// <summary>
        /// Triggered if the robot's state has changed.
        /// </summary>
        public Event OnStateChanged
        {
            get { return onStateChanged; }
            set { onStateChanged = value; }
        }

        State state;
        /// <summary>
        /// Gets the current state of the robot.
        /// </summary>
        public State State
        {
            get { return state; }
            private set {
                State old = state;
                state = value;
                if (OnStateChanged != null)
                {
                    OnStateChanged(null, new StateChangedEventArgs(old, value));
                }
            }
        }

        /// <summary>
        /// The primary axis of the robot, which is located on the top of the scaffold.
        /// </summary>
        Axis primaryAxis;
        /// <summary>
        /// The secondary axis of the robot, which is located at the end of the first axis.
        /// </summary>
        Axis secondaryAxis;
        /// <summary>
        /// The vertical axis of the robot, which is located at the end of the second axis.
        /// </summary>
        Axis verticalAxis;
        /// <summary>
        /// The tool axis of the robot, which is located above the actual tool,
        /// </summary>
        Axis toolAxis;

        IInterpolationType interpolation;
        /// <summary>
        /// Gets or sets the interpolation type, which is used for calculating the path of
        /// the robot's movements.
        /// </summary>
        public IInterpolationType Interpolation
        {
            get { return interpolation; }
            set
            {
                IInterpolationType old = interpolation; 
                interpolation = value;
                if (onInterpolationChanged != null)
                {
                    OnInterpolationChanged(null, new InterpolationChangedEventArgs(old, value));
                }
            }
        }

        /// <summary>
        /// Moves the tool to the specified 3D-Point (x,y,z).
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-coordinate.</param>
        public void MoveTo(int x, int y, int z) {
            Console.WriteLine("Calculating rotation-movements by using current interpolation type...");
            State = State.MOVING;
            Console.WriteLine("Rotating primary engine");
            if (OnAxisAngleChanged != null)
            {
                OnAxisAngleChanged(null, new AngleChangedEventArgs(AxisType.PRIMARY, 1000, 5f));
            }
            Console.WriteLine("Rotating secondary engine");
            if (OnAxisAngleChanged != null)
            {
                OnAxisAngleChanged(null, new AngleChangedEventArgs(AxisType.SECONDARY, 1500, 7.5f));
            }
            Console.WriteLine("Rotating vertical engine");
            if (OnAxisAngleChanged != null)
            {
                OnAxisAngleChanged(null, new AngleChangedEventArgs(AxisType.VERTICAL, 500, 2.5f));
            }
            Console.WriteLine("Movement completed");
            State = State.READY;
        }

        /// <summary>
        /// Activates or deactivates the robot's tool.
        /// </summary>
        /// <param name="activate">True if the tool should be activated, false if the tool should be deactivated.</param>
        public void UseTool(bool activate) {
            State = State.MOVING;
            Console.WriteLine("Setting pin of tool to: " + (activate ? "1" : "0"));
            State = State.READY;
            if (OnToolUsed != null)
            {
                OnToolUsed(null, new ToolEventArgs(activate));
            }
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
                State = State.READY;
                if (OnStartup != null)
                {
                    OnStartup(null, new System.EventArgs());
                }
        }

        /// <summary>
        /// Turns the robot off.
        /// </summary>
        public void Shutdown()
        {
            if (OnShutdown != null)
            {
                OnShutdown(null, new System.EventArgs());
            }
            Console.WriteLine("Shutting down engines...");
            Console.WriteLine("Setting enabled-pin of primary engine to 1");
            Console.WriteLine("Setting enabled-pin of secondary engine to 1");
            Console.WriteLine("Setting enabled-pin of vertical engine to 1");
            Console.WriteLine("Setting enabled-pin of tool engine to 1");
            State = State.SHUTDOWN;
        }
    }
}
