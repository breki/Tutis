using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GnuCashUtils.Framework
{
    public class Book
    {
        public Account RootAccount
        {
            get { return rootAccount; }
            set { rootAccount = value; }
        }

        public IDictionary<Guid, Transaction> Transactions
        {
            get { return transactions; }
        }

        public int TransactionsCount { get { return transactions.Count; } }

        public void AddAccount(Account account)
        {
            accounts[account.Id] = account;
        }

        public void AddCommodity(Commodity commodity)
        {
            commodities.Add(commodity.FullId, commodity);
        }

        public void AddPrice (Price price)
        {
            if (price == null)
                throw new ArgumentNullException ("price");                
            
            prices.Add (price);

            //string nsPrice = @"http://www.gnucash.org/XML/price";
            //string nsCmdty = @"http://www.gnucash.org/XML/cmdty";
            //string nsTs = @"http://www.gnucash.org/XML/ts";

            //XmlElement priceEl = file.XmlDocument.CreateElement ("price");

            //XmlElement priceIdEl = file.XmlDocument.CreateElement ("price", "id", nsPrice);
            //XmlAttribute priceIdTypeAtt = file.XmlDocument.CreateAttribute ("type");
            //priceIdTypeAtt.Value = "guid";
            //priceIdEl.Attributes.Append (priceIdTypeAtt);
            //priceIdEl.InnerText = price.PriceId.ToString ("N", System.Globalization.CultureInfo.InvariantCulture);
            //priceEl.AppendChild (priceIdEl);

            //XmlElement priceCommodityEl = file.XmlDocument.CreateElement ("price", "commodity", nsPrice);

            //XmlElement commoditySpaceEl = file.XmlDocument.CreateElement ("cmdty", "space", nsCmdty);
            //commoditySpaceEl.InnerText = price.Commodity.Space;
            //priceCommodityEl.AppendChild (commoditySpaceEl);

            //XmlElement commodityIdEl = file.XmlDocument.CreateElement ("cmdty", "id", nsCmdty);
            //commodityIdEl.InnerText = price.Commodity.Id;
            //priceCommodityEl.AppendChild (commodityIdEl);

            //priceEl.AppendChild (priceCommodityEl);

            //XmlElement priceCurrencyEl = file.XmlDocument.CreateElement ("price", "currency", nsPrice);

            //commoditySpaceEl = file.XmlDocument.CreateElement ("cmdty", "space", nsCmdty);
            //commoditySpaceEl.InnerText = price.Currency.Space;
            //priceCurrencyEl.AppendChild (commoditySpaceEl);

            //commodityIdEl = file.XmlDocument.CreateElement ("cmdty", "id", nsCmdty);
            //commodityIdEl.InnerText = price.Currency.Id;
            //priceCurrencyEl.AppendChild (commodityIdEl);

            //priceEl.AppendChild (priceCurrencyEl);

            //XmlElement priceTimeEl = file.XmlDocument.CreateElement ("price", "time", nsPrice);

            //XmlElement dateEl = file.XmlDocument.CreateElement ("ts", "date", nsTs);
            //TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(price.Time);
            //dateEl.InnerText = String.Format (System.Globalization.CultureInfo.InvariantCulture,
            //    "{0}{1:00}", price.Time.ToString ("yyyy-MM-dd hh:mm:ss zz", System.Globalization.CultureInfo.InvariantCulture),
            //    utcOffset.Minutes);
            //priceTimeEl.AppendChild (dateEl);

            //priceEl.AppendChild (priceTimeEl);

            //XmlElement priceSourceEl = file.XmlDocument.CreateElement ("price", "source", nsPrice);
            //priceSourceEl.InnerText = price.Source;
            //priceEl.AppendChild (priceSourceEl);

            //XmlElement priceTypeEl = file.XmlDocument.CreateElement ("price", "type", nsPrice);
            //priceTypeEl.InnerText = price.Type;
            //priceEl.AppendChild (priceTypeEl);

            //XmlElement priceValueEl = file.XmlDocument.CreateElement ("price", "value", nsPrice);
            //priceValueEl.InnerText = price.Value.ToGnuCashValueString();
            //priceEl.AppendChild (priceValueEl);

            //XmlNode priceDbNode = file.XmlDocument.SelectSingleNode ("gnc-v2/gnc:book/gnc:pricedb", namespaceManager);
            //priceDbNode.AppendChild (priceEl);
        }

        public void AddTransaction(Transaction transaction)
        {
            transactions.Add(transaction.Id, transaction);
        }

        public AccountBalance CalculateAccountBalance (
            Account account, 
            IDictionary<Guid, AccountBalance> allAccountsBalances, 
            DateTime? fromTime,
            DateTime? untilTime, 
            string currencyId)
        {
            AccountBalance accountBalance = new AccountBalance (account);

            // first calculate totals for children accounts and add those total to the main total
            foreach (Account child in account.ChildAccounts)
            {
                AccountBalance childBalance = CalculateAccountBalance (child, allAccountsBalances, fromTime, untilTime, currencyId);
                if (childBalance.Balance.HasValue)
                {
                    if (false == accountBalance.Balance.HasValue)
                        accountBalance.Balance = 0;

                    accountBalance.Balance += childBalance.Balance.Value;
                }
            }

            // now calculare total for the main account
            foreach (TransactionSplit split in account.TransactionSplits)
            {
                // skip transactions which are outside the specified timeframe
                if (untilTime.HasValue && split.Transaction.DatePosted >= untilTime)
                    continue;

                if (fromTime.HasValue && split.Transaction.DatePosted < fromTime)
                    continue;

                if (false == accountBalance.Balance.HasValue)
                    accountBalance.Balance = 0;

                accountBalance.Balance += split.Quantity.Value;
            }

            if (account.Commodity != null && accountBalance.Balance.HasValue)
            {
                // convert to base currency
                accountBalance.Balance *= this.CalculateHistoricalPriceForCommodity (account.Commodity.FullId, currencyId, untilTime);
            }

            // write the total into the dictionary
            allAccountsBalances[account.Id] = accountBalance;

            return accountBalance;
        }

        public IDictionary<Guid, AccountBalance> CalculateAccountsBalances (
            DateTime? fromTime, 
            DateTime? untilTime, 
            string currencyId)
        {
            Dictionary<Guid, AccountBalance> totals = new Dictionary<Guid, AccountBalance> ();

            CalculateAccountBalance (RootAccount, totals, fromTime, untilTime, currencyId);

            return totals;
        }

        public decimal CalculateHistoricalPriceForCommodity (string commodityId, string currencyId, DateTime? time)
        {
            if (commodityId == currencyId)
                return 1;

            IList<Price> prices = ListPricesForCommodity (commodityId, currencyId);

            if (prices.Count == 0)
                return 0;

            if (time.HasValue)
            {
                if (time <= prices[0].Time)
                    return prices[0].Value.Value;

                for (int i = 1; i < prices.Count; i++)
                {
                    if (time <= prices[i].Time)
                        return prices[i-1].Value.Value;
                }
            }

            return prices[prices.Count - 1].Value.Value;
        }

        public IEnumerable<Account> EnumerateAccounts ()
        {
            return accounts.Values;
        }

        public DateTime? FindFirstTransactionDatePosted ()
        {
            DateTime? firstDate = null;
            foreach (Transaction trans in transactions.Values)
            {
                if (false == firstDate.HasValue || trans.DatePosted < firstDate.Value)
                    firstDate = trans.DatePosted;
            }

            return firstDate;
        }

        /// <summary>
        /// Finds the commodity by its name.
        /// </summary>
        /// <param name="commmodityName">Name of the commmodity.</param>
        /// <returns><see cref="Commodity"/> object if it finds one; otherwise <c>null</c>.</returns>
        public Commodity FindCommodityByName (string commodityName)
        {
            if (commodityName == null)
                throw new ArgumentNullException ("commodityName");                
            
            foreach (Commodity commodity in commodities.Values)
            {
                if (String.Compare (commodityName, commodity.Name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    return commodity;
            }

            return null;
        }

        /// <summary>
        /// Finds the commodity by its full ID.
        /// </summary>
        /// <param name="commmodityName">Full ID of the commmodity.</param>
        /// <returns><see cref="Commodity"/> object if it finds one; otherwise <c>null</c>.</returns>
        public Commodity FindCommodityById (string commodityFullId)
        {
            if (commodityFullId == null)
                throw new ArgumentNullException ("commodityFullId");

            foreach (Commodity commodity in commodities.Values)
            {
                if (String.Compare (commodityFullId, commodity.FullId, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    return commodity;
            }

            return null;
        }

        public Price FindPriceForCommodityAndDate (string commodityId, DateTime date)
        {
            foreach (Price price in prices)
            {
                if (price.Commodity.FullId == commodityId && price.Time.Date == date.Date)
                    return price;
            }

            return null;
        }

        public Account FindRootAccountForType (AccountType accountType)
        {
            return FindRootAccountForTypePrivate (rootAccount, accountType);
        }

        public Account GetAccountById (Guid accountId)
        {
            return accounts[accountId];
        }

        public Commodity GetCommodity (string commodityId)
        {
            if (commodityId == null)
                throw new ArgumentNullException ("commodityId");                
            
            return commodities[commodityId];
        }

        public IList<Price> ListPricesForCommodity (string commodityId, string currencyId)
        {
            List<Price> sortedPrices = new List<Price> ();

            foreach (Price price in prices)
            {
                if (price.Commodity.FullId == commodityId && price.Currency.FullId == currencyId)
                    sortedPrices.Add (price);
            }

            sortedPrices.Sort ();

            return sortedPrices;
        }

        private Account FindRootAccountForTypePrivate (Account rootAccount, AccountType accountType)
        {
            if (rootAccount.Type == accountType)
                return rootAccount;

            foreach (Account child in rootAccount.ChildAccounts)
            {
                Account foundAccount = FindRootAccountForTypePrivate (child, accountType);
                if (foundAccount != null)
                    return foundAccount;
            }


            return null;
        }

        private Dictionary<Guid, Account> accounts = new Dictionary<Guid, Account> ();
        private Dictionary<string, Commodity> commodities = new Dictionary<string, Commodity> ();
        private List<Price> prices = new List<Price> ();
        private Account rootAccount = new Account ();
        private Dictionary<Guid, Transaction> transactions = new Dictionary<Guid, Transaction> ();

        public static readonly string[] AccountTypesNames = new string[] { String.Empty, "BANK", "CASH", "CREDIT", "ASSET", "LIABILITY", "STOCK", "MUTUAL", "CURRENCY", "INCOME", "EXPENSE", "EQUITY", "RECEIVABLE", "PAYABLE", "ROOT" };
    }
}
