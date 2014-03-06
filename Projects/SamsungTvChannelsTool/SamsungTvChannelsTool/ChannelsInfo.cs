using System.Collections.Generic;

namespace SamsungTvChannelsTool
{
    public class ChannelsInfo
    {
        public ChannelsInfo(string fileName)
        {
            this.fileName = fileName;
        }

        public string FileName
        {
            get { return fileName; }
        }

        public IList<ChannelInfo> Channels
        {
            get { return channels; }
        }

        public void AddChannel(ChannelInfo channel)
        {
            channels.Add(channel);
        }

        private readonly string fileName;
        private List<ChannelInfo> channels = new List<ChannelInfo>();
    }
}