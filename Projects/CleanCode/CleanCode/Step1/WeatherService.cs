using System;
using System.Collections.Generic;

namespace CleanCode.Step1
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
    }
}