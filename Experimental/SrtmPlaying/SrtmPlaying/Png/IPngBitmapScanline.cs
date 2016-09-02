namespace SrtmPlaying.Png
{
    public interface IPngBitmapScanline
    {
        bool HasAlpha { get; }
        byte NextByte();
        unsafe byte NextDiff();
    }
}