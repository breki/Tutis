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
</body>", 
                    result);
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
</body>", 
                    result);
            }
        }

        [Test]
        public void InternalLink()
        {
            fixture.Parse (@"This is a [[Main Page|link to somewhere]]");

            using (TestDocumentProcessor processor = new TestDocumentProcessor ())
            {
                processor.ProcessDocument (fixture.Doc);
                string result = processor.ResultHtml;
                Assert.AreEqual (
@"<body>
  <p>This is a <a href=""http://google.com/Main%20Page"">link to somewhere</a></p>
</body>", 
                    result);
            }
        }

        [Test]
        public void InternalLinkNoDescription()
        {
            fixture.Parse (@"This is a [[Main Page]]");

            using (TestDocumentProcessor processor = new TestDocumentProcessor ())
            {
                processor.ProcessDocument (fixture.Doc);
                string result = processor.ResultHtml;
                Assert.AreEqual (
@"<body>
  <p>This is a <a href=""http://google.com/Main%20Page"">Main Page</a></p>
</body>", 
                    result);
            }
        }

        [Test]
        public void ExternalLink ()
        {
            fixture.Parse (@"This is a [http://google.com link to somewhere]]");

            using (TestDocumentProcessor processor = new TestDocumentProcessor ())
            {
                processor.ProcessDocument (fixture.Doc);
                string result = processor.ResultHtml;
                Assert.AreEqual (
@"<body>
  <p>This is a <a href=""http://google.com/"">link to somewhere</a></p>
</body>", 
                    result);
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