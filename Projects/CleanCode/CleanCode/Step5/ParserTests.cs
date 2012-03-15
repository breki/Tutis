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
            FileTextFetcher textFetcher = new FileTextFetcher();
            Parser parser = new Parser(textFetcher);
            weatherData = parser.Parse("SampleData/sample.html");
            weatherService = new WeatherService();
        }

        private WeatherData weatherData;
        private WeatherService weatherService;
    }
}