namespace WinFormsMvp
{
    public class MyDocumentPresenter : DocumentPresenterBase<IMyDocumentView>
    {
        public MyDocumentPresenter(IMyDocumentView view) : base(view)
        {
        }
    }
}