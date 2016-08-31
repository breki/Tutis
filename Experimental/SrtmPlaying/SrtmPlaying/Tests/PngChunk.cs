namespace Brejc.Geo.Tests.ImagingTests
{
    public class PngChunk
    {
        public string ChunkType
        {
            get { return chunkType; }
            set { chunkType = value; }
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public int Crc
        {
            get { return crc; }
            set { crc = value; }
        }

        private string chunkType;
        private byte[] data;
        private int crc;
    }
}