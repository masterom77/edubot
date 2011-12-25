﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public abstract class IAdapter
    {
        protected float length;

        public float Length
        {
            get { return length; }
        }

        protected bool requiresPrecalculation;

        public bool RequiresPrecalculation
        {
            get { return requiresPrecalculation; }
            set { requiresPrecalculation = value; }
        }

        public abstract void MoveTo(int x, int y, int z);
        public abstract void UseTool(bool activate);
        public abstract void SetInterpolationResult(Interpolation.InterpolationResult result);
        
    }
}
