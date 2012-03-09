using NUnit.Framework;

namespace CleanCode.Step4
{
    public class ParserTests
    {
        [Test]
        public void AssertTemperatureIsRight()
        {
            int temperature = weatherService.GetTemp(weatherData, "Kredarica");
            Assert.AreEqual(-11, temperature);
        }

        [Test]
        public void AssertPressureTrendIsRight()
        {
            string pressureTrend = weatherService.GetPressure(weatherData, "Murska Sobota");
            Assert.AreEqual("raste", pressureTrend);
        }

        [Test]
        public void MakeSureAllRowsHaveBeenRead()
        {
            Assert.AreEqual(10, weatherData.RowsCount);
        }

        [SetUp]
        public void Setup()
        {
            Parser parser = new Parser();
            weatherData = parser.Parse("SampleData/sample.html", false);
            weatherService = new WeatherService();
        }

        private WeatherData weatherData;
        private WeatherService weatherService;
    }
}