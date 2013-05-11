using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class StringTable
    {
        [ProtoMember (1, Name = "s")]
        public IList<byte[]> S
        {
            get { return s; }
            set { s = value; }
        }

        public string this [uint index]
        {
            get
            {
                return Encoding.UTF8.GetString(s[(int)index]);
            }
        }

        public string this[int index]
        {
            get
            {
                if (index >= s.Count)
                    throw new ArgumentOutOfRangeException("index");

                return Encoding.UTF8.GetString (s[index]);
            }
        }

        private IList<byte[]> s = new List<byte[]>();
    }
}