using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OMetaSharp.OMetaCS;

namespace OMetaSharp.OMetaCS.UnitTests
{
    class Program : ICompileGrammars, IPerformTests
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.CompileGrammars();
            p.PerformTests();         
        }

        public void CompileGrammars()
        {
            Bootstrapper.BootstrapOMetaCS();
        }

        public void PerformTests()
        {
            CSharpRecognizerTests.PerformTests();
            OMetaParserTests.PerformTests();                        
        }
    }
}
