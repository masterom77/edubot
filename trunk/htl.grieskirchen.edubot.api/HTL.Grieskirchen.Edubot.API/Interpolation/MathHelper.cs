using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public static class MathHelper
    {
        public static float ConvertToDegrees(double rad)
        {
            return (float)(rad * 180 / Math.PI);
        }

        public static float ConvertToRadians(double degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        public static long ConvertToTicks(float degrees) {
            return (long) (degrees / Configuration.AnglePerStep);
        }

        public static float ConvertToDegrees(long steps)
        {
            return steps * Configuration.AnglePerStep;
        }

        public static Point CalculateCoordinates(float alpha1, float alpha2, float length) {
            int x = (int) Math.Round(length * Math.Cos(ConvertToRadians(alpha1)) + length * Math.Cos(ConvertToRadians(alpha1 + alpha2)));
            int y = (int) Math.Round(length * Math.Sin(ConvertToRadians(alpha1)) + length * Math.Sin(ConvertToRadians(alpha1 + alpha2)));
            return new Point(x, y);
        }

        public static float CalculateDistance(float num1, float num2) {
            if (num2 > num1)
            {
                return num2 - num1;
            }
            else {
                return num1 - num2;
            }
        }

        public static int GetQuadrant(float x, float y)
        {
            if (x >= 0 && y >= 0)
                return 1;
            if (x <= 0 && y >= 0)
                return 2;
            if (x <= 0 && y <= 0)
                return 3;
            return 4;
        }
    }
}
