using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Clavis
{
    public interface IClavisFile : IClavisSessionFactory
    {
        string FileName { get; }
        ITimeService TimeService { get; }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        TStoreImpl OpenStore<TStoreImpl> (string storeName, bool createNew)
            where TStoreImpl : IClavisStore, new();

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        void DeleteStore<TStoreImpl> (string storeName) where TStoreImpl : IClavisStore, new ();
        bool HasStore(string storeName);
    }

    public class ClavisFile : IClavisFile
    {
        public ClavisFile(string fileName, ITimeService timeService)
        {
            this.fileName = fileName;
            this.timeService = timeService;
        }

        public string FileName
        {
            get { return fileName; }
        }

        public ITimeService TimeService
        {
            get { return timeService; }
        }

        public void Create()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            string dirName = Path.GetDirectoryName(fileName);
            if (false == string.IsNullOrEmpty(dirName) && false == Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            using (ClavisSession session = (ClavisSession)OpenSession())
            {
                using (SQLiteCommand command = session.Connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = MetadataSql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TStoreImpl OpenStore<TStoreImpl> (string storeName, bool createNew) where TStoreImpl : IClavisStore, new ()
        {
            if (createNew)
                DeleteStore<TStoreImpl>(storeName);

            if (!HasStore(storeName))
            {
                if (createNew)
                    return CreateStore(storeName, () => new TStoreImpl());

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Clavis store '{0}' does not exist.",
                    storeName);
                throw new InvalidOperationException(message); 
            }

            TStoreImpl store = new TStoreImpl();
            store.Initialize(this, storeName);
            return store;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void DeleteStore<TStoreImpl> (string storeName) where TStoreImpl : IClavisStore, new ()
        {
            TStoreImpl store = new TStoreImpl();

            using (ClavisSession session = (ClavisSession)OpenSession())
            {
                store.Initialize(this, storeName);
                store.DestroyStore(session);

                using (IClavisTransaction transaction = session.BeginTransaction())
                {
                    using (SQLiteCommand command = session.Connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            "DELETE FROM [METADATA] WHERE STORENAME = :StoreName";

                        SQLiteParameter parameter;
                        parameter = command.CreateParameter();
                        parameter.ParameterName = "StoreName";
                        parameter.Value = storeName;
                        command.Parameters.Add(parameter);

                        //try
                        //{
                            command.ExecuteNonQuery();
                        //}
                        //catch (SQLiteException ex)
                        //{
                        //    if (ex.ErrorCode == SQLiteErrorCode.Constraint)
                        //    {
                        //        string message = string.Format(
                        //            CultureInfo.InvariantCulture,
                        //            "Store '{0}' already exists in the file '{1}'.",
                        //            storeName,
                        //            fileName);
                        //        throw new InvalidOperationException(message);
                        //    }

                        //    throw;
                        //}
                    }

                    transaction.Commit();
                }
            }
        }

        public bool HasStore(string storeName)
        {
            using (ClavisSession session = (ClavisSession)OpenSession())
            {
                using (SQLiteCommand command = session.Connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT STORENAME FROM [METADATA] WHERE STORENAME = :StoreName";

                    SQLiteParameter parameter;
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "StoreName";
                    parameter.Value = storeName;
                    command.Parameters.Add(parameter);

                    return command.ExecuteScalar() != null;
                }
            }
        }

        public IClavisSession OpenSession()
        {
            return new ClavisSession(this);
        }

        protected TStoreImpl CreateStore<TStoreImpl>(string storeName, Func<TStoreImpl> storeFactoryFunc) where TStoreImpl : IClavisStore
        {
            TStoreImpl store = storeFactoryFunc();

            using (ClavisSession session = (ClavisSession)OpenSession())
            {
                using (IClavisTransaction transaction = session.BeginTransaction())
                {
                    using (SQLiteCommand command = session.Connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            "INSERT INTO [METADATA] (STORENAME, STOREIMPL) VALUES (:StoreName, :StoreImpl)";

                        SQLiteParameter parameter;
                        parameter = command.CreateParameter();
                        parameter.ParameterName = "StoreName";
                        parameter.Value = storeName;
                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "StoreImpl";
                        parameter.Value = typeof(TStoreImpl).FullName;
                        command.Parameters.Add(parameter);

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
                        {
                            if (ex.ErrorCode == SQLiteErrorCode.Constraint)
                            {
                                string message = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Store '{0}' already exists in the file '{1}'.",
                                    storeName,
                                    fileName);
                                throw new InvalidOperationException(message);
                            }

                            throw;
                        }
                    }

                    store.Initialize(this, storeName);
                    store.CreateStore(session);
                    transaction.Commit();
                }
            }

            return store;
        }

        private readonly string fileName;
        private readonly ITimeService timeService;

        private const string MetadataSql =
            @"CREATE TABLE METADATA (
[STORENAME] TEXT UNIQUE NOT NULL PRIMARY KEY,
[STOREIMPL] TEXT NOT NULL
);
CREATE UNIQUE INDEX [METADATA_STORENAME] ON [METADATA](
[STORENAME]  DESC
);";
    }
}