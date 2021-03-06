﻿using LibroLib.ConsoleShells;
using SelfSignedHttpsListener.Commands;

namespace SelfSignedHttpsListener
{
    public static class Program
    {
        public static int Main (string[] args)
        {
            ConsoleShell consoleShell = new ConsoleShell ("SelfSignedHttpsListener.exe");
            consoleShell.RegisterCommand(new MakeCertificateCommand());
            consoleShell.RegisterCommand(new RunWebServerCommand());
            ConsoleShellResult consoleShellResult = consoleShell.ParseCommandLine(args);

            if (consoleShellResult.ExitCode.HasValue)
                return consoleShellResult.ExitCode.Value;

            foreach (IConsoleCommand consoleCommand in consoleShellResult.CommandsToExecute)
                consoleCommand.Execute(consoleShell);

            return 0;
        }
    }
}
