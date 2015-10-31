using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Freude.DocModel;

namespace Freude.Parsing
{
    public class FreudeTextParser : IFreudeTextParser
    {
        public FreudeTextParser(IWikiTextTokenizer tokenizer)
        {
            Contract.Requires(tokenizer != null);
            this.tokenizer = tokenizer;
        }

        public DocumentDef ParseText(string text, ParsingContext context)
        {
            DocumentDef doc = new DocumentDef();

            context.SetTextLines(text.SplitIntoLines());
            ParagraphElement currentParagraph = null;

            while (!context.EndOfText)
            {
                if (!ParseLine(doc, ref currentParagraph, context))
                    break;
            }

            return doc;
        }

        private bool ParseLine(
            IDocumentElementContainer doc, 
            ref ParagraphElement currentParagraph,
            ParsingContext context)
        {
            Contract.Requires(doc != null);

            if (context.EndOfText)
                return false;

            string lineText = context.CurrentLine;

            //// we can ignore lines with nothing but whitespace
            //if (lineText.Length == 0 || lineText.Trim().Length == 0)
            //{
            //    context.IncrementLineCounter();
            //    currentParagraph = null;
            //    return true;
            //}

            ProcessLine(doc, context, ref currentParagraph, lineText);

            context.IncrementLineCounter ();
            return true;
        }

        private void ProcessLine(
            IDocumentElementContainer doc, 
            ParsingContext context,
            ref ParagraphElement currentParagraph, 
            string lineText)
        {
            Contract.Requires(doc != null);
            Contract.Requires(lineText != null);

            WikiTokenizationSettings tokenizationSettings = new WikiTokenizationSettings();
            tokenizationSettings.IsWholeLine = true;
            IList<WikiTextToken> tokens = tokenizer.TokenizeWikiText(lineText, tokenizationSettings);

            if (tokens.Count == 0)
                throw new NotImplementedException("todo next:");

            WikiTextToken firstToken = tokens[0];
            if (firstToken.Scope == WikiTextToken.TokenScope.BeginLineOnly)
                currentParagraph = null;

            switch (firstToken.Type)
            {
                case WikiTextToken.TokenType.Header1Start:
                case WikiTextToken.TokenType.Header2Start:
                case WikiTextToken.TokenType.Header3Start:
                case WikiTextToken.TokenType.Header4Start:
                case WikiTextToken.TokenType.Header5Start:
                case WikiTextToken.TokenType.Header6Start:
                    HandleHeaderLine(doc, context, tokens);
                    break;

                default:
                    throw new NotImplementedException("todo next:");
            }
        }

        private static void HandleHeaderLine(IDocumentElementContainer doc, ParsingContext context, IList<WikiTextToken> tokens)
        {
            Contract.Requires(doc != null);
            Contract.Requires(context != null);
            Contract.Requires(tokens != null);
            Contract.Requires(tokens.Count > 0);

            WikiTextToken firstToken = tokens[0];

            WikiTextToken.TokenType endingTokenNeeded;
            int headerLevel;
            switch (firstToken.Type)
            {
                case WikiTextToken.TokenType.Header1Start:
                    headerLevel = 1;
                    endingTokenNeeded = WikiTextToken.TokenType.Header1End;
                    break;
                case WikiTextToken.TokenType.Header2Start:
                    headerLevel = 2;
                    endingTokenNeeded = WikiTextToken.TokenType.Header2End;
                    break;
                case WikiTextToken.TokenType.Header3Start:
                    headerLevel = 3;
                    endingTokenNeeded = WikiTextToken.TokenType.Header3End;
                    break;
                case WikiTextToken.TokenType.Header4Start:
                    headerLevel = 4;
                    endingTokenNeeded = WikiTextToken.TokenType.Header4End;
                    break;
                case WikiTextToken.TokenType.Header5Start:
                    headerLevel = 5;
                    endingTokenNeeded = WikiTextToken.TokenType.Header5End;
                    break;
                case WikiTextToken.TokenType.Header6Start:
                    headerLevel = 6;
                    endingTokenNeeded = WikiTextToken.TokenType.Header6End;
                    break;

                default:
                    throw new InvalidOperationException("BUG");
            }

            StringBuilder headerText = new StringBuilder();
            for (int i = 1; i < tokens.Count; i++)
            {
                WikiTextToken token = tokens[i];
                if (token.Type == endingTokenNeeded)
                {
                    if (i < tokens.Count - 1)
                        context.ReportError("Unexpected tokens after ending header token");

                    HeaderElement headerEl = new HeaderElement (headerText.ToString().Trim(), headerLevel);
                    doc.Children.Add(headerEl);
                    return;
                }

                switch (token.Type)
                {
                    case WikiTextToken.TokenType.Text:
                        headerText.Append(token.Text);
                        break;
                    case WikiTextToken.TokenType.DoubleApostrophe:
                    case WikiTextToken.TokenType.TripleApostrophe:
                        throw new NotImplementedException("todo next: add support");
                    default:
                        context.ReportError("Unexpected token in header definition: {0}".Fmt(token.Text));
                        return;
                }
            }

            context.ReportError("Missing ending header token");
        }

        //private static void ParseHeaderAnchor(
        //    ParsingContext context, HeaderElement headerElement, string lineText, int startingIndex)
        //{
        //    Contract.Requires(context != null);
        //    Contract.Requires(headerElement != null);
        //    Contract.Requires(lineText != null);

        //    int anchorHashIndex = lineText.IndexOf(TokenHash, startingIndex);
        //    if (anchorHashIndex < 0)
        //        return;

        //    string anchorId = lineText.Substring(anchorHashIndex + 1).TrimEnd();

        //    if (!ValidateAnchor(anchorId))
        //        context.ReportError("Invalid anchor ID '{0}'".Fmt(anchorId), anchorHashIndex + 1);

        //    headerElement.AnchorId = anchorId;
        //}

        //private static bool ValidateAnchor(string anchorId)
        //{
        //    if (anchorId.Length == 0)
        //        return false;

        //    return anchorRegex.IsMatch(anchorId);
        //}

        //private static void AddTextToParagraph (IDocumentElementContainer doc, ref ParagraphElement currentParagraph, string text)
        //{
        //    Contract.Ensures (currentParagraph != null);

        //    CreateParagraphIfNoneIsAlreadyOpen(doc, ref currentParagraph);

        //    int childrenCount = currentParagraph.Children.Count;
        //    if (childrenCount > 0)
        //    {
        //        IDocumentElement lastChild = currentParagraph.Children[childrenCount - 1];
        //        TextElement textChild = lastChild as TextElement;
        //        if (textChild != null)
        //        {
        //            textChild.AppendText(text);
        //            return;
        //        }
        //    }

        //    AddElement(doc, ref currentParagraph, new TextElement(text));
        //}

        //private static void AddElement (IDocumentElementContainer doc, ref ParagraphElement currentParagraph, IDocumentElement textElement)
        //{
        //    Contract.Ensures (currentParagraph != null);

        //    CreateParagraphIfNoneIsAlreadyOpen(doc, ref currentParagraph);

        //    currentParagraph.Children.Add(textElement);
        //}

        //private static void CreateParagraphIfNoneIsAlreadyOpen (IDocumentElementContainer doc, ref ParagraphElement currentParagraph)
        //{
        //    Contract.Ensures(currentParagraph != null);

        //    if (currentParagraph == null)
        //    {
        //        currentParagraph = new ParagraphElement();
        //        doc.Children.Add(currentParagraph);
        //    }
        //}

        private readonly IWikiTextTokenizer tokenizer;
        //private static readonly Regex anchorRegex = new Regex(@"^[\d\w\-\._~!\$\&`\(\)\*\+\,\;\=\:\@]+$", RegexOptions.Compiled);
    }
}