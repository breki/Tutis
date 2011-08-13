using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCashUtils.Framework
{
    public class Transaction
    {
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public Commodity Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public DateTime DatePosted
        {
            get { return datePosted; }
            set { datePosted = value; }
        }

        public DateTime DateEntered
        {
            get { return dateEntered; }
            set { dateEntered = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public IList<TransactionSplit> Splits
        {
            get { return splits; }
        }

        private Guid id;
        private Commodity currency;
        private DateTime datePosted;
        private DateTime dateEntered;
        private string description;
        private List<TransactionSplit> splits = new List<TransactionSplit> ();
    }
}
