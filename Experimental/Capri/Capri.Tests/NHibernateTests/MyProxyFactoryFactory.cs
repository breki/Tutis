using System;
using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace Capri.Tests.NHibernateTests
{
    public class MyProxyFactoryFactory : IProxyFactoryFactory
    {
        public IProxyFactory BuildProxyFactory()
        {
            return new MyProxyFactory();
        }

        public bool IsProxy(object entity)
        {
            return entity.GetType() != typeof(User);
        }

        public IProxyValidator ProxyValidator
        {
            get { return new MyProxyValidator(); }
        }

        public bool IsInstrumented(Type entityClass)
        {
            return false;
        }
    }
}