using System;
using System.Collections;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Properties;

namespace Capri.Tests.NHibernateTests
{
    public class MyGetter : IGetter
    {
        public MyGetter(Type theClass, string propertyName)
        {
            this.theClass = theClass;
            this.propertyName = propertyName;
        }

        public object Get(object target)
        {
            if (propertyName == "Id")
                return ((User)target).Id;
            if (propertyName == "Name")
                return ((User)target).Name;
            throw new NotImplementedException();
        }

        public Type ReturnType
        {
            get
            {
                if (propertyName == "Id")
                    return typeof(Int32);
                if (propertyName == "Name")
                    return typeof(string);
                throw new NotImplementedException();
            }
        }

        public string PropertyName
        {
            get { throw new NotImplementedException(); }
        }

        public MethodInfo Method
        {
            get { return null; }
        }

        public object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
        {
            return Get(owner);
        }

        private readonly Type theClass;
        private readonly string propertyName;
    }
}