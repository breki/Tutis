using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace WinFormsPlaying
{
    public class TabButton : Button
    {
        public TabButton(ITabButtonsCollection tabButtonsCollection)
        {
            this.tabButtonsCollection = tabButtonsCollection;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TextAlign = ContentAlignment.MiddleLeft;
            Margin = new Padding(0);
            Padding = new Padding(7, 0, 7, 0);
            UseCompatibleTextRendering = true;

            closeButton = new CloseButton();
            closeButton.Anchor = AnchorStyles.Right;
            //closeButton.AutoSize = true;
            //closeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            closeButton.Dock = DockStyle.None;
            closeButton.AutoSize = false;
            closeButton.Width = 16;
            closeButton.Height = 16;
            closeButton.Location = new Point(
                Width - closeButton.Width - Padding.Right,
                (Height - closeButton.Height) / 2);
            closeButton.Click += OnCloseButtonClicked;
            Controls.Add(closeButton);
        }

        public event EventHandler CloseButtonClick;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool CanClose
        {
            get { return closeButton.Visible; }
            set { closeButton.Visible = value; }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);

            preferredSize = new Size(
                preferredSize.Width, 
                TextRenderer.MeasureText("Mj", Font).Height + 4 * 2);

            if (CanClose)
                preferredSize = new Size(preferredSize.Width + Padding.Left * 2, preferredSize.Height);

            return preferredSize;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            bool isActive = tabButtonsCollection != null && tabButtonsCollection.IsTabButtonActive(this);

            TabRenderer.DrawTabItem(
                pevent.Graphics,
                new Rectangle(0, 0, Size.Width, Size.Height),
                Text,
                Font,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                false,
                isActive ? TabItemState.Selected : (isMouseOver ? TabItemState.Hot : TabItemState.Normal));
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
            isMouseOver = true;
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            isMouseOver = false;
        }

        private void OnCloseButtonClicked(object sender, EventArgs e)
        {
            if (CloseButtonClick != null)
                CloseButtonClick(this, EventArgs.Empty);
        }

        private CloseButton closeButton;
        private bool isMouseOver;
        private readonly ITabButtonsCollection tabButtonsCollection;
    }
}