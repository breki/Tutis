using Castle.MicroKernel;

namespace WinFormsMvp
{
    public class DocumentsFactory : IDocumentsFactory
    {
        public DocumentsFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public TPresenter ShowDocument<TPresenter>() where TPresenter : IDocumentPresenter
        {
            TPresenter presenter = kernel.Resolve<TPresenter>();
            presenter.Open();
            return presenter;
        }

        private readonly IKernel kernel;
    }
}