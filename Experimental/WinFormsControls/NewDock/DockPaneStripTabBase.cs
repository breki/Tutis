using System;
using System.Windows.Forms;

namespace NewDock
{
    public class DockPaneStripTabBase : IDisposable
    {
        public DockPaneStripTabBase(IDockContent content)
        {
            this.content = content;
        }

        public IDockContent Content
        {
            get { return content; }
        }

        public Form ContentForm
        {
            get { return content as Form; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        private IDockContent content;
    }
}