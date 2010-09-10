namespace WinFormsMvp
{
    public abstract class DocumentPresenterBase<TView> : PresenterBase<TView>, IDocumentPresenter
        where TView : IDocumentView
    {
        public void Open()
        {
            View.Open();
        }

        protected DocumentPresenterBase(TView view) : base(view)
        {
        }
    }
}