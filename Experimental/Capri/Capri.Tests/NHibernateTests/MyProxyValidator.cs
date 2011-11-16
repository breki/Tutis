using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Proxy;

namespace Capri.Tests.NHibernateTests
{
    public class MyProxyValidator : IProxyValidator
    {
        public ICollection<string> ValidateType(Type type)
        {
            return null;
        }

        public bool IsProxeable(MethodInfo method)
        {
            throw new NotImplementedException();
        }
    }
}