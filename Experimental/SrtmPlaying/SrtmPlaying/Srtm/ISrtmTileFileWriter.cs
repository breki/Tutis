using Brejc.Rasters;

namespace SrtmPlaying.Srtm
{
    public interface ISrtmTileFileWriter
    {
        void WriteToFile(string fileName, IRaster tileData);
    }
}