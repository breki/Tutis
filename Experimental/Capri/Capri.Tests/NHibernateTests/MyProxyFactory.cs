using System;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;

namespace Capri.Tests.NHibernateTests
{
    public class MyProxyFactory : IProxyFactory
    {
        public void PostInstantiate(string entityName, Type persistentClass, Iesi.Collections.Generic.ISet<Type> interfaces, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType)
        {
        }

        public INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            throw new NotImplementedException();
        }

        public object GetFieldInterceptionProxy(object instanceToWrap)
        {
            throw new NotImplementedException();
        }
    }
}