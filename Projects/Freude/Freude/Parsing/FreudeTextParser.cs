﻿using System;
using System.Collections.Generic;
using Brejc.Common;
using Freude.DocModel;

namespace Freude.Parsing
{
    public class FreudeTextParser : IFreudeTextParser
    {
        public DocumentDef ParseText(string text)
        {
            doc = new DocumentDef();

            IList<string> lines = text.SplitIntoLines();

            for (int line = 0; line < lines.Count; line++)
                ParseLine(lines, line);

            return doc;
        }

        private void ParseLine(IList<string> lines, int line)
        {
            string lineText = lines[line];

            // we can ignore lines with nothing but whitespace
            if (lineText.Trim().Length == 0)
            {
                currentParagraph = null;
                return;
            }

            int cursor = 0;

            char startingChar = lineText[cursor];
            if (startingChar == '#')
            {
                currentParagraph = null;

                while (lineText[cursor] == '#')
                    cursor++;

                string headerText = lineText.Substring(cursor).Trim();

                HeaderElement headerElement = new HeaderElement(headerText, cursor);
                doc.Children.Add(headerElement);
                return;
            }

            while (true)
            {
                int i = lineText.IndexOf("[[", cursor, StringComparison.Ordinal);

                if (i != -1)
                {
                    string part = lineText.Substring(cursor, i - cursor).Trim();
                    if (part.Length > 0)
                        AddTextToParagraph(part);

                    int j = lineText.IndexOf("]]", i, StringComparison.Ordinal);

                    Uri url = new Uri(lineText.Substring(i + 2, j - (i + 2)));
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
        //private LineMode currentLineMode;

        //private enum LineMode
        //{
        //    Paragraph,
        //    Header
        //}
    }
}