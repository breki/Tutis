using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace GnuCashUtils.Framework
{
    public class XmlBookReader : IBookReader
    {
        public XmlBookReader(string fileName)
        {
            this.fileName = fileName;
        }

        public IFormatProvider FormatProvider
        {
            get { return formatProvider; }
            set { formatProvider = value; }
        }

        public Book Read()
        {
            book = new Book();
            xmlDoc = new XmlDocument();

            using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                xmlDoc.Load(stream);

            xmlDoc.PreserveWhitespace = false;

            namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("act", @"http://www.gnucash.org/XML/act");
            namespaceManager.AddNamespace("cmdty", @"http://www.gnucash.org/XML/cmdty");
            namespaceManager.AddNamespace("gnc", @"http://www.gnucash.org/XML/gnc");
            namespaceManager.AddNamespace("price", @"http://www.gnucash.org/XML/price");
            namespaceManager.AddNamespace("slot", @"http://www.gnucash.org/XML/slot");
            namespaceManager.AddNamespace("split", @"http://www.gnucash.org/XML/split");
            namespaceManager.AddNamespace("trn", @"http://www.gnucash.org/XML/trn");
            namespaceManager.AddNamespace("ts", @"http://www.gnucash.org/XML/ts");

            Analyze();

            return book;
        }

        public void Analyze()
        {
            foreach (XmlNode node in xmlDoc.SelectNodes("gnc-v2/gnc:book/gnc:commodity", namespaceManager))
            {
                Commodity commodity = new Commodity();

                XmlElement childNode = node.SelectSingleNode("cmdty:space", namespaceManager) as XmlElement;
                if (childNode != null)
                    commodity.Space = childNode.InnerText;

                childNode = node.SelectSingleNode("cmdty:id", namespaceManager) as XmlElement;
                if (childNode != null)
                    commodity.Id = childNode.InnerText;

                childNode = node.SelectSingleNode("cmdty:name", namespaceManager) as XmlElement;
                if (childNode != null)
                    commodity.Name = childNode.InnerText;

                childNode = node.SelectSingleNode("cmdty:fraction", namespaceManager) as XmlElement;
                if (childNode != null)
                    commodity.Fraction = Int32.Parse(childNode.InnerText, formatProvider);

                book.AddCommodity(commodity);
            }

            foreach (XmlNode node in xmlDoc.SelectNodes("gnc-v2/gnc:book/gnc:pricedb/price", namespaceManager))
            {
                Price price = new Price();

                XmlElement childNode = node.SelectSingleNode("price:id", namespaceManager) as XmlElement;
                if (childNode != null)
                    price.PriceId = new Guid(childNode.InnerText);

                string space = String.Empty;
                string id = String.Empty;

                childNode = node.SelectSingleNode("price:commodity/cmdty:space", namespaceManager) as XmlElement;
                if (childNode != null)
                    space = childNode.InnerText;
                childNode = node.SelectSingleNode("price:commodity/cmdty:id", namespaceManager) as XmlElement;
                if (childNode != null)
                    id = childNode.InnerText;
                price.Commodity = book.GetCommodity(Commodity.ConstructFullId(space, id));

                childNode = node.SelectSingleNode("price:currency/cmdty:space", namespaceManager) as XmlElement;
                if (childNode != null)
                    space = childNode.InnerText;
                childNode = node.SelectSingleNode("price:currency/cmdty:id", namespaceManager) as XmlElement;
                if (childNode != null)
                    id = childNode.InnerText;
                price.Currency = book.GetCommodity(Commodity.ConstructFullId(space, id));

                childNode = node.SelectSingleNode("price:time/ts:date", namespaceManager) as XmlElement;
                if (childNode != null)
                    price.Time = DateTime.Parse(childNode.InnerText, System.Globalization.CultureInfo.InvariantCulture);

                childNode = node.SelectSingleNode("price:source", namespaceManager) as XmlElement;
                if (childNode != null)
                    price.Source = childNode.InnerText;

                childNode = node.SelectSingleNode("price:type", namespaceManager) as XmlElement;
                if (childNode != null)
                    price.Type = childNode.InnerText;

                childNode = node.SelectSingleNode("price:value", namespaceManager) as XmlElement;
                if (childNode != null)
                    price.Value = ExpressionCalculator.GetDecimalValue(childNode.InnerText, formatProvider);

                book.AddPrice(price);
            }

            foreach (XmlNode node in xmlDoc.SelectNodes("gnc-v2/gnc:book/gnc:account", namespaceManager))
            {
                Account account = new Account();

                XmlElement childNode;

                childNode = node.SelectSingleNode("act:name", namespaceManager) as XmlElement;
                if (childNode != null)
                    account.Name = childNode.InnerText;

                childNode = node.SelectSingleNode("act:type", namespaceManager) as XmlElement;
                if (childNode != null)
                {
                    string s = childNode.InnerText.Trim();
                    for (int i = 0; i < Book.AccountTypesNames.Length; i++)
                    {
                        string accountTypeName = Book.AccountTypesNames[i];
                        if (accountTypeName == s)
                        {
                            account.Type = (AccountType)i;
                            break;
                        }
                    }

                    if (account.Type == AccountType.None)
                        throw new NotSupportedException(String.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "Account type '{0}' is not supported.", s));
                }

                childNode = node.SelectSingleNode("act:description", namespaceManager) as XmlElement;
                if (childNode != null)
                    account.Description = childNode.InnerText;

                childNode = node.SelectSingleNode("act:id", namespaceManager) as XmlElement;
                if (childNode != null)
                    account.Id = new Guid(childNode.InnerText);

                string space = String.Empty;
                string id = String.Empty;

                childNode = node.SelectSingleNode("act:commodity/cmdty:space", namespaceManager) as XmlElement;
                if (childNode != null)
                    space = childNode.InnerText;
                childNode = node.SelectSingleNode("act:commodity/cmdty:id", namespaceManager) as XmlElement;
                if (childNode != null)
                    id = childNode.InnerText;

                if (space.Length > 0 || id.Length > 0)
                    account.Commodity = book.GetCommodity(Commodity.ConstructFullId(space, id));

                childNode = node.SelectSingleNode("act:commodity-scu", namespaceManager) as XmlElement;
                if (childNode != null)
                    account.SmallerCommodityUnit = Int32.Parse(childNode.InnerText, formatProvider);

                childNode = node.SelectSingleNode("act:parent", namespaceManager) as XmlElement;
                if (childNode != null)
                {
                    account.ParentAccount = book.GetAccountById(new Guid(childNode.InnerText));
                    account.ParentAccount.ChildAccounts.Add(account);
                }
                else
                    book.RootAccount = account;

                foreach (XmlNode slotNode in node.SelectNodes("act:slots/slot", namespaceManager))
                {
                    XmlNode slotKeyNode = slotNode.SelectSingleNode("slot:key", namespaceManager);
                    if (slotKeyNode != null)
                    {
                        switch (slotKeyNode.InnerText.Trim())
                        {
                            case "placeholder":
                                XmlNode slotValueNode = slotNode.SelectSingleNode("slot:value", namespaceManager);
                                if (slotValueNode != null)
                                    account.IsPlaceholder = bool.Parse(slotValueNode.InnerText.Trim());
                                break;
                        }
                    }
                }

                book.AddAccount(account);
            }

            foreach (XmlNode node in xmlDoc.SelectNodes("gnc-v2/gnc:book/gnc:transaction", namespaceManager))
            {
                Transaction trans = new Transaction();

                XmlElement childNode;

                childNode = node.SelectSingleNode("trn:id", namespaceManager) as XmlElement;
                if (childNode != null)
                    trans.Id = new Guid(childNode.InnerText);

                string space = String.Empty;
                string id = String.Empty;

                childNode = node.SelectSingleNode("trn:currency/cmdty:space", namespaceManager) as XmlElement;
                if (childNode != null)
                    space = childNode.InnerText;
                childNode = node.SelectSingleNode("trn:currency/cmdty:id", namespaceManager) as XmlElement;
                if (childNode != null)
                    id = childNode.InnerText;
                trans.Currency = book.GetCommodity(Commodity.ConstructFullId(space, id));

                childNode = node.SelectSingleNode("trn:date-posted/ts:date", namespaceManager) as XmlElement;
                if (childNode != null)
                    trans.DatePosted = DateTime.Parse(childNode.InnerText, System.Globalization.CultureInfo.InvariantCulture);

                childNode = node.SelectSingleNode("trn:date-entered/ts:date", namespaceManager) as XmlElement;
                if (childNode != null)
                    trans.DateEntered = DateTime.Parse(childNode.InnerText, System.Globalization.CultureInfo.InvariantCulture);

                childNode = node.SelectSingleNode("trn:description", namespaceManager) as XmlElement;
                if (childNode != null)
                    trans.Description = childNode.InnerText;

                foreach (XmlNode splitNode in node.SelectNodes("trn:splits/trn:split", namespaceManager))
                {
                    TransactionSplit split = new TransactionSplit();
                    split.Transaction = trans;

                    XmlElement childNode2;

                    childNode2 = splitNode.SelectSingleNode("split:id", namespaceManager) as XmlElement;
                    if (childNode2 != null)
                        split.Id = new Guid(childNode2.InnerText);

                    childNode2 = splitNode.SelectSingleNode("split:action", namespaceManager) as XmlElement;
                    if (childNode2 != null)
                        split.Action = childNode2.InnerText;

                    childNode2 = splitNode.SelectSingleNode("split:reconciled-state", namespaceManager) as XmlElement;
                    if (childNode2 != null)
                    {
                        switch (childNode2.InnerText.Trim())
                        {
                            case "n":
                                split.ReconciledState = TransactionReconciledState.NotReconciled;
                                break;
                            case "c":
                                split.ReconciledState = TransactionReconciledState.Cleared;
                                break;
                            case "y":
                                split.ReconciledState = TransactionReconciledState.Reconciled;
                                break;
                            default:
                                throw new NotSupportedException(String.Format(System.Globalization.CultureInfo.InvariantCulture,
                                    "Transaction reconciled state '{0}' not supported.", childNode2.InnerText));
                        }
                    }

                    childNode2 = splitNode.SelectSingleNode("split:value", namespaceManager) as XmlElement;
                    if (childNode2 != null)
                        split.Value = ExpressionCalculator.GetDecimalValue(childNode2.InnerText, formatProvider);

                    childNode2 = splitNode.SelectSingleNode("split:quantity", namespaceManager) as XmlElement;
                    if (childNode2 != null)
                        split.Quantity = ExpressionCalculator.GetDecimalValue(childNode2.InnerText, formatProvider);

                    childNode2 = splitNode.SelectSingleNode("split:account", namespaceManager) as XmlElement;
                    split.Account = book.GetAccountById(new Guid(childNode2.InnerText));

                    trans.Splits.Add(split);
                    split.Account.TransactionSplits.Add(split);
                }

                book.AddTransaction(trans);
            }
        }

        private Book book;
        private readonly string fileName;
        private XmlNamespaceManager namespaceManager;
        private XmlDocument xmlDoc;
        private IFormatProvider formatProvider = CultureInfo.InvariantCulture;
    }
}