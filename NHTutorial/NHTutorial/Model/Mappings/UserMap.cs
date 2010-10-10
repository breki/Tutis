using FluentNHibernate.Mapping;

namespace NHTutorial.Model.Mappings
{
    public sealed class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.EmailAddress).Not.Nullable().Unique();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.PasswordHash).Not.Nullable();
            HasMany(x => x.Accounts)
                .Cascade.Delete()
                .ForeignKeyConstraintName("FK_Account")
                .KeyColumn("UserId");
        }
    }
}