using System.Collections.Generic;

namespace CleanCode.Step9
{
    public class WeatherData
    {
        public int StationsCount { get { return stations.Count; } }

        public void AddStation(WeatherStation station)
        {
            stations.Add(station.StationId, station);
        }

        public WeatherStation GetStation(string stationId)
        {
            return stations[stationId];
        }

        public bool HasStation(string stationId)
        {
            return stations.ContainsKey(stationId);
        }

        private Dictionary<string, WeatherStation> stations = new Dictionary<string, WeatherStation>();
    }
}