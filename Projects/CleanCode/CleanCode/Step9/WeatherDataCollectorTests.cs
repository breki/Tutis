using NUnit.Framework;

namespace CleanCode.Step9
{
    public class WeatherDataCollectorTests
    {
        [Test]
        public void AssertTemperatureIsRight()
        {
            int temperature = weatherData.GetTemp("Kredarica");
            Assert.AreEqual(-11, temperature);
        }

        [Test]
        public void AssertPressureTrendIsRight()
        {
            string pressureTrend = weatherData.GetPressure("Murska Sobota");
            Assert.AreEqual("raste", pressureTrend);
        }

        [Test]
        public void MakeSureAllStationsHaveBeenRead()
        {
            Assert.AreEqual(11, weatherData.StationsCount);
        }

        [SetUp]
        public void Setup()
        {
            FileTextFetcher textFetcher = new FileTextFetcher();
            IWeatherDataReader weatherDataReader = new ArsoHtmlWeatherDataReader();
            WeatherDataCollector weatherDataCollector = new WeatherDataCollector(textFetcher, weatherDataReader);
            weatherData = weatherDataCollector.Parse("SampleData/sample.html");
        }

        private WeatherData weatherData;
    }
}