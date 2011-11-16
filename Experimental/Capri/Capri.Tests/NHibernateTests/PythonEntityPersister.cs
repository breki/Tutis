using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace Capri.Tests.NHibernateTests
{
    public class PythonEntityPersister : AbstractEntityPersister
    {
        public PythonEntityPersister(
            PersistentClass persistentClass, 
            ICacheConcurrencyStrategy cache, 
            ISessionFactoryImplementor factory,
            IMapping cfg) 
            : base(persistentClass, cache, factory)
        {
        }

        public override string GetSubclassTableName(int j)
        {
            throw new NotImplementedException();
        }

        public override string[] ConstraintOrderedTableNameClosure
        {
            get { throw new NotImplementedException(); }
        }

        public override string DiscriminatorSQLValue
        {
            get { throw new NotImplementedException(); }
        }

        public override object DiscriminatorValue
        {
            get { throw new NotImplementedException(); }
        }

        public override string[] PropertySpaces
        {
            get { throw new NotImplementedException(); }
        }

        protected override int SubclassTableSpan
        {
            get { throw new NotImplementedException(); }
        }

        protected override int TableSpan
        {
            get { throw new NotImplementedException(); }
        }

        public override string TableName
        {
            get { throw new NotImplementedException(); }
        }

        public override string[][] ContraintOrderedTableKeyColumnClosure
        {
            get { throw new NotImplementedException(); }
        }

        public override IType DiscriminatorType
        {
            get { throw new NotImplementedException(); }
        }

        protected override string[] GetSubclassTableKeyColumns(int j)
        {
            throw new NotImplementedException();
        }

        protected override bool IsClassOrSuperclassTable(int j)
        {
            throw new NotImplementedException();
        }

        protected override bool IsTableCascadeDeleteEnabled(int j)
        {
            throw new NotImplementedException();
        }

        protected override string GetTableName(int table)
        {
            throw new NotImplementedException();
        }

        protected override string[] GetKeyColumns(int table)
        {
            throw new NotImplementedException();
        }

        protected override bool IsPropertyOfTable(int property, int table)
        {
            throw new NotImplementedException();
        }

        protected override int GetSubclassPropertyTableNumber(int i)
        {
            throw new NotImplementedException();
        }

        public override string FilterFragment(string alias)
        {
            throw new NotImplementedException();
        }

        public override string GetPropertyTableName(string propertyName)
        {
            throw new NotImplementedException();
        }

        public override string FromTableFragment(string alias)
        {
            throw new NotImplementedException();
        }

        public override string GetSubclassForDiscriminatorValue(object value)
        {
            throw new NotImplementedException();
        }

        public override string GetSubclassPropertyTableName(int i)
        {
            throw new NotImplementedException();
        }

        protected override int[] SubclassColumnTableNumberClosure
        {
            get { throw new NotImplementedException(); }
        }

        protected override int[] SubclassFormulaTableNumberClosure
        {
            get { throw new NotImplementedException(); }
        }

        protected override int[] PropertyTableNumbersInSelect
        {
            get { throw new NotImplementedException(); }
        }

        protected override int[] PropertyTableNumbers
        {
            get { throw new NotImplementedException(); }
        }
    }
}