﻿using System;
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
            return (long) degrees * 6400;
        }

        public static float ConvertToDegrees(long ticks)
        {
            return ticks / 6400;
        }

        public static Point CalculateCoordinates(float alpha1, float alpha2, float length) {
            int x = (int) Math.Round(length * Math.Cos(ConvertToRadians(alpha1)) + length * Math.Cos(ConvertToRadians(alpha1 + alpha2)));
            int y = (int) Math.Round(length * Math.Sin(ConvertToRadians(alpha1)) + length * Math.Sin(ConvertToRadians(alpha1 + alpha2)));
            return new Point(x, y);
        }
    }
}
