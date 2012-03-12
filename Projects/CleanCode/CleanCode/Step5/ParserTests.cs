using NUnit.Framework;

namespace CleanCode.Step5
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
            FileHtmlFetcher htmlFetcher = new FileHtmlFetcher();
            Parser parser = new Parser(htmlFetcher);
            weatherData = parser.Parse("SampleData/sample.html");
            weatherService = new WeatherService();
        }

        private WeatherData weatherData;
        private WeatherService weatherService;
    }
}