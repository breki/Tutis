using System;

namespace SrtmPlaying.Png
{
    public interface IPngBitmapDataSource : IDisposable
    {
        int Width { get; } 
        int Height { get; }
        int PixelSize { get; }

        IPngBitmapScanline GetScanline(int y);
    }
}