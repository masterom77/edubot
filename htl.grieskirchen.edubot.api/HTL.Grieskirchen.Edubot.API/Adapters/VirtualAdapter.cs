using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class VirtualAdapter : IAdapter
    {


        public VirtualAdapter(float length, bool requiresPrecalculation)
        {
            this.length = length;
            this.requiresPrecalculation = requiresPrecalculation;
        }

        public override void MoveTo(int x, int y, int z)
        {
            
        }

        public override void UseTool(bool activate)
        {
            
        }

        public override void SetInterpolationResult(InterpolationResult result)
        {
           
        }
    }
}
