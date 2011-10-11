using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API;
using HTL.Grieskirchen.Edubot.Exceptions;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.Commands
{
    class InterpolationChangeCommand : ICommand
    {

        IInterpolationType interpolation;

        public void Execute()
        {
            API.Edubot.GetInstance().Interpolation = interpolation;         
        }

        public void SetArguments(string[] args)
        {
            switch (args[0].ToLower()) {
                case "linear":
                    interpolation = new LinearInterpolation();
                    break;
                case "spline":
                    interpolation = new SplineInterpolation();
                    break;
                default:
                    throw new Exception("Invalid Interpolation");
            }
            
        }
    }
}
