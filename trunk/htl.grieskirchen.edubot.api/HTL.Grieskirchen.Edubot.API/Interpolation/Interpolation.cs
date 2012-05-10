using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Exceptions;
using HTL.Grieskirchen.Edubot.API.Adapters;
using HTL.Grieskirchen.Edubot.API.EventArgs;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class Interpolation
    {
        /// <summary>
        /// Calculates the path by using the specified parameters and linear interpolation.
        /// </summary>
        /// <param name="toolCenterPoint">A point, representing the current location of the tool</param>
        /// <param name="target">The target point of the movement</param>
        /// <param name="length">The length of the first axis</param>
        /// <param name="length2">The length of the second axis</param>
        /// <returns></returns>
        //public static InterpolationResult InterpolateLinear(Point3D toolCenterPoint, Point3D target, float length, float length2)
        //{
            
        //    int steps;// = Configuration.InterpolationSteps;
        //    float toolX = toolCenterPoint.X;
        //    float toolY = toolCenterPoint.Y;
        //    float toolZ = toolCenterPoint.Z;

        //    float difX = target.X - toolX;
        //    float difY = target.Y - toolY;
        //    float difZ = target.Z - toolZ;
            
        //    float distance = (float)Math.Sqrt(difX * difX + difY * difY);
        //    steps = (int)Math.Ceiling(distance);
        //    //if (length == length2)
        //    //    steps = (int)Math.Round(distance, 0);
        //    //else
        //    //    steps = (int) (Math.Round(distance) + Math.Abs(length - length2));
        //    float incrX = difX / steps;
        //    float incrY = difY / steps;
        //    float incrZ = difZ / steps;

        //    InterpolationResult result = new InterpolationResult(InterpolationType.Linear);
        //    result.Points.Add(toolCenterPoint);
        //    InterpolationStep prevStep = Kinematics.CalculateInverse(new Point3D(toolX, toolY, 0), length, length2);
        //    InterpolationStep step;

        //    for (int i = 0; i < steps; i++)
        //    {

        //        toolX += incrX;
        //        toolY += incrY;
        //        toolZ += incrZ;
        //        step = Kinematics.CalculateInverse(new Point3D(toolX, toolY, toolZ), length, length2);
        //        result.Points.Add(new Point3D(toolX,toolY,toolZ));
        //        result.Angles.Add(step);
        //        result.Steps.Add(step - prevStep);
        //        prevStep = step;
        //    }

        //    return result;

        //}

        //public static InterpolationResult InterpolateLinear(IAdapter adapter, Point3D target)
        //{
        //    if(!adapter.IsPointValid(target)){
        //        adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zielpunkt (" + target.ToString() + ") liegt nicht im Arbeitsbereich des Roboters")));
        //    return null;
        //    }

        //    int steps;// = Configuration.InterpolationSteps;
        //    float toolX = adapter.ToolCenterPoint.X;
        //    float toolY = adapter.ToolCenterPoint.Y;
        //    float toolZ = adapter.ToolCenterPoint.Z;

        //    float difX = target.X - adapter.ToolCenterPoint.X;
        //    float difY = target.Y - adapter.ToolCenterPoint.Y;
        //    float difZ = target.Z - adapter.ToolCenterPoint.Z;

        //    float distance = (float)Math.Sqrt(difX * difX + difY * difY);
        //    steps = (int)Math.Ceiling(distance);

        //    float incrX = difX / steps;
        //    float incrY = difY / steps;
        //    float incrZ = difZ / steps;

        //    InterpolationResult result = new InterpolationResult(InterpolationType.Linear);
        //    result.Points.Add(adapter.ToolCenterPoint);
        //    InterpolationStep prevStep = Kinematics.CalculateInverse(adapter.ToolCenterPoint, adapter.Length, adapter.Length2, adapter.Transmission);
        //    InterpolationStep step = null;
        //    Point3D nextPoint;
        //    for (int i = 0; i < steps; i++)
        //    {

        //        nextPoint = new Point3D(adapter.ToolCenterPoint.X + (i + 1) * incrX, adapter.ToolCenterPoint.Y + (i + 1) * incrY, adapter.ToolCenterPoint.Z + (i + 1) * incrZ);
                
        //        if (!adapter.IsPointValid(nextPoint))
        //        {
        //            adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(nextPoint, "Der Zwischenpunkt (" + nextPoint.ToString() + ") liegt nicht im Arbeitsbereich des Roboters")));
        //            return null;
        //        }

        //        step = Kinematics.CalculateInverse(nextPoint, adapter.Length, adapter.Length2,adapter.Transmission);
                
        //        if (!adapter.AreAnglesValid(step.Alpha1, step.Alpha2, step.Alpha3))
        //        {
        //            adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(nextPoint, "Einer der berechneten Winkel (" + step.Alpha1 + "°/" + step.Alpha2 + "°/" + step.Alpha3 + "°) unterliegt nicht den angegebenen Winkeleinschränkungen (" + adapter.MinPrimaryAngle + "° >= Alpha1 <= " + adapter.MaxPrimaryAngle + "°/" + adapter.MinSecondaryAngle + "° >= Alpha2 <=" + adapter.MaxSecondaryAngle + "°/0° >= Alpha3 <= " + adapter.VerticalToolRange * adapter.Transmission + "°)")));
        //            return null;
        //        }

        //        result.Points.Add(new Point3D(toolX, toolY, toolZ));
        //        result.Angles.Add(step);
        //        result.Steps.Add(step - prevStep);
        //        prevStep = step;
        //    }

        //    return result;

        //}

        public static InterpolationResult InterpolateLinear(IAdapter adapter, Point3D target)
        {
            if (!adapter.IsPointValid(target))
            {
                adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zielpunkt (" + target.ToString() + ") liegt nicht im Arbeitsbereich des Roboters")));
                return null;
            }
            InterpolationStep[] kinResult = Kinematics.CalculateInverse(target,adapter.Length, adapter.Length2, adapter.VerticalToolRange, adapter.Transmission);
            if (!adapter.AreAnglesValid(kinResult[(int)adapter.AxisConfiguration].Alpha1, kinResult[(int)adapter.AxisConfiguration].Alpha2, kinResult[(int)adapter.AxisConfiguration].Alpha3))
            {
                AxisConfiguration otherConfig;
                if (adapter.AxisConfiguration == AxisConfiguration.Lefty)
                {
                    otherConfig = AxisConfiguration.Righty;
                }
                else
                {
                    otherConfig = AxisConfiguration.Lefty;
                }

                if (!adapter.AreAnglesValid(kinResult[(int)otherConfig].Alpha1, kinResult[(int)otherConfig].Alpha2, kinResult[(int)otherConfig].Alpha3))
                {
                    adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zielpunkt " + target.ToString() + " kann aufgrund der angegebenen Winkeleinschränkungen mit keiner der vorhandenen Achskonfigurationen erreicht werden.")));
                }
                else
                {
                    adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zielpunkt (" + target.ToString() + ") kann mit der aktuellen Achskonfiguration " + adapter.AxisConfiguration.ToString() + " nicht erreicht werden.")));
                }
                return null;
            }

            InterpolationResult result = new InterpolationResult(InterpolationType.Linear);
            result.Points.Add(adapter.ToolCenterPoint);

            int steps;// = Configuration.InterpolationSteps;
            float toolX = adapter.ToolCenterPoint.X;
            float toolY = adapter.ToolCenterPoint.Y;
            float toolZ = adapter.ToolCenterPoint.Z;

            float difX = target.X - adapter.ToolCenterPoint.X;
            float difY = target.Y - adapter.ToolCenterPoint.Y;
            float difZ = target.Z - adapter.ToolCenterPoint.Z;

            float distance = (float)Math.Sqrt(difX * difX + difY * difY);
            steps = (int)Math.Ceiling(distance);

            float incrX = difX / steps;
            float incrY = difY / steps;
            float incrZ = difZ / steps;

            
            InterpolationStep prevStep = kinResult[(int)adapter.AxisConfiguration];
            InterpolationStep step = null;
            Point3D nextPoint;
            for (int i = 0; i < steps; i++)
            {

                nextPoint = new Point3D(adapter.ToolCenterPoint.X + (i + 1) * incrX, adapter.ToolCenterPoint.Y + (i + 1) * incrY, adapter.ToolCenterPoint.Z + (i + 1) * incrZ);

                if (!adapter.IsPointValid(nextPoint))
                {
                    adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(nextPoint, "Der Zwischenpunkt (" + nextPoint.ToString() + ") liegt nicht im Arbeitsbereich des Roboters")));
                    return null;
                }

                kinResult = Kinematics.CalculateInverse(nextPoint, adapter.Length, adapter.Length2, adapter.VerticalToolRange, adapter.Transmission);
                step = kinResult[(int)adapter.AxisConfiguration];

                if (!adapter.AreAnglesValid(step.Alpha1, step.Alpha2, step.Alpha3))
                {
                    AxisConfiguration otherConfig;
                    if (adapter.AxisConfiguration == AxisConfiguration.Lefty)
                    {
                        otherConfig = AxisConfiguration.Righty;
                    }
                    else
                    {
                        otherConfig = AxisConfiguration.Lefty;
                    }

                    if (!adapter.AreAnglesValid(kinResult[(int)otherConfig].Alpha1, kinResult[(int)otherConfig].Alpha2, kinResult[(int)otherConfig].Alpha3))
                    {
                        adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zwischenpunkt " + target.ToString() + " kann aufgrund der angegebenen Winkeleinschränkungen mit keiner der vorhandenen Achskonfigurationen erreicht werden.")));
                    }
                    else
                    {
                        adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zwischenpunkt (" + target.ToString() + ") kann mit der aktuellen Achskonfiguration " + adapter.AxisConfiguration.ToString() + " nicht erreicht werden.")));
                    }
                    return null;
                }

                result.Points.Add(new Point3D(toolX, toolY, toolZ));
                result.Angles.Add(step);
                result.Steps.Add(step - prevStep);
                prevStep = step;
            }

            return result;

        }

        public static InterpolationResult InterpolateConfigurationChange(IAdapter adapter, AxisConfiguration newMode) {
            InterpolationStep[] kinResult = Kinematics.CalculateInverse(adapter.ToolCenterPoint, adapter.Length, adapter.Length2, adapter.VerticalToolRange, adapter.Transmission);

            if (!adapter.AreAnglesValid(kinResult[(int)newMode].Alpha1, kinResult[(int)newMode].Alpha2, kinResult[(int)newMode].Alpha3))
            {
                adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(adapter.ToolCenterPoint, "Interpolation: Ein Wechsel der Konfiguration ist aufgrund der angegebenen Winkeleinschränkungen an diesem Punkt nicht möglich.(" + kinResult[(int)newMode].Alpha1 + "°/" + kinResult[(int)newMode].Alpha2 + "°/" + kinResult[(int)newMode].Alpha3 + "°) unterliegt nicht den angegebenen Winkeleinschränkungen (" + adapter.MinPrimaryAngle + "° >= Alpha1 <= " + adapter.MaxPrimaryAngle + "°/" + adapter.MinSecondaryAngle + "° >= Alpha2 <=" + adapter.MaxSecondaryAngle + "°/0° >= Alpha3 <= " + adapter.VerticalToolRange * adapter.Transmission + "°)")));
                return null;
            }

            float difAlpha1 = kinResult[(int)newMode].Alpha1 - kinResult[(int)adapter.AxisConfiguration].Alpha1;
            float difAlpha2 = kinResult[(int)newMode].Alpha2 - kinResult[(int)adapter.AxisConfiguration].Alpha2;
            int steps = (int)Math.Ceiling(new float[] { difAlpha1, difAlpha2 }.Max()*10);
            float incrAlpha1 = difAlpha1 / steps;
            float incrAlpha2 = difAlpha2 / steps;
            InterpolationResult result = new InterpolationResult(InterpolationType.None);
            InterpolationStep prevStep = kinResult[(int)adapter.AxisConfiguration];
            InterpolationStep step;
            float alpha1;
            float alpha2;
            for (int i = 0; i < steps; i++) {
                alpha1 = kinResult[(int)adapter.AxisConfiguration].Alpha1 + (i + 1) * incrAlpha1;
                alpha2 = kinResult[(int)adapter.AxisConfiguration].Alpha2 + (i + 1) * incrAlpha2;
                step = new InterpolationStep(Kinematics.CalculateDirect(alpha1, alpha2, 0,adapter.Length, adapter.Length2,0,0), alpha1, alpha2, 0);
                result.Angles.Add(step);
                result.Steps.Add(step - prevStep);
                result.Points.Add(step.Target);
            }
            return result;
        }


        ///// <summary>
        ///// dCalculates the path by using the specified parameters and linear interpolation.
        ///// </summary>
        ///// <param name="toolCenterPoint">A point, representing the current location of the tool</param>
        ///// <param name="target">The target point of the movement</param>
        ///// <param name="length">The length of the first axis</param>
        ///// <param name="length2">The length of the second axis</param>
        ///// <returns></returns>
        //public static InterpolationResult InterpolateCircular(Point3D toolCenterPoint, Point3D target, Point3D center, float length, float length2)
        //{
        //    double difTargetCenterX = target.X - center.X;
        //    double difTargetCenterY = target.Y - center.Y;
        //    double difTargetCenter = Math.Sqrt(difTargetCenterX * difTargetCenterX + difTargetCenterY * difTargetCenterY);
        //    double difToolCenterX = toolCenterPoint.X - center.X;
        //    double difToolCenterY = toolCenterPoint.Y - center.Y;
        //    double difToolCenter = Math.Sqrt(difToolCenterX * difToolCenterX + difToolCenterY * difToolCenterY);

        //    if (difTargetCenter != difToolCenter)
        //        throw new InvalidCenterPointException(toolCenterPoint, target, center,"Ungültiger Mittelpunkt: Der Punkt Ausgangspunkt (" + toolCenterPoint.ToString() + ") und der Zielpunkt (" + target.ToString() + ") liegen nicht im angegeben Kreis mit Mittelpunkt("+center.ToString()+").");

       
        //    double r = difToolCenter;
        //    double difToolTargetX = target.X - toolCenterPoint.X;
        //    double difToolTargetY = target.Y - toolCenterPoint.Y;
        //    double d = Math.Sqrt(difToolTargetX * difToolTargetX + difToolTargetY * difToolTargetY);



        //    double angle;
              
        //        angle = MathHelper.ConvertToDegrees(Math.Acos(1-(d*d/(2*r*r))));
        //        int quadrant = MathHelper.GetQuadrant((float)difTargetCenterX, (float)difTargetCenterY);
        //        if (angle != 180 && quadrant == 3 || quadrant==4)
        //            angle = -angle;

            
            
           

        //    d = 2 * r * Math.PI * Math.Abs(angle) / 360;
        //    quadrant = MathHelper.GetQuadrant((float)difToolCenterX, (float)difToolCenterY);
        //    double startingAngle = MathHelper.ConvertToDegrees(Math.Acos(difToolCenterX / r));
        //    if (quadrant == 3 || quadrant == 4)
        //        startingAngle = 180 + startingAngle;
        //    double anglePerStep = angle/d;


        //    InterpolationResult result = new InterpolationResult(InterpolationType.Circular);
        //    if (angle < 0)
        //    {
        //        result.MetaData["Clockwise"] = true;
        //    }
        //    else
        //    {
        //        result.MetaData["Clockwise"] = false;
        //    }
        //    InterpolationStep prevStep = Kinematics.CalculateInverse(toolCenterPoint, length, length2);
            
        //    for (int i = 1; i <= d; i++)
        //    {
        //        double x, y;
        //        x = center.X + r * Math.Cos(MathHelper.ConvertToRadians(i * anglePerStep + startingAngle));
        //        y = center.Y + r * Math.Sin(MathHelper.ConvertToRadians( i * anglePerStep + startingAngle));
        //        InterpolationStep step = Kinematics.CalculateInverse(new Point3D((float)x, (float)y, 0), length, length2);
                
        //        result.Angles.Add(step);
        //        result.Steps.Add(step - prevStep);
        //    }




        //    return result;
            
        //}

        //public static InterpolationResult InterpolateCircular(IAdapter adapter, Point3D target, Point3D center)
        //{
        //    double difTargetCenterX = target.X - center.X;
        //    double difTargetCenterY = target.Y - center.Y;
        //    double difTargetCenter = Math.Sqrt(difTargetCenterX * difTargetCenterX + difTargetCenterY * difTargetCenterY);
        //    double difToolCenterX = adapter.ToolCenterPoint.X - center.X;
        //    double difToolCenterY = adapter.ToolCenterPoint.Y - center.Y;
        //    double difToolCenter = Math.Sqrt(difToolCenterX * difToolCenterX + difToolCenterY * difToolCenterY);
        //    double difZ = target.Z - center.Z;

        //    if (difTargetCenter != difToolCenter)
        //    {
        //        adapter.RaiseFailureEvent(new FailureEventArgs(new InvalidCenterPointException(adapter.ToolCenterPoint, target, center, "Ungültiger Mittelpunkt: Der Punkt Ausgangspunkt (" + adapter.ToolCenterPoint.ToString() + ") und der Zielpunkt (" + target.ToString() + ") liegen nicht im angegeben Kreis mit Mittelpunkt(" + center.ToString() + ").")));
        //        return null;
        //    }


        //    double r = difToolCenter;
        //    double difToolTargetX = target.X - adapter.ToolCenterPoint.X;
        //    double difToolTargetY = target.Y - adapter.ToolCenterPoint.Y;
        //    double d = Math.Sqrt(difToolTargetX * difToolTargetX + difToolTargetY * difToolTargetY);

        //    double angle;

        //    angle = MathHelper.ConvertToDegrees(Math.Acos(1 - (d * d / (2 * r * r))));
            
        //    int targetQuadrant = MathHelper.GetQuadrant((float)difTargetCenterX, (float)difTargetCenterY);
        //    double endAngle = MathHelper.ConvertToDegrees(Math.Acos(difTargetCenterX / r));
        //    if (targetQuadrant == 3 || targetQuadrant == 4)
        //        endAngle = -endAngle;  
            
        //    //Direction
            
        //    int startingQuadrant = MathHelper.GetQuadrant((float)difToolCenterX, (float)difToolCenterY);
        //    double startingAngle = MathHelper.ConvertToDegrees(Math.Acos(difToolCenterX / r));
        //    if (startingQuadrant == 3 || startingQuadrant == 4)
        //        startingAngle = -startingAngle;

        //    angle = endAngle - startingAngle;
        //    if (Math.Abs(angle) == 180) {
        //        angle = 180;
        //    }
        //    if (Math.Abs(angle) > 180) {
        //        angle = -(360 - Math.Abs(angle));
        //    }
        //    double s = 2 * r * Math.PI * Math.Abs(angle) / 360;
            
        //    double anglePerStep = angle / Math.Ceiling(s);
        //    double incrZ = difZ / Math.Ceiling(s);


        //    InterpolationResult result = new InterpolationResult(InterpolationType.Circular);
        //    if (angle < 0)
        //    {
        //        result.MetaData["Clockwise"] = true;
        //    }
        //    else {
        //        result.MetaData["Clockwise"] = false;
        //    }
        //    result.MetaData["Radius"] = r;
        //    result.Points.Add(adapter.ToolCenterPoint);
        //    InterpolationStep prevStep = Kinematics.CalculateInverse(adapter.ToolCenterPoint, adapter.Length, adapter.Length2,adapter.Transmission);
        //    Point3D nextPoint;
        //    for (int i = 1; i <= Math.Ceiling(s)+1; i++)
        //    {
        //        double x, y,z;
        //        x = center.X + r * Math.Cos(MathHelper.ConvertToRadians(i * anglePerStep + startingAngle));
        //        y = center.Y + r * Math.Sin(MathHelper.ConvertToRadians(i * anglePerStep + startingAngle));
        //        z = adapter.ToolCenterPoint.Z + i * incrZ;
        //        nextPoint = new Point3D((float)x,(float) y,(float) z);
        //        if (!adapter.IsPointValid(nextPoint))
        //        {
        //            adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(nextPoint, "Der Zwischenpunkt (" + nextPoint.ToString() + ") liegt nicht im Arbeitsbereich des Roboters")));
        //            return null;
        //        }

        //        InterpolationStep step = Kinematics.CalculateInverse(new Point3D((float)x, (float)y, (float)z), adapter.Length, adapter.Length2);

        //        if (!adapter.AreAnglesValid(step.Alpha1, step.Alpha2, step.Alpha3))
        //        {
        //            adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(nextPoint, "Einer der berechneten Winkel (" + step.Alpha1 + "°/" + step.Alpha2 + "°/" + step.Alpha3 + "°) unterliegt nicht den angegebenen Winkeleinschränkungen (" + adapter.MinPrimaryAngle + "° >= Alpha1 <= " + adapter.MaxPrimaryAngle + "°/" + adapter.MinSecondaryAngle + "° >= Alpha2 <=" + adapter.MaxSecondaryAngle + "°/0° >= Alpha3 <= " + adapter.VerticalToolRange * adapter.Transmission + "°)")));
        //            return null;
        //        }

                
        //        result.Points.Add(nextPoint);
        //        result.Angles.Add(step);
        //        result.Steps.Add(step - prevStep);
                
        //    }




        //    return result;

        //}

        public static InterpolationResult InterpolateCircular(IAdapter adapter, Point3D target, Point3D center)
        {
            if (!adapter.IsPointValid(target))
            {
                adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zielpunkt (" + target.ToString() + ") liegt nicht im Arbeitsbereich des Roboters")));
                return null;
            }
            InterpolationStep[] kinResult = Kinematics.CalculateInverse(target, adapter.Length, adapter.Length2, adapter.VerticalToolRange, adapter.Transmission);
            if (!adapter.AreAnglesValid(kinResult[(int)adapter.AxisConfiguration].Alpha1, kinResult[(int)adapter.AxisConfiguration].Alpha2, kinResult[(int)adapter.AxisConfiguration].Alpha3))
            {
                AxisConfiguration otherConfig;
                if (adapter.AxisConfiguration == AxisConfiguration.Lefty)
                {
                    otherConfig = AxisConfiguration.Righty;
                }
                else
                {
                    otherConfig = AxisConfiguration.Lefty;
                }

                if (!adapter.AreAnglesValid(kinResult[(int)otherConfig].Alpha1, kinResult[(int)otherConfig].Alpha2, kinResult[(int)otherConfig].Alpha3))
                {
                    adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zielpunkt " + target.ToString() + " kann aufgrund der angegebenen Winkeleinschränkungen mit keiner der vorhandenen Achskonfigurationen erreicht werden.")));
                }
                else
                {
                    adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zielpunkt (" + target.ToString() + ") kann mit der aktuellen Achskonfiguration " + adapter.AxisConfiguration.ToString() + " nicht erreicht werden.")));
                }
                return null;
            }


            double difTargetCenterX = target.X - center.X;
            double difTargetCenterY = target.Y - center.Y;
            double difTargetCenter = Math.Sqrt(difTargetCenterX * difTargetCenterX + difTargetCenterY * difTargetCenterY);
            double difToolCenterX = adapter.ToolCenterPoint.X - center.X;
            double difToolCenterY = adapter.ToolCenterPoint.Y - center.Y;
            double difToolCenter = Math.Sqrt(difToolCenterX * difToolCenterX + difToolCenterY * difToolCenterY);
            double difZ = target.Z - center.Z;

            if (difTargetCenter != difToolCenter)
            {
                adapter.RaiseFailureEvent(new FailureEventArgs(new InvalidCenterPointException(adapter.ToolCenterPoint, target, center, "Ungültiger Mittelpunkt: Der Punkt Ausgangspunkt (" + adapter.ToolCenterPoint.ToString() + ") und der Zielpunkt (" + target.ToString() + ") liegen nicht im angegeben Kreis mit Mittelpunkt(" + center.ToString() + ").")));
                return null;
            }


            double r = difToolCenter;
            double difToolTargetX = target.X - adapter.ToolCenterPoint.X;
            double difToolTargetY = target.Y - adapter.ToolCenterPoint.Y;
            double d = Math.Sqrt(difToolTargetX * difToolTargetX + difToolTargetY * difToolTargetY);

            double angle;

            angle = MathHelper.ConvertToDegrees(Math.Acos(1 - (d * d / (2 * r * r))));

            int targetQuadrant = MathHelper.GetQuadrant((float)difTargetCenterX, (float)difTargetCenterY);
            double endAngle = MathHelper.ConvertToDegrees(Math.Acos(difTargetCenterX / r));
            if (targetQuadrant == 3 || targetQuadrant == 4)
                endAngle = -endAngle;

            //Direction

            int startingQuadrant = MathHelper.GetQuadrant((float)difToolCenterX, (float)difToolCenterY);
            double startingAngle = MathHelper.ConvertToDegrees(Math.Acos(difToolCenterX / r));
            if (startingQuadrant == 3 || startingQuadrant == 4)
                startingAngle = -startingAngle;

            angle = endAngle - startingAngle;
            if (Math.Abs(angle) == 180)
            {
                angle = 180;
            }
            if (Math.Abs(angle) > 180)
            {
                angle = -(360 - Math.Abs(angle));
            }
            double s = 2 * r * Math.PI * Math.Abs(angle) / 360;

            double anglePerStep = angle / Math.Ceiling(s);
            double incrZ = difZ / Math.Ceiling(s);


            InterpolationResult result = new InterpolationResult(InterpolationType.Circular);
            if (angle < 0)
            {
                result.MetaData["Clockwise"] = true;
            }
            else
            {
                result.MetaData["Clockwise"] = false;
            }
            result.MetaData["Radius"] = r;
            result.Points.Add(adapter.ToolCenterPoint);
            InterpolationStep prevStep = kinResult[(int)adapter.AxisConfiguration];
            Point3D nextPoint;
            InterpolationStep step;
            for (int i = 1; i <= Math.Ceiling(s) + 1; i++)
            {
                double x, y, z;
                x = center.X + r * Math.Cos(MathHelper.ConvertToRadians(i * anglePerStep + startingAngle));
                y = center.Y + r * Math.Sin(MathHelper.ConvertToRadians(i * anglePerStep + startingAngle));
                z = adapter.ToolCenterPoint.Z + i * incrZ;
                nextPoint = new Point3D((float)x, (float)y, (float)z);
                
                if (!adapter.IsPointValid(nextPoint))
                {
                    adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(nextPoint, "Der Zwischenpunkt (" + nextPoint.ToString() + ") liegt nicht im Arbeitsbereich des Roboters")));
                    return null;
                }

                kinResult = Kinematics.CalculateInverse(nextPoint, adapter.Length, adapter.Length2, adapter.VerticalToolRange, adapter.Transmission);
                step = kinResult[(int)adapter.AxisConfiguration];

                if (!adapter.AreAnglesValid(step.Alpha1, step.Alpha2, step.Alpha3))
                {

                    AxisConfiguration otherConfig;
                    if (adapter.AxisConfiguration == AxisConfiguration.Lefty)
                    {
                        otherConfig = AxisConfiguration.Righty;
                    }
                    else
                    {
                        otherConfig = AxisConfiguration.Lefty;
                    }

                    if (!adapter.AreAnglesValid(kinResult[(int)otherConfig].Alpha1, kinResult[(int)otherConfig].Alpha2, kinResult[(int)otherConfig].Alpha3))
                    {
                        adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zwischenpunkt " + target.ToString() + " kann aufgrund der angegebenen Winkeleinschränkungen mit keiner der vorhandenen Achskonfigurationen erreicht werden.")));
                    }
                    else
                    {
                        adapter.RaiseFailureEvent(new EventArgs.FailureEventArgs(new PointOutOfRangeException(target, "Der Zwischenpunkt (" + target.ToString() + ") kann mit der aktuellen Achskonfiguration " + adapter.AxisConfiguration.ToString() + " nicht erreicht werden.")));
                    }
                    return null;
                }


                result.Points.Add(nextPoint);
                result.Angles.Add(step);
                result.Steps.Add(step - prevStep);

            }




            return result;

        }
    }
}
