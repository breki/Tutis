using System;
using System.Linq;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class ListChannelsInfoCommand : ICommand
    {
        public ListChannelsInfoCommand (IFileSystem fileSystem, IZipper2 zipper)
        {
            this.fileSystem = fileSystem;
            this.zipper = zipper;
            this.fileSystem = fileSystem;
            this.zipper = zipper;
        }

        public string CommandName
        {
            get { return "list-channels"; }
        }

        public string CommandUsage
        {
            get { return "list-channels <scm file>"; }
        }

        public string CommandDescription
        {
            get { return "lists the channels from the specified .scm file"; }
        }

        public int Execute(string[] args)
        {
            if (args.Length < 2)
                throw new InvalidOperationException ("Too few arguments");

            string scmFileName = args[1];

            ScmFileHandler handler = new ScmFileHandler (fileSystem, zipper);
            ChannelsInfo channels = handler.UnpackScmFile (scmFileName);

            foreach (ChannelInfo channelInfo in channels.Channels.OrderBy(x => x.ChannelNumber))
                Console.Out.WriteLine("{0}", channelInfo);

            return 0;
        }

        private readonly IFileSystem fileSystem;
        private readonly IZipper2 zipper;
    }
}