namespace SrtmPlaying.Png
{
    public sealed unsafe class RawReadOnlyBitmapScanline : IPngBitmapScanline
    {
        public RawReadOnlyBitmapScanline(byte* data, bool hasAlpha)
        {
            this.data = data;
            this.hasAlpha = hasAlpha;
        }

        public bool HasAlpha { get { return hasAlpha; } }

        public byte NextByte()
        {
            return data[cursor++];
        }

        private readonly byte* data;
        private readonly bool hasAlpha;
        private int cursor;
    }
}