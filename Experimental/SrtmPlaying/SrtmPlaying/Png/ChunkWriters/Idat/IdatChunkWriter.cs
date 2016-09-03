using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SrtmPlaying.Png.ChunkWriters.Idat
{
    public class IdatChunkWriter : PngChunkWriterBase, IPngChunkWriter
    {
        public IdatChunkWriter(IZLibCompressor zLibCompressor)
        {
            this.zLibCompressor = zLibCompressor;

            filters.Add(new PngFilterNone());
            filters.Add(new PngFilterSub());
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

        private MemoryStream FilterImageData(
            PngWriterSettings settings,
            PngImageAnalysisInfo pngInfo,
            IPngBitmapDataSource bitmap)
        {
            MemoryStream filteredStream = new MemoryStream();

            int bitmapHeight = bitmap.Height;
            int maxy = Math.Min(bitmapHeight, pngInfo.ClipY + pngInfo.ClipHeight);

            for (int yy = pngInfo.ClipY; yy < maxy; yy++)
                FilterScanline(filteredStream, settings, pngInfo, bitmap, yy);

            return filteredStream;
        }

        private void FilterScanline(
            Stream filteredStream, 
            PngWriterSettings settings, 
            PngImageAnalysisInfo pngInfo, 
            IPngBitmapDataSource bitmap, 
            int yy)
        {
            byte[][] filteredDataByFilters = new byte[5][];
            int[] filteredDataSums = new int[5];
            int bestSumSoFar = int.MaxValue;
            int bestFilterToUse = 0;
            for (PngFilterType filterType = PngFilterType.None; filterType <= PngFilterType.Sub; filterType++)
            {
                int filterTypeInt = (int)filterType;
                IPngFilter filterToUse = filters[filterTypeInt];
                filteredDataByFilters[filterTypeInt] = filterToUse.Filter(bitmap.GetScanline(yy), settings.ImageType, pngInfo.IsTransparencyUsed, pngInfo.DestinationPixelSize, pngInfo.ClipWidth);
                int sum = CalculateFilteredDataSum(filteredDataByFilters[filterTypeInt]);
                filteredDataSums[filterTypeInt] = sum;

                if (sum < bestSumSoFar)
                {
                    bestSumSoFar = sum;
                    bestFilterToUse = filterTypeInt;
                }
            }

            filteredStream.WriteByte((byte)bestFilterToUse);

            filteredStream.Write(filteredDataByFilters[bestFilterToUse], 0, filteredDataByFilters[bestFilterToUse].Length);
        }

        private static int CalculateFilteredDataSum(byte[] data)
        {
            int sum = 0;

            foreach (byte b in data)
                sum += (sbyte)b;

            return sum;
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
        private readonly List<IPngFilter> filters = new List<IPngFilter>();
    }
}