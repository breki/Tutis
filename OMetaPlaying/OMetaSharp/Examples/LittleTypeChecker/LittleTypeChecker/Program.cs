using System;
using OMetaSharp.OMetaCS;

namespace OMetaSharp.Examples
{    
    public class Program : OMetaCSConsoleProgram<LittleTypeChecker>
    {
        public static void Main()
        {
            OMetaConsoleProgram.Run<Program>();
        }

        protected override Func<LittleTypeChecker, Rule<char>> DefaultGrammarRuleFetcher
        {
            get { return g => g.TypeCheck; }
        }

        public override void AddSamples()
        {
            AddSample("0",
                      "1",
                      "true",
                      "false",
                      "isZero 0",
                      "isZero 1",
                      "if true then false else true",
                      @"if isZero 1
                           then if true
                                then 0
                                else 1
                           else 1");
        }

        public override void PerformTests()
        {
            AssertResult(@"if isZero 1
                           then if true
                                then 0
                                else 1
                           else 1", typeof(int));
        }
    }
}
