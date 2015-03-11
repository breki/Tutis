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
                string outputDir = @".\output";
                string layoutFileName = null;
                string contentDir = @".\content";

                foreach (string arg in args)
                {
                    if (arg.StartsWith("-i", StringComparison.OrdinalIgnoreCase))
                        inputDir = FetchArgValue(arg);
                    else if (arg.StartsWith ("-o", StringComparison.OrdinalIgnoreCase))
                        outputDir = FetchArgValue (arg);
                    else if (arg.StartsWith ("-lay", StringComparison.OrdinalIgnoreCase))
                        layoutFileName = FetchArgValue (arg);
                    else if (arg.StartsWith("-c", StringComparison.OrdinalIgnoreCase))
                        contentDir = FetchArgValue(arg);
                    else
                    {
                        string errorMessage = string.Format(CultureInfo.InvariantCulture, "Unknown command line parameter '{0}'", arg);
                        throw new CommandLineException (errorMessage);
                    }
                }

                MarkdownToHtmlGenerator generator = new MarkdownToHtmlGenerator();
                generator.Generate(inputDir, outputDir, layoutFileName, contentDir);
            }
            catch (CommandLineException ex)
            {
                Console.Error.WriteLine (ex.Message);
                Console.Out.WriteLine ();
                Console.Out.WriteLine ("USAGE:");
                Console.Out.WriteLine ("-i:<dir> - input directory (default is current dir)");
                Console.Out.WriteLine ("-o:<dir> - output directory (default is ./output)");
                Console.Out.WriteLine ("-lay:<file> - HTML layout file name");
                Console.Out.WriteLine ("-c:<file> - content directory (default is ./content");
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
