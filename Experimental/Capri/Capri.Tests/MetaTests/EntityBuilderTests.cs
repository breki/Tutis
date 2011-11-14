using Capri.Meta;
using NUnit.Framework;

namespace Capri.Tests.MetaTests
{
    public class EntityBuilderTests
    {
        [Test]
        public void Test()
        {
            BuildModel();
        }

        public static EntityBuilder BuildModel()
        {
            EntityBuilder builder = new EntityBuilder();
            builder
                .Entity("BusinessArea")
                    .Text("Name")
                .Entity("SubBusinessArea")
                    .Text("Name")
                    .BelongsTo("BusinessArea")
                .Entity("Employee")
                    .Text("Name")
                    .Text("Surname")
                    .IsPartOf("SubBusinessArea")
                    .IsPartOf("Employee", "Subordinate")
                .Entity("Project")
                    .Text("Name")
                    .Text("Tags")
                    .IsPartOf("SubBusinessArea")
                    .Has("Employee", "ProjectManager")
                .Entity("ProjectMembership")
                    .Has("Project")
                    .Has("Employee")
                .Entity("Task")
                    .Text("Name")
                    .Text("Tags")
                    .BelongsTo("Project")
                .Entity("EffortEntry")
                    .Date("Date")
                    .Integer("Hours")
                    .Has("Project")
                    .Has("Employee");
            return builder;
        }
    }
}