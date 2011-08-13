using System;

namespace TreasureChest
{
    public class Lease<T> : IDisposable
    {
        public Lease(ILeaseReturning returning, T instance)
        {
            this.returning = returning;
            this.instance = instance;
        }

        public T Instance
        {
            get { return instance; }
        }

        public static implicit operator T(Lease<T> lease)
        {
            return lease.instance;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources            
                    returning.Return(instance);
                }

                disposed = true;
            }
        }

        private bool disposed;
        private readonly ILeaseReturning returning;
        private readonly T instance;
    }
}