﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTL.Grieskirchen.Edubot.API.Interpolation
{
    public class InterpolationStep
    {
        float alpha1;

        public float Alpha1
        {
            get { return alpha1; }
            set { alpha1 = value; }
        }

        float alpha2;

        public float Alpha2
        {
            get { return alpha2; }
            set { alpha2 = value; }
        }

        float alpha3;

        public float Alpha3
        {
            get { return alpha3; }
            set { alpha3 = value; }
        }
    }
}
