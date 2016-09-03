namespace SrtmPlaying.Png.ChunkWriters.Idat
{
    public interface IPngFilter
    {
        byte[] Filter(IPngBitmapScanline scanline, PngImageType imageType, bool useAlpha, int destinationPixelSize, int clipWidth);
    }
}