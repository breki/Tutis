using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnuCashUtils.Framework;
using MbUnit.Framework;

namespace GnuCashUtils.Test.FrameworkTests
{
    [TestFixture]
    public class XmlReaderTests
    {
        [Test]
        public void ParseXmlBook()
        {
            IBookReader xmlReader = new XmlBookReader("../../../Data/Igor.xml");

            Book book = xmlReader.Read();
        }
    }
}
