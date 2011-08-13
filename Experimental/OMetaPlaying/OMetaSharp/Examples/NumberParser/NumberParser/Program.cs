using System;
using OMetaSharp.OMetaCS;

namespace OMetaSharp.Examples
{    
    public class Program : OMetaCSConsoleProgram<NumberParser>
    {
        public static void Main()
        {
            OMetaConsoleProgram.Run<Program>();
        }

        protected override Func<NumberParser, Rule<char>> DefaultGrammarRuleFetcher
        {
            get { return g => g.Number; }
        }

        public override void AddSamples()
        {
            AddSample("1", "123");
        }

        public override void PerformTests()
        {
            AssertResult("1", 1);
            AssertResult("123", 123);            
        }
    }
}
