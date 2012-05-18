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
    /// <summary>
    /// Represents an abstract class, which is used to execute commands and manage data of a robot
    /// </summary>
    public abstract class IAdapter
    {
        
        /// <summary>
        /// Initializes a new instance of the IAdapter for robots with RR-Kinematic without angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
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
        /// Initializes a new instance of the IAdapter for robots with  RR-Cinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="maxPrimaryAngle">The maximum angle, which the primary axis can adopt</param>
        /// <param name="minPrimaryAngle">The minimum angle, which the primary axis can adopt</param>
        /// <param name="maxSecondaryAngle">The maximum angle, which the secondary axis can adopt</param>
        /// <param name="minSecondaryAngle">The minimum angle, which the secondary axis can adopt</param>
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
        /// Initializes a new instance of the IAdapter for robots with RRT-Kinematic without angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="verticalToolRange">The distance between the toolcenter point and the working area, measured when the tool has adopted the highest position</param>
        /// <param name="transmission">The transmission, which defines how many degrees the third engine has to rotate, to move the tertiary axis down one unit</param>
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
        /// Initializes a new instance of the IAdapter for robots with RRT-Kinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="verticalToolRange">The distance between the toolcenter point and the working area, measured when the tool has adopted the highest position</param>
        /// <param name="transmission">The transmission, which defines how many degrees the third engine has to rotate, to move the tertiary axis down one unit</param>
        /// <param name="maxPrimaryAngle">The maximum angle, which the primary axis can adopt</param>
        /// <param name="minPrimaryAngle">The minimum angle, which the primary axis can adopt</param>
        /// <param name="maxSecondaryAngle">The maximum angle, which the secondary axis can adopt</param>
        /// <param name="minSecondaryAngle">The minimum angle, which the secondary axis can adopt</param>
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
        /// <summary>
        /// Gets or sets the equipped tool
        /// </summary>
        public Tool EquippedTool
        {
            get { return equippedTool; }
            set { equippedTool = value; }
        }

        protected Point3D toolCenterPoint;
        /// <summary>
        /// Gets the toolcenter point
        /// </summary>
        public Point3D ToolCenterPoint
        {
            get { return toolCenterPoint; }
        }

        /// <summary>
        /// Gets the home point
        /// </summary>
        public Point3D HomePoint {
            get { return new Point3D(Length + Length2, 0, 0); }
        }

        protected float length;
        /// <summary>
        /// Gets or sets the length of first axis, measured between the center of the primary and secondary engine
        /// </summary>
        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        protected float length2;
        /// <summary>
        /// Gets or sets the length of second axis, measured between the center of the secondary engine and the toolcenter point
        /// </summary>
        public float Length2
        {
            get { return length2; }
            set { length2 = value; }
        }

        protected float verticalToolRange;
        /// <summary>
        /// Gets or sets the distance between the toolcenter point and the working area, measured when the tool has adopted the highest position
        /// </summary>
        public float VerticalToolRange
        {
            get { return verticalToolRange; }
            set { verticalToolRange = value; }
        }

        protected float maxPrimaryAngle;
        /// <summary>
        /// Gets or sets the maximum angle, which the primary axis can adopt
        /// </summary>
        public float MaxPrimaryAngle
        {
            get { return maxPrimaryAngle; }
            set { maxPrimaryAngle = value; }
        }

        protected float minPrimaryAngle;
        /// <summary>
        /// Gets or sets the minimum angle, which the primary axis can adopt
        /// </summary>
        public float MinPrimaryAngle
        {
            get { return minPrimaryAngle; }
            set { minPrimaryAngle = value; }
        }

        protected float maxSecondaryAngle;
        /// <summary>
        /// Gets or sets the maximum angle, which the secondary axis can adopt
        /// </summary>
        public float MaxSecondaryAngle
        {
            get { return maxSecondaryAngle; }
            set { maxSecondaryAngle = value; }
        }

        protected float minSecondaryAngle;
        /// <summary>
        /// Gets or sets the minimum angle, which the secondary axis can adopt
        /// </summary>
        public float MinSecondaryAngle
        {
            get { return minSecondaryAngle; }
            set { minSecondaryAngle = value; }
        }

        protected float transmission;
        /// <summary>
        /// Gets or sets the transmission, which defines how many degrees the third engine has to rotate, to move the tertiary axis down one unit
        /// </summary>
        public float Transmission
        {
            get { return transmission; }
            set { transmission = value; }
        }

        protected AxisConfiguration axisConfiguration;
        /// <summary>
        /// Gets or sets the axis configuration
        /// </summary>
        public AxisConfiguration AxisConfiguration
        {
            get { return axisConfiguration; }
            set { axisConfiguration = value; }
        }

        protected State state;
        /// <summary>
        /// Gets or sets the state of the adapter
        /// </summary>
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
            }
        }


        protected Queue<ICommand> cmdQueue;
        /// <summary>
        /// Gets the command queue of the adapter
        /// </summary>
        public Queue<ICommand> CmdQueue
        {
            get { return cmdQueue; }
        }

        private InterpolationResult interpolationResult;
        /// <summary>
        /// Gets or sets the interpolation result, which is used to execute further instructions
        /// </summary>
        public InterpolationResult InterpolationResult
        {
            get { return interpolationResult; }
            set { interpolationResult = value; }
        }

        #endregion

        #region Events
        
        /// <summary>
        /// Triggered if the state of the robot has been changed
        /// </summary>
        public event Event OnStateChanged;

        /// <summary>
        /// Triggered if the robot is starting an arbitrary movement
        /// </summary>
        public event Event OnMovementStarted;

        /// <summary>
        /// Triggered if the robot is activating or deactiving the equipped tool
        /// </summary>
        public event Event OnToolUsed;

        /// <summary>
        /// Triggered if the robot starts homing
        /// </summary>
        public event Event OnHoming;

        /// <summary>
        /// Triggered if the robot should abort all current actions
        /// </summary>
        public event Event OnAbort;

        /// <summary>
        /// Triggered if the robot is shutting down
        /// </summary>
        public event Event OnShuttingDown;

        /// <summary>
        /// Triggered if the robot has been shut down
        /// </summary>
        public event Event OnShutDown;

        /// <summary>
        /// Triggered if any exception or failure occures
        /// </summary>
        public event Event OnFailure;
        #endregion

        /// <summary>
        /// Initializes the robot, so the position of the tool is at the home point.
        /// </summary>
        /// <param name="param">An object, which contains the correction angle as a float value</param>
        public abstract void Initialize(object param);
        /// <summary>
        /// Shuts the robot down
        /// </summary>
        public abstract void Shutdown();
        /// <summary>
        /// Starts a linear movement
        /// </summary>
        /// <param name="param">An object, which contains the target point as Point3D object</param>
        public abstract void MoveStraightTo(object param);
        /// <summary>
        /// Starts a circular movement
        /// </summary>
        /// <param name="param">An object, which contains the target and center point as Point3D array</param>
        public abstract void MoveCircularTo(object param);
        /// <summary>
        /// Activates or deactivates the tool
        /// </summary>
        /// <param name="param">An object, which contains a boolean value indicating whether tool should be activated or deactivated</param>
        public abstract void UseTool(object param);
        /// <summary>
        /// Changes the current axis configuration
        /// </summary>
        /// <param name="param">An object, which contains the new configuration as AxisConfiguration object</param>
        public abstract void ChangeConfiguration(object param);
        /// <summary>
        /// Aborts all current actions of the robot
        /// </summary>
        public abstract void Abort();
        /// <summary>
        /// Determines wether manual status updates are allowed or not
        /// </summary>
        /// <returns>A boolean value, which indicates wether manual status updates are allowed or not</returns>
        public abstract bool IsStateUpdateAllowed();
        /// <summary>
        /// Determines wether the kinematics and interpolation methods of the API should be used
        /// </summary>
        /// <returns>A boolean value, which indicates wether the kinematics and interpolation methods of the API should be used</returns>
        public abstract bool UsesIntegratedPathCalculation();

        #region ----------Collision Avoidance and Point Validation----------
        /// <summary>
        /// Determines wether a point is within reach of the robot or not
        /// </summary>
        /// <param name="point">The point which should be checked</param>
        /// <returns>A boolean value, which indicates wether the point is within the reach of the robot or not</returns>
        public bool IsPointValid(Point3D point)
        {
            double distance = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            bool isXYValid = distance > 0 && Math.Round(distance, 2) <= length + length2 && distance >= Math.Abs(length - length2);
            bool isZValid = Math.Round(point.Z, 4) >= 0 && Math.Round(point.Z,4) <= verticalToolRange;
            return isXYValid && isZValid;
        }

        /// <summary>
        /// Determines whether the given angles are adoptable by the robot or not
        /// </summary>
        /// <param name="alpha1">The angle of the primary axis</param>
        /// <param name="alpha2">The angle of the secondary axis</param>
        /// <param name="alpha3">The angle of the tertiary axis</param>
        /// <returns>A boolean value, which indicates whether the given angles are adoptable by the robot or not</returns>
        public bool AreAnglesValid(float alpha1, float alpha2, float alpha3)
        {
            return alpha1 <= maxPrimaryAngle && alpha1 >= minPrimaryAngle && alpha2 <= maxSecondaryAngle && alpha2 >= minSecondaryAngle;
        }
        #endregion

        /// <summary>
        /// Executes a command, by placing it in the command queue
        /// </summary>
        /// <param name="cmd">The command, which should be executed</param>
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

        /// <summary>
        /// Gets the current state of the robot
        /// </summary>
        /// <returns>The current state of the robot as State object</returns>
        public State GetState() {
            return state;
        }

        /// <summary>
        /// Updates the state of robot and triggers the OnStateChanged event
        /// </summary>
        /// <param name="newState">The new state of the robot</param>
        private void UpdateState(State newState) {
            if (OnStateChanged != null)
            {
                OnStateChanged(this, new EventArgs.StateChangedEventArgs(state, newState));
            }
            state = newState;
            CheckState();
        }

        /// <summary>
        /// Sets the current state of the robot
        /// </summary>
        /// <param name="newState">The new state of the robot</param>
        public void SetState(State newState) {
            if (IsStateUpdateAllowed())
            {
                UpdateState(newState);
            }
            else {
                RaiseFailureEvent(new FailureEventArgs(new Exception("Der Adapter erlaubt keine Updates von außen")));
            }
        }
        /// <summary>
        /// Sets the current state of the robot
        /// </summary>
        /// <param name="newState">The new state of the robot</param>
        /// <param name="ignoreConditions">A boolean value, indicating wether the conditions for manually updating the state should be ignored</param>
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

        /// <summary>
        /// Checks the current state and executes the next command if the state is READY or SHUTDOWN.
        /// </summary>
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

        /// <summary>
        /// Raises the OnFailureEvent of the robot
        /// </summary>
        /// <param name="args">The event arguments, which should be passed</param>
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

        /// <summary>
        /// Raises the OnStateChanged of the robot
        /// </summary>
        /// <param name="args">The event arguments, which should be passed</param>
        internal void RaiseStateChangedEvent(StateChangedEventArgs args)
        {
            if (OnStateChanged != null)
            {
                OnStateChanged(this, args);
            }

        }

        /// <summary>
        /// Raises the OnMovementStartedEvent of the robot
        /// </summary>
        /// <param name="args">The event arguments, which should be passed</param>
        internal void RaiseMovementStartedEvent(MovementStartedEventArgs args)
        {
            if(OnMovementStarted!=null)
            OnMovementStarted(this, args);
        }

        /// <summary>
        /// Raises the OnHomingEvent of the robot
        /// </summary>
        /// <param name="args">The event arguments, which should be passed</param>
        internal void RaiseHomingEvent(HomingEventArgs args)
        {
            if(OnHoming != null)
            OnHoming(this, args);
        }

        /// <summary>
        /// Raises the OnShuttingDownEvent of the robot
        /// </summary>
        /// <param name="args">The event arguments, which should be passed</param>
        internal void RaiseShuttingDownEvent(System.EventArgs args)
        {
            if(OnShuttingDown!=null)
                OnShuttingDown(this, args);
        }

        /// <summary>
        /// Raises the OnShutdownEvent of the robot
        /// </summary>
        /// <param name="args">The event arguments, which should be passed</param>
        internal void RaiseShutdownEvent(System.EventArgs args)
        {
            if(OnShutDown != null)
                OnShutDown(this, args);
        }

        /// <summary>
        /// Raises the OnAbortEvent of the robot
        /// </summary>
        /// <param name="args">The event arguments, which should be passed</param>
        internal void RaiseAbortEvent(System.EventArgs args)
        {
            if(OnAbort != null)
                OnAbort(this, args);
        }

        /// <summary>
        /// Raises the OnToolUsedEvent of the robot
        /// </summary>
        /// <param name="args">The event arguments, which should be passed</param>
        internal void RaiseToolUsed(ToolUsedEventArgs args)
        {
            if (OnToolUsed != null)
                OnToolUsed(this, args);            
        }
    }
}
