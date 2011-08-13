using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

namespace Clavis
{
    public interface IClavisTransaction : IDisposable
    {
        void Commit();
    }

    public class ClavisTransaction : IClavisTransaction
    {
        public ClavisTransaction(ClavisSession session)
        {
            this.session = session;
            transaction = session.Connection.BeginTransaction();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            transaction.Commit();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources            
                }

                disposed = true;
            }
        }

        private bool disposed;
        [SuppressMessage ("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private readonly ClavisSession session;
        private SQLiteTransaction transaction;
    }
}