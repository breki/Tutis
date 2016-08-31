using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace SrtmPlaying.BinaryProcessing
{
    [SuppressMessage ("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]    
    public class BinaryWriterEx : BinaryWriter
    {
        public BinaryWriterEx(Stream stream) : base(stream)
        {
        }

        public void WriteBigEndian(int value)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[3-i] = (byte)(value & 0xff);
                value >>= 8;
            }

            Write(bytes);
        }

        [CLSCompliant (false)]
        public void WriteBigEndian (uint value)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[3 - i] = (byte)(value & 0xff);
                value >>= 8;
            }

            Write (bytes);
        }

        public void WriteByteRepeat(byte value, int count)
        {
            for (int i = 0; i < count; i++)
                Write(value);
        }
    }
}