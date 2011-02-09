using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GnuCashUtils.Framework
{
    public class PriceDataMiningSettings
    {
        public Commodity CurrencyUsed
        {
            get { return currencyUsed; }
            set { currencyUsed = value; }
        }

        public void AddCommodityTranslation (string regex, string commodityId)
        {
            commodityTranslationDictionary.Add (regex, commodityId);
        }

        public string TranslateToCommodity (string name)
        {
            foreach (string regex in commodityTranslationDictionary.Keys)
            {
                Regex reg = new Regex (regex, RegexOptions.Singleline);
                if (reg.IsMatch (name))
                    return commodityTranslationDictionary[regex];
            }

            return null;
        }

        private Commodity currencyUsed;
        private Dictionary<string, string> commodityTranslationDictionary = new Dictionary<string, string> ();
    }
}
