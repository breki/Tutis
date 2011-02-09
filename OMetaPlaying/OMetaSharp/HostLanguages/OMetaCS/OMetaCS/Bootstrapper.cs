using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OMetaSharp.OMetaCS
{
    public static class Bootstrapper
    {
        public static void BootstrapOMetaCS()
        {
            // OMeta/C# bootstrapping itself..
            ParseAndTranslateGrammarFile("CSharpRecognizer");
            ParseAndTranslateGrammarFile("OMetaParser");
            ParseAndTranslateGrammarFile("NullOptimization");
            ParseAndTranslateGrammarFile("AndOrOptimization");
            ParseAndTranslateGrammarFile("OMetaOptimizer");
            ParseAndTranslateGrammarFile("OMetaTranslator");
        }

        private static string RootPath
        {
            get
            {
                return ProjectPaths.Subdir(@"..\OMetaCS\");
            }
        }

        private static string GrammarPath(string grammarName)
        {
            return Path.Combine(RootPath, @"Grammars\" + grammarName + ".ometacs");
        }

        private static string GeneratedCodePath(string grammarName)
        {
            return Path.Combine(RootPath, @"GeneratedCode\" + grammarName + ".cs");
        }

        private static void ParseAndTranslateGrammarFile(string grammarName)
        {
            var inputFileName = GrammarPath(grammarName);
            var contents = File.ReadAllText(inputFileName);
            var result = Grammars.ParseGrammarThenOptimizeThenTranslate
                <OMetaParser, OMetaOptimizer, OMetaTranslator>
                (contents,
                p => p.Grammar,
                o => o.OptimizeGrammar,
                t => t.Trans);

            File.WriteAllText(GeneratedCodePath(grammarName), result);
        }
    }
}
