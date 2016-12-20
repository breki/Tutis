using LibroLib.WebUtils.Rest;
using NUnit.Framework;

namespace SrtmPlaying.Tests
{
    public class SrtmServerTests
    {
        [Test, Explicit]
        public void DownloadDataFromNasaServer()
        {
            using (IRestClient client = new RestClient())
            {
                client.Get(
                    "http://e4ftl01.cr.usgs.gov/SRTM/SRTMGL1.003/2000.02.11/N00E006.SRTMGL1.hgt.zip");
                IRestClientResponse response = client
                    .AddHeader("Cookie", "DATA=V8x3l5g9BGcAACrbsuAAAAAB")
                    .Do()
                    .Response;
                byte[] data = response.AsBytes();
            }
        }
    }
}