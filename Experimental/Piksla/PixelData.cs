namespace Piksla
{
    public struct PixelData
    {
        public PixelData(byte alpha, byte blue, byte green, byte red)
        {
            this.alpha = alpha;
            this.blue = blue;
            this.green = green;
            this.red = red;
        }

        public byte blue;
        public byte green;
        public byte red;
        public byte alpha;
    }
}