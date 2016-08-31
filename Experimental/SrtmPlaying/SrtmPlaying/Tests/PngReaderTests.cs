using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;
using SrtmPlaying.BinaryProcessing;

namespace SrtmPlaying.Tests
{
    public class PngReaderTests
    {
        [Test]
        public void Test()
        {
            PngImage image = new PngImage();

            //using (Stream stream = File.OpenRead(Constants.DataSamplesPath + "Bitmaps/SimpleGeoref.png"))
            //using (Stream stream = File.OpenRead(Constants.DataSamplesPath + "Bitmaps/SimpleGeoref.png"))
            using (Stream stream = File.OpenRead ("output/test.png"))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                byte[] signature = reader.ReadBytes(8);
                while(true)
                {
                    if (!ReadChunk(image, reader))
                        break;
                }
            }

            image.Construct();
        }

        private static bool ReadChunk(PngImage image, BinaryReader reader)
        {
            int len = ReadInt32BigEndian(reader);
            string chunkType = Encoding.ASCII.GetString(reader.ReadBytes(4));
            byte[] data = reader.ReadBytes(len);
            int crc = ReadInt32BigEndian(reader);

            PngChunk chunk = new PngChunk();
            chunk.ChunkType = chunkType;
            chunk.Data = data;
            chunk.Crc = crc;
            image.Chunks.Add(chunk);

            Debug.WriteLine("{0}, len={1}", chunkType, len);

            ProcessChunk(image, chunk);

            return reader.PeekChar() != -1;
        }

        private static void ProcessChunk(PngImage image, PngChunk chunk)
        {
            switch (chunk.ChunkType)
            {
                case "IHDR":
                    ProcessIHDR(image, chunk);
                    break;
            }
        }

        private static void ProcessIHDR(PngImage image, PngChunk chunk)
        {
            BinaryBlock block = new BinaryBlock(chunk.Data);
            block.BigEndian = true;
            image.Width = block.ReadInt32();
            image.Height = block.ReadInt32();
            image.BitDepth = block.ReadByte();
            image.ColorType = block.ReadByte();
            image.CompressionType = block.ReadByte();
            image.FilterType = block.ReadByte();
            image.InterlaceMethod = block.ReadByte();
        }

        public static int ReadInt32BigEndian(BinaryReader reader)
        {
            short value1 = ReadInt16BigEndian(reader);
            short value2 = ReadInt16BigEndian(reader);

            return value1 << 16 | ((ushort)value2);
        }

        public static short ReadInt16BigEndian(BinaryReader reader)
        {
            byte value1 = reader.ReadByte();
            byte value2 = reader.ReadByte();

            return (short)(value1 << 8 | value2);
        }
    }
}