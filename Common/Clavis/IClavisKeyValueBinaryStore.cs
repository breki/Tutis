using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Clavis
{
    public interface IClavisKeyValueBinaryStore : IClavisStore
    {
        void SetSerializer(ISerializer serializer);

        void Add(IClavisSession session, object key, object value);
        void Add(IClavisSession session, object key, object value, params object[] tagsKeysValues);
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        object Get(IClavisSession session, object key);

        /// <summary>
        /// Finds a first value which is tagged with the specified tag key and tag value.
        /// </summary>
        /// <param name="session">The Clavis session.</param>
        /// <param name="tagKey">The tag key.</param>
        /// <param name="tagValue">The tag value.</param>
        /// <returns>
        /// An object if found; otherwise <c>null</c>.
        /// </returns>
        /// <remarks>If it finds more than one matching object, the method returns the one with the oldest timestamp.</remarks>
        object FindFirst(IClavisSession session, object tagKey, object tagValue);
        
        //object FindFirst(IDictionary tags);
        IDictionary<string, object> List(IClavisSession session);
        [SuppressMessage ("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set")]
        void Set(IClavisSession session, object key, object value);
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set")]
        void Set(IClavisSession session, object key, object value, params object[] tagsKeysValues);
    }

    public class ClavisKeyValueBinaryStore : IClavisKeyValueBinaryStore
    {
        public void Initialize(IClavisFile clavisFile, string storeName)
        {
            this.clavisFile = clavisFile;
            this.storeName = storeName;
        }

        public void CreateStore(IClavisSession session)
        {
            using (SQLiteCommand command = session.Connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format(
                    CultureInfo.InvariantCulture,
                    SqlCreate,
                    storeName);
                command.ExecuteNonQuery();
            }
        }

        public void DestroyStore(IClavisSession session)
        {
            using (SQLiteCommand command = session.Connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format(
                    CultureInfo.InvariantCulture,
                    SqlDestroy,
                    storeName);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Could not delete store '{0}' in file '{1}'. The inner exception may contain more information about the reason.",
                        storeName, 
                        session.ClavisFile.FileName);
                    throw new InvalidOperationException(message, ex);
                }
            }
        }

        public void SetSerializer(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        public IClavisSession OpenSession()
        {
            return new ClavisSession(clavisFile);
        }

        public void Add(IClavisSession session, object key, object value)
        {
            Add(session, key, value, null);
        }

        public void Add(IClavisSession session, object key, object value, params object[] tagsKeysValues)
        {
            if (tagsKeysValues != null && tagsKeysValues.Length % 2 != 0)
                throw new ArgumentException("A tag is missing a value.");

            AssertSerializerIsSet();

            string tagsSerialized = SerializeTags(tagsKeysValues);

            using (SQLiteCommand command = session.Connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SqlInsert,
                        storeName);

                PrepareCommandParametersForWrite(session, command, key, value, tagsSerialized);

                try
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected != 1)
                        throw new InvalidOperationException("BUG");
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == SQLiteErrorCode.Constraint)
                    {
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            "Key '{0}' already exists in store '{1}' in file '{2}'.",
                            key,
                            storeName,
                            session.ClavisFile.FileName);
                        throw new InvalidOperationException(message, ex);
                    }

                    throw;
                }
            }
        }

        public object Get(IClavisSession session, object key)
        {
            AssertSerializerIsSet();

            using (SQLiteCommand command = session.Connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "SELECT VALUE, SERIALIZATIONINFO FROM [{0}] WHERE KEY=:key",
                        storeName);

                SQLiteParameter parameter;
                parameter = command.CreateParameter();
                parameter.ParameterName = "key";
                string keyToString = Convert.ToString(key, CultureInfo.InvariantCulture);
                parameter.Value = keyToString;
                command.Parameters.Add(parameter);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        byte[] data = (byte[])reader[0];
                        object dbSerializationInfo = reader[1];
                        string serializationInfo = dbSerializationInfo is DBNull ? null : (string)dbSerializationInfo;

                        return serializer.DeserializeFromByteArray(data, serializationInfo);
                    }
                }

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Key '{0}' does not exist in store '{1}' in file '{2}'.",
                    keyToString,
                    storeName,
                    session.ClavisFile.FileName);
                throw new KeyNotFoundException(message);
            }
        }

        public object FindFirst(IClavisSession session, object tagKey, object tagValue)
        {
            AssertSerializerIsSet();

            using (SQLiteCommand command = session.Connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        FindFirstSql,
                        storeName);

                SQLiteParameter parameter;
                parameter = command.CreateParameter();
                parameter.ParameterName = "tags";
                parameter.DbType = DbType.String;
                parameter.Value = ConstructTagsRegex(tagKey, tagValue);
                command.Parameters.Add(parameter);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        byte[] data = (byte[])reader[0];
                        object dbSerializationInfo = reader[1];
                        string serializationInfo = dbSerializationInfo is DBNull ? null : (string)dbSerializationInfo;

                        return serializer.DeserializeFromByteArray(data, serializationInfo);
                    }

                    return null;
                }
            }
        }

        //public object FindFirst(IDictionary tags)
        //{
        //    throw new NotImplementedException();
        //}

        public IDictionary<string, object> List(IClavisSession session)
        {
            using (SQLiteCommand command = session.Connection.CreateCommand())
            {
                Dictionary<string, object> rows = new Dictionary<string, object>();

                command.CommandType = CommandType.Text;
                command.CommandText =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "SELECT KEY, VALUE, SERIALIZATIONINFO FROM [{0}]",
                        storeName);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string key = (string)reader[0];
                        byte[] data = (byte[])reader[1];
                        object dbSerializationInfo = reader[2];
                        string serializationInfo = dbSerializationInfo is DBNull ? null : (string)dbSerializationInfo;

                        rows.Add(key, serializer.DeserializeFromByteArray(data, serializationInfo));
                    }
                }

                return rows;
            }
        }

        public void Set(IClavisSession session, object key, object value)
        {
            Set(session, key, value, null);
        }

        public void Set(IClavisSession session, object key, object value, params object[] tagsKeysValues)
        {
            if (tagsKeysValues != null && tagsKeysValues.Length % 2 != 0)
                throw new ArgumentException("A tag is missing a value.");

            AssertSerializerIsSet();

            string tagsSerialized = SerializeTags(tagsKeysValues);

            using (SQLiteCommand command = session.Connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        SqlInsertOrReplace,
                        storeName);

                PrepareCommandParametersForWrite(session, command, key, value, tagsSerialized);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected != 1)
                    throw new InvalidOperationException();
            }
        }

        private void AssertSerializerIsSet()
        {
            if (serializer == null)
                throw new InvalidOperationException("Binary serializer is not set. Use SetSerializer() method.");
        }

        private void PrepareCommandParametersForWrite(
            IClavisSession session,
            SQLiteCommand command, 
            object key, 
            object value,
            string tagValuesSerialized)
        {
            SQLiteParameter parameter;
            parameter = command.CreateParameter();
            parameter.ParameterName = "key";
            parameter.DbType = DbType.String;
            parameter.Value = Convert.ToString(key, CultureInfo.InvariantCulture);
            command.Parameters.Add(parameter);

            string serializationInfo;
            parameter = command.CreateParameter();
            parameter.ParameterName = "value";
            parameter.DbType = DbType.Binary;
            parameter.Value = serializer.SerializeIntoByteArray(value, out serializationInfo);
            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();
            parameter.ParameterName = "serializationInfo";
            parameter.DbType = DbType.String;
            parameter.Value = serializationInfo;
            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();
            parameter.ParameterName = "timestamp";
            parameter.DbType = DbType.DateTime;
            parameter.Value = session.TimeService.Now;
            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();
            parameter.ParameterName = "tags";
            parameter.DbType = DbType.String;
            parameter.Value = tagValuesSerialized;
            command.Parameters.Add(parameter);
        }

        private static string SerializeTags(object[] tagsKeysValues)
        {
            string serialized = null;
            if (tagsKeysValues != null && tagsKeysValues.Length > 0)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < tagsKeysValues.Length; i+=2)
                {
                    builder.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "||{0}|=|{1}||",
                        tagsKeysValues[i],
                        tagsKeysValues[i + 1]);
                }

                serialized = builder.ToString();
            }

            return serialized;
        }

        private static string ConstructTagsRegex(object key, object value)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                @"\|\|{0}\|\=\|{1}\|\|",
                key,
                value);
        }

        private const string SqlCreate =
            @"CREATE TABLE [{0}] (
[KEY] TEXT UNIQUE NOT NULL PRIMARY KEY,
[VALUE] BLOB NULL,
[SERIALIZATIONINFO] TEXT NULL,
[TIMESTAMP] INTEGER NOT NULL,
[TAGS] TEXT NULL
);
CREATE UNIQUE INDEX [{0}_KEY] ON [{0}] ([KEY] ASC);
CREATE INDEX [{0}_TIMESTAMP] ON [{0}] ([TIMESTAMP] ASC);
";
        private const string SqlDestroy =
            @"DROP INDEX IF EXISTS [{0}_TIMESTAMP];
DROP INDEX IF EXISTS [{0}_KEY];
DROP TABLE IF EXISTS [{0}];";
        private const string SqlInsert = @"INSERT INTO [{0}] (KEY, VALUE, SERIALIZATIONINFO, TIMESTAMP, TAGS) 
VALUES (:key, :value, :serializationInfo, :timestamp, :tags)";
        private const string SqlInsertOrReplace = @"INSERT OR REPLACE INTO [{0}] (KEY, VALUE, SERIALIZATIONINFO, TIMESTAMP, TAGS) 
VALUES (:key, :value, :serializationInfo, :timestamp, :tags)";
        private const string FindFirstSql = @"SELECT VALUE, SERIALIZATIONINFO 
FROM [{0}] 
WHERE TAGS REGEXP :tags
ORDER BY TIMESTAMP
LIMIT 1";

        private IClavisFile clavisFile;
        private string storeName;
        private ISerializer serializer = new DefaultBinarySerializer();
    }
}