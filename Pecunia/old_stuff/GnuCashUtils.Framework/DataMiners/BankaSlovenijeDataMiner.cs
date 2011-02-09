using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using log4net;

namespace GnuCashUtils.Framework.DataMiners
{
    public class BankaSlovenijeDataMiner : IPriceDataMiner
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

            DataMiner dataMinerHelper = new DataMiner (new Uri ("http://www.bsi.si/podatki/tec-bs.asp"), null, Encoding.UTF8);
            dataMinerHelper.FetchData ();

            dataMinerHelper.SkipOverText ("lista z dne ");
            string dateString = dataMinerHelper.FetchToText ("</h2>");
            DateTime date = DateTime.Parse (dateString, slovenianCultureInfo).Date;

            dataMinerHelper.SkipOverText ("<table");
            dataMinerHelper.SkipOverText ("</tr>");

            while (false == dataMinerHelper.Eof)
            {
                string[] possibilities = { "</table>", "<tr>" };
                int possibilityFound;
                possibilityFound = dataMinerHelper.FindFirst (possibilities);
                if (possibilityFound == -1 || possibilityFound == 0)
                    break;

                dataMinerHelper.SkipOverText ("<td>");
                dataMinerHelper.SkipOverText ("<td>");
                dataMinerHelper.SkipOverText ("<td>");
                string currencyAbbreviation = dataMinerHelper.FetchToText ("</td>");
                dataMinerHelper.SkipOverText ("<td>");
                dataMinerHelper.SkipOverText (@"<td align=""right"">");
                string valueString = dataMinerHelper.FetchToText ("</td>");
                decimal priceValue = 1m / decimal.Parse (valueString, slovenianCultureInfo);

                if (log.IsDebugEnabled)
                    log.DebugFormat ("Found price (currency='{0}', date='{1}', price={2}", currencyAbbreviation, date, priceValue);

                Commodity commodity = gnuCashbook.FindCommodityById (Commodity.ConstructFullId ("ISO4217", currencyAbbreviation));

                if (commodity != null)
                {
                    Price price = new Price ();
                    price.Commodity = commodity;
                    price.Currency = settings.CurrencyUsed;
                    price.Source = "GnuCashUtils:Banka Slovenije";
                    price.Time = date;
                    price.Type = "unknown";
                    price.Value = new DecimalValue ((int)(priceValue * commodity.Fraction), commodity.Fraction);
                    foundPrices.Add (price);
                }
            }
        }

        #endregion

        static readonly private ILog log = LogManager.GetLogger (typeof (BankaSlovenijeDataMiner));
    }
}
