using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using Brejc.Common;
using Freude.DocModel;
using Freude.Parsing;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class ParsingFixture
    {
        public DocumentDef Doc
        {
            get { return doc; }
        }

        public ParsingFixture()
        {
            context = new ParsingContext ();
            tokenizer = new WikiTextTokenizer();
            parser = new FreudeTextParser (tokenizer);
        }

        public ParsingFixture Parse(string text)
        {
            Contract.Requires(text != null);
            Contract.Ensures (Contract.Result<ParsingFixture> () != null);

            doc = parser.ParseText (text, context);
            return this;
        }

        public ParsingFixture AssertNoErrors()
        {
            Contract.Ensures (Contract.Result<ParsingFixture> () != null);

            Assert.IsFalse (context.HasAnyErrors, context.Errors.AsQueryable ().Select (x => x.Item1).Concat (x => x, Environment.NewLine));
            return this;
        }

        public ParsingFixture AssertError (string expectedErrorMessage)
        {
            Assert.IsTrue(context.HasAnyErrors, "Parsing errors expected but there were none");
            Assert.IsTrue(
                context.Errors.Any(x => string.Equals(x.Item1, expectedErrorMessage)), 
                "Parsing error '{0}' expected, but it is missing", 
                expectedErrorMessage);
            return this;
        }

        public ParsingFixture AssertChildCount (int expectedCount)
        {
            Contract.Ensures (Contract.Result<ParsingFixture> () != null);

            Assert.AreEqual (expectedCount, doc.ChildrenCount);
            return this;
        }

        public TElement AssertElement<TElement> (int index, Action<TElement> assertAction = null)
            where TElement : IDocumentElement
        {
            Contract.Requires(index >= 0);
            Contract.Ensures(Contract.Result<TElement>() != null);

            IDocumentElement element = doc.Children[index];
            Assert.IsInstanceOf<TElement> (element);
            if (assertAction != null)
                assertAction ((TElement)element);

            return (TElement)element;
        }

        [SuppressMessage ("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public TElement AssertElement<TElement> (IDocumentElementContainer container, int index, Action<TElement> assertAction = null)
            where TElement : IDocumentElement
        {
            Contract.Requires (container != null);
            Contract.Requires (index >= 0);
            Contract.Ensures (Contract.Result<TElement> () != null);

            IDocumentElement element = container.Children[index];
            Assert.IsInstanceOf<TElement> (element);
            if (assertAction != null)
                assertAction ((TElement)element);

            return (TElement)element;
        }

        [SuppressMessage ("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public ParsingFixture AssertText (
            IDocumentElementContainer container, int index, string expectedText, TextElement.TextStyle? expectedStyle = null)
        {
            Contract.Requires (container != null);
            Contract.Requires(expectedText != null);
            Contract.Requires (index >= 0);

            IDocumentElement element = container.Children[index];
            Assert.IsInstanceOf<TextElement> (element);
            TextElement tel = (TextElement)element;
            Assert.AreEqual(expectedText, tel.Text);
            if (expectedStyle.HasValue)
                Assert.AreEqual(expectedStyle.Value, tel.Style);

            return this;
        }

        public IList<WikiTextToken> TokenizePart(string wikiText)
        {
            WikiTokenizationSettings settings = new WikiTokenizationSettings();
            settings.IsWholeLine = false;
            return tokenizer.TokenizeWikiText(wikiText, settings);
        }

        public IList<WikiTextToken> TokenizeWholeLine(string wikiText)
        {
            WikiTokenizationSettings settings = new WikiTokenizationSettings ();
            settings.IsWholeLine = true;
            return tokenizer.TokenizeWikiText (wikiText, settings);
        }

        private readonly WikiTextTokenizer tokenizer;
        private readonly FreudeTextParser parser;
        private readonly ParsingContext context;
        private DocumentDef doc;
    }
}