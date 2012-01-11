using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.Exceptions;
using HTL.Grieskirchen.Edubot.API.Commands;

namespace HTL.Grieskirchen.Edubot.Commands
{
    class CommandParser
    {

        /// <summary>
        /// Parses the given code and executes it
        /// </summary>
        /// <param name="text">The code to be executed</param>
        /// <exception cref="InvalidParameterExeption"></exception>
        /// <exception cref="UnknownCommandException"></exception>
        public static List<ICommand> Parse(string text) {
            List<ICommand> commands = new List<ICommand>();
            commands.Clear();
            text = text.Replace(Environment.NewLine, string.Empty);
            string[] lines = text.Split(';');
            int lineCount = 1;
            //Syntax check if there is only one command in the text
            if (lines.Length <= 1 && text.Trim() != string.Empty) {
                if (text.EndsWith(";"))
                {
                    lines = new string[] { text };
                }
                else {
                    throw new InvalidSyntaxException("Invalid Syntax in Line " + lineCount + ": \"" + text + "\". Check if your command has a ';' at the end.");
                }
            }
            //General Syntax, Parameter and Command parsing
            foreach (string line in lines) {
                line.Trim();
                if (line != string.Empty)
                {
                    if (!line.Contains("(") || !line.Contains(")")) {
                        throw new InvalidSyntaxException("Invalid Syntax in Line "+lineCount+": \""+line+"\". Check if your command has a ';' at the end.");
                    }
                    string cmd = line.Substring(0, line.IndexOf("(")).ToUpper();
                    string parameters = line.Substring(line.IndexOf("(") + 1);
                    parameters = parameters.Remove(parameters.Length - 1);

                    if (Enum.GetNames(typeof(Commands)).Contains(cmd))
                    {
                        ICommand command = CommandBuilder.BuildCommand(cmd, parameters.Contains(',') ? parameters.Split(',') : new string[]{parameters});
                        commands.Add(command);

                    }
                    else
                    {
                        throw new UnknownCommandException("Unknown Command: \"" + cmd + "\". Check the command reference if you don't know the command");
                    }
                }
                lineCount++;
            }
            return commands;

            

         
        }

        

    }
}
