using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace SrtmPlaying.BinaryProcessing
{
    public static class BinaryWriterExtensions
    {
        public static void WriteBigEndian(this BinaryWriter writer, int value)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[3 - i] = (byte)(value & 0xff);
                value >>= 8;
            }

            writer.Write(bytes);
        }

        public static void WriteBigEndian(this BinaryWriter writer, uint value)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[3 - i] = (byte)(value & 0xff);
                value >>= 8;
            }

            writer.Write(bytes);
        }
    }


    public static class BinaryReaderExtensions
    {
        public static ushort ReadUInt16BigEndian(this BinaryReader reader)
        {
            Contract.Requires(reader != null);
            byte value1 = reader.ReadByte();
            byte value2 = reader.ReadByte();

            return (ushort)(value1 << 8 | value2);
        }

        public static short ReadInt16BigEndian(this BinaryReader reader)
        {
            Contract.Requires(reader != null);
            byte value1 = reader.ReadByte();
            byte value2 = reader.ReadByte();

            return (short)(value1 << 8 | value2);
        }

        public static int ReadInt32BigEndian(this BinaryReader reader)
        {
            Contract.Requires(reader != null);
            ushort value1 = reader.ReadUInt16BigEndian();
            ushort value2 = reader.ReadUInt16BigEndian();

            return value1 << 16 | value2;
        }

        public static uint ReadUInt32BigEndian(this BinaryReader reader)
        {
            Contract.Requires(reader != null);
            ushort value1 = reader.ReadUInt16BigEndian();
            ushort value2 = reader.ReadUInt16BigEndian();

            return (uint)(value1 << 16 | value2);
        }

        public static ulong ReadUInt64BigEndian(this BinaryReader reader)
        {
            Contract.Requires(reader != null);
            uint value1 = reader.ReadUInt32BigEndian();
            uint value2 = reader.ReadUInt32BigEndian();

            return value1 << 32 | value2;
        }
    }
}