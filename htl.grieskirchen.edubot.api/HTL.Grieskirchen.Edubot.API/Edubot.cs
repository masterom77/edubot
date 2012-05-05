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
            //state = State.SHUTDOWN;
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

        #region ------------------Events----------------------
        public event Event OnStateChanged;

        public event Event OnMovementStarted;

        public event Event OnToolUsed;

        public event Event OnHoming;

        public event Event OnAbort;

        public event Event OnShuttingDown;

        public event Event OnShutDown;

        public event Event OnFailure;
        #endregion

        #region ------------------Public Properties-----------------------
        State state;
        /// <summary>
        /// Gets the current state of the robot.
        /// </summary>
        //public State State
        //{
        //    get { return state; }
        //    private set {
        //        State old = state;
        //        state = value;
        //        if (OnStateChanged != null)
        //        {
        //            OnStateChanged(null, new StateChangedEventArgs(old, value));
        //        }
        //    }
        //}

        //bool isConnected;
        ///// <summary>
        ///// Gets the state of the physical connection. True means that the robot is physically connected.
        ///// </summary>
        //public bool IsConnected
        //{
        //    get { return isConnected; }
        //}

        //IInterpolationType interpolation;
        /// <summary>
        /// Gets or sets the interpolation type, which is used for calculating the path of
        /// the robot's movements.
        /// </summary>
        //public IInterpolationType Interpolation
        //{
        //    get { return interpolation; }
        //    set
        //    {
        //        IInterpolationType old = interpolation;
        //        interpolation = value;
        //        if (onInterpolationChanged != null)
        //        {
        //            OnInterpolationChanged(null, new InterpolationChangedEventArgs(old, value));
        //        }
        //    }
        //}


        #endregion

        #region ------------------Public Methods-------------------

        public void Execute(ICommand cmd) {
            foreach (KeyValuePair<string, IAdapter> entry in registeredAdapters)
            {
                IAdapter currentAdapter = entry.Value;
                currentAdapter.Execute(cmd);
            }
        }
        
        public bool RegisterAdapter(string name, IAdapter adapter)
        {
            if (registeredAdapters.ContainsKey(name) || registeredAdapters.ContainsValue(adapter))
                return false;

            adapter.OnHoming += RaiseHomingEvent;
            adapter.OnFailure += RaiseFailureEvent;
            adapter.OnAbort += RaiseAbortEvent;
            adapter.OnMovementStarted += RaiseMovementStartedEvent;
            adapter.OnShutDown += RaiseShutdownEvent;
            adapter.OnShuttingDown += RaiseShuttingDownEvent;
            adapter.OnToolUsed += RaiseToolUsedEvent;
            adapter.OnStateChanged += RaiseStateChangedEvent;
            registeredAdapters.Add(name, adapter);
            return true;
        }


        public bool DeregisterAdapter(string name)
        {
            IAdapter adapter;
            if (registeredAdapters.TryGetValue(name, out adapter)) {
                adapter.OnHoming -= RaiseHomingEvent;
                adapter.OnFailure -= RaiseFailureEvent;
                adapter.OnAbort -= RaiseAbortEvent;
                adapter.OnMovementStarted -= RaiseMovementStartedEvent;
                adapter.OnShutDown -= RaiseShutdownEvent;
                adapter.OnShuttingDown -= RaiseShuttingDownEvent;
                adapter.OnToolUsed -= RaiseToolUsedEvent;
                adapter.OnStateChanged -= RaiseStateChangedEvent;
            }
            return registeredAdapters.Remove(name);
        }

        internal void RaiseStateChangedEvent(object sender, System.EventArgs args)
        {
            if (OnStateChanged != null)
            {
                OnStateChanged(sender, args);
            }

        }

        internal void RaiseMovementStartedEvent(object sender, System.EventArgs args)
        {
            if (OnMovementStarted != null)
                OnMovementStarted(sender, args);
        }

        internal void RaiseHomingEvent(object sender, System.EventArgs args)
        {
            if (OnHoming != null)
                OnHoming(sender, args);
        }

        internal void RaiseShuttingDownEvent(object sender, System.EventArgs args)
        {
            if (OnShuttingDown != null)
                OnShuttingDown(sender, args);
        }

        internal void RaiseShutdownEvent(object sender, System.EventArgs args)
        {
            if (OnShutDown != null)
                OnShutDown(sender, args);
        }

        internal void RaiseAbortEvent(object sender, System.EventArgs args)
        {
            if (OnAbort != null)
                OnAbort(sender, args);
        }

        internal void RaiseToolUsedEvent(object sender, System.EventArgs args)
        {
            if (OnToolUsed != null)
                OnToolUsed(sender, args);
        }

        internal void RaiseFailureEvent(object sender, System.EventArgs args)
        {
            if (OnFailure != null)
                OnFailure(sender, args);
        }


        #endregion
    }
}
