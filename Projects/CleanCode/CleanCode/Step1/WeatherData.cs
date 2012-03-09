using System.Collections.Generic;

namespace CleanCode.Step1
{
    public class WeatherData
    {
        public List<Dictionary<string, string>> Values
        {
            get { return values; }
            set { values = value; }
        }

        public int RowsCount
        {
            get { return values.Count; }
        }

        private List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
    }
}