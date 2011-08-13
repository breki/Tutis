namespace WinFormsMvp
{
    public interface IDialogsRunner
    {
        string Run<TDialog>() where TDialog : IDialogPresenter;
        string Run(string dialogId);
    }
}