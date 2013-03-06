using System;
using System.Globalization;
using NUnit.Framework;

namespace Elf
{
    public class ElfTests
    {
        [Test]
        public void ReadSampleLog()
        {
            IElfReader reader = new ElfReader();
            LogContents content = reader.ReadLogFile(@"..\..\..\samples\sample.txt");

            Assert.AreEqual(1, content.Entries.Count);
            Assert.AreEqual (
                DateTime.Parse ("2002-05-24 20:18:01", CultureInfo.InvariantCulture), 
                content.Entries[0].DateTime);
            Assert.IsNull(content.Entries[0].UriQuery);
        }
    }
}
