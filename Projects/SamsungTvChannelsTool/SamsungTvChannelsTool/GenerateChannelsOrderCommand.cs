using System;
using System.IO;
using System.Text;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class GenerateChannelsOrderCommand : ICommand
    {
        public GenerateChannelsOrderCommand(IFileSystem fileSystem, IZipper zipper)
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

            ScmFileReader reader = new ScmFileReader (fileSystem, zipper);
            ChannelsInfo channels = reader.ReadScmFile (scmFileName);

            using (Stream stream = fileSystem.OpenFileToWrite(channelsOrderFileName))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                int currentChannelNumber = 1;
                foreach (ChannelInfo channelInfo in channels.Channels)
                {
                    if (channelInfo.ChannelNumber == 0)
                        break;

                    while (currentChannelNumber < channelInfo.ChannelNumber)
                    {
                        writer.WriteLine();
                        currentChannelNumber++;
                    }

                    writer.WriteLine(channelInfo.Name);
                    currentChannelNumber++;
                }
            }

            Console.Out.WriteLine("Channel order file written");

            return 0;
        }

        private readonly IFileSystem fileSystem;
        private readonly IZipper zipper;
    }
}