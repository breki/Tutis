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

            using (Stream stream = fileSystem.OpenFileToRead (channelsFileName))
            using (BinaryReader reader = new BinaryReader(stream))
                return ReadScmFileFromStream(reader, channelsFileName);
        }

        public void PackScmFile(string scmFileName)
        {
            FileSet newPackageFiles = new FileSet();
            newPackageFiles.BaseDir = UnzipDir + Path.DirectorySeparatorChar;

            foreach (string file in packageFiles.Files)
                newPackageFiles.AddFile(Path.Combine(UnzipDir, file));

            zipper.Zip(new NullTaskExecutionContext(), scmFileName, newPackageFiles, null);
        }

        private static ChannelsInfo ReadScmFileFromStream(BinaryReader reader, string channelsFileName)
        {
            ChannelsInfo channels = new ChannelsInfo (channelsFileName);

            while (true)
            {
                ChannelInfo channel = ReadChannelInfo(reader);
                if (channel == null)
                    break;

                if (channel.ChannelNumber == 0)
                    continue;

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