using System;

namespace WinFormsMvp
{
    public interface IMainAppView : IView
    {
        event EventHandler Shown;

        void Run();
    }
}