using System.IO;
using MbUnit.Framework;

namespace OMetaSharp.Console
{
    public class OMetaParserGenerator
    {
        [Test]
        public void Test()
        {
            string grammar = File.ReadAllText(@"..\..\SpatialSpec.ometacs");
            string generatedCode = Grammars.ParseGrammarThenOptimizeThenTranslate<OMetaParser, OMetaOptimizer, OMetaTranslator>(
                grammar,
                p => p.Grammar,
                o => o.OptimizeGrammar,
                t => t.Trans);
            File.WriteAllText(@"..\..\..\SpatialSpecTests\SpatialSpecParserUsingOMetaSharp.cs", generatedCode);
        }
    }
}