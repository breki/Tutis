using System;
using System.Collections.Generic;
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
            doc = new DocumentDef();

            IList<string> lines = text.SplitIntoLines();

            for (int line = 0; line < lines.Count; line++)
            {
                ParseLine(context, lines);
                context.IncrementLineCounter();
            }

            return doc;
        }

        private void ParseLine(ParsingContext context, IList<string> lines)
        {
            Contract.Assume (lines[context.Line - 1] != null);

            string lineText = lines[context.Line - 1];
            
            // we can ignore lines with nothing but whitespace
            if (lineText.Trim().Length == 0)
            {
                currentParagraph = null;
                return;
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

                    HandleHeaderWithHashChar(lineText, cursor);
                    return;
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

                    HandleHeaderWithEqualsChar(context, lineText, cursor);
                    return;
                }
            }

            while (true)
            {
                int i = lineText.IndexOf("[[", cursor, StringComparison.Ordinal);

                if (i >= 0)
                {
                    string part = lineText.Substring(cursor, i - cursor).Trim();
                    if (part.Length > 0)
                        AddTextToParagraph(part);

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
                    AddElement(imageElement);

                    cursor = j + 2;
                }
                else
                {
                    string part = lineText.Substring(cursor).Trim();
                    if (part.Length > 0)
                        AddTextToParagraph(part);

                    break;
                }
            }
        }

        private void HandleHeaderWithHashChar(string lineText, int headerLevel)
        {
            string headerText = lineText.Substring(headerLevel).Trim();

            HeaderElement headerElement = new HeaderElement(headerText, headerLevel);
            doc.Children.Add(headerElement);
        }

        private void HandleHeaderWithEqualsChar(ParsingContext context, string lineText, int headerLevel)
        {
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

        private void AddTextToParagraph(string text)
        {
            CreateParagraphIfNoneIsAlreadyOpen();

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

            AddElement(new TextElement(text));
        }

        private void AddElement(IDocumentElement textElement)
        {
            CreateParagraphIfNoneIsAlreadyOpen();

            currentParagraph.Children.Add(textElement);
        }

        private void CreateParagraphIfNoneIsAlreadyOpen()
        {
            if (currentParagraph == null)
            {
                currentParagraph = new ParagraphElement();
                doc.Children.Add(currentParagraph);
            }
        }

        private DocumentDef doc;
        private ParagraphElement currentParagraph;
        private static Regex anchorRegex = new Regex(@"^[\d\w\-\._~!\$\&`\(\)\*\+\,\;\=\:\@]+$", RegexOptions.Compiled);
        //private LineMode currentLineMode;

        //private enum LineMode
        //{
        //    Paragraph,
        //    Header
        //}
    }
}