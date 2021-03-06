using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace Capri.Tests.NHibernateTests
{
    public class SimpleClassTests
    {
        [Test]
        public void Test()
        {
            //HbmMapping mapping = new HbmMapping();

            //RootClass rootClass = new RootClass();
            //rootClass.EntityName = "User";

            Configuration config = new Configuration();
            config
                .DataBaseIntegration(
                x =>
                    {
                        x.Dialect<SQLiteDialect>();
                        x.Driver<SQLite20Driver>();
                        x.ConnectionString = "Data Source=test.sqlite;Version=3;New=True;";
                    })
                .Proxy(x => x.ProxyFactoryFactory<MyProxyFactoryFactory>())
                .SetProperty(Environment.CurrentSessionContextClass, "thread_static");

            config
                .AddFile("NHibernateTests/User.hbm.xml");

            var props = config.Properties;
            if (props.ContainsKey(Environment.ConnectionStringName))
                props.Remove(Environment.ConnectionStringName);

            using (ISessionFactory sessionFactory = config.BuildSessionFactory())
            {
                SchemaExport schemaExport = new SchemaExport(config);
                schemaExport.SetOutputFile("test.sql");
                schemaExport.Create(true, true);

                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    User user = new User();
                    user.Name = "Igor";
                    session.Save(user);

                    transaction.Commit();
                }

                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    IList<User> users = session.CreateCriteria<User>().List<User>();
                    Assert.AreEqual(1, users.Count);
                }
            }
        }
    }
}