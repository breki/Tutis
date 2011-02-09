namespace NewDock2
{
    public struct LayoutSize
    {
        public LayoutSize(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        private float width;
        private float height;
    }
}