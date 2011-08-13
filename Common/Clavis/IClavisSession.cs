using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

namespace Clavis
{
    public interface IClavisSession : IDisposable
    {
        IClavisFile ClavisFile { get; }
        SQLiteConnection Connection { get; }
        ITimeService TimeService { get; }

        IClavisTransaction BeginTransaction();
    }

    public class ClavisSession : IClavisSession
    {
        public ClavisSession(IClavisFile clavisFile)
        {
            this.clavisFile = clavisFile;
            timeService = clavisFile.TimeService;
            connection = OpenConnection();
        }

        public SQLiteConnection Connection
        {
            get { return connection; }
        }

        public IClavisFile ClavisFile
        {
            get { return clavisFile; }
        }

        public ITimeService TimeService
        {
            get { return timeService; }
        }

        public IClavisTransaction BeginTransaction()
        {
            return new ClavisTransaction(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected SQLiteConnection OpenConnection()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = clavisFile.FileName;

            SQLiteConnection conn = new SQLiteConnection(builder.ToString());
            conn.Open();
            return conn;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources            
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }

                disposed = true;
            }
        }

        private bool disposed;
        [SuppressMessage ("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private readonly IClavisFile clavisFile;

        private readonly ITimeService timeService;
        private SQLiteConnection connection;
    }
}