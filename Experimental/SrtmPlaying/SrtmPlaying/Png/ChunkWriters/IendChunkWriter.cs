using System.IO;

namespace SrtmPlaying.Png.ChunkWriters
{
    public class IendChunkWriter : PngChunkWriterBase, IPngChunkWriter
    {
        public byte[] WriteChunk(PngWriterSettings settings, PngImageAnalysisInfo pngInfo,
            IPngBitmapDataSource bitmap)
        {
            using (MemoryStream chunkStream = new MemoryStream())
            {
                WriteChunkType(chunkStream, "IEND");
                return chunkStream.ToArray();
            }
        }
    }
}