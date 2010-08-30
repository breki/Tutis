using System;

namespace WinFormsMvp
{
    public interface ILoginView
    {
        event EventHandler LoginButtonClicked;

        string UserName { get; set; }
        string Password { get; set; }

        void Run ();
        void Close();
    }
}