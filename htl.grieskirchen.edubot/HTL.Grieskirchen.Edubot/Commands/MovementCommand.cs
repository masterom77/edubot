using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EduBotAPI;
using HTL.Grieskirchen.Edubot.Exceptions;

namespace HTL.Grieskirchen.Edubot.Commands
{
    class MovementCommand : ICommand
    {
        int x, y, z;

        public void Execute()
        {
            Console.WriteLine("Moving to Position: " + x + "," + y + "," + z);         
        }


        public void SetArguments(string[] args)
        {
            if (!int.TryParse(args[0], out x))
                throw new InvalidParameterException("Invalid parameter: x. Cannot convert \"" + args[0] + "\" into an int");
            if (!int.TryParse(args[1], out y))
                throw new InvalidParameterException("Invalid parameter: y. Cannot convert \"" + args[1] + "\" into an int");
            if (!int.TryParse(args[2], out z))
                throw new InvalidParameterException("Invalid parameter: z. Cannot convert \"" + args[2] + "\" into an int");
        }
    }
}
