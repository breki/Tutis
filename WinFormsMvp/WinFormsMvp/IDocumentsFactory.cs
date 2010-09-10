namespace WinFormsMvp
{
    public interface IDocumentsFactory
    {
        TPresenter ShowDocument<TPresenter>() where TPresenter : IDocumentPresenter;
    }
}