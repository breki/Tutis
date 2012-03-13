using System;
using System.Collections.Generic;

namespace CleanCode.Step9
{
    public class WeatherData
    {
        public int StationsCount { get { return stations.Count; } }

        public void AddStationData(Dictionary<string, string> stationValues)
        {
            stations.Add(stationValues);
        }

        public int GetTemp(string stationName)
        {
            foreach (Dictionary<string, string> d in stations)
            {
                if (d["meteoSI-th"] == stationName)
                    return int.Parse(d["t"]);
            }

            return -12345;
        }

        public string GetPressure(string stationName)
        {
            foreach (Dictionary<string, string> d in stations)
            {
                if (d["meteoSI-th"] == stationName)
                    return d["pa_code"];
            }

            return null;
        }

        private List<Dictionary<string, string>> stations = new List<Dictionary<string, string>>();
    }
}