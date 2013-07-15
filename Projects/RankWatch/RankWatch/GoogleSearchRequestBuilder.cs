using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace RankWatch
{
    public class GoogleSearchRequestBuilder
    {
        public string ConstructUrl(string searchTerm, int pageNumber)
        {
            const string UrlFormat 
                = @"http://www.google.si/search?q={0}&safe=off&ei=rt_jUfWEHMnNON6WgZgP&sqi=2&{1}sa=N&biw=1600&bih=1083";

            string searchTermEscaped = searchTerm.Replace(' ', '+');
            string url = string.Format(
                CultureInfo.InvariantCulture,
                UrlFormat,
                searchTermEscaped,
                pageNumber > 0 ? string.Format(CultureInfo.InvariantCulture, "start={0}&", pageNumber*10) : string.Empty);

            return url;
        }

        public string GetUserAgent()
        {
            return userAgents[rnd.Next(userAgents.Length)];
        }

        public void WaitForNextPageQuery ()
        {
            int waitSeconds = rnd.Next(25, 40);
            Debug.WriteLine("Waiting {0} secs...", waitSeconds);
            Thread.Sleep(waitSeconds * 1000);
        }

        private string[] userAgents = new[]
            {
                @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)",
                @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36"
            };

        private Random rnd = new Random();
    }
}