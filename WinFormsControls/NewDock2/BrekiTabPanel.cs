using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewDock2
{
    public class BrekiTabPanel : Panel
    {
        public BrekiTabPanel()
        {
            base.AutoSize = true;
            tabs = new BrekiTabPanelTabs(this);
        }

        public int? SelectedTabIndex
        {
            get { return tabs.ActiveTabIndex; }
            set
            {
                if (tabs.ActivateTab(value.Value))
                    Refresh();
            }
        }

        public void AddTab(string tabText)
        {
            tabs.AddTab(new BrekiTab(tabText));
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            LayoutSize preferredSize = tabs.GetPreferredSize();
            return new Size(
                proposedSize.Width,
                (int)preferredSize.Height);
                //Font.Height + (vertTextPadding + borderWidth)*2 + topPanelPadding);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            tabs.OnPaint(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int? tabIndex = tabs.FindTabOnPosition(e.Location);

            if (tabIndex == null)
                return;

            if (tabIndex != tabs.ActiveTabIndex)
            {
                tabs.ActivateTab(tabIndex.Value);
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (tabs.OnMouseMove(e.Location))
                Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (tabs.MakeTabHot(null))
                Refresh();
        }

        private BrekiTabPanelTabs tabs;
    }
}