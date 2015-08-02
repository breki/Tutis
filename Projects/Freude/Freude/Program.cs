using System;
using System.Diagnostics;
using System.Reflection;
using Brejc.Common.Console;
using Freude.Commands;
using log4net;
using log4net.Config;

namespace Freude
{
    public static class Program
    {
        public static int Main (string[] args)
        {
            Stopwatch commandStopwatch = new Stopwatch ();
            commandStopwatch.Start ();

            XmlConfigurator.Configure ();

            ConsoleShell consoleShell = new ConsoleShell ("ScalableMaps.Mapmaker.exe");
            consoleShell.RegisterCommand (new BuildCommand ());

            try
            {
                ConsoleShellResult consoleShellResult = consoleShell.ParseCommandLine (args);

                if (consoleShellResult.ExitCode.HasValue)
                    return consoleShellResult.ExitCode.Value;

                foreach (IConsoleCommand consoleCommand in consoleShellResult.CommandsToExecute)
                    consoleCommand.Execute (consoleShell);

                TimeSpan elapsed = commandStopwatch.Elapsed;
                log.InfoFormat ("Command done in {0}", elapsed);

                return 0;
            }
            catch (Exception ex)
            {
                log.Fatal ("Program failed", ex);
                return 1;
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
