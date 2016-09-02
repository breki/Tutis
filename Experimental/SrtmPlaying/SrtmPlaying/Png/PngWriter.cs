using System;
using System.Drawing;
using System.IO;
using System.Text;
using LibroLib;
using LibroLib.FileSystem;
using SrtmPlaying.BinaryProcessing;

namespace SrtmPlaying.Png
{
    public class PngWriter : IPngWriter
    {
        public void WritePng (Bitmap bitmap, PngWriterSettings settings, Stream outputStream)
        {
            using (RawReadOnlyBitmap raw = new RawReadOnlyBitmap(bitmap))
                WritePngClip(raw, 0, 0, bitmap.Width, bitmap.Height, settings, outputStream);
        }

        public void WritePng (Bitmap bitmap, PngWriterSettings settings, string fileName, IFileSystem fileSystem)
        {
            fileSystem.EnsureDirectoryExists(Path.GetDirectoryName(fileName));

            using (Stream stream = fileSystem.OpenFileToWrite (fileName))
                WritePng (bitmap, settings, stream);
        }

        public void WritePngClip (
            IPngBitmapDataSource bitmap,
            int clipX,
            int clipY,
            int clipWidth,
            int clipHeight,
            PngWriterSettings settings,
            Stream outputStream)
        {
            PngImageAnalysisInfo pngInfo = AnalyzeImage (bitmap, settings, clipX, clipY, clipWidth, clipHeight);

            using (BinaryWriter writer = new BinaryWriter (outputStream))
            {
                WriteSignature (writer);
                WriteChunk(ihdrChunkWriter, settings, pngInfo, bitmap, outputStream);
                WriteChunk(idatChunkWriter, settings, pngInfo, bitmap, outputStream);
                WriteChunk(iendChunkWriter, settings, pngInfo, bitmap, outputStream);
            }
        }

        private static PngImageAnalysisInfo AnalyzeImage(
            IPngBitmapDataSource bitmapDataSource, 
            PngWriterSettings settings,
            int clipX,
            int clipY,
            int clipWidth,
            int clipHeight)
        {
            PngImageAnalysisInfo pngInfo = new PngImageAnalysisInfo(
                bitmapDataSource.Width,
                bitmapDataSource.Height,
                clipX,
                clipY,
                clipWidth,
                clipHeight);

            switch (settings.Transparency)
            {
                case PngTransparency.Opaque:
                    pngInfo.IsTransparencyUsed = false;
                    break;
                case PngTransparency.Transparent:
                    pngInfo.IsTransparencyUsed = true;
                    break;
            }

            switch (settings.ImageType)
            {
                case PngImageType.Grayscale16:
                    pngInfo.PixelSize = 2;
                    break;
                case PngImageType.Rgb8:
                    pngInfo.PixelSize = pngInfo.IsTransparencyUsed ? 4 : 3;
                    break;
                default:
                    throw new NotSupportedException(
                        "This PNG image type ({0}) is currently not supported.".Fmt(settings.ImageType));
            }

            if (settings.Transparency != PngTransparency.AutoDetect)
            {
                // nothing more needed here
                return pngInfo;
            }

            switch (settings.ImageType)
            {
                case PngImageType.Grayscale16:
                    break;
                case PngImageType.Rgb8:
                    break;
                default:
                    throw new InvalidOperationException(
                        "PngTransparency.AutoDetect is not supported for PNG image type '{0}'.".Fmt(settings.ImageType));
            }

            int bitmapWidth = bitmapDataSource.Width;
            int bitmapHeight = bitmapDataSource.Height;
            int maxx = Math.Min(bitmapWidth, clipX + clipWidth);
            int maxy = Math.Min(bitmapHeight, clipY + clipHeight);

            unsafe
            {
                for (int yy = clipX; yy < maxy; yy++)
                {
                    // no additional information is needed, so we can skip the rest of the bitmap
                    if (pngInfo.IsMoreThan256Colors && pngInfo.IsTransparencyUsed)
                        break;

                    if (bitmapDataSource.IsRaw)
                    {
                        byte* scanline = bitmapDataSource.GetRawScanline(yy);

                        int i = clipX*4;
                        for (int xx = clipX; xx < maxx; xx++)
                        {
                            byte blue = scanline[i++];
                            byte green = scanline[i++];
                            byte red = scanline[i++];
                            byte alpha = scanline[i++];

                            if (alpha < 255)
                                pngInfo.IsTransparencyUsed = true;

                            if (!pngInfo.IsMoreThan256Colors)
                                pngInfo.AddUsedColor(alpha << 24 | red << 16 | green << 8 | blue);
                        }
                    }
                    else
                    {
                        byte[] scanline = bitmapDataSource.GetScanline(yy);

                        int i = clipX * 4;
                        for (int xx = clipX; xx < maxx; xx++)
                        {
                            byte blue = scanline[i++];
                            byte green = scanline[i++];
                            byte red = scanline[i++];
                            byte alpha = scanline[i++];

                            if (alpha < 255)
                                pngInfo.IsTransparencyUsed = true;

                            if (!pngInfo.IsMoreThan256Colors)
                                pngInfo.AddUsedColor(alpha << 24 | red << 16 | green << 8 | blue);
                        }
                    }
                }
            }

            return pngInfo;
        }

        private static void WriteSignature (BinaryWriter writer)
        {
            writer.Write (signature);
        }

        private static void WriteChunk(
            IPngChunkWriter chunkWriter, 
            PngWriterSettings settings, 
            PngImageAnalysisInfo pngInfo,
            IPngBitmapDataSource bitmap,
            Stream imageStream)
        {
            byte[] chunkData = chunkWriter.WriteChunk(settings, pngInfo, bitmap);
            WriteChunkDataToStream(chunkData, imageStream);
        }

        private static void WriteChunkDataToStream (byte[] chunkData, Stream imageStream)
        {
            using (BinaryWriter writer = new BinaryWriter(imageStream, Encoding.ASCII, true))
            {
                writer.WriteBigEndian(chunkData.Length - 4);
                writer.Write(chunkData);
                writer.WriteBigEndian(CrcCalculator.CalculateCrc(chunkData));
            }
        }

        private readonly IPngChunkWriter ihdrChunkWriter = new IhdrChunkWriter();
        private readonly IPngChunkWriter idatChunkWriter = new IdatChunkWriter(new ZLibCompressorUsingSharpZipLib());
        private readonly IPngChunkWriter iendChunkWriter = new IendChunkWriter();
        private static readonly byte[] signature = { 137, 80, 78, 71, 13, 10, 26, 10 };
    }
}