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

            TokenBuffer tokenBuffer = new TokenBuffer(tokens);

            WikiTextToken firstToken = tokenBuffer.Token;

            switch (firstToken.Type)
            {
                case WikiTextToken.TokenType.Header1Start:
                case WikiTextToken.TokenType.Header2Start:
                case WikiTextToken.TokenType.Header3Start:
                case WikiTextToken.TokenType.Header4Start:
                case WikiTextToken.TokenType.Header5Start:
                case WikiTextToken.TokenType.Header6Start:
                    currentParagraph = null;
                    HandleHeaderLine(doc, context, tokenBuffer);
                    break;

                default:
                    throw new NotImplementedException("todo next:");
            }
        }

        private static void HandleHeaderLine(IDocumentElementContainer doc, ParsingContext context, TokenBuffer tokenBuffer)
        {
            Contract.Requires(doc != null);
            Contract.Requires(context != null);
            Contract.Requires(tokenBuffer != null);

            WikiTextToken firstToken = tokenBuffer.Token;

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

            tokenBuffer.MoveToNextToken();

            StringBuilder headerText = new StringBuilder();
            if (!ProcessHeaderText(context, tokenBuffer, endingTokenNeeded, headerText))
                return;

            HeaderElement headerEl = new HeaderElement (headerText.ToString ().Trim (), headerLevel);
            doc.Children.Add (headerEl);

            tokenBuffer.MoveToNextToken();
            string anchorId;
            if (!ProcessHeaderAnchor (context, tokenBuffer, out anchorId))
                return;

            ProcessUntilEnd(
                tokenBuffer, 
                t =>
                {
                    context.ReportError("Unexpected token at the end of header");
                    return false;
                });

            headerEl.AnchorId = anchorId;
        }

        private static bool ProcessHeaderText(
            ParsingContext context, 
            TokenBuffer tokenBuffer, 
            WikiTextToken.TokenType endingTokenNeeded, 
            StringBuilder headerText)
        {
            return ProcessUntilToken(
                context, 
                tokenBuffer, 
                endingTokenNeeded, 
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
            TokenBuffer tokenBuffer, 
            out string anchorId)
        {
            anchorId = null;
            if (!tokenBuffer.EndOfTokens)
            {
                WikiTextToken token = tokenBuffer.Token;
                if (token.Type == WikiTextToken.TokenType.HeaderAnchor)
                {
                    tokenBuffer.MoveToNextToken();
                    if (!FetchHeaderAnchor(context, tokenBuffer, out anchorId))
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

        private static bool FetchHeaderAnchor(
            ParsingContext context, TokenBuffer tokenBuffer, out string anchorId)
        {
            anchorId = null;

            WikiTextToken token = ExpectToken (context, tokenBuffer, WikiTextToken.TokenType.Text);
            if (token == null)
                return false;

            string potentialAnchorId = token.Text;
            if (!ValidateAnchor(potentialAnchorId))
            {
                context.ReportError("Invalid header anchor ID: '{0}'".Fmt(potentialAnchorId));
                return false;
            }

            anchorId = token.Text;
            tokenBuffer.MoveToNextToken();
            return true;
        }

        private static bool ProcessUntilToken (
            ParsingContext context,
            TokenBuffer tokenBuffer, 
            WikiTextToken.TokenType untilType,
            Func<WikiTextToken, bool> tokenFunc)
        {
            Contract.Requires(context != null);
            Contract.Requires(tokenBuffer != null);
            Contract.Requires(tokenFunc != null);

            while (!tokenBuffer.EndOfTokens)
            {
                WikiTextToken token = tokenBuffer.Token;
                if (token.Type == untilType)
                    return true;

                if (!tokenFunc(token))
                    return false;

                tokenBuffer.MoveToNextToken();
            }

            context.ReportError("Expected token type {0}, but it is missing".Fmt(untilType));
            return false;
        }

        private static WikiTextToken ExpectToken(
            ParsingContext context,
            TokenBuffer tokenBuffer,
            WikiTextToken.TokenType expectedTokenType)
        {
            if (tokenBuffer.EndOfTokens)
            {
                context.ReportError ("Unexpected end, expected token '{0}'".Fmt (expectedTokenType));
                return null;
            }

            WikiTextToken token = tokenBuffer.Token;
            if (token.Type != WikiTextToken.TokenType.Text)
            {
                context.ReportError ("Expected token '{0}' but got '{1}".Fmt(expectedTokenType, token.Type));
                return null;
            }

            return token;
        }

        private static bool ProcessUntilEnd (
            TokenBuffer tokenBuffer, 
            Func<WikiTextToken, bool> tokenFunc)
        {
            Contract.Requires(tokenBuffer != null);
            Contract.Requires(tokenFunc != null);

            while (!tokenBuffer.EndOfTokens)
            {
                WikiTextToken token = tokenBuffer.Token;
                if (!tokenFunc(token))
                    return false;

                tokenBuffer.MoveToNextToken();
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

        private class TokenBuffer
        {
            public TokenBuffer(IList<WikiTextToken> tokens)
            {
                Contract.Requires(tokens != null);
                this.tokens = tokens;
            }

            public bool EndOfTokens
            {
                get { return tokenIndex >= tokens.Count; }
            }

            public WikiTextToken Token
            {
                get
                {
                    Contract.Ensures(Contract.Result<WikiTextToken>() != null);

                    if (EndOfTokens)
                        throw new InvalidOperationException ("No more tokens");

                    return tokens[tokenIndex];
                }
            }

            public void MoveToNextToken()
            {
                if (EndOfTokens)
                    throw new InvalidOperationException("No more tokens");

                tokenIndex++;
            }

            [ContractInvariantMethod]
            private void Invariant()
            {
                Contract.Invariant(tokens != null);
                Contract.Invariant(Contract.ForAll(tokens, x => x != null));
                Contract.Invariant(tokenIndex >= 0 && tokenIndex <= tokens.Count);
            }

            private readonly IList<WikiTextToken> tokens;
            private int tokenIndex;
        }
    }
}