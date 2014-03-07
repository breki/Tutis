using System;
using System.Collections.Generic;
using System.Linq;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class Program
    {
        public static int Main (string[] args)
        {
            IFileSystem fileSystem = new WindowsFileSystem ();
            IZipper2 zipper = new Zipper2 (fileSystem);

            List<ICommand> commands = new List<ICommand> ();
            HelpCommand helpCommand = new HelpCommand (commands);

            commands.Add (new GenerateChannelsOrderCommand (fileSystem, zipper));
            commands.Add (new ApplyChannelsOrderCommand (fileSystem, zipper));
            commands.Add(helpCommand);

            ICommand commandToExecute = helpCommand;
            if (args.Length > 0)
            {
                commandToExecute = commands.FirstOrDefault(x => x.CommandName == args[0]);
                if (commandToExecute == null)
                    commandToExecute = helpCommand;
            }

            try
            {
                return commandToExecute.Execute(args);
            }
            catch (InvalidOperationException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
