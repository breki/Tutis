using System;
using System.Collections.Generic;

namespace SamsungTvChannelsTool
{
    public class HelpCommand : ICommand
    {
        public HelpCommand(IList<ICommand> commands)
        {
            this.commands = commands;
        }

        public string CommandName
        {
            get { return "help"; }
        }

        public string CommandUsage
        {
            get { return "help"; }
        }

        public string CommandDescription
        {
            get { return "command line help"; }
        }

        public int Execute(string[] args)
        {
            foreach (ICommand command in commands)
                Console.Out.WriteLine("{0} : {1}", command.CommandName, CommandDescription);

            return 0;
        }

        private readonly IList<ICommand> commands;
    }
}