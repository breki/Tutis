using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class ChannelsOrderFile
    {
        public const string IgnoreHeader = "---IGNORE---";

        public ChannelsOrderFile (IFileSystem fileSystem)
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

        public bool IsIgnoredChannel(string channelName)
        {
            return ignoredChannels.Contains(channelName);
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

            string[] lines = fileSystem.ReadFileAsStringLines(channelsOrderFileName).ToArray();
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                int channelNumber = lineIndex+1;

                string channelName = lines[lineIndex].Trim();

                if (channelName == IgnoreHeader)
                {
                    ReadIgnoredChannels (channelsOrderFile, lines, lineIndex);
                    break;
                }
                
                if (!String.IsNullOrEmpty(channelName))
                    channelsOrderFile.AddChannel(channelNumber, channelName);
            }

            return channelsOrderFile;
        }

        private static void ReadIgnoredChannels(ChannelsOrderFile channelsOrderFile, string[] lines, int ignoredHeaderLineIndex)
        {
            for (int lineIndex = ignoredHeaderLineIndex+1; lineIndex < lines.Length; lineIndex++)
            {
                string channelName = lines[lineIndex].Trim ();
                channelsOrderFile.AddIgnoredChannel(channelName);
            }
        }

        private void AddIgnoredChannel(string channelName)
        {
            ignoredChannels.Add(channelName);
        }

        private readonly IFileSystem fileSystem;
        private SortedList<int, string> channels = new SortedList<int, string> ();
        private HashSet<string> ignoredChannels = new HashSet<string>();
    }
}