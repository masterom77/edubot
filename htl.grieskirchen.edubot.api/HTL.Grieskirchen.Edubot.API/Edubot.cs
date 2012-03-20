using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HTL.Grieskirchen.Edubot.API.EventArgs;
using HTL.Grieskirchen.Edubot.API.Exceptions;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.Commands;

namespace HTL.Grieskirchen.Edubot.API
{
    /// <summary>
    /// This class represents the robot and can be used for controlling the physical robot, or simulating it's movements by using the specified events. This class is designed as Singleton, which means theres only one instance available and you can't create a new one.
    /// </summary>
    public class Edubot
    {
        private Edubot() {

            //InitAxis();
            //interpolation = new LinearInterpolation();
            registeredAdapters = new Dictionary<string, IAdapter>();
            state = State.SHUTDOWN;
            //tool = new VirtualTool();
            //tool.X = 300;
            //tool.Y = 0;
            
        }
        /// <summary>
        /// Initializes the robot's axis
        /// </summary>
        private void InitAxis() {
            //float length = 15f;
            //primaryAxis = new Axis(0 ,0 ,50, 0);
            //secondaryAxis = new Axis((int)length*10, 0, 60, 0);
            //verticalAxis = new Axis((int)length*20, 0, 40, 0);
            //toolAxis = new Axis(60, 20, 0, 0);
            
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

        Dictionary<string, IAdapter> registeredAdapters;

        public Dictionary<string, IAdapter> RegisteredAdapters
        {
            get { return registeredAdapters; }
        }

        //List<IAdapter> registeredAdapters;

        //public delegate void Event(object sender, System.EventArgs args);

        #region ------------------Events----------------------
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
        #endregion

        #region ------------------Public Properties-----------------------
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

        bool isConnected;
        /// <summary>
        /// Gets the state of the physical connection. True means that the robot is physically connected.
        /// </summary>
        public bool IsConnected
        {
            get { return isConnected; }
        }

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


        #endregion

        #region ------------------Private Properties----------------------
        ///// <summary>
        ///// The primary axis of the robot, which is located on the top of the scaffold.
        ///// </summary>
        //Axis primaryAxis;
        ///// <summary>
        ///// The secondary axis of the robot, which is located at the end of the first axis.
        ///// </summary>
        //Axis secondaryAxis;
        ///// <summary>
        ///// The vertical axis of the robot, which is located at the end of the second axis.
        ///// </summary>
        //Axis verticalAxis;
        ///// <summary>
        ///// The tool axis of the robot, which is located above the actual tool,
        ///// </summary>
        //Axis toolAxis;

        ///// <summary>
        ///// The tool of the robot
        ///// </summary>
        //ITool tool;
        Queue<Point3D> targetCoordinates;
       

        #endregion

        #region ------------------Public Methods-------------------

        public void Execute(ICommand cmd) {
            foreach (KeyValuePair<string, IAdapter> entry in registeredAdapters)
            {
                IAdapter currentAdapter = entry.Value;
                currentAdapter.Execute(cmd);
            }
        }
        ///// <summary>
        ///// Moves the tool to the specified 3D-Point (x,y,z).
        ///// </summary>
        ///// <param name="x">The x-coordinate.</param>
        ///// <param name="y">The y-coordinate.</param>
        ///// <param name="z">The z-coordinate.</param>
        //public void MoveTo(int x, int y, int z) {
            
        //        Thread thread = new Thread(MoveTo);
        //        thread.Start(new Point3D(x, y, z));
        //}
         
        //private void MoveTo(object coordinates) {
        //    //int[] coords = (int[])coordinates;
        //    //int x = coords[0];
        //    //int y = coords[1];
        //    //int z = coords[2];
        //    Point3D target = (Point3D) coordinates;
        //    InterpolationResult result = null;
        //    State = State.MOVING;
        //    if(registeredAdapters.Count == 0)
        //        throw new ArgumentException("Es ist kein Adapter registriert. Verwenden Sie die \"RegisterAdapter\"-Methode der Edubot-Klasse.");
        //    foreach (KeyValuePair<AdapterType, IAdapter> entry in registeredAdapters)
        //    {
        //        IAdapter currentAdapter = entry.Value;
        //        if (currentAdapter.RequiresPrecalculation)
        //        {
        //            result = interpolation.CalculatePath(currentAdapter.Tool, target, currentAdapter.Length);
        //            currentAdapter.SetInterpolationResult(result);
        //        }

        //        if (OnAxisAngleChanged != null)
        //        {
        //            OnAxisAngleChanged(entry.Key, new AngleChangedEventArgs(result));
        //        }

        //        currentAdapter.MoveTo(target);
                
        //        currentAdapter.Tool.X = target.X;
        //        currentAdapter.Tool.Y = target.Y;
        //        currentAdapter.Tool.Z = target.Z;

        //    }
        //    State = State.READY;
        //}

        ///// <summary>
        ///// Activates or deactivates the robot's tool.
        ///// </summary>
        ///// <param name="activate">True if the tool should be activated, false if the tool should be deactivated.</param>
        //public void UseTool(bool activate)
        //{
        //    Thread thread = new Thread(UseTool);
        //    thread.Start(activate);


        //}

        //private void UseTool(object activate) {
        //    State = State.MOVING;
        //    if (registeredAdapters.Count == 0)
        //        throw new ArgumentException("Es ist kein Adapter registriert. Verwenden Sie die \"RegisterAdapter\"-Methode der Edubot-Klasse.");
        //    foreach (KeyValuePair<AdapterType, IAdapter> adapter in registeredAdapters)
        //    {
        //        adapter.Value.UseTool((bool)activate);
        //        if (OnToolUsed != null)
        //        {
        //            OnToolUsed(adapter.Key, new ToolEventArgs((bool)activate));
        //        }
        //    }
        //    State = State.READY;
        //}


        ///// <summary>
        ///// Turns the robot on
        ///// </summary>
        //public void Start()
        //{
        //    if (State == State.SHUTDOWN)
        //    {
        //        State = State.STARTING;
        //        foreach (KeyValuePair<AdapterType, IAdapter> entry in registeredAdapters)
        //        {

        //            IAdapter currentAdapter = entry.Value;
        //            currentAdapter.Start();
        //            if (OnStartup != null)
        //            {
        //                OnStartup(entry.Key, new System.EventArgs());
        //            }
        //        }
        //        State = State.READY;
        //    }
        //    else {
        //        throw new InvalidStateException("Starting the robot isn't possible since it's current state is: " + State);
        //    }
        //}

        ///// <summary>
        ///// Turns the robot off.
        ///// </summary>
        //public void Shutdown()
        //{
        //    if (State != State.SHUTDOWN)
        //    {
        //        State = State.SHUTTING_DOWN;
                
        //        foreach (KeyValuePair<AdapterType, IAdapter> entry in registeredAdapters)
        //        {
        //            IAdapter currentAdapter = entry.Value;
        //            if (OnShutdown != null)
        //            {
        //                OnShutdown(entry.Key, new System.EventArgs());
        //            }
        //            currentAdapter.Shutdown();
                    
        //        }
        //        State = State.SHUTDOWN;
        //    }
        //    else {
        //        throw new InvalidStateException("Shutting down the robot isn't possible since it's current state is: " + State);
        //    }
        //}


        ///// <summary>
        ///// Trys to connect to the physical robot
        ///// </summary>
        ///// <returns>Returns true if the connections was successfully established, else returns false</returns>
        //public bool Connect()
        //{
        //    if (new Random().Next(2) == 0/*try to connect by using the Controller class*/)
        //    {
        //        isConnected = true;
        //        if (OnConnected != null)
        //        {
        //            OnConnected(null, new System.EventArgs());
        //        }
        //    }
        //    else
        //    {
        //        isConnected = false;
        //    }
        //    return isConnected;
        //}

        ///// <summary>
        ///// Trys to disconnect the physical robot
        ///// </summary>
        ///// <returns>Returns true if the connections was successfully established, else returns false</returns>
        //public bool Disconnect()
        //{
        //    if (new Random().Next(2) == 0/*try to disconnect by using the Controller class*/)
        //    {
        //        isConnected = false;
        //        if (OnDiconnected != null)
        //        {
        //            OnDiconnected(null, new System.EventArgs());
        //        }
        //    }
        //    else
        //    {
        //        isConnected = true;
        //    }
        //    return isConnected;
        //}

        //public void RegisterAdapter(AdapterType adapter)
        //{
        //    if (registeredAdapters.ContainsKey(adapter))
        //        throw new AdapterException(adapter, "Der Adapter ist bereits registriert: \"" + adapter.ToString() + "\"");

        //    registeredAdapters.Add(adapter, AdapterFactory.GetAdapter(adapter));
        //}

        public bool RegisterAdapter(string name, IAdapter adapter)
        {
            if (registeredAdapters.ContainsValue(adapter))
                return false;

            registeredAdapters.Add(name, adapter);
            return true;
        }

        public bool DeregisterAdapter(string name)
        {
            if (!registeredAdapters.Remove(name))
                return false;
            return true;
        }

        #endregion
    }
}
