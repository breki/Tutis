using NUnit.Framework;

namespace CleanCode.Step6
{
    public class ArsoHtmlDataCollectionTests
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
            IWeatherDataReader weatherDataReader = new ArsoHtmlWeatherDataReader();
            WeatherDataCollector weatherDataCollector = new WeatherDataCollector(textFetcher, weatherDataReader);
            weatherData = weatherDataCollector.Parse("SampleData/sample.html");
            weatherService = new WeatherService();
        }

        private WeatherData weatherData;
        private WeatherService weatherService;
    }
}