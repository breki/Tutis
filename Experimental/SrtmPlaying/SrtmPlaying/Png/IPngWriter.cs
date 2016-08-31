using System;
using System.Drawing;
using System.IO;
using LibroLib.FileSystem;

namespace Brejc.Imaging.Png
{
    [CLSCompliant (false)]
    public interface IPngWriter
    {
        void WritePng(Bitmap bitmap, PngWriterSettings settings, Stream outputStream);
        void WritePng (Bitmap bitmap, PngWriterSettings settings, string fileName, IFileSystem fileSystem);

        [CLSCompliant(false)]
        void WritePngPart (
            IRawReadOnlyBitmap bitmap,
            int x,
            int y,
            int width,
            int height,
            PngWriterSettings settings,
            Stream outputStream);
    }
}