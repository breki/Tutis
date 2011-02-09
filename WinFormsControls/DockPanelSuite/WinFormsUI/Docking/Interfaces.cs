using System;

namespace WeifenLuo.WinFormsUI.Docking
{
    public interface IDockContent : IDisposable
    {
        DockContentHandler DockHandler { get; }
        void OnActivated(EventArgs e);
        void OnDeactivate(EventArgs e);
    }
}
