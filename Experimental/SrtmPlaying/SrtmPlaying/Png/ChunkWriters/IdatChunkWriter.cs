using System;
using System.IO;
using System.Text;

namespace SrtmPlaying.Png.ChunkWriters
{
    public class IdatChunkWriter : PngChunkWriterBase, IPngChunkWriter
    {
        public IdatChunkWriter(IZLibCompressor zLibCompressor)
        {
            this.zLibCompressor = zLibCompressor;
        }

        public byte[] WriteChunk(
            PngWriterSettings settings,
            PngImageAnalysisInfo pngInfo,
            IPngBitmapDataSource bitmap)
        {
            //int uncompressedDataSize = (1 + pngInfo.ImageWidth * pngInfo.PixelSize)
            //    * pngInfo.ImageHeight;

            MemoryStream filteredStream = FilterImageData(settings, pngInfo, bitmap);
            byte[] finalChunkData = CompressImageDataChunk(filteredStream, settings);

            using (MemoryStream chunkStream = new MemoryStream())
            {
                WriteChunkType(chunkStream, "IDAT");
                using (BinaryWriter writer = new BinaryWriter(chunkStream, Encoding.ASCII, true))
                    writer.Write(finalChunkData);

                return chunkStream.ToArray();
            }
        }

        private static MemoryStream FilterImageData(
            PngWriterSettings settings,
            PngImageAnalysisInfo pngInfo,
            IPngBitmapDataSource bitmap)
        {
            MemoryStream filteredStream = new MemoryStream();

            int bitmapHeight = bitmap.Height;
            int maxy = Math.Min(bitmapHeight, pngInfo.ClipY + pngInfo.ClipHeight);

            for (int yy = pngInfo.ClipY; yy < maxy; yy++)
            {
                filteredStream.WriteByte((byte)pngInfo.FilterType);

                switch (pngInfo.FilterType)
                {
                    case PngFilterType.None:
                        FilterMethodNone(settings, pngInfo, bitmap, yy, filteredStream);
                        break;
                    case PngFilterType.Sub:
                        FilterMethodSub(settings, pngInfo, bitmap, yy, filteredStream);
                        break;
                    case PngFilterType.Up:
                        break;
                    case PngFilterType.Average:
                        break;
                    case PngFilterType.Paeth:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return filteredStream;
        }

        private static unsafe void FilterMethodNone(
            PngWriterSettings settings,
            PngImageAnalysisInfo pngInfo,
            IPngBitmapDataSource bitmapDataSource,
            int lineY,
            Stream filteredStream)
        {
            bool alphaChannelUsed = pngInfo.IsTransparencyUsed;
            int si = pngInfo.ClipX * pngInfo.SourcePixelSize;

            if (bitmapDataSource.IsRaw)
            {
                byte* scanline = bitmapDataSource.GetRawScanline(lineY);
                for (int xx = 0; xx < pngInfo.ClipWidth; xx++)
                {
                    switch (settings.ImageType)
                    {
                        case PngImageType.Rgb8:
                            byte blue = scanline[si++];
                            byte green = scanline[si++];
                            byte red = scanline[si++];

                            filteredStream.WriteByte(red);
                            filteredStream.WriteByte(green);
                            filteredStream.WriteByte(blue);

                            if (alphaChannelUsed)
                            {
                                byte alpha = scanline[si++];
                                filteredStream.WriteByte(alpha);
                            }
                            else
                                si++;

                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            else
            {
                byte[] scanline = bitmapDataSource.GetScanline(lineY);
                for (int xx = 0; xx < pngInfo.ClipWidth; xx++)
                {
                    switch (settings.ImageType)
                    {
                        case PngImageType.Rgb8:
                            byte blue = scanline[si++];
                            byte green = scanline[si++];
                            byte red = scanline[si++];

                            filteredStream.WriteByte(red);
                            filteredStream.WriteByte(green);
                            filteredStream.WriteByte(blue);

                            if (alphaChannelUsed)
                            {
                                byte alpha = scanline[si++];
                                filteredStream.WriteByte(alpha);
                            }
                            else
                                si++;

                            break;
                        case PngImageType.Grayscale16:
                            byte hi = scanline[si++];
                            byte lo = scanline[si++];
                            filteredStream.WriteByte(hi);
                            filteredStream.WriteByte(lo);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        private static unsafe void FilterMethodSub(
            PngWriterSettings settings,
            PngImageAnalysisInfo pngInfo,
            IPngBitmapDataSource bitmapDataSource,
            int lineY,
            Stream filteredStream)
        {
            bool alphaChannelUsed = pngInfo.IsTransparencyUsed;
            int si = pngInfo.ClipX * pngInfo.SourcePixelSize;

            if (bitmapDataSource.IsRaw)
            {
                byte* scanline = bitmapDataSource.GetRawScanline(lineY);
                for (int xx = 0; xx < pngInfo.ClipWidth; xx++)
                {
                    switch (settings.ImageType)
                    {
                        case PngImageType.Rgb8:
                            byte blue = scanline[si++];
                            if (xx > 0)
                                blue -= scanline[si - pngInfo.SourcePixelSize - 1];
                            byte green = scanline[si++];
                            if (xx > 0)
                                green -= scanline[si - pngInfo.SourcePixelSize - 1];
                            byte red = scanline[si++];
                            if (xx > 0)
                                red -= scanline[si - pngInfo.SourcePixelSize - 1];

                            filteredStream.WriteByte(red);
                            filteredStream.WriteByte(green);
                            filteredStream.WriteByte(blue);

                            if (alphaChannelUsed)
                            {
                                byte alpha = scanline[si++];
                                if (xx > 0)
                                    alpha -= scanline[si - pngInfo.SourcePixelSize - 1];

                                filteredStream.WriteByte(alpha);
                            }
                            else
                                si++;

                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            else
            {
                byte[] scanline = bitmapDataSource.GetScanline(lineY);
                for (int xx = 0; xx < pngInfo.ClipWidth; xx++)
                {
                    switch (settings.ImageType)
                    {
                        case PngImageType.Rgb8:
                            byte blue = scanline[si++];
                            if (xx > 0)
                                blue -= scanline[si - pngInfo.SourcePixelSize - 1];
                            byte green = scanline[si++];
                            if (xx > 0)
                                green -= scanline[si - pngInfo.SourcePixelSize - 1];
                            byte red = scanline[si++];
                            if (xx > 0)
                                red -= scanline[si - pngInfo.SourcePixelSize - 1];

                            filteredStream.WriteByte(red);
                            filteredStream.WriteByte(green);
                            filteredStream.WriteByte(blue);

                            if (alphaChannelUsed)
                            {
                                byte alpha = scanline[si++];
                                if (xx > 0)
                                    alpha -= scanline[si - pngInfo.SourcePixelSize - 1];

                                filteredStream.WriteByte(alpha);
                            }
                            else
                                si++;

                            break;
                        case PngImageType.Grayscale16:
                            byte hi = scanline[si++];
                            if (xx > 0)
                                hi -= scanline[si - pngInfo.SourcePixelSize - 1];
                            byte lo = scanline[si++];
                            if (xx > 0)
                                lo -= scanline[si - pngInfo.SourcePixelSize - 1];

                            filteredStream.WriteByte(hi);
                            filteredStream.WriteByte(lo);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        private byte[] CompressImageDataChunk(
            // ReSharper disable once UnusedParameter.Local
            MemoryStream uncompressedImageDataStream,
            PngWriterSettings settings)
        {
            byte[] finalChunkData;
            byte[] uncompressedData = uncompressedImageDataStream.ToArray();

            using (MemoryStream compressedStream = new MemoryStream(uncompressedData.Length / 2))
            {
                zLibCompressor.Compress(uncompressedData, compressedStream);
                finalChunkData = compressedStream.ToArray();
            }

            return finalChunkData;
        }

        //private static byte[] CompressImageDataChunkUsingSharpZipLib(
        //    PngWriterSettings settings, BinaryWriteBlock uncompressedBlock)
        //{
        //    byte[] finalChunkData;
        //    byte[] uncompressedData = uncompressedBlock.ToArray();

        //    using (MemoryStream compressedStream = new MemoryStream(uncompressedData.Length / 2))
        //    {
        //        compressedStream.WriteByte((byte)'I');
        //        compressedStream.WriteByte((byte)'D');
        //        compressedStream.WriteByte((byte)'A');
        //        compressedStream.WriteByte((byte)'T');

        //        Deflater deflater = new Deflater(settings.CompressionLevel);
        //        //deflater.SetStrategy(DeflateStrategy.Filtered);
        //        deflater.SetInput(uncompressedData);
        //        deflater.Finish();

        //        byte[] outputBuffer = new byte[100 * 1024 * 4];
        //        while (deflater.IsNeedingInput == false)
        //        {
        //            int read = deflater.Deflate(outputBuffer);
        //            compressedStream.Write(outputBuffer, 0, read);
        //        }

        //        finalChunkData = compressedStream.ToArray();
        //    }

        //    return finalChunkData;
        //}

        private readonly IZLibCompressor zLibCompressor;
    }
}