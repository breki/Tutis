using System.Collections.Generic;

namespace CleanCode.Step7
{
    public class WeatherService
    {
        public int GetTemp(WeatherData data, string p)
        {
            foreach (Dictionary<string, string> d in data.Values)
            {
                if (d["meteoSI-th"] == p)
                    return int.Parse(d["t"]);
            }

            return -12345;
        }

        public string GetPressure(WeatherData data, string p)
        {
            foreach (Dictionary<string, string> d in data.Values)
            {
                if (d["meteoSI-th"] == p)
                    return d["pa_code"];
            }

            return null;
        }
    }
}