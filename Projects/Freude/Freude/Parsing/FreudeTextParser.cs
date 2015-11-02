using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;
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

            switch (firstToken.Type)
            {
                case WikiTextToken.TokenType.Header1Start:
                case WikiTextToken.TokenType.Header2Start:
                case WikiTextToken.TokenType.Header3Start:
                case WikiTextToken.TokenType.Header4Start:
                case WikiTextToken.TokenType.Header5Start:
                case WikiTextToken.TokenType.Header6Start:
                    currentParagraph = null;
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

            int tokenIndex = 1;
            StringBuilder headerText = new StringBuilder();
            if (!ProcessHeaderText(context, tokens, endingTokenNeeded, ref tokenIndex, headerText))
                return;

            HeaderElement headerEl = new HeaderElement (headerText.ToString ().Trim (), headerLevel);
            doc.Children.Add (headerEl);

            tokenIndex++;
            string anchorId;
            if (!ProcessHeaderAnchor (context, tokens, ref tokenIndex, out anchorId))
                return;

            ProcessUntilEnd(
                tokens, 
                ref tokenIndex, 
                t =>
                {
                    context.ReportError("Unexpected token at the end of header");
                    return false;
                });

            headerEl.AnchorId = anchorId;
        }

        private static bool ProcessHeaderText(
            ParsingContext context, 
            IList<WikiTextToken> tokens, 
            WikiTextToken.TokenType endingTokenNeeded, 
            ref int tokenIndex, 
            StringBuilder headerText)
        {
            return ProcessUntilToken(
                context, 
                tokens, 
                endingTokenNeeded, 
                ref tokenIndex, 
                t =>
                {
                    switch (t.Type)
                    {
                        case WikiTextToken.TokenType.Text:
                            headerText.Append(t.Text);
                            return true;
                        case WikiTextToken.TokenType.DoubleApostrophe:
                        case WikiTextToken.TokenType.TripleApostrophe:
                            throw new NotImplementedException("todo next: add support");
                        default:
                            context.ReportError("Unexpected token in header definition: {0}".Fmt(t.Text));
                            return false;
                    }
                });
        }

        private static bool ProcessHeaderAnchor(
            ParsingContext context, 
            IList<WikiTextToken> tokens, 
            ref int tokenIndex,
            out string anchorId)
        {
            anchorId = null;
            if (tokenIndex < tokens.Count)
            {
                WikiTextToken token = tokens[tokenIndex];
                if (token.Type == WikiTextToken.TokenType.HeaderAnchor)
                {
                    if (!FetchHeaderAnchor(context, tokens, tokenIndex, out anchorId))
                        return false;
                }
                else
                {
                    context.ReportError ("Unexpected tokens after ending header token");
                    return false;
                }
            }

            return true;
        }

        private static bool FetchHeaderAnchor(ParsingContext context, IList<WikiTextToken> tokens, int tokenIndex, out string anchorId)
        {
            anchorId = null;

            WikiTextToken token = ExpectToken (context, tokens, tokenIndex, WikiTextToken.TokenType.Text);
            if (token == null)
                return false;

            string potentialAnchorId = token.Text;
            if (!ValidateAnchor(potentialAnchorId))
            {
                context.ReportError("Invalid header anchor ID: '{0}'".Fmt(potentialAnchorId));
                return false;
            }

            anchorId = token.Text;
            return true;
        }

        private static bool ProcessUntilToken (
            ParsingContext context, 
            IList<WikiTextToken> tokens, 
            WikiTextToken.TokenType untilType,
            ref int tokenIndex,
            Func<WikiTextToken, bool> tokenFunc)
        {
            for (; tokenIndex < tokens.Count; tokenIndex++)
            {
                WikiTextToken token = tokens[tokenIndex];
                if (token.Type == untilType)
                    return true;

                if (!tokenFunc(token))
                    return false;
            }

            context.ReportError("Expected token type {0}, but it is missing".Fmt(untilType));
            return false;
        }

        private static WikiTextToken ExpectToken(
            ParsingContext context,
            IList<WikiTextToken> tokens,
            int tokenIndex,
            WikiTextToken.TokenType expectedTokenType)
        {
            if (tokenIndex >= tokens.Count)
            {
                context.ReportError ("Unexpected end, expected token '{0}'".Fmt (expectedTokenType));
                return null;
            }

            WikiTextToken token = tokens[tokenIndex];
            if (token.Type != WikiTextToken.TokenType.Text)
            {
                context.ReportError ("Expected token '{0}' but got '{1}".Fmt(expectedTokenType, token.Type));
                return null;
            }

            return token;
        }

        private static bool ProcessUntilEnd (
            IList<WikiTextToken> tokens, 
            ref int tokenIndex,
            Func<WikiTextToken, bool> tokenFunc)
        {
            for (; tokenIndex < tokens.Count; tokenIndex++)
            {
                WikiTextToken token = tokens[tokenIndex];
                if (!tokenFunc(token))
                    return false;
            }

            return true;
        }

        private static bool ValidateAnchor (string anchorId)
        {
            if (anchorId.Length == 0)
                return false;

            return anchorRegex.IsMatch (anchorId);
        }

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
        private static readonly Regex anchorRegex = new Regex(@"^[\d\w\-\._~!\$\&`\(\)\*\+\,\;\=\:\@]+$", RegexOptions.Compiled);
    }
}