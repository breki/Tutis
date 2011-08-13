using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Test
{
    [TestFixture]
    public class CommodityTests
    {
        [MbUnit.Framework.Test]
        public void FindCommodityById ()
        {
            Assert.IsNotNull (book.FindCommodityById (Commodity.ConstructFullId ("ISO4217", "USD")));
        }

        [MbUnit.Framework.Test]
        public void FindCommodityByIdNotExist ()
        {
            Assert.IsNull (book.FindCommodityById (Commodity.ConstructFullId ("ISO4217", "Not exists")));
        }

        [MbUnit.Framework.Test]
        public void CompareToItself ()
        {
            Commodity commodity = book.FindCommodityById (Commodity.ConstructFullId ("ISO4217", "USD"));
            Assert.AreEqual (0, commodity.CompareTo (commodity));
        }

        [SetUp]
        public void Setup ()
        {
            book = new XmlBookReader(@"..\..\..\Data\Igor.xml").Read();
        }

        private Book book;
    }
}
