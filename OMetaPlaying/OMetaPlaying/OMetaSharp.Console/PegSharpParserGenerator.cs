using System.IO;
using MbUnit.Framework;

namespace OMetaSharp2.Console
{
    public class PegSharpParserGenerator
    {
        [Test]
        public void Test()
        {
            string pegFileName = @"..\..\SpatialSpec.peg";
            string grammar = File.ReadAllText(pegFileName);
            Parser parser = new Parser();
            parser.Parse(grammar, pegFileName);
            parser.Grammar.Validate();

            using (var stream = new StreamWriter(@"..\..\..\SpatialSpecTests\SpatialSpecParserUsingPegSharp.cs", false, System.Text.Encoding.UTF8))
            {
                using (var writer = new Writer(stream, parser.Grammar))
                {
                    writer.Write(pegFileName);
                    stream.Flush();
                }
            }
        }
    }
}