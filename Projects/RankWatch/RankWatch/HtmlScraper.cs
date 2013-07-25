using System.Collections.Generic;
using System.IO;
using System.Net;

namespace RankWatch
{
    public class HtmlScraper
    {
        public void LoadFile(string fileName)
        {
            html = File.ReadAllText (@"..\..\..\data\sample-serp.html");
            index = 0;
        }

        public void Download(string url, string userAgent)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, userAgent);
                html = wc.DownloadString(url);
                index = 0;
            }
        }

        public bool FindNext(string text)
        {
            int pos = html.IndexOf(text, index, System.StringComparison.Ordinal);
            if (pos == -1)
                return false;

            index = pos + text.Length;
            return true;
        }

        public int FindNextOneOf(params string[] texts)
        {
            int? minPos = null;
            int? foundIndex = null;

            for (int i = 0; i < texts.Length; i++)
            {
                string text = texts[i];

                int pos = html.IndexOf(text, this.index, System.StringComparison.Ordinal);
                if (pos == -1)
                    continue;

                if (!minPos.HasValue || pos < minPos)
                {
                    minPos = pos;
                    foundIndex = i;
                }
            }

            if (!foundIndex.HasValue)
                return -1;

            string foundText = texts[foundIndex.Value];
            index = minPos.Value + foundText.Length;
            return foundIndex.Value;
        }

        public string ExtractUntil(string text)
        {
            int pos = html.IndexOf(text, index, System.StringComparison.Ordinal);

            string extract = html.Substring(index, pos - index);
            index = pos + text.Length;

            return extract;
        }

        public void SaveHtml (string fileName)
        {
            File.WriteAllText(fileName, html);
        }

        private string html;
        private int index;
    }
}