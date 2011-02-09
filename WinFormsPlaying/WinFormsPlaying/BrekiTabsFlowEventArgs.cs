using System;

namespace WinFormsPlaying
{
    public class BrekiTabsFlowEventArgs : EventArgs
    {
        public BrekiTabsFlowEventArgs(int tabIndex)
        {
            this.tabIndex = tabIndex;
        }

        public int TabIndex
        {
            get { return tabIndex; }
        }

        private int tabIndex;
    }
}