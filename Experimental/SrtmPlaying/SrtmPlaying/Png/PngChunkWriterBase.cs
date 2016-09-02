using System.IO;

namespace SrtmPlaying.Png
{
    public abstract class PngChunkWriterBase
    {
        protected static void WriteChunkType(Stream chunkStream, string chunkType)
        {
            for (int i = 0; i < 4; i++)
                chunkStream.WriteByte((byte)chunkType[i]);
        }
    }
}