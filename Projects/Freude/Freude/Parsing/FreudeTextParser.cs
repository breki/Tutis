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
                case WikiTextToken.TokenType.SingleSquareBracketsOpen:
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

            tokenBuffer.ProcessUntilEnd (
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
            InternalLinkIdBuilder internalLinkBuilder = new InternalLinkIdBuilder ();
            StringBuilder textBuilder = null;
            Uri externalLinkUrl = null;

            bool processingSuccessful = tokenBuffer.ProcessUntilEnd (
                t =>
                {
                    switch (t.Type)
                    {
                        case WikiTextToken.TokenType.Text:
                            return HandleTextTokenInText(
                                doc, parsingMode, t, internalLinkBuilder, ref textBuilder, ref paragraph, ref style);

                        case WikiTextToken.TokenType.TripleApostrophe:
                            return HandleTripleApostropheTokenInText(ref style);

                        case WikiTextToken.TokenType.DoubleApostrophe:
                            return HandleDoubleApostropheTokenInText(ref style);

                        case WikiTextToken.TokenType.DoubleSquareBracketsOpen:
                            return HandleDoubleSquareBracketsOpenTokenInText(context, t, ref parsingMode);

                        case WikiTextToken.TokenType.NamespaceSeparator:
                            return HandleNamespaceSeparatorTokenInText (context, parsingMode, internalLinkBuilder, t);

                        case WikiTextToken.TokenType.Pipe:
                            return HandlePipeTokenInText(context, t, ref parsingMode);

                        case WikiTextToken.TokenType.DoubleSquareBracketsClose:
                            return HandleDoubleSquareBracketsCloseTokenInText(
                                doc, 
                                context, 
                                ref parsingMode, 
                                t,
                                internalLinkBuilder, 
                                textBuilder, 
                                ref paragraph, 
                                ref style);

                        case WikiTextToken.TokenType.SingleSquareBracketsOpen:
                            return HandleSingleSquareBracketsOpenTokenInText (context, t, ref parsingMode);

                        case WikiTextToken.TokenType.ExternalLinkUrlLeadingSpace:
                            return HandleExternalLinkUrlLeadingSpaceTokenInText(context, t, parsingMode);

                        case WikiTextToken.TokenType.ExternalLinkUrl:
                            return HandleExternalLinkUrlTokenInText(context, t, ref parsingMode, ref externalLinkUrl);

                        case WikiTextToken.TokenType.SingleSquareBracketsClose:
                            return HandleSingleSquareBracketsCloseTokenInText (
                                doc,
                                context,
                                ref parsingMode,
                                t,
                                externalLinkUrl,
                                textBuilder,
                                ref paragraph,
                                ref style);

                        default:
                            throw new NotImplementedException("todo next: {0}".Fmt(t.Type));
                    }
                });

            if (processingSuccessful)
            {
                if (parsingMode == TextParsingMode.InternalLinkPageName || parsingMode == TextParsingMode.InternalLinkDescription)
                    context.ReportError ("Missing token ']]'");
            }

            currentParagraph = paragraph;
            currentStyle = style;
        }

        private static bool HandleTextTokenInText(
            IDocumentElementContainer doc, 
            TextParsingMode parsingMode, 
            WikiTextToken token,
            InternalLinkIdBuilder linkIdBuilder, 
            ref StringBuilder textBuilder, 
            ref ParagraphElement paragraph,
            ref TextElement.TextStyle? style)
        {
            Contract.Requires(doc != null);
            Contract.Requires(token != null);
            Contract.Requires(linkIdBuilder != null);

            switch (parsingMode)
            {
                case TextParsingMode.RegularText:
                    AddTextToParagraph(doc, ref paragraph, ref style, token.Text);
                    return true;
                case TextParsingMode.InternalLinkPageName:
                    linkIdBuilder.AppendText(token.Text);
                    return true;
                case TextParsingMode.InternalLinkDescription:
                    if (textBuilder == null)
                        textBuilder = new StringBuilder();
                    textBuilder.Append(token.Text);
                    return true;
                case TextParsingMode.ExternalLinkDescription:
                    if (textBuilder == null)
                        textBuilder = new StringBuilder();
                    textBuilder.Append(token.Text);
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

        private static bool HandleNamespaceSeparatorTokenInText(
            ParsingContext context, TextParsingMode parsingMode, InternalLinkIdBuilder linkIdBuilder, WikiTextToken token)
        {
            Contract.Requires(context != null);
            Contract.Requires(linkIdBuilder != null);
            Contract.Requires(token != null);

            if (parsingMode != TextParsingMode.InternalLinkPageName)
            {
                context.ReportError ("Token {0} is not allowed here".Fmt (token.Text));
                return false;
            }

            linkIdBuilder.AddSeparator();

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
            InternalLinkIdBuilder linkIdBuilder,
            StringBuilder internalLinkDescription, 
            ref ParagraphElement paragraph, 
            ref TextElement.TextStyle? style)
        {
            Contract.Requires(doc != null);
            Contract.Requires(context != null);
            Contract.Requires(token != null);
            Contract.Requires(linkIdBuilder != null);
            Contract.Requires(internalLinkDescription != null);

            if (parsingMode != TextParsingMode.InternalLinkPageName
                && parsingMode != TextParsingMode.InternalLinkDescription)
            {
                context.ReportError("Token {0} is not allowed here".Fmt(token.Text));
                return false;
            }

            parsingMode = TextParsingMode.RegularText;
            return AddInternalLink(
                doc, context, linkIdBuilder, internalLinkDescription, ref paragraph, ref style);
        }

        private static bool HandleSingleSquareBracketsOpenTokenInText (
            ParsingContext context, WikiTextToken token, ref TextParsingMode parsingMode)
        {
            Contract.Requires (context != null);
            Contract.Requires (token != null);

            if (parsingMode != TextParsingMode.RegularText)
            {
                context.ReportError ("Token {0} is not allowed here".Fmt (token.Text));
                return false;
            }

            parsingMode = TextParsingMode.ExternalLinkUrl;
            return true;
        }

        private static bool HandleExternalLinkUrlLeadingSpaceTokenInText (
            ParsingContext context, 
            WikiTextToken token, 
            TextParsingMode parsingMode)
        {
            if (parsingMode != TextParsingMode.ExternalLinkUrl)
            {
                context.ReportError ("Token {0} is not allowed here".Fmt (token.Text));
                return false;
            }

            return true;
        }

        private static bool HandleExternalLinkUrlTokenInText (
            ParsingContext context, 
            WikiTextToken token, 
            ref TextParsingMode parsingMode, 
            ref Uri externalLinkUrl)
        {
            if (parsingMode != TextParsingMode.ExternalLinkUrl)
            {
                context.ReportError ("Token {0} is not allowed here".Fmt (token.Text));
                return false;
            }

            externalLinkUrl = new Uri(token.Text.Trim());
            parsingMode = TextParsingMode.ExternalLinkDescription;

            return true;
        }

        private static bool HandleSingleSquareBracketsCloseTokenInText (
            IDocumentElementContainer doc, 
            ParsingContext context, 
            ref TextParsingMode parsingMode, 
            WikiTextToken token, 
            Uri externalLinkUrl, 
            StringBuilder textBuilder, 
            ref ParagraphElement paragraph, 
            ref TextElement.TextStyle? style)
        {
            if (parsingMode != TextParsingMode.ExternalLinkUrl
                && parsingMode != TextParsingMode.ExternalLinkDescription)
            {
                context.ReportError ("Token {0} is not allowed here".Fmt (token.Text));
                return false;
            }

            parsingMode = TextParsingMode.RegularText;
            return AddExternalLink (doc, externalLinkUrl, textBuilder, ref paragraph, ref style);
        }

        private static bool AddInternalLink(
            IDocumentElementContainer doc, 
            ParsingContext context,
            InternalLinkIdBuilder linkIdBuilder, 
            StringBuilder internalLinkDescription, 
            ref ParagraphElement paragraph, 
            ref TextElement.TextStyle? style)
        {
            Contract.Requires(doc != null);
            Contract.Requires(context != null);
            Contract.Requires(linkIdBuilder != null);

            InternalLinkId linkId = linkIdBuilder.Build (context);

            if (linkId == null)
                return false;

            string description = null;
            if (internalLinkDescription != null)
            {
                description = internalLinkDescription.ToString ().Trim ();
                if (description.Length == 0)
                {
                    context.ReportError("Internal link ('{0}') has an empty description".Fmt(linkId));
                    return false;
                }
            }

            InternalLinkElement linkEl = new InternalLinkElement(linkId, description);
            AddToParagraph(doc, ref paragraph, ref style, linkEl);

            return true;
        }

        private static bool AddExternalLink(
            IDocumentElementContainer doc, 
            Uri externalLinkUrl, 
            StringBuilder externalLinkDescription,
            ref ParagraphElement paragraph, 
            ref TextElement.TextStyle? style)
        {
            Contract.Requires(doc != null);
            Contract.Requires(externalLinkUrl != null);

            string description = null;
            if (externalLinkDescription != null)
            {
                description = externalLinkDescription.ToString ().Trim ();
                if (description.Length == 0)
                    description = null;
            }

            ExternalLinkElement linkEl = new ExternalLinkElement (externalLinkUrl, description);
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

            WikiTextToken.TokenType? actualEndingToken = tokenBuffer.ProcessUntilToken (
                context, 
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

            WikiTextToken token = tokenBuffer.ExpectToken(context, WikiTextToken.TokenType.Text);
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
            Contract.Ensures(currentStyle.HasValue);

            CreateParagraphIfNoneIsAlreadyOpen(doc, ref currentParagraph, ref currentStyle);

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
                        AddTextElementToParagraph(currentParagraph, text, currentStyle.Value);
                }
                else if (lastChild is InternalLinkElement)
                    // ReSharper disable once PossibleInvalidOperationException
                    AddTextElementToParagraph (currentParagraph, text, currentStyle.Value);
                else 
                    throw new InvalidOperationException("BUG: is this possible?");
            }
            else
            {
                AddToParagraph(
                    // ReSharper disable once PossibleInvalidOperationException
                    doc, ref currentParagraph, ref currentStyle, CreateTextElement (text, currentStyle.Value));
            }
        }

        private static void AddTextElementToParagraph(ParagraphElement paragraph, string text, TextElement.TextStyle currentStyle)
        {
            Contract.Requires(paragraph != null);
            Contract.Requires(text != null);
            TextElement newStyleChild = CreateTextElement(text, currentStyle);
            paragraph.AddChild(newStyleChild);
        }

        private static TextElement CreateTextElement(string text, TextElement.TextStyle currentStyle)
        {
            Contract.Requires(text != null);
            return new TextElement (text, currentStyle);
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
            InternalLinkDescription,
            ExternalLinkUrl,
            ExternalLinkDescription,
        }
    }
}