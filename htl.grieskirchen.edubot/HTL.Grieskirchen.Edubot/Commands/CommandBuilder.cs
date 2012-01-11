using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.API.Commands;
using HTL.Grieskirchen.Edubot.API;
using HTL.Grieskirchen.Edubot.Exceptions;
using HTL.Grieskirchen.Edubot.API.Interpolation;

namespace HTL.Grieskirchen.Edubot.Commands
{
    class CommandBuilder
    {
        public static ICommand BuildCommand(string cmd, string[] parameters){
            switch (cmd) {
                case "MOVETO": return BuildMoveCommand(parameters);
                    break;
                case "USETOOL": return BuildUseToolCommand(parameters);
                    break;
                case "START": return BuildStartCommand(parameters);
                    break;
                case "SHUTDOWN": return BuildShutdownCommand(parameters);
                    break;
                case "SET_INTERPOLATION": return BuildChangeInterpolationCommand(parameters);
                    break;
                default: throw new UnknownCommandException("Befehl \"" + cmd + "\" unbekannt");
            }
        }

        private static MoveCommand BuildMoveCommand(string[] parameters) {
            int x, y, z;
            if (parameters.Length != 3)
                throw new InvalidSyntaxException("Das Move-Command übernimmt nur 3 Parameter(x,y,z)");
            if (!int.TryParse(parameters[0], out x))
                throw new InvalidParameterException("Ungültiger x-Parameter \"" + parameters[0] + "\"");
            if (!int.TryParse(parameters[1], out y))
                throw new InvalidParameterException("Ungültiger y-Parameter \"" + parameters[1] + "\"");
            if (!int.TryParse(parameters[2], out z))
                throw new InvalidParameterException("Ungültiger z-Parameter \"" + parameters[2] + "\"");
            return new MoveCommand(new Point3D(x, y, z));
        }

        private static UseToolCommand BuildUseToolCommand(string[] parameters)
        {
            bool activate;
            if (parameters.Length != 1)
                throw new InvalidSyntaxException("Das UseTool-Command übernimmt nur 1 Parameter(activate)");
            if (!bool.TryParse(parameters[0], out activate))
                throw new InvalidParameterException("Ungültiger x-Parameter \"" + parameters[0] + "\"");
            return new UseToolCommand(activate);
        }

        private static StartCommand BuildStartCommand(string[] parameters)
        {
            if (!String.IsNullOrWhiteSpace(parameters[0]))
                throw new InvalidSyntaxException("Das Start-Command übernimmt keinen Parameter");
            return new StartCommand();
        }

        private static ShutdownCommand BuildShutdownCommand(string[] parameters)
        {
            if (!String.IsNullOrWhiteSpace(parameters[0]))
                throw new InvalidSyntaxException("Das Shutdown-Command übernimmt keinen Parameter");
            return new ShutdownCommand();
        }

        private static ChangeInterpolationCommand BuildChangeInterpolationCommand(string[] parameters)
        {
            IInterpolationType type;
            if (parameters.Length != 1)
                throw new InvalidSyntaxException("Das Shutdown-Command übernimmt nur 1 Parameter(interpolationType)");
            switch (parameters[0].ToUpper()) {
                case "LINEAR": type = new LinearInterpolation();
                    break;
                default: throw new InvalidParameterException("Unbekannte Interpolationsart: \"" + parameters[0] + "\"");
            }
            return new ChangeInterpolationCommand(type);
        }
    }
}
