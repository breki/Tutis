using NUnit.Framework;

namespace Freude.Tests.DocumentProcessingTests
{
    public class ParagraphsProcessingTests
    {
        [Test]
        public void SimpleParagraph()
        {
            const string ExpectedHtml = @"<body>
  <p>This is a <strong>paragraph</strong></p>
</body>";
            fixture.ProcessText(@"This is a '''paragraph'''", ExpectedHtml);
        }

        [Test]
        public void TextShouldBeEscaped()
        {
            const string ExpectedHtml = @"<body>
  <p>This is &lt;something special&gt;</p>
</body>";
            fixture.ProcessText(@"This is <something special>", ExpectedHtml);
        }

        [Test]
        public void NumberedList()
        {
            const string Text = @"# item 1
# item 2
# item 3
  
# item 4";
            const string ExpectedHtml = @"<body>
  <ol>
    <li>item 1</li><li>item 2</li><li>item 3</li><li>item 4</li>
  </ol>
</body>";
            fixture.ProcessText(Text, ExpectedHtml);
        }

        [Test]
        public void MultilineNumberedListItems()
        {
            const string Text = @"# item 1
# item 2
that continues here";
            const string ExpectedHtml = @"<body>
  <ol>
    <li>item 1</li><li>item 2 that continues here</li>
  </ol>
</body>";
            fixture.ProcessText (Text, ExpectedHtml);
        }

        [Test]
        public void NumberedListBetweenParagraphs()
        {
            const string Text = @"this is a paragraph
# item 1
# item 2

and this is another";
            const string ExpectedHtml = @"<body>
  <p>this is a paragraph</p><ol>
    <li>item 1</li><li>item 2</li>
  </ol><p>and this is another</p>
</body>";
            fixture.ProcessText (Text, ExpectedHtml);
        }

        [Test]
        public void MultilevelNumberedLists()
        {
            const string Text = @"
# item 1
## item 11
## item 22
# item 2";
            const string ExpectedHtml = @"<body>
  <ol>
    <li>item 1<ol>
      <li>item 11</li><li>item 22</li>
    </ol></li><li>item 2</li>
  </ol>
</body>";
            fixture.ProcessText (Text, ExpectedHtml);
        }

        [Test]
        public void BulletList()
        {
            const string Text = @"* item 1
* item 2
* item 3
  
* item 4";
            const string ExpectedHtml = @"<body>
  <ul>
    <li>item 1</li><li>item 2</li><li>item 3</li><li>item 4</li>
  </ul>
</body>";
            fixture.ProcessText(Text, ExpectedHtml);
        }

        [Test]
        public void MultilineBulletListItems()
        {
            const string Text = @"* item 1
* item 2
that continues here";
            const string ExpectedHtml = @"<body>
  <ul>
    <li>item 1</li><li>item 2 that continues here</li>
  </ul>
</body>";
            fixture.ProcessText (Text, ExpectedHtml);
        }

        [Test]
        public void BulletListBetweenParagraphs()
        {
            const string Text = @"this is a paragraph
* item 1
* item 2

and this is another";
            const string ExpectedHtml = @"<body>
  <p>this is a paragraph</p><ul>
    <li>item 1</li><li>item 2</li>
  </ul><p>and this is another</p>
</body>";
            fixture.ProcessText (Text, ExpectedHtml);
        }

        [Test]
        public void MultilevelBulletLists()
        {
            const string Text = @"
* item 1
** item 11
** item 22
* item 2";
            const string ExpectedHtml = @"<body>
  <ul>
    <li>item 1<ul>
      <li>item 11</li><li>item 22</li>
    </ul></li><li>item 2</li>
  </ul>
</body>";
            fixture.ProcessText (Text, ExpectedHtml);
        }

        [Test]
        public void IndentedParagraph ()
        {
            const string ExpectedHtml = @"<body>
  <p class=""indent-1"">This is a <strong>paragraph</strong></p>
</body>";
            fixture.ProcessText (@": This is a '''paragraph'''", ExpectedHtml);
        }

        [SetUp]
        public void Setup()
        {
            fixture = new ProcessingFixture ();
        }

        private ProcessingFixture fixture;
    }
}