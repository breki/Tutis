using FluentNHibernate.Mapping;

namespace NHTutorial.Model.Mappings
{
    public sealed class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            ImportType<AmountByDate>();

            Table("Transactions");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Date).Not.Nullable();
            Map(x => x.Description).Nullable();
            Map(x => x.Amount).Not.Nullable();

            References(x => x.FromAccount)
                .Not.Nullable().Cascade.None()
                .ForeignKey("FK_TransactionFromAccountId")
                .Column("FromAccountId")
                .Index("FromAccountId");

            References(x => x.ToAccount)
                .Not.Nullable().Cascade.None()
                .ForeignKey("FK_TransactionToAccountId")
                .Column("ToAccountId")
                .Index("ToAccountId");

            References(x => x.User)
                .Not.Nullable().Cascade.None().LazyLoad()
                .ForeignKey("FK_TransactionUserId")
                .Column("UserId").Not.Update()
                .Index("I_TransactionUserId");
        }
    }
}