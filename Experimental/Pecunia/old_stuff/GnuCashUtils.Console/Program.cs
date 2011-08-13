using System;
using System.Collections.Generic;
using System.Text;
using GnuCashUtils.Framework;
using GnuCashUtils.Framework.DataMiners;
using log4net;

namespace GnuCashUtils.Console
{
    public sealed class Program
    {
        static public void Main (string[] args)
        {
            if (args == null)
                throw new ArgumentNullException ("args");                
            
            log4net.Config.XmlConfigurator.Configure ();

            try
            {
                if (args.Length < 2)
                    throw new ArgumentException ("Missing arguments.");

                string gnuCashFileName = args[0];
                string commandName = args[1];
                ICommand command = null;

                switch (commandName.ToLower (System.Globalization.CultureInfo.InvariantCulture))
                {
                    case "sloprices":
                        command = new SloPricesCommand();
                        break;

                    case "dailyreport":
                        DailyReportCommand dailyReportCommand = new DailyReportCommand ();

                        if (args.Length != 3)
                            throw new ArgumentException ("Missing reports root directory parameter.");

                        dailyReportCommand.ReportsRootDirectory = args[2];
                        command = dailyReportCommand;

                        break;

                    default:
                        throw new ArgumentException (String.Format (System.Globalization.CultureInfo.InvariantCulture,
                            "Unknown command '{0}'", commandName));
                }

                command.Execute (gnuCashFileName);

                Sleep ();
            }
            catch (ArgumentException ex)
            {
                System.Console.Error.WriteLine (ex.Message);
                ShowHelp ();
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine (ex);
                log.Error (ex);

                Sleep ();
                throw;
            }
        }

        static void ShowHelp ()
        {
            System.Console.Out.WriteLine ("GnuCashUtils.Console by Igor Brejc");
            System.Console.Out.WriteLine ();
            System.Console.Out.WriteLine ("USAGE:");
            System.Console.Out.WriteLine ("GnuCashUtils.Console <GnuCashFile> <command>");
            System.Console.Out.WriteLine ();
            System.Console.Out.WriteLine ("COMMANDS:");
            System.Console.Out.WriteLine ("dailyreport <reports root directory> - creates a daily report from GnuCash data");
            System.Console.Out.WriteLine ("sloprices - for Slovenian users only: updates currency prices and prices of Slovenian mutual funds");
            System.Console.Out.WriteLine ("            using www.vzajemci.com and www.skladi.com data");
        }

        static void Sleep ()
        {
            int waitTimeInSeconds = 10;
            System.Console.Out.WriteLine ("Waiting for {0} seconds before closing...", waitTimeInSeconds);
            System.Threading.Thread.Sleep (TimeSpan.FromSeconds (waitTimeInSeconds));
        }

        private Program () { }

        static readonly private ILog log = LogManager.GetLogger (typeof (Program));
    }

    public interface ICommand
    {
        void Execute (string gnuCashFileName);
    }
}
