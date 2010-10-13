using System.Reflection;
using Gallio.Framework;
using log4net;
using MbUnit.Framework;
using NHibernate;
using NHTutorial.Repositories;

namespace NHTutorial.Tests
{
    public class DBTestsBase
    {
        protected ISessionFactory SessionFactory
        {
            get { return sessionFactory; }
        }

        protected IUsersRepository UsersRepository
        {
            get { return usersRepository; }
        }

        protected IAccountsRepository AccountsRepository
        {
            get { return accountsRepository; }
        }

        protected ITransactionsRepository TransactionsRepository
        {
            get { return transactionsRepository; }
        }

        [SetUp]
        protected virtual void Setup()
        {
            log.DebugFormat(">> {0}", TestContext.CurrentContext.TestStep.FullName);

            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    usersRepository.DeleteUser(session, EmailAddress);
                    transaction.Commit();
                }
            }
        }

        [FixtureSetUp]
        protected virtual void FixtureSetup()
        {
            sessionFactory = DBTestsHelper.CreateSessionFactory();
            usersRepository = new UsersRepository();
            accountsRepository = new AccountsRepository();    
            transactionsRepository = new TransactionsRepository();
        }

        protected const string EmailAddress = "bill.gates@microsoft.com";

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ISessionFactory sessionFactory;
        private IUsersRepository usersRepository;
        private IAccountsRepository accountsRepository;
        private ITransactionsRepository transactionsRepository;
    }   
}