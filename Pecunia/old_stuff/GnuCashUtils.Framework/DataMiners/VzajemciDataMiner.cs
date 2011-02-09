using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace GnuCashUtils.Framework.DataMiners
{
    public class VzajemciDataMiner : IPriceDataMiner
    {
        #region IPriceDataMiner Members

        public void MineForPrices (Book gnuCashbook, PriceDataMiningSettings settings, IList<Price> foundPrices)
        {
            if (gnuCashbook == null)
                throw new ArgumentNullException ("gnuCashbook");                
            
            if (settings == null)
                throw new ArgumentNullException ("settings");

            if (foundPrices == null)
                throw new ArgumentNullException ("foundPrices");

            return;

            //DataMiner dataMinerHelper = new DataMiner (new Uri ("http://www.vzajemci.com"), null, Encoding.UTF8);
            //dataMinerHelper.FetchData ();

            //dataMinerHelper.SkipOverText ("<!-- ZAÈETEK IZPISA IZ BAZE-->");
            //while (false == dataMinerHelper.Eof)
            //{
            //    string[] possibilities = {"<!-- KONEC IZPISA IZ BAZE  -->", "vz_factsheet"};
            //    int possibilityFound;
            //    possibilityFound = dataMinerHelper.FindFirst (possibilities);
            //    if (possibilityFound == -1 || possibilityFound == 0)
            //        break;

            //    dataMinerHelper.SkipOverText ("vz_factsheet");
            //    dataMinerHelper.SkipOverText (">");
            //    string mutualFundName = dataMinerHelper.FetchToText ("</a>").Trim ();
            //    dataMinerHelper.SkipOverText ("<span>");
            //    dataMinerHelper.SkipOverText ("<span>");
            //    decimal priceValue = decimal.Parse (dataMinerHelper.FetchToText ("</span>"),
            //        System.Globalization.CultureInfo.InvariantCulture);
            //    dataMinerHelper.SkipOverText ("</span>");
            //    dataMinerHelper.SkipOverText ("<span>");
            //    string dateString = dataMinerHelper.FetchToText ("</span>");
            //    DateTime date = DateTime.Parse (dateString,
            //        System.Globalization.CultureInfo.GetCultureInfo ("sl-SI")).Date;

            //    Commodity commodity = null;

            //    // first try to find the commodity using translation table
            //    string commodityId = settings.TranslateToCommodity (mutualFundName);

            //    if (commodityId != null)
            //        commodity = gnuCashbook.GetCommodity (commodityId);
            //    else
            //        // if that doesn't work, try to find the commodity by its name
            //        commodity = gnuCashbook.FindCommodityByName (mutualFundName);

            //    if (log.IsDebugEnabled)
            //        log.DebugFormat ("Found price (fund name='{0}', date='{1}', price={2}", mutualFundName, date, priceValue);

            //    if (commodity != null)
            //    {
            //        Price price = new Price ();
            //        price.Commodity = commodity;
            //        price.Currency = settings.CurrencyUsed;
            //        price.Source = "GnuCashUtils:www.vzajemci.com";
            //        price.Time = date;
            //        price.Type = "unknown";
            //        price.Value = new DecimalValue ((int)(priceValue * commodity.Fraction), commodity.Fraction);

            //        gnuCashbook.AddPrice (price);

            //        foundPrices.Add (price);
            //    }
            //}
        }

        #endregion

        static readonly private ILog log = LogManager.GetLogger (typeof (VzajemciDataMiner));
    }
}
