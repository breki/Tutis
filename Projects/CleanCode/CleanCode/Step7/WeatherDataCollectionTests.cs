using NUnit.Framework;

namespace CleanCode.Step7
{
    public class WeatherDataCollectionTests
    {
        [Test]
        public void AssertDataRecordReadingIsWorking()
        {
            for (int row = 0; row < 5; row++)
            {
                reader.MoveToNextRow();
                for (int field = 0; ; field++)
                {
                    string fieldId = reader.ExtractNextFieldId();
                    string fieldValue = reader.ExtractNextFieldValue();

                    if (fieldId == "rr24h_val")
                        break;

                    if (row == 4 && fieldId == "wwsyn_longText")
                        Assert.AreEqual("pretežno jasno", fieldValue);                        
                }
            }

        }

        [SetUp]
        public void Setup()
        {           
            FileTextFetcher textFetcher = new FileTextFetcher();
            string text = textFetcher.FetchText("SampleData/sample.html");

            reader = new ArsoHtmlWeatherDataReader();
            reader.SetText(text);
        }

        private ArsoHtmlWeatherDataReader reader;
    }
}