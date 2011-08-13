using System;
using System.Drawing;

namespace NewDock2
{
    [Obsolete]
    public class GraphicsLayoutContext : ILayoutContext
    {
        public GraphicsLayoutContext(Graphics graphics)
        {
            this.graphics = graphics;
        }

        public Graphics Graphics
        {
            get { return graphics; }
        }

        private readonly Graphics graphics;
    }
}