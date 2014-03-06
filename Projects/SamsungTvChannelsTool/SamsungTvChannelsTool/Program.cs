using System;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class Program
    {
        public static void Main (string[] args)
        {
            IFileSystem fileSystem = new WindowsFileSystem();
            IZipper zipper = new Zipper(fileSystem);
            ScmFileReader reader = new ScmFileReader(fileSystem, zipper);
            ChannelsInfo channels = reader.ReadScmFile(args[0]);

            int currentChannelNumber = 1;
            foreach (ChannelInfo channelInfo in channels.Channels)
            {
                if (channelInfo.ChannelNumber == 0 || channelInfo.ChannelNumber > 200)
                    break;

                while (currentChannelNumber < channelInfo.ChannelNumber)
                    Console.Out.WriteLine("{0}: ---", currentChannelNumber++);

                Console.Out.WriteLine (channelInfo);
                currentChannelNumber++;
            }
        }
    }
}
