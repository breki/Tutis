using System;
using System.Drawing;
using System.IO;
using LibroLib.FileSystem;

namespace SrtmPlaying.Png
{
    public interface IPngWriter
    {
        void WritePng(Bitmap bitmap, PngWriterSettings settings, Stream outputStream);
        void WritePng (Bitmap bitmap, PngWriterSettings settings, string fileName, IFileSystem fileSystem);

        void WritePngClip (
            IPngBitmapDataSource bitmap,
            int x,
            int y,
            int width,
            int height,
            PngWriterSettings settings,
            Stream outputStream);
    }
}