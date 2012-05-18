using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;
using HTL.Grieskirchen.Edubot.API.Adapters.Listeners;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    /// <summary>
    /// Used for interaction with a virtual robot
    /// </summary>
    public class VirtualAdapter : IAdapter
    {
        /// <summary>
        /// Initializes a new instance of the VirtualAdapter using the values of the given adapter
        /// </summary>
        /// <param name="adapter">The adapter, whose values should be used</param>
        public VirtualAdapter(IAdapter adapter)
            : base(adapter.EquippedTool, adapter.Length, adapter.Length2, adapter.VerticalToolRange, adapter.Transmission, adapter.MaxPrimaryAngle, adapter.MinPrimaryAngle, adapter.MaxPrimaryAngle, adapter.MinSecondaryAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the VirtualAdapter for robots with RR-Kinematic without angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        public VirtualAdapter(Tool equippedTool, float length, float length2)
            : base(equippedTool, length, length2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the VirtualAdapter for robots with RRT-Kinematic without angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="verticalToolRange">The distance between the toolcenter point and the working area, measured when the tool has adopted the highest position</param>
        /// <param name="transmission">The transmission, which defines how many degrees the third engine has to rotate, to move the tertiary axis down one unit</param>
        public VirtualAdapter(Tool equippedTool, float length, float length2, float verticalToolRange, float transmission)
            : base(equippedTool, length, length2, verticalToolRange, transmission)
        {
        }

        /// <summary>
        /// Initializes a new instance of the VirtualAdapter for robots with  RR-Cinematic with angle restrictions
        /// </summary>
        /// <param name="equippedTool">The equipped tool of the robot</param>
        /// <param name="length">The length of first axis, measured between the center of the primary and secondary engine</param>
        /// <param name="length2">The length of second axis, measured between the center of the secondary engine and the toolcenter point</param>
        /// <param name="maxPrimaryAngle">The maximum angle, which the primary axis can adopt</param>
        /// <param name="minPrimaryAngle">The minimum angle, which the primary axis can adopt</param>
        /// <param name="maxSecondaryAngle">The maximum angle, which the secondary axis can adopt</param>
        /// <param name="minSecondaryAngle">The minimum angle, which the secondary axis can adopt</param>
        public VirtualAdapter(Tool equippedTool, float length, float length2, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle)
            : base(equippedTool, length, length2, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the VirtualAdapter for robots with RRT-Kinematic with angle restrictions
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
        public VirtualAdapter(Tool equippedTool, float length, float length2, float verticalToolRange, int transmission, float maxPrimaryAngle, float minPrimaryAngle, float maxSecondaryAngle, float minSecondaryAngle)
            : base(equippedTool, length, length2, verticalToolRange, transmission, maxPrimaryAngle, minPrimaryAngle, maxSecondaryAngle, minSecondaryAngle)
        {
        }

        /// <summary>
        /// Starts a linear movement
        /// </summary>
        /// <param name="param">An object, which contains the target point as Point3D object</param>
        public override void MoveStraightTo(object param)
        {
            Point3D target = (Point3D)param;
            toolCenterPoint = target;
        }

        /// <summary>
        /// Starts a circular movement
        /// </summary>
        /// <param name="param">An object, which contains the target and center point as Point3D array</param>
        public override void MoveCircularTo(object param)
        {
            Point3D[] parameters = (Point3D[])param;
            Point3D target = parameters[0];
            Point3D center = parameters[1];
            toolCenterPoint = target;
        }

        /// <summary>
        /// Activates or deactivates the tool
        /// </summary>
        /// <param name="param">An object, which contains a boolean value indicating whether tool should be activated or deactivated</param>
        public override void UseTool(object param)
        {
        }

        // <summary>
        /// Initializes the robot, so the position of the tool is at the home point.
        /// </summary>
        /// <param name="param">An object, which contains the correction angle as a float value</param>
        public override void Initialize(object param)
        {
            toolCenterPoint = new Point3D(length + length2, 0, verticalToolRange);
            //State = State.READY;
        }

        /// <summary>
        /// Shuts the robot down
        /// </summary>
        public override void Shutdown()
        {
        }

        /// <summary>
        /// Aborts all current actions of the robot
        /// </summary>
        public override void Abort()
        {
        }

        /// <summary>
        /// Determines wether manual status updates are allowed or not
        /// </summary>
        /// <returns>A boolean value, which indicates wether manual status updates are allowed or not</returns>
        public override bool IsStateUpdateAllowed()
        {
            return true;
        }

        /// <summary>
        /// Determines wether the kinematics and interpolation methods of the API should be used
        /// </summary>
        /// <returns>A boolean value, which indicates wether the kinematics and interpolation methods of the API should be used</returns>
        public override bool UsesIntegratedPathCalculation()
        {
            return true;
        }

        // <summary>
        /// Changes the current axis configuration
        /// </summary>
        /// <param name="param">An object, which contains the new configuration as AxisConfiguration object</param>
        public override void ChangeConfiguration(object param)
        {
            
        }
    }
}
