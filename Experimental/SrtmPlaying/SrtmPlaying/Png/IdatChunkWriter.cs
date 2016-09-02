using System;
using System.IO;
using System.Text;

namespace SrtmPlaying.Png
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
                FilterMethod0(settings, pngInfo, bitmap, yy, filteredStream);
                //FilterMethod1 (bitmap, y, filteredScanlineBuffer, width, pngInfo);
            }

            return filteredStream;
        }

        private static unsafe void FilterMethod0(
            PngWriterSettings settings,
            PngImageAnalysisInfo pngInfo,
            IPngBitmapDataSource bitmapDataSource,
            int lineY,
            Stream filteredStream)
        {
            bool alphaChannelUsed = pngInfo.IsTransparencyUsed;
            int si = pngInfo.ClipX * pngInfo.PixelSize;

            filteredStream.WriteByte(0);

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

        // ReSharper disable once UnusedMember.Local
        //private static unsafe void FilterMethod1 (
        //    IPngBitmapDataSource bitmap, 
        //    int y, 
        //    IList<byte> filteredScanlineBuffer, 
        //    int width, 
        //    PngImageAnalysisInfo pngInfo)
        //{
        //    bool alphaChannelUsed = pngInfo.IsTransparencyUsed;

        //    byte* scanline = bitmap.GetScanline (y);
        //    int si = 0;
        //    int fi = 0;

        //    filteredScanlineBuffer[fi++] = 1;

        //    byte bb = 0, gg = 0, rr = 0, aa = 0;
        //    for (int x = 0; x < width; x++)
        //    {
        //        byte blue = scanline[si++];
        //        byte green = scanline[si++];
        //        byte red = scanline[si++];
        //        byte alpha = scanline[si++];

        //        filteredScanlineBuffer[fi++] = (byte)(red - rr);
        //        filteredScanlineBuffer[fi++] = (byte)(green - gg);
        //        filteredScanlineBuffer[fi++] = (byte)(blue - bb);

        //        if (alphaChannelUsed)
        //            filteredScanlineBuffer[fi++] = (byte)(alpha - aa);

        //        rr = red;
        //        gg = green;
        //        bb = blue;
        //        aa = alpha;
        //    }
        //}

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