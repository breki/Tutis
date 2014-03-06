using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class ChannelsOrderFile
    {
        public ChannelsOrderFile(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public SortedList<int, string> Channels
        {
            get { return channels; }
        }

        public void AddChannel (int channelNumber, string channelName)
        {
            channels.Add(channelNumber, channelName);
        }

        public void Write(string fileName)
        {
            using (Stream stream = fileSystem.OpenFileToWrite (fileName))
            using (StreamWriter writer = new StreamWriter (stream, Encoding.UTF8))
            {
                int currentChannelNumber = 1;
                foreach (KeyValuePair<int, string> channelPair in channels)
                {
                    while (currentChannelNumber < channelPair.Key)
                    {
                        writer.WriteLine ();
                        currentChannelNumber++;
                    }

                    writer.WriteLine (channelPair.Value);
                    currentChannelNumber++;
                }
            }
        }

        public static ChannelsOrderFile Read (string channelsOrderFileName, IFileSystem fileSystem)
        {
            ChannelsOrderFile channelsOrderFile = new ChannelsOrderFile(fileSystem);

            int channelNumber = 1;
            foreach (string channelName in fileSystem.ReadFileAsStringLines (channelsOrderFileName))
            {
                if (!String.IsNullOrEmpty(channelName))
                    channelsOrderFile.AddChannel(channelNumber, channelName.Trim());

                channelNumber++;
            }

            return channelsOrderFile;
        }

        private readonly IFileSystem fileSystem;
        private SortedList<int, string> channels = new SortedList<int, string> ();
    }
}