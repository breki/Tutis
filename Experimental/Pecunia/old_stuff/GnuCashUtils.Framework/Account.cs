using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GnuCashUtils.Framework
{
    public enum AccountType
    {
        None,
        Bank,
        Cash,
        CreditCard,
        Asset,
        Liability,
        Stock,
        MutualFund,
        Currency,
        Income,
        Expense,
        Equity,
        Receivable,
        Payable,
        Root,
    }

    public class Account
    {
        public Commodity Commodity
        {
            get { return commodity; }
            set { commodity = value; }
        }

        public int Depth
        {
            get
            {
                if (parentAccount != null)
                    return parentAccount.Depth + 1;

                return 0;
            }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int SmallerCommodityUnit
        {
            get { return smallerCommodityUnit; }
            set { smallerCommodityUnit = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public AccountType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Account ParentAccount
        {
            get { return parentAccount; }
            set { parentAccount = value; }
        }

        public IList<Account> ChildAccounts
        {
            get { return childAccounts; }
        }

        public IList<TransactionSplit> TransactionSplits
        {
            get { return transactionSplits; }
        }

        public bool IsPlaceholder
        {
            get { return isPlaceholder; }
            set { isPlaceholder = value; }
        }

        private string name;
        private Guid id;
        private AccountType type = AccountType.None;
        private Commodity commodity;
        private int smallerCommodityUnit;
        private string description;
        private Account parentAccount;
        private List<Account> childAccounts = new List<Account> ();
        private List<TransactionSplit> transactionSplits = new List<TransactionSplit> ();
        private bool isPlaceholder;
    }
}
