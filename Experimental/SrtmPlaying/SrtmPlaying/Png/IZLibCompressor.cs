using System.IO;

namespace SrtmPlaying.Png
{
    public interface IZLibCompressor
    {
        void Compress(byte[] originalData, Stream outputStream);
    }
}