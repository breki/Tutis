using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using GnuCashUtils.Framework.DataMiners;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Test
{
    [TestFixture]
    public class PriceMinersTests
    {
        [MbUnit.Framework.Test,Ignore]
        public void VzajemciTest ()
        {
            PriceDataMiningSettings settings = new PriceDataMiningSettings ();
            settings.CurrencyUsed = euroCommodity;
            settings.AddCommodityTranslation (@"Alfa - uravnote.eni", "FUND.PRA");

            List<Price> foundPrices = new List<Price> ();
                
            VzajemciDataMiner miner = new VzajemciDataMiner ();
            miner.MineForPrices (book, settings, foundPrices);

            Assert.AreEqual (1, foundPrices.Count);
        }

        [MbUnit.Framework.Test]
        public void SkladiTest ()
        {
            PriceDataMiningSettings settings = new PriceDataMiningSettings ();
            settings.CurrencyUsed = euroCommodity;
            settings.AddCommodityTranslation (@"Alfa - uravnote.eni", "FUND.PRA");

            List<Price> foundPrices = new List<Price> ();

            SkladiDataMiner miner = new SkladiDataMiner ();
            miner.MineForPrices (book, settings, foundPrices);

            Assert.AreEqual (3, foundPrices.Count);
        }

        [MbUnit.Framework.Test]
        public void BankaSlovenijeTest ()
        {
            PriceDataMiningSettings settings = new PriceDataMiningSettings ();
            settings.CurrencyUsed = euroCommodity;

            List<Price> foundPrices = new List<Price> ();

            BankaSlovenijeDataMiner miner = new BankaSlovenijeDataMiner ();
            miner.MineForPrices (book, settings, foundPrices);

            Assert.AreEqual (2, foundPrices.Count);
        }

        [SetUp]
        public void Setup ()
        {
            book = new XmlBookReader(@"..\..\..\Data\Igor.xml").Read();
            euroCommodity = book.GetCommodity(Commodity.ConstructFullId("ISO4217", "EUR"));
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup ()
        {
            //log4net.Config.XmlConfigurator.Configure ();
        }

        private Book book;
        private Commodity euroCommodity;
    }
}
