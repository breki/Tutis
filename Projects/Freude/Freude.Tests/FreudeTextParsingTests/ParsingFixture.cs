using System;
using Freude.DocModel;
using Freude.Parsing;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class ParsingFixture
    {
        public ParsingFixture()
        {
            context = new ParsingContext ();
            parser = new FreudeTextParser ();
        }

        public ParsingFixture Parse(string text)
        {
            doc = parser.ParseText (text, context);
            return this;
        }

        public ParsingFixture AssertNoErrrors()
        {
            Assert.IsFalse (context.HasAnyErrors);
            return this;
        }

        public ParsingFixture AssertChildCount (int expectedCount)
        {
            Assert.AreEqual (expectedCount, doc.Children.Count);
            return this;
        }

        public TElement AssertElement<TElement> (int index, Action<TElement> assertAction = null)
            where TElement : IDocumentElement
        {
            IDocumentElement element = doc.Children[index];
            Assert.IsInstanceOf<TElement> (element);
            if (assertAction != null)
                assertAction ((TElement)element);

            return (TElement)element;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public TElement AssertElement<TElement> (IDocumentElementContainer container, int index, Action<TElement> assertAction = null)
            where TElement : IDocumentElement
        {
            IDocumentElement element = container.Children[index];
            Assert.IsInstanceOf<TElement> (element);
            if (assertAction != null)
                assertAction ((TElement)element);

            return (TElement)element;
        }

        private FreudeTextParser parser;
        private ParsingContext context;
        private DocumentDef doc;
    }
}