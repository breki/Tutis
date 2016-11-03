namespace HomeSpaceOrganizer
{
    public class Stuff
    {
        public Stuff(string name, int accessibility, int width, int height, int depth)
        {
            this.name = name;
            this.accessibility = accessibility;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public string Name
        {
            get { return name; }
        }

        public int Accessibility
        {
            get { return accessibility; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public int Volume { get { return width * height * depth; } }

        public bool CanFit(Place place)
        {
            return width <= place.Width && height <= place.Height && depth <= place.Depth;
        }

        private readonly string name;
        private readonly int accessibility;
        private readonly int width;
        private readonly int height;
        private readonly int depth;
    }
}