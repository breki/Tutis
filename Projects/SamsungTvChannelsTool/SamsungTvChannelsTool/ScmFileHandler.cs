using System;
using System.IO;
using Brejc.Common.FileSystem;
using Brejc.Common.Tasks;

namespace SamsungTvChannelsTool
{
    public class ScmFileHandler
    {
        public const int RecordSize = 320;

        public ScmFileHandler(IFileSystem fileSystem, IZipper2 zipper)
        {
            this.fileSystem = fileSystem;
            this.zipper = zipper;
        }

        public ChannelsInfo UnpackScmFile(string scmFileName)
        {
            packageFiles = zipper.Unzip(scmFileName, UnzipDir);

            string channelsFileName = Path.Combine (UnzipDir, "map-CableD");
            long channelsFileSize = fileSystem.GetFileInformation(channelsFileName).Length;

            using (Stream stream = fileSystem.OpenFileToRead (channelsFileName))
            using (BinaryReader reader = new BinaryReader(stream))
                return ReadScmFileFromStream(reader, channelsFileName);
        }

        public void PackScmFile(string scmFileName)
        {
            zipper.Zip(new NullTaskExecutionContext(), scmFileName, packageFiles, null);
        }

        private static ChannelsInfo ReadScmFileFromStream(BinaryReader reader, string channelsFileName)
        {
            ChannelsInfo channels = new ChannelsInfo (channelsFileName);

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
            byte[] rawData = reader.ReadBytes(RecordSize);
            if (rawData.Length == 0)
                return null;
            return new ChannelInfo(rawData);
        }

        private const string UnzipDir = "temp";
        private readonly IFileSystem fileSystem;
        private readonly IZipper2 zipper;
        private FileSet packageFiles;
    }
}