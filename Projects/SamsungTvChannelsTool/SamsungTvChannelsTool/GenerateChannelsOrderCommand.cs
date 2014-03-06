using System;
using System.IO;
using System.Text;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class GenerateChannelsOrderCommand : ICommand
    {
        public GenerateChannelsOrderCommand(IFileSystem fileSystem, IZipper2 zipper)
        {
            this.fileSystem = fileSystem;
            this.zipper = zipper;
        }

        public string CommandName
        {
            get { return "gen-channels"; }
        }

        public string CommandUsage
        {
            get { return "gen-channels <scm file> <channels file>"; }
        }

        public string CommandDescription
        {
            get { return "generates the channels order file using the data from the specified .scm file"; }
        }

        public int Execute(string[] args)
        {
            if (args.Length < 3)
                throw new InvalidOperationException("Too few arguments");

            string scmFileName = args[1];
            string channelsOrderFileName = args[2];

            ScmFileHandler handler = new ScmFileHandler (fileSystem, zipper);
            ChannelsInfo channels = handler.UnpackScmFile (scmFileName);

            ChannelsOrderFile channelsOrderFile = new ChannelsOrderFile (fileSystem);

            foreach (ChannelInfo channelInfo in channels.Channels)
            {
                if (channelInfo.ChannelNumber == 0)
                    break;

                channelsOrderFile.AddChannel(channelInfo.ChannelNumber, channelInfo.Name);
            }

            channelsOrderFile.Write(channelsOrderFileName);

            Console.Out.WriteLine("Channel order file written");

            return 0;
        }

        private readonly IFileSystem fileSystem;
        private readonly IZipper2 zipper;
    }
}