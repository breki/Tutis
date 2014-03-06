using System;
using System.Text;
using Brejc.Common;

namespace SamsungTvChannelsTool
{
    public class ChannelInfo
    {
        public ChannelInfo(byte[] rawData)
        {
            this.rawData = rawData;
        }

        public short ChannelNumber
        {
            get { return ReadInt16LittleEndian(0); }
            set { throw new NotImplementedException(); }
        }

        public string Name
        {
            get
            {
                return RemoveNulls(Encoding.BigEndianUnicode.GetString(rawData, 64, 100));
            }
        }

        public override string ToString()
        {
            return "{0}: '{1}'".Fmt(ChannelNumber, Name);
        }

        private short ReadInt16LittleEndian (int index)
        {
            byte b1 = rawData[index];
            byte b2 = rawData[index + 1];

            return (short)(b1 | b2 << 8);
        }

        private short ReadInt16BigEndian (int index)
        {
            byte b1 = rawData[index];
            byte b2 = rawData[index + 1];

            return (short)(b1 << 8 | b2);
        }

        private static string RemoveNulls (string s)
        {
            int realLength = s.IndexOf ('\0');
            return realLength > 0 ? s.Substring (0, realLength) : s;
        }

        private byte[] rawData;
    }
}