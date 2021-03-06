using System.Collections.Generic;

namespace NHTutorial.Model
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string Name { get; set; }
        public virtual string PasswordHash { get; set; }

        public virtual IList<Account> Accounts
        {
            get { return accounts; }
            set { accounts = value; }
        }

        public virtual Account CreateAccount(string accountName, AccountType accountType)
        {
            Account account = new Account();
            account.User = this;
            account.Name = accountName;
            account.AccountType = accountType;
            return account;
        }

        public virtual Transaction CreateTransaction(
            Account fromAccount,
            Account toAccount)
        {
            Transaction trans = new Transaction();
            trans.User = this;
            trans.FromAccount = fromAccount;
            trans.ToAccount = toAccount;
            return trans;
        }

        private IList<Account> accounts = new List<Account>();
    }
}