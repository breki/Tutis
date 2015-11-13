using NUnit.Framework;

namespace Freude.Tests.DocumentProcessingTests
{
    public class BasicProcessingTests
    {
        [Test]
        public void SimpleParagraph()
        {
            fixture.ProcessText(
                @"This is a '''paragraph'''",
@"<body>
  <p>This is a <strong>paragraph</strong></p>
</body>");
        }

        [Test]
        public void Headings()
        {
            fixture.ProcessText(
                @"= Title =

== 1. Heading ==
=== 1.1 Heading ===
This is a '''paragraph'''",
@"<body>
  <h1>
    Title
  </h1><h2>
    1. Heading
  </h2><h3>
    1.1 Heading
  </h3><p>This is a <strong>paragraph</strong></p>
</body>");
        }

        [Test]
        public void HeadingWithAnchor()
        {
            fixture.ProcessText(
                @"= Title =

== 1. Heading ==
=== 1.1 Heading ===
This is a '''paragraph'''",
@"<body>
  <h1>
    Title
  </h1><h2>
    1. Heading
  </h2><h3>
    1.1 Heading
  </h3><p>This is a <strong>paragraph</strong></p>
</body>");
        }

        [Test]
        public void InternalLink()
        {
            fixture.ProcessText(
                @"This is a [[Main Page|link to somewhere]]",
@"<body>
  <p>This is a <a href=""http://google.com/Main%20Page"">link to somewhere</a></p>
</body>");
        }

        [Test]
        public void InternalLinkNoDescription()
        {
            fixture.ProcessText(
                @"This is a [[Main Page]]",
@"<body>
  <p>This is a <a href=""http://google.com/Main%20Page"">Main Page</a></p>
</body>");
        }

        [Test]
        public void ExternalLink ()
        {
            fixture.ProcessText(
                @"This is a [http://google.com link to somewhere]",
@"<body>
  <p>This is a <a href=""http://google.com/"">link to somewhere</a></p>
</body>");
        }

        [Test]
        public void NumberedList()
        {
            fixture.ProcessText(
                @"# item 1
# item 2
# item 3

# item 4",
@"<body>
  <ol>
    <li>item 1</li><li>item 2</li><li>item 3</li><li>item 4</li>
  </ol>
</body>");
        }

        [SetUp]
        public void Setup()
        {
            fixture = new ProcessingFixture ();
        }

        private ProcessingFixture fixture;
    }
}