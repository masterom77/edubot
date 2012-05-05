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
                case "MVS": return BuildMVSCommand(parameters);
                case "MVC": return BuildMVCCommand(parameters);
                case "USETOOL": return BuildUseToolCommand(parameters);
                case "INIT": return BuildInitCommand(parameters);
                case "SHUTDOWN": return BuildShutdownCommand(parameters);
                default: throw new UnknownCommandException("Befehl \"" + cmd + "\" unbekannt");
            }
        }

        private static MVSCommand BuildMVSCommand(string[] parameters) {
            int x, y, z;
            if (parameters.Length != 3)
                throw new InvalidSyntaxException("Das MVS-Command übernimmt genau 3 Parameter(x,y,z)");
            if (!int.TryParse(parameters[0], out x))
                throw new InvalidParameterException("Ungültiger x-Parameter \"" + parameters[0] + "\"");
            if (!int.TryParse(parameters[1], out y))
                throw new InvalidParameterException("Ungültiger y-Parameter \"" + parameters[1] + "\"");
            if (!int.TryParse(parameters[2], out z))
                throw new InvalidParameterException("Ungültiger z-Parameter \"" + parameters[2] + "\"");
            return new MVSCommand(new Point3D(x, y, z));
        }

        private static MVCCommand BuildMVCCommand(string[] parameters)
        {
            int targetX, targetY, targetZ, centerX, centerY, centerZ;
            
            if (parameters.Length != 6)
                throw new InvalidSyntaxException("Das MVC-Command übernimmt genau 6 Parameter(targetX,targetY,targetZ,centerX,centerY,centerZ)");
            if (!int.TryParse(parameters[0], out targetX))
                throw new InvalidParameterException("Ungültiger x-Parameter \"" + parameters[0] + "\"");
            if (!int.TryParse(parameters[1], out targetY))
                throw new InvalidParameterException("Ungültiger y-Parameter \"" + parameters[1] + "\"");
            if (!int.TryParse(parameters[2], out targetZ))
                throw new InvalidParameterException("Ungültiger z-Parameter \"" + parameters[2] + "\"");
            if (!int.TryParse(parameters[3], out centerX))
                throw new InvalidParameterException("Ungültiger centerX-Parameter \"" + parameters[3] + "\"");
            if (!int.TryParse(parameters[4], out centerY))
                throw new InvalidParameterException("Ungültiger centerY-Parameter \"" + parameters[4] + "\"");
            if (!int.TryParse(parameters[5], out centerZ))
                throw new InvalidParameterException("Ungültiger centerZ-Parameter \"" + parameters[5] + "\"");
            return new MVCCommand(new Point3D(targetX, targetY, targetZ),new Point3D(centerX,centerY,centerZ));
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

        private static InitCommand BuildInitCommand(string[] parameters)
        {
            if (!String.IsNullOrWhiteSpace(parameters[0]))
                throw new InvalidSyntaxException("Das Init-Command übernimmt keinen Parameter");
            return new InitCommand();
        }

        private static ShutdownCommand BuildShutdownCommand(string[] parameters)
        {
            if (!String.IsNullOrWhiteSpace(parameters[0]))
                throw new InvalidSyntaxException("Das Shutdown-Command übernimmt keinen Parameter");
            return new ShutdownCommand();
        }

        //private static ChangeInterpolationCommand BuildChangeInterpolationCommand(string[] parameters)
        //{
        //    IInterpolationType type;
        //    if (parameters.Length != 1)
        //        throw new InvalidSyntaxException("Das Shutdown-Command übernimmt nur 1 Parameter(interpolationType)");
        //    switch (parameters[0].ToUpper()) {
        //        case "LINEAR": type = new LinearInterpolation();
        //            break;
        //        default: throw new InvalidParameterException("Unbekannte Interpolationsart: \"" + parameters[0] + "\"");
        //    }
        //    return new ChangeInterpolationCommand(type);
        //}
    }
}
