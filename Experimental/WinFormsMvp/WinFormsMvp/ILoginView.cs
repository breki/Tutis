using System;

namespace WinFormsMvp
{
    public interface ILoginView : IDialogView
    {
        event EventHandler LoginButtonClicked;

        string UserName { get; set; }
        string Password { get; set; }

        void Close();
    }
}