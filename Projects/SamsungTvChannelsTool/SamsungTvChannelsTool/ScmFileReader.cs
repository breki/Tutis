using System;
using System.IO;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class ScmFileReader
    {
        public ScmFileReader(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public ChannelsInfo ReadScmFile(string scmFileName)
        {
            using (Stream stream = fileSystem.OpenFileToRead(scmFileName))
            using (BinaryReader reader = new BinaryReader(stream))
                return ReadScmFileFromStream(reader);
        }

        private static ChannelsInfo ReadScmFileFromStream(BinaryReader reader)
        {
            ChannelsInfo channels = new ChannelsInfo();

            while (true)
            {
                ChannelInfo channel = ReadChannelInfo(reader);
                if (channel == null)
                    break;

                channels.AddChannel(channel);
            }

            return channels;
        }

        private static ChannelInfo ReadChannelInfo(BinaryReader reader)
        {
            short channelNumber = ReadInt16LittleEndian(reader);
            short vpid = ReadInt16BigEndian(reader);
            short mpid = ReadInt16BigEndian(reader);

            reader.ReadBytes(4);

            short symbolRate = ReadInt16BigEndian (reader);

            reader.ReadBytes (4);

            byte network = reader.ReadByte();

            reader.ReadBytes (2);

            char[] chrs = reader.ReadChars(200);

            throw new NotImplementedException();
        }

        private static short ReadInt16LittleEndian(BinaryReader reader)
        {
            byte b1 = reader.ReadByte ();
            byte b2 = reader.ReadByte ();

            return (short)(b1 | b2 << 8);
        }

        private static short ReadInt16BigEndian(BinaryReader reader)
        {
            byte b1 = reader.ReadByte ();
            byte b2 = reader.ReadByte ();

            return (short)(b1 << 8 | b2);
        }

        private readonly IFileSystem fileSystem;
    }
}