using System.Data.SqlClient;
using MbUnit.Framework;
using NHibernate;

namespace NHTutorial.Tests
{
    public class Tests1
    {
        /// <summary>
        /// Creates a database structure (tables, indices, constraints) for the tutorial DB. 
        /// </summary>
        /// <remarks>You need to create the database in SQL Server first.</remarks>
        [Test]
        public void CreateStructure()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource = ".";
            connectionStringBuilder.InitialCatalog = "NHTutorial2";
            connectionStringBuilder.UserID = "sa";
            connectionStringBuilder.Password = "JungleJungle01!";
            string connectionString = connectionStringBuilder.ToString();

            using (ISessionFactory sessionFactory = DBTestsHelper.CreateSessionFactory(
                connectionString, 
                true))
            {
                //using (ISession session = sessionFactory.OpenSession())
                //{
                //    using (ITransaction transaction = session.BeginTransaction())
                //    {
                //        transaction.Commit();
                //    }
                //}
            }            
        }
    }
}