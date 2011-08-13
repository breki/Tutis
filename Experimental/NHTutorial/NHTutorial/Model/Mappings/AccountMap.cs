using FluentNHibernate.Mapping;

namespace NHTutorial.Model.Mappings
{
    public sealed class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Table("Accounts");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable().Unique();
            Map(x => x.AccountType).Not.Nullable();
            References(x => x.User)
                .Not.Nullable().Cascade.None().LazyLoad()
                .ForeignKey("FK_AccountUserId")
                .Column("UserId").Not.Update()
                .Index("I_AccountUserId");
        }
    }
}