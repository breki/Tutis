using System;
using System.IO;
using Brejc.Rasters;
using LibroLib.FileSystem;
using SrtmPlaying.Png;

namespace SrtmPlaying.Srtm
{
    [CLSCompliant(false)]
    public class SrtmTilePngFileWriter : ISrtmTileFileWriter
    {
        public SrtmTilePngFileWriter(IFileSystem fileSystem, IPngWriter pngWriter)
        {
            this.fileSystem = fileSystem;
            this.pngWriter = pngWriter;
        }

        public void WriteToFile(string fileName, IRaster tileData)
        {
            fileSystem.EnsureDirectoryExists(Path.GetDirectoryName(fileName));

            using (Stream fileStream = fileSystem.OpenFileToWrite(fileName))
            {
                IPngBitmapDataSource rawBitmap = new Dem16RasterAsPngDataSource(tileData);
                PngWriterSettings pngWriterSettings = new PngWriterSettings();
                pngWriterSettings.ImageType = PngImageType.Grayscale16;
                pngWriterSettings.Transparency = PngTransparency.Opaque;
                pngWriter.WritePngPart(
                    rawBitmap,
                    0,
                    0,
                    tileData.RasterWidth,
                    tileData.RasterHeight,
                    pngWriterSettings,
                    fileStream);
            }
        }

        private readonly IFileSystem fileSystem;
        private readonly IPngWriter pngWriter;
    }
}