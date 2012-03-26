namespace TreasureChest.Tests.SampleModule
{
    public class DisposableComponentA : IDisposableService
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