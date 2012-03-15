using System;
using System.Collections.Generic;
using System.Globalization;

namespace CleanCode.Step9
{
    public class ArsoWeatherDataSource : IWeatherDataSource
    {
        public ArsoWeatherDataSource(
            string sourceLocation,
            ITextFetcher textFetcher, 
            ITableDataReader tableDataReader)
        {
            this.sourceLocation = sourceLocation;
            this.textFetcher = textFetcher;
            this.tableDataReader = tableDataReader;
        }

        public WeatherData FetchWeatherData()
        {
            FetchText(sourceLocation);
            ExtractWeatherFromText();
            return weatherData;
        }

        private void FetchText(string sourceLocation)
        {
            string text = textFetcher.FetchText(sourceLocation);
            tableDataReader.SetText(text);
        }

        private void ExtractWeatherFromText()
        {
            weatherData = new WeatherData();
            ExtractWeatherFromTable();
        }

        private void ExtractWeatherFromTable()
        {
            while (tableDataReader.MoveToNextRow())
                ExtractStationData();
        }

        private void ExtractStationData()
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            tableDataReader.MoveToNextRow();

            while (ExtractNextField(rowValues))
            {
            }

            WeatherStation station = ConstructStationData(rowValues);
            weatherData.AddStation(station);
        }

        private bool ExtractNextField(IDictionary<string, string> rowValues)
        {
            string fieldId = tableDataReader.ExtractNextFieldId();

            if (fieldId == null)
                return false;

            string fieldValue = tableDataReader.ExtractNextFieldValue();

            rowValues.Add(fieldId, fieldValue);

            return true;
        }

        private WeatherStation ConstructStationData(IDictionary<string, string> rowValues)
        {
            if (rowValues.ContainsKey(ClassStationId))
                station = new WeatherStation(rowValues[ClassStationId]);
            else
                throw new InvalidOperationException("Invalid input data.");

            foreach (KeyValuePair<string, string> pair in rowValues)
                ExtractWeatherValue(pair);

            return station;
        }

        private void ExtractWeatherValue(KeyValuePair<string, string> pair)
        {
            switch (pair.Key)
            {
                case "t":
                    SetIntDataIfAvailable(WeatherDataType.Temperature, pair.Value);
                    break;
                case "ff_val":
                    SetIntDataIfAvailable(WeatherDataType.WindSpeed, pair.Value);
                    break;
                case "pa_code":
                    station.SetData(WeatherDataType.PressureTrend, pair.Value);
                    break;
            }
        }

        private void SetIntDataIfAvailable(WeatherDataType dataType, string value)
        {
            if (value == "&nbsp;")
                return;

            int intValue = int.Parse(value, CultureInfo.InvariantCulture);
            station.SetData(dataType, intValue);
        }

        private readonly string sourceLocation;
        private readonly ITextFetcher textFetcher;
        private readonly ITableDataReader tableDataReader;
        private WeatherData weatherData;
        private WeatherStation station;
        private const string ClassStationId = "meteoSI-th";
    }
}