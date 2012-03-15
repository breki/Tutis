using System.Collections.Generic;

namespace CleanCode.Step6
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
            
            reader.MoveToTableStart();

            ExtractWeatherFromTable();
        }

        private void ExtractWeatherFromTable()
        {
            for (int rowsCounter = 0; rowsCounter < WeatherRowsCount; rowsCounter++)
                ExtractRow();
        }

        private void ExtractRow()
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            reader.MoveToNextRow();

            while (ExtractNextField(rowValues))
            {
            }

            weatherData.Values.Add(rowValues);
        }

        private bool ExtractNextField(Dictionary<string, string> rowValues)
        {
            string fieldId = reader.ExtractNextFieldId();
            string fieldValue = reader.ExtractNextFieldValue();

            rowValues.Add(fieldId, fieldValue);

            if (fieldId == LastClassName)
                return false;

            return true;
        }

        private readonly ITextFetcher textFetcher;
        private readonly IWeatherDataReader reader;
        private WeatherData weatherData;
        private const string LastClassName = "rr24h_val";
        private const int WeatherRowsCount = 10;
    }
}