using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace NewDock2
{
    public class BrekiTab : ILayoutBox
    {
        public BrekiTab(string tabText)
        {
            this.tabText = tabText;
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public bool IsHot
        {
            get { return isHot; }
            set { isHot = value; }
        }

        public LayoutPosition Position
        {
            get { return position; }
        }

        public LayoutSize Size
        {
            get { return size; }
        }

        public string TabText
        {
            get { return tabText; }
            set { tabText = value; }
        }

        public LayoutSize GetPreferredSize(ILayoutContext context)
        {
            Graphics graphics = context.Get<Graphics>("g");
            Font baseFont = context.Get<Font>("f");
            Font activeFont = context.Get<Font>("activeFont");

            SizeF textSize = graphics.MeasureString(TabText, isActive ? activeFont : baseFont);

            return new LayoutSize(textSize.Width + HorizPadding*2, textSize.Height + VertPadding*2);
        }

        public bool IsInside(LayoutPosition point)
        {
            return point.X >= position.X && point.X < position.X + size.Width
                   && point.Y >= position.Y && point.Y < position.Y + size.Height;
        }

        public void Move(LayoutPosition newPosition)
        {
            position = newPosition;
        }

        public void Paint(IPaintingContext context)
        {
            Graphics graphics = context.Get<Graphics>("g");
            //Pen tabBorderPen = context.Get<Pen>("tabBorderPen");
            Font baseFont = context.Get<Font>("f");
            //Font activeFont = context.Get<Font>("activeFont");
            //Brush fontBrush = context.Get<Brush>("fb");
            //Brush activeTabBackgroundBrush = context.Get<Brush>("activeTabBackgroundBrush");
            //Brush inactiveTabBackgroundBrush = context.Get<Brush>("inactiveTabBackgroundBrush");

            TabRenderer.DrawTabItem(
                graphics,
                new Rectangle((int)position.X,
                    (int)position.Y,
                    (int)size.Width,
                    (int)size.Height),
                tabText,
                baseFont,
                false,
                isActive ? TabItemState.Selected : (isHot ? TabItemState.Hot : TabItemState.Normal));

            //graphics.FillRectangle(
            //    isActive ? activeTabBackgroundBrush : inactiveTabBackgroundBrush,
            //    position.X,
            //    position.Y,
            //    size.Width,
            //    size.Height);

            //graphics.DrawRectangle(
            //    tabBorderPen,
            //    position.X,
            //    position.Y,
            //    size.Width,
            //    size.Height);

            //graphics.DrawString(
            //    tabText, 
            //    isActive ? activeFont : baseFont, 
            //    fontBrush, 
            //    position.X + HorizPadding, 
            //    position.Y + VertPadding);
        }

        public void Resize(LayoutSize newSize)
        {
            size = newSize;
        }

        private bool isActive;
        private bool isHot;
        private LayoutPosition position;
        private LayoutSize size;
        private string tabText;
        private const int HorizPadding = 10;
        private const int VertPadding = 2;
    }
}