using CleanCode.Step1;
using NUnit.Framework;

namespace CleanCode.Step2
{
    public class ParserTests
    {
        [Test]
        public void Test1()
        {
            Parser parser = new Parser();
            WeatherData data = parser.Parse("SampleData/sample.html", false);

            WeatherService w = new WeatherService();
            int t = w.GetTemp(data, "Kredarica");
            Assert.AreEqual(-11, t);

            string s = w.GetPressure(data, "Murska Sobota");
            Assert.AreEqual("raste", s);
        }
    }
}