using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CleanCode.Step2
{
    public class Parser
    {
        public WeatherData Parse(string source, bool isWeb)
        {
            string htmlData;

            if (isWeb)
            {
                WebClient webClient = new WebClient();
                htmlData = webClient.DownloadString(WeatherPageUrl);
            }
            else
                htmlData = File.ReadAllText(source);

            int cursor = htmlData.IndexOf(TableMarker);
            cursor = htmlData.IndexOf(TBodyMarker, cursor);

            WeatherData weatherData = new WeatherData();

            int rowsCounter = 0;
            int classMarkerLen = ClassMarker.Length;
            int endTagMarkerLen = EndTagMarker.Length;

            while(true)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                cursor = htmlData.IndexOf(TableRowMarker, cursor);

                while(true)
                {
                    cursor = htmlData.IndexOf(ClassMarker, cursor);
                    int classEnd = htmlData.IndexOf(EndTagMarker, cursor);
                    string className = htmlData.Substring(
                        cursor + classMarkerLen, classEnd - cursor - classMarkerLen);

                    int valueEnd = htmlData.IndexOf(StartTagMarker, classEnd);
                    string classValue = htmlData.Substring(
                        classEnd + endTagMarkerLen, 
                        valueEnd - classEnd - endTagMarkerLen);
                    values.Add(className, classValue);
                    cursor = valueEnd;

                    if (className == LastClassName)
                        break;
                }

                weatherData.Values.Add(values);
                rowsCounter++;
                if (rowsCounter == WeatherRowsCount)
                    break;
            }

            return weatherData;
        }

        private const string TableMarker = "<table class=\"meteoSI-table\"";
        private const string TBodyMarker = "<tbody";
        private const string TableRowMarker = "<tr>";
        private const string ClassMarker = "<td class=\"";
        private const string EndTagMarker = "\">";
        private const string StartTagMarker = "<";
        private const string LastClassName = "rr24h_val";
        private const int WeatherRowsCount = 10;
        private const string WeatherPageUrl = "http://meteo.arso.gov.si/uploads/probase/www/observ/surface/text/sl/observation_si_latest.html";
    }
}