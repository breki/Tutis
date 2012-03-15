using NUnit.Framework;

namespace CleanCode.Step3
{
    public class ParserTests
    {
        [Test]
        public void AssertTemperatureIsRight()
        {
            Parser parser = new Parser();
            WeatherData data = parser.Parse("SampleData/sample.html", false);

            WeatherService w = new WeatherService();
            int t = w.GetTemp(data, "Kredarica");
            Assert.AreEqual(-11, t);
        }

        [Test]
        public void AssertPressureTrendIsRight()
        {
            Parser parser = new Parser();
            WeatherData data = parser.Parse("SampleData/sample.html", false);

            WeatherService w = new WeatherService();
            string s = w.GetPressure(data, "Murska Sobota");
            Assert.AreEqual("raste", s);
        }

        [Test]
        public void MakeSureAllRowsHaveBeenRead()
        {
            Parser parser = new Parser();
            WeatherData data = parser.Parse("SampleData/sample.html", false);
            Assert.AreEqual(10, data.RowsCount);
        }
    }
}