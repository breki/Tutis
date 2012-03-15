using System.Net;

namespace CleanCode.Step5
{
    public class WebTextFetcher : ITextFetcher
    {
        public string FetchText(string source)
        {
            using (WebClient webClient = new WebClient())
                return webClient.DownloadString(source);
        }
    }
}