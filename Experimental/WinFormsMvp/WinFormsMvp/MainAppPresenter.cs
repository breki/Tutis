using System;

namespace WinFormsMvp
{
    public class MainAppPresenter : PresenterBase<IMainAppView>
    {
        public MainAppPresenter(
            IMainAppView view, 
            IDialogsRunner dialogsRunner,
            IDocumentsFactory documentsFactory) : base(view)
        {
            this.dialogsRunner = dialogsRunner;
            this.documentsFactory = documentsFactory;
            view.Shown += OnShown;
        }

        public void Run()
        {
            View.Run();
        }

        private void OnShown(object sender, EventArgs e)
        {
            string result = dialogsRunner.Run<LoginPresenter>();
            if ("OK" == result)
            {
                documentsFactory.ShowDocument<MyDocumentPresenter>();
            }
        }

        private readonly IDialogsRunner dialogsRunner;
        private readonly IDocumentsFactory documentsFactory;
    }
}