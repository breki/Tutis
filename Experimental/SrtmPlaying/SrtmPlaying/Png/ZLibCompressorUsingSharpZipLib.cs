using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace SrtmPlaying.Png
{
    public class ZLibCompressorUsingSharpZipLib : IZLibCompressor
    {
        public void Compress(byte[] originalData, Stream outputStream)
        {
            Deflater deflater = new Deflater(5);
            //deflater.SetLevel(8);
            //deflater.SetStrategy(DeflateStrategy.Filtered);
            deflater.SetInput(originalData);
            deflater.Finish();

            byte[] outputBuffer = new byte[100 * 1024 * 4];
            while (deflater.IsNeedingInput == false)
            {
                int read = deflater.Deflate(outputBuffer);
                outputStream.Write(outputBuffer, 0, read);
            }
        }
    }
}