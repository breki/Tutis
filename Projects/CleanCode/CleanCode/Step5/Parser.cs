using System.Collections.Generic;

namespace CleanCode.Step5
{
    public class Parser
    {
        public Parser(IHtmlFetcher htmlFetcher)
        {
            this.htmlFetcher = htmlFetcher;
        }

        public WeatherData Parse(string source)
        {
            FetchHtml(source);
            ExtractWeatherFromHtml();
            return weatherData;
        }

        private void FetchHtml(string source)
        {
            htmlData = htmlFetcher.FetchHtml(source);
        }

        private void ExtractWeatherFromHtml()
        {
            weatherData = new WeatherData();
            MoveToTableBody();
            ExtractWeatherFromTable();
        }

        private void MoveToTableBody()
        {
            MoveToNext(TableMarker);
            MoveToNext(TBodyMarker);

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
            MoveToNext(TableRowMarker);

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
            MoveToNext(ClassMarker);
            int classEnd = FindNext(EndTagMarker);
            string className = htmlData.Substring(
                htmlCursor + classMarkerLen, classEnd - htmlCursor - classMarkerLen);
            htmlCursor = classEnd;

            return className;
        }

        private string ExtractFieldValue()
        {
            int valueEnd = FindNext(StartTagMarker);
            string classValue = htmlData.Substring(
                htmlCursor + endTagMarkerLen,
                valueEnd - htmlCursor - endTagMarkerLen);
            htmlCursor = valueEnd;
            return classValue;
        }

        private int FindNext(string text)
        {
            return htmlData.IndexOf(text, htmlCursor);
        }

        private void MoveToNext(string text)
        {
            htmlCursor = htmlData.IndexOf(text, htmlCursor);
        }

        private readonly IHtmlFetcher htmlFetcher;
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
    }
}