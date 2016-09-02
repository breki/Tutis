namespace SrtmPlaying.Png.ChunkWriters
{
    public interface IPngChunkWriter
    {
        byte[] WriteChunk(
            PngWriterSettings settings, 
            PngImageAnalysisInfo pngInfo, 
            IPngBitmapDataSource bitmap);
    }
}