using System;
using System.Collections.Generic;
using MbUnit.Framework;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Test
{
    [TestFixture]
    public class GnuCashFileTests
    {
        [MbUnit.Framework.Test]
        public void Load ()
        {
            Book book = new XmlBookReader(@"..\..\..\Data\Igor.xml").Read();

            Commodity kdBalkanCommodity = book.GetCommodity (Commodity.ConstructFullId ("FUND", "KDBAL"));
            Assert.IsNotNull (kdBalkanCommodity);
            Commodity euroCommodity = book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "EUR"));
            Assert.IsNotNull (euroCommodity);

            IList<Price> prices = book.ListPricesForCommodity (kdBalkanCommodity.FullId, euroCommodity.FullId);
            Assert.AreEqual (126, prices.Count);

            Assert.IsNotNull (book.RootAccount);

            Assert.AreEqual (1220, book.TransactionsCount);

            Price price = new Price ();
            price.Commodity = kdBalkanCommodity;
            price.Currency = euroCommodity;
            price.Source = "unit test";
            price.Type = "unknown";
            price.Value = new DecimalValue (30, 100);
            price.Time = new DateTime (2006, 2, 4);
            book.AddPrice (price);

            throw new NotImplementedException();
            //file.Save ("GnuCashData.xml");
        }
    }
}
