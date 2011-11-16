using System;
using System.Reflection;
using NHibernate.Properties;

namespace Capri.Tests.NHibernateTests
{
    public class MySetter : ISetter
    {
        public MySetter(Type theClass, string propertyName)
        {
            this.theClass = theClass;
            this.propertyName = propertyName;
        }

        public void Set(object target, object value)
        {
            if (propertyName == "Id")
            {
                ((User)target).Id = (int)value;
                return;
            }

            if (propertyName == "Name")
            {
                ((User)target).Name = (string)value;
                return;
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

        private readonly Type theClass;
        private readonly string propertyName;
    }
}