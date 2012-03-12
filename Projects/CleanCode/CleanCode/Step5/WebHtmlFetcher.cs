using System.Net;

namespace CleanCode.Step5
{
    public class WebHtmlFetcher : IHtmlFetcher
    {
        public string FetchHtml(string source)
        {
            using (WebClient webClient = new WebClient())
                return webClient.DownloadString(source);
        }
    }
}