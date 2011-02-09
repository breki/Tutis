namespace NewDock2
{
    public struct LayoutPosition
    {
        public LayoutPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        private float x;
        private float y;
    }
}