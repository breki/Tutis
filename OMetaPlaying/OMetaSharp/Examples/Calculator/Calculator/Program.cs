using System;
using OMetaSharp.OMetaCS;

namespace OMetaSharp.Examples
{
    public class Program : OMetaCSConsoleProgram<Calculator>
    {
        public static void Main()
        {
            OMetaConsoleProgram.Run<Program>();
        }

        protected override Func<Calculator, Rule<char>> DefaultGrammarRuleFetcher
        {
            get { return g => g.Expr; }            
        }

        public override void AddSamples()
        {
            AddSample("1+2",
                      "((6*(4+3))/2)-1");
        }

        public override void PerformTests()
        {
            AssertResult("1", 1);
            AssertResult("1+2", 3);
            AssertResult("2*3", 6);
            AssertResult("((6*(4+3))/2)-1", 20);
        }
    }
}
