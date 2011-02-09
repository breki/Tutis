using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using GnuCashUtils.Framework;
using GnuCashUtils.Framework.DataMiners;

namespace GnuCashUtils.Console
{
    public class SloPricesCommand : ICommand
    {
        #region ICommand Members

        public void Execute (string gnuCashFileName)
        {
            log.Info ("Execute started");

            Book book = new XmlBookReader(gnuCashFileName).Read();
            Commodity euroCommodity = book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "EUR"));

            PriceDataMiningSettings settings = new PriceDataMiningSettings ();
            settings.CurrencyUsed = euroCommodity;
            settings.AddCommodityTranslation (@"Alfa - uravnote.eni", "FUND.PRA");

            List<Price> foundPrices = new List<Price> ();

            IPriceDataMiner[] priceMiners = { new VzajemciDataMiner (), new SkladiDataMiner (), new BankaSlovenijeDataMiner() };

            foreach (IPriceDataMiner priceMiner in priceMiners)
                priceMiner.MineForPrices (book, settings, foundPrices);

            bool isBookDirty = false;

            foreach (Price foundPrice in foundPrices)
            {
                if (null == book.FindPriceForCommodityAndDate (foundPrice.Commodity.FullId, foundPrice.Time))
                {
                    if (log.IsInfoEnabled)
                        log.InfoFormat ("Found and added price '{0}'", foundPrice);

                    book.AddPrice (foundPrice);
                    isBookDirty = true;
                }
            }

            if (isBookDirty)
            {
                if (log.IsInfoEnabled)
                    log.Info ("Found new prices, updating GnuCash file.");
                throw new NotImplementedException();
                //file.Save ();
            }

            if (log.IsInfoEnabled)
                log.Info ("Execute finished.");
        }

        #endregion

        static readonly private ILog log = LogManager.GetLogger (typeof (SloPricesCommand));
    }
}
