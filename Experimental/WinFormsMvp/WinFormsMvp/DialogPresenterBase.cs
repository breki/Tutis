namespace WinFormsMvp
{
    public abstract class DialogPresenterBase<TView> : PresenterBase<TView>, IDialogPresenter
        where TView : IDialogView
    {
        public string Run()
        {
            return View.Run();
        }

        protected DialogPresenterBase(TView view) : base(view)
        {
        }
    }
}