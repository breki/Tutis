using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SrtmPlaying.Tests
{
    public class PngImage
    {
        public IList<PngChunk> Chunks
        {
            get { return chunks; }
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public byte BitDepth { get; set; }
        public byte ColorType { get; set; }
        public byte CompressionType { get; set; }
        public byte FilterType { get; set; }
        public byte InterlaceMethod { get; set; }

        public void Construct()
        {
            byte[] compressedData;
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (PngChunk chunk in chunks)
                {
                    if (chunk.ChunkType == "IDAT")
                        stream.Write(chunk.Data, 0, chunk.Data.Length);
                }

                compressedData = stream.ToArray();
            }

            byte[] decompressedData;
            using (MemoryStream decompressedStream = new MemoryStream())
            //using (BinaryWriter writer = new BinaryWriter(decompressedDataStream))
            using (MemoryStream compressedStream = new MemoryStream (compressedData))
            {
                byte compressionMethod = (byte)compressedStream.ReadByte ();
                //if (compressionMethod != 8)
                //    throw new InvalidOperationException ("compression method");
                byte flags = (byte)compressedStream.ReadByte ();

                using (DeflateStream deflateStream = new DeflateStream (compressedStream, CompressionMode.Decompress))
                    deflateStream.CopyTo(decompressedStream);

                decompressedStream.Flush ();
                decompressedData = decompressedStream.ToArray ();
            }

            int i = 0;
            for (int y = 0; y < Height; y++)
            {
                byte filter = decompressedData[i++];

                if (filter < 0 || filter > 4)
                    throw new InvalidOperationException();

                for (int x = 0; x < Width; x++)
                {
                    byte r = decompressedData[i++];
                    byte g = decompressedData[i++];
                    byte b = decompressedData[i++];
                    byte a = decompressedData[i++];
                }
            }
        }

        private readonly List<PngChunk> chunks = new List<PngChunk>();
    }
}