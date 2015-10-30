using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Freude.DocModel;

namespace Freude.Parsing
{
    public class FreudeTextParser : IFreudeTextParser
    {
        private const char CharHash = '#';
        private const char CharEquals = '=';

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

        private static bool ParseLine(
            IDocumentElementContainer doc, 
            ref ParagraphElement currentParagraph,
            ParsingContext context)
        {
            Contract.Requires(doc != null);

            if (context.EndOfText)
                return false;

            string lineText = context.CurrentLine;

            // we can ignore lines with nothing but whitespace
            if (lineText.Trim().Length == 0)
            {
                context.IncrementLineCounter();
                currentParagraph = null;
                return true;
            }

            int cursor = 0;

            char startingChar = lineText[cursor];

            switch (startingChar)
            {
                case CharHash:
                {
                    currentParagraph = null;

                    while (lineText[cursor] == CharHash)
                    {
                        cursor++;
                        if (cursor == lineText.Length)
                            throw new NotImplementedException("todo next: the whole line consists of {0}".Fmt(CharHash));
                    }

                    HandleHeaderWithHashChar(doc, lineText, cursor);
                    context.IncrementLineCounter ();
                    return true;
                }

                case CharEquals:
                {
                    currentParagraph = null;

                    while (lineText[cursor] == CharEquals)
                    {
                        cursor++;
                        if (cursor == lineText.Length)
                            throw new NotImplementedException ("todo next: the whole line consists of {0}".Fmt (CharHash));
                    }

                    HandleHeaderWithEqualsChar(doc, context, lineText, cursor);
                    context.IncrementLineCounter ();
                    return true;
                }
            }

            while (cursor < lineText.Length)
            {
                int i = lineText.IndexOf("[[", cursor, StringComparison.Ordinal);

                if (i >= 0)
                {
                    string part = lineText.Substring(cursor, i - cursor).Trim();
                    if (part.Length > 0)
                        AddTextToParagraph(doc, ref currentParagraph, part);

                    int j = lineText.IndexOf("]]", i, StringComparison.Ordinal);

                    if (j < 0)
                        throw new NotImplementedException("todo next");

                    int uriStartIndex = i + 2;
                    if (uriStartIndex >= lineText.Length)
                        throw new NotImplementedException ("todo next");

                    int uriLength = j - uriStartIndex;
                    if (uriLength <= 0)
                        throw new NotImplementedException("todo next");

                    Uri url = new Uri(lineText.Substring(uriStartIndex, uriLength));
                    ImageElement imageElement = new ImageElement(url);
                    AddElement(doc, ref currentParagraph, imageElement);

                    cursor = j + 2;
                }
                else
                {
                    string part = lineText.Substring(cursor).Trim();
                    if (part.Length > 0)
                        AddTextToParagraph(doc, ref currentParagraph, part);

                    break;
                }
            }

            context.IncrementLineCounter ();
            return true;
        }

        private static void HandleHeaderWithHashChar(IDocumentElementContainer doc, string lineText, int headerLevel)
        {
            string headerText = lineText.Substring(headerLevel).Trim();

            HeaderElement headerElement = new HeaderElement(headerText, headerLevel);
            doc.Children.Add(headerElement);
        }

        private static void HandleHeaderWithEqualsChar(
            IDocumentElementContainer doc, 
            ParsingContext context, 
            string lineText, 
            int headerLevel)
        {
            Contract.Requires(doc != null);

            int cursor = headerLevel + 1;
            string suffix = new string(CharEquals, headerLevel);
            int suffixIndex = lineText.IndexOf(suffix, cursor, StringComparison.OrdinalIgnoreCase);

            if (suffixIndex < 0)
            {
                context.ReportError("Missing header suffix");
                return;
            }

            string headerText = lineText.Substring(headerLevel, suffixIndex - headerLevel).Trim();

            HeaderElement headerElement = new HeaderElement(headerText, headerLevel);
            doc.Children.Add(headerElement);

            ParseHeaderAnchor(context, headerElement, lineText, suffixIndex);
        }

        private static void ParseHeaderAnchor(
            ParsingContext context, HeaderElement headerElement, string lineText, int startingIndex)
        {
            Contract.Requires(context != null);
            Contract.Requires(headerElement != null);
            Contract.Requires(lineText != null);

            int anchorHashIndex = lineText.IndexOf(CharHash, startingIndex);
            if (anchorHashIndex < 0)
                return;

            string anchorId = lineText.Substring(anchorHashIndex + 1).TrimEnd();

            if (!ValidateAnchor(anchorId))
                context.ReportError("Invalid anchor ID '{0}'".Fmt(anchorId), anchorHashIndex + 1);

            headerElement.AnchorId = anchorId;
        }

        private static bool ValidateAnchor(string anchorId)
        {
            if (anchorId.Length == 0)
                return false;

            return anchorRegex.IsMatch(anchorId);
        }

        private static void AddTextToParagraph (IDocumentElementContainer doc, ref ParagraphElement currentParagraph, string text)
        {
            Contract.Ensures (currentParagraph != null);

            CreateParagraphIfNoneIsAlreadyOpen(doc, ref currentParagraph);

            int childrenCount = currentParagraph.Children.Count;
            if (childrenCount > 0)
            {
                IDocumentElement lastChild = currentParagraph.Children[childrenCount - 1];
                TextElement textChild = lastChild as TextElement;
                if (textChild != null)
                {
                    textChild.AppendText(text);
                    return;
                }
            }

            AddElement(doc, ref currentParagraph, new TextElement(text));
        }

        private static void AddElement (IDocumentElementContainer doc, ref ParagraphElement currentParagraph, IDocumentElement textElement)
        {
            Contract.Ensures (currentParagraph != null);

            CreateParagraphIfNoneIsAlreadyOpen(doc, ref currentParagraph);

            currentParagraph.Children.Add(textElement);
        }

        private static void CreateParagraphIfNoneIsAlreadyOpen (IDocumentElementContainer doc, ref ParagraphElement currentParagraph)
        {
            Contract.Ensures(currentParagraph != null);

            if (currentParagraph == null)
            {
                currentParagraph = new ParagraphElement();
                doc.Children.Add(currentParagraph);
            }
        }

        private static readonly Regex anchorRegex = new Regex(@"^[\d\w\-\._~!\$\&`\(\)\*\+\,\;\=\:\@]+$", RegexOptions.Compiled);
        //private LineMode currentLineMode;

        //private enum LineMode
        //{
        //    Paragraph,
        //    Header
        //}
    }
}