namespace SrtmPlaying.Png
{
    public interface IPngChunkWriter
    {
        byte[] WriteChunk(
            PngWriterSettings settings, 
            PngImageAnalysisInfo pngInfo, 
            IPngBitmapDataSource bitmap);
    }
}