using System.Collections.Generic;

namespace CleanCode.Step9
{
    public class WeatherDataCollector
    {
        public WeatherDataCollector(ITextFetcher textFetcher, IWeatherDataReader reader)
        {
            this.textFetcher = textFetcher;
            this.reader = reader;
        }

        public WeatherData Parse(string source)
        {
            FetchText(source);
            ExtractWeatherFromText();
            return weatherData;
        }

        private void FetchText(string source)
        {
            string text = textFetcher.FetchText(source);
            reader.SetText(text);
        }

        private void ExtractWeatherFromText()
        {
            weatherData = new WeatherData();
            ExtractWeatherFromTable();
        }

        private void ExtractWeatherFromTable()
        {
            while (reader.MoveToNextRow())
                ExtractRow();
        }

        private void ExtractRow()
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            reader.MoveToNextRow();

            while (ExtractNextField(rowValues))
            {
            }

            weatherData.AddStationData(rowValues);
        }

        private bool ExtractNextField(Dictionary<string, string> rowValues)
        {
            string fieldId = reader.ExtractNextFieldId();

            if (fieldId == null)
                return false;

            string fieldValue = reader.ExtractNextFieldValue();

            rowValues.Add(fieldId, fieldValue);

            return true;
        }

        private readonly ITextFetcher textFetcher;
        private readonly IWeatherDataReader reader;
        private WeatherData weatherData;
    }
}