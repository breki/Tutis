using Freude.Tests.FreudeTextParsingTests;
using NUnit.Framework;

namespace Freude.Tests.DocumentProcessingTests
{
    public class ProcessingFixture : ParsingFixture
    {
        public void ProcessText(string text, string expectedResult)
        {
            Parse(text).AssertNoErrors();

            using (TestDocumentProcessor processor = new TestDocumentProcessor ())
            {
                processor.ProcessDocument (Doc);
                string result = processor.ResultHtml;
                Assert.AreEqual (expectedResult, result);
            }
        }
    }
}