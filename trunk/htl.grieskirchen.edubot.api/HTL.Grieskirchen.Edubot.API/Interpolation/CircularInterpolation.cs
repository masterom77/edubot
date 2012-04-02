using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using HTL.Grieskirchen.Edubot.API.Exceptions;


namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    /// <summary>
    /// An implementation of the IInterpolationType interfaces, which uses linear interpolation for path calculation
    /// </summary>
    public class CircularInterpolation
    {
        /// <summary>
        /// dCalculates the path by using the specified parameters and linear interpolation.
        /// </summary>
        /// <param name="toolCenterPoint">A point, representing the current location of the tool</param>
        /// <param name="target">The target point of the movement</param>
        /// <param name="length">The length of the first axis</param>
        /// <param name="length2">The length of the second axis</param>
        /// <returns></returns>
        public InterpolationResult CalculatePath(Point3D toolCenterPoint, Point3D target, Point3D center, float length, float length2)
        {
            double difTargetCenterX = target.X - center.X;
            double difTargetCenterY = target.Y - center.Y;
            double difTargetCenter = Math.Sqrt(difTargetCenterX * difTargetCenterX + difTargetCenterY * difTargetCenterY);
            double difToolCenterX = toolCenterPoint.X - center.X;
            double difToolCenterY = toolCenterPoint.Y - center.Y;
            double difToolCenter = Math.Sqrt(difToolCenterX * difToolCenterX + difToolCenterY * difToolCenterY);

            if (difTargetCenter != difToolCenter)
                throw new MVCException("Ungültiger Mittelpunkt: Der Punkt Ausgangspunkt (" + toolCenterPoint.ToString() + ") und der Zielpunkt (" + target.ToString() + ") liegen nicht in angegeben Kreis", toolCenterPoint, target, center);

       
            double r = difToolCenter;
            double difToolTargetX = target.X - toolCenterPoint.X;
            double difToolTargetY = target.Y - toolCenterPoint.Y;
            double d = Math.Sqrt(difToolTargetX * difToolTargetX + difToolTargetY * difToolTargetY);



            double angle;
            if (d == 0)
            {
                angle = 360;
            }
            else
            {   
                angle = MathHelper.ConvertToDegrees(Math.Acos((2 * r * r - d * d) / (2 * r * r)));
                int quadrant = MathHelper.GetQuadrant((float)difTargetCenterX, (float)difTargetCenterY);
                if (quadrant == 3)
                    angle = 180 + angle;
            }
            
            double anglePerStep = 1;

            for (int i = 1; i <= (int)Math.Ceiling(angle / anglePerStep); i++)
            {
                d = Math.Sqrt(2 * r * r - 2 * r * r * Math.Cos(MathHelper.ConvertToRadians(i)));
                Console.WriteLine(d);
            }


            return null;
            
        }

      
        
    }
}
