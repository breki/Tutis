﻿using System;
using System.IO;
using System.Text;
using LibroLib;
using SrtmPlaying.BinaryProcessing;

namespace SrtmPlaying.Png
{
    public class IhdrChunkWriter : PngChunkWriterBase, IPngChunkWriter
    {
        public byte[] WriteChunk(
            PngWriterSettings settings, 
            PngImageAnalysisInfo pngInfo, 
            IPngBitmapDataSource bitmap)
        {
            byte bitDepth;
            byte colorType;

            DetermineBitDepthAndColorType(settings, pngInfo, out bitDepth, out colorType);

            using (MemoryStream chunkStream = new MemoryStream())
            {
                WriteChunkType(chunkStream, "IHDR");

                using (BinaryWriter writer = new BinaryWriter(chunkStream, Encoding.ASCII, true))
                {
                    writer.WriteBigEndian(pngInfo.ClipWidth);
                    writer.WriteBigEndian(pngInfo.ClipHeight);
                    writer.Write(bitDepth);
                    writer.Write(colorType);
                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                }

                return chunkStream.ToArray();
            }
        }

        private static void DetermineBitDepthAndColorType(
            PngWriterSettings settings,
            PngImageAnalysisInfo pngInfo,
            out byte bitDepth,
            out byte colorType)
        {
            switch (settings.ImageType)
            {
                case PngImageType.Grayscale16:
                    colorType = 0;
                    bitDepth = 16;
                    break;
                case PngImageType.Rgb8:
                    colorType = (byte)(pngInfo.IsTransparencyUsed ? 6 : 2);
                    bitDepth = 8;
                    break;
                default:
                    throw new InvalidOperationException("Unsupported PNG image type '{0}'.".Fmt(settings.ImageType));
            }
        }
    }
}