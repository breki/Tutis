using System;

namespace SrtmPlaying.Png
{
    [CLSCompliant (false)]
    public interface IRawReadOnlyBitmap : IDisposable
    {
        int Width { get; } 
        int Height { get; }

        unsafe byte* GetScanline(int y);
    }
}