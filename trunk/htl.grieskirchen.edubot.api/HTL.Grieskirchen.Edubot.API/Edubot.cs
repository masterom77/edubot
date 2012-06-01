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
            registeredAdapters = new Dictionary<string, IAdapter>();
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
        /// Edubot class is designed as a Singleton, there is only one instance of the
        /// Edubot available.
        /// </summary>
        class EdubotCreator
        {
            internal static Edubot instance;
            static EdubotCreator() {
                instance = new Edubot();
            }

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
       
        Dictionary<string, IAdapter> registeredAdapters;

        public Dictionary<string, IAdapter> RegisteredAdapters
        {
            get { return registeredAdapters; }
        }

        #endregion

        #region ------------------Public Methods-------------------

        /// <summary>
        /// Executes the given command, by passing it to the registered adapters 
        /// </summary>
        /// <param name="cmd">The command which should be executed</param>
        public void Execute(ICommand cmd)
        {
            foreach (KeyValuePair<string, IAdapter> entry in registeredAdapters)
            {
                IAdapter currentAdapter = entry.Value;
                currentAdapter.Execute(cmd);
            }
        }

        
            
        

        private void Synchronize(object sender, System.EventArgs e) {
            StateChangedEventArgs args = (StateChangedEventArgs)e;
            IAdapter adapter = (IAdapter)sender;
            List<IAdapter> synchronizedAdapters = new List<IAdapter>();
            if (adapter.Synchronized) {
                if (args.NewState == State.READY || args.NewState == State.SHUTDOWN) {
                    //Get all adapters which should be synchronized
                    foreach (KeyValuePair<string, IAdapter> entry in registeredAdapters) {
                        if (entry.Value.Synchronized) {
                           synchronizedAdapters.Add(entry.Value);
                        }
                    }
                    //Check if all of them have the same state and queue length
                    foreach (IAdapter entry in synchronizedAdapters) {
                        
                        if (entry.GetState() != args.NewState || entry.CmdQueue.Count != adapter.CmdQueue.Count)
                        {
                            return;
                        }
                    }
                    //At this point, all states are the same
                    //Tell the synchronized adapters to execute the next command
                    foreach (IAdapter entry in synchronizedAdapters)
                    {
                        entry.ExecuteNextCommand();
                    }
                }
            }
        }

        /// <summary>
        /// Registers a new adapters and adds event handlers on its events
        /// </summary>
        /// <param name="name">An arbitrary name, which is used to identify the adapter</param>
        /// <param name="adapter">The adapter, which should registered</param>
        /// <returns>Returns a boolean value, indicating wether the registration was successful or not</returns>
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
            adapter.OnStateChanged += Synchronize;
            adapter.OnCommandEnqueued += Synchronize;
            registeredAdapters.Add(name, adapter);
            adapter.RaiseStateChangedEvent(new StateChangedEventArgs(adapter.GetState(), adapter.GetState()));
            return true;
        }

        /// <summary>
        /// Deregisteres a adapter and removes event handlers from its events
        /// </summary>
        /// <param name="name">The name of the adapter</param>
        /// <returns>Returns a value indicating wether the removal was successful or not</returns>
        public bool DeregisterAdapter(string name)
        {
            IAdapter adapter;
            if (registeredAdapters.TryGetValue(name, out adapter))
            {
                adapter.OnHoming -= RaiseHomingEvent;
                adapter.OnFailure -= RaiseFailureEvent;
                adapter.OnAbort -= RaiseAbortEvent;
                adapter.OnMovementStarted -= RaiseMovementStartedEvent;
                adapter.OnShutDown -= RaiseShutdownEvent;
                adapter.OnShuttingDown -= RaiseShuttingDownEvent;
                adapter.OnToolUsed -= RaiseToolUsedEvent;
                adapter.OnStateChanged -= RaiseStateChangedEvent;
                adapter.OnStateChanged -= Synchronize;
                adapter.OnCommandEnqueued -= Synchronize;
                adapter.Execute(new AbortCommand());
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
