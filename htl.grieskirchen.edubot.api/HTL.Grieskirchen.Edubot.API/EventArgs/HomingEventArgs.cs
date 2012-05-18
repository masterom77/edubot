using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.EventArgs
{
    /// <summary>
    /// Used for passing details about the homing process
    /// </summary>
    public class HomingEventArgs : System.EventArgs
    {

        float correctionAngle;
        /// <summary>
        /// The angle by which the axes should be corrected in every step
        /// </summary>
        public float CorrectionAngle
        {
            get { return correctionAngle; }
            set { correctionAngle = value; }
        }

        /// <summary>
        /// Creates a new instance of the HomingEventArgs class using the given values
        /// </summary>
        /// <param name="correctionAngle">The angle by which the axes should be corrected in every step</param>
        public HomingEventArgs(float correctionAngle)
            : base()
        {
            this.correctionAngle = correctionAngle;
        }
    }
}
