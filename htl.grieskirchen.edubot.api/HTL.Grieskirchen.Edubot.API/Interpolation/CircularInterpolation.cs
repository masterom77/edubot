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
                throw new MVCException("Ungültiger Mittelpunkt: Der Punkt Ausgangspunkt (" + toolCenterPoint.ToString() + ") und der Zielpunkt (" + target.ToString() + ") liegen nicht im angegeben Kreis mit Mittelpunkt("+center.ToString()+").", toolCenterPoint, target, center);

       
            double r = difToolCenter;
            double difToolTargetX = target.X - toolCenterPoint.X;
            double difToolTargetY = target.Y - toolCenterPoint.Y;
            double d = Math.Sqrt(difToolTargetX * difToolTargetX + difToolTargetY * difToolTargetY);



            double angle;
              
                angle = MathHelper.ConvertToDegrees(Math.Acos(1-(d*d/(2*r*r))));
                int quadrant = MathHelper.GetQuadrant((float)difTargetCenterX, (float)difTargetCenterY);
                if (angle != 180 && quadrant == 3 || quadrant==4)
                    angle = -angle;

            
            
           

            d = 2 * r * Math.PI * Math.Abs(angle) / 360;
            quadrant = MathHelper.GetQuadrant((float)difToolCenterX, (float)difToolCenterY);
            double startingAngle = MathHelper.ConvertToDegrees(Math.Acos(difToolCenterX / r));
            if (quadrant == 3 || quadrant == 4)
                startingAngle = 180 + startingAngle;
            double anglePerStep = angle/d;


            InterpolationResult result = new InterpolationResult();
            InterpolationStep prevStep = Kinematics.CalculateInverse(toolCenterPoint, length, length2);
            
            for (int i = 1; i <= d; i++)
            {
                double x, y;
                x = center.X + r * Math.Cos(MathHelper.ConvertToRadians(i * anglePerStep + startingAngle));
                y = center.Y + r * Math.Sin(MathHelper.ConvertToRadians( i * anglePerStep + startingAngle));
                InterpolationStep step = Kinematics.CalculateInverse(new Point3D((float)x, (float)y, 0), length, length2);
                
                result.Angles.Add(step);
                result.Steps.Add(step - prevStep);
            }




            return result;
            
        }

      
        
    }
}
