using System;
using OMetaSharp.OMetaCS;

namespace OMetaSharp.Examples
{
    public class Program : OMetaCSConsoleProgram<Markup>
    {
        public static void Main()
        {
            OMetaConsoleProgram.Run<Program>();
        }

        protected override Func<Markup, Rule<char>> DefaultGrammarRuleFetcher
        {
            get
            {
                return m => m.MarkedUpText;
            }
        }

        public override void AddSamples()
        {
            AddSample("=Introduction=\nHello World");
            AddSample("Some text");
        }

        public override void PerformTests()
        {            
            AssertResult("===Hello World===\nParagraph text", "<html><body><h3>Hello World</h3><p>Paragraph text</p></body></html>");
            AssertResult("=My H1=\nH1 Text\n==My H2 Stuff==\nNot as important stuff", "<html><body><h1>My H1</h1><p>H1 Text</p><h2>My H2 Stuff</h2><p>Not as important stuff</p></body></html>");
        }        
    }
}
