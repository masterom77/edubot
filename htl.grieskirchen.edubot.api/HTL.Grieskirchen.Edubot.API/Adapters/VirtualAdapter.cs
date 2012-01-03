using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.API.Adapters
{
    public class VirtualAdapter : IAdapter
    {


        public VirtualAdapter(ITool tool, float length, bool requiresPrecalculation)
        {
            this.tool = tool;
            tool.X = (int)length * 2;
            tool.Y = 0;
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

        public override void Start()
        {
            
        }

        public override void Shutdown()
        {
            
        }
    }
}
