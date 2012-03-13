using NUnit.Framework;

namespace CleanCode.Step9
{
    public class ArsoHtmlReadingTests
    {
        [Test]
        public void AssertDataRecordReadingIsWorking()
        {
            for (int row = 0; row < 5; row++)
            {
                Assert.IsTrue(reader.MoveToNextRow());

                for (int field = 0; ; field++)
                {
                    string fieldId = reader.ExtractNextFieldId();

                    if (fieldId == null)
                        break;

                    string fieldValue = reader.ExtractNextFieldValue();

                    if (row == 4 && fieldId == "wwsyn_longText")
                        Assert.AreEqual("pretežno jasno", fieldValue);                        
                }
            }
        }

        [Test]
        public void AssertThereAre11Records()
        {
            int rows = 0;
            while (reader.MoveToNextRow())
            {
                rows++;
                while (reader.ExtractNextFieldId() != null)
                    reader.ExtractNextFieldValue();
            }

            Assert.AreEqual(11, rows);
        }

        [SetUp]
        public void Setup()
        {           
            FileTextFetcher textFetcher = new FileTextFetcher();
            string text = textFetcher.FetchText("SampleData/sample.html");

            reader = new ArsoHtmlTableDataReader();
            reader.SetText(text);
        }

        private ArsoHtmlTableDataReader reader;
    }
}