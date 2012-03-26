using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;
using HTL.Grieskirchen.Edubot.API.Commands;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public abstract class IAdapter
    {
        public IAdapter()
        {
            cmdQueue = new Queue<ICommand>();
            state = State.SHUTDOWN;
            //interpolation = new LinearInterpolation();
        }

        public IAdapter(ITool tool, float length, float length2) {
            cmdQueue = new Queue<ICommand>();
            state = State.SHUTDOWN;
            this.tool = tool;
            tool.ToolCenterPoint = new Point3D((int)length + length2, 0, 0);
            this.length = length;
            this.length2 = length2;
            //interpolation = new LinearInterpolation();
            length2 = length;
        }

        #region Properties

        protected AdapterType type;

        public AdapterType Type
        {
            get { return type; }
        }

        protected ITool tool;

        public ITool Tool
        {
            get { return tool; }
            set { tool = value; }
        }       

        protected float length;

        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        protected float length2;

        public float Length2
        {
            get { return length2; }
            set { length2 = value; }
        }

        protected bool requiresPrecalculation;

        public bool RequiresPrecalculation
        {
            get { return requiresPrecalculation; }
            set { requiresPrecalculation = value; }
        }
        protected State state;

        public State State
        {
            get { return state; }
            set {
                if (OnStateChanged != null)
                {
                    OnStateChanged(this, new EventArgs.StateChangedEventArgs(state, value));
                }
                state = value;
                if ((state == State.READY || state == State.SHUTDOWN) && cmdQueue.Count > 0) {
                    cmdQueue.Dequeue().Execute(this);
                }
            }
        }

        private IStateListener listener;

        protected IStateListener Listener
        {
            get { return listener; }
            set { listener = value; }
        }

        Queue<ICommand> cmdQueue;

        public Queue<ICommand> CmdQueue
        {
            get { return cmdQueue; }
            set { cmdQueue = value; }
        }

        IInterpolationType interpolation;

        public IInterpolationType Interpolation
        {
            get { return interpolation; }
            set { interpolation = value; }
        }

        #endregion

        private Event onStateChanged;

        public Event OnStateChanged
        {
            get { return onStateChanged; }
            set { onStateChanged = value; }
        }

        Event onMovementStarted;

        /// <summary>
        /// Triggered if the angle of an axis has changed. The specific axis can be found in the event arguments.
        /// </summary>
        public Event OnMovementStarted
        {
            get { return onMovementStarted; }
            set { onMovementStarted = value; }
        }

        private Event onAbort;

        public Event OnAbort
        {
            get { return onAbort; }
            set { onAbort = value; }
        }

        public abstract void Start();
        public abstract void Shutdown();
        public abstract void MoveStraightTo(object param);
        public abstract void MoveCircularTo(object param);
        public abstract void UseTool(object param);
        public abstract void Abort();
        public abstract void SetInterpolationResult(Interpolation.InterpolationResult result);

        public void Execute(ICommand cmd) {
            cmdQueue.Enqueue(cmd);
            if (state == State.READY || state == State.SHUTDOWN) {
                cmdQueue.Dequeue().Execute(this);
            }
        }

        public override string ToString()
        {
            return type.ToString();
        }
    }
}
