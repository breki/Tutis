using NUnit.Framework;

namespace CleanCode.Step9
{
    public class ArsoSourceTests
    {
        [Test]
        public void AssertTemperatureIsRight()
        {
            WeatherStation station = weatherData.GetStation("Kredarica");
            int temperature = station.GetData<int>(WeatherDataType.Temperature);
            Assert.AreEqual(-11, temperature);
        }

        [Test]
        public void AssertPressureTrendIsRight()
        {
            WeatherStation station = weatherData.GetStation("Murska Sobota");
            string pressureTrend = station.GetData<string>(WeatherDataType.PressureTrend);
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
            ITableDataReader tableDataReader = new ArsoHtmlTableDataReader();
            IWeatherDataSource source = new ArsoWeatherDataSource(
                "SampleData/sample.html",
                textFetcher, 
                tableDataReader);
            weatherData = source.FetchWeatherData();
        }

        private WeatherData weatherData;
    }
}