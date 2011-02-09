using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Test
{
    [TestFixture]
    public class PriceTests
    {
        [MbUnit.Framework.Test]
        public void CompareToItself ()
        {
            Commodity euroCommodity = book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "EUR"));

            IList<Price> prices = book.ListPricesForCommodity (Commodity.ConstructFullId ("ISO4217", "USD"), euroCommodity.FullId);
            Assert.AreEqual (0, prices[0].CompareTo (prices[0]));
        }

        [SetUp]
        public void Setup ()
        {
            book = new XmlBookReader(@"..\..\..\Data\Igor.xml").Read();
        }

        private Book book;
    }
}
