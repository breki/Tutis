using System;
using OMetaSharp.OMetaCS;

namespace OMetaSharp.Examples
{
    public class Program : OMetaCSConsoleProgram<CalculatorParser>
    {
        public static void Main()
        {
            OMetaConsoleProgram.Run<Program>();
        }

        protected override Func<CalculatorParser, Rule<char>> DefaultGrammarRuleFetcher
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
            AssertResult("((6*(4+3))/2)-1", "[sub, [div, [mul, [num, 6], [add, [num, 4], [num, 3]]], [num, 2]], [num, 1]]");
        }
    }
}
