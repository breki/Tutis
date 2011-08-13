using System;

namespace WinFormsMvp
{
    public abstract class PresenterBase<TView> : IDisposable
        where TView : IDisposable
    {
        protected PresenterBase(TView view)
        {
            this.view = view;
        }

        protected TView View
        {
            get { return view; }
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
                    if (view != null)
                        view.Dispose();
                }

                disposed = true;
            }
        }

        private bool disposed;
        private readonly TView view;
    }
}