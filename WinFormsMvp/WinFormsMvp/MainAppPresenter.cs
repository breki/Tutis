using System;

namespace WinFormsMvp
{
    public class MainAppPresenter : PresenterBase<IMainAppView>
    {
        public MainAppPresenter(IMainAppView view, IDialogRunner dialogRunner) : base(view)
        {
            this.dialogRunner = dialogRunner;
            view.Shown += OnShown;
        }

        public void Run()
        {
            View.Run();
        }

        private void OnShown(object sender, EventArgs e)
        {
            dialogRunner.Run<LoginPresenter>();
            dialogRunner.Run<LoginPresenter>();
        }

        private readonly IDialogRunner dialogRunner;
    }
}