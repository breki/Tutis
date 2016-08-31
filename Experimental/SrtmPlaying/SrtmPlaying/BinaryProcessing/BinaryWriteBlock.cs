using System;
using System.IO;

namespace SrtmPlaying.BinaryProcessing
{
    public class BinaryWriteBlock : IDisposable
    {
        public BinaryWriteBlock()
        {
            writeStream = new MemoryStream();
            writer = new BinaryWriterEx (writeStream);
        }

        public BinaryWriteBlock(int byteSize)
        {
            writeStream = new MemoryStream(byteSize);
            writer = new BinaryWriterEx(writeStream);
        }

        public Endianess Endianess
        {
            get { return endianess; }
            set { endianess = value; }
        }

        public void Write(byte value)
        {
            writer.Write(value);
        }

        public void Write(int value)
        {
            if (endianess == Endianess.BigEndian)
                writer.WriteBigEndian(value);
            else
                writer.Write(value);
        }

        public void Write (byte[] data)
        {
            writer.Write(data);
        }

        public byte[] ToArray()
        {
            writeStream.Flush ();
            return writeStream.ToArray ();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources     
                    writer.Dispose();
                    writeStream.Dispose();
                }

                disposed = true;
            }
        }

        private bool disposed;
        private Endianess endianess = Endianess.BigEndian;
        private readonly MemoryStream writeStream;
        private readonly BinaryWriterEx writer;
    }
}