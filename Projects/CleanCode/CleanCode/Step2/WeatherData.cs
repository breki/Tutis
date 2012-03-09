using System.Collections.Generic;

namespace CleanCode.Step2
{
    public class WeatherData
    {
        public List<Dictionary<string, string>> Values
        {
            get { return values; }
            set { values = value; }
        }

        private List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
    }
}