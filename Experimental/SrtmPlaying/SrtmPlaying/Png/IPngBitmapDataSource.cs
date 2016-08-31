using System;

namespace SrtmPlaying.Png
{
    [CLSCompliant (false)]
    public interface IPngBitmapDataSource : IDisposable
    {
        bool IsRaw { get; }
        int Width { get; } 
        int Height { get; }

        unsafe byte* GetRawScanline(int y);
        byte[] GetScanline(int y);
    }
}