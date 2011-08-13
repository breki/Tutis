using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using log4net;

namespace GnuCashUtils.Framework.DataMiners
{
    public class SkladiDataMiner : IPriceDataMiner
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

            CultureInfo slovenianCultureInfo = System.Globalization.CultureInfo.GetCultureInfo ("sl-SI");

            DataMiner dataMinerHelper = new DataMiner (new Uri ("http://www.skladi.com"), null, Encoding.UTF8);
            dataMinerHelper.FetchData ();

            dataMinerHelper.SkipOverText ("podrobneje, Arhiv,...");
            dataMinerHelper.SkipOverText ("1 leto");
            while (false == dataMinerHelper.Eof)
            {
                string[] possibilities = { "</table>", "<tr" };
                int possibilityFound;
                possibilityFound = dataMinerHelper.FindFirst (possibilities);
                if (possibilityFound == -1 || possibilityFound == 0)
                    break;

                dataMinerHelper.SkipOverText ("sklad.php?kod=");
                dataMinerHelper.SkipOverText ("<img");
                dataMinerHelper.SkipOverText (">");
                string mutualFundName = dataMinerHelper.FetchToText ("</a>").Trim ();
                dataMinerHelper.SkipOverText ("<td");
                dataMinerHelper.SkipOverText ("<td");
                
                dataMinerHelper.SkipOverText (">");
                dataMinerHelper.SkipWhiteSpace ();

                string dateString = null;
                if (true == dataMinerHelper.IsNextText ("<font"))
                {
                    dataMinerHelper.SkipOverText ("<font");
                    dataMinerHelper.SkipOverText (">");
                    dateString = dataMinerHelper.FetchToText ("</font>");
                }
                else
                    dateString = dataMinerHelper.FetchToText ("</td>");

                DateTime date = DateTime.Parse (dateString,
                     slovenianCultureInfo).Date;

                dataMinerHelper.SkipOverText ("<td>");
                decimal priceValue = decimal.Parse (dataMinerHelper.FetchToText ("</td>"),
                    slovenianCultureInfo);
                dataMinerHelper.SkipOverText ("</tr>");

                Commodity commodity = null;

                // first try to find the commodity using translation table
                string commodityId = settings.TranslateToCommodity (mutualFundName);

                if (commodityId != null)
                    commodity = gnuCashbook.GetCommodity (commodityId);
                else
                    // if that doesn't work, try to find the commodity by its name
                    commodity = gnuCashbook.FindCommodityByName (mutualFundName);

                if (log.IsDebugEnabled)
                    log.DebugFormat ("Found price (fund name='{0}', date='{1}', price={2}", mutualFundName, date, priceValue);

                if (commodity != null)
                {
                    Price price = new Price ();
                    price.Commodity = commodity;
                    price.Currency = settings.CurrencyUsed;
                    price.Source = "GnuCashUtils:www.skladi.com";
                    price.Time = date;
                    price.Type = "unknown";
                    price.Value = new DecimalValue ((int)(priceValue * commodity.Fraction), commodity.Fraction);
                    foundPrices.Add (price);
                }
            }
        }

        #endregion

        static readonly private ILog log = LogManager.GetLogger (typeof (SkladiDataMiner));
    }
}
