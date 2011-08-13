using System.Data.SqlClient;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHTutorial.Model.Mappings;

namespace NHTutorial.Tests
{
    public static class DBTestsHelper
    {
        public static ISessionFactory CreateSessionFactory()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = ".";
            connectionStringBuilder.InitialCatalog = "NHTutorial2";
            connectionStringBuilder.UserID = "sa";
            connectionStringBuilder.Password = "JungleJungle01!";
            string connectionString = connectionStringBuilder.ToString();
            return CreateSessionFactory(connectionString, false);            
        }

        public static ISessionFactory CreateSessionFactory(
            string connectionString, 
            bool createSchema)
        {
            FluentConfiguration config = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(
                              connectionString)
                              .ShowSql());
            config.Mappings(x => x.MergeMappings().FluentMappings.AddFromAssemblyOf<UserMap>());

            if (createSchema)
                config.ExposeConfiguration(BuildSchema);

            //ConfigureNHibernateValidator(config.BuildConfiguration(), databaseConfiguration);

            return config.BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            new SchemaExport(config).SetOutputFile("NHTutorial.CreateSchema.sql").Create(true, true);
        }        
    }
}