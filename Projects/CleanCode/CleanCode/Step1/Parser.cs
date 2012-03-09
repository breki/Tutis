using System.Collections.Generic;
using System.Net;

namespace CleanCode.Step1
{
    public class Parser
    {
        public WeatherData Parse()
        {
            WebClient webClient = new WebClient();
            string s = webClient.DownloadString("http://meteo.arso.gov.si/uploads/probase/www/observ/surface/text/sl/observation_si_latest.html");
            int i = s.IndexOf("<table class=\"meteoSI-table\"");
            i = s.IndexOf("<tbody", i);
            WeatherData data = new WeatherData();
            int c = 0;
            while(true)
            {
                Dictionary<string, string> v = new Dictionary<string, string>();
                i = s.IndexOf("<tr>", i);
                while(true)
                {
                    i = s.IndexOf("<td class=\"", i);
                    int j = s.IndexOf("\">", i);
                    string a = s.Substring(i + 11, j - i - 11);
                    int k = s.IndexOf("<", j);
                    string b = s.Substring(j + 2, k - j - 2);
                    v.Add(a, b);
                    i = k;
                    if (a == "rr24h_val")
                        break;
                }
                data.Values.Add(v);
                c++;
                if (c == 10)
                    break;
            }
            return data;
        }
    }
}