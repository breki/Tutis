using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CleanCode.Step4
{
    public class Parser
    {
        public WeatherData Parse(string source, bool isWeb)
        {
            FetchHtml(source, isWeb);
            ExtractWeatherFromHtml();
            return weatherData;
        }

        private void FetchHtml(string source, bool isWeb)
        {
            if (isWeb)
                FetchHtmlFromWeb();
            else
                FetchHtmlFromFile(source);
        }

        private void FetchHtmlFromWeb()
        {
            WebClient webClient = new WebClient();
            htmlData = webClient.DownloadString(WeatherPageUrl);
        }

        private void FetchHtmlFromFile(string source)
        {
            htmlData = File.ReadAllText(source);
        }

        private void ExtractWeatherFromHtml()
        {
            weatherData = new WeatherData();
            MoveToTableBody();
            ExtractWeatherFromTable();
        }

        private void MoveToTableBody()
        {
            htmlCursor = htmlData.IndexOf(TableMarker);
            htmlCursor = htmlData.IndexOf(TBodyMarker, htmlCursor);

            classMarkerLen = ClassMarker.Length;
            endTagMarkerLen = EndTagMarker.Length;
        }

        private void ExtractWeatherFromTable()
        {
            for (int rowsCounter = 0; rowsCounter < WeatherRowsCount; rowsCounter++)
                ExtractRow();
        }

        private void ExtractRow()
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            htmlCursor = htmlData.IndexOf(TableRowMarker, htmlCursor);

            while (ExtractNextField(rowValues))
            {
            }

            weatherData.Values.Add(rowValues);
        }

        private bool ExtractNextField(Dictionary<string, string> rowValues)
        {
            string fieldId = ExtractFieldId();
            string fieldValue = ExtractFieldValue();

            rowValues.Add(fieldId, fieldValue);

            if (fieldId == LastClassName)
                return false;

            return true;
        }

        private string ExtractFieldId()
        {
            htmlCursor = htmlData.IndexOf(ClassMarker, htmlCursor);
            int classEnd = htmlData.IndexOf(EndTagMarker, htmlCursor);
            string className = htmlData.Substring(
                htmlCursor + classMarkerLen, classEnd - htmlCursor - classMarkerLen);
            htmlCursor = classEnd;

            return className;
        }

        private string ExtractFieldValue()
        {
            int valueEnd = htmlData.IndexOf(StartTagMarker, htmlCursor);
            string classValue = htmlData.Substring(
                htmlCursor + endTagMarkerLen,
                valueEnd - htmlCursor - endTagMarkerLen);
            htmlCursor = valueEnd;
            return classValue;
        }

        private string htmlData;
        private WeatherData weatherData;
        private int classMarkerLen;
        private int endTagMarkerLen;
        private int htmlCursor;
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