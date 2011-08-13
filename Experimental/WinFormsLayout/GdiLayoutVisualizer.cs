using System.Drawing;

namespace App
{
    public class GdiLayoutVisualizer : ILayoutVisualizer
    {
        public GdiLayoutVisualizer(Graphics graphics, float scale, int padding)
        {
            this.graphics = graphics;
            this.scale = scale;
            this.padding = padding;
        }

        public int Padding
        {
            get { return padding; }
        }

        public int StartingY
        {
            get { return startingY; }
            set { startingY = value; }
        }

        public void Visualize(IVisualSystem visualSystem)
        {
            using (Pen pen = new Pen(Color.Red, 2))
            using (Brush brush = new SolidBrush(Color.FromArgb(0x20, Color.Blue)))
            using (Font font = new Font("Segoe UI", 12))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                if (clearGraphics)
                {
                    graphics.Clear(Color.Wheat);
                    clearGraphics = false;
                }

                foreach (ILayoutElement element in visualSystem.Elements)
                {
                    Point loc = new Point(
                        (int)CalcCoord(element.X),
                        (int)CalcCoord(element.Y) + startingY);
                    Point loc2 = new Point(
                        (int)CalcCoord(element.X + element.Width),
                        (int)CalcCoord(element.Y + element.Height) + startingY);

                    Rectangle rect = new Rectangle(
                        loc.X,
                        loc.Y,
                        loc2.X - loc.X + 1,
                        loc2.Y - loc.Y + 1);

                    if (element.Visible)
                    {
                        graphics.DrawRectangle(pen, rect);
                        graphics.FillRectangle(brush, rect);
                        graphics.DrawString(
                            element.Name[0].ToString().ToUpper(), 
                            font, 
                            Brushes.Black, 
                            rect.X + rect.Width/2, 
                            rect.Y + rect.Height/2, 
                            format);
                    }
                    else
                        graphics.FillRectangle(brush, rect);
                }
            }
        }

        private float CalcCoord(int value)
        {
            return (value + padding)*scale;
        }

        private bool clearGraphics = true;
        private Graphics graphics;
        private readonly float scale;
        private readonly int padding;
        private int startingY;
    }
}