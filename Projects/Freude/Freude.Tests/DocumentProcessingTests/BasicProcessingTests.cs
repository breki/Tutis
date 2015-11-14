using System.Diagnostics.CodeAnalysis;
using System.Web.Compilation;
using NUnit.Framework;

namespace Freude.Tests.DocumentProcessingTests
{
    public class BasicProcessingTests
    {
        [SuppressMessage ("Microsoft.StyleCop.CSharp.Readability", "SA1118")]
        [Test]
        public void Headings()
        {
            const string Text = @"= Title =

== 1. Heading ==
=== 1.1 Heading ===
This is a '''paragraph'''";
            const string ExpectedHtml = @"<body>
  <h1>
    Title
  </h1><h2>
    1. Heading
  </h2><h3>
    1.1 Heading
  </h3><p>This is a <strong>paragraph</strong></p>
</body>";
            fixture.ProcessText(Text, ExpectedHtml);
        }

        [Test]
        public void HeadingsShouldBeEscaped()
        {
            const string Text = @"= <special> title =";
            const string ExpectedHtml = @"<body>
  <h1>
    &lt;special&gt; title
  </h1>
</body>";
            fixture.ProcessText (Text, ExpectedHtml);
        }

        [Test]
        public void HeadingWithAnchor()
        {
            const string Text = @"== 1. Heading ==#link1";
            const string ExpectedHtml = @"<body>
  <h2 id=""link1"">
    1. Heading
  </h2>
</body>";
            fixture.ProcessText(Text, ExpectedHtml);
        }

        [Test]
        public void InternalLink()
        {
            const string ExpectedHtml = @"<body>
  <p>This is a <a href=""http://google.com/Main%20Page"">link to somewhere</a></p>
</body>";
            fixture.ProcessText(@"This is a [[Main Page|link to somewhere]]", ExpectedHtml);
        }

        [Test]
        public void InternalLinkNoDescription()
        {
            const string ExpectedHtml = @"<body>
  <p>This is a <a href=""http://google.com/Main%20Page"">Main Page</a></p>
</body>";
            fixture.ProcessText(@"This is a [[Main Page]]", ExpectedHtml);
        }

        [Test]
        public void ExternalLink ()
        {
            const string ExpectedHtml = @"<body>
  <p>This is a <a href=""http://google.com/"">link to somewhere</a></p>
</body>";
            fixture.ProcessText(@"This is a [http://google.com link to somewhere]", ExpectedHtml);
        }

        [SetUp]
        public void Setup()
        {
            fixture = new ProcessingFixture ();
        }

        private ProcessingFixture fixture;
    }
}