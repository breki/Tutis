using NUnit.Framework;

namespace CleanCode.Step1
{
    public class ParserTests
    {
        [Test]
        public void Test1()
        {
            Parser parser = new Parser();
            WeatherData data = parser.Parse();

            WeatherService w = new WeatherService();
            int t = w.GetTemp(data, "Kredarica");
            Assert.AreEqual(-11, t);

            string s = w.GetPressure(data, "Murska Sobota");
            Assert.AreEqual("raste", s);
        }

        [Test]
        public void Test2()
        {
            Parser parser = new Parser();
            WeatherData data = parser.Parse();

            WeatherService w = new WeatherService();
            int t = w.GetTemp(data, "Kredarica");
            Assert.AreEqual(-11, t);
        }
    }
}