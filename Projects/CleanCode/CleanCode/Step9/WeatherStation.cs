using System.Collections.Generic;

namespace CleanCode.Step9
{
    public class WeatherStation
    {
        public WeatherStation(string stationId)
        {
            this.stationId = stationId;
        }

        public string StationId
        {
            get { return stationId; }
        }

        public T GetData<T>(WeatherDataType dataType)
        {
            return (T)weatherData[dataType];
        }

        public void SetData(WeatherDataType dataType, object value)
        {
            weatherData[dataType] = value;
        }

        private string stationId;
        private Dictionary<WeatherDataType, object> weatherData = new Dictionary<WeatherDataType, object>();
    }
}