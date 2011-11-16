using System;
using NHibernate.Properties;

namespace Capri.Tests.NHibernateTests
{
    public class MyPropertyAccessor : IPropertyAccessor
    {
        public IGetter GetGetter(Type theClass, string propertyName)
        {
            return new MyGetter(theClass, propertyName);
        }

        public ISetter GetSetter(Type theClass, string propertyName)
        {
            return new MySetter(theClass, propertyName);
        }

        public bool CanAccessThroughReflectionOptimizer
        {
            get { return false; }
        }
    }
}