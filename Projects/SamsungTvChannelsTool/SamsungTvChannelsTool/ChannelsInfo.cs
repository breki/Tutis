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

        public ChannelInfo FindChannel (string channelName, int? channelTsid)
        {
            if (channelTsid == null)
                return channels.Find(x => x.Name == channelName);

            return channels.Find (x => x.Name == channelName && x.MultiplexTsid == channelTsid);
        }

        public ChannelsInfo Clone()
        {
            ChannelsInfo clone = new ChannelsInfo(fileName);
            clone.channels.AddRange(channels);
            return clone;
        }

        public void RemoveChannel (ChannelInfo channelInfo)
        {
            channels.Remove(channelInfo);
        }

        private readonly string fileName;
        private List<ChannelInfo> channels = new List<ChannelInfo>();
    }
}