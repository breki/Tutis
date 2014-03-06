using System;
using System.Collections.Generic;
using System.IO;
using Brejc.Common;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class ApplyChannelsOrderCommand : ICommand
    {
        public ApplyChannelsOrderCommand(IFileSystem fileSystem, IZipper2 zipper)
        {
            this.fileSystem = fileSystem;
            this.zipper = zipper;
        }

        public string CommandName
        {
            get { return "apply-channels"; }
        }

        public string CommandUsage
        {
            get { return "apply-channels <channels file> <scm file>"; }
        }

        public string CommandDescription
        {
            get { return "applies the channels order file to the specified .scm file"; }
        }

        public int Execute(string[] args)
        {
            if (args.Length < 3)
                throw new InvalidOperationException("Too few arguments");

            string channelsOrderFileName = args[1];
            string scmFileName = args[2];

            // read channels order file
            ChannelsOrderFile channelsOrderFile = ChannelsOrderFile.Read(channelsOrderFileName, fileSystem);

            // unzip the SCM file
            ScmFileHandler handler = new ScmFileHandler (fileSystem, zipper);
            ChannelsInfo channels = handler.UnpackScmFile (scmFileName);

            // read all records from the CableD and index them by the channel name
            Dictionary<string, ChannelInfo> indexedChannels = IndexChannelsByName(channels);

            // start preparing a new CableD file
            // save the file in place of the existing one
            using (Stream writeStream = fileSystem.OpenFileToWrite (channels.FileName))
            using (BinaryWriter writer = new BinaryWriter(writeStream))
            {
                int channelsCount = 0;

                // for each record in channels order file
                foreach (KeyValuePair<int, string> channelPair in channelsOrderFile.Channels)
                {
                    // insert the CableD record (with the channel number and checksum corrected)
                    int channelNumber = channelPair.Key;
                    string channelName = channelPair.Value;

                    ChannelInfo channelInfo;
                    if (!indexedChannels.TryGetValue(channelName, out channelInfo))
                        throw new InvalidOperationException("Channel '{0}' does not exist".Fmt(channelName));

                    channelInfo.ChannelNumber = (short)channelNumber;
                    channelInfo.RecalculateChecksum();

                    channelInfo.Write(writer);
                    channelsCount++;
                }

                // fill the rest of the file with zeros
                while (channelsCount % 1000 != 0)
                {
                    for (int i = 0; i < ScmFileHandler.RecordSize; i++)
                        writer.Write((byte)0);
                }
            }

            // zip back the SCM file
            handler.PackScmFile (scmFileName);

            return 0;
        }

        private static Dictionary<string, ChannelInfo> IndexChannelsByName(ChannelsInfo channels)
        {
            Dictionary<string, ChannelInfo> indexedChannels = new Dictionary<string, ChannelInfo>();

            foreach (ChannelInfo channel in channels.Channels)
                indexedChannels.Add(channel.Name, channel);

            return indexedChannels;
        }

        private readonly IFileSystem fileSystem;
        private readonly IZipper2 zipper;
    }
}