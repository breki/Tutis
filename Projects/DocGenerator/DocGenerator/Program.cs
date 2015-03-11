using System;
using System.Globalization;

namespace DocGenerator
{
    public class Program
    {
        public static void Main (string[] args)
        {
            try
            {
                string inputDir = ".";
                string outputDir = ".";
                string layoutFileName = null;
                string cssFileName = null;

                foreach (string arg in args)
                {
                    if (arg.StartsWith("-i", StringComparison.OrdinalIgnoreCase))
                        inputDir = FetchArgValue(arg);
                    else if (arg.StartsWith ("-o", StringComparison.OrdinalIgnoreCase))
                        outputDir = FetchArgValue (arg);
                    else if (arg.StartsWith ("-lay", StringComparison.OrdinalIgnoreCase))
                        layoutFileName = FetchArgValue (arg);
                    else if (arg.StartsWith("-css", StringComparison.OrdinalIgnoreCase))
                        cssFileName = FetchArgValue(arg);
                    else
                    {
                        string errorMessage = string.Format(CultureInfo.InvariantCulture, "Unknown command line parameter '{0}'", arg);
                        throw new CommandLineException (errorMessage);
                    }
                }

                MarkdownToHtmlGenerator generator = new MarkdownToHtmlGenerator();
                generator.Generate(inputDir, outputDir, layoutFileName, cssFileName);
            }
            catch (CommandLineException ex)
            {
                Console.Error.WriteLine (ex.Message);
                Console.Out.WriteLine ();
                Console.Out.WriteLine ("USAGE:");
                Console.Out.WriteLine ("-i:<dir> - input directory (default is current dir)");
                Console.Out.WriteLine ("-o:<dir> - output directory (default is current dir)");
                Console.Out.WriteLine ("-lay:<file> - HTML layout file name");
                Console.Out.WriteLine ("-css:<file> - CSS file name");
            }
        }

        private static string FetchArgValue(string arg)
        {
            int i = arg.IndexOf(':');

            if (i == -1)
            {
                string errorMessage = string.Format(CultureInfo.InvariantCulture, "Invalid argument value: '{0}'", arg);
                throw new CommandLineException(errorMessage);
            }

            return arg.Substring(i + 1);
        }
    }
}
