using System;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class FreudeTextParser
    {
        public DocumentDef ParseText(string text)
        {
            DocumentDef doc = new DocumentDef();

            int cursor = 0;
            while (true)
            {
                int i = text.IndexOf("[[", cursor, StringComparison.Ordinal);

                if (i != -1)
                {
                    string part = text.Substring(cursor, i - cursor).Trim();
                    if (part.Length > 0)
                    {
                        TextElement textElement = new TextElement(part);
                        doc.Children.Add(textElement);
                    }

                    int j = text.IndexOf("]]", i, StringComparison.Ordinal);

                    string url = text.Substring(i + 2, j - (i + 2));
                    ImageElement imageElement = new ImageElement(url);
                    doc.Children.Add(imageElement);

                    cursor = j + 2;
                }
                else
                {
                    string part = text.Substring (cursor).Trim ();
                    if (part.Length > 0)
                    {
                        TextElement textElement = new TextElement (part);
                        doc.Children.Add (textElement);
                    }

                    break;
                }
            }

            return doc;
        }
    }
}