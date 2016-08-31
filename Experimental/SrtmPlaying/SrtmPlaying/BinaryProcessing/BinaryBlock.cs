using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace SrtmPlaying.BinaryProcessing
{
    public class BinaryBlock
    {
        public BinaryBlock(byte[] data)
        {
            this.data = data;
        }

        public BinaryBlock(BinaryReader reader, int length)
        {
            Contract.Requires(reader != null);
            Contract.Requires(length >= 0);

            data = reader.ReadBytes(length);
        }

        public bool BigEndian
        {
            get { return bigEndian; }
            set { bigEndian = value; }
        }

        public bool Eof
        {
            get
            {
                return sequentialIndex >= data.Length;
            }
        }

        public int SequentialIndex
        {
            get { return sequentialIndex; }
        }

        public byte ReadByte()
        {
            if (Eof)
                throw new InvalidOperationException("Eof");

            return data[sequentialIndex++];
        }

        public byte[] ReadBytes(int count)
        {
            Contract.Requires (count >= 0);

            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
                bytes[i] = ReadByte();

            return bytes;
        }

        public byte[] ReadBytes(int offset, int count)
        {
            Contract.Requires (count >= 0);

            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
                bytes[i] = data[offset + i];

            return bytes;
        }

        public double ReadDouble()
        {
            byte[] bytes = ReadBytes(sizeof(double));
            if (!bigEndian)
                Array.Reverse(bytes);

            return BitConverter.ToDouble(bytes, 0);
        }

        public short ReadInt16 ()
        {
            return bigEndian ? ReadInt16BigEndian () : ReadInt16LittleEndian ();
        }

        public short ReadInt16BigEndian ()
        {
            byte value1 = ReadByte ();
            byte value2 = ReadByte ();

            return (short)(value1 << 8 | value2);
        }

        public short ReadInt16LittleEndian ()
        {
            byte value1 = ReadByte();
            byte value2 = ReadByte();

            return (short)(value2 << 8 | value1);
        }

        [CLSCompliant(false)]
        public ushort ReadUInt16()
        {
            return bigEndian ? ReadUInt16BigEndian() : ReadUInt16LittleEndian();
        }

        [CLSCompliant(false)]
        public ushort ReadUInt16BigEndian()
        {
            byte value1 = ReadByte();
            byte value2 = ReadByte();

            return (ushort)(value1 << 8 | value2);
        }

        [CLSCompliant(false)]
        public ushort ReadUInt16LittleEndian()
        {
            byte value1 = ReadByte();
            byte value2 = ReadByte();

            return (ushort)(value2 << 8 | value1);
        }

        public int ReadInt32 ()
        {
            return bigEndian ? ReadInt32BigEndian () : ReadInt32LittleEndian ();
        }

        public int ReadInt32BigEndian ()
        {
            short value1 = ReadInt16BigEndian ();
            short value2 = ReadInt16BigEndian ();

            return value1 << 16 | ((ushort)value2);
        }

        public int ReadInt32LittleEndian ()
        {
            short value1 = ReadInt16LittleEndian ();
            short value2 = ReadInt16LittleEndian ();

            return value2 << 16 | ((ushort)value1);
        }

        public string ReadString (int offset, int length)
        {
            return Encoding.ASCII.GetString(data, offset, length);
        }

        public int[] ReadInt32Values (int offset, int count)
        {
            Contract.Requires (count >= 0);

            return ReadValues<int>(offset, count, ReadInt32);
        }

        [CLSCompliant(false)]
        public uint[] ReadUInt32Values (int offset, int count)
        {
            Contract.Requires (count >= 0);

            return ReadValues (offset, count, () => (uint)ReadInt32 ());
        }

        [CLSCompliant (false)]
        public ushort[] ReadUInt16Values (int offset, int count)
        {
            Contract.Requires (count >= 0);

            return ReadValues(offset, count, () => (ushort)ReadInt16());
        }

        public TValue[] ReadValues<TValue> (int offset, int count, Func<TValue> readValueFunction)
        {
            Contract.Requires (count >= 0);

            TemporaryMoveTo (offset);

            TValue[] values = new TValue[count];
            for (int i = 0; i < count; i++)
                values[i] = readValueFunction();

            RestorePosition ();

            return values;
        }

        public void RestorePosition()
        {
            sequentialIndex = indices.Pop();
        }

        public void Skip(int bytesCount)
        {
            for (int i = 0; i < bytesCount; i++)
                ReadByte();
        }

        public void TemporaryMoveTo(int newOffset)
        {
            indices.Push(sequentialIndex);
            sequentialIndex = newOffset;
        }

        private bool bigEndian;
        private readonly byte[] data;
        private readonly Stack<int> indices = new Stack<int>();
        private int sequentialIndex;
    }
}