﻿using System.Collections.Generic;

namespace SamsungTvChannelsTool
{
    public class ChannelsInfo
    {
        public void AddChannel(ChannelInfo channel)
        {
            channels.Add(channel);
        }

        private List<ChannelInfo> channels = new List<ChannelInfo>();
    }
}