namespace SrtmPlaying.Png
{
    public sealed unsafe class RawReadOnlyBitmapScanline : IPngBitmapScanline
    {
        public RawReadOnlyBitmapScanline(byte* data, int pixelSize, bool hasAlpha)
        {
            this.data = data;
            this.pixelSize = pixelSize;
            this.hasAlpha = hasAlpha;
        }

        public bool HasAlpha { get { return hasAlpha; } }

        public byte NextByte()
        {
            return data[cursor++];
        }

        public byte NextDiff()
        {
            byte a = 0;
            if (cursor >= pixelSize)
                a = data[cursor - pixelSize];

            return (byte)(data[cursor++] - a);
        }

        private readonly byte* data;
        private readonly int pixelSize;
        private readonly bool hasAlpha;
        private int cursor;
    }
}