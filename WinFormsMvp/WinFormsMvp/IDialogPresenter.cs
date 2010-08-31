using System;

namespace WinFormsMvp
{
    public interface IDialogPresenter : IDisposable
    {
        string Run();
    }
}