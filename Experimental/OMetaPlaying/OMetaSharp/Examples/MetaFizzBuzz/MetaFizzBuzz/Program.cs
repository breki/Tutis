using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMetaSharp.OMetaCS;
using OMetaSharp.OMetaCS.UnitTests;
using System.IO;

namespace OMetaSharp.Examples
{   
    public class Program : OMetaCSConsoleProgram<MetaFizzBuzz>
    {
        public static void Main()
        {
            OMetaConsoleProgram.Run<Program>();
        }

        protected override Func<MetaFizzBuzz, Rule<char>> DefaultGrammarRuleFetcher
        {
            get { return g => g.Program; }
        }
        
        public override void PerformTests()
        {
            BoringTests();

            foreach (string fileName in Directory.GetFiles(ProjectPaths.Programs))
            {
                Console.WriteLine("Executing '{0}'", Path.GetFileName(fileName));
                ExecuteProgram(File.ReadAllText(fileName));
            }
        }

        private static void BoringTests()
        {
            OMetaCSAssert.ParsesTo<MetaFizzBuzz, int>(
                    @"-123",
                    p => p.Number,
                    -123);

            OMetaCSAssert.ParsesTo<MetaFizzBuzz, int>(
                    @"+123",
                    p => p.Number,
                    123);

            OMetaCSAssert.ParsesTo<MetaFizzBuzz, int>(
                    @"123",
                    p => p.Number,
                    123);

            OMetaCSAssert.ParsesTo<MetaFizzBuzz, string>(
                    @"""Hello""",
                    p => p.QuotedString,
                    "Hello");

            OMetaCSAssert.ParsesTo<MetaFizzBuzz, string>(
                    @"varName",
                    p => p.VariableName,
                    "varName");

            OMetaCSAssert.ParsesTo<MetaFizzBuzz, string>(
                    @"varName12",
                    p => p.VariableName,
                    "varName12");

            OMetaCSAssert.ParsesTo<MetaFizzBuzz, string>(
                    @"the integer",
                    p => p.VariableName,
                    "integer");
        }

        public override void AddSamples()
        {
            // just go up to 15
            AddSample(Environment.NewLine + GetProgramContents("FizzBuzz1.mfb").Replace("100", "15"));            
        }

        private static string GetProgramContents(string programName)
        {
            return File.ReadAllText(Path.Combine(ProjectPaths.Programs, programName));
        }

        private void ExecuteProgram(string program)
        {
            Execute(CreateInputStream(program));
        }        

        protected override void Execute(OMetaStream<char> input)
        {
            var result = Grammars.ParseWith<MetaFizzBuzz, char>(input, DefaultGrammarRuleFetcher).As<Action>();
            result();
        }
    }
}
