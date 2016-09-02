namespace SrtmPlaying.Png.ChunkWriters.Idat
{
    public interface IPngFilter
    {
        void Filter(
            IPngBitmapScanline scanline,
            PngImageType imageType,
            bool useAlpha,
            int clipWidth, 
            byte[] filtered);
    }
}