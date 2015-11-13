using NUnit.Framework;

namespace Freude.Tests.DocumentProcessingTests
{
    public class ParagraphsProcessingTests
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

        [Test]
        public void MultilineNumberedListItems()
        {
            fixture.ProcessText (
                @"# item 1
# item 2
that continues here",
                @"<body>
  <ol>
    <li>item 1</li><li>item 2 that continues here</li>
  </ol>
</body>");
        }

        [Test]
        public void NumberedListBetweenParagraphs()
        {
            fixture.ProcessText (
                @"this is a paragraph
# item 1
# item 2

and this is another",
                @"<body>
  <p>this is a paragraph</p><ol>
    <li>item 1</li><li>item 2</li>
  </ol><p>and this is another</p>
</body>");
        }

        [Test]
        public void MultilevelNumberedLists()
        {
            fixture.ProcessText (
                @"
# item 1
## item 11
## item 22
# item 2",
                @"<body>
  <ol>
    <li>item 1<ol><li>item 11</li><li>item 22</li></ol></li><li>item 2</li>
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