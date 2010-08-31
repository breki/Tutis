using System;
using Castle.MicroKernel;

namespace WinFormsMvp
{
    public class DialogRunner : IDialogRunner
    {
        public DialogRunner(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public string Run<TDialog>() where TDialog : IDialogPresenter
        {
            using (TDialog dialogPresenter = kernel.Resolve<TDialog>())
            {
                return dialogPresenter.Run();
            }
        }

        public string Run(string dialogId)
        {
            throw new NotImplementedException();
        }

        private readonly IKernel kernel;
    }
}