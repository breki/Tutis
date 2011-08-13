using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsPlaying
{
    public class BrekiTabsFlow : FlowLayoutPanel, ITabButtonsCollection
    {
        public BrekiTabsFlow()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(0);
        }

        public event EventHandler<BrekiTabsFlowEventArgs> TabActivated;
        public event EventHandler<BrekiTabsFlowEventArgs> TabCloseButtonClick;
        public event EventHandler<BrekiTabsFlowEventArgs> TabDeactivated;

        public void ActivateTab (int tabIndex)
        {
            TabButton newActiveTabButton = (TabButton) Controls[tabIndex];
            if (!ReferenceEquals(newActiveTabButton, activeTabButton))
            {
                if (activeTabButton != null)
                {
                    int oldActiveIndex = FindControlIndex(activeTabButton);
                    if (TabDeactivated != null)
                        TabDeactivated(this, new BrekiTabsFlowEventArgs(oldActiveIndex));
                }

                activeTabButton = newActiveTabButton;
                if (TabActivated != null)
                    TabActivated(this, new BrekiTabsFlowEventArgs(tabIndex));
                
                Invalidate(true);
            }
        }

        public void AddTab (string tabText, object tag, bool canBeClosed)
        {
            TabButton button = new TabButton(this);
            button.Text = tabText;
            button.CanClose = canBeClosed;
            button.Tag = tag;
            button.Click += OnTabButtonClick;
            button.CloseButtonClick += new EventHandler(OnTabCloseButtonClicked);
            Controls.Add(button);
            ActivateTab(0);
        }

        public TabButton GetTab (int tabIndex)
        {
            return (TabButton)Controls[tabIndex];
        }

        public void RemoveTab (int tabIndex)
        {
            TabButton tabButton = (TabButton)Controls[tabIndex];

            bool wasActive = false;
            int activeTabIndex = 0;

            if (ReferenceEquals(tabButton, activeTabButton))
            {
                wasActive = true;
                activeTabIndex = FindControlIndex(activeTabButton);
                if (TabDeactivated != null)
                    TabDeactivated(this, new BrekiTabsFlowEventArgs(activeTabIndex));
                activeTabButton = null;
            }

            Controls.RemoveAt(tabIndex);
            tabButton.Dispose();

            if (wasActive)
            {
                if (activeTabIndex >= Controls.Count)
                    activeTabIndex--;
                if (activeTabIndex >= 0)
                    ActivateTab(activeTabIndex);
            }
        }

        public bool IsTabButtonActive(TabButton button)
        {
            return ReferenceEquals(button, activeTabButton);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //TabRenderer.DrawTabPage(e.Graphics, new Rectangle(0, 0, Size.Width, Size.Height));
        }

        private void OnTabButtonClick(object sender, EventArgs e)
        {
            int tabIndex = FindControlIndex(sender);
            ActivateTab(tabIndex);
        }

        private void OnTabCloseButtonClicked(object sender, EventArgs e)
        {
            if (TabCloseButtonClick != null)
            {
                int tabIndex = FindControlIndex(sender);
                TabCloseButtonClick(this, new BrekiTabsFlowEventArgs(tabIndex));
            }
        }

        private int FindControlIndex(object sender)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (sender == Controls[i])
                    return i;
            }

            throw new KeyNotFoundException();
        }

        private TabButton activeTabButton;
    }
}