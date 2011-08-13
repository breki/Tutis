using System;

namespace NHTutorial.Model
{
    public class Transaction
    {
        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual Account FromAccount { get; set; }
        public virtual Account ToAccount { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Description { get; set; }
        public virtual User User { get; set; }
    }
}