using System.IO;

namespace OMetaSharp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string grammar = File.ReadAllText("SpatialSpec.ometacs");
            string generatedCode = Grammars.ParseGrammarThenOptimizeThenTranslate<OMetaParser, OMetaOptimizer, OMetaTranslator>(
                grammar, 
                p => p.Grammar, 
                o => o.OptimizeGrammar,
                t => t.Trans);
            File.WriteAllText("SpatialSpecParserUsingOMetaSharp.cs", generatedCode);
        }
    }
}
