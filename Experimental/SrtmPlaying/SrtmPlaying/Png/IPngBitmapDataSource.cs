using System;

namespace SrtmPlaying.Png
{
    public interface IPngBitmapDataSource : IDisposable
    {
        bool IsRaw { get; }
        int Width { get; } 
        int Height { get; }
        int PixelSize { get; }

        unsafe byte* GetRawScanline(int y);
        byte[] GetScanline(int y);
    }
}