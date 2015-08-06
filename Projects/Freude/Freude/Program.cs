using System;
using System.Diagnostics;
using System.Reflection;
using Brejc.Common.Console;
using Brejc.Common.FileSystem;
using Brejc.Common.Ftp;
using Freude.Commands;
using Freude.Parsing;
using Freude.ProjectServices;
using Freude.Templating;
using log4net;
using log4net.Config;
using Syborg.Razor;

namespace Freude
{
    public static class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public static int Main (string[] args)
        {
            Stopwatch commandStopwatch = new Stopwatch ();
            commandStopwatch.Start ();

            XmlConfigurator.Configure ();

            IFileSystem fileSystem = new WindowsFileSystem ();
            IFreudeTextParser freudeTextParser = new FreudeTextParser ();
            IRazorCompiler razorCompiler = new InMemoryRazorCompiler ();
            IFreudeTemplatingEngine freudeTemplatingEngine = new FreudeTemplatingEngine (razorCompiler);
            IProjectBuilder projectBuilder = new ProjectBuilder (fileSystem);
            IFtpChannelFactory ftpChannelFactory = new FtpChannelFactoryUsingSockets ();
            IFtpCommunicator ftpCommunicator = new FtpCommunicator ();
            IFtpSessionFactory ftpSessionFactory = new FtpSessionFactory(ftpChannelFactory, ftpCommunicator, fileSystem);

            ConsoleShell consoleShell = new ConsoleShell ("ScalableMaps.Mapmaker.exe");
            consoleShell.RegisterCommand (new BuildCommand (fileSystem, projectBuilder, freudeTextParser, freudeTemplatingEngine));
            consoleShell.RegisterCommand (new DeployCommand(projectBuilder, ftpSessionFactory));

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
