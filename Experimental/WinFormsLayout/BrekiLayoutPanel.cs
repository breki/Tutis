using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace App
{
    public class BrekiLayoutPanel : Panel
    {
        public BrekiLayoutPanel()
        {
            Padding = new Padding(10);
            visualSystem.AddElement(rightBorder);
            visualSystem.AddElement(bottomBorder);
        }

        public ILayoutElement RightBorder
        {
            get { return rightBorder; }
        }

        public ILayoutElement BottomBorder
        {
            get { return bottomBorder; }
        }

        public void AddControl (Control control, ILayoutElement controlLayout)
        {
            Controls.Add(control);
            layouts.Add(control, controlLayout);
            visualSystem.AddElement(controlLayout);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            int right = Width - (Padding.Left + Padding.Right);
            if (right < 0)
                return;

            rightBorder.X = right;

            int bottom = Height - (Padding.Top + Padding.Bottom);
            if (bottom < 0)
                return;

            bottomBorder.Y = bottom;

            ExperimentalLayoutMechanism layoutMechanism = new ExperimentalLayoutMechanism();
            layoutMechanism.PerformLayout(visualSystem);

            foreach (KeyValuePair<Control, ILayoutElement> pair in layouts)
            {
                Control control = pair.Key;
                ILayoutElement layoutElement = pair.Value;
                control.Location = new Point(layoutElement.X + Padding.Left, layoutElement.Y + Padding.Top);
                control.Size = new Size(layoutElement.Width, layoutElement.Height);
            }
        }

        private VisualSystem visualSystem = new VisualSystem();
        private Dictionary<Control, ILayoutElement> layouts = new Dictionary<Control, ILayoutElement>();
        private SimpleLayoutElement rightBorder = new SimpleLayoutElement("rightBorder");
        private SimpleLayoutElement bottomBorder = new SimpleLayoutElement("bottomBorder");
    }
}