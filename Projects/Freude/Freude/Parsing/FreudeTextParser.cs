using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
            TextElement.TextStyle? currentParagraphTextStyle = null;

            while (!context.EndOfText)
            {
                if (!ParseLine(doc, ref currentParagraph, ref currentParagraphTextStyle, context))
                    break;
            }

            FinalizeCurrentParagraph(ref currentParagraph);

            return doc;
        }

        private bool ParseLine(
            IDocumentElementContainer doc, 
            ref ParagraphElement currentParagraph,
            ref TextElement.TextStyle? currentParagraphTextStyle,
            ParsingContext context)
        {
            Contract.Requires(doc != null);

            if (context.EndOfText)
                return false;

            string lineText = context.CurrentLine;

            // we can ignore lines with nothing but whitespace
            if (lineText.Length == 0 || lineText.Trim ().Length == 0)
                FinalizeCurrentParagraph(ref currentParagraph);
            else
                ProcessLine(doc, context, ref currentParagraph, ref currentParagraphTextStyle, lineText);

            context.IncrementLineCounter ();
            return true;
        }

        private void ProcessLine(
            IDocumentElementContainer doc, 
            ParsingContext context,
            ref ParagraphElement currentParagraph, 
            ref TextElement.TextStyle? currentParagraphTextStyle,
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
                    FinalizeCurrentParagraph(ref currentParagraph);
                    HandleHeaderLine(doc, context, tokenBuffer);
                    break;

                case WikiTextToken.TokenType.Text:
                case WikiTextToken.TokenType.DoubleApostrophe:
                case WikiTextToken.TokenType.TripleApostrophe:
                case WikiTextToken.TokenType.DoubleSquareBracketsOpen:
                    HandleText (doc, context, tokenBuffer, ref currentParagraph, ref currentParagraphTextStyle);
                    break;

                default:
                    throw new NotImplementedException("todo next: {0}".Fmt(firstToken.Type));
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
            doc.AddChild(headerEl);

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

        private static void HandleText(
            IDocumentElementContainer doc, 
            ParsingContext context, 
            TokenBuffer tokenBuffer, 
            ref ParagraphElement currentParagraph,
            ref TextElement.TextStyle? currentStyle)
        {
            Contract.Requires(doc != null);
            Contract.Requires(context != null);
            Contract.Requires(tokenBuffer != null);

            ParagraphElement paragraph = currentParagraph;
            TextElement.TextStyle? style = currentStyle;

            TextParsingMode parsingMode = TextParsingMode.RegularText;
            StringBuilder internalLinkPageName = new StringBuilder();
            StringBuilder internalLinkDescription = null;

            ProcessUntilEnd(
                tokenBuffer,
                t =>
                {
                    switch (t.Type)
                    {
                        case WikiTextToken.TokenType.Text:
                            return HandleTextTokenInText(
                                doc, parsingMode, t, internalLinkPageName, ref internalLinkDescription, ref paragraph, ref style);

                        case WikiTextToken.TokenType.TripleApostrophe:
                            return HandleTripleApostropheTokenInText(ref style);

                        case WikiTextToken.TokenType.DoubleApostrophe:
                            return HandleDoubleApostropheTokenInText(ref style);

                        case WikiTextToken.TokenType.DoubleSquareBracketsOpen:
                            return HandleDoubleSquareBracketsOpenTokenInText(context, t, ref parsingMode);

                        case WikiTextToken.TokenType.Pipe:
                            return HandlePipeTokenInText(context, t, ref parsingMode);

                        case WikiTextToken.TokenType.DoubleSquareBracketsClose:
                            return HandleDoubleSquareBracketsCloseTokenInText(
                                doc, 
                                context, 
                                ref parsingMode, 
                                t, 
                                internalLinkPageName, 
                                internalLinkDescription, 
                                ref paragraph, 
                                ref style);

                        default:
                            throw new NotImplementedException("todo next: {0}".Fmt(t.Type));
                    }
                });

            currentParagraph = paragraph;
            currentStyle = style;
        }

        private static bool HandleTextTokenInText(
            IDocumentElementContainer doc, 
            TextParsingMode parsingMode, 
            WikiTextToken token,
            StringBuilder internalLinkPageName, 
            ref StringBuilder internalLinkDescription, 
            ref ParagraphElement paragraph,
            ref TextElement.TextStyle? style)
        {
            Contract.Requires(doc != null);
            Contract.Requires(token != null);
            Contract.Requires(internalLinkPageName != null);
            Contract.Requires(internalLinkDescription != null);

            switch (parsingMode)
            {
                case TextParsingMode.RegularText:
                    AddTextToParagraph(doc, ref paragraph, ref style, token.Text);
                    return true;
                case TextParsingMode.InternalLinkPageName:
                    internalLinkPageName.Append(token.Text);
                    return true;
                case TextParsingMode.InternalLinkDescription:
                    if (internalLinkDescription == null)
                        internalLinkDescription = new StringBuilder();
                    internalLinkDescription.Append(token.Text);
                    return true;
                default:
                    throw new NotImplementedException("todo next:");
            }
        }

        private static bool HandleTripleApostropheTokenInText(ref TextElement.TextStyle? style)
        {
            switch (style)
            {
                case null:
                case TextElement.TextStyle.Regular:
                    style = TextElement.TextStyle.Bold;
                    break;
                case TextElement.TextStyle.Bold:
                    style = TextElement.TextStyle.Regular;
                    break;
                case TextElement.TextStyle.Italic:
                    style = TextElement.TextStyle.BoldItalic;
                    break;
                case TextElement.TextStyle.BoldItalic:
                    style = TextElement.TextStyle.Italic;
                    break;
            }

            return true;
        }

        private static bool HandleDoubleApostropheTokenInText(ref TextElement.TextStyle? style)
        {
            switch (style)
            {
                case null:
                case TextElement.TextStyle.Regular:
                    style = TextElement.TextStyle.Italic;
                    break;
                case TextElement.TextStyle.Italic:
                    style = TextElement.TextStyle.Regular;
                    break;
                case TextElement.TextStyle.Bold:
                    style = TextElement.TextStyle.BoldItalic;
                    break;
                case TextElement.TextStyle.BoldItalic:
                    style = TextElement.TextStyle.Bold;
                    break;
            }

            return true;
        }

        private static bool HandleDoubleSquareBracketsOpenTokenInText(
            ParsingContext context, 
            WikiTextToken token,
            ref TextParsingMode parsingMode)
        {
            Contract.Requires(context != null);
            Contract.Requires(token != null);

            if (parsingMode != TextParsingMode.RegularText)
            {
                context.ReportError("Token {0} is not allowed here".Fmt(token.Text));
                return false;
            }

            parsingMode = TextParsingMode.InternalLinkPageName;
            return true;
        }

        private static bool HandlePipeTokenInText(ParsingContext context, WikiTextToken token, ref TextParsingMode parsingMode)
        {
            Contract.Requires(context != null);
            Contract.Requires(token != null);

            if (parsingMode != TextParsingMode.InternalLinkPageName)
            {
                context.ReportError("Token {0} is not allowed here".Fmt(token.Text));
                return false;
            }

            parsingMode = TextParsingMode.InternalLinkDescription;
            return true;
        }

        private static bool HandleDoubleSquareBracketsCloseTokenInText(
            IDocumentElementContainer doc, 
            ParsingContext context,
            ref TextParsingMode parsingMode, 
            WikiTextToken token, 
            StringBuilder internalLinkPageName,
            StringBuilder internalLinkDescription, 
            ref ParagraphElement paragraph, 
            ref TextElement.TextStyle? style)
        {
            Contract.Requires(doc != null);
            Contract.Requires(context != null);
            Contract.Requires(token != null);
            Contract.Requires(internalLinkPageName != null);
            Contract.Requires(internalLinkDescription != null);

            if (parsingMode != TextParsingMode.InternalLinkPageName
                && parsingMode != TextParsingMode.InternalLinkDescription)
            {
                context.ReportError("Token {0} is not allowed here".Fmt(token.Text));
                return false;
            }

            parsingMode = TextParsingMode.RegularText;
            return AddInternalLink(
                doc, context, internalLinkPageName, internalLinkDescription, ref paragraph, ref style);
        }

        private static bool AddInternalLink(
            IDocumentElementContainer doc, 
            ParsingContext context,
            StringBuilder internalLinkPageName, 
            StringBuilder internalLinkDescription, 
            ref ParagraphElement paragraph, 
            ref TextElement.TextStyle? style)
        {
            Contract.Requires(doc != null);
            Contract.Requires(context != null);
            Contract.Requires(internalLinkPageName != null);

            string pageName = internalLinkPageName.ToString().Trim();
            if (pageName.Length == 0)
            {
                context.ReportError ("Internal link has an empty page name");
                return false;
            }

            string description = null;
            if (internalLinkDescription != null)
            {
                description = internalLinkDescription.ToString ().Trim ();
                if (description.Length == 0)
                {
                    context.ReportError("Internal link ('{0}') has an empty description".Fmt(pageName));
                    return false;
                }
            }

            InternalLinkElement linkEl = new InternalLinkElement(pageName, description);
            AddToParagraph(doc, ref paragraph, ref style, linkEl);

            return true;
        }

        private static bool ProcessHeaderText(
            ParsingContext context, 
            TokenBuffer tokenBuffer, 
            WikiTextToken.TokenType endingTokenNeeded, 
            StringBuilder headerText)
        {
            Contract.Requires(context != null);
            Contract.Requires(tokenBuffer != null);
            Contract.Requires(headerText != null);

            WikiTextToken.TokenType? actualEndingToken = ProcessUntilToken(
                context, 
                tokenBuffer, 
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
                }, 
                endingTokenNeeded);

            return actualEndingToken != null && actualEndingToken == endingTokenNeeded;
        }

        private static bool ProcessHeaderAnchor(ParsingContext context, TokenBuffer tokenBuffer, out string anchorId)
        {
            Contract.Requires(context != null);
            Contract.Requires(tokenBuffer != null);

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
                    context.ReportError("Unexpected tokens after ending header token");
                    return false;
                }
            }

            return true;
        }

        private static bool FetchHeaderAnchor(ParsingContext context, TokenBuffer tokenBuffer, out string anchorId)
        {
            Contract.Requires(context != null);
            Contract.Requires(tokenBuffer != null);

            anchorId = null;

            WikiTextToken token = ExpectToken(context, tokenBuffer, WikiTextToken.TokenType.Text);
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

        private static WikiTextToken.TokenType? ProcessUntilToken(
            ParsingContext context, 
            TokenBuffer tokenBuffer, 
            Func<WikiTextToken, bool> tokenFunc, 
            params WikiTextToken.TokenType[] untilTypes)
        {
            Contract.Requires(context != null);
            Contract.Requires(tokenBuffer != null);
            Contract.Requires(tokenFunc != null);

            while (!tokenBuffer.EndOfTokens)
            {
                WikiTextToken token = tokenBuffer.Token;
                if (untilTypes.Contains(token.Type))
                    return token.Type;

                if (!tokenFunc(token))
                    return null;

                tokenBuffer.MoveToNextToken();
            }

            context.ReportError("Expected one of token types ({0}), but they are missing".Fmt(untilTypes.Concat(x => x.ToString(), ",")));
            return null;
        }

        private static WikiTextToken ExpectToken(
            ParsingContext context, TokenBuffer tokenBuffer, WikiTextToken.TokenType expectedTokenType)
        {
            Contract.Requires(context != null);
            Contract.Requires(tokenBuffer != null);

            if (tokenBuffer.EndOfTokens)
            {
                context.ReportError("Unexpected end, expected token '{0}'".Fmt(expectedTokenType));
                return null;
            }

            WikiTextToken token = tokenBuffer.Token;
            if (token.Type != WikiTextToken.TokenType.Text)
            {
                context.ReportError("Expected token '{0}' but got '{1}".Fmt(expectedTokenType, token.Type));
                return null;
            }

            return token;
        }

        private static bool ProcessUntilEnd(TokenBuffer tokenBuffer, Func<WikiTextToken, bool> tokenFunc)
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

        private static bool ValidateAnchor(string anchorId)
        {
            Contract.Requires(anchorId != null);

            if (anchorId.Length == 0)
                return false;

            return anchorRegex.IsMatch(anchorId);
        }

        private static void AddTextToParagraph(
            IDocumentElementContainer doc, 
            ref ParagraphElement currentParagraph, 
            ref TextElement.TextStyle? currentStyle, 
            string text)
        {
            Contract.Requires(doc != null);
            Contract.Ensures(currentParagraph != null);
            Contract.Ensures(currentStyle != null);

            CreateParagraphIfNoneIsAlreadyOpen(doc, ref currentParagraph, ref currentStyle);

            bool createNewTextElement = true;

            int childrenCount = currentParagraph.ChildrenCount;
            if (childrenCount > 0)
            {
                IDocumentElement lastChild = currentParagraph.Children[childrenCount - 1];
                TextElement textChild = lastChild as TextElement;
                if (textChild != null)
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    if (textChild.Style == currentStyle.Value)
                        textChild.AppendText(text);
                    else
                    {
                        TextElement newStyleChild = new TextElement(text, currentStyle.Value);
                        currentParagraph.AddChild(newStyleChild);
                    }
                }
                else
                    throw new InvalidOperationException("BUG: is this possible?");
            }
            else
            {
                // ReSharper disable once PossibleInvalidOperationException
                AddToParagraph(
                    doc, ref currentParagraph, ref currentStyle, new TextElement(text, currentStyle.Value));
            }
        }

        private static void AddToParagraph(
            IDocumentElementContainer doc, 
            ref ParagraphElement currentParagraph, 
            ref TextElement.TextStyle? currentStyle, 
            IDocumentElement elementToAdd)
        {
            Contract.Requires(doc != null);
            Contract.Requires(elementToAdd != null);
            Contract.Ensures(currentParagraph != null);

            CreateParagraphIfNoneIsAlreadyOpen(doc, ref currentParagraph, ref currentStyle);

            currentParagraph.AddChild(elementToAdd);
        }

        private static void CreateParagraphIfNoneIsAlreadyOpen(
            IDocumentElementContainer doc, 
            ref ParagraphElement currentParagraph, 
            ref TextElement.TextStyle? textStyle)
        {
            Contract.Requires(doc != null);
            Contract.Ensures(currentParagraph != null);
            Contract.Ensures(textStyle != null);

            if (currentParagraph == null)
            {
                currentParagraph = new ParagraphElement();
                textStyle = textStyle ?? TextElement.TextStyle.Regular;
                doc.AddChild(currentParagraph);
            }
        }

        private static void FinalizeCurrentParagraph(ref ParagraphElement currentParagraph)
        {
            if (currentParagraph != null)
                currentParagraph.Trim();

            currentParagraph = null;
        }

        private readonly IWikiTextTokenizer tokenizer;
        private static readonly Regex anchorRegex = new Regex(@"^[\d\w\-\._~!\$\&`\(\)\*\+\,\;\=\:\@]+$", RegexOptions.Compiled);

        private enum TextParsingMode
        {
            RegularText,
            InternalLinkPageName,
            InternalLinkDescription
        }

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
                        throw new InvalidOperationException("No more tokens");

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