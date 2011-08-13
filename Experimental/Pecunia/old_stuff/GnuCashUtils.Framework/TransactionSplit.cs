using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCashUtils.Framework
{
    public enum TransactionReconciledState
    {
        NotReconciled,
        Cleared,
        Reconciled,
    }

    public class TransactionSplit
    {
        public Transaction Transaction
        {
            get { return transaction; }
            set { transaction = value; }
        }

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        public TransactionReconciledState ReconciledState
        {
            get { return reconciledState; }
            set { reconciledState = value; }
        }

        public DecimalValue Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public DecimalValue Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public Account Account
        {
            get { return account; }
            set { account = value; }
        }

        private Transaction transaction;
        private Guid id;
        private string action;
        private TransactionReconciledState reconciledState;
        private DecimalValue value;
        private DecimalValue quantity;
        private Account account;
    }
}
