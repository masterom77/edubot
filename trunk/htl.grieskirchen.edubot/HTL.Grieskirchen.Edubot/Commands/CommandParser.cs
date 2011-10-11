using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTL.Grieskirchen.Edubot.Exceptions;

namespace HTL.Grieskirchen.Edubot.Commands
{
    class CommandParser
    {
        List<ICommand> commands;
        Dictionary<string, Type> commandList;

        public CommandParser() {
            commands = new List<ICommand>();
            commandList = new Dictionary<string, Type>();
            commandList.Add("MOVETO", typeof(MovementCommand));
            commandList.Add("USETOOL", typeof(ToolCommand));
            commandList.Add("START", typeof(StartCommand));
            commandList.Add("SHUTDOWN", typeof(ShutdownCommand));
            commandList.Add("SET_INTERPOLATION", typeof(InterpolationChangeCommand));
            
        }

        /// <summary>
        /// Parses the given code and executes it
        /// </summary>
        /// <param name="text">The code to be executed</param>
        /// <exception cref="InvalidParameterExeption"></exception>
        /// <exception cref="UnknownCommandException"></exception>
        public void Parse(string text) {
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
                    string cmd = line.Substring(0, line.IndexOf("("));
                    string parameters = line.Substring(line.IndexOf("(") + 1);
                    parameters = parameters.Remove(parameters.Length - 1);
                    Type cmdType;
                    if (commandList.TryGetValue(cmd.ToUpper(), out cmdType))
                    {
                        ICommand command = ((ICommand)Activator.CreateInstance(cmdType));
                        command.SetArguments(parameters.Contains(',') ? parameters.Split(',') : new string[]{parameters});
                        commands.Add(command);

                    }
                    else
                    {
                        throw new UnknownCommandException("Unknown Command: \"" + cmd + "\". Check the command reference if you don't know the command");
                    }
                }
                lineCount++;
            }

            foreach (ICommand cmd in commands) {
                cmd.Execute();
            }

         
        }

    }
}
