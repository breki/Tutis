using System;
using System.IO;
using System.IO.Compression;

namespace SrtmPlaying.Png
{
    // As it is now implemented, this does not work since DeflateStream is not enough
    // to fully support ZLib.
    // http://stackoverflow.com/a/6283224/55408
    // http://www.libpng.org/pub/png/spec/1.2/PNG-Compression.html
    [Obsolete("Not working")]
    public class ZLibCompressorUsingDotNetDeflate : IZLibCompressor
    {
        public void Compress(byte[] originalData, Stream outputStream)
        {
            CompressionLevel compressionLevel = CompressionLevel.Optimal;

            using (DeflateStream deflateStream = new DeflateStream(
                outputStream, compressionLevel, false))
            {
                deflateStream.Write(originalData, 0, originalData.Length);
                deflateStream.Flush();
            }
        }
    }
}