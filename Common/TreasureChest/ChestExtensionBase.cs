using System;
using TreasureChest.Policies;

namespace TreasureChest
{
    public abstract class ChestExtensionBase : GlobalChestPolicyBase, IChestExtension
    {
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
                    DisposeManagedResources();
                }

                disposed = true;
            }
        }

        protected virtual void DisposeManagedResources()
        {
        }

        private bool disposed;
    }
}