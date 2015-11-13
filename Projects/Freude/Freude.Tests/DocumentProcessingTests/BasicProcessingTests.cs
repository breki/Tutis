using Freude.Tests.FreudeTextParsingTests;
using NUnit.Framework;

namespace Freude.Tests.DocumentProcessingTests
{
    public class BasicProcessingTests
    {
        [Test]
        public void SimpleParagraph()
        {
            fixture.Parse(@"This is a '''paragraph'''");

            using (TestDocumentProcessor processor = new TestDocumentProcessor())
            {
                processor.ProcessDocument(fixture.Doc);
                string result = processor.ResultHtml;
                Assert.AreEqual (
@"<body>
  <p>This is a <strong>paragraph</strong></p>
</body>", result);
            }
        }

        [Test]
        public void Headings()
        {
            fixture.Parse(@"= Title =

== 1. Header ==
=== 1.1 Header ===
This is a '''paragraph'''");

            using (TestDocumentProcessor processor = new TestDocumentProcessor())
            {
                processor.ProcessDocument(fixture.Doc);
                string result = processor.ResultHtml;
                Assert.AreEqual (
@"<body>
  <h1>
    Title
  </h1><h2>
    1. Header
  </h2><h3>
    1.1 Header
  </h3><p>This is a <strong>paragraph</strong></p>
</body>", result);
            }
        }

        [SetUp]
        public void Setup()
        {
            fixture = new ParsingFixture();
        }

        private ParsingFixture fixture;
    }
}