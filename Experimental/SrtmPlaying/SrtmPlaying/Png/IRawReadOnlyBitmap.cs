using System;

namespace Brejc.Imaging.Png
{
    [CLSCompliant (false)]
    public interface IRawReadOnlyBitmap : IDisposable
    {
        int Width { get; } 
        int Height { get; }

        unsafe byte* GetScanline(int y);
    }
}