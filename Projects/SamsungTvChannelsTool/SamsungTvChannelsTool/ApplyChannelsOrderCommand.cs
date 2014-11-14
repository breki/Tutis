using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            // read all records from the CableD 
            ChannelsInfo workingChannels = channels.Clone();

            // start preparing a new CableD file
            using (Stream writeStream = fileSystem.OpenFileToWrite (channels.FileName))
            using (BinaryWriter writer = new BinaryWriter(writeStream))
            {
                int channelsCount = 0;

                // for each record in channels order file
                foreach (KeyValuePair<int, Tuple<string, int?>> channelPair in channelsOrderFile.Channels)
                {
                    // insert the CableD record (with the channel number and checksum corrected)
                    int channelNumber = channelPair.Key;
                    string channelName = channelPair.Value.Item1;
                    int? channelTsid = channelPair.Value.Item2;

                    ChannelInfo channelInfo = workingChannels.FindChannel(channelName, channelTsid);
                    if (channelInfo == null)
                        throw new InvalidOperationException("Channel '{0} #{1}' does not exist".Fmt(channelName, channelTsid));

                    // remove the channel from the index so we know it has been used
                    workingChannels.RemoveChannel(channelInfo);

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
                    channelsCount++;
                }
            }

            // zip the new SCM file
            string outputScmFileName = Path.Combine (
                Path.GetDirectoryName (scmFileName),
                Path.GetFileNameWithoutExtension (scmFileName) + "_new" + Path.GetExtension (scmFileName));

            handler.PackScmFile (outputScmFileName);

            WriteOutUnusedChannels (workingChannels, channelsOrderFile);

            return 0;
        }

        private static void WriteOutUnusedChannels(
            ChannelsInfo channels, ChannelsOrderFile channelsOrderFile)
        {
            IEnumerable<ChannelInfo> unusedChannels = channels.Channels.Where(x => !channelsOrderFile.IsIgnoredChannel(x.Name));

            if (unusedChannels.Any())
            {
                Console.Out.WriteLine("These channels are not in your order list:");

                foreach (ChannelInfo unusedChannel in unusedChannels.OrderBy(x => x.Name))
                    Console.Out.WriteLine(unusedChannel);
            }
        }

        private readonly IFileSystem fileSystem;
        private readonly IZipper2 zipper;
    }
}