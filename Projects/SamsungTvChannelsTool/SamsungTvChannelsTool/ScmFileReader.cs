using System;
using System.IO;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class ScmFileReader
    {
        public ScmFileReader(IFileSystem fileSystem, IZipper zipper)
        {
            this.fileSystem = fileSystem;
            this.zipper = zipper;
        }

        public ChannelsInfo ReadScmFile(string scmFileName)
        {
            const string UnzipDir = "temp";
            zipper.Unzip(scmFileName, UnzipDir);

            string channelsFileName = Path.Combine (UnzipDir, "map-CableD");
            long channelsFileSize = fileSystem.GetFileInformation(channelsFileName).Length;

            using (Stream stream = fileSystem.OpenFileToRead (channelsFileName))
            using (BinaryReader reader = new BinaryReader(stream))
                return ReadScmFileFromStream(reader);
        }

        private static ChannelsInfo ReadScmFileFromStream(BinaryReader reader)
        {
            ChannelsInfo channels = new ChannelsInfo ();

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
            const int RecordSize = 320;
            byte[] rawData = reader.ReadBytes(RecordSize);
            if (rawData.Length == 0)
                return null;
            return new ChannelInfo(rawData);
        }

        private readonly IFileSystem fileSystem;
        private readonly IZipper zipper;
    }
}