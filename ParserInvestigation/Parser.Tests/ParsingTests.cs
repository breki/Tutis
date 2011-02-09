using System.IO;
using System.Text;
using MbUnit.Framework;
using Parser.Coco;

namespace Parser.Tests
{
    public class ParsingTests
    {
        [Test]
        public void CocoTest()
        {
            string text = "@func(test @func2(test3, \"simple\"), [[test2]], [[name:en]]) \"literal is just fine\" tag ";

            using (MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (StringWriter errorStream = new StringWriter())
            {
                Scanner scanner = new Scanner(inputStream);
                Parser.Coco.Parser parser = new Parser.Coco.Parser(scanner);
                parser.errors.errorStream = errorStream;
                parser.Parse();

                Assert.AreEqual(string.Empty, errorStream.ToString());

                LabelDefinition labelDefinition = parser.ParsedLabelDefinition;

                Assert.AreEqual(3, labelDefinition.Parts.Count);
                FunctionCall funcCall = (FunctionCall) labelDefinition.Parts[0];
                Assert.AreEqual("func", funcCall.FunctionName);
                Assert.AreEqual(3, funcCall.Arguments.Count);
                LabelDefinition innerDef = funcCall.Arguments[0].LabelDefinition;
                Assert.AreEqual(2, innerDef.Parts.Count);
                innerDef = funcCall.Arguments[2].LabelDefinition;
                Assert.AreEqual("name:en", ((TagReference)innerDef.Parts[0]).TagName);
                Assert.AreEqual("literal is just fine", ((Literal)labelDefinition.Parts[1]).Text);
            }
        }
    }
}