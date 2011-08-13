using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NewDock2
{
    public class BrekiTabPanelTabs : ILayoutAlgorithm
    {
        public BrekiTabPanelTabs(BrekiTabPanel panel)
        {
            this.panel = panel;
        }

        public int? ActiveTabIndex
        {
            get
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    BrekiTab tab = tabs[i];
                    if (tab.IsActive)
                        return i;
                }

                return null;
            }
        }

        public void AddTab (BrekiTab tab)
        {
            tabs.Add(tab);

            if (tabs.Count == 1)
                tab.IsActive = true;

            PerformLayout();
        }

        public LayoutSize GetPreferredSize()
        {
            if (!layoutPerformed)
                PerformLayout();

            return size;
        }

        public void OnPaint(PaintEventArgs e)
        {
            if (!layoutPerformed)
                PerformLayout();

            //using (Brush fontBrush = new SolidBrush(panel.ForeColor))
            //using (Pen tabBorderPen = new Pen(SystemColors.ControlDark))
            using (Font activeFont = new Font(panel.Font, FontStyle.Bold))
            //using (Brush activeTabBackgroundBrush = new SolidBrush(SystemColors.ControlLightLight))
            //using (Brush inactiveTabBackgroundBrush = new SolidBrush(SystemColors.ControlLight))
            {
                PaintingContext context = new PaintingContext();
                context.Set("g", e.Graphics);
                //context.Set("tabBorderPen", tabBorderPen);
                context.Set("f", panel.Font);
                context.Set("activeFont", activeFont);
                //context.Set("fb", fontBrush);
                //context.Set("inactiveTabBackgroundBrush", inactiveTabBackgroundBrush);
                //context.Set("activeTabBackgroundBrush", activeTabBackgroundBrush);

                TabRenderer.DrawTabPage(e.Graphics, new Rectangle(0, 0, (int) size.Width, (int) size.Height));

                foreach (BrekiTab tab in tabs)
                    tab.Paint(context);
            }            
        }

        public void RemoveTab (BrekiTab tab)
        {
            tabs.Remove(tab);
            PerformLayout();
        }

        public void PerformLayout()
        {
            using (Graphics graphics = panel.CreateGraphics())
            //using (Brush brush = new SolidBrush(panel.ForeColor))
            //using (Pen pen = new Pen(SystemColors.ControlDark))
            using (Font activeFont = new Font(panel.Font, FontStyle.Bold))
            {
                LayoutContext context = new LayoutContext();
                context.Set("g", graphics);
                context.Set("f", panel.Font);
                context.Set("activeFont", activeFont);

                float x = 0;
                float y = 0;
                float preferredHeight = 0;
                for (int i = 0; i < tabs.Count; i++)
                {
                    BrekiTab tab = tabs[i];
                    LayoutSize preferredSize = tab.GetPreferredSize(context);
                    preferredHeight = preferredSize.Height;
                    tab.Resize(preferredSize);
                    tab.Move(new LayoutPosition(x, y));

                    x += preferredSize.Width;
                }

                size = new LayoutSize(x, preferredHeight);
            }

            layoutPerformed = true;
        }

        public bool ActivateTab(int tabIndex)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                BrekiTab tab = tabs[i];
                if (tabIndex == i)
                {
                    if (tab.IsActive)
                        return false;

                    tab.IsActive = true;
                }
                else if (tab.IsActive)
                    tab.IsActive = false;
            }

            return true;
        }

        public int? FindTabOnPosition(Point point)
        {
            if (!layoutPerformed)
                PerformLayout();

            for (int i = 0; i < tabs.Count; i++)
            {
                BrekiTab tab = tabs[i];

                if (tab.IsInside(new LayoutPosition(point.X, point.Y)))
                    return i;
            }

            return null;
        }

        public bool MakeTabHot(int? tabIndex)
        {
            if (currentHotTabIndex == tabIndex)
                return false;

            if (currentHotTabIndex.HasValue)
                tabs[currentHotTabIndex.Value].IsHot = false;

            if (tabIndex.HasValue)
                tabs[tabIndex.Value].IsHot = true;

            currentHotTabIndex = tabIndex;

            return true;
        }

        public bool OnMouseMove(Point mousePosition)
        {
            if (!layoutPerformed)
                PerformLayout();

            return MakeTabHot(FindTabOnPosition(mousePosition));
        }

        private int? currentHotTabIndex;
        private bool layoutPerformed;
        private readonly BrekiTabPanel panel;
        private LayoutSize size;
        private List<BrekiTab> tabs = new List<BrekiTab>();
    }
}