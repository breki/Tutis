using System;

namespace TreasureChest.Tests.SampleModule
{
    public class DisposableComponentA : IDisposable
    {
        public bool Disposed
        {
            get { return disposed; }
        }

        public void Dispose()
        {
            disposed = true;
        }

        private bool disposed;
    }
}