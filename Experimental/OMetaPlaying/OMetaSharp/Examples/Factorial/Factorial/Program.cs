using System;
using OMetaSharp.OMetaCS;
using System.Linq;

namespace OMetaSharp.Examples
{
    public class Program : OMetaCSConsoleProgram<Factorial, int>
    {
        public static void Main()
        {
            OMetaConsoleProgram.Run<Program>();
        }

        protected override Func<Factorial, Rule<int>> DefaultGrammarRuleFetcher
        {
            get { return g => g.Fact; }
        }

        public override void AddSamples()
        {
            AddSample(0,
                      1,
                      5,
                      10);
        }


        public override void PerformTests()
        {
            AssertResult(0, 1);
            AssertResult(1, 1);
            AssertResult(2, 2);
            AssertResult(3, 6);
            AssertResult(4, 24);            
        }
    }
}
