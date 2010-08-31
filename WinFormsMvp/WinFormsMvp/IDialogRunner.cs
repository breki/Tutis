namespace WinFormsMvp
{
    public interface IDialogRunner
    {
        string Run<TDialog>() where TDialog : IDialogPresenter;
        string Run(string dialogId);
    }
}