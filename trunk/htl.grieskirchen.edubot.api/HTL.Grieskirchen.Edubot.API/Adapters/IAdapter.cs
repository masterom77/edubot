using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;
using HTL.Grieskirchen.Edubot.API.Commands;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.EventArgs;
using HTL.Grieskirchen.Edubot.API.Exceptions;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public abstract class IAdapter
    {
        /// <summary>
        /// Constructor for RR-Kinematic without angle restrictions
        /// </summary>
        /// <param name="equippedTool"></param>
        /// <param name="length"></param>
        /// <param name="length2"></param>
        /// <param name="maxPrimaryAngle"></param>
        /// <param name="minPrimaryAngle"></param>
        /// <param name="maxSecondaryAngle"></param>
        /// <param name="minSecondaryAngle"></param>
        public IAdapter(Tool equippedTool, float length, float length2)
        {
            cmdQueue = new Queue<ICommand>();
            state = State.SHUTDOWN;
            axisConfiguration = AxisConfiguration.Lefty;
            this.equippedTool = equippedTool;
            toolCenterPoint = new Point3D((int)length + length2, 0, 0);
            this.length = length;
            this.length2 = length2;
            this.verticalToolRange = 0;
            this.transmission = 0;
            this.maxPrimaryAngle = float.MaxValue;
            this.minPrimaryAngle = float.MinValue;
            this.maxSecondaryAngle = float.MaxValue;
            this.minSecondaryAngle = float.MinValue;
        }

        /// <summary>
        /// Constructor for RR-Cinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool"></param>
        /// <param name="length"></param>
        /// <param name="length2"></param>
        /// <param name="maxPrimaryAngle"></param>
        /// <param name="minPrimaryAngle"></param>
        /// <param name="maxSecondaryAngle"></param>
        /// <param name="minSecondaryAngle"></param>
        public IAdapter(Tool equippedTool, float length, float length2, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle)
        {
            cmdQueue = new Queue<ICommand>();
            state = State.SHUTDOWN;
            this.equippedTool = equippedTool;
            axisConfiguration = AxisConfiguration.Lefty;
            toolCenterPoint = new Point3D((int)length + length2, 0, 0);
            this.length = length;
            this.length2 = length2;
            this.verticalToolRange = 0;
            this.transmission = 0;
            this.maxPrimaryAngle = maxPrimaryAngle;
            this.minPrimaryAngle = minPrimaryAngle;
            this.maxSecondaryAngle = maxSecondaryAngle;
            this.minSecondaryAngle = minSecondaryAngle;
        }

        /// <summary>
        /// Constructor for RRT-Kinematic without angle restrictions
        /// </summary>
        /// <param name="equippedTool"></param>
        /// <param name="length"></param>
        /// <param name="length2"></param>
        /// <param name="verticalToolRange"></param>
        /// <param name="transmission"></param>
        /// <param name="maxPrimaryAngle"></param>
        /// <param name="minPrimaryAngle"></param>
        /// <param name="maxSecondaryAngle"></param>
        /// <param name="minSecondaryAngle"></param>
        public IAdapter(Tool equippedTool, float length, float length2, float verticalToolRange, float transmission)
        {
            cmdQueue = new Queue<ICommand>();
            state = State.SHUTDOWN;
            this.equippedTool = equippedTool;
            axisConfiguration = AxisConfiguration.Lefty;
            toolCenterPoint = new Point3D((int)length + length2, 0, verticalToolRange);
            this.length = length;
            this.length2 = length2;
            this.verticalToolRange = verticalToolRange;
            this.transmission = transmission;
            this.maxPrimaryAngle = float.MaxValue;
            this.minPrimaryAngle = float.MinValue;
            this.maxSecondaryAngle = float.MaxValue;
            this.minSecondaryAngle = float.MinValue;
        }

        /// <summary>
        /// Constructor for RRT-Kinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool"></param>
        /// <param name="length"></param>
        /// <param name="length2"></param>
        /// <param name="verticalToolRange"></param>
        /// <param name="transmission"></param>
        /// <param name="maxPrimaryAngle"></param>
        /// <param name="minPrimaryAngle"></param>
        /// <param name="maxSecondaryAngle"></param>
        /// <param name="minSecondaryAngle"></param>
        public IAdapter(Tool equippedTool, float length, float length2, float verticalToolRange, float transmission, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle)
        {
            cmdQueue = new Queue<ICommand>();
            state = State.SHUTDOWN;
            axisConfiguration = AxisConfiguration.Lefty;
            this.equippedTool = equippedTool;
            toolCenterPoint = new Point3D((int)length + length2, 0, verticalToolRange);
            this.length = length;
            this.length2 = length2;
            this.verticalToolRange = verticalToolRange;
            this.transmission = transmission;
            this.maxPrimaryAngle = maxPrimaryAngle;
            this.minPrimaryAngle = minPrimaryAngle;
            this.maxSecondaryAngle = maxSecondaryAngle;
            this.minSecondaryAngle = minSecondaryAngle;
        }

        #region Properties

        protected Tool equippedTool;

        public Tool EquippedTool
        {
            get { return equippedTool; }
            set { equippedTool = value; }
        }

        protected Point3D toolCenterPoint;

        public Point3D ToolCenterPoint
        {
            get { return toolCenterPoint; }
            set { toolCenterPoint = value; }
        }

        public Point3D HomePoint {
            get { return new Point3D(Length + Length2, 0, 0); }
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

        protected float verticalToolRange;

        public float VerticalToolRange
        {
            get { return verticalToolRange; }
            set { verticalToolRange = value; }
        }

        protected float maxPrimaryAngle;

        public float MaxPrimaryAngle
        {
            get { return maxPrimaryAngle; }
            set { maxPrimaryAngle = value; }
        }

        protected float minPrimaryAngle;

        public float MinPrimaryAngle
        {
            get { return minPrimaryAngle; }
            set { minPrimaryAngle = value; }
        }

        protected float maxSecondaryAngle;

        public float MaxSecondaryAngle
        {
            get { return maxSecondaryAngle; }
            set { maxSecondaryAngle = value; }
        }

        protected float minSecondaryAngle;

        public float MinSecondaryAngle
        {
            get { return minSecondaryAngle; }
            set { minSecondaryAngle = value; }
        }

        protected float transmission;

        public float Transmission
        {
            get { return transmission; }
            set { transmission = value; }
        }

        protected AxisConfiguration axisConfiguration;

        public AxisConfiguration AxisConfiguration
        {
            get { return axisConfiguration; }
            set { axisConfiguration = value; }
        }

        protected State state;

        protected State State
        {
            get { return state; }
            set {
                if (OnStateChanged != null)
                {
                    OnStateChanged(this, new EventArgs.StateChangedEventArgs(state, value));
                }
                state = value;
                CheckState();
                //if ((state == State.READY || state == State.SHUTDOWN) && cmdQueue.Count > 0) {
                //    ICommand nextCmd = cmdQueue.Dequeue();
                //    FailureEventArgs failureArgs = nextCmd.CanExecute(this);
                //    if (failureArgs == null)
                //    {
                //        nextCmd.Execute(this);
                //    }
                //    else
                //    {
                //        RaiseFailureEvent(failureArgs);
                //    }
                //}
            }
        }


        Queue<ICommand> cmdQueue;

        public Queue<ICommand> CmdQueue
        {
            get { return cmdQueue; }
            set { cmdQueue = value; }
        }

        private InterpolationResult interpolationResult;

        public InterpolationResult InterpolationResult
        {
            get { return interpolationResult; }
            set { interpolationResult = value; }
        }

        #endregion

        #region Events
        //private event Event onStateChanged;

        public event Event OnStateChanged;

        /// <summary>
        /// Triggered if the angle of an axis has changed. The specific axis can be found in the event arguments.
        /// </summary>
        /// 
        public event Event OnMovementStarted;

        public event Event OnToolUsed;

        public event Event OnHoming;

        public event Event OnAbort;

        public event Event OnShuttingDown;

        public event Event OnShutDown;

        public event Event OnFailure;
        #endregion

        public abstract void Initialize(object param);
        public abstract void Shutdown();
        public abstract void MoveStraightTo(object param);
        public abstract void MoveCircularTo(object param);
        public abstract void UseTool(object param);
        public abstract void ChangeConfiguration(object param);
        public abstract void Abort();
        public abstract bool IsStateUpdateAllowed();
        public abstract bool UsesIntegratedPathCalculation();

        #region ----------Collision Avoidance and Point Validation----------
        public bool IsPointValid(Point3D point)
        {
            double distance = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            bool isXYValid = distance > 0 && Math.Round(distance, 2) <= length + length2 && distance >= Math.Abs(length - length2);
            bool isZValid = Math.Round(point.Z, 4) >= 0 && Math.Round(point.Z,4) <= verticalToolRange;
            return isXYValid && isZValid;
        }

        public bool AreAnglesValid(float alpha1, float alpha2, float alpha3)
        {
            return alpha1 <= maxPrimaryAngle && alpha1 >= minPrimaryAngle && alpha2 <= maxSecondaryAngle && alpha2 >= minSecondaryAngle;
        }
        #endregion

        public void Execute(ICommand cmd) {
            if (cmd is AbortCommand)
            {
                cmd.Execute(this);
            }
            else
            {
                cmdQueue.Enqueue(cmd);
                CheckState();
            }
        }

        public State GetState() {
            return state;
        }

        private void UpdateState(State newState) {
            if (OnStateChanged != null)
            {
                OnStateChanged(this, new EventArgs.StateChangedEventArgs(state, newState));
            }
            state = newState;
            CheckState();
        }

        public void SetState(State newState) {
            if (IsStateUpdateAllowed())
            {
                UpdateState(newState);
            }
            else {
                RaiseFailureEvent(new FailureEventArgs(new Exception("Der Adapter erlaubt keine Updates von außen")));
            }
        }

        public void SetState(State newState, bool ignoreConditions)
        {
            if (ignoreConditions)
            {
                UpdateState(newState);
            }
            else {
                SetState(newState);
            }
        }

        private void CheckState()
        {
            if ((state == State.READY || state == State.SHUTDOWN) && cmdQueue.Count > 0)
            {
                ICommand nextCmd = cmdQueue.Dequeue();
                FailureEventArgs failureArgs = nextCmd.CanExecute(this);
                if (failureArgs == null)
                {
                    nextCmd.Execute(this);
                }
                else
                {
                    RaiseFailureEvent(failureArgs);
                }
            }
        }

        internal void RaiseFailureEvent(FailureEventArgs args) {
            cmdQueue.Clear();
            SetState(API.State.SHUTDOWN,true);
            if (OnFailure != null)
            {
                OnFailure(this, new FailureEventArgs(args.ThrownException));
            }
            else {
                throw args.ThrownException;
            }
        }


        internal void RaiseStateChangedEvent(StateChangedEventArgs args)
        {
            if (OnStateChanged != null)
            {
                OnStateChanged(this, args);
            }

        }

        internal void RaiseMovementStartedEvent(MovementStartedEventArgs args)
        {
            if(OnMovementStarted!=null)
            OnMovementStarted(this, args);
        }
        
        internal void RaiseHomingEvent(HomingEventArgs args)
        {
            if(OnHoming != null)
            OnHoming(this, args);
        }

        internal void RaiseShuttingDownEvent(System.EventArgs args)
        {
            State = State.SHUTTING_DOWN;
            if(OnShuttingDown!=null)
                OnShuttingDown(this, args);
        }

        internal void RaiseShutdownEvent(System.EventArgs args)
        {
            if(OnShutDown != null)
                OnShutDown(this, args);
        }

        internal void RaiseAbortEvent(System.EventArgs args)
        {
            if(OnAbort != null)
                OnAbort(this, args);
        }

        internal void RaiseToolUsed(ToolUsedEventArgs args)
        {
            if (OnToolUsed != null)
                OnToolUsed(this, args);            
        }
    }
}
